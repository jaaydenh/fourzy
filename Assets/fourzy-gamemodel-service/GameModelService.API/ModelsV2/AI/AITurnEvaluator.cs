using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourzyGameModel.Model
{
    public class AITurnEvaluator
    {
        #region "Properties and Initialization"
        public GameState EvalState { get; set; }
        public List<PlayerTurn> WinningTurns { get; set; }
        public TurnEvaluator Evaluator { get; set; }
        public List<SimpleMove> AvailableSimpleMoves { get; set; }

        bool WinningMove
        {
            get
            {
                if (WinningTurns.Count > 0) return true;
                return false;
            }
        }

        public AITurnEvaluator(GameState State)
        {
            this.EvalState = State;
            this.Evaluator = new TurnEvaluator(this.EvalState);
            this.EvalState = Evaluator.EvaluateStartOfTurn();

            this.AvailableSimpleMoves = Evaluator.GetAvailableSimpleMoves();
            this.WinningTurns = new List<PlayerTurn>();
            foreach (SimpleMove m in Evaluator.GetWinningMoves())
            {
                this.WinningTurns.Add(new PlayerTurn(m));
            }
        }

        public AITurnEvaluator(GameState State, PlayerTurn Turn)
        {
            this.EvalState = State;
            this.Evaluator = new TurnEvaluator(this.EvalState);
            this.Evaluator.EvaluateTurn(Turn);
            this.EvalState = Evaluator.EvaluateStartOfTurn();

            this.AvailableSimpleMoves = Evaluator.GetAvailableSimpleMoves();
            this.WinningTurns = new List<PlayerTurn>();
            foreach (SimpleMove m in Evaluator.GetWinningMoves())
            {
                this.WinningTurns.Add(new PlayerTurn(m));
            }
        }

        #endregion

        #region "Random"
        //Get a completely random move.
        public PlayerTurn GetRandomTurn()
        {
            List<SimpleMove> Moves = AvailableSimpleMoves;
            if (Moves.Count == 0) return null;
            PlayerTurn Turn = new PlayerTurn(Moves[EvalState.Board.Random.RandomInteger(0, Moves.Count - 1)]);

            return Turn;
        }
        #endregion

        #region "OK Moves"
        public List<SimpleMove> GetOkMoves(IMove BossMove)
        {
            EvalState.TakeTurn(new PlayerTurn(EvalState.ActivePlayerId,BossMove));
            return GetOkMoves();
        }

        //Return Moves that:
        //Win
        //Don't lose 
        //
        public List<SimpleMove> GetOkMoves()
            {
            List<SimpleMove> Moves = new List<SimpleMove>();

            foreach (SimpleMove m in AvailableSimpleMoves)
            {
                TurnEvaluator OPP = new TurnEvaluator(Evaluator.EvaluateTurn(new PlayerTurn(m)));
                if (OPP.EvalState.WinnerId == m.Piece.PlayerId)
                {
                    Moves.Clear();
                    Moves.Add(m);
                    break;
                }
                if (OPP.EvalState.WinnerId == EvalState.Opponent(m.Piece.PlayerId)) continue;
                if (OPP.GetFirstWinningMove() == null) Moves.Add(m);
            }

            return Moves;
        }

        public Dictionary<SimpleMove, int> GetOkMovesWithOppScores()
        {
            Dictionary<SimpleMove, int> Moves = new Dictionary<SimpleMove, int>();

            foreach (SimpleMove m in AvailableSimpleMoves)
            {
                TurnEvaluator OPP = new TurnEvaluator(Evaluator.EvaluateTurn(new PlayerTurn(m)));
                if (OPP.EvalState.WinnerId == m.Piece.PlayerId)
                {
                    Moves.Clear();
                    Moves.Add(m,10000000);
                    break;
                }
                if (OPP.EvalState.WinnerId == EvalState.Opponent(m.Piece.PlayerId)) continue;


                if (OPP.GetFirstWinningMove() == null) {
                    AITurnEvaluator AI = new AITurnEvaluator(OPP.EvalState);

                    Tuple<SimpleMove, int> Top = AI.TopScore();
                    TurnEvaluator TE = new TurnEvaluator(OPP.EvalState);
                    GameState GS = TE.EvaluateTurn(new PlayerTurn(Top.Item1));
                    int Score = AITurnEvaluator.Score(GS, m.Piece.PlayerId);
                    Moves.Add(m, Score);
                }
            }

            return Moves;
        }

        public SimpleMove GetBestOkMovesWithLeastOppScore()
        {
            Dictionary<SimpleMove, int> Moves = new Dictionary<SimpleMove, int>();

            foreach (SimpleMove m in AvailableSimpleMoves)
            {
                TurnEvaluator OPP = new TurnEvaluator(Evaluator.EvaluateTurn(new PlayerTurn(m)));
                if (OPP.EvalState.WinnerId == m.Piece.PlayerId)
                {
                    Moves.Clear();
                    Moves.Add(m, 10000000);
                    break;
                }
                if (OPP.EvalState.WinnerId == EvalState.Opponent(m.Piece.PlayerId)) continue;

                
                if (OPP.GetFirstWinningMove() == null)
                {
                    OPP.Reset();
                    AITurnEvaluator AI = new AITurnEvaluator(OPP.EvalState);
                    Tuple<SimpleMove, int> Top = AI.TopScore();
                    TurnEvaluator TE = new TurnEvaluator(OPP.EvalState);
                    GameState GS = TE.EvaluateTurn(new PlayerTurn(Top.Item1));
                    int Score = AITurnEvaluator.Score(GS, m.Piece.PlayerId);
                    Moves.Add(m, Score );
                }
            }
            Moves = Moves.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            if (Moves.Count == 0) return null;
            return Moves.First().Key;
        }

        public SimpleMove GetBestLostCauseMove()
        {
            Dictionary<SimpleMove, int> WeightedMoves = new Dictionary<SimpleMove, int>();
            foreach (SimpleMove m in AvailableSimpleMoves)
            {
                TurnEvaluator TE = new TurnEvaluator(EvalState);
                GameState GS = TE.EvaluateTurn(new PlayerTurn(m));

                TurnEvaluator TE2 = new TurnEvaluator(GS);
                int WinMoves = TE2.GetWinningMoves(GS.Opponent(m.Piece.PlayerId)).Count;
                WeightedMoves.Add(m, WinMoves);
            }
            WeightedMoves = WeightedMoves.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            return WeightedMoves.Keys.First();
        }

        public SimpleMove GetFirstOkMove()
        {

            foreach (SimpleMove m in AvailableSimpleMoves)
            {
                TurnEvaluator OPP = new TurnEvaluator(Evaluator.EvaluateTurn(new PlayerTurn(m)));
                if (OPP.GetFirstWinningMove() == null) return m;
            }

            return null;
        }

        public List<SimpleMove> GetTopOkMoves(int NumberMoves = 5)
        {
            List<SimpleMove> Moves = new List<SimpleMove>();

            Dictionary<SimpleMove, int> WeightedOkMoves = ScoreMoves(EvalState, GetOkMoves());
            if (WeightedOkMoves.Count == 0) return null;
            WeightedOkMoves = WeightedOkMoves.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            foreach (KeyValuePair<SimpleMove, int> m in WeightedOkMoves.Take(NumberMoves))
            {
                Moves.Add(m.Key);
            }

            return Moves;
        }

        public Tuple<SimpleMove, int> TopScore()
        {
            Dictionary<SimpleMove, int> WeightedOkMoves = ScoreMoves(EvalState);
            if (WeightedOkMoves.Count == 0) return new Tuple<SimpleMove, int>(null, 0);

            WeightedOkMoves = WeightedOkMoves.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            return new Tuple<SimpleMove, int>(WeightedOkMoves.First().Key, WeightedOkMoves.First().Value);
        }


        public SimpleMove GetRandomOkMove()
        {
            List<SimpleMove> Moves = GetTopOkMoves(2);

            if (Moves == null || Moves.Count == 0) return null;
            return Moves[EvalState.Board.Random.RandomInteger(0, Moves.Count - 1)];
        }
        #endregion

        #region "Better Move"
        //Consider a set of moves
        //Look at each move.
        //  Check all the possible my opponent can make
        //  if AI has no good moves, then make this move
        //  otherwise, look at each opponentmove
        //      score them
        //      take a look a the top x scoring moves
        //      record the lowest of my scores
        //      if I cannot make an ok move, remove this move from consideration
        //  after I find x suitable moves, stop and return

        public Dictionary<SimpleMove, int> DeeperReview(List<SimpleMove> OkMoves, int MovesToReturn = 3, int MovesToConsider = 12, int OppMovesToConsider = 12)
        {
            Dictionary<SimpleMove, int> BetterMoves = new Dictionary<SimpleMove, int>();
            Dictionary<SimpleMove, int> WeightedOkMoves = ScoreMoves(EvalState, OkMoves, true, MovesToConsider);


            foreach (KeyValuePair<SimpleMove, int> m in WeightedOkMoves)
            {
                AITurnEvaluator AIEval = new AITurnEvaluator(Evaluator.EvaluateTurn(new PlayerTurn(m.Key)));
                List<SimpleMove> AIMoves = AIEval.GetOkMoves();
                if (AIMoves.Count == 0)
                {
                    //AI has no good moves, this is great!
                    //Return and make this move.
                    BetterMoves.Clear();
                    BetterMoves.Add(m.Key, m.Value);
                    break;
                }
                else
                {
                    Dictionary<SimpleMove, int> WeightedAIMoves = ScoreMoves(EvalState, OkMoves, true, OppMovesToConsider);
                    int ok_move_count = 0;
                    int mybestscore = 0;
                    bool scored = false;
                    foreach (SimpleMove a in WeightedAIMoves.Select(x => x.Key).Take(OppMovesToConsider))
                    {
                        AITurnEvaluator PE = new AITurnEvaluator(AIEval.Evaluator.EvaluateTurn(new PlayerTurn(a)));
                        if (PE.Evaluator.GetFirstWinningMove() != null) { ok_move_count++; continue; }
                        if (PE.GetFirstOkMove() != null) ok_move_count++;
                        int myscore = AITurnEvaluator.Score(PE.EvalState, m.Key.Piece.PlayerId);
                        if (!scored) mybestscore = myscore;
                        else if (myscore < mybestscore) mybestscore = myscore;
                    }

                    if (ok_move_count == WeightedAIMoves.Count)
                    {
                        if (scored)
                            BetterMoves.Add(m.Key, mybestscore);
                        else BetterMoves.Add(m.Key, m.Value);
                    }
                }
                if (BetterMoves.Count > MovesToReturn) break;
            }
            return BetterMoves;
        }

        public Dictionary<SimpleMove, int> GetBetterMoves(int MovesToReturn = 2, int MovesToConsider = 24, int OppMovesToConsider = 12)
        {
            Dictionary<SimpleMove, int> BetterMoves = new Dictionary<SimpleMove, int>();

            List<SimpleMove> OkMoves = GetOkMoves();
            Dictionary<SimpleMove, int> WeightedOkMoves = ScoreMoves(EvalState, OkMoves, true, MovesToConsider);
   
            foreach (KeyValuePair<SimpleMove, int> m in WeightedOkMoves)
            {
                AITurnEvaluator AIEval = new AITurnEvaluator(Evaluator.EvaluateTurn(new PlayerTurn(m.Key)));
                List<SimpleMove> AIMoves = AIEval.GetOkMoves();
                if (AIMoves.Count == 0) BetterMoves.Add(m.Key, m.Value);
                else
                {
                    Dictionary<SimpleMove, int> WeightedAIMoves = ScoreMoves(EvalState, OkMoves);
                    int ok_move_count = 0;
                    WeightedAIMoves = WeightedAIMoves.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

                    foreach (SimpleMove a in WeightedAIMoves.Select(x => x.Key).Take(OppMovesToConsider))
                    {
                        AITurnEvaluator PE = new AITurnEvaluator(AIEval.Evaluator.EvaluateTurn(new PlayerTurn(a)));
                        if (PE.Evaluator.GetFirstWinningMove() != null) { ok_move_count++; continue; }
                        if (PE.GetFirstOkMove() != null) ok_move_count++;
                    }

                    if (ok_move_count == WeightedAIMoves.Count) BetterMoves.Add(m.Key, m.Value);
                }
                if (BetterMoves.Count > MovesToReturn) break;
            }
            return BetterMoves;
        }
        
        public SimpleMove GetRandomBetterMove(int MovesToDiveInto = 6, int MovesToConsider = 24, int OppMovesToConsider = 12)
        {
            List<SimpleMove> OkMoves = GetOkMoves();
            Dictionary<SimpleMove, int> Moves = DeeperReview(OkMoves, MovesToDiveInto, MovesToConsider, OppMovesToConsider);
            if (Moves == null || Moves.Count == 0) return null;
            int TotalScore = 0;
            foreach (SimpleMove m in Moves.Keys)
                TotalScore += Moves[m];

            int RandomNum = EvalState.Board.Random.RandomInteger(0, TotalScore);

            foreach (KeyValuePair<SimpleMove, int> m in Moves)
            {
                RandomNum -= m.Value;
                if (RandomNum <= 0) return m.Key;
            }
            return null;
        }
        #endregion

        #region "Score Positions"

        public List<SimpleMove> TopScoringMoves(List<SimpleMove> MovesToConsider, int MovesToReturn = 3)
        {
            List<SimpleMove> Moves = new List<SimpleMove>();

            Dictionary<SimpleMove, int> WeightedOkMoves = ScoreMoves(EvalState, MovesToConsider);
            if (WeightedOkMoves.Count == 0) return null;
            WeightedOkMoves = WeightedOkMoves.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            foreach (SimpleMove m in WeightedOkMoves.Select(x => x.Key))
            {
                Moves.Add(m);
                if (Moves.Count == MovesToReturn) break;
            }

            return Moves;
        }

        public SimpleMove GetTopMoveNoRestriction()
        {
            List<SimpleMove> MovesToConsider = Evaluator.GetAvailableSimpleMoves();
            Dictionary<SimpleMove, int> WeightedMoves = ScoreMoves(EvalState, MovesToConsider);
            WeightedMoves = WeightedMoves.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            return WeightedMoves.Keys.First();
        }

        public static Dictionary<SimpleMove, int> ScoreMoves(GameState State)
        { 
            TurnEvaluator TE = new TurnEvaluator(State);
            List<SimpleMove> Moves = TE.GetAvailableSimpleMoves();
            return ScoreMoves(State, Moves);
        }

        public static Dictionary<SimpleMove, int> ScoreMoves(GameState State, List<SimpleMove> Moves, bool Sort = true, int TopMoves = -1)
        {
            Dictionary<SimpleMove, int> ScoredMoves = new Dictionary<SimpleMove, int>();
            TurnEvaluator ME = new TurnEvaluator(State);
            foreach (SimpleMove m in Moves)
            {
                GameState GS = ME.EvaluateTurn(new PlayerTurn(m));
                //                int Score = AITurnEvaluator.Score(GS, m.Piece.PlayerId) - AITurnEvaluator.Score(GS, GS.Opponent(m.Piece.PlayerId));
                int Score = AITurnEvaluator.Score(GS, m.Piece.PlayerId);

                ScoredMoves.Add(m, Score);
            }
            if (Sort) ScoredMoves = ScoredMoves.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            if (TopMoves > 0) ScoredMoves = ScoredMoves.Take(TopMoves).ToDictionary(x => x.Key, x => x.Value);

            return ScoredMoves;
        }

        public static int Score(GameState State, PlayerTurn Turn)
        {
            int Score = 0;
            int PlayerId = Turn.PlayerId;

            TurnEvaluator TE = new TurnEvaluator(State);
            TE.EvaluateTurn(Turn);
            List<BoardLocation> Dead = TE.FindDeadLocations();

            Score += ScoreFours(State, PlayerId, Dead) * 10;
            Score -= ScoreFours(State, State.Opponent(PlayerId), Dead) * 10;
            Score += ScoreFives(State, PlayerId, Dead) * 100;
            Score -= ScoreFives(State, State.Opponent(PlayerId), Dead) * 100;
            Score += ScorePositions(State, PlayerId);
            Score -= ScorePositions(State, State.Opponent(PlayerId));

            return Score;
        }

        public static int Score(GameState State, int PlayerId)
        {
            int Score = 0;

            TurnEvaluator TE = new TurnEvaluator(State);
            List<BoardLocation> Dead = TE.FindDeadLocations();

            Score += ScoreFours(State, PlayerId, Dead) * 10;
            Score -= ScoreFours(State, State.Opponent(PlayerId), Dead) * 10;
            Score += ScoreFives(State, PlayerId, Dead) * 25;
            Score -= ScoreFives(State, State.Opponent(PlayerId), Dead) * 25;
            Score += ScorePositions(State, PlayerId);
            Score -= ScorePositions(State, State.Opponent(PlayerId));

            return Score;
        }

        public static string ScoreExplanation(GameState State, int PlayerId)
        {
            int Score = 0;
            int OpponentId = State.Opponent(PlayerId);

            StringBuilder output = new StringBuilder();

            TurnEvaluator TE = new TurnEvaluator(State);
            List<BoardLocation> Dead = TE.FindDeadLocations();

            output.AppendLine("Scoring for Player=" + PlayerId);
            output.AppendLine("  Opponent=" + OpponentId);
            output.AppendLine("Dead Space Count=" + Dead.Count);

            int MyFour = ScoreFours(State, PlayerId, Dead) * 10;
            int TheirFour = ScoreFours(State, OpponentId, Dead) * 10;
            int MyFive = ScoreFives(State, PlayerId, Dead) * 25;
            int TheirFive = ScoreFives(State, OpponentId, Dead) * 25;
            int MyPosition = ScorePositions(State, PlayerId);
            int TheirPosition = ScorePositions(State, OpponentId);
            Score = MyFive - TheirFive + MyFour - TheirFour + MyPosition - TheirPosition;

            output.AppendLine("Score=" + Score);
            output.AppendLine("MyFour=" + MyFour);
            output.AppendLine("TheirFour=" + TheirFour);
            output.AppendLine("MyFive=" + MyFive);
            output.AppendLine("TheirFive=" + TheirFive);
            output.AppendLine("MyPos=" + MyPosition);
            output.AppendLine("TheirPos=" + TheirPosition);

            return output.ToString();
        }


        public static int CountTheDead(GameState State)
        {
            TurnEvaluator TE = new TurnEvaluator(State);
            return TE.FindDeadLocations().Count;
        }

        public static int CountTheDead(GameBoard Board)
        {
            TurnEvaluator TE = new TurnEvaluator(new GameState(Board, new GameOptions()));
            return TE.FindDeadLocations().Count;
        }

        public static List<BoardLocation> FindDeadLocations(GameState State)
        {
            TurnEvaluator TE = new TurnEvaluator(State);
            return TE.FindDeadLocations();
        }

        public static List<BoardLocation> FindDeadLocations(GameBoard Board)
        {
            TurnEvaluator TE = new TurnEvaluator(new GameState(Board,new GameOptions()) );
            return TE.FindDeadLocations();
        }

        public static int ScorePositions(GameState State, int PlayerId)
        {
            int Score = 0;
            foreach (KeyValuePair<BoardLocation, Piece> p in State.Board.Pieces)
                if (p.Value.PlayerId == PlayerId)
                    Score += ScorePosition(p.Key, State.Board);
            return Score;
        }

        public static int ScorePosition(BoardLocation Location, GameBoard Board)
        {
            int Score = 0;
            if (Location.Row <= Board.Rows / 2) Score += Location.Row;
            else Score += (Board.Rows - Location.Row);

            if (Location.Column <= Board.Columns / 2) Score += Location.Column;
            else Score += (Board.Columns - Location.Column);

            return Score;
        }

        public static int ScoreFours(GameState State, int PlayerId)
        {
            List<BoardLocation> Dead = new List<BoardLocation>();
            return ScoreFours(State, PlayerId, Dead); 
        }

        public static int ScoreFours(GameState State, int PlayerId, List<BoardLocation> DeadLocations)
        {
            int Score = 0;
            for (var row = 0; row < State.Board.Rows; row++)
            {
                for (var col = 0; col < State.Board.Columns; col++)
                {
                    foreach (WinDirection Direction in Enum.GetValues(typeof(WinDirection)))
                    {
                        switch (Direction)
                        {
                            case WinDirection.HORIZONTAL:
                                if (State.Board.Columns - col < 4) continue; break;
                            case WinDirection.VERTICAL:
                                if (State.Board.Rows - row < 4) continue; break;
                            case WinDirection.DIAGONAL_NE_SW:
                                if (State.Board.Columns - col < 4) continue;
                                if (row < State.Board.Rows - 4 - 1) continue; break;
                            case WinDirection.DIAGONAL_NW_SE:
                                if (State.Board.Columns - col < 4) continue;
                                if (State.Board.Rows - row < 4) continue; break;

                        }

                        Score += ScoreFour(State, new BoardLocation(row, col), Direction, PlayerId, DeadLocations);
                    }
                }
            }
            return Score;
        }

        public static int ScoreFives(GameState State, int PlayerId, List<BoardLocation> DeadLocations)
        {
            int Score = 0;
            for (var row = 0; row < State.Board.Rows; row++)
            {
                for (var col = 0; col < State.Board.Columns; col++)
                {
                    foreach (WinDirection Direction in Enum.GetValues(typeof(WinDirection)))
                    {
                        switch (Direction)
                        {
                            case WinDirection.HORIZONTAL:
                                if (State.Board.Columns - col < 5) continue; break;
                            case WinDirection.VERTICAL:
                                if (State.Board.Rows - row < 5) continue; break;

                            case WinDirection.DIAGONAL_NW_SE:
                            case WinDirection.DIAGONAL_NE_SW:
                                continue; 
                        }
                        Score += ScoreFive(State, new BoardLocation(row, col), Direction, PlayerId, DeadLocations);
                    }
                }
            }
            return Score;
        }

        public static string ReviewFives(GameState State, int PlayerId, List<BoardLocation> DeadLocations)
        {
            int Score = 0;
            StringBuilder output = new StringBuilder();
            for (var row = 0; row < State.Board.Rows; row++)
            {
                for (var col = 0; col < State.Board.Columns; col++)
                {
                    foreach (WinDirection Direction in Enum.GetValues(typeof(WinDirection)))
                    {
                        switch (Direction)
                        {
                            case WinDirection.HORIZONTAL:
                                if (State.Board.Columns - col < 5) continue; break;
                            case WinDirection.VERTICAL:
                                if (State.Board.Rows - row < 5) continue; break;
                        }
                        Score += ScoreFive(State, new BoardLocation(row, col), Direction, PlayerId, DeadLocations);
                        output.AppendLine(row + "," + col + Direction.ToString() + ':' + Score);
                    }
                }
            }
            return output.ToString();       
        }


        public static int ScoreFour(GameState State, BoardLocation Location, WinDirection WinDirection, int PlayerId, List<BoardLocation> DeadLocations)
        {
            int count = 0;
            int i = 0;
            bool contiguous = true;
            BoardLocation loc;
            switch (WinDirection)
            {
                case WinDirection.HORIZONTAL:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(State.Board) && i<4)
                    {
                        if (!State.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadLocations.Contains(loc))
                        {
                            return 0;
                        }
                        else if (State.Board.ContentsAt(loc).ContainsPiece
                            && !State.Board.ContentsAt(loc).TokensAllowPushing
                            && State.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (State.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                            contiguous = false;

                        i++;
                        loc = new BoardLocation(Location.Row, Location.Column + i);
                    }
                    break;

                case WinDirection.VERTICAL:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(State.Board) && i < 4)
                    {

                        if (!State.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadLocations.Contains(loc))
                        {
                            return 0;
                        }
                        else if (State.Board.ContentsAt(loc).ContainsPiece
                            && !State.Board.ContentsAt(loc).TokensAllowPushing
                            && State.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (State.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                            contiguous = false;

                        i++;
                        loc = new BoardLocation(Location.Row + i, Location.Column);

                    }

                    break;
                case WinDirection.DIAGONAL_NE_SW:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(State.Board) && i < 4)
                    {

                        if (!State.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadLocations.Contains(loc))
                        {
                            return 0;
                        }
                        else if (State.Board.ContentsAt(loc).ContainsPiece
                            && !State.Board.ContentsAt(loc).TokensAllowPushing
                            && State.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (State.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                            contiguous = false;

                        i++;
                        loc = new BoardLocation(Location.Row - i, Location.Column + i);

                    }
                    break;


                case WinDirection.DIAGONAL_NW_SE:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(State.Board) && i < 4)
                    {

                        if (!State.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadLocations.Contains(loc))
                        {
                            return 0;
                        }
                        else if (State.Board.ContentsAt(loc).ContainsPiece
                            && !State.Board.ContentsAt(loc).TokensAllowPushing
                            && State.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (State.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                            contiguous = false;

                        i++;
                        loc = new BoardLocation(Location.Row + i, Location.Column + i);

                    }
                    break;

            }

            int score = 0;
            switch (count) {
                case 0:
                    score = 1;
                    break;
                case 1:
                    score = 2;
                    break;
                case 2:
                    score = 9;
                    if (contiguous) score += 4;
                    break;
                case 3:
                    score = 20;
                    if (contiguous) score += 20;
                    break;
            }
      

            return score;
        }

        public static int ScoreFive(GameState State, BoardLocation Location, WinDirection WinDirection, int PlayerId, List<BoardLocation> DeadLocations)
        {
            int multiplier = 0;
            int count = 0;
            int i = 0;
            bool contiguous = true;
            BoardLocation loc;
            switch (WinDirection)
            {
                case WinDirection.HORIZONTAL:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(State.Board) && i < 5)
                    {
                        if (State.Board.ContentsAt(loc).Control == PlayerId && (i == 0 || i == 4)) return 0;

                        if (!State.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }

                        if (DeadLocations.Contains(loc))
                        {
                            return 0;
                        }

                        else if (State.Board.ContentsAt(loc).ContainsPiece
                            && !State.Board.ContentsAt(loc).TokensAllowPushing
                            && State.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (State.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                            contiguous = false;

                        i++;
                        loc = new BoardLocation(Location.Row, Location.Column + i);
                    }
                    multiplier = 2;

                    break;

                case WinDirection.VERTICAL:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(State.Board) && i < 5)
                    {
                        if (State.Board.ContentsAt(loc).Control == PlayerId && (i == 0 || i == 4)) return 0;

                        if (!State.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadLocations.Contains(loc))
                        {
                            return 0;
                        }
                        else if (State.Board.ContentsAt(loc).ContainsPiece
                            && !State.Board.ContentsAt(loc).TokensAllowPushing
                            && State.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (State.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                            contiguous = false;

                        i++;
                        loc = new BoardLocation(Location.Row + i, Location.Column);

                    }
                    multiplier = 2;

                    break;

                case WinDirection.DIAGONAL_NE_SW:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(State.Board) && i < 5)
                    {
                        if (State.Board.ContentsAt(loc).Control == PlayerId && (i == 0 || i == 4)) return 0;

                        if (!State.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadLocations.Contains(loc))
                        {
                            return 0;
                        }
                        else if (State.Board.ContentsAt(loc).ContainsPiece
                            && !State.Board.ContentsAt(loc).TokensAllowPushing
                            && State.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (State.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                            contiguous = false;

                        i++;
                        loc = new BoardLocation(Location.Row + i, Location.Column - i);

                    }
                    multiplier = 0;

                    break;


                case WinDirection.DIAGONAL_NW_SE:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(State.Board) && i < 5)
                    {
                        if (State.Board.ContentsAt(loc).Control == PlayerId && (i == 0 || i == 4)) return 0;

                        if (!State.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadLocations.Contains(loc))
                        {
                            return 0;
                        }
                        else if (State.Board.ContentsAt(loc).ContainsPiece
                            && !State.Board.ContentsAt(loc).TokensAllowPushing
                            && State.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (State.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                            contiguous = false;

                        i++;
                        loc = new BoardLocation(Location.Row + i, Location.Column + i);

                    }
                    multiplier = 0;

                    break;

            }

            int score = 0;
            switch (count)
            {
                case 0:
                    score = 1;
                    break;
                case 1:
                    score = 2;
                    break;
                case 2:
                    score = 50;
                    if (contiguous) score += 50;
                    break;
                case 3:
                    score = 200;
                    break;
            }
            score = score * multiplier;

            return score;
        }
               
        public static bool DeadSpace(GameState State, BoardLocation Location, int PlayerId, List<BoardLocation> DeadLocations)
        {
            if (!Location.OnBoard(State.Board))
            {
                return true;
            }
            if (!State.Board.ContentsAt(Location).TokensAllowEndHere)
            {
                return true;
            }
            if (DeadLocations.Contains(Location))
            {
                return true;
            }
            else if (State.Board.ContentsAt(Location).ContainsPiece
                && !State.Board.ContentsAt(Location).TokensAllowPushing
                && State.Board.ContentsAt(Location).Control != PlayerId)
            {
                return true;
            }

            return false;
        }

        public static Dictionary<int,int> CountFours(GameState State, int PlayerId, List<BoardLocation> DeadLocations)
        {
            Dictionary<int, int> Results = new Dictionary<int, int>();
            Results.Add(0, 0);
            Results.Add(1, 0);
            Results.Add(2, 0);
            Results.Add(3, 0);

            for (var row = 0; row < State.Board.Rows; row++)
            {
                for (var col = 0; col < State.Board.Columns; col++)
                {
                    foreach (WinDirection Direction in Enum.GetValues(typeof(WinDirection)))
                    {
                        switch (Direction)
                        {
                            case WinDirection.HORIZONTAL:
                                if (State.Board.Columns - col < 4) continue; break;
                            case WinDirection.VERTICAL:
                                if (State.Board.Rows - row < 4) continue; break;
                            case WinDirection.DIAGONAL_NE_SW:
                                if (State.Board.Columns - col < 4) continue;
                                if (row < State.Board.Rows - 4 - 1) continue; break;
                            case WinDirection.DIAGONAL_NW_SE:
                                if (State.Board.Columns - col < 4) continue;
                                if (State.Board.Rows - row < 4) continue; break;

                        }

                        CountFour(State, new BoardLocation(row, col), Direction, PlayerId, DeadLocations, Results);
                    }
                }
            }
            return Results;
        }

        public static int CountFour(GameState State, BoardLocation Location, WinDirection WinDirection, int PlayerId, List<BoardLocation> DeadLocations, Dictionary<int,int> Results)
        {
            int count = 0;
            int i = 0;
            bool contiguous = true;
            BoardLocation loc;
            switch (WinDirection)
            {
                case WinDirection.HORIZONTAL:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(State.Board) && i < 4)
                    {
                        if (!State.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadLocations.Contains(loc))
                        {
                            return 0;
                        }
                        else if (State.Board.ContentsAt(loc).ContainsPiece
                            && !State.Board.ContentsAt(loc).TokensAllowPushing
                            && State.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (State.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                            contiguous = false;

                        i++;
                        loc = new BoardLocation(Location.Row, Location.Column + i);
                    }
                    break;

                case WinDirection.VERTICAL:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(State.Board) && i < 4)
                    {

                        if (!State.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadLocations.Contains(loc))
                        {
                            return 0;
                        }
                        else if (State.Board.ContentsAt(loc).ContainsPiece
                            && !State.Board.ContentsAt(loc).TokensAllowPushing
                            && State.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (State.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                            contiguous = false;

                        i++;
                        loc = new BoardLocation(Location.Row + i, Location.Column);

                    }

                    break;
                case WinDirection.DIAGONAL_NE_SW:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(State.Board) && i < 4)
                    {

                        if (!State.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadLocations.Contains(loc))
                        {
                            return 0;
                        }
                        else if (State.Board.ContentsAt(loc).ContainsPiece
                            && !State.Board.ContentsAt(loc).TokensAllowPushing
                            && State.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (State.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                            contiguous = false;

                        i++;
                        loc = new BoardLocation(Location.Row - i, Location.Column - i);

                    }
                    break;


                case WinDirection.DIAGONAL_NW_SE:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(State.Board) && i < 4)
                    {

                        if (!State.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadLocations.Contains(loc))
                        {
                            return 0;
                        }
                        else if (State.Board.ContentsAt(loc).ContainsPiece
                            && !State.Board.ContentsAt(loc).TokensAllowPushing
                            && State.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (State.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                            contiguous = false;

                        i++;
                        loc = new BoardLocation(Location.Row + i, Location.Column + i);

                    }
                    break;

            }

            int score = 0;
            switch (count)
            {
                case 0:
                    Results[0]++;
                    break;
                case 1:
                    Results[1]++;
                    break;
                case 2:
                    Results[2]++;
                    break;
                case 3:
                    Results[3]++;
                    break;
            }


            return score;
        }

        public static Dictionary<int,int> CountFives(GameState State, int PlayerId, List<BoardLocation> DeadLocations)
        {
            Dictionary<int, int> Results = new Dictionary<int, int>();
            Results.Add(0, 0);
            Results.Add(1, 0);
            Results.Add(2, 0);
            Results.Add(3, 0);

            for (var row = 0; row < State.Board.Rows; row++)
            {
                for (var col = 0; col < State.Board.Columns; col++)
                {
                    foreach (WinDirection Direction in Enum.GetValues(typeof(WinDirection)))
                    {
                        switch (Direction)
                        {
                            case WinDirection.HORIZONTAL:
                                if (State.Board.Columns - col < 5) continue; break;
                            case WinDirection.VERTICAL:
                                if (State.Board.Rows - row < 5) continue; break;
                            case WinDirection.DIAGONAL_NE_SW:
                                if (State.Board.Columns - col < 5) continue;
                                if (row < State.Board.Rows - 5 -1) continue; break;
                            case WinDirection.DIAGONAL_NW_SE:
                                if (State.Board.Columns - col < 5) continue;
                                if (State.Board.Rows - row < 5) continue; break;

                        }
                        CountFive(State, new BoardLocation(row, col), Direction, PlayerId, DeadLocations, Results);
                    }
                }
            }
            return Results;
        }
        public static int CountFive(GameState State, BoardLocation Location, WinDirection WinDirection, int PlayerId, List<BoardLocation> DeadLocations, Dictionary<int, int> Results)
        {
            int count = 0;
            int i = 0;
            bool contiguous = true;
            BoardLocation loc;
            switch (WinDirection)
            {
                case WinDirection.HORIZONTAL:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(State.Board) && i < 5)
                    {
                        if (State.Board.ContentsAt(loc).Control == PlayerId && (i == 0 || i == 4)) return 0;

                        if (!State.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }

                        if (DeadLocations.Contains(loc))
                        {
                            return 0;
                        }

                        else if (State.Board.ContentsAt(loc).ContainsPiece
                            && !State.Board.ContentsAt(loc).TokensAllowPushing
                            && State.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (State.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                            contiguous = false;

                        i++;
                        loc = new BoardLocation(Location.Row, Location.Column + i);
                    }
                    break;

                case WinDirection.VERTICAL:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(State.Board) && i < 5)
                    {
                        if (State.Board.ContentsAt(loc).Control == PlayerId && (i == 0 || i == 4)) return 0;

                        if (!State.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadLocations.Contains(loc))
                        {
                            return 0;
                        }
                        else if (State.Board.ContentsAt(loc).ContainsPiece
                            && !State.Board.ContentsAt(loc).TokensAllowPushing
                            && State.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (State.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                            contiguous = false;

                        i++;
                        loc = new BoardLocation(Location.Row + i, Location.Column);

                    }

                    break;
                case WinDirection.DIAGONAL_NE_SW:
                    return 0;
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(State.Board) && i < 5)
                    {
                        if (State.Board.ContentsAt(loc).Control == PlayerId && (i == 0 || i == 4)) return 0;

                        if (!State.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadLocations.Contains(loc))
                        {
                            return 0;
                        }
                        else if (State.Board.ContentsAt(loc).ContainsPiece
                            && !State.Board.ContentsAt(loc).TokensAllowPushing
                            && State.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (State.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                            contiguous = false;

                        i++;
                        loc = new BoardLocation(Location.Row - i, Location.Column + i);

                    }
                    break;


                case WinDirection.DIAGONAL_NW_SE:
                    return 0;
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(State.Board) && i < 5)
                    {
                        if (State.Board.ContentsAt(loc).Control == PlayerId && (i == 0 || i == 4)) return 0;

                        if (!State.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadLocations.Contains(loc))
                        {
                            return 0;
                        }
                        else if (State.Board.ContentsAt(loc).ContainsPiece
                            && !State.Board.ContentsAt(loc).TokensAllowPushing
                            && State.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (State.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                            contiguous = false;

                        i++;
                        loc = new BoardLocation(Location.Row + i, Location.Column + i);

                    }
                    break;

            }

            int score = 0;
            switch (count)
            {
                case 0:
                    Results[0]++;
                    break;
                case 1:
                    Results[1]++;
                    break;
                case 2:
                    Results[2]++;
                    break;
                case 3:
                    Results[3]++;
                    break;
            }

            return score;
        }


        #endregion
    }
}
