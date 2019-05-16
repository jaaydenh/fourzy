////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Koretsky Konstantin (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using SA.Foundation.Events;
using SA.Foundation.Templates;

namespace SA.iOS.UserNotifications.Internal
{
    internal class ISN_UNEditorAPI : ISN_iUNAPI
    {

        private SA_Event<ISN_UNNotification> m_willPresentNotification = new SA_Event<ISN_UNNotification>();
        private SA_Event<ISN_UNNotificationResponse> m_didReceiveNotificationResponse = new SA_Event<ISN_UNNotificationResponse>();


        public SA_iEvent<ISN_UNNotification> WillPresentNotification {
            get {
                return m_willPresentNotification;
            }
        }

        public SA_iEvent<ISN_UNNotificationResponse> DidReceiveNotificationResponse  {
            get {
                return m_didReceiveNotificationResponse;
            }
        }

        public ISN_UNNotificationResponse LastReceivedResponse {
            get {
                return null;
            }
        }

        public void AddNotificationRequest(ISN_UNNotificationRequest request, Action<SA_Result> callback) {
            
        }

        public void ClearLastReceivedResponse() {
           
        }

        public void GetDeliveredNotifications(Action<List<ISN_UNNotification>> callback) {
           
        }

        public void GetNotificationSettings(Action<ISN_UNNotificationSettings> callback) {
            
        }

        public void GetPendingNotificationRequests(Action<List<ISN_UNNotificationRequest>> callback) {
           
        }

        public void RemoveAllDeliveredNotifications() {
           
        }

        public void RemoveAllPendingNotificationRequests() {
            
        }

        public void RemoveDeliveredNotifications(List<string> notificationsIds) {
           
        }

        public void RemovePendingNotificationRequests(List<string> notificationRequestsIds) {
            
        }

        public void RequestAuthorization(int options, Action<SA_Result> callback) {
            
        }
    }
}
