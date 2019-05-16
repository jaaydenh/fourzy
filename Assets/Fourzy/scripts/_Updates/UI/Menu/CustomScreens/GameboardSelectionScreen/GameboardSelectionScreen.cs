//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GameboardSelectionScreen : MenuScreen
    {
        public RectTransform gameboardsParent;
        public MiniGameboardWidget miniGameboardPrefab;

        public ToggleGroup toggleGroup { get; private set; }
        public GameBoardDefinition data { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            toggleGroup = GetComponent<ToggleGroup>();
        }

        protected override void Start()
        {
            base.Start();

            foreach (Transform board in gameboardsParent)
                Destroy(board.gameObject);

            //spawn random one
            MiniGameboardWidget gameboard = Instantiate(miniGameboardPrefab, gameboardsParent);
            gameboard.toggle.group = toggleGroup;
            gameboard.toggle.isOn = true;

            //and the rest
            foreach (GameBoardDefinition gameboardDefinition in GameContentManager.Instance.passAndPlayGameboards)
            {
                gameboard = Instantiate(miniGameboardPrefab, gameboardsParent);
                gameboard.SetData(gameboardDefinition);
                gameboard.toggle.group = toggleGroup;
            }
        }

        public void SetGame(GameBoardDefinition data)
        {
            this.data = data;
        }

        public void Play()
        {
            ClientFourzyGame game;
            Debug.Log("GameContentManager.Instance.currentTheme.themeID: " + GameContentManager.Instance.currentTheme.themeID);
            if (data != null)
                game = new ClientFourzyGame(data, UserManager.Instance.meAsPlayer, new Player(2, "Player Two"));
            else
                game = new ClientFourzyGame(
                    GameContentManager.Instance.currentTheme.themeID, 
                    UserManager.Instance.meAsPlayer, new Player(2, "Player Two"), 
                    UserManager.Instance.meAsPlayer.PlayerId);

            game._Type = GameType.PASSANDPLAY;

            GameManager.Instance.StartGame(game);
        }
    }
}