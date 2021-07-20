//@vadym udod

using Fourzy._Updates.Audio;
using FourzyGameModel.Model;
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

            AudioHolder.instance.PlaySelfSfxOneShot(onGamePieceEnter, volume, other.speedMltp);

            if (!outlineShowed)
            {
                AnimateOutlineFrom(other.speedMltp * 2f, .1f, .0011f, other.speedMltp + .15f);
            }
        }

        public override void OnBitExit(BoardBit other)
        {
            base.OnBitExit(other);

            if (outlineShowed && !gameboard.game._State.Board.ContentsAt(location).ContainsPiece)
            {
                AnimateOutlineFrom(0f, .4f, .0011f, other.speedMltp + .15f);
            }
        }
    }
}