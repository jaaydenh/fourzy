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
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class VSLandscapeScreen : MenuScreen
    {
        public FlowLayoutGroup gamepiecesParent;
        public VSScreenReadyButton readyButton;
        public ButtonExtended timerToggle;
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

        private GameBoardDefinition gameBoardDefinition;
        private AreasDataHolder.GameArea selectedArea;
        private AreasDataHolder.GameArea prevArea;

        private int demoCounter = 0;

        public GamePieceWidgetLandscape[] selectedPlayers { get; private set; } = new GamePieceWidgetLandscape[2];
        public int p1DifficultyLevel { get; private set; } = -1;
        public int p2DifficultyLevel { get; private set; } = -1;

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

        protected override void Start()
        {
            base.Start();

            timerToggle.SetState(p2DifficultyLevel < 0);
        }

        /// <summary>
        /// Invoked from ReadyButton
        /// </summary>
        public void StartGame()
        {
            string herdId;
            string displaName;
            int magic = 100;

            Player player1 = UserManager.Instance.meAsPlayer;
            if (selectedPlayers[0] != null)
            {
                if (selectedPlayers[0].data != null)
                {
                    herdId = selectedPlayers[0].data.ID;
                    displaName = selectedPlayers[0].data.name;
                    magic = selectedPlayers[0].data.startingMagic;
                }
                else
                {
                    GamePieceData random = unlockedGamepieces.Random().data;
                    herdId = random.ID;
                    displaName = random.name;
                    magic = random.startingMagic;
                }

                player1.HerdId = herdId;
                player1.DisplayName = displaName;
                player1.Magic = magic;
            }

            Player player2 = new Player(2, "Player Two");
            if (selectedPlayers[1] != null)
            {
                if (selectedPlayers[1].data != null)
                {
                    herdId = selectedPlayers[1].data.ID;
                    displaName = selectedPlayers[1].data.name;
                    magic = selectedPlayers[1].data.startingMagic;
                }
                else
                {
                    GamePieceData random = unlockedGamepieces.Random().data;
                    herdId = random.ID;
                    displaName = random.name;
                    magic = random.startingMagic;
                }

                player2.HerdId = herdId;
                player2.DisplayName = displaName;
                player2.Magic = magic;
            }

            if (p2DifficultyLevel > -1) player2.Profile = AIPlayerFactory.RandomProfile((AIDifficulty)p2DifficultyLevel);

            ClientFourzyGame game;
            if (gameBoardDefinition == null)
                game = new ClientFourzyGame(selectedArea == null ? GameContentManager.Instance.enabledAreas.Random().areaID : selectedArea.areaID, player1, player2, player1.PlayerId);
            else
                game = new ClientFourzyGame(gameBoardDefinition, player1, player2);

            if (p1DifficultyLevel > -1 && p2DifficultyLevel > -1) game._Type = GameType.PRESENTATION;
            else if (p1DifficultyLevel > -1 || p2DifficultyLevel > -1) game._Type = GameType.AI;
            else game._Type = GameType.PASSANDPLAY;

            GameManager.Instance.StartGame(game, GameTypeLocal.LOCAL_GAME);
        }

        public override void Open()
        {
            base.Open();

            areaPicker.image.sprite = selectedArea == null ? randomAreaIcon : selectedArea._4X3;
            areaPicker.SetLabel(LocalizationManager.Value(selectedArea == null ? "random" : selectedArea.name));

            if (selectedArea != null && prevArea != selectedArea)
            {
                //new theme selected
                //selectedBoardWidget.Clear(false);
                selectedBoardWidget.SetArea(selectedArea != null ? selectedArea.areaID : Area.NONE);
                selectedBoardWidgetButton.SetState(true);

                gameBoardDefinition = null;
            }
            else if (selectedArea == null)
            {
                selectedBoardWidgetButton.SetState(false);

                gameBoardDefinition = null;
            }

            //get board
            selectedBoardWidget.SetData(gameBoardDefinition, selectedArea == null);

            prevArea = selectedArea;
        }

        public override void OnBack()
        {
            base.OnBack();

            //check backCaller id 
            GamePieceWidgetLandscape prev;
            bool removeById = CustomInputManager.GamepadCount > 1 && (StandaloneInputModuleExtended.BackCallerId == 0 || StandaloneInputModuleExtended.BackCallerId == 1);

            if (removeById)
            {
                if (selectedPlayers[StandaloneInputModuleExtended.BackCallerId] == null)
                {
                    BackToRoot();
                    return;
                }
                else
                {
                    prev = selectedPlayers[StandaloneInputModuleExtended.BackCallerId];
                    selectedPlayers[StandaloneInputModuleExtended.BackCallerId] = null;
                    prev.SelectAsPlayer(GetSameProfiles(prev, -1));

                    UpdateProfiles();
                }
            }
            else
            {
                if (selectedPlayers[1] != null || selectedPlayers[0] != null)
                {
                    for (int index = selectedPlayers.Length - 1; index >= 0; index--)
                    {
                        if (selectedPlayers[index] == null) continue;

                        prev = selectedPlayers[index];
                        selectedPlayers[index] = null;
                        prev.SelectAsPlayer(GetSameProfiles(prev, -1));

                        break;
                    }

                    UpdateProfiles();
                }
                else
                {
                    BackToRoot();
                    return;
                }
            }
            //switch (StandaloneInputModuleExtended.BackCallerId)
            //{
            //    case 0:
            //    case 1:
                    

            //        break;

            //    default:
                    

            //        break;
            //}

            readyButton.SetState(selectedPlayers.ToList().TrueForAll(_widget => _widget != null));
        }

        public void PickBoard() => 
            menuController
            .GetScreen<LandscapeGameboardSelectionScreen>()
            .Open(selectedArea.areaID);

        public void ToggleP2()
        {
            if (p2DifficultyLevel > -1)
            {
                timerToggle.SetState(true);

                p2Button.SetLabel("P2");
                p2DifficultyLevel = -1;
                profiles[1].DisplayDifficulty(-1);

                if (selectedPlayers[1] != null)
                {
                    selectedPlayers[1].SetP2AsCPU(false);
                }
            }
            else
            {
                PickP2();
            }
        }

        public void PickP2() => 
            difficultyDropdown
            .Open(false)
            .SetPosition(profiles[1].transform, Vector2.zero)
            .SetOnClick(OnP2DifficultySelected);

        public void ToggleP1()
        {
            if (p1DifficultyLevel > -1)
            {
                p1Button.SetLabel("P1");
                p1DifficultyLevel = -1;
                profiles[0].DisplayDifficulty(-1);
            }
            else
            {
                difficultyDropdown
                    .Open(true)
                    .SetPosition(profiles[0].transform, Vector2.zero)
                    .SetOnClick(OnP1DifficultySelected);
            }
        }

        public void ToggleLocalTimer() => SettingsManager.Toggle(SettingsManager.KEY_LOCAL_TIMER);

        public void ToggleMagic() => SettingsManager.Toggle(SettingsManager.KEY_REALTIME_MAGIC);

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

            if (CustomInputManager.GamepadCount < 2)
            {
                OnP2DifficultySelected(InternalSettings.Current.DEFAULT_STANDALONE_CPU_DIFFICULTY);
            }
        }

        private void CreatePieces()
        {
            if (!initialized) return;

            //remove prev
            foreach (GamePieceWidgetLandscape gamePieceWidgetLandscape in gamePieceWidgets) Destroy(gamePieceWidgetLandscape.gameObject);
            gamePieceWidgets.Clear();

            unlockedGamepieces = GameContentManager.Instance.piecesDataHolder.gamePieces.list.Where(_widget => _widget.data.profilePicture);
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

            foreach (GamePiecePrefabData prefabData in unlockedGamepieces) CreateGamepieceWidget(prefabData.data);

            //add random
            CreateGamepieceWidget(null);
        }

        private GamePieceWidgetLandscape CreateGamepieceWidget(GamePieceData data)
        {
            GamePieceWidgetLandscape widget = GameContentManager.InstantiatePrefab<GamePieceWidgetLandscape>(GameContentManager.PrefabType.GAME_PIECE_LANDSCAPE, gamePiecesRectTransform);

            widget
                .SetData(data)
                .SetOnClick(OnGPSelected);

            widgets.Add(widget);
            gamePieceWidgets.Add(widget);

            return widget;
        }

        private void UpdateProfiles()
        {
            for (int profileIndex = 0; profileIndex < profiles.Length; profileIndex++)
                profiles[profileIndex].SetData(selectedPlayers[profileIndex]);
        }

        private void OnGPSelected(PointerEventData data, GamePieceWidgetLandscape piece)
        {
            if (CustomInputManager.GamepadCount < 2 || data.pointerId == -1)
            {
                for (int index = 0; index < selectedPlayers.Length; index++)
                {
                    if (selectedPlayers[index] != null && index == 0) continue;

                    UpdateSelectedPlayer(index, piece);

                    break;
                }
            }
            else
                UpdateSelectedPlayer(data.pointerId, piece);

            readyButton.SetState(selectedPlayers.ToList().TrueForAll(_widget => _widget != null));
            UpdateProfiles();

            gamePieceWidgets.ForEach(_widget => _widget.OnPieceSelected(piece));
        }

        private int[] GetSameProfiles(GamePieceWidgetLandscape piece, int indexToIgnore)
        {
            List<int> result = new List<int>();

            for (int index = 0; index < selectedPlayers.Length; index++)
            {
                if (indexToIgnore == index) continue;
                if (selectedPlayers[index] == piece) result.Add(index);
            }

            if (result.Count == 0) result.Add(-1);

            return result.ToArray();
        }

        private void UpdateSelectedPlayer(int index, GamePieceWidgetLandscape piece)
        {
            GamePieceWidgetLandscape prev = selectedPlayers[index];
            selectedPlayers[index] = piece;

            if (prev != null) prev.SelectAsPlayer(GetSameProfiles(prev, index));

            selectedPlayers[index].SelectAsPlayer(GetSameProfiles(piece, -1));

            bool isP2CPU = p2DifficultyLevel > -1;

            selectedPlayers[index].SetP2AsCPU(isP2CPU);
        }

        private void OnBoardSelected(GameBoardDefinition boardDefinition)
        {
            gameBoardDefinition = boardDefinition;
        }

        private void OnAreaSelected(AreasDataHolder.GameArea areaData)
        {
            selectedArea = areaData;
        }

        private void OnP1DifficultySelected(int option)
        {
            //not applicable to p1 yet
        }

        private void OnP2DifficultySelected(int option)
        {
            if (selectedPlayers[1] != null) selectedPlayers[1].SetP2AsCPU(true);

            p2Button.SetLabel("CPU");
            p2DifficultyLevel = option;

            profiles[1].DisplayDifficulty(option);

            timerToggle.SetState(p2DifficultyLevel < 0);
        }

        public enum CURRENT_VS_STAGE
        {
            P1_SELECT,
            P2_SELECT,
            READY,
        }
    }
}