//modded @vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.Serialized;
using FourzyGameModel.Model;
using System.Collections;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class TokenView : BoardBit
    {
        public new string name;
        public TokenType tokenType;
        [Range(0f, 1f)]
        public float volume = 1f;
        public AudioTypes onGamePieceEnter;
        public AudioTypes onActivate;

        public override void OnBitEnter(BoardBit other)
        {
            AudioHolder.instance.PlaySelfSfxOneShotTracked(onGamePieceEnter, volume);
        }

        public virtual TokenView SetData(IToken tokenData = null)
        {
            return this;
        }

        public virtual void OnActivate()
        {
            AudioHolder.instance.PlaySelfSfxOneShotTracked(onActivate, volume);
        }

        public virtual IEnumerator OnActivated() { yield break; }
    }
}