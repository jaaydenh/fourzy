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

        public Transform CachedTransform { get; private set; }
        public GameObject CachedGO { get; private set; }

		private void Awake()
		{
            gamePieceCollider = gameObject.GetComponent<CircleCollider2D>();
            CachedTransform = this.transform;
            View = this.GetComponent<GamePieceView>();
            CachedGO = this.gameObject;
		}

        private void Start()
        {
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

            if (activeTokens != null)
            {

                for (int t = 0; t < activeTokens.Count; t++)
                {
                    Position piecePos = gameBoardView.Vec3ToPosition(transform.position);

                    if (piecePos.column == activeTokens[t].Column && piecePos.row == activeTokens[t].Row)
                    {


                        if (activeTokens[t].tokenType == Token.PIT)
                        {
                            SpriteRenderer sr = gameBoardView.TokenAt(activeTokens[t].Row, activeTokens[t].Column).GetComponent<SpriteRenderer>();
                            sr.DOFade(0f, 1.5f);

                            GamePiece gp = gameBoardView.GamePieceAt(activeTokens[t].Row, activeTokens[t].Column);
                            if (gp)
                            {
                                gp.View.Fade(0f, 1f);
                            }

                            this.View.FadeAfterPit();
                        }
                        else if (activeTokens[t].tokenType == Token.CIRCLE_BOMB)
                        {
                            SpriteRenderer sr = gameBoardView.TokenAt(activeTokens[t].Row, activeTokens[t].Column).GetComponent<SpriteRenderer>();
                            sr.DOFade(0f, 1.5f);

                            if (activeTokens[t].Row > 0)
                            {
                                // TopMiddle
                                GamePiece gp = gameBoardView.GamePieceAt(activeTokens[t].Row - 1, activeTokens[t].Column);
                                if (gp)
                                {
                                    gp.View.Fade(0f, 1f);
                                }

                                if (activeTokens[t].Column > 0)
                                {
                                    //TopLeft
                                    gp = gameBoardView.GamePieceAt(activeTokens[t].Row - 1, activeTokens[t].Column - 1);
                                    if (gp)
                                    {
                                        gp.View.Fade(0f, 1f);
                                    }

                                    //Left
                                    gp = gameBoardView.GamePieceAt(activeTokens[t].Row, activeTokens[t].Column - 1);
                                    if (gp)
                                    {
                                        gp.View.Fade(0f, 1f);
                                    }
                                }
                                if (activeTokens[t].Column < Constants.numColumns - 1)
                                {
                                    //TopRight
                                    gp = gameBoardView.GamePieceAt(activeTokens[t].Row - 1, activeTokens[t].Column + 1);
                                    if (gp)
                                    {
                                        gp.View.Fade(0f, 1f);
                                    }
                                    //Right
                                    gp = gameBoardView.GamePieceAt(activeTokens[t].Row, activeTokens[t].Column + 1);
                                    if (gp)
                                    {
                                        gp.View.Fade(0f, 1f);
                                    }
                                }
                            }

                            if (activeTokens[t].Row < Constants.numRows - 1)
                            {
                                //Bottom
                                GamePiece gp = gameBoardView.GamePieceAt(activeTokens[t].Row + 1, activeTokens[t].Column);
                                if (gp)
                                {
                                    gp.View.Fade(0f, 1f);
                                }

                                //BottomLeft
                                if (activeTokens[t].Column > 0)
                                {
                                    gp = gameBoardView.GamePieceAt(activeTokens[t].Row + 1, activeTokens[t].Column - 1);
                                    if (gp)
                                    {
                                        gp.View.Fade(0f, 1f);
                                    }
                                }
                                //BottomRight
                                if (activeTokens[t].Column < Constants.numColumns - 1)
                                {
                                    gp = gameBoardView.GamePieceAt(activeTokens[t].Row + 1, activeTokens[t].Column + 1);
                                    if (gp)
                                    {
                                        gp.View.Fade(0f, 1f);
                                    }
                                }
                            }

                            this.View.Fade(0f, 1.0f);
                        }
                    }
                }
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
                   

                    activeTokens.RemoveAt(i);
                }
            }
        }

    }
}
