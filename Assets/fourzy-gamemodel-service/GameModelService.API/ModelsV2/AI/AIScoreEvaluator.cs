using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{

    //The purpose of this class is to score a particular state
    public class AIScoreEvaluator
    {

        #region "Properties and Initialization"
        const int WIN_SCORE_VALUE = 10000000;
        const int LOSS_SCORE_VALUE = -10000000;

        private GameState EvalState { get; set; }
        public AIHeuristicWeight AIHeuristics { get; set; }
        private HashSet<BoardLocation> DeadSpaces { get; set; }
        private TurnEvaluator Evaluator { get; set; }

        public AIScoreEvaluator(GameState State, AIHeuristicWeight Heuristics = null)
        {
            this.EvalState = State;
            if (Heuristics == null)
                this.AIHeuristics = new AIHeuristicWeight();
            else
                this.AIHeuristics = Heuristics;

            Evaluator = new TurnEvaluator(State);
            this.DeadSpaces = Evaluator.FindDeadLocations();

        }

        public int Score(int PlayerId =-1)
        {
            if (PlayerId < 0)
                PlayerId = EvalState.ActivePlayerId;

            return Score(PlayerId, AIHeuristics);
        }

        public int Score(int PlayerId, AIHeuristicWeight AIWeight)
        {
            int Score = 0;
            if (AIWeight == null) AIWeight = new AIHeuristicWeight();

            //My Pieces
            Score += ScoreFours(PlayerId) * AIWeight.FourWeight;
            Score += ScoreFives(PlayerId) * AIWeight.FiveWeight;
            Score += ScorePositions(EvalState, PlayerId) * AIWeight.PositionWeight;
         
             if (IsAThreat()) if (EvalState.ActivePlayerId == PlayerId)
                                       { Score += AIWeight.ThreatWeight; }
                                  else { Score -= AIWeight.ThreatWeight; }
                 
            //Opponent Pieces
            if (AIWeight.ConsiderOpponentPieces)
            {
                Score -= ScoreFours(EvalState.Opponent(PlayerId)) * AIWeight.FourWeight;
                Score -= ScoreFives(EvalState.Opponent(PlayerId)) * AIWeight.FiveWeight;
                Score -= ScorePositions(EvalState, EvalState.Opponent(PlayerId)) * AIWeight.PositionWeight;
            }
  
            return Score;
        }

        public bool PossibleSetup()
        {
            GameState GSReview = Evaluator.EvaluateTurn(new PlayerTurn(EvalState.ActivePlayerId, new PassMove()));
            AITurnEvaluator AITE = new AITurnEvaluator(GSReview);
            if (AITE.WinningTurns.Count > 1)
            {
                return true;
            }

            return false;
        }

        public bool IsASetup()
        {
            GameState GSReview = Evaluator.EvaluateTurn(new PlayerTurn(EvalState.ActivePlayerId, new PassMove()));
            AITurnEvaluator AITE = new AITurnEvaluator(GSReview);

            //Check if unique states. Not perfect.  
            //Possible to block 2 winning moves with one 
            //Possible to only have one resultant state with two different moves.

            if (AITE.WinningTurns.Count > 1)
            {
                List<string> WinningStates = new List<string>();
                foreach (PlayerTurn t in AITE.WinningTurns)
                {
                    GameState GS = AITE.Evaluator.EvaluateTurn(t);
                    if (!WinningStates.Contains(GS.StateString)) WinningStates.Add(GS.StateString);
                    if (WinningStates.Count > 1) return true;
                }
            }

            return false;
        }

        public bool IsAThreat()
        {
            GameState GSReview = Evaluator.EvaluateTurn(new PlayerTurn(EvalState.ActivePlayerId, new PassMove()));

            TurnEvaluator TE = new TurnEvaluator(GSReview);
            if (TE.GetFirstWinningMove() != null) return true;
            //AITurnEvaluator AITE = new AITurnEvaluator(GSReview);
            //if (AITE.WinningTurns.Count > 0) return true;

            return false;
        }
        public bool IsUnstoppableThreat()
        {
            GameState GSReview = Evaluator.EvaluateTurn(new PlayerTurn(EvalState.ActivePlayerId, new PassMove()));
 
            TurnEvaluator TE = new TurnEvaluator(GSReview);
            if (TE.GetFirstWinningMove() != null) return true;
            //AITurnEvaluator AITE = new AITurnEvaluator(GSReview);
            //if (AITE.WinningTurns.Count == 0) return false;


            Evaluator.Reset();
            foreach (SimpleMove m in Evaluator.GetAvailableSimpleMoves())
            {
                GSReview = Evaluator.EvaluateTurn(m);
                TurnEvaluator WinEvaluator = new TurnEvaluator(GSReview);
                if (WinEvaluator.GetFirstWinningMove() == null) return false;
            }

            return true;
        }



        // A 'FiveSetup' occurs in a set of five spaces in a row/column/diag where: 
        //    -- the three center spaces are unmovable
        //    -- a player can play on either side to win two ways.
        //
        //public bool FindFiveSetup(int PlayerId)
        //{
        //    bool found = false;
        //    for (var row = 0; row < EvalState.Board.Rows; row++)
        //    {
        //        for (var col = 0; col < EvalState.Board.Columns; col++)
        //        {
        //            foreach (WinDirection Direction in Enum.GetValues(typeof(WinDirection)))
        //            {
        //                switch (Direction)
        //                {
        //                    case WinDirection.HORIZONTAL:
        //                        if (EvalState.Board.Columns - col < 5) continue; break;
        //                    case WinDirection.VERTICAL:
        //                        if (EvalState.Board.Rows - row < 5) continue; break;

        //                        //case WinDirection.DIAGONAL_NW_SE:
        //                        //case WinDirection.DIAGONAL_NE_SW:
        //                        //    //continue;
        //                }
        //                found = FindFiveSetup(new BoardLocation(row, col), Direction, PlayerId);
        //                if (found) return true;
        //            }
        //        }
        //    }
        //    return found;
        //}

        public int ScoreFours(int PlayerId)
        {
            int Score = 0;
            for (var row = 0; row < EvalState.Board.Rows; row++)
            {
                for (var col = 0; col < EvalState.Board.Columns; col++)
                {
                    foreach (WinDirection Direction in Enum.GetValues(typeof(WinDirection)))
                    {
                        switch (Direction)
                        {
                            case WinDirection.HORIZONTAL:
                                if (EvalState.Board.Columns - col < 4) continue; break;
                            case WinDirection.VERTICAL:
                                if (EvalState.Board.Rows - row < 4) continue; break;
                            case WinDirection.DIAGONAL_NE_SW:
                                if (!AIHeuristics.ConsiderDiagonals) continue;
                                if (EvalState.Board.Columns - col < 4) continue;
                                if (row < EvalState.Board.Rows - 4 - 1) continue; break;
                            case WinDirection.DIAGONAL_NW_SE:
                                if (!AIHeuristics.ConsiderDiagonals) continue;
                                if (EvalState.Board.Columns - col < 4) continue;
                                if (EvalState.Board.Rows - row < 4) continue; break;

                        }

                        Score += ScoreFour(new BoardLocation(row, col), Direction, PlayerId);
                    }
                }
            }
            return Score;
        }
        public int ScoreFives(int PlayerId)
        {
            int Score = 0;
            for (var row = 0; row < EvalState.Board.Rows; row++)
            {
                for (var col = 0; col < EvalState.Board.Columns; col++)
                {
                    foreach (WinDirection Direction in Enum.GetValues(typeof(WinDirection)))
                    {
                        switch (Direction)
                        {
                            case WinDirection.HORIZONTAL:
                                if (EvalState.Board.Columns - col < 5) continue; break;
                            case WinDirection.VERTICAL:
                                if (EvalState.Board.Rows - row < 5) continue; break;

                            case WinDirection.DIAGONAL_NW_SE:
                            case WinDirection.DIAGONAL_NE_SW:
                                if (!AIHeuristics.ConsiderDiagonals) continue;
                                continue;
                        }
                        Score += ScoreFive(new BoardLocation(row, col), Direction, PlayerId);
                    }
                }
            }
            return Score;
        }
      
        public int ScoreFour(BoardLocation Location, WinDirection WinDirection, int PlayerId)
        {
            int count = 0;
            int alive = 0;
            int i = 0;
            bool contiguous = true;
            BoardLocation loc;
            switch (WinDirection)
            {
                case WinDirection.HORIZONTAL:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(EvalState.Board) && i < 4)
                    {
                        if (!EvalState.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadSpaces.Contains(loc))
                        {
                            return 0;
                        }
                        else if (EvalState.Board.ContentsAt(loc).ContainsPiece
                            && !EvalState.Board.ContentsAt(loc).TokensAllowPushing
                            && EvalState.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (EvalState.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                        {
                            contiguous = false;
                        }

                        i++;
                        loc = new BoardLocation(Location.Row, Location.Column + i);
                    }
                    break;

                case WinDirection.VERTICAL:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(EvalState.Board) && i < 4)
                    {

                        if (!EvalState.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadSpaces.Contains(loc))
                        {
                            return 0;
                        }
                        else if (EvalState.Board.ContentsAt(loc).ContainsPiece
                            && !EvalState.Board.ContentsAt(loc).TokensAllowPushing
                            && EvalState.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (EvalState.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                        {
                            contiguous = false;
                          
                        }
                        i++;
                        loc = new BoardLocation(Location.Row + i, Location.Column);

                    }

                    break;
                case WinDirection.DIAGONAL_NE_SW:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(EvalState.Board) && i < 4)
                    {

                        if (!EvalState.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadSpaces.Contains(loc))
                        {
                            return 0;
                        }
                        else if (EvalState.Board.ContentsAt(loc).ContainsPiece
                            && !EvalState.Board.ContentsAt(loc).TokensAllowPushing
                            && EvalState.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (EvalState.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                        {
                            contiguous = false;
                         
                        }

                        i++;
                        loc = new BoardLocation(Location.Row - i, Location.Column + i);

                    }
                    break;


                case WinDirection.DIAGONAL_NW_SE:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(EvalState.Board) && i < 4)
                    {

                        if (!EvalState.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadSpaces.Contains(loc))
                        {
                            return 0;
                        }
                        else if (EvalState.Board.ContentsAt(loc).ContainsPiece
                            && !EvalState.Board.ContentsAt(loc).TokensAllowPushing
                            && EvalState.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (EvalState.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                        {
                            contiguous = false;
                       
                        }

                        i++;
                        loc = new BoardLocation(Location.Row + i, Location.Column + i);

                    }
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
                    score = 9;
                    if (contiguous) score += 4;
                    score += alive;
                    break;
                case 3:
                    score = 20;
                    if (contiguous) score += 20;
                    if (alive > 0) score += 50;
                    break;
            }


            return score;
        }
        public int ScoreFive(BoardLocation Location, WinDirection WinDirection, int PlayerId)
        {
            int multiplier = 0;
            int count = 0;
            int i = 0;
            bool contiguous = true;
            int alive = 0;
            BoardLocation loc;
            switch (WinDirection)
            {
                case WinDirection.HORIZONTAL:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(EvalState.Board) && i < 5)
                    {
                        if (EvalState.Board.ContentsAt(loc).Control == PlayerId && (i == 0 || i == 4)) return 0;

                        if (!EvalState.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }

                        if (DeadSpaces.Contains(loc))
                        {
                            return 0;
                        }

                        else if (EvalState.Board.ContentsAt(loc).ContainsPiece
                            && !EvalState.Board.ContentsAt(loc).TokensAllowPushing
                            && EvalState.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (EvalState.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                        {
                            //if (AliveSpaces.Contains(loc)) alive++;
                            contiguous = false;
                        }

                        i++;
                        loc = new BoardLocation(Location.Row, Location.Column + i);
                    }
                    multiplier = 2;

                    break;

                case WinDirection.VERTICAL:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(EvalState.Board) && i < 5)
                    {
                        if (EvalState.Board.ContentsAt(loc).Control == PlayerId && (i == 0 || i == 4)) return 0;

                        if (!EvalState.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadSpaces.Contains(loc))
                        {
                            return 0;
                        }
                        else if (EvalState.Board.ContentsAt(loc).ContainsPiece
                            && !EvalState.Board.ContentsAt(loc).TokensAllowPushing
                            && EvalState.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (EvalState.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                        {
                            //if (AliveSpaces.Contains(loc)) alive++;
                            contiguous = false;
                        }

                        i++;
                        loc = new BoardLocation(Location.Row + i, Location.Column);

                    }
                    multiplier = 2;

                    break;

                case WinDirection.DIAGONAL_NE_SW:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(EvalState.Board) && i < 5)
                    {
                        if (EvalState.Board.ContentsAt(loc).Control == PlayerId && (i == 0 || i == 4)) return 0;

                        if (!EvalState.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadSpaces.Contains(loc))
                        {
                            return 0;
                        }
                        else if (EvalState.Board.ContentsAt(loc).ContainsPiece
                            && !EvalState.Board.ContentsAt(loc).TokensAllowPushing
                            && EvalState.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (EvalState.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                        {
                            //if (AliveSpaces.Contains(loc)) alive++;
                            contiguous = false;
                        }

                        i++;
                        loc = new BoardLocation(Location.Row + i, Location.Column - i);

                    }
                    multiplier = 0;

                    break;


                case WinDirection.DIAGONAL_NW_SE:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(EvalState.Board) && i < 5)
                    {
                        if (EvalState.Board.ContentsAt(loc).Control == PlayerId && (i == 0 || i == 4)) return 0;

                        if (!EvalState.Board.ContentsAt(loc).TokensAllowEndHere)
                        {
                            return 0;
                        }
                        if (DeadSpaces.Contains(loc))
                        {
                            return 0;
                        }
                        else if (EvalState.Board.ContentsAt(loc).ContainsPiece
                            && !EvalState.Board.ContentsAt(loc).TokensAllowPushing
                            && EvalState.Board.ContentsAt(loc).Control != PlayerId)
                        {
                            return 0;
                        }

                        if (EvalState.Board.ContentsAt(loc).Control == PlayerId)
                            count++;
                        else
                        {
                            //if (AliveSpaces.Contains(loc)) alive++;
                            contiguous = false;
                        }

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
                    //if (contiguous) score += 50;
                    if (alive > 2)
                    {
                        if (EvalState.ActivePlayerId == PlayerId)
                            score += 200;
                        else
                            score += 50;
                    }
                    break;
                case 3:
                    score = 100;
                    if (alive > 1)
                        if (EvalState.ActivePlayerId != PlayerId)
                            score += 400;
                        else
                            score += 100;

                    break;
            }
            score = score * multiplier;

            return score;
        }

        //public bool FindFiveSetup(BoardLocation Location, WinDirection WinDirection, int PlayerId)
        //{
        //    int count = 0;
        //    int i = 0;
        //    int alive = 0;
        //    BoardLocation loc;
        //    switch (WinDirection)
        //    {
        //        case WinDirection.HORIZONTAL:
        //            loc = new BoardLocation(Location.Row, Location.Column);
        //            while (loc.OnBoard(EvalState.Board) && i < 5)
        //            {
        //                if (EvalState.Board.ContentsAt(loc).Control == PlayerId && (i == 0 || i == 4)) return false;

        //                if (!EvalState.Board.ContentsAt(loc).TokensAllowEndHere)
        //                {
        //                    return false;
        //                }

        //                if (DeadSpaces.Contains(loc))
        //                {
        //                    return false;
        //                }

        //                else if (EvalState.Board.ContentsAt(loc).ContainsPiece
        //                    && !EvalState.Board.ContentsAt(loc).TokensAllowPushing
        //                    && EvalState.Board.ContentsAt(loc).Control != PlayerId)
        //                {
        //                    return false;
        //                }

        //                if (EvalState.Board.ContentsAt(loc).Control == PlayerId
        //                    && !EvalState.Board.ContentsAt(loc).TokensAllowPushing)
        //                {
        //                    count++;
        //                }
        //                else
        //                {
        //                    //if (AliveSpaces.Contains(loc)) alive++;
        //                    //contiguous = false;
        //                }

        //                i++;
        //                loc = new BoardLocation(Location.Row, Location.Column + i);
        //            }

        //            break;

        //        case WinDirection.VERTICAL:
        //            loc = new BoardLocation(Location.Row, Location.Column);
        //            while (loc.OnBoard(EvalState.Board) && i < 5)
        //            {
        //                if (EvalState.Board.ContentsAt(loc).Control == PlayerId && (i == 0 || i == 4)) return false;

        //                if (!EvalState.Board.ContentsAt(loc).TokensAllowEndHere)
        //                {
        //                    return false;
        //                }
        //                if (DeadSpaces.Contains(loc))
        //                {
        //                    return false;
        //                }
        //                else if (EvalState.Board.ContentsAt(loc).ContainsPiece
        //                    && !EvalState.Board.ContentsAt(loc).TokensAllowPushing
        //                    && EvalState.Board.ContentsAt(loc).Control != PlayerId)
        //                {
        //                    return false;
        //                }

        //                if (EvalState.Board.ContentsAt(loc).Control == PlayerId
        //                    && !EvalState.Board.ContentsAt(loc).TokensAllowPushing)
        //                    count++;
        //                else
        //                {
        //                    //if (AliveSpaces.Contains(loc)) alive++;
        //                    //contiguous = false;
        //                }

        //                i++;
        //                loc = new BoardLocation(Location.Row + i, Location.Column);

        //            }
        //            break;

        //        case WinDirection.DIAGONAL_NE_SW:
        //            loc = new BoardLocation(Location.Row, Location.Column);
        //            while (loc.OnBoard(EvalState.Board) && i < 5)
        //            {
        //                if (EvalState.Board.ContentsAt(loc).Control == PlayerId && (i == 0 || i == 4)) return false;

        //                if (!EvalState.Board.ContentsAt(loc).TokensAllowEndHere)
        //                {
        //                    return false;
        //                }
        //                if (DeadSpaces.Contains(loc))
        //                {
        //                    return false;
        //                }
        //                else if (EvalState.Board.ContentsAt(loc).ContainsPiece
        //                    && !EvalState.Board.ContentsAt(loc).TokensAllowPushing
        //                    && EvalState.Board.ContentsAt(loc).Control != PlayerId)
        //                {
        //                    return false;
        //                }

        //                if (EvalState.Board.ContentsAt(loc).Control == PlayerId
        //                    && !EvalState.Board.ContentsAt(loc).TokensAllowPushing)
        //                    count++;
        //                else
        //                {
        //                    //if (AliveSpaces.Contains(loc)) alive++;
        //                    //contiguous = false;
        //                }

        //                i++;
        //                loc = new BoardLocation(Location.Row + i, Location.Column - i);

        //            }
        //            break;


        //        case WinDirection.DIAGONAL_NW_SE:
        //            loc = new BoardLocation(Location.Row, Location.Column);
        //            while (loc.OnBoard(EvalState.Board) && i < 5)
        //            {
        //                if (EvalState.Board.ContentsAt(loc).Control == PlayerId && (i == 0 || i == 4)) return false;

        //                if (!EvalState.Board.ContentsAt(loc).TokensAllowEndHere)
        //                {
        //                    return false;
        //                }
        //                if (DeadSpaces.Contains(loc))
        //                {
        //                    return false;
        //                }
        //                else if (EvalState.Board.ContentsAt(loc).ContainsPiece
        //                    && !EvalState.Board.ContentsAt(loc).TokensAllowPushing
        //                    && EvalState.Board.ContentsAt(loc).Control != PlayerId)
        //                {
        //                    return false;
        //                }

        //                if (EvalState.Board.ContentsAt(loc).Control == PlayerId
        //                    && !EvalState.Board.ContentsAt(loc).TokensAllowPushing)
        //                    count++;
        //                else
        //                {
 
        //                }

        //                i++;
        //                loc = new BoardLocation(Location.Row + i, Location.Column + i);

        //            }

        //            break;

        //    }

        //    if (count > 1
        //        && (count + alive) > 4
        //        && EvalState.ActivePlayerId == PlayerId)
        //        return true;

        //    return false;
        //}

#endregion

        #region "Positions"
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
        #endregion

        #region "Counting"
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
        public static HashSet<BoardLocation> FindDeadLocations(GameState State)
        {
            TurnEvaluator TE = new TurnEvaluator(State);
            return TE.FindDeadLocations();
        }
        public static HashSet<BoardLocation> FindDeadLocations(GameBoard Board)
        {
            TurnEvaluator TE = new TurnEvaluator(new GameState(Board, new GameOptions()));
            return TE.FindDeadLocations();
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

        public static Dictionary<int, int> CountFours(GameState State, int PlayerId, HashSet<BoardLocation> DeadLocations)
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
        public static int CountFour(GameState State, BoardLocation Location, WinDirection WinDirection, int PlayerId, HashSet<BoardLocation> DeadLocations, Dictionary<int, int> Results)
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
        public static Dictionary<int, int> CountFives(GameState State, int PlayerId, HashSet<BoardLocation> DeadLocations)
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
                                if (row < State.Board.Rows - 5 - 1) continue; break;
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
        public static int CountFive(GameState State, BoardLocation Location, WinDirection WinDirection, int PlayerId, HashSet<BoardLocation> DeadLocations, Dictionary<int, int> Results)
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
