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

        private Dictionary<Area, List<GameBoardDefinition>> gameboards = new Dictionary<Area, List<GameBoardDefinition>>();
        private int currentBoard = -1;
        private MiniGameboardWidget currentGameboardWidget;

        public OpponentWidget player1Profile { get; private set; }
        public OpponentWidget player2Profile { get; private set; }
        public PracticeScreenAreaSelectWidget currentAreaWidget { get; private set; }
        public bool timerState { get; private set; }

        protected override void Start()
        {
            foreach (GameBoardDefinition gameBoardDefinition in GameContentManager.Instance.passAndPlayGameboards)
            {
                if (gameBoardDefinition.Area <= Area.NONE) gameBoardDefinition.Area = Area.TRAINING_GARDEN;

                if (!gameboards.ContainsKey(gameBoardDefinition.Area)) gameboards.Add(gameBoardDefinition.Area, new List<GameBoardDefinition>());
                gameboards[gameBoardDefinition.Area].Add(gameBoardDefinition);
            }

            //load profiles
            OnWidgetTap(AddWidget(AIProfile.Player, player1select.content, 1));
            AddWidget(AIProfile.SimpleAI, player1select.content, 1);
            AddWidget(AIProfile.EasyAI, player1select.content, 1);
            AddWidget(AIProfile.AggressiveAI, player1select.content, 1);

            OnWidgetTap(AddWidget(AIProfile.Player, player2select.content, 2));
            AddWidget(AIProfile.EasyAI, player2select.content, 2);
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
            if (currentBoard + 1 == gameboards[currentAreaWidget.area].Count) currentBoard = -1;
            else currentBoard++;

            LoadBoard(currentBoard);
        }

        public void PreviousBoard()
        {
            if (currentBoard - 1 == -2) currentBoard = gameboards[currentAreaWidget.area].Count - 1;
            else currentBoard--;

            LoadBoard(currentBoard);
        }

        public void Play()
        {
            Player player1;
            if (player1Profile.aiProfile != AIProfile.Player)
                player1 = new Player(1, player1Profile.aiProfile.ToString(), player1Profile.aiProfile) { HerdId = player1Profile.prefabData.data.ID };
            else
                player1 = new Player(1, LocalizationManager.Value("player_one")) { PlayerString = UserManager.Instance.userId };

            Player player2;
            if (player2Profile.aiProfile != AIProfile.Player)
                player2 = new Player(2, player2Profile.aiProfile.ToString(), player2Profile.aiProfile) { HerdId = player2Profile.prefabData.data.ID };
            else
            {
                if (player1.Profile == AIProfile.Player)
                    player2 = new Player(2, LocalizationManager.Value("player_two"));
                else
                    player2 = new Player(2, LocalizationManager.Value("player_one")) { PlayerString = UserManager.Instance.userId };
            }

            GameType type = GameType.PASSANDPLAY;

            if (player1Profile.aiProfile != AIProfile.Player && player2Profile.aiProfile != AIProfile.Player) type = GameType.PRESENTATION;
            else if (player1Profile.aiProfile != AIProfile.Player || player2Profile.aiProfile != AIProfile.Player) type = GameType.AI;

            ClientFourzyGame game;
            if (currentBoard > -1)
                game = new ClientFourzyGame(gameboards[currentAreaWidget.area][currentBoard], player1, player2)
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

            currentBoard = -1;
            LoadBoard(currentBoard);
        }

        private void LoadBoard(int index)
        {
            if (currentGameboardWidget) Destroy(currentGameboardWidget.gameObject);

            MiniGameboardWidget gameboard = Instantiate(miniGameboardPrefab, gameboardParent);
            gameboard.ResetAnchors();
            gameboard.button.interactable = false;
            gameboard.SetArea(currentAreaWidget.area);

            if (index > -1) gameboard.QuickLoadBoard(gameboards[currentAreaWidget.area][index]);

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