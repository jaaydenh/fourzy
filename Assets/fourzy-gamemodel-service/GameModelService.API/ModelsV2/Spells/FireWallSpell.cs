using System;

namespace FourzyGameModel.Model
{
    public class FireWallSpell : ISpell, IMove
    {
        public string Name { get { return "FIRE_WALL"; } }
        public SpellId SpellId { get { return SpellId.FIRE_WALL; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "FIRE_WALL SPELL at Location=x,y"; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }
        public bool RequiresLocation { get; set; }

        public FireWallSpell(int PlayerId, BoardLocation Location, int Duration = SpellConstants.DefaultFireWallDuration)
        {
            this.Location = new BoardLocation(Location);
            this.Duration = Duration;
            this.Cost = SpellConstants.CostFireWallSpell;
            this.PlayerId = PlayerId;
            this.RequiresLocation = true;
    }

    public bool Cast(GameState State)
        {
            if (State.Board.ContentsAt(Location).Empty)
            {
                State.Board.ContentsAt(Location).AddToken(new MagicFireToken(PlayerId, Duration));
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
