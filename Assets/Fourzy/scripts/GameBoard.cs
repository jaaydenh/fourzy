using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {

    GameObject gamePieces;
    public GameObject pieceRed;
    public GameObject pieceBlue;
    public enum Token {Empty, UpArrow, DownArrow, LeftArrow, RightArrow}
    public enum Piece {Empty, Blue, Red}
    public GameObject[,] gamePiecesArray;
    public Token[,] tokenBoard;
    [Range(3, 8)]
    public int numRows = 8;
    [Range(3, 8)]
    public int numColumns = 8;
    bool isLoading = true;

	void Start () {
        tokenBoard = new Token[numColumns, numRows];
        gamePiecesArray = new GameObject[numColumns, numRows];
	}

    public IEnumerator SetGameBoard(int[] boardData) {
        isLoading = true;

        for(int col = 0; col < numColumns; col++)
        {
            for(int row = 0; row < numRows; row++)
            {
                int piece = boardData[col * numColumns + row];
                //gameBoard[col, row] = piece;
                if (piece == (int)Piece.Blue)
                {
                    gamePiecesArray[col,row] = Instantiate(pieceBlue, new Vector3(col, row * -1, 10), Quaternion.identity, gamePieces.transform);
                }
                else if (piece == (int)Piece.Red)
                {
                    gamePiecesArray[col,row] = Instantiate(pieceRed, new Vector3(col, row * -1, 10), Quaternion.identity, gamePieces.transform);
                }
            }
        }

        isLoading = false;

        yield return 0;
    }
}
