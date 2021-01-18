using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class TriArrowFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public TokenType Token { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.SINGULARITY; } }

        public Direction PatternDirection {get; set;}

        public AddTokenMethod AddMethod { get; set; }
        public bool ReplaceTokens { get; set; }

        public TriArrowFeature(Direction PatternDirection = Direction.RANDOM, AddTokenMethod AddMethod = AddTokenMethod.ONLY_TERRAIN, bool ReplaceTokens = false)
        {
            this.PatternDirection = PatternDirection;
            this.Name = "TriArrow";
            this.Type = IngredientType.SMALLFEATURE;
            this.Token = TokenType.ARROW;

            this.AddMethod = AddMethod;
            this.ReplaceTokens = ReplaceTokens;
            this.Width = -1;
            this.Height = -1;
        }


        public void Build(GameBoard Board)
        {
            Insert = Board.Random.RandomLocation(new BoardLocation(2, 2), Board.Rows - 4, Board.Columns - 4);
            if (PatternDirection == Direction.RANDOM) PatternDirection = Board.Random.RandomDirection();

            switch (PatternDirection)
            {
                case Direction.UP:
                    Board.AddToken(new ArrowToken(Direction.UP), new BoardLocation(Insert.Row + 1, Insert.Column));
                    Board.AddToken(new ArrowToken(Direction.DOWN), new BoardLocation(Insert.Row - 1, Insert.Column - 1));
                    Board.AddToken(new ArrowToken(Direction.DOWN), new BoardLocation(Insert.Row - 1, Insert.Column + 1));
                    break;

                case Direction.DOWN:
                    Board.AddToken(new ArrowToken(Direction.DOWN), new BoardLocation(Insert.Row - 1, Insert.Column));
                    Board.AddToken(new ArrowToken(Direction.UP), new BoardLocation(Insert.Row + 1, Insert.Column - 1));
                    Board.AddToken(new ArrowToken(Direction.UP), new BoardLocation(Insert.Row + 1, Insert.Column + 1));
                    break;

                case Direction.LEFT:
                    Board.AddToken(new ArrowToken(Direction.LEFT), new BoardLocation(Insert.Row, Insert.Column +1));
                    Board.AddToken(new ArrowToken(Direction.RIGHT), new BoardLocation(Insert.Row -1 , Insert.Column - 1));
                    Board.AddToken(new ArrowToken(Direction.RIGHT), new BoardLocation(Insert.Row + 1, Insert.Column - 1));
                    break;

                case Direction.RIGHT:
                    Board.AddToken(new ArrowToken(Direction.RIGHT), new BoardLocation(Insert.Row, Insert.Column -1));
                    Board.AddToken(new ArrowToken(Direction.LEFT), new BoardLocation(Insert.Row - 1, Insert.Column + 1));
                    Board.AddToken(new ArrowToken(Direction.LEFT), new BoardLocation(Insert.Row + 1, Insert.Column + 1));
                    break;
            }

        }
    }
}
