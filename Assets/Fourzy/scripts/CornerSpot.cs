using UnityEngine;
using System.Collections;

namespace Fourzy
{
    public class CornerSpot : MonoBehaviour {

        public int row;
        public int column;
        public bool rightArrowActive = false;
        public bool leftArrowActive = false;
        public bool upArrowActive = false;
        public bool downArrowActive = false;
        public GameObject RightArrow;
        public GameObject LeftArrow;
        public GameObject UpArrow;
        public GameObject DownArrow;

    	void Start () {
            HideArrows();
    	}
    	
        public void HideArrows() {
            RightArrow.SetActive(false);
            LeftArrow.SetActive(false);
            UpArrow.SetActive(false);
            DownArrow.SetActive(false);
        }

        void OnMouseDown() {
            if (Fourzy.GameManager.instance.isCurrentPlayerTurn && !Fourzy.GameManager.instance.gameOver)
            {
                int noMoves = 0;
                //GameBoardView board = GameManager.instance.gameBoardView;

                if (rightArrowActive)
                {
                    if (column == 0) {
                        Move move = new Move(new Position(column + 1, row), Direction.RIGHT);
                        if (GameManager.instance.gameBoard.CanMove(move, GameManager.instance.tokenBoard.tokens)) {
                            RightArrow.SetActive(true);    
                        } else {
                            noMoves++;
                        }
                    }
                }
                if (leftArrowActive)
                {
                    if (column == 7) {
                        Move move = new Move(new Position(column - 1, row), Direction.LEFT);
                        if (GameManager.instance.gameBoard.CanMove(move, GameManager.instance.tokenBoard.tokens)) {
                            LeftArrow.SetActive(true);
                        } else {
                            noMoves++;
                        }
                    }
                }
                if (upArrowActive)
                {
                    if (row == 7) {
                        Move move = new Move(new Position(column, row - 1), Direction.UP);
                        if (GameManager.instance.gameBoard.CanMove(move, GameManager.instance.tokenBoard.tokens)) {
                            UpArrow.SetActive(true);
                        } else {
                            noMoves++;
                        }
                    }
                }
                if (downArrowActive)
                {
                    if (row == 0) {
                        Move move = new Move(new Position(column, row + 1), Direction.DOWN);
                        if (GameManager.instance.gameBoard.CanMove(move, GameManager.instance.tokenBoard.tokens)) {
                            DownArrow.SetActive(true);
                        } else {
                            noMoves++;
                        }
                    }
                }

                if (noMoves >= 2) {
                    Position pos = new Position(column, row);
                    Direction direction = new Direction();
                    if (column == 0) {
                        pos.column = -1;
                        direction = Direction.RIGHT;
                    } else if (column == 7 ) {
                        pos.column = Constants.numColumns;
                        direction = Direction.LEFT;
                    }
                    Move move = new Move(pos, direction);
                    StartCoroutine(GameManager.instance.MovePiece(move, false, true));
                }
            }
        }
    }
}