//@vadym udod

using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class HexSpellTokenView : TokenSpell
    {
        public override Color outlineColor => Color.red;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ShowOutline(true);
        }

        public override void SetData(ISpell spell)
        {
            if (spell.SpellId != SpellId.HEX) return;

            HexSpellToken hex = spell as HexSpellToken;
            //currentCountdownValue = hex.Countdown;
            //temp solutions, as HEX token does not exists at this moment, it only getting creating from spell inside model code
            currentCountdownValue = 7;

            countdown.SetValue(currentCountdownValue);
        }

        public override TokenView UpdateGraphics()
        {
            countdown.targetText.UpdateTMP_Text();

            return base.UpdateGraphics();
        }

        public override void OnAfterTurn(bool startTurn)
        {
            base.OnAfterTurn(startTurn);

            /*if (!startTurn) */countdown.SetValue(currentCountdownValue--);
        }
    }
}
