using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using marijnz.EditorCoroutines;
using MoPubInternal.ThirdParty.MiniJSON;
using UnityEditor;
using UnityEngine;

public class MoPubSDKManager : EditorWindow
{
    private const string downloadDir = "Assets/MoPub";
    private const string manifestURL = "https://mopub-mediation.firebaseio.com/.json";
    private const string stagingURL  = "https://mopub-mediation-staging.firebaseio.com/.json";
    private const string migrateNote = "A legacy directory structure of MoPub was found in your project.\n" +
                                       "Pressing 'Migrate' will move any files in the legacy locations into the new " +
                                       "location, so all MoPub-related files are under Assets/MoPub/, and will also " +
                                       "remove any redundant MoPub files.\nPlease open the link below for details.\n" +
                                       "BE AWARE THAT THE MIGRATION IS NOT REVERSIBLE.";
    private const string migrateLink = "https://developers.mopub.com/docs/unity/getting-started/#migrating-to-54";
    private const string mediationNote = "A legacy directory of MoPub Mediation was found in your project.\n" +
                                         "If you wish to mediate other networks, please delete these\n" +
                                         "directories and all of their contents:\n\n" +
                                         "    Assets/Plugins/Android/mopub-support\n" +
                                         "    Assets/Plugins/iOS/MoPub-Mediation-Adapters\n" +
                                         "    Assets/MoPub/Plugins/iOS/MoPubSDKFramework.framework\n" +
                                         "\nPlease open the link below for details.";
    private const string mediationLink = "https://developers.mopub.com/docs/unity/getting-started/#integrate-the-plugin-and-adapters";
    private const string helpLink    = "https://developers.mopub.com/docs/unity/getting-started/#integrate-the-plugin-and-adapters";

    private struct SdkInfo
    {
        public string Name;
        public string Key;
        public string Url;
        public string LatestVersion;
        public string CurrentVersion;
        public string Filename;
        public string Instructions;

        public Dictionary<PackageConfig.Platform, string> NetworkVersions;  // Versions of the Android/iOS network SDKs

        public bool FromJson(string name, Dictionary<string, object> dict)
        {
            if (string.IsNullOrEmpty(name) || dict == null)
                return false;

            // This is the "branded" name of the SDK, which is displayed in the dialog.
            // This can change from time to time when networks rebrand their SDKs.
            Name = name;

            object obj;
            dict.TryGetValue("keyname", out obj);
            // This is our internal keyname for the SDK, which will not change, even if the branded name does.
            // The PackageConfig.Name property is matched against this.
            Key = obj as string ?? name;

            dict.TryGetValue("Unity", out obj);
            var unityDict = obj as Dictionary<string, object>;
            if (unityDict == null)
                return false;

            if (unityDict.TryGetValue("packageUrl", out obj) || unityDict.TryGetValue("download_url", out obj))
                Url = obj as string;
            if (unityDict.TryGetValue("packageVersion", out obj) || unityDict.TryGetValue("version", out obj))
                LatestVersion = obj as string;
            if (unityDict.TryGetValue("filename", out obj))
                Filename = obj as string;
            else if (!string.IsNullOrEmpty(Url)) {
                var uri = new Uri(Url);
                var path = uri.IsAbsoluteUri ? uri.AbsolutePath : uri.LocalPath;
                Filename = Path.GetFileName(path);
            }
            if (unityDict.TryGetValue("installGuide", out obj))
                Instructions = obj as string;

            // Get the versions of the Android/iOS network SDKs
            NetworkVersions = new Dictionary<PackageConfig.Platform, string>();
            dict.TryGetValue("Android", out obj);
            var versionDict = obj as Dictionary<string, object>;
            if (versionDict != null) {
                versionDict.TryGetValue("version", out obj);
                versionDict = obj as Dictionary<string, object>;
                if (versionDict != null && versionDict.TryGetValue("sdk", out obj))
                    NetworkVersions[PackageConfig.Platform.ANDROID] = obj as string;
            }
            dict.TryGetValue("iOS", out obj);
            versionDict = obj as Dictionary<string, object>;
            if (versionDict != null) {
                versionDict.TryGetValue("version", out obj);
                versionDict = obj as Dictionary<string, object>;
                if (versionDict != null && versionDict.TryGetValue("sdk", out obj))
                    NetworkVersions[PackageConfig.Platform.IOS] = obj as string;
            }

            return true;
        }

