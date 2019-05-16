using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ACoupleGhostsFeature :  IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }

        public SmallFeatureType Feature { get { return SmallFeatureType.A_COUPLE_OF_GHOSTS; } }
        public int GhostCount { get; set; }

        public ACoupleGhostsFeature(int GhostCount = 8)
        {
            this.Name = "A Couple of Ghosts";
            this.Type = IngredientType.SMALLFEATURE;
            this.GhostCount = GhostCount;
        }

        public void Build(GameBoard Board)
        {
            //After 500 cycles. give up
            int count = 500;
            int ghosts = 0;
            while (count-- > 0 && ghosts < GhostCount)
            {
                //Add a ghost where it won't be in a critical spot.  
                //Around the edges of the board is good.
                //Maybe one or two in the middle, as long as they are not
                //  1. Next to another ghost or blocker

                BoardLocation loc = Board.Random.RandomLocation(new BoardLocation(0, 0), Board.Rows, Board.Columns);
                BoardSpace target = Board.ContentsAt(loc);
                if (!target.Empty && !target.ContainsOnlyTerrain) continue;
                if (loc.Neighbor(Direction.UP).OnBoard(Board) && !Board.ContentsAt(loc.Neighbor(Direction.UP)).TokensAllowEndHere) continue;
                if (loc.Neighbor(Direction.DOWN).OnBoard(Board) && !Board.ContentsAt(loc.Neighbor(Direction.DOWN)).TokensAllowEndHere) continue;
                if (loc.Neighbor(Direction.LEFT).OnBoard(Board) && !Board.ContentsAt(loc.Neighbor(Direction.LEFT)).TokensAllowEndHere) continue;
                if (loc.Neighbor(Direction.RIGHT).OnBoard(Board) && !Board.ContentsAt(loc.Neighbor(Direction.RIGHT)).TokensAllowEndHere) continue;
                Board.AddToken(new GhostToken(), loc);
                ghosts++;
            }


        }

        
    }
}
