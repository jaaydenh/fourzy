using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class DoubleArrowFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public List<TokenType> Tokens { get; }


        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.DOUBLE_ARROW; } }
        public Direction LineDirection { get; set; }
        public Direction ArrowDirection { get; set; }
       
        public AddTokenMethod AddMethod { get; set; }
        public bool ReplaceTokens { get; set; }

        public DoubleArrowFeature(Direction LineDirection = Direction.RANDOM, Direction ArrowDirection = Direction.RANDOM, AddTokenMethod AddMethod = AddTokenMethod.ONLY_TERRAIN, bool ReplaceTokens = false)
        {
            this.LineDirection = LineDirection;
            this.ArrowDirection = LineDirection;
            this.Name = "Double Arrow" + " " + LineDirection.ToString();
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.ARROW };

            this.AddMethod = AddMethod;
            this.ReplaceTokens = ReplaceTokens;
            this.Width = -1;
            this.Height = -1;
        }


        public void Build(GameBoard Board)
        {
            if (LineDirection == Direction.RANDOM) LineDirection = Board.Random.RandomDirection();
            if (ArrowDirection == Direction.RANDOM) ArrowDirection = Board.Random.RandomDirection();
            Insert = Board.Random.RandomLocation(new BoardLocation(2, 2), Board.Rows - 4, Board.Columns - 4);
            int count = 2;
            for (int i =0; i<count; i++)
            {
                BoardLocation l = new BoardLocation(Insert);
                switch(LineDirection)
                {
                    case Direction.UP:
                        l.Row--;
                        break;
                    case Direction.DOWN:
                        l.Row++;
                        break;
                    case Direction.LEFT:
                        l.Column--;
                        break;
                    case Direction.RIGHT:
                        l.Column++;
                        break;

                }

                Board.AddToken(new ArrowToken(ArrowDirection), l);
            }
        }
    }
}
