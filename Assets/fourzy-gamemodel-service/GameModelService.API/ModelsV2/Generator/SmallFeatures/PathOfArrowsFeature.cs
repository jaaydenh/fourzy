using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class PathOfArrowsFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public List<TokenType> Tokens { get; }


        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.PATH_OF_ARROWS; } }
        public Direction FirstDirection { get; set; }

        public AddTokenMethod AddMethod { get; set; }
        public bool ReplaceTokens { get; set; }

        public PathOfArrowsFeature(Direction FirstDirection = Direction.RANDOM, AddTokenMethod AddMethod = AddTokenMethod.ONLY_TERRAIN, bool ReplaceTokens = false)
        {
            this.FirstDirection = FirstDirection;
            this.Name = "Path Of Arrows" + " " + FirstDirection.ToString();
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

            switch (FirstDirection)
            {
                case Direction.UP:
                    Insert = new BoardLocation(Board.Rows - 1, Board.Random.RandomInteger(2, Board.Columns - 2));
                    break;
                case Direction.DOWN:
                    Insert = new BoardLocation(0, Board.Random.RandomInteger(2, Board.Columns - 2));
                    break;
                case Direction.LEFT:
                    Insert = new BoardLocation(Board.Random.RandomInteger(2, Board.Rows - 2), Board.Columns - 1);
                    break;
                case Direction.RIGHT:
                    Insert = new BoardLocation(Board.Random.RandomInteger(2, Board.Rows - 2), 0);
                    break;
            }

            //Board.AddToken(new ArrowToken(FirstDirection), Insert);

            BoardLocation l = new BoardLocation(Insert);
            Direction d = FirstDirection;
            int addcount = 0;

            while (addcount < Board.Rows * 2)
            {
                Board.AddToken(new ArrowToken(d), l);

                int count = Board.Random.RandomInteger(2, 4);
                while (count > 0)
                {

                    switch (d)
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

                    if (!l.OnBoard(Board))
                        return;

                    count--;
                    addcount++;
                }

                switch (d)
                {
                    case Direction.LEFT:
                    case Direction.RIGHT:
                        d = Board.Random.RandomDirection(new List<Direction>() { Direction.UP, Direction.DOWN });
                        break;
                    case Direction.UP:
                    case Direction.DOWN:
                        d = Board.Random.RandomDirection(new List<Direction>() { Direction.LEFT, Direction.RIGHT });
                        break;
                }
            }

        }
    }
}
