//@vadym udod

using Fourzy._Updates.ClientModel;
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
        public Image areaPicker;
        public int optimalColumnCount = 5;
        public int maxGridWidth = 2000;
        [Range(5, 50)]
        public int testPieces = 20;

        private List<GamePieceWidgetLandscape> gamePieceWidgets = new List<GamePieceWidgetLandscape>();
        private IEnumerable<GamePiecePrefabData> unlockedGamepieces;
        private GamePieceWidgetLandscape widgetPrefab;
        private RectTransform gamePiecesRectTransform;

        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public Vector2 WidgetSize { get; private set; }

        /// <summary>
        /// Invoked from ReadyButton
        /// </summary>
        public void StartGame()
        {
            Debug.Log("GameContentManager.Instance.currentTheme.themeID: " + GameContentManager.Instance.currentTheme.themeID);
            //Player player1 = new Player(1, "Player One");
            Player player1 = UserManager.Instance.meAsPlayer;
            Player player2 = new Player(2, "Player Two");

            ClientFourzyGame game  = new ClientFourzyGame(GameContentManager.Instance.currentTheme.themeID, player1, player2, player1.PlayerId);

            game._Type = GameType.PASSANDPLAY;

            GameManager.Instance.StartGame(game);
        }

        public override void Open()
        {
            base.Open();

            areaPicker.sprite = GameContentManager.Instance.currentTheme.landscapePreview;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            gamePiecesRectTransform = gamepiecesParent.GetComponent<RectTransform>();
            widgetPrefab = GameContentManager.GetPrefab<GamePieceWidgetLandscape>(GameContentManager.PrefabType.GAME_PIECE_LANDSCAPE);
            WidgetSize = widgetPrefab.GetComponent<RectTransform>().rect.size;

            //set default ready button state
            //readyButton.SetState(false);

            CreatePieces();
        }

        private void CreatePieces()
        {
            print(initialized);
            if (!initialized) return;

            //remove prev
            foreach (GamePieceWidgetLandscape gamePieceWidgetLandscape in gamePieceWidgets) Destroy(gamePieceWidgetLandscape.gameObject);
            gamePieceWidgets.Clear();

            unlockedGamepieces = GameContentManager.Instance.piecesDataHolder.gamePieces.list.Where(_widget => _widget.data.State == GamePieceState.FoundAndLocked);
            int piecesCount = testPieces;
            //int piecesCount = unlockedGamepieces.Count();

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
            for (int testIndex = 0; testIndex < piecesCount; testIndex++)
            //foreach (GamePiecePrefabData prefabData in unlockedGamepieces)
            {
                GamePieceWidgetLandscape widget = GameContentManager.InstantiatePrefab<GamePieceWidgetLandscape>(GameContentManager.PrefabType.GAME_PIECE_LANDSCAPE, gamePiecesRectTransform);
                //widget.SetData(prefabData.data);
                widget.SetData(GameContentManager.Instance.piecesDataHolder.gamePieces.list[testIndex % GameContentManager.Instance.piecesDataHolder.gamePieces.list.Count].data);

                widgets.Add(widget);
                gamePieceWidgets.Add(widget);
            }
        }
    }
}