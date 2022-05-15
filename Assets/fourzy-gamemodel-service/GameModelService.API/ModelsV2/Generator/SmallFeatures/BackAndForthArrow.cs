using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class BackAndForthArrowFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public List<TokenType> Tokens { get; }


        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.BACK_AND_FORTH; } }

        public Direction AddDirection { get; set; }
        public AddTokenMethod AddMethod { get; set; }
        public bool ReplaceTokens { get; set; }

        public BackAndForthArrowFeature(Direction AddDirection = Direction.RANDOM, AddTokenMethod AddMethod = AddTokenMethod.ONLY_TERRAIN, bool ReplaceTokens = false)
        {
            this.Name = "Back And Forth Arrows";
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.ARROW };
            this.AddDirection = AddDirection;
            this.AddMethod = AddMethod;
            this.ReplaceTokens = ReplaceTokens;
            this.Width = -1;
            this.Height = -1;
        }


        public void Build(GameBoard Board)
        {
            BoardLocation insert = Board.Random.RandomLocation(new BoardLocation(2, 2), Board.Rows - 4, Board.Columns - 4);

            Direction d = AddDirection;
            if (d == Direction.RANDOM) d = Board.Random.RandomDirection();
            Board.AddToken(new ArrowToken(d), insert);

            switch (d)
            {
                case Direction.LEFT:
                    Board.AddToken(new ArrowToken(Direction.RIGHT), new BoardLocation(insert.Row, insert.Column - 1));
                    break;

                case Direction.RIGHT:
                    Board.AddToken(new ArrowToken(Direction.LEFT), new BoardLocation(insert.Row, insert.Column + 1));
                    break;

                case Direction.UP:
                    Board.AddToken(new ArrowToken(Direction.DOWN), new BoardLocation(insert.Row-1, insert.Column));
                    break;

                case Direction.DOWN:
                    Board.AddToken(new ArrowToken(Direction.UP), new BoardLocation(insert.Row+1, insert.Column));
                    break;

            }


        }
    }
}
