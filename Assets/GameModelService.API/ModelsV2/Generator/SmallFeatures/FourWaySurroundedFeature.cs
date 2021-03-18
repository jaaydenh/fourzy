using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class FourWaySurroundedFeature : ISmallFeature, IBoardIngredient
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
        public TokenType SurroundToken { get; set; }

        public int SpacingDistance { get; set; }

        public FourWaySurroundedFeature(BoardLocation InsertLocation, int SpacingDistance = 2, TokenType SurroundToken = TokenType.STICKY)
        {
            this.Insert = InsertLocation;
            this.Name = "FourWay Surround";
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.FOURWAY_ARROW, SurroundToken};
            this.SurroundToken = SurroundToken;
            this.AddMethod = AddTokenMethod.ALWAYS;
            this.ReplaceTokens = true;
            this.SpacingDistance = SpacingDistance;
        }

        public FourWaySurroundedFeature()
        {
            this.Insert = new BoardLocation(0, 0);
            this.Name = "FourWay Surround";
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { SurroundToken, TokenType.FOURWAY_ARROW };
            this.AddMethod = AddTokenMethod.ALWAYS;
            this.ReplaceTokens = true;

        }

        public void Build(GameBoard Board)
        {
            if (Insert.Row == 0 && Insert.Column == 0)
            {
                Insert = Board.Random.RandomLocation(new BoardLocation(2, 2), Board.Rows -4, Board.Columns -4);
            }

            Board.AddToken(new FourWayArrowToken(), Insert, AddMethod, ReplaceTokens);     
        
            foreach (Direction d in new List<Direction> { Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT})
            {
                BoardLocation l = Insert.Neighbor(d, SpacingDistance);
                if (l.OnBoard(Board))
                    Board.AddToken(TokenFactory.Create(SurroundToken), l, AddMethod, ReplaceTokens);
            }

        }
    }
}
