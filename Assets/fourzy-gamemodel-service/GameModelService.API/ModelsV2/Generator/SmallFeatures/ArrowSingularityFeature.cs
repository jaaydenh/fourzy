using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ArrowSingularityFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public List<TokenType> Tokens { get; }


        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.SINGULARITY; } }

        public AddTokenMethod AddMethod { get; set; }
        public bool ReplaceTokens { get; set; }
        public bool PlaceFourWayAtCenter { get; set; }

        public ArrowSingularityFeature(AddTokenMethod AddMethod = AddTokenMethod.ONLY_TERRAIN, bool ReplaceTokens = false, bool PlaceFourWayAtCenter = false)
        {
            this.Name = "Singularity";
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.ARROW };

            this.AddMethod = AddMethod;
            this.ReplaceTokens = ReplaceTokens;
            this.Width = -1;
            this.Height = -1;
            this.PlaceFourWayAtCenter = PlaceFourWayAtCenter;
        }


        public void Build(GameBoard Board)
        {
            Insert = Board.Random.RandomLocation(new BoardLocation(2, 2), Board.Rows - 4, Board.Columns - 4);
            Board.AddToken(new ArrowToken(Direction.DOWN), new BoardLocation(Insert.Row +1, Insert.Column));
            Board.AddToken(new ArrowToken(Direction.UP), new BoardLocation(Insert.Row -1, Insert.Column));
            Board.AddToken(new ArrowToken(Direction.RIGHT), new BoardLocation(Insert.Row, Insert.Column-1));
            Board.AddToken(new ArrowToken(Direction.LEFT), new BoardLocation(Insert.Row, Insert.Column+1));
            if (PlaceFourWayAtCenter)
                Board.AddToken(new FourWayArrowToken(), Insert, AddMethod, ReplaceTokens);
        }
    }
    
}
