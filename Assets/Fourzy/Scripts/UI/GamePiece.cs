﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace Fourzy
{
    public class GamePiece : MonoBehaviour 
    {
        [SerializeField]
        public GamePieceView View;

        public PlayerEnum player;
        public int column;
        public int row;
        //public List<Position> positions;
        public bool isMoveableUp = false;
        public bool isMoveableDown = false;
        public bool isMoveableLeft = false;
        public bool isMoveableRight = false;
        public bool isMoving;
        public bool animating;


        private CircleCollider2D gamePieceCollider;

        private Transform cachedTransform;


		private void Awake()
		{
            gamePieceCollider = gameObject.GetComponent<CircleCollider2D>();
            cachedTransform = this.transform;
            View = this.GetComponent<GamePieceView>();
		}

		//public void Reset()
        //{
        //    player = PlayerEnum.NONE;
        //    column = 0;
        //    row = 0;
        //    //if (positions != null) {
        //    //    positions.Clear();    
        //    //}
        //    isMoveableUp = false;
        //    isMoveableDown = false;
        //    isMoveableLeft = false;
        //    isMoveableRight = false;
        //    //isMoving = false;
        //    //didAnimateNextPiece = false;
        //    //animating = false;
        //    //nextPiece = null;
        //    //if (movingPieces != null) {
        //    //    movingPieces.Clear();    
        //    //}
        //    //if (activeTokens != null) {
        //    //    activeTokens.Clear();    
        //    //}
        //}

        public void SetupPlayer(PlayerEnum playerEnum, PieceAnimState animState)
        {          
            this.player = playerEnum;

            if (playerEnum == PlayerEnum.TWO)
            {
                View.SetupHSVColor(new Vector4(0.3f, 0, 0, 0));
            }
            else
            {
                View.SetupHSVColor(new Vector4(0, 0, 0, 0));
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

        public IEnumerator MoveGamePiece(List<MovingGamePiece> movingPieces, List<IToken> activeTokens) 
        {
            if (movingPieces.Count == 0) 
            {
                Debug.LogError("movingPieces = 0");
                yield return false;

                throw new UnityException("Method: MoveGamePiece - movingPieces is empty");
            }

            GamePlayManager.Instance.numPiecesAnimating++;

            var movingGamePiece = movingPieces[0];
            movingPieces.RemoveAt(0);

            List<Position> positions = movingGamePiece.positions;

            Position endPosition = movingGamePiece.positions[movingGamePiece.positions.Count - 1];

            GamePiece nextPiece = GamePlayManager.Instance.gameBoardView.gamePieces[endPosition.row, endPosition.column];

            if (movingGamePiece.isDestroyed)
            {
                // pieceView.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                // Lean.LeanPool.Despawn(pieceView);
            }
            else
            {
                // Update the state of the game board views
                GamePlayManager.Instance.gameBoardView.gamePieces[endPosition.row, endPosition.column] = this;
            }

            Direction endMoveDirection = findDirection(positions[positions.Count - 2], positions[positions.Count - 1]);

            animating = true;

            View.PlayMovement();

            for (int i = 0; i < positions.Count - 1; i++)
            {
                int nextPos = i + 1;
                int columnDir = positions[nextPos].column - positions[i].column;
                int rowDir = positions[nextPos].row - positions[i].row;
                for (int j = i + 1; j < positions.Count; j++)
                {
                    int newColumnDir = positions[j].column - positions[j-1].column;
                    int newRowDir = positions[j].row - positions[j-1].row;
                    bool isDirectionChanged = (newColumnDir != columnDir || newRowDir != rowDir);
                    if (isDirectionChanged)
                    {
                        break;
                    }
                    nextPos = j;
                }
                View.PutMovementDirection(columnDir, rowDir);
               
                Vector3 start = positions[i].ConvertToVec3();
                Vector3 end = positions[nextPos].ConvertToVec3();

                float distance = Vector3.Distance(start, end);

                for (float t = 0; t < 1; t += Time.deltaTime * Constants.moveSpeed / distance)
                {
                    float interpolation = View.movementCurve.Evaluate(t);
                    cachedTransform.position = Vector3.Lerp(start, end, interpolation);

                    this.CheckNextPieceCollision(nextPiece, movingPieces, activeTokens);
                    this.CheckActiveTokenCollision(activeTokens);

                    yield return null;
                }

                cachedTransform.position = end;
                start = end;
                i = nextPos - 1;
            }

            animating = false;

            View.PlayFinishMovement();

            //Tweener tweener = AfterMovementAnimations(movingGamePiece, endMoveDirection);
            //if (tweener != null)
            //{
            //    yield return tweener.WaitForCompletion();
            //}

            View.SetupAsleep();

            GamePlayManager.Instance.numPiecesAnimating--;

            yield return true;
        }

        private void CheckNextPieceCollision(GamePiece nextPiece, List<MovingGamePiece> movingPieces, List<IToken> activeTokens)
        {
            if (nextPiece != null && !nextPiece.animating && gamePieceCollider.bounds.Intersects(nextPiece.gamePieceCollider.bounds))
            {
                // Animate punch and hit for both pieces

                nextPiece.StartCoroutine(nextPiece.MoveGamePiece(movingPieces, activeTokens));
            }
        }

        private void CheckActiveTokenCollision(List<IToken> activeTokens)
        {
            if (activeTokens == null)
            {
                return;
            }

            int row1 = GetRowFromPosition(transform.position.y);
            int col1 = GetColumnFromPosition(transform.position.x);

            for (int i = 0; i < activeTokens.Count; i++)
            {
                Position piecePos = Utility.GetPositonFromTransform(transform.position);

                //Debug.Log("nextPiecePosition row: " + piecePos.row + " col: " + piecePos.column);
                if (piecePos.column == activeTokens[i].Column && piecePos.row == activeTokens[i].Row)
                {
                    Debug.Log("PIECE IS IN TOKENS POSITION: row: " + piecePos.row + " col: " + piecePos.column + " type: " + activeTokens[i].tokenType);
                    if (activeTokens[i].tokenType == Token.FRUIT)
                    {
                        GamePlayManager.Instance.CreateStickyToken(activeTokens[i].Row, activeTokens[i].Column);
                    }
                    else if (activeTokens[i].tokenType == Token.PIT)
                    {
                        SpriteRenderer sr = GamePlayManager.Instance.tokenViews[activeTokens[i].Row, activeTokens[i].Column].GetComponent<SpriteRenderer>();
                        sr.DOFade(0f, 1.5f);

                        // GamePiece animation
                    }

                    activeTokens.RemoveAt(i);
                }
            }
        }


        private int GetRowFromPosition(float y) {
            return Mathf.CeilToInt((y * -1 - .3f));
        }

        private int GetColumnFromPosition(float x) {
            return Mathf.RoundToInt(x);
        }

        private Tweener AfterMovementAnimations(MovingGamePiece piece, Direction direction) 
        {
            float punchDistance = 0.12f;
            float punchDuration = 0.12f;

            if (piece.animationState == PieceAnimState.FALLING)
            {
                return cachedTransform.DOScale(0.0f, 1.0f);
            } 
            else 
            {
                switch (direction)
                {
                    case Direction.UP:
                        //gamePieceAnimator.Play("movetest");
                        return cachedTransform.DOPunchPosition(new Vector3(0.0f, punchDistance, 0), punchDuration, 7);
                    case Direction.DOWN:
                        //gamePieceAnimator.Play("movetest");
                        return cachedTransform.DOPunchPosition(new Vector3(0.0f, punchDistance * -1, 0), punchDuration, 7);
                    case Direction.LEFT:
                        //gamePieceAnimator.Play("movetest");
                        return cachedTransform.DOPunchPosition(new Vector3(punchDistance * -1, 0.0f, 0), punchDuration, 7);
                    case Direction.RIGHT:
                        //gamePieceAnimator.Play("movetest");
                        return cachedTransform.DOPunchPosition(new Vector3(punchDistance, 0.0f, 0), punchDuration, 7);
                    default:
                        return cachedTransform.DOPunchPosition(new Vector3(0.0f, 0.0f, 0), punchDuration, 7);
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
