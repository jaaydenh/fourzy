using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace Fourzy
{
    public class GamePiece : MonoBehaviour {

        public PlayerEnum player;
        public int column;
        public int row;
        public List<Position> positions;
        public bool isMoveableUp = false;
        public bool isMoveableDown = false;
        public bool isMoveableLeft = false;
        public bool isMoveableRight = false;
        public bool isMoving;
        public bool didAnimateNextPiece;
        public bool animating;
        GameObject nextPiece = null;
        List<MovingGamePiece> movingPieces;
        List<IToken> activeTokens;
        //public AnimationCurve moveCurve;
        Renderer rend;
        CircleCollider2D gamePieceCollider;


		private void Start()
		{
            gamePieceCollider = gameObject.GetComponent<CircleCollider2D>();
		}

		public void Reset()
        {
            player = PlayerEnum.NONE;
            column = 0;
            row = 0;
            //if (positions != null) {
            //    positions.Clear();    
            //}
            isMoveableUp = false;
            isMoveableDown = false;
            isMoveableLeft = false;
            isMoveableRight = false;
            //isMoving = false;
            //didAnimateNextPiece = false;
            //animating = false;
            //nextPiece = null;
            //if (movingPieces != null) {
            //    movingPieces.Clear();    
            //}
            //if (activeTokens != null) {
            //    activeTokens.Clear();    
            //}
        }

        public void SetAlternateColor(bool useAlternateColor) {
            rend = GetComponent<Renderer>();
            if (useAlternateColor)
            {
                rend.material.SetVector("_HSVAAdjust", new Vector4(0.3f, 0, 0, 0));
            }
            else
            {
                rend.material.SetVector("_HSVAAdjust", new Vector4(0, 0, 0, 0));
            }
        }

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

        public IEnumerator MoveGamePiece(List<MovingGamePiece> movingPieces, List<IToken> activeTokens) {
            //Debug.Log("Animatepiece movingpieces count: " + movingPieces.Count);
            //Debug.Log("MoveGamePiece activeTokens count: " + activeTokens.Count);

            if (movingPieces.Count == 0) {
                yield return false;

                throw new UnityException("Method: MoveGamePiece - movingPieces is empty");
                //yield break;
            }
            this.activeTokens = activeTokens;

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

            animating = true;
            //Debug.Log("next piece end column: " + endPosition.column + " row: " + endPosition.row);
            int i = 0;
            do
            {
                float startXPos = (positions[i].column + .1f) * .972f;
                float startYPos = (positions[i].row * -1 + .05f) * .96f;
                Vector3 start = new Vector3(startXPos, startYPos);
                float endXPos = (positions[i + 1].column + .1f) * .972f;
                float endYPos = (positions[i + 1].row * -1 + .05f) * .96f;
                Vector3 end = new Vector3(endXPos, endYPos);
                float distance = Vector3.Distance(start, end);
                //Debug.Log("Animation Start x: " + start.x + " y: " + start.y);
                //Debug.Log("Animation End x: " + end.x + " y: " + end.y);

                float t = 0;
                while (this != null && gameObject != null && t < 1)
                {
                    //t = t + Time.deltaTime;
                    //float percent = Mathf.Clamp01(t / GameManager.instance.moveSpeed);
                    //float curvePercent = moveCurve.Evaluate(percent);
                    //transform.position = Vector3.LerpUnclamped(start, end, percent);

                    t += Time.deltaTime * GameManager.instance.moveSpeed;
                    if (Constants.numRows - distance > 0)
                    {
                        t += (Constants.numRows - distance) / 500;
                    }
                    transform.position = Vector3.Lerp(start, end, t);
                    //transform.position = Vector3.MoveTowards(start, end, t);
                    yield return null;
                }
                if (this != null) {
                    transform.position = end;

                    start = end;
                    i++; 
                }

            } while (this != null && gameObject != null && i < positions.Count - 1);

            while (i < positions.Count-1) {
                yield return null;
            }

            animating = false;
            Direction endMoveDirection = findDirection(positions[positions.Count - 2], positions[positions.Count - 1]);
            Tweener tweener = AfterMovementAnimations(movingGamePiece, endMoveDirection);
            if (tweener != null) {
                yield return tweener.WaitForCompletion();
            }
            //if (player == PlayerEnum.ONE) {
            //    GetComponent<SpriteRenderer>().sprite = GameManager.instance.playerOneSpriteAsleep;
            //} else {
            //    GetComponent<SpriteRenderer>().sprite = GameManager.instance.playerTwoSpriteAsleep;    
            //}

            GameManager.instance.animatingGamePieces = false;

            yield return true;
        }

        void Update()
        {
            //CircleCollider2D collider1 = gameObject.GetComponent<CircleCollider2D>();
            int row1 = GetRowFromPosition(transform.position.y);
            int col1 = GetColumnFromPosition(transform.position.x);

            if (nextPiece != null && !didAnimateNextPiece) {
                if (gamePieceCollider.IsTouching(nextPiece.GetComponent<CircleCollider2D>()))
                {
                    didAnimateNextPiece = true;
                    StartCoroutine(CallAnimatePiece());
                }
            }
            if (activeTokens != null) {
                for (int i = 0; i < activeTokens.Count; i++)
                {
                    Position piecePos = GameManager.instance.GetPositonFromTransform(transform.position);

                    //Debug.Log("nextPiecePosition row: " + piecePos.row + " col: " + piecePos.column);
                    if (piecePos.column == activeTokens[i].Column && piecePos.row == activeTokens[i].Row)
                    {
                        Debug.Log("PIECE IS IN TOKENS POSITION: row: " + piecePos.row + " col: " + piecePos.column + " type: " + activeTokens[i].tokenType);
                        if (activeTokens[i].tokenType == Token.FRUIT) {
                            GameManager.instance.CreateStickyToken(activeTokens[i].Row, activeTokens[i].Column);    
                        } else if (activeTokens[i].tokenType == Token.PIT) {
                            SpriteRenderer sr = GameManager.instance.tokenViews[activeTokens[i].Row, activeTokens[i].Column].GetComponent<SpriteRenderer>();
                            sr.DOFade(0f, 1.5f);
                        }

                        activeTokens.RemoveAt(i);
                    }
                }
            }
        }

        private int GetRowFromPosition(float y) {
            return Mathf.CeilToInt((y * -1 - .3f));
        }

        private int GetColumnFromPosition(float x) {
            return Mathf.RoundToInt(x);
        }

        IEnumerator CallAnimatePiece() {
            GamePiece gamePiece = nextPiece.GetComponent<GamePiece>();
            while (gamePiece.animating)
                yield return null;
            StartCoroutine(gamePiece.MoveGamePiece(movingPieces, this.activeTokens));
        }

        private Tweener AfterMovementAnimations(MovingGamePiece piece, Direction direction) {
            float punchDistance = 0.12f;
            float punchDuration = 0.12f;

            if (piece.animationState == PieceAnimState.FALLING)
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
