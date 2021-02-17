using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class OPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public OPattern(GameBoard Board, BoardLocation Reference, int Width, int Height)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c < Width; c++)
                {
                    if (c==0 || r==0 || c==Width-1 || r==Height-1) 
                        Locations.Add(new BoardLocation(Reference.Row + r, Reference.Column + c));
                }
            }

        }
    }
}
