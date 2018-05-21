using UnityEngine;
using UnityEngine.UI;
using GameSparks.Api.Responses;

namespace Fourzy
{
    public class LeaderboardPlayer : MonoBehaviour {

        public delegate void GameActive();
        public static event GameActive OnActiveGame;
        public string userName, id, facebookId;
        public bool isOnline;
        public Text playerNameLabel, rankLabel, ratingLabel, typeLabel;
        public Image profilePicture;
        public Texture2D defaultProfilePicture;
        public Image onlineTexture;

        public void Reset() {
            playerNameLabel.text = "";
            rankLabel.text = "";
            ratingLabel.text = "";
            profilePicture.sprite = null;
            isOnline = false;
            userName = "";
            id = "";
            typeLabel.text = "";
            facebookId = "";
        }

        public void UpdatePlayer()
        {
            onlineTexture.color = isOnline ? Color.green : Color.gray;

            ChallengeManager.instance.GetGamePiece(id, GetGamePieceIdSuccess, GetGamePieceIdFailure);

            // if (facebookId != null) {
            //     StartCoroutine(UserManager.instance.GetFBPicture(facebookId, (sprite)=>
            //         {
            //             profilePicture.sprite = sprite;
            //         }));
            // } else {
            //     profilePicture.sprite = Sprite.Create(defaultProfilePicture, 
            //         new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height), 
            //         new Vector2(0.5f, 0.5f));
            // }
        }

        public void OpenNewLeaderboardChallengeGame()
        {
            Debug.Log("OpenNewLeaderboardChallengeGame userName: " + userName);

            ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            ViewController.instance.HideTabView();
            ViewGameBoardSelection.instance.TransitionToViewGameBoardSelection(GameType.LEADERBOARD, id, userName, profilePicture);
            // GameManager.instance.TransitionToGameOptionsScreen(GameType.LEADERBOARD, id, userName, profilePicture);
        }

        private void GetGamePieceIdSuccess(LogEventResponse response) {
            int gamePieceId = int.Parse(response.ScriptData.GetString("gamePieceId"));
            Debug.Log("GetGamePieceIdSuccess: " + response.ScriptData.GetString("gamePieceId"));
            profilePicture.sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(gamePieceId);
        }

        private void GetGamePieceIdFailure(LogEventResponse response) {
            Debug.Log("***** Error getting player gamepiece: " + response.Errors.JSON);
            AnalyticsManager.LogError("get_player_gamepiece_error", response.Errors.JSON);
            profilePicture.sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(0);
        }
    }
}
