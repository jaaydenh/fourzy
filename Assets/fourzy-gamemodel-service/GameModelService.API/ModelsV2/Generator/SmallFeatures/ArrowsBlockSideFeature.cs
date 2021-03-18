using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ArrowsBlockSideFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public List<TokenType> Tokens { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.ARROWS_IN; } }
        public RelativeDirection RelativeDirection { get; set; }
        public Direction BlockDirection { get; set; }

        public ArrowsBlockSideFeature(Direction BlockDirection = Direction.NONE, RelativeDirection RelativeDirection = RelativeDirection.NONE )
        {
            this.Name = "Block With Arrow Pattern";
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.ARROW };
            this.RelativeDirection = RelativeDirection;
            this.BlockDirection = BlockDirection;
        }

        public void Build(GameBoard Board)
        {
            if (BlockDirection == Direction.NONE) BlockDirection = Board.Random.RandomDirection();
            if (RelativeDirection == RelativeDirection.NONE) RelativeDirection = (RelativeDirection) Board.Random.RandomInteger(0, 1);

            switch (BlockDirection)
            {
                case Direction.UP:
                    for (int c = 0; c < Board.Columns; c++)
                        Board.AddToken(new ArrowToken(RelativeDirection==RelativeDirection.IN?Direction.DOWN:Direction.UP), 
                            new BoardLocation(0, c));
                    break;
                case Direction.DOWN:
                    for (int c = 0; c < Board.Columns; c++)
                        Board.AddToken(new ArrowToken(RelativeDirection == RelativeDirection.IN ? Direction.UP : Direction.DOWN),
                            new BoardLocation(Board.Rows-1, c));
                    break;
                case Direction.LEFT:
                    for (int r = 0; r < Board.Rows; r++)
                        Board.AddToken(new ArrowToken(RelativeDirection == RelativeDirection.IN ? Direction.RIGHT: Direction.LEFT),
                            new BoardLocation(r, 0));
                    break;
                case Direction.RIGHT:
                    for (int r = 0; r < Board.Rows; r++)
                        Board.AddToken(new ArrowToken(RelativeDirection == RelativeDirection.IN ? Direction.LEFT : Direction.RIGHT),
                            new BoardLocation(r, Board.Columns-1));
                    break;

            }
        }
    }
}
