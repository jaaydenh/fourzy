using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Templates;

using SA.iOS.Foundation;
using SA.iOS.GameKit;
using SA.iOS.Utilities;


namespace SA.iOS.Tests.GameKit
{
    public class ISN_GKReportingScores_Test : ISN_GKSingleLeaderboard_Test
    {
        public override void OnLeaderboardReady(ISN_GKLeaderboard leaderboard) {

            ISN_Logger.Log("leaderboard.LocalPlayerScore.Rank: " + leaderboard.LocalPlayerScore.Rank);
            ISN_Logger.Log("leaderboard.LocalPlayerScore.Value: " + leaderboard.LocalPlayerScore.Value);


            ISN_GKScore scoreReporter = new ISN_GKScore(leaderboard.Identifier);
            scoreReporter.Value = leaderboard.LocalPlayerScore.Value++;
            scoreReporter.Context = 1;


            scoreReporter.Report((result) => {
                SetAPIResult(result);
            });
        }
    }
}