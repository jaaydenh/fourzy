using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public static class BossFactory
    {
        public static IBoss Create(BossType Type)
        {

            switch (Type) {
                case BossType.EarthQuake:
                    return new EarthQuakeBoss();
                case BossType.MadBomber:
                    return new MadBomberBoss();
                case BossType.DirectionMaster:
                   return new DirectionMasterBoss();
                case BossType.EntryWay:
                    return new EntryWayBoss();
                case BossType.Treant:
                    return new TreantBoss();
                case BossType.Necrodancer:
                    return new NecromancerBoss();
            }

            return null; 
        }
              
        public static List<IBossPower> GetPowers(BossType Type)
        {
            IBoss Boss = BossFactory.Create(Type);
            return Boss.Powers;
        }

        public static GameBoard CreateBoard(BossType Type, GameOptions Options = null, string SeedString="")
        {
            if (Options == null) Options = new GameOptions();

            GameBoard Board = BoardFactory.CreateDefaultBoard(Options);
            Board.Random = new RandomTools(SeedString);
            switch (Type)
            {
                case BossType.MadBomber:
                    break;

                case BossType.DirectionMaster:
                    List<int> Columns = new List<int>();
                    for (int c = 0; c < Board.Columns; c++)
                        Columns.Add(c);
                    for (int r=0; r< Board.Rows; r++)
                    {
                        int col = Board.Random.RandomNumericItem(Columns);
                        if (r == 0 && col ==0) continue;
                        if (r == Board.Rows && col == Board.Columns) continue;
                        Columns.Remove(col);
                        Board.AddToken(new ArrowToken(Board.Random.RandomDirection()), new BoardLocation(r, col), AddTokenMethod.ALWAYS);
                    }
                    break;
                case BossType.EntryWay:
                    Board = BoardFactory.CreateGameBoard(new BeginnerGardenRandomGenerator(""), Options);

                    break;

                case BossType.EarthQuake:
                    Board = BoardFactory.CreateGameBoard(new BeginnerGardenRandomGenerator(""), Options);

                    break;

                case BossType.Necrodancer:
                    Board = BoardFactory.CreateGameBoard(new BeginnerGardenRandomGenerator(""), Options);

                    break;
                
                case BossType.Treant:
                    //List<BoardLocation> TreeLocations = new List<BoardLocation>();
                    //for (int i=0; i<4; i++)
                    //{
                    //    BoardLocation l = Board.Random.RandomLocation(new BoardLocation(0, 0), Board.Rows, Board.Columns);
                    //    if (!TreeLocations.Contains(l)) TreeLocations.Add(l);
                    //}
                    //foreach (BoardLocation l in TreeLocations)
                    //    Board.AddToken(new BlockerToken(), l, AddTokenMethod.ALWAYS);

                    break;


            }

            return Board;
        }

    }
}
