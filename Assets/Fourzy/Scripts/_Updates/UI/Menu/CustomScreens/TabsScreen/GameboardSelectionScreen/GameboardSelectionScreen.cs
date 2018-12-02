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
                miniGameBoard.tokenBoard = board;
                miniGameBoard.CreateTokens();
                miniGameBoard.CreateGamePieces();

                miniGameBoard.toggle.group = toggleGroup;
            }
        }
    }
}