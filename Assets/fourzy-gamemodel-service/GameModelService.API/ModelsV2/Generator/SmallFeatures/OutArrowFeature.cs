using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class OutArrowFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public List<TokenType> Tokens { get; }


        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.ARROWS_OUT; } }
        public bool Solid { get; set; }

        public OutArrowFeature(BoardLocation InsertLocation, bool Solid=false)
        {
            this.Insert = InsertLocation;
            this.Name = "In Arrow Pattern";
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.ARROW };
            this.Solid = Solid;
        }

        public OutArrowFeature()
        {
            this.Insert = new BoardLocation(0, 0);
            this.Name = "In Arrow Pattern";
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.ARROW };
            this.Solid = false;
        }


        //Not perfect.  The insert location might not match the dot convention.
        public void Build(GameBoard Board)
        {
            if (Insert.Row == 0 && Insert.Column == 0)
            {
                Insert = Board.Random.RandomLocation(new BoardLocation(2, 2), Board.Rows - 3, Board.Columns - 3);
            }

            int count = 0;
            foreach (BoardLocation l in Insert.Look(Board, Direction.UP))
            {
                if (count++ % 2 == 0 || Solid)
                    Board.AddToken(new ArrowToken(Direction.UP), l);
            }
            count = 0;
            foreach (BoardLocation l in Insert.Look(Board, Direction.DOWN))
            {
                if (count++ % 2 == 0 || Solid)
                    Board.AddToken(new ArrowToken(Direction.DOWN), l);
            }

            count = 0;
            foreach (BoardLocation l in Insert.Look(Board, Direction.LEFT))
            {
                if (count++ % 2 == 0 || Solid)
                    Board.AddToken(new ArrowToken(Direction.LEFT), l);
            }

            count = 0;
            foreach (BoardLocation l in Insert.Look(Board, Direction.RIGHT))
            {
                if (count++ % 2 == 0 || Solid)
                    Board.AddToken(new ArrowToken(Direction.RIGHT), l);
            }


        }
    }
}
