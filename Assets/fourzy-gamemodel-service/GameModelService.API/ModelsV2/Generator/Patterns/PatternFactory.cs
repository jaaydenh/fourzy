using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public static class PatternFactory
    {
        public static List<BoardLocation> GetLocations(PatternType Pattern, GameBoard Board)
        {
            switch (Pattern)
            {
                case PatternType.ACouple:
                    return new ACouplePattern(Board).Locations;
                case PatternType.OneRandom:
                    return new ACouplePattern(Board, 1).Locations;
                case PatternType.TwoRandom:
                    return new ACouplePattern(Board,2).Locations;
                case PatternType.ThreeRandom:
                    return new ACouplePattern(Board, 3).Locations;
                case PatternType.CenterBlob:
                    return new CenterBlobPattern(Board).Locations;
                case PatternType.CenterDiagonal:
                    return new SolidFullLinePattern(Board,new BoardLocation(1,1), LineType.DIAGONAL).Locations;
                case PatternType.CrossTheBoard:
                    return new CrossTheBoardPattern(Board, Direction.LEFT, Direction.RIGHT).Locations;
                case PatternType.CrossQuads:
                    return new CrossQuadPattern(Board).Locations;
                case PatternType.DoubleCross:
                    return new WideCrossPattern(Board, 2).Locations;
                case PatternType.DiagonalLines:
                    return new DiagonalLinesPattern(Board).Locations;
                case PatternType.EdgeBump:
                    return new EdgeIntrusionPattern(Board,2,2,1).Locations;
                case PatternType.EdgeBumpDuo:
                    return new EdgeIntrusionPattern(Board, 2, 2, 2).Locations;
                case PatternType.EdgeBumpQuartet:
                    return new EdgeIntrusionPattern(Board, 2, 2, 4).Locations;
                case PatternType.EdgeSpike:
                    return new EdgeIntrusionPattern(Board, 1, 3, 1).Locations;
                case PatternType.EdgeSpikeDuo:
                    return new EdgeIntrusionPattern(Board, 1, 3, 2).Locations;
                case PatternType.EdgeSpikeQuartet:
                    return new EdgeIntrusionPattern(Board, 1, 3, 4).Locations;
                case PatternType.FullDots:
                    return new DotPattern(Board, new BoardLocation(0, 0), Board.Columns, Board.Rows).Locations;
                case PatternType.AlmostFull:
                    return new RectanglePattern(Board,new BoardLocation(1,1), Board.Rows-2, Board.Columns-2).Locations;
                case PatternType.Full:
                    return new RectanglePattern(Board, new BoardLocation(0, 0), Board.Rows , Board.Columns).Locations;
                case PatternType.Half:
                    return new HalfPattern(Board).Locations;
                case PatternType.BlockEdgesOneSpace:
                    return new BlockRandomEdgePattern(Board,1).Locations;
                case PatternType.BlockEdgesTwoSpace:
                    return new BlockRandomEdgePattern(Board, 2).Locations;
                case PatternType.PartialBlockEdges:
                    return new BlockRandomEdgePattern(Board,4).Locations;
                case PatternType.RandomBlockEdges:
                    return new BlockRandomEdgePattern(Board,6).Locations;
                case PatternType.BlockEdge:
                    return new BlockEdgePattern(Board, Board.Random.RandomDirection()).Locations;
                case PatternType.BlockAllEdges:
                    return new BlockAllEdgePattern(Board).Locations;
                case PatternType.SmallSymmetricBlockEdgePattern:
                    return new SymmetricEdgeBlockPattern(Board,1).Locations;
                case PatternType.MediumSymmetricBlockEdgePattern:
                    return new SymmetricEdgeBlockPattern(Board,2).Locations;
                case PatternType.LargeSymmetricBlockEdgePattern:
                    return new SymmetricEdgeBlockPattern(Board,3).Locations;
                case PatternType.BubbleEdge:
                    return new BubbleEdgePattern(Board, Board.Random.RandomDirection()).Locations;
                case PatternType.BubbleEdgeLeft:
                    return new BubbleEdgePattern(Board, Direction.LEFT).Locations;
                case PatternType.BubbleEdgeRight:
                    return new BubbleEdgePattern(Board, Direction.RIGHT).Locations;
                case PatternType.BubbleEdgeUp:
                    return new BubbleEdgePattern(Board, Direction.UP).Locations;
                case PatternType.BubbleEdgeDown:
                    return new BubbleEdgePattern(Board, Direction.DOWN).Locations;
                case PatternType.CenterFour:
                    return new CenterFourPattern(Board).Locations;
                case PatternType.CenterSixteen:
                    return new RectanglePattern(Board,new BoardLocation(Board.Rows/2-2,Board.Columns/2-2),4,4).Locations;
                case PatternType.CentralCorners:
                    return new CornerPattern(Board, new BoardLocation(2,2), Board.Columns-4, Board.Rows-4).Locations;
                case PatternType.DottedCross:
                    return new DottedCrossPattern(Board, Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows - 2, Board.Columns - 2)).Locations;
                case PatternType.DottedLine:
                    return new DottedLinePattern(Board, Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows - 2, Board.Columns - 2), Board.Random.RandomLineType()).Locations;
                case PatternType.Four:
                    return new FourPattern(Board).Locations;
                case PatternType.LargeCheckers:
                    return new LargeCheckerPattern(Board).Locations;
                case PatternType.OuterRing:
                    return new OPattern(Board, new BoardLocation(1,1),Board.Columns-2, Board.Rows-2).Locations;
                case PatternType.OuterCorners:
                    return new CornerThreePattern(Board).Locations;
                case PatternType.CenterRing:
                    return new OPattern(Board, new BoardLocation(2,2),Board.Columns - 4, Board.Rows - 4).Locations;
                case PatternType.FullLines:
                    return new FullLinesPattern(Board).Locations;
                case PatternType.OneFullLine:
                    return new SolidFullLinePattern(Board, Board.Random.RandomLocation(new BoardLocation(2, 2), Board.Rows - 2, Board.Columns - 2), Board.Random.RandomLineType()).Locations;
                case PatternType.HorizontalLine:
                    return new SolidFullLinePattern(Board, Board.Random.RandomLocation(new BoardLocation(2, 2), Board.Rows - 3, Board.Columns - 3), LineType.HORIZONTAL).Locations;
                case PatternType.VerticalLine:
                    return new SolidFullLinePattern(Board, Board.Random.RandomLocation(new BoardLocation(2, 2), Board.Rows - 3, Board.Columns - 3), LineType.VERTICAL).Locations;
                case PatternType.AlmostFullCheckers:
                    return new CheckerPattern(Board, new BoardLocation(1, 1), Board.Columns - 2, Board.Rows - 2).Locations;
                case PatternType.FullCheckers:
                    return new CheckerPattern(Board, new BoardLocation(0, 0), Board.Columns, Board.Rows).Locations;
                case PatternType.FullStarPattern:
                    return new FullStarPattern(Board, Board.Random.RandomLocation(new BoardLocation(2, 2), Board.Rows - 3, Board.Columns - 3)).Locations;
                case PatternType.SmallPinWheel:
                    return new PinWheelPattern(Board,"",4,6).Locations;
                case PatternType.PinWheel:
                    return new PinWheelPattern(Board).Locations;
                case PatternType.FillGaps:
                    return new FillGapsPattern(Board).Locations;
                case PatternType.WideLine:
                    return new WideLinePattern(Board).Locations;
                case PatternType.WideCross:
                    return new WideCrossPattern(Board).Locations;
            }
            return new List<BoardLocation>();
        }
    }
}
