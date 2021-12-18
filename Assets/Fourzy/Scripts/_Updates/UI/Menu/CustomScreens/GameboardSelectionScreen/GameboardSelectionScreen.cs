//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public GameBoardDefinition data { get; private set; }

        private List<MiniGameboardWidget> gameboardWidgets;
        private int lastInstantiatedIndex = 0;

        private bool wasDisabled = false;
        private MiniGameboardWidget selected;

        protected void OnDisable()
        {
            wasDisabled = true;
        }

        protected void OnEnable()
        {
            if (wasDisabled)
            {
                wasDisabled = false;

                if (lastInstantiatedIndex < GameContentManager.Instance.passAndPlayBoards.Count - 1)
                {
                    StartRoutine(
                        "continueInitialize", 
                        gameboardWidgets[lastInstantiatedIndex].gameboardView.CreateBitsRoutine(false, true), () =>
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

            if (selected) selected.Deselect(.25f);

            selected = widget;
            selected.Select(.25f);
        }

        public void Play()
        {
            ClientFourzyGame game;

            Debug.Log("GameContentManager.Instance.currentTheme.themeID: " + GameContentManager.Instance.currentArea.areaID);
            Player player1 = new Player(1, "Player One");
            Player player2 = new Player(2, "Player Two");

            if (data != null)
            {
                game = new ClientFourzyGame(data, player1, player2);
            }
            else
            {
                game = new ClientFourzyGame(
                    GameContentManager.Instance.currentArea.areaID, 
                    player1, 
                    player2, 
                    player1.PlayerId);
            }

            game._Type = GameType.PASSANDPLAY;
            game.SetRandomActivePlayer();
            game.UpdateFirstState();

            GameManager.Instance.StartGame(game, GameTypeLocal.LOCAL_GAME);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            gameboardWidgets = new List<MiniGameboardWidget>();

            //spawn random one
            //select first one
            SetGame(Instantiate(miniGameboardPrefab, gameboardsParent));

            StartRoutine("initializeBoards", InstantiateBoards());
        }

        private IEnumerator InstantiateBoards()
        {
            while (lastInstantiatedIndex < GameContentManager.Instance.passAndPlayBoards.Count)
            {
                MiniGameboardWidget gameboard = Instantiate(miniGameboardPrefab, gameboardsParent);

                gameboard.HideBoard();
                gameboard.ShowSpinner();

                widgets.Add(gameboard);
                gameboardWidgets.Add(gameboard);

                yield return gameboard
                    .SetData(GameContentManager.Instance.passAndPlayBoards[lastInstantiatedIndex])
                    .CreateBitsRoutine(true, true);

                gameboardWidgets[lastInstantiatedIndex].FinishedLoading();

                lastInstantiatedIndex++;
            }
        }
    }
}