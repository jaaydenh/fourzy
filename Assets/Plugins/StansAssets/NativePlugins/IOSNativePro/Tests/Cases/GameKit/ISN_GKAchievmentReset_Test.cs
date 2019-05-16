using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Templates;

using SA.iOS.GameKit;
using SA.iOS.Utilities;


namespace SA.iOS.Tests.GameKit
{
    public class ISN_GKAchievmentReset_Test : SA_BaseTest
    {

        public override void Test() {


            ISN_GKAchievement.ResetAchievements((result) => {
                if (result.IsSucceeded) {
                    CheckIfResetCompleted();
                } else {
                    SetAPIResult(result);
                }
            });

        }



        private void CheckIfResetCompleted() {
            ISN_GKAchievement.LoadAchievements((result) => {
                if (result.IsSucceeded) {
                    if (result.Achievements.Count == 0) {
                        SetResult(SA_TestResult.OK);
                    } else {
                        SetResult(SA_TestResult.WithError("Reporteed achivemnts list has to be eempty, but it's not"));
                    }
                } else {
                    SetAPIResult(result);
                }
            });
        }

    }
}