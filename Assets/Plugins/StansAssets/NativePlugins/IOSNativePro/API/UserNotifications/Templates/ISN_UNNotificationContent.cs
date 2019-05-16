using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.UserNotifications
{
    /// <summary>
    /// The content of a local or remote notification.
    /// </summary>
    [Serializable]
    public class ISN_UNNotificationContent
    {
        [SerializeField] string m_title = string.Empty;
        [SerializeField] string m_subtitle = string.Empty;
        [SerializeField] string m_body = string.Empty;
        [SerializeField] long m_badge = 0;
        [SerializeField] string m_sound = string.Empty;
        [SerializeField] string m_userInfo = string.Empty;


        /// <summary>
        /// A short description of the reason for the alert.
        /// 
        /// When a title is present, the system attempts to display a notification alert. 
        /// Apps must be authorized to display alerts.
        /// 
        /// Title strings should be short, usually only a couple of words describing the reason for the notification.
        /// In watchOS, the title string is displayed as part of the short look notification interface,
        /// which has limited space.
        /// </summary>
        public string Title {
            get {
                return m_title;
            }

            set {
                m_title = value;
            }
        }


        /// <summary>
        /// A secondary description of the reason for the alert.
        /// 
        /// Subtitles offer additional context in cases where the title alone is not clear. 
        /// Subtitles are not displayed in all cases.
        /// </summary>
        public string Subtitle {
            get {
                return m_subtitle;
            }

            set {
                m_subtitle = value;
            }
        }


        /// <summary>
        /// The message displayed in the notification alert.
        /// 
        /// Printf style escape characters are stripped from the string prior to display; 
        /// to include a percent symbol (%) in the message body, use two percent symbols (%%).
        /// </summary>
        public string Body {
            get {
                return m_body;
            }

            set {
                m_body = value;
            }
        }


        /// <summary>
        /// The number to display as the app’s icon badge.
        /// 
        /// When the number in this property is 0, the system does not display a badge. 
        /// When the number is greater than 0, the system displays the badge with the specified number.
        /// </summary>
        public long Badge {
            get {
                return m_badge;
            }

            set {
                m_badge = value;
            }
        }


        /// <summary>
        /// The sound to play when the notification is delivered.
        /// 
        /// Notifications can play a default sound or a custom sound. 
        /// For information on how to specify custom sounds for your notifications, 
        /// see <see cref="ISN_UNNotificationSound"/>.
        /// </summary>
        public ISN_UNNotificationSound Sound {
            get {
                if(string.IsNullOrEmpty(m_sound)) {
                    return null;
                } else {
                    return new ISN_UNNotificationSound(m_sound);
                }
            }

            set {                
                m_sound = value.SoundName;
            }
        }


       
        /// <summary>
        /// A custom developer defined serializable object associated with the notification.
        /// 
        /// The object will be serialized with <see cref="JsonUtility.ToJson"/>. 
        /// Make sure you are using appropriate object that can be serialized.
        /// </summary>
        /// <param name="userInfo">Serializable object.</param>
        public void SetUserInfo(object userInfo) {
            m_userInfo = JsonUtility.ToJson(userInfo);
        }


        /// <summary>
        /// Get's custom developer defined serializable object associated with the notification.
        /// The object will be deserialized with <see cref="JsonUtility.FromJson"/>. 
        /// Make sure you are using appropriate object that can be deserialized.
        /// </summary>
        public T GetUserInfo<T>() {
            if(string.IsNullOrEmpty(m_userInfo)) {
                return default(T);
            }
            return JsonUtility.FromJson<T>(m_userInfo);
        }

    }
}