//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    /// <summary>
    /// One of the 'heavy' screens, needs a routine to initialize all gameboards
    /// Also since it don't listen to any updates of those boards it can have them 'disabled' when not opened
    /// </summary>
    public class GameboardSelectionScreen : MenuScreen
    {
        public RectTransform gameboardsParent;
        public MiniGameboardWidget miniGameboardPrefab;

        public ToggleGroup toggleGroup { get; private set; }
        public GameBoardDefinition data { get; private set; }

        private List<GameBoardDefinition> gameboards;
        private List<MiniGameboardWidget> gameboardWidgets;
        private int lastInstantiatedIndex = 0;

        private bool wasDisabled = false;
        private MiniGameboardWidget selected;

        protected override void Awake()
        {
            base.Awake();

            toggleGroup = GetComponent<ToggleGroup>();
        }

        protected void OnDisable()
        {
            wasDisabled = true;
        }

        protected void OnEnable()
        {
            if (wasDisabled)
            {
                wasDisabled = false;

                if (lastInstantiatedIndex < gameboards.Count - 1)
                {
                    StartRoutine("continueInitialize", gameboardWidgets[lastInstantiatedIndex].gameboardView.CreateBitsRoutine(false, true), () =>
                    {
                        gameboardWidgets[lastInstantiatedIndex].FinishedLoading();

                        lastInstantiatedIndex++;

                        CancelRoutine("initializeBoards");
                        StartRoutine("initializeBoards", InstantiateBoards());
                    }, null);
                }
            }
        }

        public void SetGame(MiniGameboardWidget widget)
        {
            if (selected && selected == widget) return;

            data = widget.data;

            if (selected) selected.Deselect();

            selected = widget;
            selected.Select();
        }

        public void Play()
        {
            ClientFourzyGame game;

            UnityEngine.Debug.Log("GameContentManager.Instance.currentTheme.themeID: " + GameContentManager.Instance.currentTheme.themeID);
            Player player1 = new Player(1, "Player One");
            Player player2 = new Player(2, "Player Two");

            if (data != null)
                game = new ClientFourzyGame(data, player1, player2);
            else
                game = new ClientFourzyGame(GameContentManager.Instance.currentTheme.themeID, player1, player2, player1.PlayerId);

            game._Type = GameType.PASSANDPLAY;

            GameManager.Instance.StartGame(game);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            gameboards = GameContentManager.Instance.passAndPlayGameboards;
            gameboardWidgets = new List<MiniGameboardWidget>();

            //spawn random one
            //select first one
            SetGame(Instantiate(miniGameboardPrefab, gameboardsParent));

            StartRoutine("initializeBoards", InstantiateBoards());
        }

        private IEnumerator InstantiateBoards()
        {
            while (lastInstantiatedIndex < gameboards.Count)
            {
                MiniGameboardWidget gameboard = Instantiate(miniGameboardPrefab, gameboardsParent);

                gameboard.HideBoard();
                gameboard.ShowSpinner();

                widgets.Add(gameboard);
                gameboardWidgets.Add(gameboard);

                yield return gameboard.SetData(gameboards[lastInstantiatedIndex]).CreateBitsRoutine(true, true);

                gameboardWidgets[lastInstantiatedIndex].FinishedLoading();

                lastInstantiatedIndex++;
            }
        }
    }
}