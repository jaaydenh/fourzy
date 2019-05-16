using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.iOS.UserNotifications
{

    /// <summary>
    /// The object for managing notification-related settings and the authorization status of your app
    /// </summary>
    [Serializable]
    public class ISN_UNNotificationSettings 
    {
#pragma warning disable 649
        [SerializeField]  ISN_UNAuthorizationStatus m_authorizationStatus;
        [SerializeField]  ISN_UNNotificationStatus m_notificationCenterSetting;
        [SerializeField]  ISN_UNNotificationStatus m_lockScreenSetting;
        [SerializeField]  ISN_UNNotificationStatus m_carPlaySetting;
        [SerializeField]  ISN_UNNotificationStatus m_alertSetting;
        [SerializeField]  ISN_UNAlertStyle m_alertStyle;
        [SerializeField]  ISN_UNNotificationStatus m_badgeSetting;
        [SerializeField]  ISN_UNNotificationStatus m_soundSetting;
        [SerializeField]  ISN_UNShowPreviewsSetting m_showPreviewsSetting;
#pragma warning restore 649


        //--------------------------------------
        // Getting Device Settings
        //--------------------------------------


        /// <summary>
        /// The authorization status indicating the app’s ability to interact with the user.
        /// </summary>
        public ISN_UNAuthorizationStatus AuthorizationStatus {
            get {
                return m_authorizationStatus;
            }
        }

        /// <summary>
        /// The setting that indicates whether your app’s notifications are displayed in Notification Center.
        /// Your app’s notifications appear in Notification Center by default, but the user may disable this setting later.
        /// </summary>
        public ISN_UNNotificationStatus NotificationCenterSetting {
            get {
                return m_notificationCenterSetting;
            }

        }


        /// <summary>
        /// The setting that indicates whether your app’s notifications appear onscreen when the device is locked.
        /// Even if the user disables lock screen notifications, your notifications may still appear onscreen when the device is unlocked.
        /// </summary>
        public ISN_UNNotificationStatus LockScreenSetting {
            get {
                return m_lockScreenSetting;
            }
        }



        /// <summary>
        /// The setting that indicates whether your app’s notifications may be displayed in a CarPlay environment.
        /// </summary>
        public ISN_UNNotificationStatus CarPlaySetting {
            get {
                return m_carPlaySetting;
            }
        }



        //--------------------------------------
        // Getting User Notification Settings
        //--------------------------------------



        /// <summary>
        /// The authorization status for displaying alerts.
        /// </summary>
        public ISN_UNNotificationStatus AlertSetting {
            get {
                return m_alertSetting;
            }
        }


        /// <summary>
        /// The type of alert that the app may display when the device is unlocked.
        /// 
        /// When alerts are authorized, this property specifies the presentation style for alerts when the device is unlocked. 
        /// The user may choose to display alerts as automatically disappearing banners or as modal windows that require explicit dismissal. 
        /// The user may also choose not to display alerts at all.
        /// </summary>
        public ISN_UNAlertStyle AlertStyle {
            get {
                return m_alertStyle;
            }
        }


        /// <summary>
        /// The authorization status for badging your app’s icon.
        /// 
        /// When this setting is enabled, notifications may update the badge value displayed on top of the app’s icon. 
        /// The badge value is stored in the badge property of the UNNotificationContent object.
        /// </summary>
        public ISN_UNNotificationStatus BadgeSetting {
            get {
                return m_badgeSetting;
            }
        }


        /// <summary>
        /// The authorization status for playing sounds for incoming notifications.
        /// 
        /// When this setting is enabled, notifications may play sounds upon delivery. 
        /// The sound to be played for the notification is stored in the sound property of the UNNotificationContent object.
        /// </summary>
        public ISN_UNNotificationStatus SoundSetting {
            get {
                return m_soundSetting;
            }
        }


        /// <summary>
        /// The setting for whether apps show a preview of the notification's content.
        /// </summary>
        public ISN_UNShowPreviewsSetting ShowPreviewsSetting {
            get {
                return m_showPreviewsSetting;
            }
        }
    }
}