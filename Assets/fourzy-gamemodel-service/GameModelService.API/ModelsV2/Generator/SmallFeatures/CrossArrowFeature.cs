using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class CrossArrowFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public List<TokenType> Tokens { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.ARROWS_IN; } }
        public Direction VerticalDirection { get; set; }
        public Direction HorizontalDirection { get; set; }
        public bool Solid { get; set; }
        public bool ReplaceTokens { get; set; }
        public AddTokenMethod AddMethod { get; set; }

        public CrossArrowFeature(BoardLocation InsertLocation, Direction VerticalDirection = Direction.NONE, Direction HorizontalDirection = Direction.NONE, bool Solid = false)
        {
            this.Insert = InsertLocation;
            this.VerticalDirection = VerticalDirection;
            this.HorizontalDirection = HorizontalDirection;
            this.Name = "Cross Arrow Pattern";
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.ARROW };
            this.Solid = Solid;
            this.AddMethod = AddTokenMethod.ONLY_TERRAIN;
            this.ReplaceTokens = false;
        }

        public CrossArrowFeature()
        {
            this.Insert = new BoardLocation(0, 0);
            this.Name = "In Arrow Pattern";
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.ARROW };
            this.VerticalDirection = Direction.NONE;
            this.HorizontalDirection = Direction.NONE;
            this.Solid = false;
            this.AddMethod = AddTokenMethod.ONLY_TERRAIN;
            this.ReplaceTokens = false;
        }


        //Not perfect.  The insert location might not match the dot convention.
        public void Build(GameBoard Board)
        {
            if (VerticalDirection == Direction.NONE) VerticalDirection = Board.Random.Chance(50) ? Direction.UP: Direction.DOWN;
            if (HorizontalDirection == Direction.NONE) HorizontalDirection = Board.Random.Chance(50) ? Direction.RIGHT: Direction.LEFT;
 
            if (Insert.Row == 0 && Insert.Column == 0)
            {
                Insert = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows - 2, Board.Columns - 2);
            }
            int count = 0;
            foreach (BoardLocation l in Insert.Look(Board, Direction.UP))
            {
                if (count++%2==0 || Solid)
                Board.AddToken(new ArrowToken(VerticalDirection), l, AddMethod, ReplaceTokens);
            }
            count = 0;
            foreach (BoardLocation l in Insert.Look(Board, Direction.DOWN))
            {
                if (count++ % 2 == 0 || Solid)
                    Board.AddToken(new ArrowToken(VerticalDirection), l, AddMethod, ReplaceTokens);
            }
            count = 0;
            foreach (BoardLocation l in Insert.Look(Board, Direction.LEFT))
            {
                if (count++ % 2 == 0 || Solid)
                    Board.AddToken(new ArrowToken(HorizontalDirection), l, AddMethod, ReplaceTokens);
            }
            count = 0;
            foreach (BoardLocation l in Insert.Look(Board, Direction.RIGHT))
            {
                if (count++ % 2 == 0 || Solid)
                    Board.AddToken(new ArrowToken(HorizontalDirection), l, AddMethod, ReplaceTokens);
            }


        }
    }
}
