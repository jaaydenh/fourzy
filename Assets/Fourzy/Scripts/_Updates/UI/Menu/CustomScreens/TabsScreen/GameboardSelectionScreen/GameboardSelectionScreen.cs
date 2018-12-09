//@vadym udod

using Fourzy._Updates.UI.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GameboardSelectionScreen : MenuScreen
    {
        public GridLayoutGroup gameboardGrid;
        public MiniGameboardWidget miniGameboardPrefab;

        public ToggleGroup toggleGroup { get; private set; }

        private GameType gameType;
        private Opponent opponent;

        protected override void Awake()
        {
            base.Awake();

            toggleGroup = GetComponent<ToggleGroup>();
        }

        protected override void Start()
        {
            base.Start();

            //gen board
            TokenBoard[] boards = TokenBoardLoader.Instance.GetTokenBoardsForBoardSelection();

            foreach (Transform board in gameboardGrid.transform)
                Destroy(board.gameObject);
            
            MiniGameboardWidget random = Instantiate(miniGameboardPrefab, gameboardGrid.transform);
            random.SetAsRandom();
            random.toggle.group = toggleGroup;
            random.toggle.isOn = true;

            foreach (var board in boards)
            {
                MiniGameboardWidget miniGameBoard = Instantiate(miniGameboardPrefab, gameboardGrid.transform);
                miniGameBoard.SetData(board);

                miniGameBoard.toggle.group = toggleGroup;
            }
        }

        public void SelectBoard(GameType gameType, string opponentId = "", string opponentName = "", Image opponentProfilePicture = null, string opponentLeaderboardRank = "")
        {
            menuController.OpenScreen(this);

            if (opponentId != "")
                ChallengeManager.Instance.GetOpponentGamePiece(opponentId);

            this.gameType = gameType;

            opponent = new Opponent(opponentId, opponentName, "");
            opponent.opponentLeaderboardRank = opponentLeaderboardRank;
        }

        public void PlayButton()
        {
            GameManager.Instance.OpenNewGame(gameType, opponent, true, null);
        }
    }
}