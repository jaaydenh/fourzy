using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class RandomPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public int Width { get; set; }
        public int Height { get; set; }
        public int Count { get; set; }


        public RandomPattern(GameBoard Board, BoardLocation Reference, int Count, int Height, int Width)
        {
            this.Reference = Reference;
            this.Width = Width;
            this.Height = Height;
            this.Count = Count;
            Locations = new List<BoardLocation>();
            
            while (Locations.Count < Count)
            {
                BoardLocation l = Board.Random.RandomLocation(Reference, Height, Width);
                if (!Locations.Contains(l)) Locations.Add(l);
            }

        }
    }
}
