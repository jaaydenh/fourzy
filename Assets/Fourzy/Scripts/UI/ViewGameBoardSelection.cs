using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class ViewGameBoardSelection : UIView
    {
        public static ViewGameBoardSelection instance;
        private GameType gameType;
        private Opponent opponent;

        void Start()
        {
            instance = this;
            keepHistory = false;
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void PlayButton() {
            Hide();
            GameManager.instance.OpenNewGame(gameType, opponent, true, null);
        }

        public void BackButton()
        {
            Debug.Log("viewgameboardselection current view: " + ViewController.instance.GetCurrentView().name);
            Hide();
            if (ViewController.instance.GetCurrentView() != null) {
                if (ViewController.instance.GetCurrentView().GetType() != typeof(ViewTraining))
                {
                    ViewController.instance.ShowTabView();
                }

                ViewController.instance.ChangeView(ViewController.instance.GetCurrentView());
            } else {
                ViewController.instance.ShowTabView();
                ViewController.instance.ChangeView(ViewController.instance.view3);
            }
        }

        public void TransitionToViewGameBoardSelection(GameType gameType, string opponentId = "", string opponentName = "", Image opponentProfilePicture = null, string opponentLeaderboardRank = "")
        {
            // challengerGamePieceId = 0;
            // challengedGamePieceId = 0;

            // challengerGamePieceId = UserManager.instance.gamePieceId;
            if (opponentId != "") {
                ChallengeManager.instance.GetOpponentGamePiece(opponentId);
            }

            // GameManager.instance.ResetUIGameScreen();
            // challengeInstanceId = null;
            this.gameType = gameType;

            this.opponent = new Opponent(opponentId, opponentName, "");
            
            // GameManager.instance.opponentUserId = opponentId;
            // GameManager.instance.opponentNameLabel.text = opponentName;

            // if (opponentProfilePicture != null)
            // {
            //     GameManager.instance.opponentProfilePicture.sprite = opponentProfilePicture.sprite;
            // }
            this.opponent.opponentLeaderboardRank = opponentLeaderboardRank;

            BoardSelectionManager.instance.LoadMiniBoards();
        }
    }
}
