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
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;
using static Fourzy._Updates.UI.Helpers.OnRatio;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class VSLandscapeScreen : MenuScreen
    {
        [SerializeField]
        private FlowLayoutGroup gamepiecesParent;
        [SerializeField]
        private VSScreenReadyButton readyButton;
        [SerializeField]
        private ButtonExtended timerToggle;
        [SerializeField]
        private ButtonExtended areaPicker;
        [SerializeField]
        private ButtonExtended p1Button;
        [SerializeField]
        private ButtonExtended p2Button;
        [SerializeField]
        private RectTransform footer;
        [SerializeField]
        private RectTransform header;
        [SerializeField]
        private TMP_Text title;
        [SerializeField]
        private MiniGameboardWidget selectedBoardWidget;
        [SerializeField]
        private VSScreenDifficultyDropdown difficultyDropdown;
        [SerializeField]
        private Sprite randomAreaIcon;
        [SerializeField]
        private VSScreenDragableGamepiece dragablePiecePrefab;
        [SerializeField]
        private VSScreenPlayerWidget[] profiles;

        private List<GamePieceWidgetLandscape> gamePieceWidgets = new List<GamePieceWidgetLandscape>();
        private IEnumerable<GamePieceData> unlockedGamepieces;
        private GamePieceWidgetLandscape widgetPrefab;
        private RectTransform gamePiecesRectTransform;
        private ButtonExtended selectedBoardWidgetButton;

        private OnRatio footerRatio;
        private GameBoardDefinition gameBoardDefinition;
        private AreasDataHolder.GameArea selectedArea;
        private AreasDataHolder.GameArea prevArea;

        private List<VSScreenDragableGamepiece> activeDragables = new List<VSScreenDragableGamepiece>();
        private int optimalColumnCount = 1;
        private int demoCounter = 0;
        private int columns;
        private int rows;
        private Vector2 widgetSize;
        private bool acrossLayout;

        public GamePieceWidgetLandscape[] SelectedPlayers { get; private set; } = new GamePieceWidgetLandscape[2];
        public int P1DifficultyLevel { get; private set; } = -1;
        public int P2DifficultyLevel { get; private set; } = -1;

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
                {
                    demoCounter = 0;
                }
            }
        }

        protected override void Start()
        {
            base.Start();

            timerToggle.SetState(P2DifficultyLevel < 0);
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
            if (SelectedPlayers[0] != null)
            {
                if (SelectedPlayers[0].data != null)
                {
                    herdId = SelectedPlayers[0].data.Id;
                    displaName = SelectedPlayers[0].data.name;
                    magic = SelectedPlayers[0].data.Magic;
                }
                else
                {
                    GamePieceData random = unlockedGamepieces.Random();
                    herdId = random.Id;
                    displaName = random.name;
                    magic = random.Magic;
                }

                player1.HerdId = herdId;
                player1.DisplayName = displaName;
                player1.Magic = magic;
            }

            Player player2 = new Player(2, "Player Two");
            if (SelectedPlayers[1] != null)
            {
                if (SelectedPlayers[1].data != null)
                {
                    herdId = SelectedPlayers[1].data.Id;
                    displaName = SelectedPlayers[1].data.name;
                    magic = SelectedPlayers[1].data.Magic;
                }
                else
                {
                    GamePieceData random = unlockedGamepieces.Random();
                    herdId = random.Id;
                    displaName = random.name;
                    magic = random.Magic;
                }

                player2.HerdId = herdId;
                player2.DisplayName = displaName;
                player2.Magic = magic;
            }

            if (P2DifficultyLevel > -1)
            {
                player2.Profile = AIPlayerFactory.RandomProfile((AIDifficulty)P2DifficultyLevel);
            }

            ClientFourzyGame game;
            Area _area = selectedArea == null ?
                    GameContentManager.Instance.areasDataHolder.areas.Random().areaID :
                    selectedArea.areaID;

            if (gameBoardDefinition == null)
            {
                game = new ClientFourzyGame(_area, player1, player2, player1.PlayerId);
            }
            else
            {
                game = new ClientFourzyGame(gameBoardDefinition, player1, player2);
            }

            if (P1DifficultyLevel > -1 && P2DifficultyLevel > -1)
            {
                game._Type = GameType.PRESENTATION;
            }
            else if (P1DifficultyLevel > -1 || P2DifficultyLevel > -1)
            {
                game._Type = GameType.AI;
            }
            else
            {
                game._Type = GameType.PASSANDPLAY;
            }
            game.UpdateFirstState();

            AnalyticsManager.Instance.LogEvent(
                "PRACTICE_GAME_CREATED",
                new Dictionary<string, object>()
                {
                    ["player1"] = player1.Profile.ToString(),
                    ["player2"] = player2.Profile.ToString(),
                    ["area"] = _area,
                    ["isTimerEnabled"] = SettingsManager.Get(SettingsManager.KEY_LOCAL_TIMER),
                    ["isMagicEnabled"] = SettingsManager.Get(SettingsManager.KEY_REALTIME_MAGIC),
                });

            GameManager.Instance.StartGame(game, GameTypeLocal.LOCAL_GAME);
        }

        public override void Open()
        {
            base.Open();

            acrossLayout = GameManager.Instance.buildIntent == BuildIntent.MOBILE_INFINITY && PlayerPositioningPromptScreen.PlayerPositioning == PlayerPositioning.ACROSS;
            //position player widgets
            if (acrossLayout)
            {
                footer.localScale = new Vector3(.7f, .7f, 1f);
            }
            else
            {
                footerRatio.CheckOrientation();
            }

            gamePiecesRectTransform.anchoredPosition = acrossLayout ? Vector2.zero : new Vector2(0f, 175f);
            footer.anchoredPosition = new Vector3(0f, acrossLayout  ? 10f : 80f, 0f);
            profiles[1].transform.SetParent(acrossLayout ? header : footer);
            profiles[1].transform.localEulerAngles = Vector3.zero;
            profiles[1].transform.localScale = Vector3.one;
            header.gameObject.SetActive(acrossLayout);
            title.gameObject.SetActive(!acrossLayout);

            CreatePieces();

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

            //across layout is specific only to infinity table platform, thus gamepads check won't be performed
            if (acrossLayout)
            {
                //release dragged gamepieces
                while (activeDragables.Count > 0)
                {
                    activeDragables[0].Release(false);
                }

                BackToRoot();
                return;
            }
            else
            {
                //this first checks if back was pressed by a specific gamepad
                GamePieceWidgetLandscape prev;
                bool removeById = CustomInputManager.GamepadCount > 1 && (StandaloneInputModuleExtended.BackCallerId == 0 || StandaloneInputModuleExtended.BackCallerId == 1);

                if (removeById)
                {
                    if (SelectedPlayers[StandaloneInputModuleExtended.BackCallerId] == null)
                    {
                        BackToRoot();
                        return;
                    }
                    else
                    {
                        prev = SelectedPlayers[StandaloneInputModuleExtended.BackCallerId];
                        SelectedPlayers[StandaloneInputModuleExtended.BackCallerId] = null;
                        prev.SelectAsPlayer(GetSameProfiles(prev, -1));

                        UpdateProfiles();
                    }
                }
                //otherwise remove p2 selection(if selected), than p1
                else
                {
                    if (SelectedPlayers[1] != null || SelectedPlayers[0] != null)
                    {
                        for (int index = SelectedPlayers.Length - 1; index >= 0; index--)
                        {
                            if (SelectedPlayers[index] == null) continue;

                            prev = SelectedPlayers[index];
                            SelectedPlayers[index] = null;
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
            }

            readyButton.SetState(SelectedPlayers.ToList().TrueForAll(_widget => _widget != null));
        }

        public void PickBoard() => menuController
            .GetScreen<LandscapeGameboardSelectionScreen>()
            .Open(selectedArea.areaID);

        public void ToggleP2()
        {
            if (P2DifficultyLevel > -1)
            {
                timerToggle.SetState(true);

                p2Button.SetLabel("P2");
                P2DifficultyLevel = -1;
                profiles[1].DisplayDifficulty(-1);

                if (SelectedPlayers[1] != null)
                {
                    SelectedPlayers[1].SetP2AsCPU(false);
                }
            }
            else
            {
                PickP2();
            }
        }

        public void PickP2()
        {
            difficultyDropdown
                .Open(false)
                .SetPosition(profiles[1].transform, Vector2.zero)
                .SetOnClick(OnP2DifficultySelected);
        }

        public void ToggleP1()
        {
            if (P1DifficultyLevel > -1)
            {
                p1Button.SetLabel("P1");
                P1DifficultyLevel = -1;
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
            widgetPrefab = GameContentManager.GetPrefab<GamePieceWidgetLandscape>("GAME_PIECE_LANDSCAPE");
            widgetSize = widgetPrefab.GetComponent<RectTransform>().rect.size;
            selectedBoardWidgetButton = selectedBoardWidget.GetComponent<ButtonExtended>();
            footerRatio = footer.GetComponent<OnRatio>();

            //listen to board select screen
            menuController.GetScreen<LandscapeGameboardSelectionScreen>().onBoardSelected += OnBoardSelected;
            menuController.GetScreen<AreaSelectLandscapeScreen>().onAreaSeleected += OnAreaSelected;

            //set default ready button state
            readyButton.SetState(false);

            if (CustomInputManager.GamepadCount < 2)
            {
                OnP2DifficultySelected(InternalSettings.Current.DEFAULT_STANDALONE_CPU_DIFFICULTY);
            }

            switch (OnRatio.GetRatio(DeviceOrientation.LandscapeLeft))
            {
                case DisplayRatioOption.IPAD:
                    optimalColumnCount = 2;

                    break;

                case DisplayRatioOption.IPHONE:
                    optimalColumnCount = 5;

                    break;

                case DisplayRatioOption.IPHONEX:
                    optimalColumnCount = 5;

                    break;
            }
        }

        private void CreatePieces()
        {
            if (!initialized) return;

            //remove prev
            foreach (GamePieceWidgetLandscape gamePieceWidgetLandscape in gamePieceWidgets)
            {
                Destroy(gamePieceWidgetLandscape.gameObject);
            }
            gamePieceWidgets.Clear();

            unlockedGamepieces = GameContentManager.Instance.piecesDataHolder.gamePieces.Where(_widget => _widget.profilePicture);
            int piecesCount = unlockedGamepieces.Count() + 1;
            Vector3 scale = Vector3.one;

            columns = optimalColumnCount;
            rows = 1;

            while (piecesCount > columns * rows)
            {
                if (piecesCount > columns * rows)
                {
                    rows++;
                }
                else
                {
                    break;
                }

                if (piecesCount > columns * rows)
                {
                    columns++;
                }
                else
                {
                    break;
                }
            }

            float maxGridSize = 1400f;
            switch (OnRatio.GetRatio(DeviceOrientation.LandscapeLeft))
            {
                case DisplayRatioOption.IPHONE:
                    maxGridSize = 2000f;

                    break;

                case DisplayRatioOption.IPHONEX:
                    maxGridSize = 2300f;

                    break;
            }

            gamePiecesRectTransform.sizeDelta = new Vector2((widgetSize.x * columns) + (gamepiecesParent.SpacingX * (columns - 1)), (widgetSize.y * rows) + (gamepiecesParent.SpacingY * (rows - 1)));
            if (gamePiecesRectTransform.sizeDelta.x > maxGridSize)
            {
                scale = new Vector3(maxGridSize / gamePiecesRectTransform.sizeDelta.x, maxGridSize / gamePiecesRectTransform.sizeDelta.x);

            }
            gamePiecesRectTransform.localScale = scale;

            foreach (GamePieceData prefabData in unlockedGamepieces)
            {
                CreateGamepieceWidget(prefabData);
            }

            //add random
            CreateGamepieceWidget(null);
        }

        private GamePieceWidgetLandscape CreateGamepieceWidget(GamePieceData data)
        {
            GamePieceWidgetLandscape widget = GameContentManager.InstantiatePrefab<GamePieceWidgetLandscape>("GAME_PIECE_LANDSCAPE", gamePiecesRectTransform)
                .SetData(data);

            if (GameManager.Instance.buildIntent == BuildIntent.MOBILE_INFINITY)
            {
                widget.SetOnPointerDown(StartDrag);
            }
            else
            {
                widget.SetOnClick((eventData, pieceWidget) => OnGPSelected(eventData.pointerId, pieceWidget));
            }

            widgets.Add(widget);
            gamePieceWidgets.Add(widget);

            return widget;
        }

        private void UpdateProfiles()
        {
            for (int profileIndex = 0; profileIndex < profiles.Length; profileIndex++)
            {
                profiles[profileIndex].SetData(SelectedPlayers[profileIndex]);
            }
        }

        private void OnGPSelected(int pointerId, GamePieceWidgetLandscape piece)
        {
            if (GameManager.Instance.buildIntent != BuildIntent.MOBILE_INFINITY && (CustomInputManager.GamepadCount < 2 || pointerId == -1))
            {
                for (int index = 0; index < SelectedPlayers.Length; index++)
                {
                    if (SelectedPlayers[index] != null && index == 0) continue;

                    UpdateSelectedPlayer(index, piece);

                    break;
                }
            }
            else
            {
                UpdateSelectedPlayer(pointerId, piece);
            }

            readyButton.SetState(SelectedPlayers.ToList().TrueForAll(_widget => _widget != null));
            UpdateProfiles();

            gamePieceWidgets.ForEach(_widget => _widget.OnPieceSelected(piece));
        }

        private void StartDrag(PointerEventData data, GamePieceWidgetLandscape piece)
        {
            VSScreenDragableGamepiece dragablePiece = Instantiate(dragablePiecePrefab, transform);

            dragablePiece.gameObject.SetActive(true);
            dragablePiece
                .AttachToPointer(data)
                .SetOnRemoved(OnDragableGamepieceRemoved)
                .SetOnDropped((data) => OnDragableGamepieceDropped(data, piece))
                .SetGamepiece(piece.data);
            activeDragables.Add(dragablePiece);

            //outline player profiles
            foreach (var profile in profiles)
            {
                profile.SetOutline(true);
            }
        }

        private void OnDragableGamepieceRemoved(VSScreenDragableGamepiece target)
        {
            activeDragables.Remove(target);

            if (activeDragables.Count == 0)
            {
                //hide player profiles outline
                foreach (var profile in profiles)
                {
                    profile.SetOutline(false);
                }
            }
        }

        private void OnDragableGamepieceDropped(VSScreenDragableGamepiece target, GamePieceWidgetLandscape piece)
        {
            for (int profileIndex = 0; profileIndex < profiles.Length; profileIndex++)
            {
                if (profiles[profileIndex].CheckDroppedPiece(target))
                {
                    OnGPSelected(profileIndex, piece);
                    break;
                }
            }
        }

        private int[] GetSameProfiles(GamePieceWidgetLandscape piece, int indexToIgnore)
        {
            List<int> result = new List<int>();

            for (int index = 0; index < SelectedPlayers.Length; index++)
            {
                if (indexToIgnore == index) continue;
                if (SelectedPlayers[index] == piece)
                {
                    result.Add(index);
                }
            }

            if (result.Count == 0)
            {
                result.Add(-1);
            }

            return result.ToArray();
        }

        private void UpdateSelectedPlayer(int index, GamePieceWidgetLandscape piece)
        {
            GamePieceWidgetLandscape prev = SelectedPlayers[index];
            SelectedPlayers[index] = piece;

            if (prev != null)
            {
                prev.SelectAsPlayer(GetSameProfiles(prev, index));
            }

            SelectedPlayers[index].SelectAsPlayer(GetSameProfiles(piece, -1));

            bool isP2CPU = P2DifficultyLevel > -1;

            SelectedPlayers[index].SetP2AsCPU(isP2CPU);
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
            if (SelectedPlayers[1] != null)
            {
                SelectedPlayers[1].SetP2AsCPU(true);
            }

            p2Button.SetLabel("CPU");
            P2DifficultyLevel = option;

            profiles[1].DisplayDifficulty(option);

            timerToggle.SetState(P2DifficultyLevel < 0);
        }

        public enum CURRENT_VS_STAGE
        {
            P1_SELECT,
            P2_SELECT,
            READY,
        }
    }
}