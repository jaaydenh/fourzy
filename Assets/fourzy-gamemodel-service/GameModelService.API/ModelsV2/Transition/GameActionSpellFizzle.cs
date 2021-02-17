using System;

namespace FourzyGameModel.Model
{
    public class GameActionSpellFizzle : GameAction
    {
        public GameActionType Type { get { return GameActionType.SPELL_FIZZLE; } }
        public GameActionTiming Timing { get { return GameActionTiming.AFTER_MOVE; } }
        
        //Fizzle is a generic failure type
        public SpellFailureType Reason { get; set; }
        public ISpell Spell { get; set; }

        public GameActionSpellFizzle(ISpell Spell, SpellFailureType Reason = SpellFailureType.FIZZLE)
        {
            this.Spell = Spell;
            this.Reason = Reason;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
