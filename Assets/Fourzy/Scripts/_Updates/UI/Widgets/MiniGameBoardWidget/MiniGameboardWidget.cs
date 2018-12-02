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
        public TokenBoard tokenBoard;

        public GameBoardView gameboardView { get; private set; }
        public Toggle toggle { get; private set; }

        protected override void Awake()
        {
            gameboardView = GetComponentInChildren<GameBoardView>();
            toggle = GetComponent<Toggle>();
        }

        public void CreateGamePieces()
        {
            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    Piece piece = (Piece)tokenBoard.initialGameBoard[row * Constants.numRows + col];
                    if (piece == Piece.EMPTY)
                        continue;

                    MiniGameboardPiece go = gameboardView.SpawnMinigameboardPiece(row, col, GameContentManager.GetPrefab<MiniGameboardPiece>(GameContentManager.PrefabType.MINI_GAME_BOARD_PIECE));
                    go.SetGamePiece(piece);
                }
            }
        }

        public void CreateTokens()
        {
            tokenBoard.SetTokenBoardFromData(tokenBoard.tokenBoard);

            gameboardView.CreateTokenViews(tokenBoard.tokens);
        }

        public void SetAsRandom()
        {
            questionMark.SetActive(true);
        }

        public void BoardSelect()
        {
            if (tokenBoard == null)
                return;

            if (toggle.isOn)
            {
                Debug.Log("GAME BOARD SELECTED ON: tokenboard.id: " + tokenBoard.id);

                //if (OnSetTokenBoard != null && tokenBoard != null)
                //    OnSetTokenBoard(tokenBoard.id);
            }
        }
    }
}
