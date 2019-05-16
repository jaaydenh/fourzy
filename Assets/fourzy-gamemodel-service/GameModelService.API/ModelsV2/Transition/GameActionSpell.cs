using System;

namespace FourzyGameModel.Model
{
    public class GameActionSpell : GameAction
    {
        public GameActionType Type { get { return GameActionType.SPELL; } }
        public GameActionTiming Timing { get { return GameActionTiming.AFTER_MOVE; } }
        public ISpell Spell { get; set; }

        public GameActionSpell(ISpell Spell)
        {
            this.Spell = Spell;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
