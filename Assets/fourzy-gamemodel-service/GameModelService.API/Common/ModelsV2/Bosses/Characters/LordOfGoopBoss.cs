using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class LordOfGoopBoss : IBoss
    {
        public string Name { get { return "The Lord Of Goop"; } }
        public List<IBossPower> Powers { get; }
        public bool UseCustomAI { get { return true; } }
        public BossType Boss { get { return BossType.LordOfGoop; } }

        public LordOfGoopBoss()
        {
            this.Powers = new List<IBossPower>();
            this.Powers.Add(new GoopAPiecePower());
            //this.Powers.Add(new WallOfGoopPower());
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
            return true;
        }

        public PlayerTurn GetTurn(GameState State)
        {
            List<IMove> Powers = GetPossibleActivations(State);

          

            foreach (IMove p in Powers)
            {
                GoopAPiecePower goop = (GoopAPiecePower)p;
                List<List<BoardLocation>> Fours = State.Board.GetFoursFromLocation(goop.GoopLocation);
                bool goopit = false;
                foreach (List<BoardLocation> set_of_four in Fours)
                {
                    int count_player = 0;
                    int count_opponent = 0;
                    foreach (BoardLocation l in set_of_four)
                    {
                        if (State.Board.ContentsAt(l).Control == State.ActivePlayerId ) count_player++;
                        if (State.Board.ContentsAt(l).Control == State.Opponent(State.ActivePlayerId)) count_opponent++;
                    }
                    if (count_player >=2 || count_opponent == 3)
                    {
                        goopit = true;
                        break;
                    } 
                }

                if (!goopit) Powers.Remove(p);
            }

            //If No Powers, then just make a good move.
            if (Powers.Count == 0)
            {
                AggressiveAI AI = new AggressiveAI(State);
                return AI.GetTurn();
            }

            Dictionary<PlayerTurn, int> PossibleTurns = new Dictionary<PlayerTurn, int>();

            TurnEvaluator TE = new TurnEvaluator(State);
            SimpleMove Win = TE.GetFirstWinningMove();
            if (Win != null) return new PlayerTurn(Win);

            List<string> ProcessedStates = new List<string>();
            Dictionary<SimpleMove, GameState> MoveInfo = TE.GetAvailableMoveInfo();
            foreach (KeyValuePair<SimpleMove, GameState> move in MoveInfo)
            {
                if (ProcessedStates.Contains(move.Value.StateString)) continue;
                ProcessedStates.Add(move.Value.StateString);

                BoardLocation Start = TurnEvaluator.FirstLocation(move.Key, State.Board);

                PlayerTurn Turn = null;
                AITurnEvaluator AITE = null;
                foreach (IMove p in Powers)
                {
                    Turn = null;
                    if (p is GoopAPiecePower)
                    {
                        GoopAPiecePower Goop = (GoopAPiecePower)p;
                        if (Goop.GoopLocation.Row == Start.Row || Goop.GoopLocation.Column == Start.Column)
                        {
                            Turn = new PlayerTurn(move.Key, Goop);
                        }
                    }
                    else
                    {
                        Turn = new PlayerTurn(move.Key);
                    }

                    if (Turn != null)
                    {
                        AITE = new AITurnEvaluator(State, Turn);
                        int PowerScore = AITE.TopMoveScore().Item2;
                        PossibleTurns.Add(Turn, PowerScore);

                    }
                }

                Turn = new PlayerTurn(move.Key);
                AITE = new AITurnEvaluator(State, Turn);
                int Score = AITE.TopMoveScore().Item2;
                PossibleTurns.Add(Turn, Score);
            }

            PossibleTurns = PossibleTurns.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            return PossibleTurns.First().Key;
        }

        public bool TriggerPower(GameState State)
        {
            return true;
        }

        //Use for special conditions.

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
