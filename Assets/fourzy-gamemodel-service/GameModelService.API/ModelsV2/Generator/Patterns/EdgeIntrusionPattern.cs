using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class EdgeIntrusionPattern : IBoardPattern
    {
        public List<BoardLocation> Locations { get; }

        public EdgeIntrusionPattern(GameBoard Board, int Width = 2, int Depth=2, int IntrusionCount = 1)
        {

            Locations = new List<BoardLocation>();
            List<BoardLocation> PossibleLocations = new List<BoardLocation>();
            PossibleLocations = Board.Edges;
            
            for (int i=0; i<IntrusionCount; i++)
            {
                while (PossibleLocations.Count > 0)
                {
                    int Index = Board.Random.RandomInteger(0, PossibleLocations.Count - 1);
                    BoardLocation Target = PossibleLocations[Index];
                    PossibleLocations.Remove(Target);

                    //Top
                    if (Target.Row ==0 )
                    {
                        if (Target.Column + Width >= Board.Columns) continue;
                        for (int w = 0; w < Width; w++)
                            for (int d = 0; d < Depth; d++)
                                Locations.Add(new BoardLocation(Target.Row + d, Target.Column + w));

                    }

                    //Left
                    if (Target.Column == 0)
                    {
                        if (Target.Row + Width >= Board.Rows) continue;
                        for (int w = 0; w < Width; w++)
                            for (int d = 0; d < Depth; d++)
                                Locations.Add(new BoardLocation(Target.Row + w, Target.Column + d));
                    }

                    //Bottom
                    if (Target.Row == Board.Rows -1)
                    {
                        if (Target.Column + Width >= Board.Columns) continue;
                        for (int w = 0; w < Width; w++)
                            for (int d = 0; d < Depth; d++)
                                Locations.Add(new BoardLocation(Target.Row - d, Target.Column + w));
                    }

                    //Right
                    if (Target.Column == Board.Columns -1)
                    {
                        if (Target.Row + Width >= Board.Rows) continue;
                        for (int w = 0; w < Width; w++)
                            for (int d = 0; d < Depth; d++)
                                Locations.Add(new BoardLocation(Target.Row + w, Target.Column - d));
                    }

                    break;
                }
            }

        }
    }
}
