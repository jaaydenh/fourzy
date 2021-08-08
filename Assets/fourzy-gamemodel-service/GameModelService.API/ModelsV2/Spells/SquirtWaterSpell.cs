using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{

    //Add a water effect to a space adjacent to a water Fourzy.
    public class SquirtWaterSpell : ISpell, IMove
    {
        public string Name { get { return "SQUIRT"; } }
        public SpellId SpellId { get { return SpellId.SQUIRT_WATER; } }
        public SpellType SpellType { get { return SpellType.MOVE; } }

        public int Cost { get; set; }
        public MoveType MoveType { get { return MoveType.SPELL; } }
        public string Notation { get { return "SQUIRT WATER SPELL at Location=x,y "; } }

        public BoardLocation Location { get; set; }
        public int Duration { get; set; }
        public int PlayerId { get; set; }

        public bool RequiresLocation { get; set; }

        //need an empty constructor for MoveConverter
        public SquirtWaterSpell() { }

        public SquirtWaterSpell(int PlayerId, BoardLocation Location, int Cost = SpellConstants.DefaultFruitCost)
        {
            this.Location = new BoardLocation(Location);
            this.Duration = -1;
            this.Cost = Cost;
            this.PlayerId = PlayerId;
            this.RequiresLocation = true;
        }

        public List<BoardLocation> GetValidSpellLocations(GameBoard Board)
        {
            List<BoardLocation> Locations = new List<BoardLocation>() { };

            foreach (BoardSpace s in Board.Contents)
            {
                if (!s.ContainsSpell
                        && !Board.Corners.Contains(s.Location)
                        && s.ContainsOnlyTerrain)
                    Locations.Add(s.Location);
            }

            return Locations;
        }

        public bool Cast(GameState State, out List<IToken> tokens)
        {
            tokens = new List<IToken>();

            BoardSpace s = State.Board.ContentsAt(Location);
            if (!s.ContainsOnlyTerrain
                || s.ContainsSpell)
            {
                return false;
            }

            MagicWaterToken _token = new MagicWaterToken(PlayerId, Duration);
            State.Board.ContentsAt(Location).AddToken(_token);
            tokens.Add(_token);
            foreach (BoardLocation l in Location.GetOrthogonals(State.Board))
            {
                _token = new MagicWaterToken(PlayerId, Duration);
                State.Board.ContentsAt(l).AddToken(_token);
                tokens.Add(_token);
                State.Board.RecordGameAction(new GameActionTokenDrop(new MagicWaterToken(PlayerId, Duration), TransitionType.SPELL_CAST, Location, Location));
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
