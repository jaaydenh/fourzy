//modded @vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.Mechanics._GamePiece
{
    public class GamePieceView : BoardBit
    {
        private const int indexBaseLayer = 0;
        private const int indexEyeMouthLayer = 1;

        public Animator pieceAnimator;
        public AnimationCurve movementCurve;

        public string moveSfx = "gamepiece_move";

        private int h_win = Animator.StringToHash("win");
        private int h_Idle = Animator.StringToHash("Idle");
        private int h_Jumping = Animator.StringToHash("Jumping");

        private int h_MovingHorizontal = Animator.StringToHash("MovingHorizontal");
        private int h_MovingVertical = Animator.StringToHash("MovingVertical");

        private int h_RightHit = Animator.StringToHash("RightHit");
        private int h_LeftHit = Animator.StringToHash("LeftHit");
        private int h_BottomHit = Animator.StringToHash("BottomHit");
        private int h_TopHit = Animator.StringToHash("TopHit");

        private int h_RightSmash = Animator.StringToHash("SmashRight");
        private int h_LeftSmash = Animator.StringToHash("SmashLeft");
        private int h_BottomSmash = Animator.StringToHash("SmashDown");
        private int h_TopSmash = Animator.StringToHash("SmashTop");

        private int h_MoveLeft = Animator.StringToHash("MoveLeft");
        private int h_MoveRight = Animator.StringToHash("MoveRight");
        private int h_MoveTop = Animator.StringToHash("MoveTop");
        private int h_MoveBottom = Animator.StringToHash("MoveDown");

        private Direction moveDirection = Direction.NONE;

        public GamePieceData pieceData => GamePiecesDataHolder._GetGamePieceData(this);

        public GamePieceMouth mouth { get; private set; }
        public GamePieceEyes eyes { get; private set; }
        public Piece piece { get; private set; }
        public List<BoardBit> interactions { get; private set; } = new List<BoardBit>();

        public override Color outlineColor => pieceData.outlineColor;

        protected override void Awake()
        {
            base.Awake();

            if (pieceAnimator == null)
            {
                pieceAnimator = GetComponent<Animator>();
                pieceAnimator.keepAnimatorControllerStateOnDisable = true;
            }

            mouth = GetComponentInChildren<GamePieceMouth>();
            eyes = GetComponentInChildren<GamePieceEyes>();
        }

        protected void OnEnable()
        {
            if (IsRoutineActive("blinking"))
            {
                CancelRoutine("blinking");
                StartBlinking();
            }
        }

        public override void OnBeforeTurn(bool startTurn)
        {
            base.OnBeforeTurn(startTurn);

            interactions.Clear();
        }

        public override void OnAfterTurn(bool startTurn)
        {
            base.OnAfterTurn(startTurn);

            interactions.Clear();
        }

        public override void OnEnter(BoardLocation location)
        {
            base.OnEnter(location);

            IEnumerable<BoardBit> _interactions = gameboard.BoardTokensAt(location);
            interactions.AddRange(_interactions);

            foreach (TokenView _token in _interactions)
            {
                switch (_token.Token.Type)
                {
                    case TokenType.ARROW:
                    case TokenType.ROTATING_ARROW:
                    case TokenType.FOURWAY_ARROW:
                        speedMltp += .1f;

                        break;
                }
            }
        }

        public IEnumerable<T> InteractionsWithToken<T>(TokenType type) where T : TokenView
        {
            return interactions.Where(_bit => (_bit.GetType() == typeof(T) || _bit.GetType().IsSubclassOf(typeof(T))) && ((T)_bit).Token.Type == type).Cast<T>();
        }

        public void PutMovementDirection(Direction direction)
        {
            moveDirection = direction;

            const float transitionTime = 0.1f;

            switch (direction)
            {
                case Direction.RIGHT:
                    pieceAnimator.CrossFade(h_MovingHorizontal, transitionTime, indexBaseLayer);
                    pieceAnimator.CrossFade(h_MoveRight, transitionTime, indexEyeMouthLayer);

                    break;

                case Direction.LEFT:
                    pieceAnimator.CrossFade(h_MovingHorizontal, transitionTime, indexBaseLayer);
                    pieceAnimator.CrossFade(h_MoveLeft, transitionTime, indexEyeMouthLayer);

                    break;

                case Direction.DOWN:
                    pieceAnimator.CrossFade(h_MovingVertical, transitionTime, indexBaseLayer);
                    pieceAnimator.CrossFade(h_MoveBottom, transitionTime, indexEyeMouthLayer);

                    break;

                case Direction.UP:
                    pieceAnimator.CrossFade(h_MovingVertical, transitionTime, indexBaseLayer);
                    pieceAnimator.CrossFade(h_MoveTop, transitionTime, indexEyeMouthLayer);

                    break;
            }
        }

        public void PlaySmashAnimation(Direction direction)
        {
            gameboard.OnGamepieceSmashed(this);
            AudioHolder.instance.PlaySelfSfxOneShotTracked("gamepiece_smash");

            const float transitionTime = 0.1f;

            switch (direction)
            {
                case Direction.DOWN:
                    pieceAnimator.CrossFade(h_BottomSmash, transitionTime, indexBaseLayer);

                    break;

                case Direction.UP:
                    pieceAnimator.CrossFade(h_TopSmash, transitionTime, indexBaseLayer);

                    break;

                case Direction.RIGHT:
                    pieceAnimator.CrossFade(h_RightSmash, transitionTime, indexBaseLayer);

                    break;

                case Direction.LEFT:
                    pieceAnimator.CrossFade(h_LeftSmash, transitionTime, indexBaseLayer);

                    break;
            }

            PlayFinishAnimation();
        }

        public void PlayFinishMovement(bool animateHit)
        {
            const float transitionTime = 0.03f;
            if (animateHit)
            {
                switch (moveDirection)
                {
                    case Direction.RIGHT:
                        pieceAnimator.CrossFade(h_RightHit, transitionTime, indexBaseLayer);

                        break;
                    case Direction.LEFT:
                        pieceAnimator.CrossFade(h_LeftHit, transitionTime, indexBaseLayer);

                        break;
                    case Direction.DOWN:
                        pieceAnimator.CrossFade(h_BottomHit, transitionTime, indexBaseLayer);

                        break;
                    case Direction.UP:
                        pieceAnimator.CrossFade(h_TopHit, transitionTime, indexBaseLayer);

                        break;
                }
            }
            else
            {
                pieceAnimator.CrossFade(h_Idle, transitionTime, indexBaseLayer);
            }

            PlayFinishAnimation();
        }

        public void PlayFinishAnimation()
        {
            if (pieceAnimator.GetCurrentAnimatorStateInfo(indexEyeMouthLayer).shortNameHash == h_Idle)
            {
                pieceAnimator.Play(h_Idle, indexEyeMouthLayer);
            }
            else
            {
                pieceAnimator.CrossFade(h_Idle, 0.1f, indexEyeMouthLayer);
            }
        }

        public void PlayWinAnimation(float delay)
        {
            AnimateOutline(0f, 1f, 1f, .0015f);

            StartRoutine("winAnimation", delay, () =>
            {
                pieceAnimator.SetBool(h_win, true);
                Happy();
            });
        }

        public void ShowUIWinAnimation()
        {
            CancelRoutine("jumping");
            StartRoutine("jumping", JumpRoutine(5));

            Happy();
        }

        public void ShowTurnAnimation()
        {
            WakeUp();

            CancelRoutine("jumping");
            StartRoutine("jumping", JumpRoutine(3), () => pieceAnimator.Play(h_Idle, indexBaseLayer), () => pieceAnimator.Play(h_Idle, indexBaseLayer));

            AnimateOutline(0f, 1f, 1f, .0015f, repeat: true);
        }

        public void StopTurnAnimation()
        {
            AnimateOutlineFrom(0f, 1f, .0015f, 1.15f);

            Sleep();
            pieceAnimator.CrossFade(h_Idle, 0.35f, indexBaseLayer);
        }

        public void WakeUp()
        {
            if (!eyes.IsRoutineActive("blink"))
            {
                eyes.SetState(GamePieceEyes.EyesState.OPENED);
            }

            if (!IsRoutineActive("blinking"))
            {
                StartBlinking();
            }

            mouth.SetState(GamePieceMouth.MouthState.CLOSED);
        }

        public void Happy()
        {
            if (!eyes.IsRoutineActive("blink"))
            {
                eyes.SetState(GamePieceEyes.EyesState.OPENED);
            }

            if (!IsRoutineActive("blinking"))
            {
                StartBlinking();
            }

            mouth.SetState(GamePieceMouth.MouthState.OPENED);
        }

        public void Sleep()
        {
            CancelRoutine("jumping");
            CancelRoutine("blinking");

            eyes.SetState(GamePieceEyes.EyesState.CLOSED);

            if (mouth.statesFastAccess.ContainsKey(GamePieceMouth.MouthState.SLEEPY))
            {
                mouth.SetState(GamePieceMouth.MouthState.SLEEPY);
            }
            else
            {
                mouth.SetState(GamePieceMouth.MouthState.CLOSED);
            }
        }

        public void StartBlinking()
        {
            if (!gameObject.activeInHierarchy) return;

            StartRoutine("blinking", BlinkingRoutine(), null, () => eyes.CancelRoutine("blink"));
        }

        public void Blink() => eyes.Blink(Random.Range(.2f, .4f));

        public void SetPiece(Piece piece)
        {
            id = piece.UniqueId;
            this.piece = piece;
        }

        public override void OnBeforeMoveActions(bool startTurn, BoardLocation from, BoardLocation to)
        {
            base.OnBeforeMoveActions(startTurn, from, to);

            AudioHolder.instance.PlaySelfSfxOneShotTracked(moveSfx);
        }

        public override void OnAfterMoveAction(bool startTurn, BoardLocation from, BoardLocation to)
        {
            Direction direction = Utils.GetDirectionFromLocations(from, to);
            BoardLocation _next = to.Neighbor(direction);

            bool playSmash = false;
            if (speedMltp >= 1.0f)
            {
                if (_next.OnBoard(gameboard.game._State.Board))
                {
                    //check next
                    if (gameboard.BoardTokensAt(_next).Any(_token => !_token.Token.pieceCanEnter && !_token.Token.pieceCanEndMoveOn))
                    {
                        playSmash = true;
                    }
                }
            }

            if (playSmash)
            {
                PlaySmashAnimation(direction);
            }
            else
            {
                PlayFinishMovement(true);
            }

            base.OnAfterMoveAction(startTurn, from, to);
        }

        public override float StartMoveRoutine(bool startMove, BoardLocation from, BoardLocation to)
        {
            PutMovementDirection(Utils.GetDirectionFromLocations(from, to));
            speedMltp += .15f;

            return base.StartMoveRoutine(startMove, from, to);
        }

        public override float _Destroy(DestroyType reason)
        {
            switch (reason)
            {
                case DestroyType.FALLING:
                    Happy();

                    break;
            }

            return base._Destroy(reason);
        }

        public void PlaySubtleMove(Vector2 direction, float time)
        {
            positionTween.from = transform.localPosition;
            positionTween.to = positionTween.from + (Vector3)direction;

            positionTween.repeat = Tween.RepeatType.PING_PONG;
            positionTween.playbackTime = time;

            positionTween.PlayForward(true);
        }

        public void StopSubtleMove()
        {
            if (positionTween.isPlaying)
            {
                positionTween.StopTween(false);
            }
        }

        private IEnumerator BlinkingRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(3f, 6f));

                Blink();
            }
        }

        private IEnumerator JumpRoutine(int count)
        {
            pieceAnimator.Play(h_Jumping);

            while (pieceAnimator.GetCurrentAnimatorStateInfo(indexBaseLayer).shortNameHash != h_Jumping)
                yield return true;

            AnimatorStateInfo stateInfo = pieceAnimator.GetCurrentAnimatorStateInfo(indexBaseLayer);

            yield return new WaitForSeconds(stateInfo.length * count);

            pieceAnimator.Play(h_Idle, indexBaseLayer);
        }
    }
}
