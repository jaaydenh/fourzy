using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class CrossTheBoardPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        Direction Start { get; set; }
        Direction End { get; set; }

        public CrossTheBoardPattern(GameBoard Board, Direction Start, Direction End)
        {
            this.Locations = new List<BoardLocation>();

            BoardLocation flow = new RandomEdgeLocation(Board, Start).Locations.First();
            Locations.Add(flow);
            int MinRiver = Board.Rows + 4;
            int MaxRiver = Board.Rows * 2 + 4;
            int RiverCount = 1;
            int MaxTries = 100;
            Direction FlowDirection = End;
            Direction LastDirection = End;
            int direction_count = 0;
    
            while (flow.Column < Board.Columns - 1 && MaxTries-- > 0)
            {
                //The river will flow from Start to End.

                //go with the the flow.
                BoardLocation next = flow.Neighbor(FlowDirection);

                //if hit the end of the board, go to the target direction
                if (!next.OnBoard(Board)) { FlowDirection = End; continue; }

                //for backtracking.
                if (Locations.Contains(next))
                {
                    flow = next;
                    if (LastDirection == FlowDirection) direction_count++;
                    LastDirection = FlowDirection;

                    continue;
                }

                BoardSpace target = Board.ContentsAt(next);

                bool progress = false;
                //try to go this way. Only if a piece can enter, and end a turn.
                if (target.TokensAllowEndHere && target.TokensAllowEndHere)
                {
                    Locations.Add(next);
                    if (LastDirection == FlowDirection) direction_count++;
                    else direction_count = 1;
                    LastDirection = FlowDirection;
                    RiverCount++;
                    flow = next;
                    progress = true;
                }


               if (progress)
               {
                    //25 percent river change course

                    if (Board.Random.Chance(20*(direction_count-1)))
                    {
                        switch (FlowDirection)
                        {
                            case Direction.UP:
                                FlowDirection = Board.Random.RandomDirection(new List<Direction>() { Direction.LEFT, Direction.RIGHT });
                                break;
                            case Direction.DOWN:
                                FlowDirection = Board.Random.RandomDirection(new List<Direction>() { Direction.LEFT, Direction.RIGHT });
                                break;
                            case Direction.LEFT:
                                FlowDirection = Board.Random.RandomDirection(new List<Direction>() { Direction.DOWN, Direction.UP });
                                break;
                            case Direction.RIGHT:
                                FlowDirection = Board.Random.RandomDirection(new List<Direction>() { Direction.DOWN, Direction.UP});
                                break;
                        }
                        continue;
                    }


                    switch (End)
                    {
                        case Direction.UP:
                        case Direction.DOWN:
                            if (flow.Column == 0) { FlowDirection = Direction.RIGHT; continue; }
                            if (flow.Column == Board.Columns - 1) {FlowDirection = Direction.LEFT; continue; }
                            break;
                        case Direction.LEFT:
                        case Direction.RIGHT:
                            if (flow.Row == 0) { FlowDirection = Direction.DOWN; continue; }
                            if (flow.Row == Board.Rows - 1) { FlowDirection = Direction.UP; continue; }
                            break;
                    }

                    //50 percent river will recent to target direction
                    if (FlowDirection != End)
                        if (Board.Random.Chance(50))
                        { FlowDirection = End; continue; }

                }


                //if can't go foward, try to go left or right.
                if (!progress)
                {
                    List<Direction> possible = new List<Direction>();
                    foreach (Direction d in Enum.GetValues(typeof(Direction)))
                    {
                        if (d == Direction.NONE) continue;
                        if (d == Direction.TELEPORT) continue;
                        if (d == Start) continue;
                        if (d == FlowDirection) continue;
                        BoardSpace t = Board.ContentsAt(flow.Neighbor(d));
                        if (t.TokensAllowEndHere && t.TokensAllowEnter) possible.Add(d);
                    }

                    if (possible.Count == 0) return;
                    if (possible.Count == 1) { FlowDirection = possible.First();  continue; }
                    FlowDirection = Board.Random.RandomDirection(possible);
                }

            }
        }

    }
}
