//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace FourzyGameModel.Model
//{
//    public class HalfFeature : ILargeFeature
//    {
//        public Direction Filled { get; set; }
//        public TokenType Token { get; set; }
//        public int Height { get; set; }
//        public int Width { get; set; }
//        public BoardLocation Insert { get; set; }
//        public LargeFeatureType Feature { get { return LargeFeatureType.HALF; } }
        
//        public HalfFeature(TokenType Token, Direction Filled = Direction.NONE, string RandomSeed = "")
//        {
//            this.Token = Token;
//            if (Filled == Direction.NONE)
//            {
//                RandomTools RT = new RandomTools(RandomSeed + Feature.ToString());
//                Filled = RT.RandomDirection();
//            }
//            this.Filled = Filled;
//            this.Insert = new BoardLocation(0, 0);
//        }

//        public void Build(GameBoard Board)
//        {

//            Height = Board.Rows / 2;
//            Width = Board.Columns / 2;

//            switch (Filled)
//            {
//                case Direction.UP:

//                    this.Insert = new BoardLocation(0, 0);
//                    Height = Board.Rows;
//                    Width = Board.Columns / 2;
//                    break;

//                case Direction.DOWN:
//                    this.Insert = new BoardLocation(0, Board.Columns / 2);
//                    Height = Board.Rows;
//                    Width = Board.Columns / 2;
//                    break;

//                case Direction.RIGHT:
//                    this.Insert = new BoardLocation(0, 0);
//                    Height = Board.Rows / 2;
//                    Width = Board.Columns;
//                    break;

//                case Direction.LEFT:
//                    this.Insert = new BoardLocation(Board.Rows / 2, 0);
//                    Height = Board.Rows / 2;
//                    Width = Board.Columns;
//                    break;

//            }

//            for (int r = 0; r < Height; r++)
//            {
//                for (int c = 0; c < Width; c++)
//                {
//                    Board.AddToken(TokenFactory.Create(Token), new BoardLocation(Insert.Row + r, Insert.Column + c));
//                }
//            }

//        }
//    }
//}
