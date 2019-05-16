using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public static class BossGameFactory
    {
        public static GameState CreateBossGame(Area BossArea, int BossId, Player Player, GameOptions Options = null)
        {
            if (Options == null) Options = new GameOptions();
            Player Boss = new Player(1, "Boss");
            GameBoard Board = BoardFactory.CreateRandomBoard(Options, Boss, Player, BossArea);
            GameState BossGS = new GameState(Board,Options,1);
            BossGS.Players.Add(1, Boss);
            BossGS.Players.Add(2, new Player(2, "Player"));

            return BossGS;
        }

    }
}
