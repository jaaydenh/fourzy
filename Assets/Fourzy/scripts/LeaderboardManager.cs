using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using UnityEngine;

namespace Fourzy {

    public class LeaderboardManager : MonoBehaviour {

        void Start () {

        }

        public void GetLeaderboard()
        {
            Debug.Log ("Fetching Leaderboard Data...");

            new LeaderboardDataRequest ()
                .SetLeaderboardShortCode ("ratingLeaderboard")
                .SetEntryCount(100) // we need to parse this text input, since the entry count only takes long
                .Send ((response) => {

                    if(!response.HasErrors)
                    {
                        Debug.Log("Found Leaderboard Data...");
                        foreach(LeaderboardDataResponse._LeaderboardData entry in response.Data) // iterate through the leaderboard data
                        {
                            int rank = (int)entry.Rank; // we can get the rank directly
                            string playerName = entry.UserName;
                            string playerRating = entry.JSONData["playerRating"].ToString(); // we need to get the key, in order to get the score
                        }
                    }
                    else
                    {
                        Debug.Log("Error Retrieving Leaderboard Data...");
                    }

                });
        }
    }
}

