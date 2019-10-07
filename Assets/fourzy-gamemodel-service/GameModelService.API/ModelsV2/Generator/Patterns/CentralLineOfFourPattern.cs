using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class CentralLineOfFourPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        //create a patter

        public CentralLineOfFourPattern(GameBoard Board, int Count=1)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            BoardLocation Origin = new BoardLocation(Board.Rows/2-2, Board.Columns/2-2);
            List<int> ChosenLines = new List<int>();

            while (ChosenLines.Count < Count)
            {
                int i = Board.Random.RandomInteger(0, 9);
                if (!ChosenLines.Contains(i))
                    ChosenLines.Add(i);
            }

            for (int line = 0; line < ChosenLines.Count; line++)
            {
                int id = ChosenLines[line];
                switch (id)
                {
                    //Rows
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        for (int i=0; i<4; i++)
                        {
                            BoardLocation l = new BoardLocation(Origin.Row + id, Origin.Column + i);
                            if (!Locations.Contains(l)) Locations.Add(l);
                        }
                        break;

                    //Cols
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        for (int i = 0; i < 4; i++)
                        {
                            BoardLocation l = new BoardLocation(Origin.Row + i, Origin.Column + id - 4);
                            if (!Locations.Contains(l)) Locations.Add(l);

                        }
                        break;

                    //Diags
                    case 8:
                        for (int i = 0; i < 4; i++)
                        {
                            BoardLocation l = new BoardLocation(Origin.Row + i, Origin.Column + 1);
                            if (!Locations.Contains(l)) Locations.Add(l);

                        }
                        break;

                    case 9:
                        for (int i = 0; i < 4; i++)
                        {
                            BoardLocation l = new BoardLocation(Origin.Row + i, Origin.Column + 3 - i);
                               if (!Locations.Contains(l)) Locations.Add(l);

                        }
                        break;
                }
            }

           
                //case Direction.RIGHT:
                //    for (int r = 1; r < Board.Rows - 1; r++) Locations.Add(new BoardLocation(r, Board.Columns - 1));
                //    break;

        }
    }
}
