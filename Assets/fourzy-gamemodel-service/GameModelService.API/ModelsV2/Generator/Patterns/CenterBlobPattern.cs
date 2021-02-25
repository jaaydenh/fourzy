using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class CenterBlobPattern : IBoardPattern
    {
        public List<BoardLocation> Locations { get; }

        public CenterBlobPattern(GameBoard Board)
        {
            Locations = new List<BoardLocation>();
            BoardLocation Center = new BoardLocation(Board.Rows / 2-1, Board.Columns / 2-1);
            Locations.Add(Center);
            Locations.Add(Center.Neighbor(Direction.RIGHT));
            Locations.Add(Center.Neighbor(Direction.DOWN));
            Locations.Add(new BoardLocation(Center.Row+1, Center.Column+1));

            Locations.Add(new BoardLocation(Center.Row - 1, Center.Column - 1));
            Locations.Add(new BoardLocation(Center.Row - 1, Center.Column + 2));
            Locations.Add(new BoardLocation(Center.Row + 2, Center.Column - 1));
            Locations.Add(new BoardLocation(Center.Row + 2, Center.Column + 2));


            foreach (BoardLocation l in Center.Look(Board,Direction.LEFT))
            {
                int chance = 90;
                if (Board.Random.Chance(chance--)) Locations.Add(l);
                else break;
            }
            foreach (BoardLocation l in Center.Look(Board, Direction.UP))
            {
                int chance = 90;
                if (Board.Random.Chance(chance--)) Locations.Add(l);
                else break;
            }
            foreach (BoardLocation l in Center.Neighbor(Direction.RIGHT).Look(Board, Direction.UP))
            {
                int chance = 90;
                if (Board.Random.Chance(chance--)) Locations.Add(l);
                else break;
            }
            foreach (BoardLocation l in Center.Neighbor(Direction.RIGHT).Look(Board, Direction.RIGHT))
            {
                int chance = 90;
                if (Board.Random.Chance(chance--)) Locations.Add(l);
                else break;
            }
            foreach (BoardLocation l in Center.Neighbor(Direction.DOWN).Look(Board, Direction.LEFT))
            {
                int chance = 90;
                if (Board.Random.Chance(chance--)) Locations.Add(l);
                else break;
            }
            foreach (BoardLocation l in Center.Neighbor(Direction.DOWN).Look(Board, Direction.DOWN))
            {
                int chance = 90;
                if (Board.Random.Chance(chance--)) Locations.Add(l);
                else break;
            }
            foreach (BoardLocation l in (new BoardLocation(Center.Row + 1, Center.Column + 1)).Look(Board, Direction.RIGHT))
            {
                if (Board.Random.Chance(75)) Locations.Add(l);
                else break;
            }
            foreach (BoardLocation l in (new BoardLocation(Center.Row + 1, Center.Column + 1)).Look(Board, Direction.DOWN))
            {
                if (Board.Random.Chance(75)) Locations.Add(l);
                else break;
            }


        }
    }
}
