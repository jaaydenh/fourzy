using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Async;

using SA.iOS.GameKit;
using SA.iOS.Utilities;


namespace SA.iOS.Tests.GameKit
{
    public class ISN_GKAchievmentReport_Test : SA_BaseTest
    {

        public override void Test() {
            string id = "my_first_achievement";
            ReportCompletedAchievment(id);
        }


        private void ReportCompletedAchievment(string achievementId) {
            ISN_GKAchievement achievement1 = new ISN_GKAchievement(achievementId);
            achievement1.PercentComplete = 100.0f;

            achievement1.Report((result) => {
                if(result.IsSucceeded) {

                    //Smalle delay before requiesting reported list
                    SA_Coroutine.WaitForSeconds(3f, () => {
                        CheckIfCompleted(achievementId);
                    });
                   
                } else {
                    SetAPIResult(result);
                }

            });
        }


        private void CheckIfCompleted(string achievementId) {
            ISN_GKAchievement.LoadAchievements((result) => {
                if (result.IsSucceeded) {
                    if (result.Achievements.Count == 1) {
                        var achievement = result.Achievements[0];
                        if(achievement.Completed) {
                            SetResult(SA_TestResult.OK);
                        } else {
                            SetResult(SA_TestResult.WithError("Achievement has to be completed, but it's not"));
                        }
                    } else {
                        SetResult(SA_TestResult.WithError("App should 1 reeported achivement, found " + result.Achievements.Count + " instead"));
                    }
                } else {
                    SetAPIResult(result);
                }
            });
        }

    }
}