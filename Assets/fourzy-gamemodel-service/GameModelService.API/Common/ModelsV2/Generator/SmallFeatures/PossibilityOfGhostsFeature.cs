using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class PossibilityOfGhostsFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public TokenType Token { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.ARROW_HIGHWAY; } }
        public AddTokenMethod AddMethod { get; set; }
        public bool ReplaceTokens { get; set; }
        public int GhostChance { get; set; }
        public int GhostCount { get; set; }

        public PossibilityOfGhostsFeature(int GhostChance = -1, int GhostCount = -1)
        {
            this.Name = "PossiblityOfGhosts " ;
            this.Type = IngredientType.SMALLFEATURE;
            this.Token = TokenType.ARROW;
            this.AddMethod = AddMethod;
            this.ReplaceTokens = ReplaceTokens;
            this.GhostChance = GhostChance;
            this.GhostCount = GhostCount;
        }


        public void Build(GameBoard Board)
        {
            if (GhostCount <0) GhostCount = Board.Random.RandomInteger(1, Board.Rows - 3);
            if (GhostChance <0) GhostChance = Board.Random.RandomInteger(0, 50);

            if (Board.Random.Chance(GhostChance))
            {
                int g = 0;
                while (g<GhostCount)
                {
                    BoardLocation l = Board.Random.RandomLocationNoCorner();
                    if (Board.ContentsAt(l).ContainsOnlyTerrain)
                    {
                        if (g%2==1)
                            Board.AddToken(new MovingGhostToken(Direction.RIGHT, MoveMethod.VERTICAL_PACE), l);
                        else
                            Board.AddToken(new MovingGhostToken(Direction.DOWN, MoveMethod.HORIZONTAL_PACE), l);
                        g++;
                    }
                }
            }
           
        }
    }
}
