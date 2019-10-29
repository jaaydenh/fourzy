﻿//modded @vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
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
        public PlayerEnum player;

        public AudioTypes onMoveSfx = AudioTypes.GAME_PIECE_MOVE;

        private int h_win = Animator.StringToHash("win");
        private int h_Idle = Animator.StringToHash("Idle");
        private int h_Jumping = Animator.StringToHash("Jumping");

        private int h_MovingHorizontal = Animator.StringToHash("MovingHorizontal");
        private int h_MovingVertical = Animator.StringToHash("MovingVertical");
        private int h_RightHit = Animator.StringToHash("RightHit");
        private int h_LeftHit = Animator.StringToHash("LeftHit");
        private int h_BottomHit = Animator.StringToHash("BottomHit");
        private int h_TopHit = Animator.StringToHash("TopHit");

        private int h_MoveLeft = Animator.StringToHash("MoveLeft");
        private int h_MoveRight = Animator.StringToHash("MoveRight");
        private int h_MoveTop = Animator.StringToHash("MoveTop");
        private int h_MoveBottom = Animator.StringToHash("MoveDown");

        private Direction moveDirection = Direction.NONE;
        
        public GamePieceData pieceData => GamePiecesDataHolder._GetGamePieceData(this);

        public GamePieceMouth mouth { get; private set; }
        public GamePieceEyes eyes { get; private set; }
        public bool isMoving { get; private set; }

        public override Color outlineColor => pieceData.outlineColor;

        protected override void Awake()
        {
            base.Awake();

            if (pieceAnimator == null) pieceAnimator = GetComponent<Animator>();
            
            mouth = GetComponentInChildren<GamePieceMouth>();
            eyes = GetComponentInChildren<GamePieceEyes>();
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

            if (pieceAnimator.GetCurrentAnimatorStateInfo(indexEyeMouthLayer).shortNameHash == h_Idle)
                pieceAnimator.Play(h_Idle, indexEyeMouthLayer);
            else
                pieceAnimator.CrossFade(h_Idle, 0.1f, indexEyeMouthLayer);
        }

        public void PlayWinAnimation(float delay)
        {
            ShowOutline(false);

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
            StartRoutine("jumping", JumpRoutine(3));

            ShowOutline(true);
        }

        public void StopTurnAnimation()
        {
            HideOutline(false);

            Sleep();
            pieceAnimator.CrossFade(h_Idle, 0.35f, indexBaseLayer);
        }

        public void WakeUp()
        {
            if (!eyes.IsRoutineActive("blink"))
                eyes.SetState(GamePieceEyes.EyesState.OPENED);

            if (!IsRoutineActive("blinking")) StartBlinking();

            mouth.SetState(GamePieceMouth.MouthState.CLOSED);
        }

        public void Happy()
        {
            if (!eyes.IsRoutineActive("blink")) eyes.SetState(GamePieceEyes.EyesState.OPENED);

            if (!IsRoutineActive("blinking")) StartBlinking();

            mouth.SetState(GamePieceMouth.MouthState.OPENED);
        }

        public void Sleep()
        {
            CancelRoutine("blinking");
            
            eyes.SetState(GamePieceEyes.EyesState.CLOSED);

            if (mouth.statesFastAccess.ContainsKey(GamePieceMouth.MouthState.SLEEPY))
                mouth.SetState(GamePieceMouth.MouthState.SLEEPY);
            else
                mouth.SetState(GamePieceMouth.MouthState.CLOSED);
        }

        public void StartBlinking()
        {
            if (!gameObject.activeInHierarchy) return;

            StartRoutine("blinking", BlinkingRoutine(), null, () => eyes.CancelRoutine("blink"));
        }

        public void Blink() => eyes.Blink(Random.Range(.2f, .4f));

        public override void OnBeforeMoveAction(params BoardLocation[] locations)
        {
            base.OnBeforeMoveAction();

            AudioHolder.instance.PlaySelfSfxOneShotTracked(onMoveSfx);
            
            if (locations.Length < 2) return;

            PutMovementDirection(locations.GetDirectionFromLocations());
        }

        public override void OnAfterMove(params BoardLocation[] actionsMoves)
        {
            base.OnAfterMove(actionsMoves);

            PlayFinishMovement(actionsMoves.Length > 1);
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

        public void AddToModel()
        {
            if (!gameboard)
                return;

            //add to board model
            gameboard.game._State.Board.AddPiece(new Piece((int)player), location);
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

            while (pieceAnimator.GetCurrentAnimatorStateInfo(indexBaseLayer).shortNameHash != h_Jumping) yield return true;

            AnimatorStateInfo stateInfo = pieceAnimator.GetCurrentAnimatorStateInfo(indexBaseLayer);

            yield return new WaitForSeconds(stateInfo.length * count);

            pieceAnimator.Play(h_Idle, indexBaseLayer);
        }
    }
}