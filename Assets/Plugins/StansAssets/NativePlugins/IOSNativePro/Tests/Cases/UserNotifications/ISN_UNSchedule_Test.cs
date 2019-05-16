using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Templates;

using SA.iOS.Foundation;
using SA.iOS.UserNotifications;


namespace SA.iOS.Tests.UserNotifications
{
    public class ISN_UNSchedule_Test : SA_BaseTest
    {

        private ISN_UNNotificationRequest m_request;

        public override void Test() {



           

            ISN_UNUserNotificationCenterDelegate.WillPresentNotification.AddListener((ISN_UNNotification notification) => {
                ValidateRequest(notification.Request);
                ISN_UNUserNotificationCenterDelegate.ClearLastReceivedResponse();
            });

            ISN_UNUserNotificationCenterDelegate.DidReceiveNotificationResponse.AddListener((ISN_UNNotificationResponse notification) => {
                ValidateRequest(notification.Notification.Request);
                ISN_UNUserNotificationCenterDelegate.ClearLastReceivedResponse();
            });


            var userInfo = new ISN_UNNotificationContent();
            userInfo.Title = "Info Test";

            var content = new ISN_UNNotificationContent();
            content.Title = "Wake up!";
            content.Body = "Rise and shine! It's morning time!";

            content.SetUserInfo(userInfo);
          

            int seconds = 2;
            bool repeats = false;
            var trigger = new ISN_UNTimeIntervalNotificationTrigger(seconds, repeats);

            // Create the request object.
            string identifier = "IntervalAlarm";
            m_request = new ISN_UNNotificationRequest(identifier, content, trigger);

            ISN_UNUserNotificationCenter.AddNotificationRequest(m_request, (result) => {
                if(result.IsFailed) {
                    SetAPIResult(result);
                }
            });

        }


        private void ValidateRequest(ISN_UNNotificationRequest presentdeRequest) {
            bool valid = true;

            if(!presentdeRequest.Content.Body.Equals(m_request.Content.Body)) {
                valid = false;
            }

            if (!presentdeRequest.Content.Title.Equals(m_request.Content.Title)) {
                valid = false;
            }

            if (!presentdeRequest.Content.Subtitle.Equals(m_request.Content.Subtitle)) {
                valid = false;
            }


            var userInfoJson1 = presentdeRequest.Content.GetUserInfo<ISN_UNNotificationContent>();
            if(!userInfoJson1.Title.Equals("Info Test")) {
                valid = false;
            }

            if(valid) {
                SetResult(SA_TestResult.OK);
            } else {
                SetResult(SA_TestResult.WithError("Request Validation failede"));
            }




        }

       
    }
}