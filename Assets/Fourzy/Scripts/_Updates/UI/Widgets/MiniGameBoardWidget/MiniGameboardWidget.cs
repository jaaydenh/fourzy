//@vadym udod

using Fourzy._Updates.Mechanics.Board;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class MiniGameboardWidget : WidgetBase
    {
        public Sprite player1Piece;
        public Sprite player2Piece;
        public GameObject questionMark;

        [HideInInspector]
        public TokenBoard tokenBoardData;

        public GameBoardView gameboardView { get; private set; }
        public Toggle toggle { get; private set; }

        protected override void Awake()
        {
            gameboardView = GetComponentInChildren<GameBoardView>();
            toggle = GetComponent<Toggle>();
        }

        public void SetData(TokenBoard data)
        {
            tokenBoardData = data;

            CreateTokens();
            CreateGamePieces();
        }

        public void CreateGamePieces()
        {
            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    Piece piece = (Piece)tokenBoardData.initialGameBoard[row * Constants.numRows + col];
                    if (piece == Piece.EMPTY)
                        continue;

                    gameboardView.SpawnMinigameboardPiece(row, col, GameContentManager.GetPrefab<MiniGameboardPiece>(GameContentManager.PrefabType.MINI_GAME_BOARD_PIECE)).SetGamePiece(piece);
                }
            }
        }

        public void CreateTokens()
        {
            tokenBoardData.SetTokenBoardFromData(tokenBoardData.tokenBoard);

            gameboardView.CreateTokenViews(tokenBoardData.tokens);
        }

        public void SetAsRandom()
        {
            questionMark.SetActive(true);
        }

        public void BoardSelect()
        {
            if (tokenBoardData == null)
                return;

            ChallengeManager.SetTokenBoard(tokenBoardData.id);
        }
    }
}
