using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

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
        public float dropTime = 1.0f;
        public bool isMoving;
        public GameManager gameManager;
        GameObject nextPiece = null;

        void OnTriggerEnter2D(Collider2D coll) {
            // if (coll.gameObject.tag == "GamePiece" && !isMoving) {
            //     StartCoroutine(AnimatePiece());
            // }
        }

        public void AnimatePiece(Move move, List<MovingGamePiece> movingPieces, GameBoardView gameBoardView) {
            var movingGamePiece = movingPieces[0];
            Position endPosition = movingGamePiece.positions[movingGamePiece.positions.Count - 1];
            nextPiece = gameBoardView.gamePieces[endPosition.row, endPosition.column];
        }

        void Update()
        {
            Rigidbody2D rb2d = gameObject.GetComponent<Rigidbody2D>();
            //if (rb2d.IsTouching())
        }

        //private IEnumerator AnimatePiece() {
        //    isMoving = true;
        //    gameManager.isAnimating = true;
        //    Sequence mySequence = DOTween.Sequence();
        //    for (int i = 0; i < positions.Count; i++)
        //    {
        //        Vector3 end = new Vector3(positions[i].column, positions[i].row * -1);
        //        mySequence.Append(gameObject.GetComponent<Rigidbody2D>().DOMove(end, dropTime, false).SetEase(Ease.Linear));
        //    }
        //    mySequence.Play();

        //    yield return mySequence.WaitForCompletion();
        //    isMoving = false;
        //    gameManager.isAnimating = false;
        //}

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
