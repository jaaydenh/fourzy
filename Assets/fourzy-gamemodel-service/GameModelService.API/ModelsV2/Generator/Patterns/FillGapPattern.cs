using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class FillGapsPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public FillGapsPattern(GameBoard Board, int Count =4)
        {
            this.Reference = Reference;

            List<BoardLocation> PossibleLocations = new List<BoardLocation>();
            foreach (BoardSpace s in Board.Contents)
            {
                if (s.ContainsToken)
                {
                    foreach (IToken t in s.Tokens.Values)
                    {
                        if (t.Classification != TokenClassification.TERRAIN) PossibleLocations.Add(s.Location);
                    }
                } else
                {
                    PossibleLocations.Add(s.Location);
                }
            }

            Locations = new List<BoardLocation>();
            while (Locations.Count < Count && PossibleLocations.Count > 0)
            {
                int index = Board.Random.RandomInteger(0, PossibleLocations.Count - 1);
                Locations.Add(PossibleLocations[index]);
                PossibleLocations.RemoveAt(index);
            }
        }
    }
}
