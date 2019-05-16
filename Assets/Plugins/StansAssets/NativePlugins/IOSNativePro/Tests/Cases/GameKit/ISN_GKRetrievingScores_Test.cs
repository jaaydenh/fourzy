using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Templates;

using SA.iOS.Foundation;
using SA.iOS.GameKit;
using SA.iOS.Utilities;


namespace SA.iOS.Tests.GameKit
{
    public class ISN_GKRetrievingScores_Test : ISN_GKSingleLeaderboard_Test
    {
        public override void OnLeaderboardReady(ISN_GKLeaderboard leaderboard) {
            var leaderboardRequest = new ISN_GKLeaderboard();
            leaderboardRequest.Identifier = leaderboard.Identifier;
            leaderboardRequest.PlayerScope = ISN_GKLeaderboardPlayerScope.Global;
            leaderboardRequest.TimeScope = ISN_GKLeaderboardTimeScope.AllTime;
            leaderboardRequest.Range = new ISN_NSRange(1, 10);


            leaderboardRequest.LoadScores((result) => {
                if (result.IsSucceeded) {
                    ISN_Logger.Log("Score Load Success");
                    foreach (var score in result.Scores) {
                        ISN_Logger.Log("score.Value: " + score.Value);
                        ISN_Logger.Log("score.Context: " + score.Context);
                        ISN_Logger.Log("score.Date: " + score.Date);
                        ISN_Logger.Log("score.Rank: " + score.Rank);
                        ISN_Logger.Log("score.Player.PlayerID: " + score.Player.PlayerID);
                        ISN_Logger.Log("score.Player.DisplayName: " + score.Player.DisplayName);
                        ISN_Logger.Log("score.Player.Alias: " + score.Player.Alias);
                    }

                    ISN_Logger.Log("leaderboardRequest.MaxRange: " + leaderboardRequest.MaxRange);
                    ISN_Logger.Log("leaderboardRequest.LocalPlayerScore.Value: " + leaderboardRequest.LocalPlayerScore.Value);
                    ISN_Logger.Log("leaderboardRequest.LocalPlayerScore.Context: " + leaderboardRequest.LocalPlayerScore.Context);
                    ISN_Logger.Log("leaderboardRequest.LocalPlayerScore.Date: " + leaderboardRequest.LocalPlayerScore.Date);
                    ISN_Logger.Log("leaderboardRequest.LocalPlayerScore.Rank: " + leaderboardRequest.LocalPlayerScore.Rank);

                }
                SetAPIResult(result);
            });
        }
    }
}