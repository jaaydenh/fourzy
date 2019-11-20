
using System;
using System.Collections.Generic;
using System.Linq;

namespace FourzyGameModel.Model
{
    //BAD AI PROFILE

    public class BadBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }
        private int InitialChanceToWin = 0;
        private int ChanceToWinPerPiece = 2;
        private int MaxChanceToWin = 75;
        private int MinChanceToWin = 10;
        
        private int InitialBlockChance = 100;
        private int ChanceToBlockPerPiece = -2;
        private int MinBlockChange = 25;
        private int NumberOfMovesToConsider = 3;

        public BadBotAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            AITurnEvaluator AI = new AITurnEvaluator(EvalState);
            if (AI.WinningTurns.Count > 0)
            {
                if (EvalState.Board.Random.Chance(Math.Min(MaxChanceToWin, Math.Max(MinChanceToWin,(EvalState.Board.PieceCount - 8)) * 2)))
                    return AI.WinningTurns.First();
            }

            SimpleMove Move = null;

            //Chance to block decreases as game goes on            
            if (EvalState.Board.Random.Chance(
                  Math.Min(MinBlockChange,InitialBlockChance - Math.Max(0,(EvalState.Board.PieceCount - 8)) * ChanceToBlockPerPiece)
                  ))
                Move = AI.GetOkBadMove(NumberOfMovesToConsider);
            else
                Move = AI.GetAnyBadMove(NumberOfMovesToConsider);



            //If for any reason, no good moves, get best lost cause move.
            if (Move == null) return new PlayerTurn(AI.GetBestLostCauseMove());

            return new PlayerTurn(Move);
        }
    }

}