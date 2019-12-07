//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PracticeScreen : MenuScreen
    {
        public ScrollRect player1select;
        public ScrollRect player2select;
        public ScrollRect areasContainer;
        public RectTransform gameboardParent;

        public OpponentWidget opponentWidgetPrefab;
        public PracticeScreenAreaSelectWidget areaWidgetPrefab;
        public StringEventTrigger timerToggle;
        public MiniGameboardWidget miniGameboardPrefab;

        private List<GameBoardDefinition> gameboards;
        private int currentBoard = -1;
        private MiniGameboardWidget currentGameboardWidget;

        public OpponentWidget player1Profile { get; private set; }
        public OpponentWidget player2Profile { get; private set; }
        public PracticeScreenAreaSelectWidget currentAreaWidget { get; private set; }
        public bool timerState { get; private set; }

        protected override void Start()
        {
            gameboards = GameContentManager.Instance.passAndPlayGameboards;

            //load profiles
            OnWidgetTap(AddWidget(AIProfile.Player, player1select.content, 1));
            AddWidget(AIProfile.SimpleAI, player1select.content, 1);
            AddWidget(AIProfile.EasyAI, player1select.content, 1);
            AddWidget(AIProfile.AggressiveAI, player1select.content, 1);

            OnWidgetTap(AddWidget(AIProfile.EasyAI, player2select.content, 2));
            AddWidget(AIProfile.Player, player2select.content, 2);
            AddWidget(AIProfile.SimpleAI, player2select.content, 2);
            AddWidget(AIProfile.AggressiveAI, player2select.content, 2);

            timerToggle.TryInvoke(timerState ? "on" : "off");

            //load areas
            SetArea(AddAreaWidget(Area.TRAINING_GARDEN));
            AddAreaWidget(Area.ENCHANTED_FOREST);
            AddAreaWidget(Area.SANDY_ISLAND);
            AddAreaWidget(Area.ICE_PALACE);

            LoadBoard(currentBoard);

            base.Start();
        }

        public override void Open()
        {
            base.Open();

            HeaderScreen.instance.Close();
        }

        public void ToggleTimer()
        {
            timerState = !timerState;
            timerToggle.TryInvoke(timerState ? "on" : "off");
        }

        public void SetArea(PracticeScreenAreaSelectWidget widget)
        {
            if (currentAreaWidget)
            {
                if (currentAreaWidget != widget)
                {
                    currentAreaWidget.Deselect();
                    SetAreaWidget(widget);
                }
            }
            else
                SetAreaWidget(widget);
        }

        public void NextBoard()
        {
            if (currentBoard + 1 == gameboards.Count) currentBoard = -1;
            else currentBoard++;

            LoadBoard(currentBoard);
        }

        public void PreviousBoard()
        {
            if (currentBoard - 1 == -2) currentBoard = gameboards.Count - 1;
            else currentBoard--;

            LoadBoard(currentBoard);
        }

        public void Play()
        {
            Player player1 = new Player(1, player1Profile.aiProfile.ToString(), player1Profile.aiProfile);
            if (player1Profile.aiProfile != AIProfile.Player) player1.HerdId = player1Profile.prefabData.data.ID;

            Player player2 = new Player(2, player2Profile.aiProfile.ToString(), player2Profile.aiProfile);
            if (player2Profile.aiProfile != AIProfile.Player) player2.HerdId = player2Profile.prefabData.data.ID;

            GameType type = GameType.PASSANDPLAY;

            if (player1Profile.aiProfile != AIProfile.Player && player2Profile.aiProfile != AIProfile.Player) type = GameType.PRESENTATION;
            else if (player1Profile.aiProfile != AIProfile.Player || player2Profile.aiProfile != AIProfile.Player) type = GameType.AI;

            ClientFourzyGame game;
            if (currentBoard > -1)
                game = new ClientFourzyGame(gameboards[currentBoard], player1, player2)
                {
                    _Area = currentAreaWidget.area,
                    _Type = type,
                };
            else
                game = new ClientFourzyGame(currentAreaWidget.area, player1, player2, 1)
                {
                    _Type = type,
                };

            GameManager.Instance.StartGame(game);
        }

        protected OpponentWidget AddWidget(AIProfile aiProfile, Transform parent, int playerIndex)
        {
            OpponentWidget instance = Instantiate(opponentWidgetPrefab, parent).SetData(aiProfile, playerIndex);
            instance.button.onTap += () => OnWidgetTap(instance);

            return instance;
        }

        protected PracticeScreenAreaSelectWidget AddAreaWidget(Area area)
        {
            PracticeScreenAreaSelectWidget instance = Instantiate(areaWidgetPrefab, areasContainer.content).SetData(area);
            instance.button.onTap += () => SetArea(instance);

            return instance;
        }

        private void SetAreaWidget(PracticeScreenAreaSelectWidget widget)
        {
            currentAreaWidget = widget;
            currentAreaWidget.Select();
            if (currentGameboardWidget) currentGameboardWidget.SetArea(currentAreaWidget.area);
        }

        private void LoadBoard(int index)
        {
            if (currentGameboardWidget) Destroy(currentGameboardWidget.gameObject);

            MiniGameboardWidget gameboard = Instantiate(miniGameboardPrefab, gameboardParent);
            gameboard.ResetAnchors();
            gameboard.button.interactable = false;
            gameboard.SetArea(currentAreaWidget.area);

            if (index > -1) gameboard.QuickLoadBoard(gameboards[index]);

            currentGameboardWidget = gameboard;
        }

        private void OnWidgetTap(OpponentWidget widget)
        {
            switch (widget.playerID)
            {
                case 1:
                    if (player1Profile)
                    {
                        if (player1Profile != widget)
                        {
                            player1Profile.Deselect();
                            player1Profile = widget;
                            widget.Select();
                        }
                    }
                    else
                    {
                        player1Profile = widget;
                        widget.Select();
                    }

                    break;

                case 2:
                    if (player2Profile)
                    {
                        if (player2Profile != widget)
                        {
                            player2Profile.Deselect();
                            player2Profile = widget;
                            widget.Select();
                        }
                    }
                    else
                    {
                        player2Profile = widget;
                        widget.Select();
                    }

                    break;
            }
        }
    }
}