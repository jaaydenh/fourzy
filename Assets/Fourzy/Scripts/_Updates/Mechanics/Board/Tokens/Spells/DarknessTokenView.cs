//@vadym udod

using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class DarknessTokenView : TokenSpell
    {
        public override void SetData(ISpell spell)
        {
            if (spell.SpellId != SpellId.DARKNESS) return;

            DarknessSpell darkness = spell as DarknessSpell;
            currentCountdownValue = darkness.Duration;
            Debug.Log(currentCountdownValue);

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

            Debug.Log(currentCountdownValue);
            /*if (!startTurn) */
            countdown.SetValue(currentCountdownValue--);
        }
    }
}