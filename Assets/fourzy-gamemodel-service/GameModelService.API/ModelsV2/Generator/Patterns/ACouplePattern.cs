using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ACouplePattern : IBoardPattern
    {
        public List<BoardLocation> Locations { get; }

        public string Name { get; }
        public IngredientType Type { get; }

        public SmallFeatureType Feature { get { return SmallFeatureType.A_COUPLE_OF_GHOSTS; } }
        public int TokenCount { get; set; }

        public ACouplePattern(GameBoard Board, int TokenCount = 6)
        {
            this.Name = "A Couple";
            this.Type = IngredientType.SMALLFEATURE;
            this.TokenCount= TokenCount;
            Locations = new List<BoardLocation>();

            //After 500 cycles. give up
            int count = 500;
            int tokens = 0;
            while (count-- > 0 && tokens < TokenCount)
            {
                BoardLocation loc = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows - 2, Board.Columns - 2);
                BoardSpace target = Board.ContentsAt(loc);
                if (!target.Empty && !target.ContainsOnlyTerrain) continue;
                if (loc.Neighbor(Direction.UP).OnBoard(Board) && !Board.ContentsAt(loc.Neighbor(Direction.UP)).TokensAllowEndHere && Locations.Contains(loc.Neighbor(Direction.UP))) continue;
                if (loc.Neighbor(Direction.DOWN).OnBoard(Board) && !Board.ContentsAt(loc.Neighbor(Direction.DOWN)).TokensAllowEndHere && Locations.Contains(loc.Neighbor(Direction.DOWN))) continue;
                if (loc.Neighbor(Direction.LEFT).OnBoard(Board) && !Board.ContentsAt(loc.Neighbor(Direction.LEFT)).TokensAllowEndHere && Locations.Contains(loc.Neighbor(Direction.LEFT))) continue;
                if (loc.Neighbor(Direction.RIGHT).OnBoard(Board) && !Board.ContentsAt(loc.Neighbor(Direction.RIGHT)).TokensAllowEndHere && Locations.Contains(loc.Neighbor(Direction.RIGHT))) continue;
                Locations.Add(loc);
                tokens++;
            }

        }

    }
}
