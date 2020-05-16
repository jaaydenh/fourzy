//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class VSLandscapeScreen : MenuScreen
    {
        public FlowLayoutGroup gamepiecesParent;
        public VSScreenReadyButton readyButton;
        public VSScreenToggle timerToggle;
        public MiniGameboardWidget selectedBoardWidget;
        public Image areaPicker;
        public VSScreenPlayerWidget[] profilePics;
        public int optimalColumnCount = 5;
        public int maxGridWidth = 2000;

        private List<GamePieceWidgetLandscape> gamePieceWidgets = new List<GamePieceWidgetLandscape>();
        private IEnumerable<GamePiecePrefabData> unlockedGamepieces;
        private GamePieceWidgetLandscape widgetPrefab;
        private RectTransform gamePiecesRectTransform;

        private List<GamePieceWidgetLandscape> selectedPlayers = new List<GamePieceWidgetLandscape>();

        private GameBoardDefinition gameBoardDefinition;

        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public Vector2 WidgetSize { get; private set; }

        /// <summary>
        /// Invoked from ReadyButton
        /// </summary>
        public void StartGame()
        {
            Debug.Log("GameContentManager.Instance.currentTheme.themeID: " + GameContentManager.Instance.currentTheme.themeID);
            Player player1 = UserManager.Instance.meAsPlayer;
            if (selectedPlayers.Count > 0) player1.HerdId = selectedPlayers[0].data.ID;

            Player player2 = new Player(2, "Player Two");
            if (selectedPlayers.Count > 1) player2.HerdId = selectedPlayers[1].data.ID;

            ClientFourzyGame game;
            if (gameBoardDefinition == null) game = new ClientFourzyGame(GameContentManager.Instance.currentTheme.themeID, player1, player2, player1.PlayerId);
            else
            {
                gameBoardDefinition.Area = GameContentManager.Instance.currentTheme.themeID;
                game = new ClientFourzyGame(gameBoardDefinition, player1, player2);
            }

            game._Type = GameType.PASSANDPLAY;

            GameManager.Instance.StartGame(game);
        }

        public override void Open()
        {
            base.Open();

            areaPicker.sprite = GameContentManager.Instance.currentTheme.landscapePreview;

            //get board
            selectedBoardWidget.SetData(gameBoardDefinition, false);
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

        protected override void OnInitialized()
        {
            base.OnInitialized();

            gamePiecesRectTransform = gamepiecesParent.GetComponent<RectTransform>();
            widgetPrefab = GameContentManager.GetPrefab<GamePieceWidgetLandscape>(GameContentManager.PrefabType.GAME_PIECE_LANDSCAPE);
            WidgetSize = widgetPrefab.GetComponent<RectTransform>().rect.size;

            //listen to board select screen
            menuController.GetScreen<LandscapeGameboardSelectionScreen>().onBoardSelected += OnBoardSelected;

            //set default ready button state
            //readyButton.SetState(false);

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
            for (int profileIndex = 0; profileIndex < profilePics.Length; profileIndex++)
                profilePics[profileIndex].SetData(selectedPlayers.Count > profileIndex ? selectedPlayers[profileIndex].data : null);
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
                gm.SelectAsPlayer(selectedPlayers.Count);
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

        public enum CURRENT_VS_STAGE
        {
            P1_SELECT,
            P2_SELECT,
            READY,
        }
    }
}