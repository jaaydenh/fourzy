using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FourzyGameModel.Model
{
    public class RandomTools
    {
        private GameState State { get; }
        private Random R { get; set; }

        public RandomTools(GameState State)
        {
            this.State = State;
            this.R = new Random(CreateSeed());
        }

        public RandomTools(string SeedString = "")
        {
            this.State = new GameState();
            if (SeedString == "") SeedString = Guid.NewGuid().ToString();
            this.R = new Random(CreateSeed(SeedString));
        }
        
        public void Reset()
        {
            this.R = new Random(CreateSeed());
        }

        public bool Chance(int Percentage)
        {
            if (RandomInteger(0, 100) <= Percentage) return true;
            return false;
        }

        public Area RandomArea()
        {
            var v = Enum.GetValues(typeof(Area));
            Area a = Area.NONE;
            while (a == Area.NONE) { a = (Area)v.GetValue(R.Next(v.Length)); }
            return a;
        }

        public T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(new Random().Next(v.Length));
        }
        
        public Direction RandomDirection()
        {
            return (Direction)RandomInteger(0,4);
        }

        public Direction RandomDirection(List<Direction> Directions)
        {
            return Directions[RandomInteger(0, Directions.Count -1)];
        }
        public CompassDirection RandomCompassDirection(List<CompassDirection> Directions)
        {
            return Directions[RandomInteger(0, Directions.Count - 1)];
        }
        public CompassDirection RandomCompassDirection()
        {
            return (CompassDirection)RandomInteger(0,7);
        }

        public Rotation RandomRotation()
        {
            return (Rotation)RandomInteger(0, 1);
        }
        
        public LineType RandomLineType()
        {
            return (LineType)RandomInteger(0, 2);
        }

        public BoardLocation RandomLocation(List<BoardLocation> Locations)
        {
            if (Locations.Count == 0) throw new Exception("No Locations passed to function");
            return Locations[Range(0, Locations.Count-1)];
        }

        public BoardLocation RandomLocation(BoardLocation Reference, int Height, int Width)
        {
            return new BoardLocation(Reference.Row + RandomInteger(0, Height), Reference.Column + RandomInteger(0, Width));
        }

        public BoardLocation RandomLocationNoCorner()
        {
            BoardLocation l = new BoardLocation(RandomInteger(0, State.Board.Rows - 1), RandomInteger(0, State.Board.Columns - 1));
            while (State.Board.Corners.Contains(l))
                l = new BoardLocation(RandomInteger(0, State.Board.Rows - 1), RandomInteger(0, State.Board.Columns - 1));

            return l;
        }

        public string RandomItem(List<string> Items)
        {
            return Items[RandomInteger(0, Items.Count - 1)];
        }

        public int RandomNumericItem(List<int> Items)
        {
            return Items[RandomInteger(0, Items.Count - 1)];
        }

        public string RandomWeightedItem(Dictionary<string, int> Items)
        {
            int Total = 0;
            foreach (string key in Items.Keys)
            {
                Total += Items[key];
            }
            int Countdown = RandomInteger(0, Total);
            foreach (string key in Items.Keys)
            {
                Countdown-= Items[key];
                if (Countdown <= 0) return key;
            }

            return "";
        }

        public BoardRecipe RandomWeightedRecipe(Dictionary<BoardRecipe, int> Recipes)
        {
            int Total = 0;
            foreach (BoardRecipe key in Recipes.Keys)
            {
                Total += Recipes[key];
            }
            int Countdown = RandomInteger(0, Total);
            foreach (BoardRecipe key in Recipes.Keys)
            {
                Countdown -= Recipes[key];
                if (Countdown <= 0) return key;
            }

            return null;
        }


        public int Range(int Min, int Max)
        {
            return R.Next(Min, Max+1);
        }

        public int RandomInteger(int Min, int Max)
        {
            return R.Next(Min, Max+1);
        }

        private int CreateSeed(string SeedString="RandomPlaceHolder")
        {
            //int x = Game.GameSeed.ToCharArray().Sum(x => x) % 100;
            int TurnCount = 0;
            if (State != null)
            {
                SeedString = State.GameSeed;
                TurnCount = State.TurnCount;
            }

            MD5 md5Hasher = MD5.Create();
            var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(CycleShift(SeedString, TurnCount)));
            return Math.Abs(BitConverter.ToInt32(hashed, 0));
        }

        public static string CycleShift(string s, int k)
        {
            k = k % s.Length;
            return s.Substring(k) + s.Substring(0, k);
        }

    }
}
