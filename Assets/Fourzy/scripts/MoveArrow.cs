﻿using UnityEngine;

namespace Fourzy
{
    public class MoveArrow : MonoBehaviour {

        public Direction direction;
        bool mouseButtonPressed = false;

        void OnMouseDown() {
            if (!mouseButtonPressed && GameManager.Instance.activeGame.gameState.isCurrentPlayerTurn)
            {
                mouseButtonPressed = true;
                int row = gameObject.GetComponentInParent<CornerSpot>().row;
                int column = gameObject.GetComponentInParent<CornerSpot>().column;
                Position pos = new Position(column, row);
                if (direction == Direction.DOWN) {
                    pos.row = -1;
                } else if (direction == Direction.UP) {
                    pos.row = Constants.numRows;
                } else if (direction == Direction.RIGHT) {
                    pos.column = -1;
                } else if (direction == Direction.LEFT) {
                    pos.column = Constants.numColumns;
                }

                PlayerEnum player = GameManager.Instance.activeGame.gameState.IsPlayerOneTurn ? PlayerEnum.ONE : PlayerEnum.TWO;
                Move move = new Move(pos, direction, player);

                StartCoroutine(GamePlayManager.Instance.ProcessMove(move));
            }
        }
            
        void Start () {
            GamePlayManager.OnStartMove += setMouseButtonPressed;
        }

        void setMouseButtonPressed() {
            mouseButtonPressed = false;
        }
    }
}
