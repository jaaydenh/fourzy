using System.Collections.Generic;

namespace FourzyGameModel.Model
{

    public static class BossAIHelper
    {
        //Loop through a list of Moves and a List of Powers. 
        //Calculate the score for the next player.
        //Since the score is in perspective of next player, look for the lowest score.
        public static PlayerTurn GetBestBossTurn(GameState State, List<SimpleMove> Moves, List<IMove> Powers)
        {
            PlayerTurn BossTurn = null;
            int BestScore = -1;
            foreach (SimpleMove m in Moves)
                foreach(IMove p in Powers)
                {
                    PlayerTurn Turn = new PlayerTurn(m);
                    Turn.Moves.Add(p);
                    GameState GS = State.TakeTurn(Turn).GameState;
                    AITurnEvaluator AITE = new AITurnEvaluator(GS);
                    if (AITE.WinningTurns.Count > 0) continue;
                    int Score = AITE.TopScoreValue();
                    if (Score < BestScore || BossTurn == null)
                    {
                        BestScore = Score;
                        BossTurn = Turn;
                    }
                }
            return BossTurn;       
        }

        public static List<PlayerTurn> FindOkMove(GameState State, List<SimpleMove> Moves, List<IMove> Powers, int MaxTurns = 3)
        {
            List<PlayerTurn> OkTurns = new List<PlayerTurn>();
            foreach (SimpleMove m in Moves)
                foreach (IMove p in Powers)
                {
                    PlayerTurn Turn = new PlayerTurn(m);
                    Turn.Moves.Add(p);
                    GameState GS = State.TakeTurn(Turn).GameState;
                    AITurnEvaluator AITE = new AITurnEvaluator(GS);
                    if (AITE.Evaluator.GetFirstWinningMove() == null)
                    {
                        OkTurns.Add(Turn);
                        if (OkTurns.Count >= MaxTurns) return OkTurns;
                    }
                }
            return OkTurns;
        }
                             
        public static Player GetBoss(GameState State)
        {
            foreach(Player p in State.Players.Values)
            {
                if (p.Profile == AIProfile.BossAI) return p;
            }
            return null;
        }

    }
}