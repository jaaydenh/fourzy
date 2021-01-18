using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public interface ILargeFeature
    {
        BoardLocation Insert { get; set; }
        int Width { get; set; }
        int Height { get; set; }

        LargeFeatureType Feature { get; }
        void Build(GameBoard Board);
    }

    public interface ISmallFeature
    {
        BoardLocation Insert { get; set; }
        int Width { get; set; }
        int Height { get; set; }

        SmallFeatureType Feature { get; }
        void Build(GameBoard Board);
    }

}
