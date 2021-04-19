//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
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
        private const string kPracticeScreenOpened = "practiceScreenOpened";

        public ScrollRect player1select;
        public ScrollRect player2select;
        public ScrollRect areasContainer;
        public RectTransform gameboardParent;
        public RectTransform body;
        public float landscapeBodyHeight;

        public OpponentWidget opponentWidgetPrefab;
        public PracticeScreenAreaSelectWidget areaWidgetPrefab;
        public MiniGameboardWidget miniGameboardPrefab;
        public StringEventTrigger timerToggle;
        public ButtonExtended timerButton;

        private Dictionary<Area, List<GameBoardDefinition>> gameboards =
            new Dictionary<Area, List<GameBoardDefinition>>();
        private int currentBoard = -1;
        private MiniGameboardWidget currentGameboardWidget;

        public OpponentWidget player1Profile { get; private set; }
        public OpponentWidget player2Profile { get; private set; }
        public PracticeScreenAreaSelectWidget currentAreaWidget { get; private set; }

        protected override void Start()
        {
            foreach (GameBoardDefinition gameBoardDefinition in GameContentManager.Instance.passAndPlayBoards)
            {
                if (gameBoardDefinition.Area <= Area.NONE)
                {
                    gameBoardDefinition.Area = Area.TRAINING_GARDEN;
                }

                if (!gameboards.ContainsKey(gameBoardDefinition.Area))
                {
                    gameboards.Add(gameBoardDefinition.Area, new List<GameBoardDefinition>());
                }

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

            //load areas
            bool first = true;
            foreach (Serialized.AreasDataHolder.GameArea areaData in GameContentManager.Instance.enabledAreas)
            {
                if (first)
                {
                    first = false;
                    SetArea(AddAreaWidget(areaData.areaID));
                }
                else
                {
                    AddAreaWidget(areaData.areaID);
                }
            }

            LoadBoard(currentBoard);

            SetTimerState(SettingsManager.Get(SettingsManager.KEY_LOCAL_TIMER));

            //configure body
            if (GameManager.Instance.Landscape)
            {
                body.anchorMin = new Vector2(.5f, 0f);
                body.anchorMax = new Vector2(.5f, 1f);

                body.offsetMin = new Vector2(-landscapeBodyHeight * .5f, 0f);
                body.offsetMax = new Vector2(landscapeBodyHeight * .5f, 0f);
            }
            else
            {
                body.anchorMin = Vector2.zero;
                body.anchorMax = Vector2.one;

                body.offsetMin = body.offsetMax = Vector2.zero;
            }

            base.Start();
        }

        public override void Open()
        {
            base.Open();

            HeaderScreen.Instance.Close();

            //check if was opened
            if (!PlayerPrefsWrapper.GetBool(kPracticeScreenOpened))
            {
                PlayerPrefsWrapper.SetBool(kPracticeScreenOpened, true);

                ShowHelp();
            }
        }

        public void ShowHelp() =>
            menuController.GetOrAddScreen<PromptScreen>().Prompt(
                LocalizationManager.Value("practiceScreenPromptTitle"),
                LocalizationManager.Value("practiceScreenPromptDescription"),
                null,
                LocalizationManager.Value("ok"));

        public void ToggleTimer() => SetTimerState(!SettingsManager.Get(SettingsManager.KEY_LOCAL_TIMER));

        public void ToggleMagic() => SettingsManager.Toggle(SettingsManager.KEY_REALTIME_MAGIC);

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
            {
                SetAreaWidget(widget);
            }
        }

        public void NextBoard()
        {
            if (currentBoard + 1 == gameboards[currentAreaWidget.area].Count)
            {
                currentBoard = -1;
            }
            else
            {
                currentBoard++;
            }

            LoadBoard(currentBoard);
        }

        public void PreviousBoard()
        {
            if (currentBoard - 1 == -2)
            {
                currentBoard = gameboards[currentAreaWidget.area].Count - 1;
            }
            else
            {
                currentBoard--;
            }

            LoadBoard(currentBoard);
        }

        public void Play()
        {
            Player player1;
            if (player1Profile.aiProfile != AIProfile.Player)
            {
                player1 = new Player(1, player1Profile.aiProfile.ToString(), player1Profile.aiProfile)
                {
                    HerdId = player1Profile.prefabData.Id
                };
            }
            else
            {
                player1 = new Player(1, LocalizationManager.Value("player_one"))
                {
                    PlayerString = UserManager.Instance.userId,
                    HerdId = UserManager.Instance.gamePieceID
                };
            }

            Player player2;
            if (player2Profile.aiProfile != AIProfile.Player)
            {
                player2 = new Player(2, player2Profile.aiProfile.ToString(), player2Profile.aiProfile)
                {
                    HerdId = player2Profile.prefabData.Id
                };
            }
            else
            {
                if (player1.Profile == AIProfile.Player)
                {
                    player2 = new Player(2, LocalizationManager.Value("player_two"))
                    {
                        HerdId = GameContentManager.Instance.piecesDataHolder.random.Id
                    };
                }
                else
                {
                    player2 = new Player(2, LocalizationManager.Value("player_one"))
                    {
                        PlayerString = UserManager.Instance.userId,
                        HerdId = UserManager.Instance.gamePieceID
                    };
                }
            }

            GameType type = GameType.PASSANDPLAY;

            if (player1Profile.aiProfile != AIProfile.Player && player2Profile.aiProfile != AIProfile.Player)
            {
                type = GameType.PRESENTATION;
            }
            else if (player1Profile.aiProfile != AIProfile.Player || player2Profile.aiProfile != AIProfile.Player)
            {
                type = GameType.AI;
            }

            ClientFourzyGame game;
            if (currentBoard > -1)
            {
                game = new ClientFourzyGame(gameboards[currentAreaWidget.area][currentBoard], player1, player2)
                {
                    _Area = currentAreaWidget.area,
                    _Type = type,
                };
            }
            else
            {
                game = new ClientFourzyGame(currentAreaWidget.area, player1, player2, 1)
                {
                    _Type = type,
                };
            }

            game.UpdateFirstState();
            GameManager.Instance.StartGame(game, GameTypeLocal.LOCAL_GAME);
        }

        protected OpponentWidget AddWidget(AIProfile aiProfile, Transform parent, int playerIndex)
        {
            OpponentWidget instance = Instantiate(opponentWidgetPrefab, parent).SetData(aiProfile, playerIndex);
            instance.button.onTap += data => OnWidgetTap(instance);

            return instance;
        }

        protected PracticeScreenAreaSelectWidget AddAreaWidget(Area area)
        {
            PracticeScreenAreaSelectWidget instance = Instantiate(areaWidgetPrefab, areasContainer.content)
                .SetData(area);
            instance.button.onTap += data => SetArea(instance);

            return instance;
        }

        private void SetTimerState(bool value)
        {
            timerToggle.TryInvoke(value ? "on" : "off");
            SettingsManager.Set(SettingsManager.KEY_LOCAL_TIMER, value);
        }

        private void SetAreaWidget(PracticeScreenAreaSelectWidget widget)
        {
            currentAreaWidget = widget;
            currentAreaWidget.Select();

            if (currentGameboardWidget)
            {
                currentGameboardWidget.SetArea(currentAreaWidget.area);
            }

            currentBoard = -1;
            LoadBoard(currentBoard);
        }

        private void LoadBoard(int index)
        {
            if (currentGameboardWidget)
            {
                Destroy(currentGameboardWidget.gameObject);
            }

            MiniGameboardWidget gameboard = Instantiate(miniGameboardPrefab, gameboardParent);
            gameboard.ResetAnchors();
            gameboard.button.interactable = false;
            gameboard.SetArea(currentAreaWidget.area);

            if (index > -1)
            {
                gameboard.QuickLoadBoard(gameboards[currentAreaWidget.area][index]);
            }

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

            if (player1Profile && player2Profile)
            {
                timerButton.SetState(
                    player1Profile.aiProfile == AIProfile.Player &&
                    player2Profile.aiProfile == AIProfile.Player);
            }
        }
    }
}