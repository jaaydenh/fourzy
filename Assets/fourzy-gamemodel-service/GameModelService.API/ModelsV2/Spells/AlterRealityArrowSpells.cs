using System;

namespace FourzyGameModel.Model
{
    public class AlterArrowsSpell : ISpell, IMove
    {
        public string Name { get { return "HEX"; } }
        public SpellId SpellId { get { return SpellId.HEX; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "ALTER ARROW SPELL direction="; } }

        public int PlayerId { get; set; }
        public Direction Orientation { get; set; }
        public bool RequiresLocation { get; set; }

        public AlterArrowsSpell(int PlayerId, Direction Orientation)
        {
            this.PlayerId = PlayerId;
            this.Cost = SpellConstants.CostAlterArrowsSpell;
            this.RequiresLocation = false;
        }

        public bool Cast(GameState State)
        {
            foreach(IToken t in State.Board.FindTokens(TokenType.ARROW))
            {
                t.Orientation = Orientation;
            }
            return true;
        }

        public void StartOfTurn(int PlayerId)
        {

        }

        public string Export()
        {
            return "";
        }

    }
}
