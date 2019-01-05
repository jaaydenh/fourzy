//@vadym udod

using Fourzy._Updates.UI.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class LeaderboardPlayerWidget : WidgetBase
    {
        public TMP_Text playerNameLabel;
        public TMP_Text rankLabel;
        public TMP_Text typeLabel;
        public Image profilePicture;
        public Image onlineTexture;
        public Texture2D defaultProfilePicture;

        public ButtonExtended playButton;

        private Leaderboard leaderboard;

        public void SetData(Leaderboard leaderboard)
        {
            this.leaderboard = leaderboard;
            
            playerNameLabel.text = leaderboard.userName;
            rankLabel.text = leaderboard.rank.ToString();

            if (leaderboard.isWinsLeaderboard)
                typeLabel.text = "<color=#808080ff>Wins :</color> " + leaderboard.wins;
            else
                typeLabel.text = "<color=#808080ff>Puzzles :</color> " + leaderboard.coins;

            playButton.SetActive(leaderboard.userId != UserManager.Instance.userId);

            //if (leaderboard.facebookId != null)
            //{
            //    StartCoroutine(UserManager.Instance.GetFBPicture(leaderboard.facebookId, (sprite) =>
            //        {
            //            profilePicture.sprite = sprite;
            //        }));
            //}
            //else
            //{
            //    profilePicture.sprite = Sprite.Create(defaultProfilePicture,
            //        new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
            //        new Vector2(0.5f, 0.5f));
            //}
        }

        public void OpenNewLeaderboardChallengeGame()
        {
            Debug.Log("OpenNewLeaderboardChallengeGame userName: ");

            //ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            //ViewController.instance.HideTabView();
            //ViewGameBoardSelection.instance.TransitionToViewGameBoardSelection(GameType.LEADERBOARD, id, userName, profilePicture);
        }
    }
}