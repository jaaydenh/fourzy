using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ACoupleArrowsFeature : IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }

        public SmallFeatureType Feature { get { return SmallFeatureType.A_COUPLE_OF_ARROWS; } }
        public int ArrowCount { get; set; }

        public ACoupleArrowsFeature(int ArrowCount = 4)
        {
            this.Name = "A Couple of Arrows";
            this.Type = IngredientType.SMALLFEATURE;
            this.ArrowCount= ArrowCount;
        }

        public void Build(GameBoard Board)
        {
            //After 500 cycles. give up
            int count = 500;
            int arrows = 0;
            while (count-- > 0 && arrows < ArrowCount)
            {
                BoardLocation loc = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows-2, Board.Columns-2);
                BoardSpace target = Board.ContentsAt(loc);
                if (!target.Empty && !target.ContainsOnlyTerrain) continue;
                if (loc.Neighbor(Direction.UP).OnBoard(Board) && !Board.ContentsAt(loc.Neighbor(Direction.UP)).TokensAllowEndHere) continue;
                if (loc.Neighbor(Direction.DOWN).OnBoard(Board) && !Board.ContentsAt(loc.Neighbor(Direction.DOWN)).TokensAllowEndHere) continue;
                if (loc.Neighbor(Direction.LEFT).OnBoard(Board) && !Board.ContentsAt(loc.Neighbor(Direction.LEFT)).TokensAllowEndHere) continue;
                if (loc.Neighbor(Direction.RIGHT).OnBoard(Board) && !Board.ContentsAt(loc.Neighbor(Direction.RIGHT)).TokensAllowEndHere) continue;
                Board.AddToken(new ArrowToken(Board.Random.RandomDirection()), loc);
                arrows++;
            }


        }


    }
}
