//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PracticeScreen : MenuScreen
    {
        private const string kPracticeScreenOpened = "practiceScreenOpened";

        [SerializeField]
        private ScrollRect player1select;
        [SerializeField]
        private ScrollRect player2select;
        [SerializeField]
        private ScrollRect areasContainer;
        [SerializeField]
        private RectTransform gameboardParent;
        [SerializeField]
        private RectTransform body;
        [SerializeField]
        private float landscapeBodyHeight;

        [SerializeField]
        private OpponentWidget opponentWidgetPrefab;
        [SerializeField]
        private MiniGameboardWidget miniGameboardPrefab;
        [SerializeField]
        private StringEventTrigger timerToggle;
        [SerializeField]
        private ButtonExtended timerButton;
        [SerializeField]
        private ButtonExtended magicButton;
        [SerializeField]
        private ButtonExtended playButton;

        private Dictionary<Area, List<GameBoardDefinition>> gameboards = new Dictionary<Area, List<GameBoardDefinition>>();
        private int currentBoard = -1;
        private MiniGameboardWidget currentGameboardWidget;
        private List<PracticeScreenAreaSelectWidget> areaWidgets = new List<PracticeScreenAreaSelectWidget>();

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

            magicButton.SetActive(Constants.MAGIC_TOGGLE_ACTIVE_STATE);

            //load areas
            bool first = true;
            foreach (Serialized.AreasDataHolder.GameArea areaData in GameContentManager.Instance.areasDataHolder.areas)
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

            UserManager.onAreaProgression += OnAreaProgression;
            UserManager.onPlayfabValuesLoaded += OnPlayfabValueLoaded;
            OnAreaSet();
            UpdateAreasWidgets();

            base.Start();
        }

        private void OnAreaProgression(Area area, int progression)
        {
            OnAreaSet();
            UpdateAreasWidgets();
        }

        private void OnPlayfabValueLoaded(PlayfabValuesLoaded value)
        {
            if (value == PlayfabValuesLoaded.PLAYER_STATS_RECEIVED)
            {
                OnAreaSet();
                UpdateAreasWidgets();
            }
        }

        public override void Open()
        {
            if (!isOpened)
            {
                OnAreaSet(true);
            }

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
            if (currentAreaWidget && currentAreaWidget != widget)
            {
                currentAreaWidget.Deselect();
            }

            SetAreaWidget(widget);
        }

        public void NextBoard()
        {
            if (currentBoard + 1 == gameboards[currentAreaWidget.Area].Count)
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
                currentBoard = gameboards[currentAreaWidget.Area].Count - 1;
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
                    HerdId = UserManager.Instance.gamePieceId
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
                        HerdId = UserManager.Instance.gamePieceId
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
                game = new ClientFourzyGame(gameboards[currentAreaWidget.Area][currentBoard], player1, player2)
                {
                    _Area = currentAreaWidget.Area,
                    _Type = type,
                };
            }
            else
            {
                game = new ClientFourzyGame(currentAreaWidget.Area, player1, player2, 1)
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
            PracticeScreenAreaSelectWidget instance = Instantiate(GameContentManager.GetPrefab<PracticeScreenAreaSelectWidget>("AREA_SELECT_WIDGET_SMALL"), areasContainer.content).SetData(area, false);
            areaWidgets.Add(instance);
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

            OnAreaSet();

            if (currentGameboardWidget)
            {
                currentGameboardWidget.SetArea(currentAreaWidget.Area);
            }

            currentBoard = -1;
            LoadBoard(currentBoard);
        }

        private void OnAreaSet(bool forcePopup = false)
        {
            if (!currentAreaWidget)
            {
                return;
            }

            bool enabled;
            int gamesToPlay = 0;
            switch (currentAreaWidget.Area)
            {
                case Area.TRAINING_GARDEN:
                    gamesToPlay = Constants.UNLOCK_PRACTICE_TRAINING_GARDEN;

                    break;

                case Area.ENCHANTED_FOREST:
                    gamesToPlay = Constants.UNLOCK_PRACTICE_ENCHANTED_FOREST;

                    break;

                case Area.SANDY_ISLAND:
                    gamesToPlay = Constants.UNLOCK_PRACTICE_SANDY_ISLAND;

                    break;

                case Area.ICE_PALACE:
                    gamesToPlay = Constants.UNLOCK_PRACTICE_ICE_PALACE;

                    break;
            }

            int gamesPlayed = UserManager.Instance.GetAreaProgression(currentAreaWidget.Area);
            enabled = gamesPlayed >= gamesToPlay;

            if (!enabled && (isOpened || forcePopup))
            {
                string areaname = GameContentManager.Instance.areasDataHolder[currentAreaWidget.Area].Name;
                menuController.GetOrAddScreen<PromptScreen>().Prompt(
                    string.Format(LocalizationManager.Value("practice_locked_title"), areaname),
                    string.Format(LocalizationManager.Value("unlock_practice_message"), gamesToPlay - gamesPlayed, areaname),
                    null,
                    LocalizationManager.Value("ok"));
            }

            playButton.SetState(enabled);
        }

        private void UpdateAreasWidgets()
        {
            foreach (PracticeScreenAreaSelectWidget widget in areaWidgets)
            {
                switch (widget.Area)
                {
                    case Area.TRAINING_GARDEN:
                        widget.SetState(UserManager.Instance.GetAreaProgression(widget.Area) >= Constants.UNLOCK_PRACTICE_TRAINING_GARDEN, true);

                        break;

                    case Area.ENCHANTED_FOREST:
                        widget.SetState(UserManager.Instance.GetAreaProgression(widget.Area) >= Constants.UNLOCK_PRACTICE_ENCHANTED_FOREST, true);

                        break;

                    case Area.SANDY_ISLAND:
                        widget.SetState(UserManager.Instance.GetAreaProgression(widget.Area) >= Constants.UNLOCK_PRACTICE_SANDY_ISLAND, true);

                        break;

                    case Area.ICE_PALACE:
                        widget.SetState(UserManager.Instance.GetAreaProgression(widget.Area) >= Constants.UNLOCK_PRACTICE_ICE_PALACE, true);

                        break;
                }
            }
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
            gameboard.SetArea(currentAreaWidget.Area);

            if (index > -1)
            {
                gameboard.QuickLoadBoard(gameboards[currentAreaWidget.Area][index]);
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