using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Templates;

using SA.iOS.GameKit;
using SA.iOS.Utilities;


namespace SA.iOS.Tests.GameKit
{
    public abstract class ISN_GKSingleLeaderboard_Test : SA_BaseTest
    {

        public override void Test() {

            ISN_GKLeaderboard.LoadLeaderboards((result) => {

                if (result.IsSucceeded && result.Leaderboards.Count > 0) {
                    OnLeaderboardReady(result.Leaderboards[0]);
                } else {
                    SetResult(SA_TestResult.WithError("Wasn't able to find leaderboards"));
                }
            });

        }

        public abstract void OnLeaderboardReady(ISN_GKLeaderboard leaderboard);
    }
}