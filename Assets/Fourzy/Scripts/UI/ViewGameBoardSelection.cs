using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class ViewGameBoardSelection : UIView
    {
        public static ViewGameBoardSelection instance;
        private GameType gameType;
        private Opponent opponent;

        [SerializeField]
        private Transform gameboardGrid;

        [SerializeField]
        private MiniGameBoard miniBoardPrefab;

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

        public void PlayButton() 
        {
            Hide();
            this.ClearMiniBoards();
            GameManager.instance.OpenNewGame(gameType, opponent, true, null);
        }

        public void BackButton()
        {
            Debug.Log("viewgameboardselection current view: " + ViewController.instance.GetCurrentView().name);
            Hide();
            if (ViewController.instance.GetCurrentView() != null)
            {
                if (ViewController.instance.GetCurrentView().GetType() != typeof(ViewPuzzleSelection))
                {
                    ViewController.instance.ShowTabView();
                }

                ViewController.instance.ChangeView(ViewController.instance.GetCurrentView());
            }
            else
            {
                ViewController.instance.ShowTabView();
                ViewController.instance.ChangeView(ViewController.instance.view3);
            }

            this.ClearMiniBoards();
        }

        public void TransitionToViewGameBoardSelection(GameType gameType, string opponentId = "", string opponentName = "", Image opponentProfilePicture = null, string opponentLeaderboardRank = "")
        {
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

            this.LoadMiniBoards();
        }

        private void LoadMiniBoards()
        {
            Debug.Log("LoadMiniBoards");
            TokenBoard[] boards = TokenBoardLoader.instance.GetTokenBoardsForBoardSelection();

            ClearMiniBoards();

            // Create Random Miniboard
            MiniGameBoard random = Instantiate(miniBoardPrefab);
            random.SetAsRandom();
            random.transform.SetParent(gameboardGrid, false);

            var toggler = random.GetComponentInChildren<Toggle>();
            ToggleGroup toggleGroup = gameboardGrid.GetComponent<ToggleGroup>();
            toggler.group = toggleGroup;
            toggler.isOn = true;

            foreach (var board in boards)
            {
                MiniGameBoard miniGameBoard = Instantiate(miniBoardPrefab);
                miniGameBoard.tokenBoard = board;
                miniGameBoard.CreateTokens();
                miniGameBoard.CreateGamePieces();
                miniGameBoard.transform.SetParent(gameboardGrid, false);

                var toggle = miniGameBoard.GetComponentInChildren<Toggle>();
                toggle.group = toggleGroup;
            }
        }

        private void ClearMiniBoards()
        {
            foreach(Transform board in gameboardGrid)
            {
                Destroy(board.gameObject);
            }
        }
    }
}
