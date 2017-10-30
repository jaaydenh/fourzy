using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace Fourzy
{
    public class GamePiece : MonoBehaviour {

        public Player player;
        public int column;
        public int row;
        public List<Position> positions;
        public bool isMoveableUp = false;
        public bool isMoveableDown = false;
        public bool isMoveableLeft = false;
        public bool isMoveableRight = false;
        public bool isMoving;
        public bool didAnimateNextPiece;
        public GameManager gameManager;
        GameObject nextPiece = null;
        List<MovingGamePiece> movingPieces;
        GameBoardView gameBoardView;

        //void OnTriggerEnter2D(Collider2D coll) {
        //    // if (coll.gameObject.tag == "GamePiece" && !isMoving) {
        //    //     StartCoroutine(AnimatePiece());
        //    // }
        //}

        private Direction findDirection(Position start, Position end) {
            if (start.column < end.column) {
                return Direction.RIGHT;
            } else if (start.column > end.column) {
                return Direction.LEFT;
            } else if (start.row < end.row) {
                return Direction.DOWN;
            } else if (start.row > end.row) {
                return Direction.UP;
            }

            return Direction.NONE;
        }

        public IEnumerator AnimatePiece(List<MovingGamePiece> movingPieces) {
            Debug.Log("Animatepiece movingpieces count: " + movingPieces.Count);

            GameManager.instance.animatingGamePieces = true;
            didAnimateNextPiece = false;

            var movingGamePiece = movingPieces[0];
            positions = movingGamePiece.positions;

            if (movingPieces.Count > 1) {
                movingPieces.RemoveAt(0);
                this.movingPieces = movingPieces;
            }

            Position endPosition = movingGamePiece.positions[movingGamePiece.positions.Count - 1];
            nextPiece = GameManager.instance.gameBoardView.gamePieces[endPosition.row, endPosition.column];

            Sequence mySequence = DOTween.Sequence();

            int i = 0;
            do
            {
                Vector3 start = new Vector3(positions[i].column, positions[i].row * -1);
                Vector3 end = new Vector3(positions[i + 1].column, positions[i + 1].row * -1);
                //Debug.Log("start column: " + column + " row: " + row * -1);
                //Debug.Log("end column: " + positions[i].column + " row: " + positions[i].row * -1);
                //mySequence.Append(transform.DOMove(end, dropTime, false).SetEase(Ease.Linear));
                float distance = Vector3.Distance(start, end);
                float t = 0;
                while (t < 1)
                {
                    t += Time.deltaTime * GameManager.instance.moveSpeed;
                    if (Constants.numRows - distance > 0)
                    {
                        t += (Constants.numRows - distance) / 500;
                    }
                    transform.position = Vector3.Lerp(start, end, t);

                    yield return null;
                }
                start = end;
                i++;
            } while (i < positions.Count - 1);

            while (i < positions.Count-1) {
                yield return null;
            }

            //StartCoroutine(AfterMovementAnimations(mySequence, movingGamePiece));
            Direction endMoveDirection = findDirection(positions[positions.Count - 2], positions[positions.Count - 1]);
            Tweener tweener = AfterMovementAnimations(movingGamePiece, endMoveDirection);
            if (tweener != null) {
                yield return tweener.WaitForCompletion();
            }
            if (movingGamePiece.isDestroyed)
            {
                // pieceView.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                // Lean.LeanPool.Despawn(pieceView);
            }
            else
            {
                // Update the state of the game board views
                GameManager.instance.gameBoardView.gamePieces[endPosition.row, endPosition.column] = gameObject;
            }

            GameManager.instance.animatingGamePieces = false;
        }

        void Update()
        {
            CircleCollider2D col = gameObject.GetComponent<CircleCollider2D>();

            if (nextPiece != null && !didAnimateNextPiece) {
                if (col.IsTouching(nextPiece.GetComponent<CircleCollider2D>()))
                {
                    didAnimateNextPiece = true;
                    GamePiece gamePiece = nextPiece.GetComponent<GamePiece>();
                    StartCoroutine(gamePiece.AnimatePiece(movingPieces));
                }
            }

        }

        private Tweener AfterMovementAnimations(MovingGamePiece piece, Direction direction) {
            float punchDistance = 0.12f;
            float punchDuration = 0.12f;

            if (piece.animationState == PieceAnimStates.DROPPING)
            {
                return transform.DOScale(0.0f, 1.0f);
            } else {
                switch (direction)
                {
                    case Direction.UP:
                        return transform.DOPunchPosition(new Vector3(0.0f, punchDistance, 0), punchDuration, 7);
                    case Direction.DOWN:
                        return transform.DOPunchPosition(new Vector3(0.0f, punchDistance * -1, 0), punchDuration, 7);
                    case Direction.LEFT:
                        return transform.DOPunchPosition(new Vector3(punchDistance * -1, 0.0f, 0), punchDuration, 7);
                    case Direction.RIGHT:
                        return transform.DOPunchPosition(new Vector3(punchDistance, 0.0f, 0), punchDuration, 7);
                    default:
                        return transform.DOPunchPosition(new Vector3(0.0f, 0.0f, 0), punchDuration, 7);
                }

            }
        }

        public void MakeMoveable(bool moveable, Direction direction) {
            switch (direction)
            {
                case Direction.UP:
                    isMoveableUp = moveable;
                    break;
                case Direction.DOWN:
                    isMoveableDown = moveable;
                    break;
                case Direction.LEFT:
                    isMoveableLeft = moveable;
                    break;
                case Direction.RIGHT:
                    isMoveableRight = moveable;
                    break;
                default:
                    break;
            }
        }
    }
}
