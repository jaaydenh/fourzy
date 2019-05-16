using System;

namespace FourzyGameModel.Model
{
    public class IceWallSpell : ISpell, IMove
    {
        public string Name { get { return "ICE_WALL"; } }
        public SpellId SpellId { get { return SpellId.ICE_WALL; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "ICE WALL SPELL at Location=x,y"; } }

        public int PlayerId { get; set; }

        public BoardLocation Location { get; set; }
        public bool RequiresLocation { get; set; }

        public IceWallSpell(BoardLocation Location)
        {
            this.Location = new BoardLocation(Location);
            this.Cost = SpellConstants.CostIceWallSpell;
            this.PlayerId = PlayerId;
            this.RequiresLocation = true;
        }

        public bool Cast(GameState State)
        {
            if (State.Board.ContentsAt(Location).Empty)
            {
                State.Board.ContentsAt(Location).AddToken(new IceBlockToken());
                return true;
            }
            return false;
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
