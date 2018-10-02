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

        [SerializeField]
        private AnimationCurve movementCurve;

        public int gamePieceID;
        public Sprite gamePieceIcon;
        public PlayerEnum player;
        public int column;
        public int row;
        public GameBoardView gameBoardView;

        public bool isMoving;

        private CircleCollider2D gamePieceCollider;

        public Transform CachedTransform { get; private set; }
        public GameObject CachedGO { get; private set; }

		private void Awake()
		{
            gamePieceCollider = gameObject.GetComponent<CircleCollider2D>();
            CachedTransform = this.transform;
            View = this.GetComponent<GamePieceView>();
            CachedGO = this.gameObject;

            View.SetupZOrder(5);
		}

        public void Move(List<MovingGamePiece> movingPieces, List<IToken> activeTokens, bool firstPiece = true)
        {
            this.StartCoroutine(MoveRoutine(movingPieces, activeTokens, firstPiece));
        }

        private IEnumerator MoveRoutine(List<MovingGamePiece> movingPieces, List<IToken> activeTokens, bool firstPiece) 
        {
            if (movingPieces.Count == 0) 
            {
                Debug.LogError("movingPieces = 0");
                yield return false;

                throw new UnityException("Method: MoveGamePiece - movingPieces is empty");
            }

            View.SetupZOrder(5);

            gameBoardView.NumPiecesAnimating++;

            var movingGamePiece = movingPieces[0];
            movingPieces.RemoveAt(0);

            List<Position> positions = movingGamePiece.positions;
            Position endPosition = positions[positions.Count - 1];
            GamePiece nextPiece = gameBoardView.GamePieceAt(endPosition);

            gameBoardView.gamePieces[endPosition.row, endPosition.column] = this;

            isMoving = true;

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

                Vector3 start = gameBoardView.PositionToVec3(positions[i]);
                Vector3 end = gameBoardView.PositionToVec3(positions[nextPos]);

                float distance = Position.Distance(positions[i], positions[nextPos]);

                for (float t = 0; t < 1; t += Time.deltaTime * Constants.moveSpeed / distance)
                {
                    float interpolation = movementCurve.Evaluate(t);
                    CachedTransform.position = Vector3.Lerp(start, end, interpolation);

                    this.CheckNextPieceCollision(nextPiece, movingPieces, activeTokens);
                    this.CheckActiveTokenCollision(activeTokens);

                    if (nextPos == positions.Count - 1 && t + 2 * Time.deltaTime * Constants.moveSpeed / distance > 1)
                    {
                        bool playHitAnimation = ((nextPiece == null && positions.Count > 2) || firstPiece);
                        View.PlayFinishMovement(playHitAnimation);
                    }

                    yield return null;
                }

                CachedTransform.position = end;
                start = end;
                i = nextPos - 1;
            }

            isMoving = false;
            column = positions[positions.Count - 1].column;
            row = positions[positions.Count - 1].row;

            this.CheckActiveTokenCollision(activeTokens);
            this.CheckNextPieceCollision(nextPiece, movingPieces, activeTokens);

            View.SetupZOrder(5 + row * 2);

            gameBoardView.NumPiecesAnimating--;

            yield return true;
        }

        private void CheckNextPieceCollision(GamePiece nextPiece, List<MovingGamePiece> movingPieces, List<IToken> activeTokens)
        {
            if (nextPiece == null || nextPiece.isMoving)
            {
                return;
            }

            if (gamePieceCollider.Distance(nextPiece.gamePieceCollider).isOverlapped)
            {
                // Animate punch and hit for both pieces

                nextPiece.Move(movingPieces, activeTokens, false);
            }
        }

        private void CheckActiveTokenCollision(List<IToken> activeTokens)
        {
            if (activeTokens == null)
            {
                return;
            }

            for (int i = 0; i < activeTokens.Count; i++)
            {
                Position piecePos = gameBoardView.Vec3ToPosition(transform.position);

                if (piecePos.column == activeTokens[i].Column && piecePos.row == activeTokens[i].Row)
                {
                    if (activeTokens[i].tokenType == Token.FRUIT)
                    {
                        gameBoardView.TokenAt(activeTokens[i].Row, activeTokens[i].Column).GetComponent<FruitTokenView>().PlayFruitIntoStickyAnimation();
                    }
                    else if (activeTokens[i].tokenType == Token.PIT)
                    {
                        SpriteRenderer sr = gameBoardView.TokenAt(activeTokens[i].Row, activeTokens[i].Column).GetComponent<SpriteRenderer>();
                        sr.DOFade(0f, 1.5f);

                        this.View.FadeAfterPit();
                    }

                    activeTokens.RemoveAt(i);
                }
            }
        }

    }
}
