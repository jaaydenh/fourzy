////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using SA.iOS.Utilities;
using SA.Foundation.Events;

#if (UNITY_IPHONE || UNITY_TVOS) && APP_DELEGATE_ENABLED
using System.Runtime.InteropServices;
#endif


namespace SA.iOS.UIKit
{
    /// <summary>
    /// A set of methods that are called in response to important events in the lifetime of your app.
    /// </summary>
    public class ISN_UIApplicationDelegate : ISN_Singleton<ISN_UIApplicationDelegate>
    {

        #if (UNITY_IPHONE || UNITY_TVOS) && APP_DELEGATE_ENABLED
        [DllImport("__Internal")] static extern void _ISN_AppDelegate_Subscribe();
        [DllImport("__Internal")] static extern string _ISN_AppDelegate_GetAppOpenShortcutItem();
        [DllImport("__Internal")] static extern string _ISN_AppDelegate_GetLaunchUniversalLink();
        [DllImport("__Internal")] static extern string _ISN_AppDelegate_GetLaunchURL();
        #endif

        private SA_Event<string> m_openURL = new SA_Event<string>();
        private SA_Event<string> m_continueUserActivity = new SA_Event<string>();
        private SA_Event<ISN_UIApplicationShortcutItem> m_performActionForShortcutItem = new SA_Event<ISN_UIApplicationShortcutItem>();

        private SA_Event m_applicationDidEnterBackground = new SA_Event();
        private SA_Event m_applicationWillEnterForeground = new SA_Event();
        private SA_Event m_applicationDidBecomeActive = new SA_Event();
        private SA_Event m_applicationWillResignActive = new SA_Event();
        private SA_Event m_applicationDidReceiveMemoryWarning = new SA_Event();
        private SA_Event m_applicationWillTerminate = new SA_Event();


        private SA_Event<ISN_UIRegisterRemoteNotificationsResult> m_didRegisterForRemoteNotifications = new SA_Event<ISN_UIRegisterRemoteNotificationsResult>();
      

        //--------------------------------------
        // Initialization
        //--------------------------------------

        protected override void Awake() {
            base.Awake();
            #if (UNITY_IPHONE || UNITY_TVOS) && APP_DELEGATE_ENABLED && !UNITY_EDITOR
            _ISN_AppDelegate_Subscribe();
            #endif
        }


        //--------------------------------------
        // ShortcutItem
        //--------------------------------------


        /// <summary>
        /// Called when the user selects a Home screen quick action for your app, 
        /// except when you’ve intercepted the interaction in a launch method.
        /// </summary>
        public SA_iEvent<ISN_UIApplicationShortcutItem> PerformActionForShortcutItem {
            get {
                return m_performActionForShortcutItem;
            }
        }

        /// <summary>
        /// In case application was launched using Home screen quick action,
        /// This method will return instance of <see cref="ISN_UIApplicationShortcutItem"/>,
        /// with the correspondent item type.
        /// <c>null</c> if app wasn't launched using Home screen quick action.
        /// </summary>
        public ISN_UIApplicationShortcutItem GetAppOpenShortcutItem() {
            string shortcutItemType = string.Empty;
            #if (UNITY_IPHONE || UNITY_TVOS) && APP_DELEGATE_ENABLED && !UNITY_EDITOR
            shortcutItemType = _ISN_AppDelegate_GetAppOpenShortcutItem();
            #endif

            if(string.IsNullOrEmpty(shortcutItemType)) {
                return null;
            } else {
                return GetShortcutByType(shortcutItemType);
            }

        }

        void performActionForShortcutItem(string shortcutItemType) {
            ISN_UIApplicationShortcutItem shortcutItem = GetShortcutByType(shortcutItemType);
            m_performActionForShortcutItem.Invoke(shortcutItem);
        }

        ISN_UIApplicationShortcutItem GetShortcutByType(string type) {
            foreach (var shortcut in ISN_Settings.Instance.ShortcutItems) {
                if(shortcut.Type.Equals(type)) {
                    return shortcut;
                }
            }

            return new ISN_UIApplicationShortcutItem(type);
        }


        //--------------------------------------
        //  Universal Links (Deeplinking)
        //--------------------------------------

        /// <summary>
        /// Tells the delegate that the data for continuing an activity is available.
        /// 
        /// The app calls this method when it receives data associated with a user activity. 
        /// For example, when the user uses Handoff to transfer an activity from a different device.
        /// Use this method to update your iOS app so that the user can continue the activity from where they left off. 
        /// </summary>
        public SA_iEvent<string> ContinueUserActivity {
            get {
                return m_continueUserActivity;
            }
        }

