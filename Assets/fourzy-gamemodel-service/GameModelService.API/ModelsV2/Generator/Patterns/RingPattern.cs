using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class RingPattern : IBoardPattern
    {
        public List<BoardLocation> Locations { get; }

        public string Name { get; }
        public IngredientType Type { get; }

        public SmallFeatureType Feature { get { return SmallFeatureType.FILL_THE_RING; } }
        public int TokenCount { get; set; }
        public LineFillStyle FillStyle { get; set; }

        public RingPattern(GameBoard Board, int Ring, int TokenCount = 1, LineFillStyle FillStyle = LineFillStyle.RANDOM)
        {
            this.Name = "Ring";
            this.Type = IngredientType.SMALLFEATURE;
            this.TokenCount = TokenCount;
            Locations = new List<BoardLocation>();
            //int count = 500;
            //int tokens = 0;
            int pick = -1;
            List<BoardLocation> RingLocations = new BoardLocation(Ring, Ring).Ring(Board, Rotation.CLOCKWISE);

            switch (FillStyle)
            {
                //pick x unique locations
                case LineFillStyle.RANDOM:
                    for (int i = 0; i < TokenCount; i++)
                    {
                        pick = Board.Random.RandomInteger(0, RingLocations.Count-1);
                        Locations.Add(RingLocations.ElementAt(pick));
                        RingLocations.RemoveAt(pick);
                    }
                    break;

                //pick 1 unique location.
                //fill x times going clockwise
                case LineFillStyle.CONNECTED:
                    pick = Board.Random.RandomInteger(0, RingLocations.Count-1);
                    for (int i = 0; i < TokenCount; i++)
                    {
                        Locations.Add(RingLocations.ElementAt(pick));
                        pick = ++pick % RingLocations.Count;
                    }
                    break;

                //pick 1 unique location.
                //divide up ring and space evenly
                case LineFillStyle.SYMMETRIC:
                    pick = Board.Random.RandomInteger(0, RingLocations.Count-1);
                    int spacing = RingLocations.Count / TokenCount;
                    for (int i = 0; i < TokenCount; i++)
                    {
                        Locations.Add(RingLocations.ElementAt(pick));
                        pick = (pick + spacing) % RingLocations.Count;
                    }
                    break;

                case LineFillStyle.ALTERNATING:
                    bool use = Board.Random.Chance(50); 
                    foreach (BoardLocation l in RingLocations)
                    {
                        if (use) Locations.Add(l);
                        use = !use;
                    }
                    break;

                case LineFillStyle.FILLED:
                    Locations.AddRange(RingLocations.ToList());
                    break;
            }
        }
    }
}
