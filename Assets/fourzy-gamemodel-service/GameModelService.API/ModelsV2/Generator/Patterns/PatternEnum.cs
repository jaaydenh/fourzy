using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public enum PatternType
    {
        FullDots,
        BlockEdge,
        BlockAllEdges,
        BubbleEdge,
        LargeCircle,  //A circle one space from edge
        FullCheckers,
        AlmostFullCheckers,
        DiagonalLines,
        Dots,
        DottedCross,
        DottedLine,
        OneFullLine,
        FullLines,
        HorizontalLine,
        VerticalLine,
        DoubleLine,
        DoubleRow,
        PinWheel,
        FillGaps,
        CrossTheBoard,
        FullStarPattern,
        BubbleEdgeRight,
        BubbleEdgeLeft,
        BubbleEdgeUp,
        BubbleEdgeDown,
        Corners,
        CentralCorners,
        Four,
        CenterFour,
        ACouple,
        OneRandom,
        TwoRandom,
        ThreeRandom,
        CenterSixteen,
        LargeCheckers,
        WideLine,
        WideCross,
        DoubleCross,
        SmallPinWheel,
        PartialBlockEdges,
        BlockEdgesOneSpace,
        BlockEdgesTwoSpace,
        RandomBlockEdges,
        SmallSymmetricBlockEdgePattern,
        MediumSymmetricBlockEdgePattern,
        LargeSymmetricBlockEdgePattern,
        CenterBlob,
        OuterRing,
        CenterRing,
        CrossQuads,
        OuterCorners,
        CenterDiagonal,
        EdgeBump,
        EdgeBumpDuo,
        EdgeSpike,
        EdgeSpikeDuo,
        EdgeBumpQuartet,
        EdgeSpikeQuartet,
        Half,
        Full,
        AlmostFull
    }

    public enum RelativeDirection { IN, OUT, NONE}
}
