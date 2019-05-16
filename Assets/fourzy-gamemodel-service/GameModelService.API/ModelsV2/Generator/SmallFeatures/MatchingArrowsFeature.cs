using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class MatchingArrowsFeature : IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }

        public SmallFeatureType Feature { get { return SmallFeatureType.MATCHING_ARROWS; } }
        public int MatchCount { get; set; }

        public MatchingArrowsFeature(int MatchCount = 8)
        {
            this.Name = "Matching Arrows";
            this.Type = IngredientType.SMALLFEATURE;
            this.MatchCount = MatchCount;
        }

        public void Build(GameBoard Board)
        {
            int count = 0;
 
            while (count < MatchCount)
            {
                TwoPattern Two = new TwoPattern(Board, new BoardLocation(0,0), CompassDirection.NONE, Board.Random.RandomInteger(1, 2));
                Board.AddToken(new ArrowToken(Board.Random.RandomDirection()), Two.Locations);
                count++;
            }


        }


    }
}
