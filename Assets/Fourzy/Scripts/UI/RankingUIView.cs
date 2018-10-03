using UnityEngine;
using UnityEngine.UI;
using HedgehogTeam.EasyTouch;
using System.Collections.Generic;
using Lean.Pool;

namespace Fourzy
{
    public class RankingUIView : UIView
    {
        //Instance
        public static RankingUIView instance;
        public Text winTabText;
        public Text coinsEarnedTabText;

        [SerializeField] List<GameObject> players = new List<GameObject>();
        [SerializeField] GameObject leaderboardPlayersList;
        [SerializeField] GameObject leaderboardPlayerPrefab;
        [SerializeField] Text noPlayerText;
        private bool loadingCoinsEarnLeaderboard;
        private bool loadingWinsLeaderboard;

        void Start()
        {
            instance = this;
            keepHistory = true;
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void ShowAnimated(AnimationDirection sourceDirection)
        {
            ViewController.instance.SetActiveView(TotalView.viewRanking);
            base.ShowAnimated(sourceDirection);
            this.GetWinsLeaderboard();
        }

        public override void HideAnimated(AnimationDirection getAwayDirection)
        {
            base.HideAnimated(getAwayDirection);
        }

        public void WinsButton() 
        {
            this.GetWinsLeaderboard();
            coinsEarnedTabText.fontSize = 35;
            winTabText.fontSize = 40;
            winTabText.GetComponent<Outline>().enabled = true;
            coinsEarnedTabText.GetComponent<Outline>().enabled = false;
        }

        public void CoinsEarnedButton() 
        {
            this.GetCoinsEarnedLeaderboard();
            coinsEarnedTabText.fontSize = 40;
            winTabText.fontSize = 35;
            winTabText.GetComponent<Outline>().enabled = false;
            coinsEarnedTabText.GetComponent<Outline>().enabled = true;
        }

        private void GetWinsLeaderboard()
        {
            if (loadingWinsLeaderboard)
            {
                return;
            }

            loadingWinsLeaderboard = true;

            LoginManager.instance.GetWinsLeaderboard(GetLeaderboardCallback);
        }

        private void GetCoinsEarnedLeaderboard()
        {
            if (loadingCoinsEarnLeaderboard)
            {
                return;
            }

            loadingCoinsEarnLeaderboard = true;

            LoginManager.instance.GetCoinsEarnedLeaderboard(GetLeaderboardCallback);
        }

        private void GetLeaderboardCallback(List<Leaderboard> leaderboards, string errorMessage)
        {
            if (leaderboards == null)
            {
                noPlayerText.text = errorMessage;
                noPlayerText.gameObject.SetActive(true);
                return;
            }

            for (int i = 0; i < players.Count; i++)
            {
                LeanPool.Despawn(players[i].gameObject);
            }

            players.Clear();

            foreach (Leaderboard leadboard in leaderboards)
            {
                GameObject go = LeanPool.Spawn(leaderboardPlayerPrefab, Vector3.zero, Quaternion.identity, leaderboardPlayersList.transform) as GameObject;

                LeaderboardPlayer leaderboardPlayer = go.GetComponent<LeaderboardPlayer>();
                leaderboardPlayer.Reset();

                leaderboardPlayer.id = leadboard.userId;
                leaderboardPlayer.userName = leadboard.userName;
                leaderboardPlayer.facebookId = leadboard.facebookId;
                leaderboardPlayer.playerNameLabel.text = leadboard.userName;
                leaderboardPlayer.rankLabel.text = leadboard.rank.ToString();

                if (leadboard.isWinsLeaderboard)
                {
                    leaderboardPlayer.typeLabel.text = "Wins :";
                    leaderboardPlayer.ratingLabel.text = leadboard.wins;
                    loadingWinsLeaderboard = false;
                }
                else
                {
                    leaderboardPlayer.typeLabel.text = "Puzzles :";
                    leaderboardPlayer.ratingLabel.text = leadboard.coins;
                    loadingCoinsEarnLeaderboard = false;
                }

                if (leadboard.userId == UserManager.instance.userId)
                {
                    leaderboardPlayer.HidePlayButton();
                }


                leaderboardPlayer.UpdatePlayer();
                players.Add(go);
            }

            if (leaderboards.Count == 0)
            {
                noPlayerText.gameObject.SetActive(true);
            }
            else
            {
                noPlayerText.gameObject.SetActive(false);
            }
        }
    }
}
