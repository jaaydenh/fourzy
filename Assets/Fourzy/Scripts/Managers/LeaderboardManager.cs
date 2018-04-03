using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

namespace Fourzy
{
    public class LeaderboardManager : Singleton<LeaderboardManager> {

        public List<GameObject> players = new List<GameObject>();
        public GameObject leaderboardPlayersList;
        public GameObject leaderboardPlayerPrefab;
        public Text noPlayerText;
        private bool loadingLeaderboard = false;
        bool loadingWinsLeaderboard;

        public void GetCoinsEarnedLeaderboard()
        {
            if (!loadingLeaderboard)
            {
                loadingLeaderboard = true;

                //Every time we call GetChallengeInvites we'll refresh the list
                for (int i = 0; i < players.Count; i++)
                {
                    //Destroy all runningGame gameObjects currently in the scene
                    //Destroy(players[i]);
                    LeanPool.Despawn(players[i].gameObject);
                }

                players.Clear();

                int playerCount = 0;

                new LeaderboardDataRequest()
                .SetLeaderboardShortCode("coinsEarnedLeaderboard")
                .SetEntryCount(150) // we need to parse this text input, since the entry count only takes long
                .Send((response) =>
                    {
                        if (!response.HasErrors)
                        {
                            foreach (LeaderboardDataResponse._LeaderboardData entry in response.Data) // iterate through the leaderboard data
                            {                                
                                //GameObject go = Instantiate(leaderboardPlayerPrefab) as GameObject;
                                //GameObject go = LeanPool.Spawn(leaderboardPlayerPrefab) as GameObject;

                                GameObject go = LeanPool.Spawn(leaderboardPlayerPrefab, Vector3.zero,Quaternion.identity, leaderboardPlayersList.transform) as GameObject;
                                
                                //go.gameObject.transform.SetParent(leaderboardPlayersList.transform);
                                LeaderboardPlayer leaderboardPlayer = go.GetComponent<LeaderboardPlayer>();
                                leaderboardPlayer.Reset();

                                leaderboardPlayer.id = entry.UserId;
                                leaderboardPlayer.userName = entry.UserName;
                                leaderboardPlayer.facebookId = entry.ExternalIds.GetString("FB");
                                leaderboardPlayer.playerNameLabel.text = entry.UserName;
                                leaderboardPlayer.rankLabel.text = entry.Rank.ToString();
                                leaderboardPlayer.typeLabel.text = "Coins :";    

                                string coins = entry.JSONData["coins"].ToString();
                                leaderboardPlayer.ratingLabel.text = coins;

                                leaderboardPlayer.UpdatePlayer();
                                //leaderboardPlayer.transform.localScale = new Vector3(1f,1f,1f);
                                playerCount++;
                                players.Add(go);
                            }
                            if (playerCount == 0) {
                                noPlayerText.gameObject.SetActive(true);
                            } else {
                                noPlayerText.gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            Debug.Log("***** Error Retrieving Leaderboard Data: " + response.Errors.JSON);
                            noPlayerText.text = "Error Retrieving Leaderboard Data: " + response.Errors.JSON;
                            noPlayerText.gameObject.SetActive(true);
                        }
                        
                        loadingLeaderboard = false;
                    });
            }
        }

        public void GetWinsLeaderboard()
        {
            if (!loadingWinsLeaderboard)
            {
                loadingWinsLeaderboard = true;

                //Every time we call GetChallengeInvites we'll refresh the list
                for (int i = 0; i < players.Count; i++)
                {
                    //Destroy all runningGame gameObjects currently in the scene
                    //Destroy(players[i]);
                    LeanPool.Despawn(players[i].gameObject);
                }

                players.Clear();

                int playerCount = 0;

                new LeaderboardDataRequest()
                .SetLeaderboardShortCode("winLossLeaderboard")
                .SetEntryCount(100)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        foreach (LeaderboardDataResponse._LeaderboardData entry in response.Data) // iterate through the leaderboard data
                        {
                            GameObject go = LeanPool.Spawn(leaderboardPlayerPrefab) as GameObject;
                            go.gameObject.transform.SetParent(leaderboardPlayersList.transform);
                            LeaderboardPlayer leaderboardPlayer = go.GetComponent<LeaderboardPlayer>();
                            leaderboardPlayer.Reset();

                            leaderboardPlayer.id = entry.UserId;
                            leaderboardPlayer.userName = entry.UserName;
                            leaderboardPlayer.facebookId = entry.ExternalIds.GetString("FB");
                            leaderboardPlayer.playerNameLabel.text = entry.UserName;
                            leaderboardPlayer.rankLabel.text = entry.Rank.ToString();
                            leaderboardPlayer.typeLabel.text = "Wins :";    

                            string wins = entry.JSONData["wins"].ToString();
                            leaderboardPlayer.ratingLabel.text = wins;

                            leaderboardPlayer.UpdatePlayer();
                            leaderboardPlayer.transform.localScale = new Vector3(1f, 1f, 1f);
                            playerCount++;
                            players.Add(go);
                        }
                        if (playerCount == 0)
                        {
                            noPlayerText.gameObject.SetActive(true);
                        }
                        else
                        {
                            noPlayerText.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        Debug.Log("***** Error Retrieving Leaderboard Data: " + response.Errors.JSON);
                        noPlayerText.text = "Error Retrieving Leaderboard Data: " + response.Errors.JSON;
                        noPlayerText.gameObject.SetActive(true);
                    }

                    loadingWinsLeaderboard = false;
                });
            }
        }
    }
}

