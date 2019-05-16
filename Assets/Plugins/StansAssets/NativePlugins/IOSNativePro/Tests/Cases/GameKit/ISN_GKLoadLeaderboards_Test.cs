

using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Templates;

using SA.iOS.GameKit;
using SA.iOS.Utilities;


namespace SA.iOS.Tests.GameKit
{
    public class ISN_GKLoadLeaderboards_Test : SA_BaseTest
    {

        public override void Test() {

            ISN_GKLeaderboard.LoadLeaderboards((result) => {
                if (result.IsSucceeded) {

                    if (result.Leaderboards.Count == 0) {
                        SetResult(SA_TestResult.WithError("No leaderboards inside the Sucsses result"));
                        return;
                    }

                    foreach (var leaderboards in result.Leaderboards) {
                        ISN_Logger.Log(leaderboards.Identifier);
                        ISN_Logger.Log(leaderboards.GroupIdentifier);
                        ISN_Logger.Log(leaderboards.Title);
                    }
                }

                SetAPIResult(result);
            });

        }
    }
}