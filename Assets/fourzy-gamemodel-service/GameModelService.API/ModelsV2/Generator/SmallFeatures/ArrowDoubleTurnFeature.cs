using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ArrowDoubleTurnFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public List<TokenType> Tokens { get; }


        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.ARROW_DOUBLETURN; } }
        public Direction FirstDirection { get; set; }
        public Direction SecondDirection { get; set; }

        public AddTokenMethod AddMethod { get; set; }
        public bool ReplaceTokens { get; set; }

        public ArrowDoubleTurnFeature(Direction FirstDirection = Direction.RANDOM, Direction SecondDirection = Direction.RANDOM, AddTokenMethod AddMethod = AddTokenMethod.ONLY_TERRAIN, bool ReplaceTokens = false)
        {
            this.FirstDirection = FirstDirection;
            this.SecondDirection = SecondDirection;
            this.Name = "Arrow Turn" + " " + FirstDirection.ToString() + SecondDirection.ToString();
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.ARROW };

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
            int count = 4;
            for (int i = 0; i < count; i++)
            {
                BoardLocation l = new BoardLocation(Insert);
                Direction d = Direction.NONE;

                if (i % 2 == 0)
                    d = FirstDirection;
                else
                    d = SecondDirection;

                    switch (FirstDirection)
                    {
                        case Direction.UP:
                            l.Row-= (i+1)/2;
                            break;
                        case Direction.DOWN:
                            l.Row += (i + 1) / 2;
                            break;
                        case Direction.LEFT:
                            l.Column -= (i + 1) / 2;
                            break;
                        case Direction.RIGHT:
                            l.Column -= (i + 1) / 2;
                            break;

                    }

                switch (SecondDirection)
                {
                    case Direction.UP:
                        l.Row -= i/2;
                        break;
                    case Direction.DOWN:
                        l.Row += i / 2;
                        break;
                    case Direction.LEFT:
                        l.Column-=i/2;
                        break;
                    case Direction.RIGHT:
                        l.Column += i/2;
                        break;
                }

                Board.AddToken(new ArrowToken(d), l);
            }
        }
    }
}
