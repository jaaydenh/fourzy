using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class EntryWayBoss : IBoss
    {
        public string Name { get { return "Entry Way Blocker"; } }
        public List<IBossPower> Powers { get; }
        const int MinStartingArrows = 6;   //too few arrows and boss is too easy
        const int MaxStartingArrows = 12;  //too many arrows and boss has too many combinations to think about.
        public bool UseCustomAI { get { return true; } }
        public bool BossGoesFirst { get { return false; } }

        public EntryWayBoss()
        {
            this.Powers = new List<IBossPower>();
            this.Powers.Add(new BlockMovePower());
            this.Powers.Add(new BlockASidePower());
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Activations = new List<IMove>();
            foreach (IBossPower p in Powers)
            {
                if (p.IsAvailable(State, IsDesparate))
                    Activations.AddRange(p.GetPossibleActivations(State, IsDesparate));
            }
            return Activations;
        }

        public PlayerTurn GetTurn(GameState State)
        {
            AIPlayer AI = new AggressiveAI(State);
            PlayerTurn Turn = AI.GetTurn();

            TurnEvaluator TE = new TurnEvaluator(State);
            GameState GS2 = TE.EvaluateTurn(Turn);

            List<BoardLocation> Blockers = GS2.Board.FindTokenLocations(TokenType.MOVE_BLOCKER);
            foreach (BoardLocation l in Blockers)
            {
                GS2.Board.ContentsAt(l).RemoveTokens(TokenType.MOVE_BLOCKER);
            }

            //AI = new AggressiveAI(GS2);
            //PlayerTurn StopThisTurn = AI.GetTurn();
            //SimpleMove m = (SimpleMove)StopThisTurn.Moves[0];
            //Turn.Moves.Add(new BlockMovePower(State, m));

            AITurnEvaluator AITE = new AITurnEvaluator(GS2);
            AITE.AIHeuristics.IsAggressive = true;
            
            List<SimpleMove> WhatWillPlayerDo = AITE.GetTopOkMoves(2);
            if (WhatWillPlayerDo != null)
                Turn.Moves.Add(new DoubleBlockPower(State, WhatWillPlayerDo));

            return Turn;
        }


        public bool StartGame(GameState State)
        {
            return true;
        }

        public bool TriggerPower(GameState State)
        {
            return true;
        }

        public bool TriggerBossWin(GameState State)
        {
            return false;
        }

        public bool TriggerBossLoss(GameState State)
        {
            return false;
        }

    }
}
