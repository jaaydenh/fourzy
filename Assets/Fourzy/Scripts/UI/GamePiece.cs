using UnityEngine;
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

        public Vector4 SecondaryColor = Vector4.zero;
        public PlayerEnum player;
        public int column;
        public int row;

        public bool isMoving;

        private CircleCollider2D gamePieceCollider;

        private Transform cachedTransform;


		private void Awake()
		{
            gamePieceCollider = gameObject.GetComponent<CircleCollider2D>();
            cachedTransform = this.transform;
            View = this.GetComponent<GamePieceView>();
		}

        public void SetupPlayer(PlayerEnum playerEnum, PieceAnimState animState)
        {          
            this.player = playerEnum;

            if (playerEnum == PlayerEnum.TWO)
            {
                View.SetupHSVColor(SecondaryColor);
            }
            else
            {
                View.SetupHSVColor(Vector4.zero);
            }
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

            isMoving = true;

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
                    float interpolation = movementCurve.Evaluate(t);
                    cachedTransform.position = Vector3.Lerp(start, end, interpolation);

                    this.CheckNextPieceCollision(nextPiece, movingPieces, activeTokens);
                    this.CheckActiveTokenCollision(activeTokens);

                    if (nextPos == positions.Count - 1 && t + 2 * Time.deltaTime * Constants.moveSpeed / distance > 1)
                    {
                        bool playHitAnimation = (nextPiece == null && (firstPiece || positions.Count > 2));
                        playHitAnimation = ((nextPiece == null && positions.Count > 2) || firstPiece);
                        View.PlayFinishMovement(playHitAnimation);
                    }

                    yield return null;
                }

                cachedTransform.position = end;
                start = end;
                i = nextPos - 1;
            }

            isMoving = false;
            column = positions[positions.Count - 1].column;
            row = positions[positions.Count - 1].row;

            this.CheckActiveTokenCollision(activeTokens);
            this.CheckNextPieceCollision(nextPiece, movingPieces, activeTokens);

            View.SetupZOrder(5 + row * 2);

            GamePlayManager.Instance.numPiecesAnimating--;

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
                Position piecePos = Position.Vec3ToPosition(transform.position);

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

                        this.View.FadeAfterPit();
                    }

                    activeTokens.RemoveAt(i);
                }
            }
        }

    }
}
