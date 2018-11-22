//modded @vadym udod

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

        public int gamePieceID;
        public Sprite gamePieceIcon;
        public PlayerEnum player;
        public int column;
        public int row;
        public GameBoardView gameBoardView;

        public bool isMoving;

        private CircleCollider2D gamePieceCollider;

		private void Awake()
		{
            gamePieceCollider = gameObject.GetComponent<CircleCollider2D>();
            View = GetComponent<GamePieceView>();
		}

        private void Start()
        {
            View.SetupZOrder(5);
        }

        public void Move(MovingGamePiece mgp, List<IToken> activeTokens)
        {
            this.StartCoroutine(MoveRoutine(mgp, activeTokens));
        }

        private IEnumerator MoveRoutine(MovingGamePiece mgp, List<IToken> activeTokens)
        {
            gameBoardView.NumPiecesAnimating++;

            List<Position> positions = mgp.positions;

            isMoving = true;

            for (int i = 0; i < positions.Count - 1; i++)
            {
                int nextPos = i + 1;
                int columnDir = positions[nextPos].column - positions[i].column;
                int rowDir = positions[nextPos].row - positions[i].row;
                for (int j = i + 1; j < positions.Count; j++)
                {
                    int newColumnDir = positions[j].column - positions[j - 1].column;
                    int newRowDir = positions[j].row - positions[j - 1].row;
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
                    transform.position = Vector3.Lerp(start, end, interpolation);

                    CheckActiveTokenCollision(activeTokens);

                    if (nextPos == positions.Count - 1 && t + 2 * Time.deltaTime * Constants.moveSpeed / distance > 1)
                    {
                        View.PlayFinishMovement(mgp.playHitAnimation);
                    }

                    yield return null;
                }

                transform.position = end;
                start = end;
                i = nextPos - 1;
            }

            isMoving = false;

            Position endPosition = mgp.endPosition;
            gameBoardView.gamePieces[endPosition.row, endPosition.column] = this;
            column = endPosition.column;
            row = endPosition.row;

            this.CheckActiveTokenCollision(activeTokens);
            this.CheckTokensAfterMove(activeTokens, endPosition);

            View.SetupZOrder(5 + row * 2);

            gameBoardView.NumPiecesAnimating--;

            yield return true;
        }

        private void CheckTokensAfterMove(List<IToken> activeTokens, Position piecePos)
        {
            if (activeTokens == null)
            {
                return;
            }

            for (int i = 0; i < activeTokens.Count; i++)
            {
                if (piecePos.column != activeTokens[i].Column || piecePos.row != activeTokens[i].Row)
                {
                    continue;
                }

                if (activeTokens[i].tokenType == Token.PIT)
                {
                    SpriteRenderer sr = gameBoardView.TokenAt(activeTokens[i].Row, activeTokens[i].Column).GetComponent<SpriteRenderer>();
                    sr.DOFade(0f, 1.5f);

                    GamePiece gp = gameBoardView.GamePieceAt(activeTokens[i].Row, activeTokens[i].Column);
                    if (gp)
                    {
                        gp.View.Fade(0f, 1f);
                        gameBoardView.gamePieces[activeTokens[i].Row, activeTokens[i].Column] = null;
                        Destroy(gp.gameObject, 1.0f);
                    }

                    this.View.FadeAfterPit();
                    activeTokens.RemoveAt(i--);
                }
                else if (activeTokens[i].tokenType == Token.CIRCLE_BOMB)
                {
                    SpriteRenderer sr = gameBoardView.TokenAt(activeTokens[i].Row, activeTokens[i].Column).GetComponent<SpriteRenderer>();
                    sr.DOFade(0f, 1.5f);

                    const int bombRadius = 1;
                    int minX = Mathf.Max(activeTokens[i].Row - bombRadius, 0);
                    int minY = Mathf.Max(activeTokens[i].Column - bombRadius, 0);
                    int maxX = Mathf.Min(activeTokens[i].Row + bombRadius, Constants.numRows - 1);
                    int maxY = Mathf.Min(activeTokens[i].Column + bombRadius, Constants.numColumns - 1);

                    for (int x = minX; x <= maxX; x++)
                    {
                        for (int y = minY; y <= maxY; y++)
                        {
                            GamePiece gp = gameBoardView.GamePieceAt(x, y);
                            if (gp)
                            {
                                gp.View.Fade(0f, 1f);
                                gameBoardView.gamePieces[activeTokens[i].Row, activeTokens[i].Column] = null;
                                Destroy(gp.gameObject, 1.0f);
                            }
                        }
                    }
                    activeTokens.RemoveAt(i--);
                }
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

                if (piecePos.column != activeTokens[i].Column || piecePos.row != activeTokens[i].Row)
                {
                    continue;
                }

                if (activeTokens[i].tokenType == Token.FRUIT)
                {
                    gameBoardView.TokenAt(activeTokens[i].Row, activeTokens[i].Column).GetComponent<FruitTokenView>().PlayFruitIntoStickyAnimation();
                    activeTokens.RemoveAt(i--);
                }
            }
        }

        public bool IsOverlapped(GamePiece gamePiece)
        {
            return gamePieceCollider.Distance(gamePiece.gamePieceCollider).isOverlapped;
        }

    }
}
