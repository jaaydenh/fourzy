using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ArrowStormFeature : IBoardIngredient
    {
        public string Name { get { return "ArrowStorm"; } }
        public IngredientType Type { get; }
        public LargeFeatureType Feature { get { return LargeFeatureType.ARROW_STORM; } }
        public int NumberCycles { get; set; }

        public ArrowStormFeature(int NumberCycles = 3)
        {
            this.NumberCycles = NumberCycles;
        }

        public void Build(GameBoard Board)
        {
            Rotation CR = Rotation.CLOCKWISE;
            for (int i =0; i< NumberCycles; i++)
            {
                BoardLocation Insert = Board.Random.RandomLocation(new BoardLocation(1, 1), 3, 3);
                ArrowCycleFeature ACF = new ArrowCycleFeature(Rotation.COUNTER_CLOCKWISE, Insert, 4,4);
                ACF.Build(Board);
                if (CR == Rotation.CLOCKWISE) CR = Rotation.COUNTER_CLOCKWISE;
                else CR = Rotation.CLOCKWISE;
            }

        }
    }
}
