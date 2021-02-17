using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ThreePattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public ThreePattern(GameBoard Board, BoardLocation Reference, CompassDirection Direction)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();
            Locations.Add(Reference);

            BoardLocation Two = Reference.Neighbor(Direction, 2);
            if (Two.OnBoard(Board)) Locations.Add(Two);

            BoardLocation Three = Two.Neighbor(Direction, 2);
            if (Three.OnBoard(Board)) Locations.Add(Three);

        }
    }
}
