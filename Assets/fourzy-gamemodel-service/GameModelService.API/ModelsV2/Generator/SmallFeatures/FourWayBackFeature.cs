using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class FourWayBackFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public List<TokenType> Tokens { get; }


        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.ARROWS_IN; } }
        public AddTokenMethod AddMethod { get; set; }
        public bool ReplaceTokens { get; set; }
        public int SpacingDistance { get; set; }

        public FourWayBackFeature(BoardLocation InsertLocation, int SpacingDistance = 2)
        {
            this.Insert = InsertLocation;
            this.Name = "FourWay Back";
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.RANDOM_FOURWAY, TokenType.ARROW};
            this.AddMethod = AddTokenMethod.ALWAYS;
            this.ReplaceTokens = true;
            this.SpacingDistance = SpacingDistance;
        }

        public FourWayBackFeature()
        {
            this.Insert = new BoardLocation(0, 0);
            this.Name = "FourWay Back";
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.RANDOM_FOURWAY, TokenType.ARROW };
            this.AddMethod = AddTokenMethod.ALWAYS;
            this.ReplaceTokens = true;

        }

        public void Build(GameBoard Board)
        {
            if (Insert.Row == 0 && Insert.Column == 0)
            {
                Insert = Board.Random.RandomLocation(new BoardLocation(2, 2), Board.Rows - 3, Board.Columns - 3);
            }

            Board.AddToken(new FourWayArrowToken(), Insert, AddMethod, ReplaceTokens);

            foreach (Direction d in new List<Direction> { Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT })
            {
                BoardLocation l = Insert.Neighbor(d, SpacingDistance);
                if (l.OnBoard(Board))
                    Board.AddToken(new ArrowToken(BoardLocation.Reverse(d)), l, AddMethod, ReplaceTokens);
            }
        }
    }
}
