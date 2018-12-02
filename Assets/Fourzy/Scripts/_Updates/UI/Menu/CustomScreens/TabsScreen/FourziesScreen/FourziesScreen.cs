//@vadym udod

using Fourzy._Updates.UI.Widgets;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class FourziesScreen : MenuScreen
    {
        public FlowLayoutGroup piecesGroup;
        public GridLayoutGroup tokensGroup;

        protected override void Start()
        {
            base.Start();

            //CreateGamePieces();
        }

        public override void Open()
        {
            base.Open();

            CreateGamePieces();
            CreateTokens();
        }

        private void CreateGamePieces()
        {
            //remove old ones
            foreach (Transform gamePiece in piecesGroup.transform)
                Destroy(gamePiece.gameObject);

            //load game pieces
            foreach (GamePieceData data in GameContentManager.Instance.GetAllGamePieces())
                GameContentManager.InstantiatePrefab<GamePieceWidgetMedium>(GameContentManager.PrefabType.GAME_PIECE_MEDIUM, piecesGroup.transform).SetData(data);
        }

        private void CreateTokens()
        {
            //remove old ones
            foreach (Transform token in tokensGroup.transform)
                Destroy(token.gameObject);

            //load tokens
            foreach (TokenData data in GameContentManager.Instance.GetAllTokens())
                GameContentManager.InstantiatePrefab<TokenWidget>(GameContentManager.PrefabType.TOKEN_SMALL, tokensGroup.transform).SetData(data);
        }
    }
}                                                                                                                                                                                                                                                         