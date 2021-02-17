using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ArrowTurnFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public TokenType Token { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.ARROW_TURN; } }
        public Direction FirstDirection { get; set; }
        public Direction SecondDirection { get; set; }

        public AddTokenMethod AddMethod { get; set; }
        public bool ReplaceTokens { get; set; }

        public ArrowTurnFeature(Direction FirstDirection = Direction.RANDOM, Direction SecondDirection = Direction.RANDOM, AddTokenMethod AddMethod = AddTokenMethod.ONLY_TERRAIN, bool ReplaceTokens = false)
        {
            this.FirstDirection = FirstDirection;
            this.SecondDirection = SecondDirection;
            this.Name = "Arrow Turn" + " " + FirstDirection.ToString() + SecondDirection.ToString();
            this.Type = IngredientType.SMALLFEATURE;
            this.Token = TokenType.ARROW;

            this.AddMethod = AddMethod;
            this.ReplaceTokens = ReplaceTokens;
            this.Width = -1;
            this.Height = -1;
        }


        public void Build(GameBoard Board)
        {
            if (FirstDirection == Direction.RANDOM) FirstDirection = Board.Random.RandomDirection();
            if (SecondDirection == Direction.RANDOM) SecondDirection = Board.Random.RandomDirection();
            Insert = Board.Random.RandomLocation(new BoardLocation(2, 2), Board.Rows - 4, Board.Columns - 4);
            int count = 2;
            for (int i = 0; i < count; i++)
            {
                BoardLocation l = new BoardLocation(Insert);
                Direction d = Direction.NONE;

                if (i % 2 == 0)
                    d = FirstDirection;
                else
                    d = SecondDirection;

                    if (i%2 ==1)
                switch (FirstDirection)
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

                Board.AddToken(new ArrowToken(d), l);
            }
        }
    }
}
