using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class LeaderboardPlayer : MonoBehaviour {

        public delegate void GameActive();
        public static event GameActive OnActiveGame;
        public string userName, id, facebookId;
        public bool isOnline;
        public Text playerNameLabel, rankLabel, ratingLabel;
        public Image profilePicture;
        public Texture2D defaultProfilePicture;
        public Image onlineTexture;
        private GameObject UIScreen;

        void Start () {
            UIScreen = GameObject.Find("UI Screen");
            //UpdatePlayer();
        }

        public void Reset() {
            playerNameLabel.text = "";
            rankLabel.text = "";
            ratingLabel.text = "";
            profilePicture.sprite = null;
            isOnline = false;
            userName = "";
            id = "";
            facebookId = "";
        }

        public void UpdatePlayer()
        {
            onlineTexture.color = isOnline ? Color.green : Color.gray;

            if (facebookId != null) {
                StartCoroutine(UserManager.instance.GetFBPicture(facebookId, (sprite)=>
                    {
                        profilePicture.sprite = sprite;
                    }));
            } else {
                profilePicture.sprite = Sprite.Create(defaultProfilePicture, 
                    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height), 
                    new Vector2(0.5f, 0.5f));
            }
        }

        public void OpenNewLeaderboardChallengeGame()
        {
            Debug.Log("OpenNewLeaderboardChallengeGame userName: " + userName);

            ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            ViewController.instance.HideTabView();
            //BoardSelectionManager.instance.LoadMiniBoards();
            GameManager.instance.TransitionToGameOptionsScreen(GameType.LEADERBOARD, id, userName, profilePicture);
        }
    }
}
