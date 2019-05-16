using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Utility;


namespace SA.iOS.UIKit
{
    /// <summary>
    /// An action that can be taken when the user taps a button in an alert.
    /// </summary>
    [Serializable]
    public class ISN_UIAlertAction 
    {

        [SerializeField] int m_id;
        [SerializeField] string m_title;
        [SerializeField] ISN_UIAlertActionStyle m_style;
        [SerializeField] bool m_enabled = true;
        [SerializeField] bool m_preffered = false;


        #pragma warning disable 414
        [SerializeField] string m_image;
        #pragma warning restore 414


        private Action m_action;


        /// <summary>
        /// Create and return an action with the specified title and behavior.
        /// </summary>
        /// <param name="title">
        /// The text to use for the button title. The value you specify should be localized for the user’s current language. 
        /// This parameter must not be nil, except in a tvOS app where a nil title may be used with <see cref="ISN_UIAlertActionStyle.Cancel"/>..
        /// </param>
        /// <param name="style">
        /// Additional styling information to apply to the button. 
        /// Use the style information to convey the type of action that is performed by the button. 
        /// For a list of possible values, see the constants in <see cref="ISN_UIAlertActionStyle"/>.
        /// </param>
        /// <param name="action">A block to execute when the user selects the action.</param>
        public ISN_UIAlertAction(string title, ISN_UIAlertActionStyle style, Action action) {
            m_id = SA_IdFactory.NextId;
            m_title = title;
            m_style = style;
            m_action = action;
        }

        /// <summary>
        /// Gets the unique action identifier.
        /// </summary>
        public int Id {
            get {
                return m_id;
            }
        }

        /// <summary>
        /// The title of the action’s button.
        /// </summary>
        public string Title {
            get {
                return m_title;
            }

        }

        /// <summary>
        /// The style that is applied to the action’s button.
        /// </summary>
        public ISN_UIAlertActionStyle Style {
            get {
                return m_style;
            }
        }

        /// <summary>
        /// A Boolean value indicating whether the action is currently enabled.
        /// </summary>
        public bool Enabled {
            get {
                return m_enabled;
            }

            set {
                m_enabled = value;
            }
        }

        /// <summary>
        /// True if action is preffered.
        /// </summary>
        public bool Preffered {
            get {
                return m_preffered;
            }
        }


        /// <summary>
        /// Add's an image to the action ui.
        /// </summary>
        /// <param name="image">Action Image. The image has to be redeable.</param>
        public void SetImage(Texture2D image) {
            m_image = image.ToBase64String(); 
        }



        public void MakePreffered() {
            m_preffered = true;
        }

        public void Invoke() {
            m_action.Invoke();
        }
    }
}