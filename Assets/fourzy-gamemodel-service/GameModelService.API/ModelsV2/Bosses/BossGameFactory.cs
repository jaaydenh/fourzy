using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public static class BossGameFactory
    {
        public static GameState CreateBossGame(Area BossArea, BossType Boss, Player Player, GameOptions Options = null, string SeedString = "")
        {
            if (Options == null) Options = new GameOptions();
            Player BossPlayer = new Player(2, Boss.ToString());
            BossPlayer.BossType = Boss;
            BossPlayer.SpecialAbilityCount = 1;
            BossPlayer.Profile = AIProfile.BossAI;
            GameBoard Board = BossFactory.CreateBoard(Boss, Options, SeedString);
            //GameBoard Board = BoardFactory.CreateRandomBoard(Options, BossPlayer, Player, BossArea);
            GameState BossGS = new GameState(Board,Options,1);
            BossGS.Players.Add(1, Player);
            BossGS.Players.Add(2, BossPlayer);

            return BossGS;
        }

        public static GameState CreateBossGame(GameBoardDefinition definition, BossType Boss, Player Player, GameOptions Options = null)
        {
            if (Options == null) Options = new GameOptions();
            Player BossPlayer = new Player(2, Boss.ToString(), AIProfile.BossAI);
            BossPlayer.BossType = Boss;
            BossPlayer.SpecialAbilityCount = 1;
            BossPlayer.Profile = AIProfile.BossAI;
            GameBoard Board = new GameBoard(definition);
            GameState BossGS = new GameState(Board, Options, 1);
            BossGS.Players.Add(1, Player);
            BossGS.Players.Add(2, BossPlayer);

            return BossGS;
        }
    }
}
