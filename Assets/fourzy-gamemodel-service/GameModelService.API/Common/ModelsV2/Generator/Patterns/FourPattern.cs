using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class FourPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }
        public List<BoardLocation> Locations { get; }

        public FourPattern(GameBoard Board)
        {
            Locations = new List<BoardLocation>();

            //Four Corners Large Seperation 4
            //Four Corners Spearation 2
            //Four Plus Sep =1 Top.Col=[2-5] Top.Row = [0-3]            
            //Four Plus Sep =2 Top.Col=[3-4] Top.Row = [0-1]
            //Four Rotate Edge=[1-2], Offset=[1-4]
            Dictionary<string, int> Patterns = new Dictionary<string, int>();
            Patterns.Add("FourCornerLarge", 10);
            Patterns.Add("FourCornerSmall", 10);
            Patterns.Add("FourPlusSep1", 10);
            Patterns.Add("FourPlusSep2", 10);
            Patterns.Add("FourRotateEdge1", 10);
            Patterns.Add("FourRotateEdge2", 10);

            string pattern = Board.Random.RandomWeightedItem(Patterns);

            switch (pattern)
            {
                case "FourCornerLarge":
                    Locations.AddRange( new CornerPattern(Board, new BoardLocation(1,1),Board.Columns -2, Board.Rows-2).Locations);
                    break;
                case "FourCornerSmall":
                    Locations.AddRange(new CornerPattern(Board, new BoardLocation(2, 2), Board.Columns - 4, Board.Rows - 4).Locations);
                    break;

                case "FourPlusSep1":
                    Locations.AddRange(new FourPlusPattern(Board,new BoardLocation(Board.Random.RandomInteger(0,3), Board.Random.RandomInteger(2,5)),1).Locations);
                    break;
                case "FourPlusSep2":
                    Locations.AddRange(new FourPlusPattern(Board, new BoardLocation(Board.Random.RandomInteger(0, 1), Board.Random.RandomInteger(3, 4)), 2).Locations);
                    break;

                case "FourRotateEdge1":
                    Locations.AddRange(new FourRotatedPattern(Board, Board.Random.RandomInteger(1,4),1).Locations);
                    break;

                case "FourRotateEdge2":
                    Locations.AddRange(new FourRotatedPattern(Board, Board.Random.RandomInteger(1, 3), 2).Locations);
                    break;
            }
        }
    }
}
