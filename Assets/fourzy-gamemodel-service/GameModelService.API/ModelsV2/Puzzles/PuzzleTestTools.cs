using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public static class PuzzleTestTools
    {
        public static PuzzleTestResults EvaluateState(GameState GSEval, int SearchDepth = 3)
        {
            PuzzleTestResults PR = new PuzzleTestResults(SearchDepth);

            Dictionary<int, List<GameState>> StateBucket = new Dictionary<int, List<GameState>>();
            Dictionary<string, PuzzleTurnTransition> TurnBucket = new Dictionary<string, PuzzleTurnTransition>();

            TurnEvaluator TE = new TurnEvaluator(GSEval);
            StateBucket[0] = new List<GameState>();
            StateBucket[0].Add(GSEval);

            for (int d = 1; d <= SearchDepth; d++)
            {
                PR.VictoryDepth = d;
                StateBucket[d] = new List<GameState>();

                int evaluated_states = 0;
                foreach (GameState GS in StateBucket[PR.VictoryDepth - 1])
                {

                    TE = new TurnEvaluator(GS);

                    foreach (SimpleMove m in TE.GetAvailableSimpleMoves())
                    {
                        GameState GSTest = TE.EvaluateTurn(m);
                        if (GSTest.WinnerId == GS.ActivePlayerId)
                        {
                            PR.NumberOfVictoryPaths++;
                            PR.VictoryStates.Add(GSTest);
                            if (PR.Solution.Count==0)
                            {
                                PuzzleTurnTransition PTT = null;
                                if (TurnBucket.ContainsKey(GS.UniqueId))
                                    PTT = TurnBucket[GS.UniqueId];

                                while (PTT != null)
                                {
                                    PR.Solution.Add(PTT.Turn);
                                    if (PTT.InitialState.UniqueId == GSEval.UniqueId) break;
                                    if (TurnBucket.ContainsKey(PTT.InitialState.UniqueId))
                                        PTT = TurnBucket[PTT.InitialState.UniqueId];
                                    else
                                        PTT = null;
                                }
                                PR.Solution.Add(new PlayerTurn(m));
                          

                            }
                        }
                        else
                        {
                            if (GSTest.WinnerId < 0 && PR.NumberOfVictoryPaths == 0 && PR.VictoryDepth <= SearchDepth)
                            {
                                AIPlayer AI = new PuzzleAI(GSTest);
                                PlayerTurn AITurn = AI.GetTurn();
                                TurnEvaluator AITE = new TurnEvaluator(GSTest);
                                GameState GSAI = AITE.EvaluateTurn(AITurn);
                                if (GSAI.WinnerId < 0)
                                {
                                    StateBucket[PR.VictoryDepth].Add(GSAI);
                                    if (!TurnBucket.ContainsKey(GSAI.UniqueId))
                                        TurnBucket.Add(GSAI.UniqueId, new PuzzleTurnTransition(GS, new PlayerTurn(m), GSAI));
                                }
                            }
                        }
                    }

                    evaluated_states++;
                }

                if (PR.NumberOfVictoryPaths > 0)
                {
                    //PR.Solution.Reverse();
                    break;
                }

            }

            return PR;
        }

        public static int PuzzleComplexity(GameBoard Board, int NumberMoves)
        {
            int score = BoardFactory.BoardComplexityScore(Board);
            score += Board.PieceCount * 50;
            score += NumberMoves * NumberMoves * 100;

            return score;
        }

        public static bool TestSolution(GameState State, List<PlayerTurn> Solution, int PlayerId = 1)
        {
            GameState GS = new GameState(State);
            if (GS.Players.Count == 0)
            {
                GS.Players.Add(1, new Player(1, "ONE"));
                GS.Players.Add(2, new Player(2, "TWO"));
            }

            GS.ActivePlayerId = PlayerId;
            for (int i = 0; i < Solution.Count; i++)
            {
                PlayerTurn t = Solution[i];
                //PlayerTurn t = Solution[Solution.Count -i -1];

                TurnEvaluator TE = new TurnEvaluator(GS);
                GS = TE.EvaluateTurn(t);

                //if wrong player or draw.
                if (GS.WinnerId > 0 && GS.WinnerId != PlayerId) return false;

                //Don't make AI move on last player turn.
                if (i == Solution.Count - 1) break;

                if (GS.WinnerId < 0)
                {
                    AIPlayer AI = AIPlayerFactory.Create(GS, AIProfile.PuzzleAI);
                    PlayerTurn Turn = AI.GetTurn();

                    TE = new TurnEvaluator(GS);
                    GS = TE.EvaluateTurn(Turn);

                    //if wrong player or draw.
                    if (GS.WinnerId > 0 && GS.WinnerId != PlayerId) return false;
                }

            }

            //If not winner, or Wrong player/draw
            if (GS.WinnerId < 0 || GS.WinnerId != PlayerId) return false;

            return true;
        }

    }

    public class PuzzleTestResults
    {
        public int VictoryDepth { get; set; }
        public int NumberOfVictoryPaths { get; set; }
        public int MaxSearchDepth { get; set; }
        public List<GameState> VictoryStates { get; set; }
        public List<PlayerTurn> Solution { get; set; }

        public PuzzleTestResults(int MaxSearchDepth =3)
        {
            VictoryDepth = -1;
            NumberOfVictoryPaths = 0;
            this.MaxSearchDepth = MaxSearchDepth;
            this.VictoryStates = new List<GameState>();
            this.Solution = new List<PlayerTurn>();
        }
    }

    public class PuzzleTurnTransition
    {
        public GameState InitialState { get; set; }
        public PlayerTurn Turn { get; set; }
        public GameState FinalState { get; set; }

        public PuzzleTurnTransition(GameState InitialState, PlayerTurn Turn, GameState FinalState)
        {
            this.InitialState = InitialState;
            this.Turn = Turn;
            this.FinalState = FinalState;
        }
    }




}
