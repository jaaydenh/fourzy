//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GamePiecesScreen : MenuTab
    {
        public GridLayoutGroup piecesGroup;
        public GridLayoutGroup tokensGroup;

        private List<GamePieceWidgetMedium> gamePieceWidgets;

        public override bool isCurrent => base.isCurrent;

        protected override void Awake()
        {
            base.Awake();

            gamePieceWidgets = new List<GamePieceWidgetMedium>();
        }

        protected override void Start()
        {
            base.Start();

            CreateGamePieces();
            CreateTokens();
        }

        protected void Update()
        {
            //remove
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.A))
            {
                ////give game pieces
                //foreach (GamePieceWidgetMedium widget in gamePieceWidgets)
                //    widget.data.AddPieces(Random.Range(1, 5));

                //give gems
                //UserManager.Instance.gems = 3;

                ////update gamepieces data
                //foreach (GamePieceWidgetMedium gamePieceWidget in gamePieceWidgets)
                //    gamePieceWidget.UpdateData();
            }
#endif
        }

        private void CreateGamePieces()
        {
            gamePieceWidgets.Clear();

            //remove old ones
            foreach (Transform gamePiece in piecesGroup.transform) Destroy(gamePiece.gameObject);

            //load game pieces
            foreach (GamePiecePrefabData prefabData in GameContentManager.Instance.piecesDataHolder.gamePieces.list)
            {
                GamePieceWidgetMedium widget = GameContentManager.InstantiatePrefab<GamePieceWidgetMedium>(GameContentManager.PrefabType.GAME_PIECE_MEDIUM, piecesGroup.transform);
                widget.SetData(prefabData.data);
                gamePieceWidgets.Add(widget);

                widgets.Add(widget);
            }
        }

        private void CreateTokens()
        {
            //remove old ones
            foreach (Transform token in tokensGroup.transform)
                Destroy(token.gameObject);

            //load tokens
            foreach (TokensDataHolder.TokenData data in GameContentManager.Instance.enabledTokens)
                GameContentManager.InstantiatePrefab<TokenWidget>(GameContentManager.PrefabType.TOKEN_SMALL, tokensGroup.transform).SetData(data);
        }
    }
}
