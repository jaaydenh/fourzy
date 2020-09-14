using System;
using System.Collections.Generic;

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

        //This spell doesn't require a location.
        public List<BoardLocation> GetValidSpellLocations(GameBoard Board)
        {
            //List<BoardLocation> Locations = Board.FindTokenLocations(TokenType.ARROW);
            List<BoardLocation> Locations = new List<BoardLocation>() { };
            return Locations;
        }

        public bool Cast(GameState State)
        {
            foreach(IToken t in State.Board.FindTokens(TokenType.ARROW))
            {
                Direction d0 = t.Orientation; 
                t.Orientation = Orientation;
                State.Board.RecordGameAction(new GameActionTokenRotation(t, TransitionType.SPELL_CAST, Rotation.CLOCKWISE, d0, t.Orientation));
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
