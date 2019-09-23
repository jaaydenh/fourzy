using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public interface AIPlayer
    {
        PlayerTurn GetTurn();
    }

    public static class AIPlayerFactory
    {
        public static AIPlayer Create(GameState State, AIProfile Profile)
        {
            switch (Profile)
            {
                case AIProfile.EasyAI:
                    return new EasyAI(State);
                case AIProfile.UpBot:
                    return new UpBotAI(State);
                case AIProfile.VerticalBot:
                    return new VerticalBotAI(State);
                case AIProfile.HorizontalBot:
                    return new HorizontalBotAI(State);
                case AIProfile.RotatorBot:
                    return new RotatorBotAI(State);
                case AIProfile.BeginnerAI:
                    return new BeginnerAI(State);
                case AIProfile.PassBot:
                    return new PassBotAI(State);
                case AIProfile.PositionBot:
                    return new PositionBot(State);
                case AIProfile.PuzzleAI:
                    return new PuzzleAI(State);
                case AIProfile.SimpleAI:
                    return new SimpleAI(State);
                case AIProfile.ScoreBot:
                    return new ScoreBotAI(State);
                case AIProfile.WaitBot:
                    return new WaitBotAI(State);
            }
            return null;
        }
    }

}


