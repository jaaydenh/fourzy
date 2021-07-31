//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.Mechanics._GamePiece;
using FourzyGameModel.Model;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class ArrowTokenView : TokenView
    {
        public float maxPitchValue = 2f;
        public int maxPitchIn = 8;
        public Direction direction { get; private set; }
        public override Color outlineColor => Color.yellow;

        public override TokenView SetData(IToken tokenData)
        {
            base.SetData(tokenData);

            direction = tokenData.Orientation;

            switch (direction)
            {
                case Direction.LEFT:
                    body.transform.localEulerAngles = new Vector3(0f, 0f, 90f);

                    break;

                case Direction.RIGHT:
                    body.transform.localEulerAngles = new Vector3(0f, 0f, -90f);

                    break;

                case Direction.DOWN:
                    body.transform.localEulerAngles = new Vector3(0f, 0f, -180f);

                    break;
            }

            return this;
        }

        public override void OnBitEnter(BoardBit other)
        {
            base.OnBitEnter(other);

            if (other.GetType() == typeof(GamePieceView) || other.GetType().IsSubclassOf(typeof(GamePieceView)))
            {
                GamePieceView _gamepiece = other as GamePieceView;
                float mltp = GetMltp(_gamepiece.InteractionsWithToken<ArrowTokenView>(Token.Type).Count());

                AudioHolder.instance.PlaySelfSfxOneShot(onGamePieceEnter, volume, mltp * 1.3f);

                AnimateOutline(mltp, 1.2f, .1f, .0011f, 1.2f);
                Debug.Log(originalColor);
                AnimateColor(originalColor, Color.white, 0f);
            }
        }

        public override void OnBitExit(BoardBit other)
        {
            base.OnBitExit(other);

            if (outlineShowed && other.GetType() == typeof(GamePieceView) || other.GetType().IsSubclassOf(typeof(GamePieceView)))
            {
                GamePieceView _gamepiece = other as GamePieceView;

                AnimateOutlineFrom(0f, .4f, .0011f, GetMltp(_gamepiece.InteractionsWithToken<ArrowTokenView>(Token.Type).Count()));
                AnimateColorFrom(originalColor, .4f);
            }
        }

        private float GetMltp(int size)
        {
            return Mathf.Clamp(1.5f + (size * .06f), 1f, 2f);
        }
    }
}