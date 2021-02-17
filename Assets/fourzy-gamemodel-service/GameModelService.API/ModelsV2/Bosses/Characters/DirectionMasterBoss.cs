using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class DirectionMasterBoss : IBoss
    {
        public string Name { get { return "The Direction Master"; } } 
        public List<IBossPower> Powers { get; }
        const int MinStartingArrows = 6;   //too few arrows and boss is too easy
        const int MaxStartingArrows = 12;  //too many arrows and boss has too many combinations to think about.
        public bool UseCustomAI { get { return true; } }
        public bool BossGoesFirst { get { return false; } }

        public DirectionMasterBoss()
        {
            this.Powers = new List<IBossPower>();
            this.Powers.Add(new ArrowCharmPower());
            this.Powers.Add(new GlobalArrowChangePower());
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

        public bool StartGame(GameState State)
        {
            List<IToken> Arrows = State.Board.FindTokens(TokenType.ARROW);

            if (Arrows.Count > MaxStartingArrows)
            {
                int Over = Arrows.Count - MaxStartingArrows;
                for (int i = 0; i < Over; i++)
                    Arrows.Remove(Arrows[0]);
            }

            if (State.Board.FindTokens(TokenType.ARROW).Count < MinStartingArrows)
            {
                int count = 100;
                while (count-->0 && State.Board.FindTokens(TokenType.ARROW).Count < MinStartingArrows)
                {
                    BoardLocation Target = State.Board.Random.RandomLocation(new BoardLocation(1, 1), State.Board.Rows - 2, State.Board.Columns - 2);
                    if (State.Board.ContentsAt(Target).ContainsOnlyTerrain)
                        State.Board.AddToken(new ArrowToken(State.Board.Random.RandomDirection()), Target);
                }
            }
            return true;
        }

        public PlayerTurn GetTurn(GameState State)
        {
            Dictionary<PlayerTurn, int> PossibleTurns = new Dictionary<PlayerTurn, int>();
            List<IMove> Powers = GetPossibleActivations(State);
            TurnEvaluator TE = new TurnEvaluator(State);
            List<string> States = new List<string>();

            foreach (SimpleMove m in TE.GetAvailableSimpleMoves())
            {
                List<BoardLocation> trace = TE.TraceMove(m);
                GameState GSNew = null;
                foreach (IMove p in Powers)
                {
                    PlayerTurn t = new PlayerTurn(m);

                    IBossPower power = (IBossPower)p;
                    if (power.PowerType == BossPowerType.ArrowCharm)
                    {
                        ArrowCharmPower charm = (ArrowCharmPower)power;
                        if (trace.Contains(charm.ArrowLocation) )
                        {
                            t.Moves.Insert(0, charm);
                            GSNew = TE.EvaluateTurn(t);

                            if (GSNew.WinnerId == State.ActivePlayerId || State.WinnerId==0) return t;
                            if (GSNew.WinnerId == State.Opponent(State.ActivePlayerId)) continue;
                        }
                    }
                }

                GSNew = TE.EvaluateTurn(m);
                if (!States.Contains(GSNew.StateString))
                    PossibleTurns.Add(new PlayerTurn(m), 0);
            }

            foreach (PlayerTurn t in PossibleTurns.Keys)
            {
                GameState ResultState = TE.EvaluateTurn(t);
                AIScoreEvaluator AI = new AIScoreEvaluator(ResultState);
                PossibleTurns[t] = AI.Score(State.ActivePlayerId);
            }

            PossibleTurns = PossibleTurns.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                        
            return PossibleTurns.Keys.First(); 
        }

        public bool TriggerPower(GameState State)
        {
            //loop through powers and if any trigger, return true;
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
