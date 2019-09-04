//@vadym udod

using Fourzy._Updates.Audio;
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

        public override TokenView SetData(IToken tokenData)
        {
            direction = tokenData.Orientation;

            switch (direction)
            {
                case Direction.LEFT:
                    transform.localEulerAngles = new Vector3(0f, 0f, 90f);
                    break;

                case Direction.RIGHT:
                    transform.localEulerAngles = new Vector3(0f, 0f, -90f);
                    break;

                case Direction.DOWN:
                    transform.localEulerAngles = new Vector3(0f, 0f, -180f);
                    break;
            }

            return this;
        }

        public override void OnBitEnter(BoardBit other)
        {
            AudioHolder.instance.PlaySelfSfxOneShot(onGamePieceEnter, volume, 
                Mathf.Lerp(1f, maxPitchValue, other.turnTokensInteractionList.Where(bit => bit.tokenType == TokenType.ARROW).Count() / (float)maxPitchIn));
        }
    }
}