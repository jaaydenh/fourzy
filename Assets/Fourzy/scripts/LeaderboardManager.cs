using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using UnityEngine;

namespace Fourzy
{

    public class LeaderboardManager : MonoBehaviour {

        public static LeaderboardManager instance;

        public List<GameObject> players = new List<GameObject>();
        public GameObject leaderboardPlayersList;
        public GameObject leaderboardPlayerPrefab;
        private bool loadingLeaderboard = false;

        void Start()
        {
            instance = this;
        }

        public void GetLeaderboard()
        {
            if (!loadingLeaderboard)
            {
                loadingLeaderboard = true;

                //Every time we call GetChallengeInvites we'll refresh the list
                for (int i = 0; i < players.Count; i++)
                {
                    //Destroy all runningGame gameObjects currently in the scene
                    Destroy(players[i]);
                }

                players.Clear();
                new LeaderboardDataRequest()
                .SetLeaderboardShortCode("gamesCompletedLeaderboard")
                .SetEntryCount(100) // we need to parse this text input, since the entry count only takes long
                .Send((response) =>
                    {
                        if (!response.HasErrors)
                        {
                            Debug.Log("LEADERBOARD");
                            foreach (LeaderboardDataResponse._LeaderboardData entry in response.Data) // iterate through the leaderboard data
                            {                                
                                Debug.Log("LEADERBOARD: userid: " + entry.UserId);
                                GameObject go = Instantiate(leaderboardPlayerPrefab) as GameObject;
                                go.gameObject.transform.SetParent(leaderboardPlayersList.transform);
                                LeaderboardPlayer leaderboardPlayer = go.GetComponent<LeaderboardPlayer>();

                                //int rank = (int)entry.Rank; // we can get the rank directly
                                //int? playerRating = entry.ScriptData.GetInt("playerRaing");
                                //string playerName = entry.UserName;
                                //string playerRating = entry.JSONData["playerRating"].ToString();

                                leaderboardPlayer.playerId = entry.UserId;
                                leaderboardPlayer.facebookId = entry.ExternalIds.GetString("FB");
                                leaderboardPlayer.playerNameLabel.text = entry.UserName;
                                leaderboardPlayer.rankLabel.text = entry.Rank.ToString();

                                string gamesCompleted = entry.JSONData["gamesCompleted"].ToString();
                                leaderboardPlayer.ratingLabel.text = gamesCompleted;
                                // string wins = entry.JSONData["wins"].ToString();
                                // string losses = entry.JSONData["losses"].ToString();
                                // leaderboardPlayer.ratingLabel.text = wins + "/" + losses;
                                // leaderboardPlayer.ratingLabel.text = entry.JSONData["playerRating"].ToString();
                                leaderboardPlayer.transform.localScale = new Vector3(1f,1f,1f);
                                players.Add(go);
                            }
                        }
                        else
                        {
                            Debug.Log("Error Retrieving Leaderboard Data...");
                        }
                        
                        loadingLeaderboard = false;
                    });
            }
        }
    }
}

