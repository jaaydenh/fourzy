//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class VSLandscapeScreen : MenuScreen
    {
        public FlowLayoutGroup gamepiecesParent;
        public VSScreenReadyButton readyButton;
        public VSScreenToggle timerToggle;
        public MiniGameboardWidget selectedBoardWidget;
        public ButtonExtended areaPicker;
        public ButtonExtended p1Button;
        public ButtonExtended p2Button;
        public VSScreenDifficultyDropdown difficultyDropdown;
        public Sprite randomAreaIcon;
        public VSScreenPlayerWidget[] profiles;
        public int optimalColumnCount = 5;
        public int maxGridWidth = 2000;

        private List<GamePieceWidgetLandscape> gamePieceWidgets = new List<GamePieceWidgetLandscape>();
        private IEnumerable<GamePiecePrefabData> unlockedGamepieces;
        private GamePieceWidgetLandscape widgetPrefab;
        private RectTransform gamePiecesRectTransform;
        private ButtonExtended selectedBoardWidgetButton;

        private List<GamePieceWidgetLandscape> selectedPlayers = new List<GamePieceWidgetLandscape>();

        private GameBoardDefinition gameBoardDefinition;
        private ThemesDataHolder.GameTheme selectedTheme;
        private ThemesDataHolder.GameTheme prevTheme;

        private int p1DifficultyLevel = -1;
        private int p2DifficultyLevel = -1;
        private int demoCounter = 0;

        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public Vector2 WidgetSize { get; private set; }

        protected void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
            {
                //toggle demo mode
                SettingsManager.Toggle(SettingsManager.KEY_DEMO_MODE);
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 viewportPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);

                if (viewportPoint.x > .9f && viewportPoint.y > .9f)
                {
                    demoCounter++;
                    if (demoCounter == 5)
                    {
                        demoCounter = 0;

                        //toggle demo mode
                        SettingsManager.Toggle(SettingsManager.KEY_DEMO_MODE);
                    }
                }
                else
                    demoCounter = 0;
            }
        }

        /// <summary>
        /// Invoked from ReadyButton
        /// </summary>
        public void StartGame()
        {
            Player player1 = UserManager.Instance.meAsPlayer;
            if (selectedPlayers.Count > 0) player1.HerdId = selectedPlayers[0].data.ID;

            Player player2 = new Player(2, "Player Two");
            if (selectedPlayers.Count > 1) player2.HerdId = selectedPlayers[1].data.ID;
            if (p2DifficultyLevel > -1) player2.Profile = AIPlayerFactory.RandomProfile((AIDifficulty)p2DifficultyLevel);

            ClientFourzyGame game;
            if (gameBoardDefinition == null)
                game = new ClientFourzyGame(selectedTheme == null ? GameContentManager.Instance.enabledThemes.Random().themeID : selectedTheme.themeID, player1, player2, player1.PlayerId);
            else
                game = new ClientFourzyGame(gameBoardDefinition, player1, player2);

            if (p1DifficultyLevel > -1 && p2DifficultyLevel > -1) game._Type = GameType.PRESENTATION;
            else if (p1DifficultyLevel > -1 || p2DifficultyLevel > -1) game._Type = GameType.AI;
            else game._Type = GameType.PASSANDPLAY;

            GameManager.Instance.StartGame(game);
        }

        public override void Open()
        {
            base.Open();

            areaPicker.image.sprite = selectedTheme == null ? randomAreaIcon : selectedTheme.landscapePreview;
            areaPicker.SetLabel(LocalizationManager.Value(selectedTheme == null ? "random" : selectedTheme.id));

            if (selectedTheme != null && prevTheme != selectedTheme)
            {
                //new theme selected
                selectedBoardWidget.Clear();
                selectedBoardWidgetButton.SetState(true);

                gameBoardDefinition = null;
            }
            else if (selectedTheme == null)
            {
                //cant select board
                selectedBoardWidget.Clear();
                selectedBoardWidgetButton.SetState(false);

                gameBoardDefinition = null;
            }

            //get board
            selectedBoardWidget.SetData(gameBoardDefinition, false);

            prevTheme = selectedTheme;
        }

        public override void OnBack()
        {
            base.OnBack();

            if (selectedPlayers.Count() > 0)
            {
                GamePieceWidgetLandscape last = selectedPlayers.Last();

                switch (selectedPlayers.Count())
                {
                    case 2:
                        last.SelectAsPlayer(selectedPlayers[0] == last ? 0 : -1);

                        selectedPlayers.Remove(last);

                        break;

                    case 1:
                        last.SelectAsPlayer(-1);

                        selectedPlayers.Remove(last);

                        break;
                }

                readyButton.SetState(selectedPlayers.Count == 2);
                UpdateProfiles();
            }
            else
                BackToRoot();
        }

        public void PickBoard() => menuController.GetScreen<LandscapeGameboardSelectionScreen>().Open(selectedTheme.themeID);

        public void OpenP2Difficulty()
        {
            if (p2DifficultyLevel > -1)
            {
                p2Button.SetLabel("P2");
                p2DifficultyLevel = -1;
                profiles[1].DisplayDifficulty(-1);
            }
            else
                difficultyDropdown.Open(false).SetPosition(profiles[1].transform, Vector2.zero).SetOnClick(OnP2DifficultySelected);
        }

        public void OpenP1Difficulty()
        {
            if (p1DifficultyLevel > -1)
            {
                p1Button.SetLabel("P1");
                p1DifficultyLevel = -1;
                profiles[0].DisplayDifficulty(-1);
            }
            else
                difficultyDropdown.Open(true).SetPosition(profiles[0].transform, Vector2.zero).SetOnClick(OnP1DifficultySelected);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            gamePiecesRectTransform = gamepiecesParent.GetComponent<RectTransform>();
            widgetPrefab = GameContentManager.GetPrefab<GamePieceWidgetLandscape>(GameContentManager.PrefabType.GAME_PIECE_LANDSCAPE);
            WidgetSize = widgetPrefab.GetComponent<RectTransform>().rect.size;
            selectedBoardWidgetButton = selectedBoardWidget.GetComponent<ButtonExtended>();

            //listen to board select screen
            menuController.GetScreen<LandscapeGameboardSelectionScreen>().onBoardSelected += OnBoardSelected;
            menuController.GetScreen<AreaSelectLandscapeScreen>().onAreaSeleected += OnAreaSelected;

            //set default ready button state
            readyButton.SetState(false);

            CreatePieces();

            timerToggle.SetState(SettingsManager.Get(SettingsManager.KEY_PASS_N_PLAY_TIMER), false);
            timerToggle.onValueSet += TimerToggleValueSet;
        }

        private void CreatePieces()
        {
            if (!initialized) return;

            //remove prev
            foreach (GamePieceWidgetLandscape gamePieceWidgetLandscape in gamePieceWidgets) Destroy(gamePieceWidgetLandscape.gameObject);
            gamePieceWidgets.Clear();

            unlockedGamepieces = GameContentManager.Instance.piecesDataHolder.gamePieces.list.Where(_widget => _widget.data.profilePicture);
            //int piecesCount = testPieces;
            int piecesCount = unlockedGamepieces.Count();

            Columns = optimalColumnCount;
            Rows = 1;

            while (piecesCount > Columns * Rows)
            {
                if (piecesCount > Columns * Rows)
                    Rows++;
                else
                    break;

                if (piecesCount > Columns * Rows)
                    Columns++;
                else
                    break;
            }

            float _maxGridSize = maxGridWidth > rectTransform.rect.width ? rectTransform.rect.width - 100f : maxGridWidth;
            gamePiecesRectTransform.sizeDelta = new Vector2((WidgetSize.x * Columns) + (gamepiecesParent.SpacingX * (Columns - 1)), (WidgetSize.y * Rows) + (gamepiecesParent.SpacingY * (Rows - 1)));
            if (gamePiecesRectTransform.sizeDelta.x > _maxGridSize)
                gamePiecesRectTransform.localScale = new Vector3(_maxGridSize / gamePiecesRectTransform.sizeDelta.x, _maxGridSize / gamePiecesRectTransform.sizeDelta.x);

            //only go though unlocked pieces
            //for (int testIndex = 0; testIndex < piecesCount; testIndex++)
            foreach (GamePiecePrefabData prefabData in unlockedGamepieces)
            {
                GamePieceWidgetLandscape widget = GameContentManager.InstantiatePrefab<GamePieceWidgetLandscape>(GameContentManager.PrefabType.GAME_PIECE_LANDSCAPE, gamePiecesRectTransform);

                widget
                    .SetData(prefabData.data)
                    //.SetData(GameContentManager.Instance.piecesDataHolder.gamePieces.list[testIndex % GameContentManager.Instance.piecesDataHolder.gamePieces.list.Count].data)
                    .SetOnClick(OnGPSelected);

                widgets.Add(widget);
                gamePieceWidgets.Add(widget);
            }
        }

        private void UpdateProfiles()
        {
            for (int profileIndex = 0; profileIndex < profiles.Length; profileIndex++)
                profiles[profileIndex].SetData(selectedPlayers.Count > profileIndex ? selectedPlayers[profileIndex].data : null);
        }

        private void OnGPSelected(GamePieceWidgetLandscape gm)
        {
            if (selectedPlayers.Count == 2)
            {
                if (selectedPlayers[1] == selectedPlayers[0]) selectedPlayers[1].SelectAsPlayer(0);
                else selectedPlayers[1].SelectAsPlayer(-1);

                selectedPlayers[1] = gm;

                if (selectedPlayers[1] == selectedPlayers[0]) selectedPlayers[1].SelectAsPlayer(0, 1);
                else selectedPlayers[1].SelectAsPlayer(1);
            }
            else
            {
                if (selectedPlayers.Count == 0)
                    gm.SelectAsPlayer(0);
                else
                {
                    if (selectedPlayers[0] == gm)
                        gm.SelectAsPlayer(0, 1);
                    else
                        gm.SelectAsPlayer(1);
                }

                selectedPlayers.Add(gm);
            }

            readyButton.SetState(selectedPlayers.Count == 2);
            UpdateProfiles();
        }

        private void TimerToggleValueSet(bool value)
        {
            SettingsManager.Set(SettingsManager.KEY_PASS_N_PLAY_TIMER, value);
        }

        private void OnBoardSelected(GameBoardDefinition boardDefinition)
        {
            gameBoardDefinition = boardDefinition;
        }

        private void OnAreaSelected(ThemesDataHolder.GameTheme areaData)
        {
            selectedTheme = areaData;
        }

        private void OnP1DifficultySelected(int option)
        {
            //not applicable to p1 yet
        }

        private void OnP2DifficultySelected(int option)
        {
            p2Button.SetLabel("CPU");
            p2DifficultyLevel = option;

            profiles[1].DisplayDifficulty(option);
        }

        public enum CURRENT_VS_STAGE
        {
            P1_SELECT,
            P2_SELECT,
            READY,
        }
    }
}