//modded @vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.UI.Helpers;
using FourzyGameModel.Model;
using System.Collections;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class TokenView : BoardBit
    {
        public new string name;
        public IToken Token;
        [Range(0f, 1f)]
        public float volume = 1f;
        public string onGamePieceEnter;
        public string onActivate;

        protected Badge countdown;

        protected int currentCountdownValue;
        protected int frequency;

        public override void OnBitEnter(BoardBit other)
        {
            AudioHolder.instance.PlaySelfSfxOneShotTracked(onGamePieceEnter, volume);
        }

        public virtual TokenView SetData(IToken tokenData = null)
        {
            Token = tokenData;

            return this;
        }

        public virtual void OnActivate()
        {
            AudioHolder.instance.PlaySelfSfxOneShotTracked(onActivate, volume);
        }

        public virtual IEnumerator OnActivated() { yield break; }

        public virtual TokenView UpdateGraphics()
        {
            return this;
        }

        protected override void OnInitialized()
        {
            countdown = GetComponentInChildren<Badge>(true);
            countdown.Initialize();

            UpdateGraphics();

            base.OnInitialized();
        }
    }
}