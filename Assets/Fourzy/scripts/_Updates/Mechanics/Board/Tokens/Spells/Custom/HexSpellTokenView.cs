//@vadym udod

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
    }
}