        public bool FromConfig(PackageConfig config)
        {
            if (config == null || string.IsNullOrEmpty(config.Name) || !string.IsNullOrEmpty(Key) && Key != config.Name)
                return false;
            if (string.IsNullOrEmpty(Name))
                Name = config.Name;
            if (string.IsNullOrEmpty(Key))
                Key = config.Name;
            CurrentVersion = config.Version;
            if (NetworkVersions == null)
                NetworkVersions = new Dictionary<PackageConfig.Platform, string>();
            foreach (var platform in config.NetworkSdkVersions.Keys)
                if (!NetworkVersions.ContainsKey(platform))
                    NetworkVersions[platform] = config.NetworkSdkVersions[platform];
            return true;
        }
    }

    // Version and download info for the SDK and network mediation adapters.
    private static SdkInfo mopubSdkInfo;
    private static readonly SortedDictionary<string, SdkInfo> sdkInfo = new SortedDictionary<string, SdkInfo>();

    // Async download operations tracked here.
    private EditorCoroutines.EditorCoroutine coroutine;
    private WebClient downloader;
    private float progress;
    private string activity;
    private bool legacyMoPub;
    private bool legacyMediation;

    // For overriding button enable/disable states, etc.
    private bool staging = false;
    private bool testing = false;

    // Custom style for LabelFields.  (Don't make static or the dialog doesn't recover well from Unity compiling
    // a new or changed editor script.)
    private GUIStyle labelStyle;
    private GUIStyle labelStyleArea;
    private GUIStyle labelStyleLink;
    private GUIStyle headerStyle;
    private readonly GUILayoutOption fieldWidth = GUILayout.Width(60);

    private Vector2 scrollPos;


    public static void ShowSDKManager(bool stage)
    {
        var win = GetWindow<MoPubSDKManager>(true);
        bool change = stage != win.staging;
        win.staging = stage;
        win.titleContent = new GUIContent(stage ? "MoPub SDK Manager (Staging)" : "MoPub SDK Manager");
        win.Focus();
        if (change) {
            win.CancelOperation();
            win.OnEnable();
        }
    }


    void Awake()
    {
        labelStyle = new GUIStyle(EditorStyles.label) {
            fontSize = 14,
            fontStyle = FontStyle.Bold
        };
        labelStyleArea = new GUIStyle(EditorStyles.label) {
            wordWrap = true
        };
        labelStyleLink = new GUIStyle(EditorStyles.label) {
            normal = { textColor = Color.blue },
            active = { textColor = Color.white },
        };
        headerStyle = new GUIStyle(EditorStyles.label) {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            fixedHeight = 18
        };
        CancelOperation();
    }


    void OnEnable()
    {
        legacyMoPub = MoPubUpgradeMigration.LegacyMoPubPresent();
        legacyMediation = MoPubUpgradeMigration.LegacyMediationPresent();
        coroutine = this.StartCoroutine(GetSDKVersions());
    }


    void OnDisable()
    {
        CancelOperation();
    }


    private IEnumerator GetSDKVersions()
    {
        // Wait one frame so that we don't try to show the progress bar in the middle of OnGUI().
        yield return null;

        activity = "Downloading SDK version manifest...";
        var www = new WWW(staging ? stagingURL : manifestURL);
        yield return www;

        var json = www.text;
        if (string.IsNullOrEmpty(json)) {
            json = "{}";
            Debug.LogError("Unable to retrieve SDK version manifest");
        }

        // Got the file.  Now extract info on latest SDKs available.
        mopubSdkInfo = new SdkInfo();
        sdkInfo.Clear();
        var dict = Json.Deserialize(json) as Dictionary<string,object>;
        if (dict != null) {
            object obj;
            if (dict.TryGetValue("mopubBaseConfig", out obj)) {
                mopubSdkInfo.FromJson("Unity SDK", obj as Dictionary<string, object>);
                mopubSdkInfo.CurrentVersion = MoPub.MoPubSdkVersion;
            }
            if (dict.TryGetValue("releaseInfo", out obj))
                foreach (var item in obj as Dictionary<string, object>) {
                    var info = new SdkInfo();
                    if (info.FromJson(item.Key, item.Value as Dictionary<string, object>))
                        sdkInfo[info.Key] = info;
                }
        }

        // Figure out what versions of SDKs are currently installed.
        var baseType = typeof(PackageConfig);
        var configs = from t in Assembly.GetExecutingAssembly().GetTypes()
                      where t.IsSubclassOf(baseType) && !t.IsAbstract
                      select Activator.CreateInstance(t) as PackageConfig;
        foreach (var config in configs) {
            SdkInfo info;
            sdkInfo.TryGetValue(config.Name, out info);
            if (info.FromConfig(config))
                sdkInfo[info.Key] = info;
        }

        // Clear up the async-job state.
        coroutine = null;
        Repaint();
    }


