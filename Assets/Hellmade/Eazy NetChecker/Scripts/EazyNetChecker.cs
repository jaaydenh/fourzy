using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Hellmade.Net
{
    /// <summary>
    /// Enum representing the response type the internet check will use to determine internet status
    /// </summary>
    public enum NetCheckResponseType
    {
        /// <summary>
        /// Check is performed using the response HTTP status code
        /// </summary>
        HTTPStatusCode,
        /// <summary>
        /// Check is performed using the response content
        /// </summary>
        ResponseContent,
        /// <summary>
        /// Check is performed using part of the response content
        /// </summary>
        ResponseContainContent
    }

    /// <summary>
    /// Enum representing the internet connection status
    /// </summary>
    public enum NetStatus
    {
        /// <summary>
        /// A network check has not being performed yet, or it is currently in progress for the first time
        /// </summary>
        PendingCheck,
        /// <summary>
        /// No connection could be established to a valid DNS destination
        /// </summary>
        NoDNSConnection,
        /// <summary>
        /// General network connection was established, but target destination could not be reached due to restricted internet access.
        /// </summary>
        WalledGarden,
        /// <summary>
        /// Network connection was established succesfully
        /// </summary>
        Connected
    }

    /// <summary>
    /// Static class responsible of checking if internet connection can be established.It handles all the checks and stores all standard and custom check methods.
    /// </summary>
    [HelpURL("http://www.hellmadegames.com/Projects/eazy-netchecker/docs/manual/Manual.pdf")]
    [DisallowMultipleComponent]
    public class EazyNetChecker : MonoBehaviour
    {
        public delegate void Event();
        /// <summary>
        /// Event raised when a check is started
        /// </summary>
        public static event Event OnCheckStarted;
        /// <summary>
        /// Event raised when a check just finished
        /// </summary>
        public static event Event OnCheckFinished;
        /// <summary>
        /// Event raised when the connection status has changed
        /// </summary>
        public static event Event OnConnectionStatusChanged;
        /// <summary>
        /// Event raised when a check times out
        /// </summary>
        public static event Event OnCheckTimeout;

        /// <summary>
        /// How often an internet check will be performed (if a continuous check is started). The minimum is 3 seconds. If less than the minimum is given, it will automatically set on the minimum.
        /// </summary>
        public static float CheckInterval
        {
            get { return Instance.checkInterval; }
            set
            {
                Instance.checkInterval = Mathf.Max(3, value);
            }
        }

        /// <summary>
        /// The time after which an internet check will timeout
        /// </summary>
        public static float Timeout { get { return Instance.timeout; } set { Instance.timeout = value; } }
        /// <summary>
        /// Whether to show debug information during the operation of Eazy NetChecker.
        /// </summary>
        public static bool ShowDebug { get { return Instance.showDebug; } set { Instance.showDebug = value; } }
        /// <summary>
        /// Whether the option to always choose the paltform default check method is enabled.
        /// </summary>
        public static bool PlatformDefaultSelected { get { return Instance.platformDefaultSelected; } }

        /// <summary>
        /// The current detected status of the internet connection
        /// </summary>
        public static NetStatus Status { get; private set; }

        /// <summary>
        /// The type of internet reachability.
        /// </summary>
        public static NetworkReachability ReachabilityType { get { return Application.internetReachability; } }

        /// <summary>
        /// Whether it is currently checking the internet connection status
        /// </summary>
        public static bool IsChecking { get; private set; }

        /// <summary>
        /// The number of seconds remaining until the next check. Returns 0 if no check is planned.
        /// </summary>
        public static float NextCheckRemaingSeconds { get { return IsChecking || !keepChecking ? 0 : Mathf.Max(0, nextCheckTime - Time.time); } }

        /// <summary>
        /// The total time in seconds that Eazy NetChecker has been running
        /// </summary>
        public static float Runtime { get; private set; }

        /// <summary>
        /// The total time in seconds that Eazy NetChecker has determined there was an established connection
        /// </summary>
        public static float Uptime { get; private set; }

        /// <summary>
        /// The total time in seconds that Eazy NetChecker could not establish a connection
        /// </summary>
        public static float Downtime { get; private set; }

        private static EazyNetChecker instance;
        public static EazyNetChecker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<EazyNetChecker>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = "Eazy NetChecker";
                        instance = obj.AddComponent<EazyNetChecker>();
                        Init();
                    }
                }
                return instance;
            }
        }

        [SerializeField]
        private List<NetCheckMethod> methods;
        [SerializeField]
        private List<NetCheckMethod> customMethods;
        [SerializeField]
        private bool initialized = false;
        [SerializeField]
        private NetCheckMethod selectedMethod = null;
        [SerializeField]
        public bool platformDefaultSelected = true;

        private static Coroutine checkerCoroutine = null;
        private static bool keepChecking = false;
        private static float checkStartedTime;
        private static float nextCheckTime = 0;
        private static bool stopOnSuccess = false;

        [SerializeField]
        private float timeout = 10f;
        [SerializeField]
        private float checkInterval = 10;
        [SerializeField]
        private bool showDebug = true;

        public virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as EazyNetChecker;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        private void Update()
        {
            // Update runtime, uptime and downtime
            Runtime += Time.deltaTime;
            if(Status == NetStatus.Connected)
            {
                Uptime += Time.deltaTime;
            }
            else if(Status != NetStatus.PendingCheck)
            {
                Downtime += Time.deltaTime;
            }

            // check if netCheck has timed out
            if (IsChecking && Time.time > checkStartedTime + timeout)
            {
                StopConnectionCheck();

                // Raise OnCheckTimeout event
                if (OnCheckTimeout != null)
                {
                    OnCheckTimeout();
                }

                Status = NetStatus.NoDNSConnection;

                if (ShowDebug)
                {
                    Debug.Log("[Eazy NetChecker] Internet check timed out.");
                }
            }
        }

        /// <summary>
        /// Initializes NetChecker. Call this only if you want to reinitialize. First initialization is done automatically.
        /// </summary>
        public static void Init()
        {
            SetStandardCheckMethods();

            if (Instance.initialized)
            { 
                return;
            }

            Instance.customMethods = new List<NetCheckMethod>();
            Instance.initialized = true;

            if (ShowDebug)
            {
                Debug.Log("[Eazy NetChecker] Initialization successful.");
            }
        }

        /// <summary>
        /// Validates and sets the standard check methods. You do not need to call this function. It is only used by the editor.
        /// </summary>
        public static void SetStandardCheckMethods()
        {
            if(Instance.methods == null)
            {
                Instance.methods = new List<NetCheckMethod>();
            }

            NetCheckMethod checkMethod;

            // Add standard check methods
            // Google204
            checkMethod = new NetCheckMethod("google204", "https://clients3.google.com/generate_204", HttpStatusCode.NoContent);
            bool use = false;
            if (Instance.methods.Count >= 1 && !Instance.methods[0].Equals(checkMethod))
            {
                use = GetSelectedMethod().Equals(Instance.methods[0]);
                Instance.methods[0] = checkMethod;

                if(use)
                {
                    UseGoogle204Method();
                }
            }
            else if(Instance.methods.Count < 1)
            {
                Instance.methods.Add(checkMethod);
            }

            // Microsoft Connect test
            checkMethod = new NetCheckMethod("msftconnecttest", "http://www.msftconnecttest.com/connecttest.txt", "Microsoft Connect Test");
            if (Instance.methods.Count >= 2 && !Instance.methods[1].Equals(checkMethod))
            {
                use = GetSelectedMethod().Equals(Instance.methods[1]);
                Instance.methods[1] = checkMethod;

                if (use)
                {
                    UseMicrosoftConnectTestMethod();
                }
            }
            else if(Instance.methods.Count < 2)
            {
                Instance.methods.Add(checkMethod);
            }

            // Apple Hotspot
            checkMethod = new NetCheckMethod("applehotspot", "https://captive.apple.com/hotspot-detect.html", "<HTML><HEAD><TITLE>Success</TITLE></HEAD><BODY>Success</BODY></HTML>");
            if (Instance.methods.Count >= 3 && !Instance.methods[2].Equals(checkMethod))
            {
                use = GetSelectedMethod().Equals(Instance.methods[2]);
                Instance.methods[2] = checkMethod;

                if (use)
                {
                    UseAppleHotspotMethod();
                }
            }
            else if (Instance.methods.Count < 3)
            { 
                Instance.methods.Add(checkMethod);
            }
        }

        /// <summary>
        /// Returns a list with the all the predefined standard check methods
        /// </summary>
        /// <returns></returns>
        public static List<NetCheckMethod> GetStandardMethods()
        {
            return Instance.methods;
        }

        /// <summary>
        /// Returns the Google204 method
        /// </summary>
        /// <returns></returns>
        public static NetCheckMethod GetGoogle204Method()
        {
            return Instance.methods[0];
        }

        /// <summary>
        /// Returns the Microsoft Connect Test method. Use this only on windows, as it may be blocked on other devices.
        /// </summary>
        /// <returns></returns>
        public static NetCheckMethod GetMicrosoftConnectTestMethod()
        {
            return Instance.methods[1];
        }

        /// <summary>
        /// Returns the Apple Hotspot method
        /// </summary>
        /// <returns></returns>
        public static NetCheckMethod GetAppleHotspotMethod()
        {
            return Instance.methods[2];
        }

        /// <summary>
        /// Returns a list with all the added custom methods.
        /// </summary>
        /// <returns></returns>
        public static List<NetCheckMethod> GetCustomMethods()
        {
            return Instance.customMethods;
        }

        /// <summary>
        /// Returns the specified custom method
        /// </summary>
        /// <param name="methodID">The ID of the custom method to be retrieved</param>
        /// <returns></returns>
        public static NetCheckMethod GetCustomMethod(string methodID)
        {
            for (int i = 0; i < Instance.customMethods.Count; i++)
            {
                if (Instance.customMethods[i].id == methodID)
                {
                    return Instance.customMethods[i];
                }
            }

            Debug.LogError("[Eazy NetChecker] The method " + methodID + " could not be found. Null will be returned.", Instance.transform);

            return null;
        }

        /// <summary>
        /// Returns the default method. The default method is selected based on which platform it is running on.
        /// </summary>
        /// <returns></returns>
        public static NetCheckMethod GetDefaultMethod()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                case RuntimePlatform.WebGLPlayer:
                default:
                    return GetGoogle204Method();
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.XboxOne:
                    return GetMicrosoftConnectTestMethod();
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return GetAppleHotspotMethod();
            }
        }

        /// <summary>
        /// Returns the current selected method.
        /// </summary>
        /// <returns></returns>
        public static NetCheckMethod GetSelectedMethod()
        {
            if (PlatformDefaultSelected)
            {
                UseDefaultMethod();
            }
            else if (Instance.selectedMethod == null)
            {
                UseDefaultMethod();
                Debug.LogWarning("[Eazy NetChecker] There is no selected method. The platform default will be used.", Instance.transform);
            }

            return Instance.selectedMethod;
        }

        /// <summary>
        /// Adds a new custom method
        /// </summary>
        /// <param name="method">The method to add</param>
        public static void AddCustomMethod(NetCheckMethod method)
        {
            AddCustomMethod(method, false);
        }

        /// <summary>
        /// Adds a new custom method
        /// </summary>
        /// <param name="method">The method to add</param>
        /// <param name="use">Whether to automatically mark the new method as the selected one.</param>
        public static void AddCustomMethod(NetCheckMethod method, bool use)
        {
            if (method == null)
            {
                Debug.LogError("[Eazy NetChecker] The method you are trying to add is null.", Instance.transform);
                return;
            }

            Instance.customMethods.Add(method);

            if (Instance.showDebug)
            {
                Debug.Log("[Eazy NetChecker] Method " + method.id + " added.");
            }
        }

        /// <summary>
        /// Selects the specified method for internet connection checks.
        /// </summary>
        /// <param name="methodID">The method to be selected</param>
        public static void UseMethod(NetCheckMethod method)
        {
            if (method == null)
            {
                Debug.LogError("[Eazy NetChecker] The method you are trying to use is null.", Instance.transform);
                return;
            }

            Instance.selectedMethod = method;
            Instance.platformDefaultSelected = false;

            if (Instance.showDebug && Application.isPlaying)
            {
                Debug.Log("[Eazy NetChecker] Method " + method.id + " selected.");
            }
        }

        /// <summary>
        /// Selects the default method which is selected based on which platform it is running on.
        /// </summary>
        public static void UseDefaultMethod()
        {
            if (!Application.isPlaying)
            {
                Instance.platformDefaultSelected = true;
                Instance.selectedMethod = null;
                return;
            }

            Instance.selectedMethod = GetDefaultMethod();
        }

        /// <summary>
        /// Selects the Google204 method for internet connection checks.
        /// </summary>
        public static void UseGoogle204Method()
        {
            UseMethod(Instance.methods[0]);
        }

        /// <summary>
        /// Selects the Microsoft Connect Test method for internet connection checks.
        /// </summary>
        public static void UseMicrosoftConnectTestMethod()
        {
            UseMethod(Instance.methods[1]);
        }

        /// <summary>
        /// Selects the Apple Hotspot method for internet connection checks.
        /// </summary>
        public static void UseAppleHotspotMethod()
        {
            UseMethod(Instance.methods[2]);
        }

        /// <summary>
        /// Starts an iterated internet connection check on the specified interval. It uses the selected method. If no check method is selected, it uses the default one.
        /// </summary>
        public static void StartConnectionCheck()
        {
            StartConnectionCheck(false, false);
        }

        /// <summary>
        /// Starts an iterated internet connection check on the specified interval. It uses the selected method. If no check method is selected, it uses the default one.
        /// </summary>
        /// <param name="stopOnSuccess">Whether to automatically stop checking the internet connection as soon as a check is successful (Internet ins connected)</param>
        public static void StartConnectionCheck(bool stopOnSuccess, bool interruptActiveChecks)
        {
            if (interruptActiveChecks)
            {
                StopConnectionCheck();
            }

            keepChecking = true;
            EazyNetChecker.stopOnSuccess = stopOnSuccess;
            CheckConnection();
        }

        /// <summary>
        /// Stops all current connections checks
        /// </summary>
        public static void StopConnectionCheck()
        {
            IsChecking = false;
            keepChecking = false;
            Instance.StopAllCoroutines();
            checkerCoroutine = null;
        }

        /// <summary>
        /// Checks and determines the current internet connection status only once. It uses the selected method. If no check method is selected, it uses the default one.
        /// </summary>
        public static void CheckConnection()
        {
            if (checkerCoroutine == null)
            {
                checkerCoroutine = Instance.StartCoroutine(CheckConnectionCoroutine());
            }
        }

        private static IEnumerator CheckConnectionCoroutine()
        {
            do
            {
                checkStartedTime = Time.time;
                IsChecking = true;

                // Raise OnCheckStarted event
                if (OnCheckStarted != null)
                {
                    OnCheckStarted();
                }

                if (Instance.showDebug)
                {
                    Debug.Log("[Eazy NetChecker] Internet check started");
                }

                // Check internet connection with the selected method and get current status
                yield return Instance.StartCoroutine(GetSelectedMethod().Check());
                NetStatus previousStatus = Status;
                Status = GetSelectedMethod().GetCheckStatus();

                if (Status != previousStatus)
                {
                    // Raise OnConnectionStatusChanged event
                    if (OnConnectionStatusChanged != null)
                    {
                        OnConnectionStatusChanged();
                    }
                }

                IsChecking = false;

                // Raise OnCheckFinished event
                if (OnCheckFinished != null)
                {
                    OnCheckFinished();
                }

                if (Instance.showDebug)
                {
                    Debug.Log("[Eazy NetChecker] Internet check finished. Internet status: " + Status);
                }

                if (Status == NetStatus.Connected && stopOnSuccess)
                {
                    keepChecking = false;
                }

                // calculate the next time a net check will be performed
                nextCheckTime = keepChecking ? Time.time + Instance.checkInterval : nextCheckTime;

                yield return keepChecking ? new WaitForSeconds(Instance.checkInterval) : null;
            } while (keepChecking);

            checkerCoroutine = null;
        }
    }
}
