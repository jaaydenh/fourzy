//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class VSInfinityScreen : MenuScreen
    {
        [SerializeField]
        private FlowLayoutGroup gamepiecesParent;
        [SerializeField]
        private MiniGameboardWidget selectedBoardWidget;
        [SerializeField]
        private VSScreenPlayerWidget[] profiles;
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

        private GamePieceWidgetLandscape widgetPrefab;
        private RectTransform gamePiecesRectTransform;
        private ButtonExtended selectedBoardWidgetButton;
        private Vector2 pieceWidgetSize;
        private List<GamePieceWidgetLandscape> gamePieceWidgets = new List<GamePieceWidgetLandscape>();

        private GameBoardDefinition gameBoardDefinition;
        private AreasDataHolder.GameArea selectedArea;
        private AreasDataHolder.GameArea prevArea;

        public int P1DifficultyLevel { get; private set; } = -1;
        public int P2DifficultyLevel { get; private set; } = -1;
        public GamePieceWidgetLandscape[] SelectedPlayers { get; private set; } = new GamePieceWidgetLandscape[2];

        public override void Open()
        {
            base.Open();

            //areaPicker.image.sprite = selectedArea == null ? randomAreaIcon : selectedArea._4X3;
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

        protected override void OnInitialized()
        {
            base.OnInitialized();

            gamePiecesRectTransform = gamepiecesParent.GetComponent<RectTransform>();
            widgetPrefab = GameContentManager.GetPrefab<GamePieceWidgetLandscape>("GAME_PIECE_LANDSCAPE");
            pieceWidgetSize = widgetPrefab.GetComponent<RectTransform>().rect.size;
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

        private void OnBoardSelected(GameBoardDefinition boardDefinition)
        {
            gameBoardDefinition = boardDefinition;
        }

        private void OnAreaSelected(AreasDataHolder.GameArea areaData)
        {
            selectedArea = areaData;
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

            foreach (GamePieceData prefabData in GameContentManager.Instance.piecesDataHolder.gamePieces.Where(_widget => _widget.profilePicture))
            {
                CreateGamepieceWidget(prefabData);
            }

            //add random
            CreateGamepieceWidget(null);
        }

        private GamePieceWidgetLandscape CreateGamepieceWidget(GamePieceData data)
        {
            GamePieceWidgetLandscape widget = GameContentManager.InstantiatePrefab<GamePieceWidgetLandscape>("GAME_PIECE_LANDSCAPE", gamePiecesRectTransform);

            widget
                .SetData(data)
                .SetOnClick(OnGPSelected);

            widgets.Add(widget);
            gamePieceWidgets.Add(widget);

            return widget;
        }

        private void OnGPSelected(PointerEventData data, GamePieceWidgetLandscape piece)
        {
            if (CustomInputManager.GamepadCount < 2 || data.pointerId == -1)
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
                UpdateSelectedPlayer(data.pointerId, piece);
            }

            readyButton.SetState(SelectedPlayers.ToList().TrueForAll(_widget => _widget != null));
            UpdateProfiles();

            gamePieceWidgets.ForEach(_widget => _widget.OnPieceSelected(piece));
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

        private void UpdateProfiles()
        {
            for (int profileIndex = 0; profileIndex < profiles.Length; profileIndex++)
            {
                profiles[profileIndex].SetData(SelectedPlayers[profileIndex]);
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
    }
}