    void OnGUI()
    {
        // Is any async job in progress?
        var stillWorking = coroutine != null || downloader != null;

        GUILayout.Space(5);
        EditorGUILayout.LabelField("MoPub SDKs", labelStyle, GUILayout.Height(20));

        using (new EditorGUILayout.VerticalScope("box")) {
            SdkHeaders();
            var migratable = false;
            SdkRow(mopubSdkInfo, canInstall => {
                // Migration does not take precedence over installation/upgrade.
                migratable = !canInstall && (legacyMoPub || testing);
                if (migratable) {
                    GUI.enabled = !stillWorking;
                    if (GUILayout.Button(new GUIContent("Migrate"), fieldWidth))
                        LegacyCleanup();
                    GUI.enabled = true;
                }
                return migratable;
            });
            if (migratable || testing) {
                GUILayout.Space(6);
                EditorGUILayout.LabelField(migrateNote, labelStyleArea, GUILayout.Height(70));
                if (GUILayout.Button(migrateLink, labelStyleLink))
                    Application.OpenURL(migrateLink);
            }
        }

        if (sdkInfo.Count > 0) {
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Network SDKs", labelStyle, GUILayout.Height(20));
            using (new EditorGUILayout.VerticalScope("box"))
            using (var s = new EditorGUILayout.ScrollViewScope(scrollPos, false, false)) {
                scrollPos = s.scrollPosition;
                if (legacyMediation) {
                    // The Google Jar Resolver puts .jars into a different location from our legacy directory,
                    // so this will prevent duplicate jars ending up in two places causing weird build errors.
                    EditorGUILayout.LabelField(mediationNote, labelStyleArea, GUILayout.Height(120));
                    if (GUILayout.Button(mediationLink, labelStyleLink))
                        Application.OpenURL(mediationLink);
                    GUILayout.Space(6);
                } else {
                    SdkHeaders();
                    foreach (var item in sdkInfo)
                        SdkRow(item.Value);
                }
            }
        }

        // Indicate async operation in progress.
        using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
            EditorGUILayout.LabelField(stillWorking ? activity : " ");
        if (stillWorking && progress > 0 &&
                EditorUtility.DisplayCancelableProgressBar("MoPub SDK Manager", activity, progress) &&
                Event.current.rawType == EventType.Repaint)  // OnGUI gets called several times per frame...
            CancelOperation();

        using (new EditorGUILayout.HorizontalScope()) {
            GUILayout.Space(10);
            if (!stillWorking) {
                if (GUILayout.Button("Done", fieldWidth))
                    Close();
            } else {
                if (GUILayout.Button("Cancel", fieldWidth))
                    CancelOperation();
            }
            if (GUILayout.Button("Help", fieldWidth))
                Application.OpenURL(helpLink);
        }

#if mopub_developer
        GUILayout.Space(10);
        using (new EditorGUILayout.HorizontalScope()) {
            GUILayout.Space(10);
            testing = EditorGUILayout.ToggleLeft("Testing Mode", testing);
        }
#endif
    }


