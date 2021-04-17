using System;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hellmade.Net
{
    [CustomEditor(typeof(EazyNetChecker))]
    public class EazyNetCheckerInspector : Editor
    {
        private EazyNetChecker instance;

        private Color defaultColor;
        private Color warningBackColor;

        private GUIStyle bannerStyle;
        private GUIStyle header1Style;
        private GUIStyle tab1Style;
        private GUIStyle tabSelectedStyle;
        private GUIStyle fullWidthButton1;
        private GUIStyle listItemTitleStyle;
        private GUIStyle groupBox1Style;
        private GUIStyle groupBox2Style;
        private GUIStyle paragraphLabel;
        private GUIStyle selectedIcon;

        private float windowScrollsHeight;
        private Vector2 settingsScrollPos;
        private Vector2 tagsScrollPos;
        private Vector2 sortingLayersScrollPos;
        private Vector2 layersScrollPos;
        private Vector2 scenesScrollPos;
        private Vector2 stringsScrollPos;

        private bool stylesSetup = false;
        private int methodToDeleteIndex = -1;
        private string newMethodID = "";
        private string methodEditedID = "";
        private int methodToEditIndex = -1;

        private Vector2 bannerPos;
        private float bannerWidth;
        private float bannerHeight;

        private void SetupStyles()
        {
            defaultColor = GUI.backgroundColor;
            warningBackColor = Color.yellow;

            // Banner
            bannerStyle = new GUIStyle(EditorStyles.label);

            // Full Width Button 1 
            fullWidthButton1 = new GUIStyle(GUI.skin.GetStyle("MiniButton"));
            fullWidthButton1.margin = new RectOffset(1, 1, 0, 0);

            // list item title
            listItemTitleStyle = new GUIStyle(GUI.skin.GetStyle("dockarea"));
            listItemTitleStyle.margin = new RectOffset(1, 1, 3, 1);
            listItemTitleStyle.padding = new RectOffset(0, 0, 0, 0);
            listItemTitleStyle.fixedHeight = 0;
            listItemTitleStyle.alignment = TextAnchor.MiddleLeft;

            // Header 1
            header1Style = new GUIStyle(GUI.skin.GetStyle("IN BigTitle"));
            header1Style.alignment = TextAnchor.MiddleLeft;
            header1Style.margin = new RectOffset(0, 0, 0, 0);
            header1Style.fontSize = 15;
            header1Style.fixedHeight = 30;
            header1Style.stretchWidth = true;
            header1Style.contentOffset = new Vector2(0, 0);
            //header1Style.normal.background = Resources.Load("Header1NetCheckerBackground_Hellmade") as Texture2D;
            //header1Style.normal.textColor = Color.white;

            // groupbox 1
            groupBox1Style = new GUIStyle(GUI.skin.GetStyle("GroupBox"));
            groupBox1Style.margin = new RectOffset(4, 4, 0, 0);

            // groupbox 2
            groupBox2Style = new GUIStyle(GUI.skin.GetStyle("GroupBox"));
            groupBox2Style.margin = new RectOffset(4, 4, 0, 0);
            groupBox2Style.padding = new RectOffset(0, 0, 0, 0);
            groupBox2Style.border = new RectOffset(5, 5, 5, 5);

            // paragraph Label
            paragraphLabel = new GUIStyle(GUI.skin.GetStyle("Label"));
            paragraphLabel.wordWrap = true;

            // selected icon
            //selectedIcon = new GUIStyle(GUI.skin.GetStyle("Icon.Activation"));
            //selectedIcon.margin = new RectOffset(2, 2, 2, 2);
            //selectedIcon.fixedWidth = 15;
            //selectedIcon.fixedHeight = 15;
        }

        [MenuItem("GameObject/Hellmade Games/Eazy NetChecker", false, 10)]
        private static void AddGameObject()
        {
            GameObject go = new GameObject("Eazy NetChecker");
            go.AddComponent<EazyNetChecker>();

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

            Selection.activeObject = go;
        }

        [MenuItem("GameObject/Hellmade Games/Eazy NetChecker", true)]
        private static bool AddGameObjectCheck()
        {
            return GameObject.FindObjectOfType<EazyNetChecker>() == null;
        }

        private void OnEnable()
        {
            instance = (EazyNetChecker)target;

            EazyNetChecker.Init();
        }

        public override void OnInspectorGUI()
        {
            if (!stylesSetup)
            {
                SetupStyles();
            }

            // BANNER
            GUILayout.Label("");
            if (Event.current.type == EventType.Repaint)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();
                bannerPos = new Vector2(0, lastRect.position.y);
                bannerWidth = lastRect.width;
                bannerHeight = (150f / 1250f) * bannerWidth;
            }
            GUI.DrawTexture(new Rect(bannerPos.x + 14, bannerPos.y, bannerWidth, bannerHeight), Resources.Load("eazyNetchecker-Header_Hellmade") as Texture2D, ScaleMode.StretchToFill, true, 10.0F);
            GUILayout.Space(bannerHeight - 18);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Website", "ButtonLeft"))
            {
                Application.OpenURL("http://www.hellmadegames.com");
            }
            if (GUILayout.Button("Documentation", "ButtonMid"))
            {
                Application.OpenURL("http://www.hellmadegames.com/Projects/eazy-netchecker/docs/manual/Manual.pdf");
            }
            if (GUILayout.Button("More Assets", "ButtonRight"))
            {
                Application.OpenURL("https://assetstore.unity.com/publishers/23684");
            }
            EditorGUILayout.EndHorizontal();

            // STATISTICS
            GUILayout.Space(10);
            GUILayout.Label(new GUIContent("Statistics", ""), header1Style);
            EditorGUILayout.BeginVertical(groupBox1Style);

            if (Application.isPlaying)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(new GUIContent("Connection Status", ""), EditorStyles.boldLabel);
                EditorGUILayout.LabelField(new GUIContent(EazyNetChecker.Status.ToString(), ""));
                EditorGUILayout.EndVertical();

                GUILayout.Space(5);

                EditorGUILayout.LabelField(new GUIContent("Checks", ""), EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Method", ""), GUILayout.Width(160));
                EditorGUILayout.LabelField(new GUIContent(EazyNetChecker.GetSelectedMethod().id, ""));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Status", ""), GUILayout.Width(160));
                EditorGUILayout.LabelField(new GUIContent(EazyNetChecker.IsChecking ? "Checking.." : EazyNetChecker.NextCheckRemaingSeconds == 0 ? "Idle" : "Next check in " + EazyNetChecker.NextCheckRemaingSeconds.ToString("f0"), ""));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Reachability type", ""), GUILayout.Width(160));
                EditorGUILayout.LabelField(new GUIContent(EazyNetChecker.ReachabilityType.ToString(), ""));
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5);

                EditorGUILayout.LabelField(new GUIContent("Timers", ""), EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Runtime", ""), GUILayout.Width(160));
                EditorGUILayout.LabelField(new GUIContent(EazyNetChecker.Runtime.ToString("f2"), ""));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Uptime", ""), GUILayout.Width(160));
                EditorGUILayout.LabelField(new GUIContent(EazyNetChecker.Uptime.ToString("f2"), ""));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Downtime", ""), GUILayout.Width(160));
                EditorGUILayout.LabelField(new GUIContent(EazyNetChecker.Downtime.ToString("f2"), ""));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("Statistics are only available on play mode.", MessageType.Warning);
            }

            EditorGUILayout.EndToggleGroup();

            // SETTINGS
            GUILayout.Space(10);
            GUILayout.Label(new GUIContent("Settings", ""), header1Style);
            EditorGUILayout.BeginVertical(groupBox1Style);

            GUILayout.Label(new GUIContent("Check Intervals", ""), EditorStyles.boldLabel);

            // Check interval Normal
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Normal", "How often an internet check will be performed (if a continuous check is started)"), GUILayout.Width(160));
            EazyNetChecker.CheckIntervalNormal = EditorGUILayout.FloatField(EazyNetChecker.CheckIntervalNormal, GUILayout.Width(50));
            EditorGUILayout.LabelField("Seconds");
            EditorGUILayout.EndHorizontal();

            // Check interval on no connection
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("On no connection", "How often an internet check will be performed (if a continuous check is started) when there is no connection"), GUILayout.Width(160));
            EazyNetChecker.CheckIntervalOnNoConnection = EditorGUILayout.FloatField(EazyNetChecker.CheckIntervalOnNoConnection, GUILayout.Width(50));
            EditorGUILayout.LabelField("Seconds");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Timeout
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Timeout", "The time after which an internet check will timeout"), GUILayout.Width(160));
            EazyNetChecker.Timeout = EditorGUILayout.FloatField(EazyNetChecker.Timeout, GUILayout.Width(50));
            EditorGUILayout.LabelField("Seconds");
            EditorGUILayout.EndHorizontal();

            // Continue check on timeout
            EditorGUILayout.BeginHorizontal();
            EazyNetChecker.ContinueCheckAfterTimeout = EditorGUILayout.Toggle(EazyNetChecker.ContinueCheckAfterTimeout, GUILayout.Width(10));
            EditorGUILayout.LabelField(new GUIContent("Continue check after timeout", "Whether to continue checking after a timeout (Only applicable when a continuous check is started.)"));
            EditorGUILayout.EndHorizontal();

            // Show Debug
            EditorGUILayout.BeginHorizontal();
            EazyNetChecker.ShowDebug = EditorGUILayout.Toggle(EazyNetChecker.ShowDebug, GUILayout.Width(10));
            EditorGUILayout.LabelField(new GUIContent("Show Debug", "Whether to show debug information during the operation of Eazy NetChecker."));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            // STANDARD METHODS
            GUILayout.Space(10);
            GUILayout.Label(new GUIContent("Standard Check Methods", ""), header1Style);
            List<NetCheckMethod> standardMethods = EazyNetChecker.GetStandardMethods();
            EditorGUILayout.BeginVertical(groupBox1Style);

            bool isSelected = EazyNetChecker.PlatformDefaultSelected;
            EditorGUILayout.BeginVertical(groupBox2Style);
            EditorGUILayout.BeginHorizontal();
            if (isSelected)
            {
                //GUILayout.Label(new GUIContent(""), selectedIcon);
            }
            EditorGUILayout.LabelField(new GUIContent("Platform Default method", "A method will be choosen automatically based on what platform the check is running on"), EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(new GUIContent("A method will be choosen automatically based on what platform the check is running on."), paragraphLabel);

            // Select
            GUILayout.Space(5);
            GUI.backgroundColor = isSelected ? Color.green : defaultColor;
            GUI.enabled = !isSelected;
            if (GUILayout.Button(isSelected ? "Selected" : "Select", fullWidthButton1))
            {
                EazyNetChecker.UseDefaultMethod();
            }
            GUI.backgroundColor = defaultColor;
            GUI.enabled = true;
            EditorGUILayout.EndVertical();

            // STANDARD METHODS
            GUILayout.Space(5);
            for (int i = 0; i < standardMethods.Count; i++)
            {
                isSelected = EazyNetChecker.GetSelectedMethod() != null && EazyNetChecker.GetSelectedMethod().id == standardMethods[i].id;
                EditorGUILayout.BeginVertical(groupBox2Style);

                // ID
                EditorGUILayout.BeginHorizontal();
                if (isSelected)
                {
                    //GUILayout.Label(new GUIContent(""), selectedIcon);
                }
                EditorGUILayout.LabelField(new GUIContent(standardMethods[i].id, "The ID of the method"), EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

                if (!standardMethods[i].link.ToLower().StartsWith("https"))
                {
                    GUI.backgroundColor = warningBackColor;
                    EditorGUILayout.HelpBox("This method does not use a secured link. Cleartext HTTP traffic is not permitted on Android 9+ and iPhone, therefore it may not work as expected for such devices. If you are targeting such devices, please use a secured link.", MessageType.Warning);
                    GUI.backgroundColor = defaultColor;
                }

                // Link
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Link:", "The link the method uses for internet check"), GUILayout.Width(30));
                EditorGUILayout.LabelField(new GUIContent(standardMethods[i].link, ""));
                EditorGUILayout.EndHorizontal();

                // Expected response
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Expected Response:", "The expected response from the HTTP request when target destination can succesfully be reached"), GUILayout.Width(120));
                if (standardMethods[i].responseType == NetCheckResponseType.HTTPStatusCode)
                {
                    EditorGUILayout.LabelField(new GUIContent("HTTP " + (int)standardMethods[i].expectedHttpStatusCode + " [" + standardMethods[i].expectedHttpStatusCode + "]", ""));
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.EndHorizontal();
                    GUI.enabled = false;
                    EditorGUILayout.TextArea(standardMethods[i].expectedContent, GUILayout.MaxHeight(60));
                    GUI.enabled = true;
                }

                // Select
                GUILayout.Space(5);
                GUI.backgroundColor = isSelected ? Color.green : defaultColor;
                GUI.enabled = !isSelected;
                if (GUILayout.Button(isSelected ? "Selected" : "Select", fullWidthButton1))
                {
                    EazyNetChecker.UseMethod(standardMethods[i]);
                }
                GUI.backgroundColor = defaultColor;
                GUI.enabled = true;
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);
            }
            EditorGUILayout.EndVertical();

            // CUSTOM METHODS
            GUILayout.Space(10);
            GUILayout.Label(new GUIContent("Custom Check Methods", ""), header1Style);
            List<NetCheckMethod> customMethods = EazyNetChecker.GetCustomMethods();
            EditorGUILayout.BeginVertical(groupBox1Style);

            // Add method
            EditorGUILayout.LabelField("Add new custom method");
            EditorGUILayout.BeginHorizontal();

            bool uniqueID = true;
            for (int i = 0; i < standardMethods.Count; i++)
            {
                if (newMethodID.Trim() == standardMethods[i].id)
                {
                    uniqueID = false;
                    break;
                }
            }

            for (int i = 0; i < customMethods.Count; i++)
            {
                if (!uniqueID || newMethodID.Trim() == customMethods[i].id)
                {
                    uniqueID = false;
                    break;
                }
            }

            newMethodID = EditorGUILayout.TextField(newMethodID);
            GUI.enabled = uniqueID && newMethodID.Trim() != "";
            if (GUILayout.Button("+", "MiniButtonRight", GUILayout.Width(30)))
            {
                EazyNetChecker.AddCustomMethod(new NetCheckMethod(newMethodID.Trim(), "", ""));
                newMethodID = "";
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            if (!uniqueID)
            {
                GUI.backgroundColor = warningBackColor;
                EditorGUILayout.HelpBox("ID of the new method must be unique.", MessageType.Warning);
                GUI.backgroundColor = defaultColor;
            }

            GUILayout.Space(10);
            for (int i = 0; i < customMethods.Count; i++)
            {
                isSelected = EazyNetChecker.GetSelectedMethod() != null && EazyNetChecker.GetSelectedMethod().id == customMethods[i].id;
                bool uniqueEditedID = true;
                EditorGUILayout.BeginVertical(groupBox2Style);

                // Options and ID
                EditorGUILayout.BeginHorizontal();
                if (isSelected)
                {
                    //GUILayout.Label(new GUIContent("", "Selected method"), selectedIcon);
                }
                if (GUILayout.Button("X", "MinibuttonLeft", GUILayout.Width(20)) && EditorUtility.DisplayDialog("Eazy NetChecker", "Are you sure you want to delete this method? Deleting is permanent and cannot be undone.", "Yes, delete", "No, keep it"))
                {
                    methodToDeleteIndex = i;
                }
                if (methodToEditIndex == i)
                {
                    GUI.backgroundColor = warningBackColor;
                    if (GUILayout.Button("Cancel", "MinibuttonMid", GUILayout.Width(60)))
                    {
                        methodToEditIndex = -1;
                        methodEditedID = "";

                    }

                    for (int j = 0; j < standardMethods.Count; j++)
                    {
                        if (methodEditedID.Trim() == standardMethods[j].id)
                        {
                            uniqueEditedID = false;
                            break;
                        }
                    }

                    for (int j = 0; j < customMethods.Count; j++)
                    {
                        if (j != methodToEditIndex && (!uniqueEditedID || methodEditedID.Trim() == customMethods[j].id))
                        {
                            uniqueEditedID = false;
                            break;
                        }
                    }

                    GUI.enabled = uniqueEditedID && methodEditedID.Trim() != "";
                    if (GUILayout.Button("Save", "MinibuttonRight", GUILayout.Width(60)))
                    {
                        customMethods[i].id = methodEditedID;
                        methodToEditIndex = -1;
                        methodEditedID = "";

                        if (isSelected)
                        {
                            EazyNetChecker.UseMethod(customMethods[i]);
                        }

                    }
                    GUI.enabled = true;
                    GUI.backgroundColor = defaultColor;
                }
                else
                {
                    if (GUILayout.Button("Rename", "MinibuttonRight", GUILayout.Width(60)))
                    {
                        methodToEditIndex = i;
                        methodEditedID = customMethods[i].id;
                    }
                }

                if (methodToEditIndex == i)
                {
                    methodEditedID = EditorGUILayout.TextField(methodEditedID);
                }
                else
                {
                    EditorGUILayout.LabelField(new GUIContent(customMethods[i].id, "The ID of the method"), EditorStyles.boldLabel);
                }
                EditorGUILayout.EndHorizontal();
                if (!uniqueEditedID)
                {
                    GUI.backgroundColor = warningBackColor;
                    EditorGUILayout.HelpBox("ID of the new method must be unique.", MessageType.Warning);
                    GUI.backgroundColor = defaultColor;
                }

                // Link
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Link:", "The link the method will use for internet check"), GUILayout.Width(30));
                customMethods[i].link = EditorGUILayout.TextField(customMethods[i].link);
                EditorGUILayout.EndHorizontal();

                Uri uri;
                bool isUrl = Uri.TryCreate(customMethods[i].link, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
                if (!isUrl)
                {
                    GUI.backgroundColor = warningBackColor;
                    EditorGUILayout.HelpBox("Link does not seem to be valid.", MessageType.Warning);
                    GUI.backgroundColor = defaultColor;
                }
                else if (uri.Scheme == Uri.UriSchemeHttp)
                {
                    GUI.backgroundColor = warningBackColor;
                    EditorGUILayout.HelpBox("Link does not seem to be secured. Cleartext HTTP traffic is not permitted on Android 9+ and iPhone, therefore it may not work as expected for such devices. If you are targeting such devices, please use a secured link.", MessageType.Warning);
                    GUI.backgroundColor = defaultColor;
                }

                // Response type
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                bool isResponseTypeSelected = customMethods[i].responseType == NetCheckResponseType.HTTPStatusCode;
                GUI.enabled = !isResponseTypeSelected;
                if (GUILayout.Button(isResponseTypeSelected ? "✔ HTTP Status Code" : "HTTP Status Code", "buttonLeft"))
                {
                    customMethods[i].responseType = NetCheckResponseType.HTTPStatusCode;
                }
                isResponseTypeSelected = customMethods[i].responseType == NetCheckResponseType.ResponseContent;
                GUI.enabled = !isResponseTypeSelected;
                if (GUILayout.Button(isResponseTypeSelected ? "✔ Response Content" : "Response Content", "buttonMid"))
                {
                    customMethods[i].responseType = NetCheckResponseType.ResponseContent;
                }
                isResponseTypeSelected = customMethods[i].responseType == NetCheckResponseType.ResponseContainContent;
                GUI.enabled = !isResponseTypeSelected;
                if (GUILayout.Button(isResponseTypeSelected ? "✔ Response Contain Content" : "Response Contain Content", "buttonRight"))
                {
                    customMethods[i].responseType = NetCheckResponseType.ResponseContainContent;
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                // Expected repsonse
                GUILayout.Space(5);
                if (customMethods[i].responseType == NetCheckResponseType.HTTPStatusCode)
                {
                    EditorGUILayout.LabelField(new GUIContent("The HTTP status code is used as a response check."), paragraphLabel);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Expected Response:", "The expected response from the HTTP request when target destination can succesfully be reached"), GUILayout.Width(120));
                    customMethods[i].expectedHttpStatusCode = (HttpStatusCode)EditorGUILayout.EnumPopup(customMethods[i].expectedHttpStatusCode);
                    EditorGUILayout.EndHorizontal();
                }
                else if (customMethods[i].responseType == NetCheckResponseType.ResponseContent)
                {
                    EditorGUILayout.LabelField(new GUIContent("The response content is used as a response check. The whole content needs to match. Check is done on trimmed content, therefore whitespaces, newlines and tabs are ignored."), paragraphLabel);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Expected Response:", ""), GUILayout.Width(120));
                    EditorGUILayout.EndHorizontal();
                    customMethods[i].expectedContent = EditorGUILayout.TextArea(customMethods[i].expectedContent, GUILayout.MaxHeight(60));
                }
                else
                {
                    EditorGUILayout.LabelField(new GUIContent("The response content is used as a response check. The response content just needs to contain the expected text. Check is done on trimmed content, therefore whitespaces, newlines and tabs are ignored."), paragraphLabel);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Expected Response:", ""), GUILayout.Width(120));
                    EditorGUILayout.EndHorizontal();
                    customMethods[i].expectedContent = EditorGUILayout.TextArea(customMethods[i].expectedContent, GUILayout.MaxHeight(60));
                }

                // Select
                GUILayout.Space(5);
                GUI.backgroundColor = isSelected ? Color.green : defaultColor;
                GUI.enabled = !isSelected;
                if (GUILayout.Button(isSelected ? "Selected" : "Select", fullWidthButton1))
                {
                    EazyNetChecker.UseMethod(customMethods[i]);
                }
                GUI.backgroundColor = defaultColor;
                GUI.enabled = true;
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);
            }

            if (methodToDeleteIndex != -1)
            {
                customMethods.RemoveAt(methodToDeleteIndex);
                methodToDeleteIndex = -1;
                if (isSelected)
                {
                    EazyNetChecker.UseDefaultMethod();
                }
            }

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(instance);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            Repaint();
        }
    }
}