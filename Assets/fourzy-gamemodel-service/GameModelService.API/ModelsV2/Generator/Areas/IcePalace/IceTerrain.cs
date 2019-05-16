//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace FourzyGameModel.Model
//{
//    public class IceTerrain : IBoardIngredient
//    {
//        public string Name { get; }
//        public IngredientType Type { get; }
//        public TokenType Token { get; set; }

//        public int Width { get; set; }
//        public int Height { get; set; }

//        public BoardLocation Insert { get; set; }
//        //public LargeFeatureType Feature { get { return LargeFeatureType.BIGSTEPS; } }

//        private Dictionary<string, int> Patterns;

//        public IceTerrain(int FillPattern = 1)
//        {
//            this.Name = "Ice Terrain";
//            this.Type = IngredientType.TERRAIN;
//            this.Token = Token;

//            this.Patterns = new Dictionary<string, int>();
//        }

//        public void Build(GameBoard Board)
//        {
//            string Pattern = Board.Random.RandomWeightedItem(Patterns);
//            List<BoardLocation> Locations = new FillPattern(Board, new BoardLocation(0, 0), Board.Columns, Board.Rows).Locations;
//            Board.AddToken(new IceToken(), Locations, AddTokenMethod.NO_TERRAIN, true);
//        }
//    }
//}
