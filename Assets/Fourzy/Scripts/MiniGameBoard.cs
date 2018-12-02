using Fourzy._Updates.Mechanics.Board;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class MiniGameBoard : MonoBehaviour
    {
        public delegate void SetTokenBoard(string tokenBoardId);
        public static event SetTokenBoard OnSetTokenBoard;

        [HideInInspector]
        public TokenBoard tokenBoard;

        public GameBoardView gameboardView;
        public GameObject glow;
        public GameObject questionMark;

        public Sprite Player1Piece;
        public Sprite Player2Piece;
        public GameObject gamePiecePrefab;

        public void CreateGamePieces()
        {
            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    Piece piece = (Piece)tokenBoard.initialGameBoard[row * Constants.numRows + col];
                    if (piece == Piece.EMPTY)
                    {
                        continue;
                    }

                    //GameObject go = gameboardView.SpawnMinigameboardPiece(row, col, gamePiecePrefab);
                    //SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
                    //spriteRenderer.enabled = true;
                    //spriteRenderer.sortingOrder = 50;

                    //if (piece == Piece.BLUE)
                    //{
                    //    spriteRenderer.sprite = Player1Piece;
                    //}
                    //else if (piece == Piece.RED)
                    //{
                    //    spriteRenderer.sprite = Player2Piece;
                    //}
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
            // Debug.Log("BoardSelect");
            Toggle toggle = this.GetComponentInChildren<Toggle>();
            if (toggle.isOn)
            {
                Debug.Log("GAME BOARD SELECTED ON: tokenboard.id: " + tokenBoard.id);
                glow.SetActive(true);
                if (OnSetTokenBoard != null && tokenBoard != null)
                    OnSetTokenBoard(tokenBoard.id);
            }
            else
            {
                //Debug.Log("GAME BOARD SELECTED OFF");
                glow.SetActive(false);
            }
        }
    }
}