    private void SdkHeaders()
    {
        using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false))) {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Package", headerStyle);
            GUILayout.Button("Version", headerStyle);
            GUILayout.Space(3);
            GUILayout.Button("Action", headerStyle, fieldWidth);
            GUILayout.Button(" ", headerStyle, GUILayout.Width(1));
            GUILayout.Space(5);
        }
        GUILayout.Space(4);
    }


    private void SdkRow(SdkInfo info, Func<bool,bool> customButton = null)
    {
        var lat = info.LatestVersion;
        var cur = info.CurrentVersion;
        var isInst = !string.IsNullOrEmpty(cur);
        var canInst = !string.IsNullOrEmpty(lat) && (!isInst || MoPubUtils.CompareVersions(cur, lat) < 0);
        // Is any async job in progress?
        var stillWorking = coroutine != null || downloader != null;

        string tooltip = string.Empty;
        if (isInst && (MoPubUtils.CompareVersions(cur, lat) != 0 || testing))
            tooltip += "\n  Installed:  " + cur;
        if (info.NetworkVersions != null) {
            string version;
            if (info.NetworkVersions.TryGetValue(PackageConfig.Platform.ANDROID, out version))
                tooltip += "\n  Android SDK:  " + version;
            if (info.NetworkVersions.TryGetValue(PackageConfig.Platform.IOS, out version))
                tooltip += "\n  iOS SDK:  " + version;
        }
        if (!string.IsNullOrEmpty(tooltip))
            tooltip = info.Name + "\n  Package:  " + (lat ?? "n/a") + tooltip;

        GUILayout.Space(4);
        using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false))) {
            GUILayout.Space(10);
            EditorGUILayout.LabelField(new GUIContent { text = info.Name, tooltip = tooltip });
            GUILayout.Button(new GUIContent {
                text = lat ?? "--",
                tooltip = tooltip
            }, canInst ? EditorStyles.boldLabel : EditorStyles.label);
            GUILayout.Space(3);
            if (customButton == null || !customButton(canInst)) {
                GUI.enabled = !stillWorking && (canInst || testing);
                if (GUILayout.Button(new GUIContent {
                        text = isInst ? "Upgrade" : "Install",
                        tooltip = tooltip
                    }, fieldWidth))
                    this.StartCoroutine(DownloadSDK(info));
                GUI.enabled = true;
            }

            if (!string.IsNullOrEmpty(info.Instructions) && (info.Instructions != helpLink || testing)) {
                if (GUILayout.Button("?", GUILayout.ExpandWidth(false)))
                    Application.OpenURL(info.Instructions);
            } else
                // Need to fill space so that the Install/Upgrade buttons all line up nicely.
                GUILayout.Button(" ", EditorStyles.label, GUILayout.Width(17));
            GUILayout.Space(5);
        }
        GUILayout.Space(4);
    }


    private void CancelOperation()
    {
        // Stop any async action taking place.

        if (downloader != null) {
            downloader.CancelAsync(); // The coroutine should resume and clean up.
            return;
        }

        if (coroutine != null)
            this.StopCoroutine(coroutine.routine);
        if (progress > 0)
            EditorUtility.ClearProgressBar();
        coroutine = null;
        downloader = null;
        progress = 0;
    }


    private void LegacyCleanup()
    {
        MoPubUpgradeMigration.DoSDKMigration();
        legacyMoPub = MoPubUpgradeMigration.LegacyMoPubPresent();
    }


    private IEnumerator DownloadSDK(SdkInfo info)
    {
        // Wait one frame so that we don't try to show the progress bar in the middle of OnGUI().
        yield return null;

        // Track download progress (updated by event callbacks below).
        bool ended = false;
        bool cancelled = false;
        Exception error = null;
        int oldPercentage = 0, newPercentage = 0;

        var path = Path.Combine(downloadDir, info.Filename);

        Debug.LogFormat("Downloading {0} to {1}", info.Url, path);
        activity = string.Format("Downloading {0}...", info.Filename);
        progress = 0.01f;  // Set > 0 in order to show progress bar.

        // Hook the certificate-fixer callback to make TLS1.0 work (on some sites, anyway).
        ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;

        // Start the async download job.
        downloader = new WebClient();
        downloader.Encoding = Encoding.UTF8;
        downloader.DownloadProgressChanged += (sender, args) => { newPercentage = args.ProgressPercentage; };
        downloader.DownloadFileCompleted += (sender, args) => { ended = true; cancelled = args.Cancelled; error = args.Error; };
        downloader.DownloadFileAsync(new Uri(info.Url), path);

        // Pause until download done/cancelled/fails, keeping progress bar up to date.
        while (!ended) {
            Repaint();
            yield return new WaitUntil(() => ended || newPercentage > oldPercentage);
            oldPercentage = newPercentage;
            progress = oldPercentage / 100.0f;
        }
        if (error != null) {
            Debug.LogError(error);
            cancelled = true;
        }

        // Reset async state so the GUI is operational again.
        downloader = null;
        coroutine = null;
        progress = 0;
        EditorUtility.ClearProgressBar();

        if (!cancelled)
            AssetDatabase.ImportPackage(path, true);  // OK, got the file, so let the user import it if they want.
        else
            Debug.Log("Download terminated.");
    }


    // Found the following workaround for TLS negotiation issues here:
    //   https://forum.unity.com/threads/how-to-properly-download-and-save-big-size-file.455384/#post-2975169
    // This occurs because of Unity's old (when using .NET < 4.5) TLS implementation which is no longer compatible
    // with some sites' download security policies.
    // This only seems to help in some cases (e.g. Firebase storage) but not others (e.g. Github).
    private static bool RemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate,
                                                            X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors != SslPolicyErrors.None &&
              chain.ChainStatus.Any(s => s.Status != X509ChainStatusFlags.RevocationStatusUnknown)) {
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
            return chain.Build((X509Certificate2)certificate);
        }
        return true;
    }
}
