//vadym udod

using FourzyGameModel.Model;

namespace Fourzy._Updates.Mechanics.Board
{
    public class TokenSpell : TokenView
    {
        public SpellId spellId;

        public ISpell spell;

        public virtual void SetData(ISpell spell)
        {
            this.spell = spell;
        }
    }
}