//vadym udod

using FourzyGameModel.Model;

namespace Fourzy._Updates.Mechanics.Board
{
    public class TokenSpell : TokenView
    {
        public SpellId spellId;

        internal ISpell spell { get; private set; }
        internal IMove spellMove { get; private set; }

        public virtual void SetData(ISpell spell, IMove spellMove)
        {
            this.spell = spell;
            this.spellMove = spellMove;
        }
    }
}