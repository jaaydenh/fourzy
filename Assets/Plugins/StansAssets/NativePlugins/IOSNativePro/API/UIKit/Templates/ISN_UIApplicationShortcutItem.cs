using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.UIKit
{
    /// <summary>
    /// An application shortcut item, 
    /// also called a Home screen dynamic quick action, that specifies a user-initiated action for your app.
    /// </summary>
    [Serializable]
    public class ISN_UIApplicationShortcutItem 
    {

        [SerializeField] string m_title = string.Empty;
        [SerializeField] string m_subtitle = string.Empty;
        [SerializeField] string m_type =  string.Empty;

        public ISN_UIApplicationShortcutItem(string type) {
            m_type = type;
        }


        /// <summary>
        /// A required, app-specific string that you employ to identify the type of quick action to perform.
        /// </summary>
        /// <value>The type.</value>
        public string Type {
            get {
                return m_type;
            }

            set {
                m_type = value;
            }
        }


        /// <summary>
        /// The quick action title
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
        /// The quick action title
        /// </summary>
        public string Subtitle {
            get {
                return m_subtitle;
            }

            set {
                m_subtitle = value;
            }
        }
    }
}