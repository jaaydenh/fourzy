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
                case AIProfile.BeginnerAI:
                    return new BeginnerAI(State);
                case AIProfile.PositionBot:
                    return new PositionBot(State);
                case AIProfile.PuzzleAI:
                    return new PuzzleAI(State);
                case AIProfile.SimpleAI:
                    return new SimpleAI(State);
            }
            return null;
        }
    }

}