        /// <summary>
        /// If applicaion was launched usning deeplinking, 
        /// method will return URL that was used to launch the app. 
        /// An Empty string will be returns otherwise. 
        /// </summary>
        public string GetLaunchUniversalLink() {
            string launchUrl = string.Empty;
            #if (UNITY_IPHONE || UNITY_TVOS) && APP_DELEGATE_ENABLED && !UNITY_EDITOR
            launchUrl = _ISN_AppDelegate_GetLaunchUniversalLink();
            #endif

            return launchUrl;
        }

        void continueUserActivity(string webpageURL) {
            m_continueUserActivity.Invoke(webpageURL);
        }



        //--------------------------------------
        //  Application URL Sheme
        //--------------------------------------

        /// <summary>
        /// Asks the delegate to open a resource specified by a URL
        /// 
        /// If a URL arrives while your app is suspended or running in the background, 
        /// the system moves your app to the foreground prior to calling this method.
        /// </summary>
        public SA_iEvent<string> OpenURL {
            get {
                return m_openURL;
            }
        }

      
        /// <summary>
        /// If your app has received reques to open spesific URL, 
        /// the URL will be saved and always accessible by this method. 
        /// Methods will return an empty string if no open URL was received
        /// </summary>
        /// <returns>The launch URL.</returns>
        public string GetLaunchURL() {
            string launchUrl = string.Empty;
            #if (UNITY_IPHONE || UNITY_TVOS) && APP_DELEGATE_ENABLED && !UNITY_EDITOR
            launchUrl = _ISN_AppDelegate_GetLaunchURL();
            #endif

            return launchUrl;
        }

        void openURL(string url) {
            m_openURL.Invoke(url);
        }


        //--------------------------------------
        // Push Notificagions
        //--------------------------------------



        /// <summary>
        /// Tells the delegate result of app attempt to registered with Apple Push Notification service (APNs).
        /// 
        /// After you call the <see cref="ISN_UIApplication.RegisterForRemoteNotifications"/> method, 
        /// the app calls this method when device registration completes. 
        /// In your implementation of this method, connect with your push notification server and give the token to it. 
        /// APNs pushes notifications only to the device represented by the token.
        /// Or act accordinally to a given error desribtion.
        /// 
        /// The app might call this method in other rare circumstances, 
        /// such as when the user launches an app after having restored a device from data 
        /// that is not the device’s backup data. 
        /// In this exceptional case, the app won’t know the new device’s token until the user launches it.
        /// 
        /// For more information about how to implement remote notifications in your app, 
        /// see <see href="https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/index.html#//apple_ref/doc/uid/TP40008194">Local and Remote Notification Programming Guide.</see>
        /// </summary>
        public SA_iEvent<ISN_UIRegisterRemoteNotificationsResult> DidRegisterForRemoteNotifications {
            get {
                return m_didRegisterForRemoteNotifications;
            }
        }


        void RemoteNotificationsRegisterationResult(string json) {
            ISN_UIRegisterRemoteNotificationsResult result = JsonUtility.FromJson<ISN_UIRegisterRemoteNotificationsResult>(json); 
            m_didRegisterForRemoteNotifications.Invoke(result);
        }


        //--------------------------------------
        // Application System Events
        //--------------------------------------


        /// <summary>
        /// Tells the delegate that the app is now in the background.
        /// Use this method to release shared resources, invalidate timers, 
        /// and store enough app state information to restore your app to its current state in case it is terminated later. 
        /// You should also disable updates to your app’s user interface and avoid using some types of shared system resources 
        /// (such as the user’s contacts database). It is also imperative that you avoid using OpenGL ES in the background.
        /// 
        /// Your implementation of this method has approximately five seconds to perform any tasks and return.
        /// </summary>
        public SA_iEvent ApplicationDidEnterBackground {
            get {
                return m_applicationDidEnterBackground;
            }
        }

        void applicationDidEnterBackground(string data) {
            m_applicationDidEnterBackground.Invoke();
        }

        /// <summary>
        /// Tells the delegate that the app is about to enter the foreground.
        /// 
        /// In iOS 4.0 and later, this method is called as part of the transition from the background to the active state. 
        /// You can use this method to undo many of the changes you made to your app upon entering the background. 
        /// The call to this method is invariably followed by a call to the <see cref="ApplicationDidBecomeActive"/> method, 
        /// which then moves the app from the inactive to the active state.
        /// </summary>
        public SA_iEvent ApplicationWillEnterForeground {
            get {
                return m_applicationWillEnterForeground;
            }
        }

