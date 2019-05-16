using System;
using System.Collections.Generic;
using UnityEngine;
using SA.Foundation.Utility;
using SA.iOS.UIKit.Internal;

namespace SA.iOS.UIKit
{

    /// <summary>
    /// An object that displays an alert message to the user.
    /// Use this class to configure alerts and action sheets with the message that you want to display and the actions from which to choose. 
    /// After configuring the alert controller with the actions and style you want, present it using the <see cref="Present"/> method. 
    /// UIKit displays alerts and action sheets modally over your app's content.
    /// 
    /// In addition to displaying a message to a user, you can associate actions with your alert controller to give the user a way to respond. 
    /// For each action you add using the <see cref="AddAction"/> method, 
    /// the alert controller configures a button with the action details. 
    /// When the user taps that action, the alert controller executes the block you provided when creating the action object. 
    /// </summary>
    [Serializable]
    public class ISN_UIAlertController 
    {
        [SerializeField] int m_id;
        [SerializeField] string m_title;
        [SerializeField] string m_message;
        [SerializeField] ISN_UIAlertControllerStyle m_preferredStyle;

        [SerializeField] List<ISN_UIAlertAction> m_actions = new List<ISN_UIAlertAction>();

        private ISN_UIAlertAction m_preferredAction = null;


        public ISN_UIAlertController(string title, string message, ISN_UIAlertControllerStyle preferredStyle) {
            m_id = SA_IdFactory.NextId;
            m_title = title;
            m_message = message;
            m_preferredStyle = preferredStyle;

        }

        /// <summary>
        /// Presents a view controller modally.
        /// </summary>
        public void Present() {
            ISN_UILib.API.PresentUIAlertController(this);
            ISN_UILib.API.OnUIAlertActionPerformed.AddListener(OnAction);
        }

        /// <summary>
        /// Dismiss view controller.
        /// </summary>
        public void Dismiss() {
            ISN_UILib.API.DismissUIAlertController(this);
            ISN_UILib.API.OnUIAlertActionPerformed.RemoveListener(OnAction);
        }


        /// <summary>
        /// Attaches an action object to the alert or action sheet.
        /// If your alert has multiple actions, the order in which you add those actions determines their order in the resulting alert or action sheet.
        /// </summary>
        /// <param name="action">
        /// The action object to display as part of the alert. Actions are displayed as buttons in the alert. 
        /// The action object provides the button text and the action to be performed when that button is tapped.
        /// </param>
        public void AddAction(ISN_UIAlertAction action) {
            m_actions.Add(action);
        }

        /// <summary>
        /// The preferred action for the user to take from an alert.
        /// 
        /// The preferred action is relevant for the <see cref="ISN_UIAlertControllerStyle.Alert"/> style only; 
        /// it is not used by action sheets. 
        /// When you specify a preferred action, the alert controller highlights the text of that action to give it emphasis. 
        /// (If the alert also contains a cancel button, the preferred action receives the highlighting instead of the cancel button.) 
        /// If the iOS device is connected to a physical keyboard, pressing the Return key triggers the preferred action.
        /// 
        /// The action object you assign to this property must have already been added to the alert controller’s list of actions.
        /// Assigning an object to this property before adding it with the <see cref="AddAction"/> method is a programmer error.
        /// The default value of this property is nil.
        /// </summary>
        public ISN_UIAlertAction PreferredAction {
            get {
                return m_preferredAction;
            } 

            set {
                m_preferredAction = value;
                m_preferredAction.MakePreffered();
            }
        }

        /// <summary>
        /// Gets the unique alert identifier.
        /// </summary>
        public int Id {
            get {
                return m_id;
            }
        }

        /// <summary>
        /// The title of the alert.
        /// 
        /// The title string is displayed prominently in the alert or action sheet. 
        /// You should use this string to get the user’s attention and communicate the reason for displaying the alert.
        /// </summary>
        public string Title {
            get {
                return m_title;
            }
        }

        /// <summary>
        /// Descriptive text that provides more details about the reason for the alert.
        /// 
        /// The message string is displayed below the title string and is less prominent. 
        /// Use this string to provide additional context about the reason for the alert or about the actions that the user might take.
        /// </summary>
        public string Message {
            get {
                return m_message;
            }
        }

        /// <summary>
        /// The style of the alert controller.
        /// </summary>
        public ISN_UIAlertControllerStyle PreferredStyle {
            get {
                return m_preferredStyle;
            }
        }

        /// <summary>
        /// The actions that the user can take in response to the alert or action sheet.
        /// 
        /// The actions are in the order in which you added them to the alert controller. 
        /// This order also corresponds to the order in which they are displayed in the alert or action sheet. 
        /// The second action in the array is displayed below the first, the third is displayed below the second, and so on.
        /// </summary>
        public List<ISN_UIAlertAction> Actions {
            get {
                return m_actions;
            }

            set {
                m_actions = value;
            }
        }



        private void OnAction(ISN_UIAlertActionId actionId) {
            if(m_id.Equals(actionId.AlertId)) {
                foreach(var action in m_actions) {
                    if(action.Id.Equals(actionId.ActionId)) {
                        action.Invoke();
                        ISN_UILib.API.OnUIAlertActionPerformed.RemoveListener(OnAction);
                    }
                }
            }
        }
    }
}