

using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Templates;

using SA.iOS.GameKit;
using SA.iOS.Utilities;


namespace SA.iOS.Tests.GameKit
{
    public class ISN_GKAchievementsLoad_Test : SA_BaseTest
    {

        public override void Test() {


            ISN_GKAchievement.LoadAchievements((result) => {
                if (result.IsSucceeded) {
                    foreach (ISN_GKAchievement achievement in result.Achievements) {
                        ISN_Logger.Log("achievement.Identifier: " + achievement.Identifier);
                        ISN_Logger.Log("achievement.PercentComplete: " + achievement.PercentComplete);
                        ISN_Logger.Log("achievement.LastReportedDate: " + achievement.LastReportedDate);
                        ISN_Logger.Log("achievement.Completed: " + achievement.Completed);
                        ISN_Logger.Log("--------------------------------------");
                    }
                } 

                SetAPIResult(result);
            });
        }
    }
}