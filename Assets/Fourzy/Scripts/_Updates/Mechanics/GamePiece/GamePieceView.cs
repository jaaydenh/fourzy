//modded @vadym udod

using DG.Tweening;
using Fourzy._Updates.Mechanics.Board;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Mechanics._GamePiece
{
    public class GamePieceView : BoardBit
    {
        private const int indexBaseLayer = 0;
        private const int indexEyeMouthLayer = 1;
        
        public Animator pieceAnimator;
        public AnimationCurve movementCurve;

        [HideInInspector]
        public PlayerEnum player { get; set; }
        [HideInInspector]
        public bool isMoving = false;

        private int h_win = Animator.StringToHash("win");
        private int h_Idle = Animator.StringToHash("Idle");
        private int h_Jumping = Animator.StringToHash("Jumping");

        private int h_MovingHorizontal = Animator.StringToHash("MovingHorizontal");
        private int h_MovingVertical = Animator.StringToHash("MovingHorizontal");
        private int h_RightHit = Animator.StringToHash("RightHit");
        private int h_LeftHit = Animator.StringToHash("LeftHit");
        private int h_BottomHit = Animator.StringToHash("BottomHit");
        private int h_TopHit = Animator.StringToHash("TopHit");

        private int h_MoveLeft = Animator.StringToHash("MoveLeft");
        private int h_MoveRight = Animator.StringToHash("MoveRight");
        private int h_MoveTop = Animator.StringToHash("MoveTop");
        private int h_MoveBottom = Animator.StringToHash("MoveDown");

        private MoveDirection moveDirection = MoveDirection.IDLE;

        private CircleCollider2D gamePieceCollider;

        public GamePieceMouth mouth { get; private set; }
        public GamePieceEyes eyes { get; private set; }
        public GamePieceData pieceData { get; set; }

        protected override void Awake()
        {
            base.Awake();

            if (pieceAnimator == null)
                pieceAnimator = GetComponent<Animator>();

            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            parentRectTransform = GetComponentInParent<RectTransform>();
            mouth = GetComponentInChildren<GamePieceMouth>();
            eyes = GetComponentInChildren<GamePieceEyes>();
            gamePieceCollider = gameObject.GetComponent<CircleCollider2D>();
        }

        public void PutMovementDirection(int x, int y)
        {
            moveDirection = ChooseDirection(x, y);

            const float transitionTime = 0.1f;
            switch (moveDirection)
            {
                case MoveDirection.RIGHT:
                    pieceAnimator.CrossFade(h_MovingHorizontal, transitionTime, indexBaseLayer);
                    pieceAnimator.CrossFade(h_MoveRight, transitionTime, indexEyeMouthLayer);
                    break;
                case MoveDirection.LEFT:
                    pieceAnimator.CrossFade(h_MovingHorizontal, transitionTime, indexBaseLayer);
                    pieceAnimator.CrossFade(h_MoveLeft, transitionTime, indexEyeMouthLayer);
                    break;
                case MoveDirection.DOWN:
                    pieceAnimator.CrossFade(h_MovingVertical, transitionTime, indexBaseLayer);
                    pieceAnimator.CrossFade(h_MoveBottom, transitionTime, indexEyeMouthLayer);
                    break;
                case MoveDirection.TOP:
                    pieceAnimator.CrossFade(h_MovingVertical, transitionTime, indexBaseLayer);
                    pieceAnimator.CrossFade(h_MoveTop, transitionTime, indexEyeMouthLayer);
                    break;
            }
        }

        public void PlayFinishMovement(bool animateHit)
        {
            const float transitionTime = 0.03f;
            if (animateHit)
            {
                switch (moveDirection)
                {
                    case MoveDirection.RIGHT:
                        pieceAnimator.CrossFade(h_RightHit, transitionTime, indexBaseLayer);
                        break;
                    case MoveDirection.LEFT:
                        pieceAnimator.CrossFade(h_LeftHit, transitionTime, indexBaseLayer);
                        break;
                    case MoveDirection.DOWN:
                        pieceAnimator.CrossFade(h_BottomHit, transitionTime, indexBaseLayer);
                        break;
                    case MoveDirection.TOP:
                        pieceAnimator.CrossFade(h_TopHit, transitionTime, indexBaseLayer);
                        break;
                }
            }
            else
            {
                pieceAnimator.CrossFade(h_Idle, transitionTime, indexBaseLayer);
            }

            if (pieceAnimator.GetCurrentAnimatorStateInfo(indexEyeMouthLayer).shortNameHash == h_Idle)
                pieceAnimator.Play(h_Idle, indexEyeMouthLayer);
            else
                pieceAnimator.CrossFade(h_Idle, 0.1f, indexEyeMouthLayer);
        }

        private MoveDirection ChooseDirection(int x, int y)
        {
            MoveDirection direction = MoveDirection.IDLE;
            if (x > 0)
                direction = MoveDirection.RIGHT;
            else if (x < 0)
                direction = MoveDirection.LEFT;

            if (y > 0)
                direction = MoveDirection.DOWN;
            else if (y < 0)
                direction = MoveDirection.TOP;

            return direction;
        }

        public void PlayWinAnimation(float delay)
        {
            ShowOutline(false);

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(delay);
            sequence.AppendCallback(() =>
            {
                pieceAnimator.SetBool(h_win, true);
            });
        }

        public void ShowTurnAnimation()
        {
            WakeUp();

            CancelRoutine("jumping");
            StartRoutine("jumping", JumpRoutine(3), () =>
            {
                StartRoutine("blinking", BlinkingRoutine(), null, () => { eyes.SetState(GamePieceEyes.EyesState.OPENED); });
            },
            () =>
            {
                CancelRoutine("blinking");
            });

            ShowOutline(true);
        }

        public void StopTurnAnimation()
        {
            HideOutline(false);

            StopAllCoroutines();

            Sleep();
            pieceAnimator.CrossFade(h_Idle, 0.35f, indexBaseLayer);
        }

        public void ShowUIWinAnimation()
        {
            StopAllCoroutines();
            StartCoroutine(ShowUIWinAnimationRoutine());
        }

        public void Blink()
        {
            eyes.Blink(Random.Range(.25f, .45f));
        }

        public void WakeUp()
        {
            if (!eyes.IsRoutineActive("blink"))
                eyes.SetState(GamePieceEyes.EyesState.OPENED);

            mouth.SetState(GamePieceMouth.MouthState.OPENED);
        }

        public void Sleep()
        {
            eyes.CancelRoutine("blink");
            eyes.SetState(GamePieceEyes.EyesState.CLOSED);

            if (mouth.statesFastAccess.ContainsKey(GamePieceMouth.MouthState.SLEEPY))
                mouth.SetState(GamePieceMouth.MouthState.SLEEPY);
            else
                mouth.SetState(GamePieceMouth.MouthState.CLOSED);
        }

        public void StartBlinking()
        {
            if (!gameObject.activeInHierarchy)
                return;

            StartCoroutine(BlinkingRoutine());
        }

        public void Move(MovingGamePiece mgp, List<IToken> activeTokens)
        {
            StartCoroutine(MoveRoutine(mgp, activeTokens));
        }

        private IEnumerator MoveRoutine(MovingGamePiece mgp, List<IToken> activeTokens)
        {
            gameboard.piecesAnimating++;

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
                PutMovementDirection(columnDir, rowDir);

                Vector3 start = gameboard.PositionToVec3(positions[i]);
                Vector3 end = gameboard.PositionToVec3(positions[nextPos]);

                float distance = Position.Distance(positions[i], positions[nextPos]);

                for (float t = 0; t < 1; t += Time.deltaTime * Constants.moveSpeed / distance)
                {
                    float interpolation = movementCurve.Evaluate(t);
                    transform.localPosition = Vector3.Lerp(start, end, interpolation);

                    CheckActiveTokenCollision(activeTokens);

                    if (nextPos == positions.Count - 1 && t + 2 * Time.deltaTime * Constants.moveSpeed / distance > 1)
                    {
                        PlayFinishMovement(mgp.playHitAnimation);
                    }

                    yield return null;
                }

                transform.localPosition = end;
                start = end;
                i = nextPos - 1;
            }

            isMoving = false;

            Position endPosition = mgp.endPosition;
            gameboard.gamePieces[endPosition.row, endPosition.column] = this;

            CheckActiveTokenCollision(activeTokens);
            CheckTokensAfterMove(activeTokens, endPosition);

            //SetSortingLayer(5 + position.row * 2);
            gameboard.piecesAnimating--;

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
                    gameboard.TokenAt(activeTokens[i].Row, activeTokens[i].Column).Fade(0f, 1.5f);

                    GamePieceView gp = gameboard.GamePieceAt(activeTokens[i].Row, activeTokens[i].Column);
                    if (gp)
                    {
                        gp.Fade(0f, 1f);
                        gameboard.gamePieces[activeTokens[i].Row, activeTokens[i].Column] = null;
                        Destroy(gp.gameObject, 1.0f);
                    }

                    Scale(Vector3.zero, 1f);
                    activeTokens.RemoveAt(i--);
                }
                else if (activeTokens[i].tokenType == Token.CIRCLE_BOMB)
                {
                    gameboard.TokenAt(activeTokens[i].Row, activeTokens[i].Column).Fade(0f, 1.5f);

                    const int bombRadius = 1;
                    int minX = Mathf.Max(activeTokens[i].Row - bombRadius, 0);
                    int minY = Mathf.Max(activeTokens[i].Column - bombRadius, 0);
                    int maxX = Mathf.Min(activeTokens[i].Row + bombRadius, Constants.numRows - 1);
                    int maxY = Mathf.Min(activeTokens[i].Column + bombRadius, Constants.numColumns - 1);

                    for (int x = minX; x <= maxX; x++)
                    {
                        for (int y = minY; y <= maxY; y++)
                        {
                            GamePieceView gp = gameboard.GamePieceAt(x, y);
                            if (gp)
                            {
                                gp.Fade(0f, 1f);
                                gameboard.gamePieces[activeTokens[i].Row, activeTokens[i].Column] = null;
                                Destroy(gp.gameObject, 1.0f);
                            }
                        }
                    }
                    activeTokens.RemoveAt(i--);
                }
            }
        }

        public bool IsOverlapped(GamePieceView gamePiece)
        {
            return gamePieceCollider.Distance(gamePiece.gamePieceCollider).isOverlapped;
        }

        private void CheckActiveTokenCollision(List<IToken> activeTokens)
        {
            if (activeTokens == null)
            {
                return;
            }

            for (int i = 0; i < activeTokens.Count; i++)
            {
                Position piecePos = gameboard.Vec3ToPosition(transform.position);

                if (piecePos.column != activeTokens[i].Column || piecePos.row != activeTokens[i].Row)
                    continue;

                if (activeTokens[i].tokenType == Token.FRUIT)
                {
                    gameboard.TokenAt(activeTokens[i].Row, activeTokens[i].Column).GetComponent<FruitTokenView>().PlayFruitIntoStickyAnimation();
                    activeTokens.RemoveAt(i--);
                }
            }
        }

        private IEnumerator ShowUIWinAnimationRoutine()
        {
            yield return StartCoroutine(JumpRoutine(5));
            //this.PlayWinAnimation(0.0f);
        }

        private IEnumerator JumpRoutine(int count)
        {
            pieceAnimator.Play(h_Jumping);

            while (pieceAnimator.GetCurrentAnimatorStateInfo(indexBaseLayer).shortNameHash != h_Jumping) yield return true;

            AnimatorStateInfo stateInfo = pieceAnimator.GetCurrentAnimatorStateInfo(indexBaseLayer);

            yield return new WaitForSeconds(stateInfo.length * count);

            pieceAnimator.Play(h_Idle, indexBaseLayer);
        }

        private IEnumerator BlinkingRoutine()
        {
            yield return new WaitForSeconds(Random.Range(1f, 5f));

            while (true)
            {
                Blink();
                yield return new WaitForSeconds(Random.Range(5f, 15f));
            }
        }
    }

    enum MoveDirection
    {
        LEFT,
        RIGHT,
        TOP,
        DOWN,
        IDLE
    }
}