        void applicationWillEnterForeground(string data) {
            m_applicationWillEnterForeground.Invoke();
        }


        /// <summary>
        /// Tells the delegate that the app has become active.
        /// 
        /// This method is called to let your app know that it moved from the inactive to active state. 
        /// This can occur because your app was launched by the user or the system. 
        /// Apps can also return to the active state if the user chooses to ignore an interruption 
        /// (such as an incoming phone call or SMS message) that sent the app temporarily to the inactive state.
        /// 
        /// You should use this method to restart any tasks that were paused (or not yet started) while the app was inactive. 
        /// For example, you could use it to restart timers or throttle up OpenGL ES frame rates. 
        /// If your app was previously in the background, you could also use it to refresh your app’s user interface.
        /// </summary>
        public SA_iEvent ApplicationDidBecomeActive {
            get {
                return m_applicationDidBecomeActive;
            }
        }

        /// <summary>
        /// Tells the delegate that the app is about to become inactive.
        /// 
        /// This method is called to let your app know that it is about to move from the active to inactive state. 
        /// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
        /// or when the user quits the app and it begins the transition to the background state. 
        /// An app in the inactive state continues to run but does not dispatch incoming events to responders.
        ///
        /// You should use this method to pause ongoing tasks, disable timers, and throttle down OpenGL ES frame rates.
        /// Games should use this method to pause the game.
        /// An app in the inactive state should do minimal work while it waits to transition to either the active or background state.
        /// If your app has unsaved user data, you can save it here to ensure that it is not lost. 
        /// However, it is recommended that you save user data at appropriate points throughout the execution of your app, 
        /// usually in response to specific actions. For example, save data when the user dismisses a data entry screen. 
        /// Do not rely on specific app state transitions to save all of your app’s critical data.
        /// </summary>
        void applicationDidBecomeActive(string data) {
            m_applicationDidBecomeActive.Invoke();
        }


        /// <summary>
        /// Tells the delegate that the app is about to become inactive.
        /// 
        /// This method is called to let your app know that it is about to move from the active to inactive state. 
        /// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
        /// or when the user quits the app and it begins the transition to the background state. 
        /// An app in the inactive state continues to run but does not dispatch incoming events to responders.
        ///
        /// You should use this method to pause ongoing tasks, disable timers, 
        /// and throttle down OpenGL ES frame rates.Games should use this method to pause the game.
        /// An app in the inactive state should do minimal work while it waits to transition to either the active or background state.
        ///
        /// If your app has unsaved user data, you can save it here to ensure that it is not lost. 
        /// However, it is recommended that you save user data at appropriate points throughout the execution of your app, 
        /// usually in response to specific actions. 
        /// For example, save data when the user dismisses a data entry screen. 
        /// Do not rely on specific app state transitions to save all of your app’s critical data.
        /// </summary>
        public SA_iEvent ApplicationWillResignActive {
            get {
                return m_applicationWillResignActive;
            }
        }

        void applicationWillResignActive(string data) {
            m_applicationDidBecomeActive.Invoke();
        }


        /// <summary>
        /// Tells the delegate when the app receives a memory warning from the system.
        /// 
        /// Your implementation of this method should free up as much memory as possible 
        /// by purging cached data objects that can be recreated (or reloaded from disk) later.
        /// 
        /// It is strongly recommended that you implement this method. 
        /// If your app does not release enough memory during low-memory conditions, the system may terminate it outright. 
        /// </summary>
        public SA_iEvent ApplicationDidReceiveMemoryWarning {
            get {
                return m_applicationDidReceiveMemoryWarning;
            }
        }

        void applicationDidReceiveMemoryWarning(string data) {
            m_applicationDidReceiveMemoryWarning.Invoke();
        }

        /// <summary>
        /// Tells the delegate when the app is about to terminate.
        /// 
        /// This method lets your app know that it is about to be terminated and purged from memory entirely. 
        /// You should use this method to perform any final clean-up tasks for your app, such as freeing shared resources, 
        /// saving user data, and invalidating timers. 
        /// Your implementation of this method has approximately five seconds to perform any tasks and return. 
        /// If the method does not return before time expires, the system may kill the process altogether.
        /// </summary>
        public SA_iEvent ApplicationWillTerminate {
            get {
                return m_applicationWillTerminate;
            }
        }

        void applicationWillTerminate(string data) {
            m_applicationWillTerminate.Invoke();
        }



    }
}