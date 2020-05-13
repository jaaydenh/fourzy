//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Widgets;
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

        }


        protected override void OnInitialized()
        {
            base.OnInitialized();

            gamePiecesRectTransform = gamepiecesParent.GetComponent<RectTransform>();
            widgetPrefab = GameContentManager.GetPrefab<GamePieceWidgetLandscape>(GameContentManager.PrefabType.GAME_PIECE_LANDSCAPE);
            WidgetSize = widgetPrefab.GetComponent<RectTransform>().rect.size;

            //set default ready button state
            readyButton.SetState(false);

            CreatePieces();
        }

        private void CreatePieces()
        {
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