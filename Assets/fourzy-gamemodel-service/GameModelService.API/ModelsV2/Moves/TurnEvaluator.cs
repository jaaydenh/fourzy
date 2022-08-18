using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FourzyGameModel.Model
{
    public class TurnEvaluator
    {
        public Stack<MovingPiece> ActivePieces;
        public List<GameAction> ResultActions;
        public GameState OriginalState;
        public GameState EvalState;

        public TurnEvaluator(GameState GameState)
        {
            this.OriginalState = new GameState(GameState);
            this.EvalState = new GameState(OriginalState);
            this.EvalState.SetActionRecorder(RecordAction);
            this.ResultActions = new List<GameAction>() { };
            if (!this.EvalState.ProcessStartOfTurn) this.EvalState.StartOfTurn(this.EvalState.ActivePlayerId);
        }
        
        #region "Available Moves"
        public List<SimpleMove> GetAvailableSimpleMoves(int PlayerId=0)
        {
            if (PlayerId == 0) PlayerId = EvalState.ActivePlayerId;

            List<SimpleMove> PossibleMoves = new List<SimpleMove>();
            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                switch (d) {
                    case Direction.UP:
                    case Direction.DOWN:
                        for (int c=1; c<EvalState.Board.Columns -1; c++)
                        {
                            SimpleMove m = new SimpleMove(new Piece(PlayerId), d, c);
                            if (EvalState.Board.ContentsAt(FirstLocation(m, EvalState.Board)).CanMoveInto(new MovingPiece(m, EvalState.Board), d)){
                                PossibleMoves.Add(m);
                            }
                        }
                        break;
                    case Direction.LEFT:
                    case Direction.RIGHT:
                        for (int r = 1; r < EvalState.Board.Rows - 1; r++)
                        {
                            SimpleMove m = new SimpleMove(new Piece(PlayerId), d, r);
                            if (EvalState.Board.ContentsAt(FirstLocation(m, EvalState.Board)).CanMoveInto(new MovingPiece(m, EvalState.Board), d))
                            {
                                PossibleMoves.Add(m);
                            }
                        }
                        break;
                }

            }
            return PossibleMoves;
        }

        public bool IsAvailableSimpleMove(int PlayerId = 0)
        {
            
            if (PlayerId == 0) PlayerId = EvalState.ActivePlayerId;

            List<SimpleMove> PossibleMoves = new List<SimpleMove>();
            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                switch (d)
                {
                    case Direction.UP:
                    case Direction.DOWN:
                        for (int c = 1; c < EvalState.Board.Columns - 1; c++)
                        {
                            SimpleMove m = new SimpleMove(new Piece(PlayerId), d, c);
                            if (EvalState.Board.ContentsAt(FirstLocation(m, EvalState.Board)).CanMoveInto(new MovingPiece(m, EvalState.Board), d))
                            {
                                return true;
                            }
                        }
                        break;
                    case Direction.LEFT:
                    case Direction.RIGHT:
                        for (int r = 1; r < EvalState.Board.Rows - 1; r++)
                        {
                            SimpleMove m = new SimpleMove(new Piece(PlayerId), d, r);
                            if (EvalState.Board.ContentsAt(FirstLocation(m, EvalState.Board)).CanMoveInto(new MovingPiece(m, EvalState.Board), d))
                            {
                                return true;
                            }
                        }
                        break;
                }

            }
            return false;
        }


        public List<SimpleMove> GetAvailableSimpleMoves(Direction MoveDirection, int PlayerId = 0)
        {
            if (PlayerId == 0) PlayerId = EvalState.ActivePlayerId;

            List<SimpleMove> PossibleMoves = new List<SimpleMove>();
                switch (MoveDirection)
                {
                    case Direction.UP:
                    case Direction.DOWN:
                        for (int c = 1; c < EvalState.Board.Columns - 1; c++)
                        {
                            SimpleMove m = new SimpleMove(new Piece(PlayerId), MoveDirection, c);
                            if (EvalState.Board.ContentsAt(FirstLocation(m, EvalState.Board)).CanMoveInto(new MovingPiece(m, EvalState.Board), MoveDirection))
                            {
                                PossibleMoves.Add(m);
                            }
                        }
                        break;
                    case Direction.LEFT:
                    case Direction.RIGHT:
                        for (int r = 1; r < EvalState.Board.Rows - 1; r++)
                        {
                            SimpleMove m = new SimpleMove(new Piece(PlayerId), MoveDirection, r);
                            if (EvalState.Board.ContentsAt(FirstLocation(m, EvalState.Board)).CanMoveInto(new MovingPiece(m, EvalState.Board), MoveDirection))
                            {
                                PossibleMoves.Add(m);
                            }
                        }
                        break;
                }
            return PossibleMoves;
        }

        public Dictionary<SimpleMove, GameState> GetAvailableMoveInfo(int PlayerId = 0)
        {
            if (PlayerId == 0) PlayerId = EvalState.ActivePlayerId;

            Dictionary<SimpleMove, GameState> MoveInfo = new Dictionary<SimpleMove, GameState>();

            foreach (SimpleMove m in GetAvailableSimpleMoves(PlayerId))
            {
                MoveInfo.Add(m, new GameState(EvaluateTurn(new PlayerTurn(m))));
            }
  
            return MoveInfo;
        }

        public Dictionary<SimpleMove, GameState> GetAvailableMoveInfo(List<SimpleMove> Moves)
        {

            Dictionary<SimpleMove, GameState> MoveInfo = new Dictionary<SimpleMove, GameState>();
            foreach (SimpleMove m in Moves)
            {
                MoveInfo.Add(m, new GameState(EvaluateTurn(new PlayerTurn(m))));
            }

            return MoveInfo;
        }


        public List<SimpleMove> GetWinningMoves(int PlayerId=0, bool ConsiderDiagonals = true)
        {
            if (PlayerId == 0) PlayerId = EvalState.ActivePlayerId;

            DateTime Before = DateTime.Now;

            List<SimpleMove> WinningMoves = new List<SimpleMove>();
            foreach (SimpleMove m in GetAvailableSimpleMoves(PlayerId))
            {
                GameState GS = EvaluateTurn(new PlayerTurn(m));
                if (GS.WinnerId == PlayerId)
                {
                    if (!ConsiderDiagonals) 
                        if (GS.WinningLocations[0].Row != GS.WinningLocations[0].Row
                            && GS.WinningLocations[0].Column != GS.WinningLocations[0].Column) continue;
                   WinningMoves.Add(m);
                }
            }
            double TimeTaken = DateTime.Now.Subtract(Before).TotalMilliseconds;

            return WinningMoves;                
        }

        public SimpleMove GetFirstWinningMove(int PlayerId = 0, bool ConsiderDiagonals = true)
        {
            if (PlayerId == 0) PlayerId = EvalState.ActivePlayerId;

            //Need at least 4 pieces on the board to win.
            if (EvalState.Board.FindPieces(PlayerId).Count < 3) return null;

            foreach (SimpleMove m in GetAvailableSimpleMoves(PlayerId))
            {
                Reset();
                GameState GS = EvaluateTurn(new PlayerTurn(m));
                if (GS.WinnerId == PlayerId)
                {
                    if (!ConsiderDiagonals)
                        if (GS.WinningLocations[0].Row != GS.WinningLocations[0].Row
                            && GS.WinningLocations[0].Column != GS.WinningLocations[0].Column) continue;
                    Reset();
                    return m;
                }
            }
            Reset();
            return null;
        }


        public bool CanIMakeMove(SimpleMove Move)
        {
            //Check to see if piece can move into the first location.
            if (EvalState.Board.ContentsAt(FirstLocation(Move, EvalState.Board)).CanMoveInto(new MovingPiece(Move, EvalState.Board), Move.Direction))
            {
                return true;
            }
            return false;
        }
        #endregion

        public GameState EvaluateStartOfTurn()
        {
            this.ResultActions = new List<GameAction>();
            this.EvalState = new GameState(OriginalState);
            this.EvalState.SetActionRecorder(RecordAction);
            this.EvalState.Random.Reset();

            this.EvalState.StartOfTurn(EvalState.ActivePlayerId);
            return EvalState;
        }

        public void Reset()
        {
            this.ResultActions = new List<GameAction>();
            this.EvalState = new GameState(OriginalState);
            this.EvalState.SetActionRecorder(RecordAction);
            this.EvalState.Random.Reset();
            if (!this.EvalState.ProcessStartOfTurn) this.EvalState.StartOfTurn(this.EvalState.ActivePlayerId);
        }

        public GameState EvaluateTurn(SimpleMove Move, bool IgnoreActivePlayer = false, bool TriggerStartOfTurn = true, bool TriggerEndOfTurn = true)
        {
            return EvaluateTurn(new PlayerTurn(Move), IgnoreActivePlayer, TriggerStartOfTurn, TriggerEndOfTurn);
        }
        
        public GameState EvaluateTurn(PlayerTurn Turn, bool IgnoreActivePlayer = false, bool TriggerStartOfTurn = true, bool TriggerEndOfTurn = true)
        {

            //if (TriggerStartOfTurn && TriggerEndOfTurn)
            //{
            //    GameState GSCache = AITurnEvaluatorCache.GetState(this.EvalState.StateString, Turn.Notation);
            //    if (GSCache != null)
            //        return GSCache;
            //}

            this.ResultActions = new List<GameAction>();
            this.EvalState = new GameState(OriginalState);
            this.EvalState.SetActionRecorder(RecordAction);
            this.EvalState.Random.Reset();

            if (Turn.PlayerId == 0)
            {
                ResultActions.Add(new GameActionInvalidMove(null, "PlayerId Not Set (0)", InvalidTurnType.BADPLAYERID));
                return OriginalState;
            }

            if (Turn.PlayerId != EvalState.ActivePlayerId && !IgnoreActivePlayer)
            {
                ResultActions.Add(new GameActionInvalidMove(null, "PlayerId Does Not Match Active PlayerId", InvalidTurnType.WRONGPLAYER));
                return OriginalState;
            }

            if (TriggerStartOfTurn)
                this.EvalState.StartOfTurn(Turn.PlayerId);

            foreach (IMove m in Turn.Moves)
            {
                switch(m.MoveType)
                {
                    case MoveType.SIMPLE:
                        if (EvalState.Options.MovesReduceHerd)
                        {
                            if (EvalState.Herds != null) 
                                if (EvalState.Herds.Count > 0)
                            if (EvalState.Herds[EvalState.ActivePlayerId].Members.Count < 1 )
                            {
                                EvalState.WinnerId = this.EvalState.Opponent(this.EvalState.WinnerId);
                                RecordAction(new GameActionGameEnd(GameEndType.NOPIECES, this.EvalState.WinnerId, null));
                                return OriginalState;
                                //RecordAction(new GameActionGameEnd(GameEndType.WIN, this.EvalState.Opponent(this.EvalState.WinnerId), null));
                            }
                        }

                        if (!ProcessSimpleMove((SimpleMove)m)) return OriginalState;
                        if (EvalState.Options.MovesReduceHerd)
                            if (EvalState.Herds.Count > 0)
                                if (EvalState.Herds[EvalState.ActivePlayerId] != null)
                                    if (EvalState.Herds[EvalState.ActivePlayerId].Members.Count > 0)
                                        EvalState.Herds[EvalState.ActivePlayerId].Members.RemoveAt(0);
                        break; 
                    case MoveType.SPELL:
                        if (!ProcessSpell((ISpell)m)) return OriginalState;

                        break; 
                    case MoveType.TOKEN:
                        break; 

                    case MoveType.PASS:
                        PassMove Pass = (PassMove)m;
                        Player Passer = EvalState.Players[this.EvalState.ActivePlayerId];

                        RecordAction(new GameActionPass(Passer));
                        break; 

                    case MoveType.BOSSPOWER:
                        if (!ProcessPower((IBossPower)m)) return OriginalState;
                        break; 

                }
            }
            if (TriggerEndOfTurn)
                this.EvalState.EndOfTurn(Turn.PlayerId);

            if (EvaluateWinner())
            {
                if (EvalState.WinnerId > 0)
                {
                    RecordAction(new GameActionGameEnd(GameEndType.WIN, this.EvalState.WinnerId, EvalState.WinningLocations));
                    EvalState.ActivePlayerId = 0;
                }
                else
                {
                    RecordAction(new GameActionGameEnd(GameEndType.DRAW, this.EvalState.WinnerId, EvalState.WinningLocations));
                    EvalState.ActivePlayerId = 0;
                }
            }
            else
            {
                if (EvaluateDraw())
                {
                    RecordAction(new GameActionGameEnd(GameEndType.DRAW, this.EvalState.WinnerId, null));
                    EvalState.ActivePlayerId = 0;
                }
                
            }
            EvalState.TurnCount++;

            AITurnEvaluatorCache.AddState(EvalState.StateString, Turn.Notation, EvalState);

            return EvalState;
        }

        #region "Simple Piece Movement"

        public List<BoardLocation> TraceMove(SimpleMove Move)
        {
            List<BoardLocation> Locations = new List<BoardLocation>();
            GameBoard Board = EvalState.Board;

            //Check to see if piece can move into the first location.
            if (!Board.ContentsAt(FirstLocation(Move, Board)).CanMoveInto(new MovingPiece(Move, Board), Move.Direction))
            {
                return Locations;
            }

            //Trace all possible spaces a piece can move to.
            MovingPiece CurrentPiece = new MovingPiece(Move, Board);
            Locations.Add(CurrentPiece.Location);

            //ignore momentum.
            //some cheating on friction.
            int count = 100;
            while (true && count-->0)
            {
                if (!Board.ContentsAt(CurrentPiece.Location).TokensAllowPushing
                    && Board.ContentsAt(CurrentPiece.Location).TokenForceStop) break;
                if (!Board.ContentsAt(CurrentPiece.Location).TokensAllowPushing
                    && Board.ContentsAt(CurrentPiece.Location).TokenAdjustFrictionAmount == 100) break;

                CurrentPiece.Direction = Board.ContentsAt(CurrentPiece.Location).ProcessDirection(CurrentPiece);
                BoardLocation NextLocation = GetNextMovingLocation(CurrentPiece);
                if (!NextLocation.OnBoard(Board)) break;
                if (Board.ContentsAt(NextLocation).NoMorePieces) break;

                //This isn't perfect because of chain reaction.  A pileup might prevent a piece from getting deeper in a push chain.
                if (!Board.ContentsAt(NextLocation).CanMoveInto(CurrentPiece, CurrentPiece.Direction)) break;

                if (!Locations.Contains(NextLocation))
                    Locations.Add(NextLocation);
                CurrentPiece.Location = NextLocation;
            }

            return Locations;
        }

        public List<BoardLocation> FindPossibleLocations()
        {
            List<BoardLocation> Possible = new List<BoardLocation>();
            foreach (SimpleMove m in GetAvailableSimpleMoves())
            {
                foreach(BoardLocation l in TraceMove(m))
                {
                    if (!Possible.Contains(l)) Possible.Add(l);
                }
            }

            return Possible;
        }

        public HashSet<BoardLocation> FindDeadLocations()
        {
            List<BoardLocation> Possible = FindPossibleLocations();
            HashSet<BoardLocation> Dead = new HashSet<BoardLocation>();
            foreach (BoardSpace s in EvalState.Board.Contents )
            {
                if (!Possible.Contains(s.Location))
                    if (!s.ContainsPiece)
                        Dead.Add(s.Location);
            }

            return Dead;
        }

        public List<BoardLocation> FindDeadLocationsList()
        {
            List<BoardLocation> Possible = FindPossibleLocations();
            List<BoardLocation> Dead = new List<BoardLocation>();
            foreach (BoardSpace s in EvalState.Board.Contents)
            {
                if (!Possible.Contains(s.Location))
                    if (!s.ContainsPiece)
                        Dead.Add(s.Location);
            }

            return Dead;
        }

        public bool ProcessSimpleMove(SimpleMove Move)
        {
            if (!CanIMakeMove(Move))
            {
                RecordAction(new GameActionInvalidMove(Move, "Cannot Make Move", InvalidTurnType.BLOCKED));
                return false;
            }

            ActivePieces = new Stack<MovingPiece>();
            //ActivePieces.Add(new MovingPiece(Move, EvalState.Board.Rows, EvalState.Board.Columns));

            //any move may translate into a several moving pieces.
            foreach (MovingPiece p in SendPieceOnBoard(Move))
            {
                ActivePieces.Push(p);
            }

            while (ActivePieces.Count > 0)
            {
                //Get an active piece
                MovingPiece CurrentPiece = ActivePieces.Pop();
   
                if (CurrentPiece.Momentum <= 0)
                {

                    if (!EvalState.Board.ContentsAt(CurrentPiece.Location).TokensAllowEndHere)
                    {
                        CurrentPiece.Momentum = 1;
                    }
                    else
                    {

                        if (!EvalState.Board.ContentsAt(CurrentPiece.Location).ContainsPiece)
                        {
                            RecordAction(new GameActionStop(CurrentPiece, CurrentPiece.Location, StopType.MOMENTUM));
                            EvalState.Board.AddPiece(CurrentPiece.Piece, CurrentPiece.Location);
                            EvalState.PieceStopsOnSpace(CurrentPiece);
                            continue;
                        }
                        if (EvalState.Board.ContentsAt(CurrentPiece.Location).ContainsPiece
                          && EvalState.Board.ContentsAt(CurrentPiece.Location).ActivePiece.UniqueId == CurrentPiece.UniqueId)
                        {
                            RecordAction(new GameActionStop(CurrentPiece, CurrentPiece.Location, StopType.MOMENTUM));
                            EvalState.PieceStopsOnSpace(CurrentPiece);
                            continue;
                        }
                        else if (CurrentPiece.Momentum == 0) CurrentPiece.Momentum = 1;

                    }

                }

                else if (CurrentPiece.Friction >= 100)
                {
                    if (!EvalState.Board.ContentsAt(CurrentPiece.Location).TokensAllowEndHere)
                    {
                        CurrentPiece.Momentum = 1;
                    }
                    else
                    {
                        RecordAction(new GameActionStop(CurrentPiece, CurrentPiece.Location, StopType.FRICTION));
                        if (!EvalState.Board.ContentsAt(CurrentPiece.Location).ContainsPiece)
                        {
                            EvalState.Board.AddPiece(CurrentPiece.Piece, CurrentPiece.Location);
                            EvalState.PieceStopsOnSpace(CurrentPiece);
                            continue;
                        }
                        if (EvalState.Board.ContentsAt(CurrentPiece.Location).ContainsPiece
                   && EvalState.Board.ContentsAt(CurrentPiece.Location).ActivePiece.UniqueId == CurrentPiece.UniqueId)
                        {
                            EvalState.PieceStopsOnSpace(CurrentPiece);
                            continue;
                        }
                        else if (CurrentPiece.Momentum == 0) CurrentPiece.Momentum = 1;

                    }
                }
 

                else if (CurrentPiece.Piece.ConditionCount(PieceConditionType.INERTIA) > 0)
                {
                    CurrentPiece.RemoveOneCondition(PieceConditionType.INERTIA);

                    if (CurrentPiece.Piece.ConditionCount(PieceConditionType.INERTIA) == 0)
                    {
                        if (!EvalState.Board.ContentsAt(CurrentPiece.Location).TokensAllowEndHere)
                        {
                            CurrentPiece.Momentum = 1;
                        }
                        else
                        {
                            if (!EvalState.Board.ContentsAt(CurrentPiece.Location).ContainsPiece)
                            {
                                EvalState.Board.AddPiece(CurrentPiece.Piece, CurrentPiece.Location);
                                EvalState.PieceStopsOnSpace(CurrentPiece);
                                RecordAction(new GameActionStop(CurrentPiece, CurrentPiece.Location, StopType.INERTIA));
                                continue;
                            }
                            if (EvalState.Board.ContentsAt(CurrentPiece.Location).ContainsPiece
                       && EvalState.Board.ContentsAt(CurrentPiece.Location).ActivePiece.UniqueId == CurrentPiece.UniqueId)
                            {
                                EvalState.PieceStopsOnSpace(CurrentPiece);
                                RecordAction(new GameActionStop(CurrentPiece, CurrentPiece.Location, StopType.INERTIA));
                                continue;
                            }
                            else
                            {
                                if (CurrentPiece.Momentum == 0) CurrentPiece.Momentum = 1;
                            }

                        }

                    }
                }

                else if (EvalState.Board.ContentsAt(CurrentPiece.Location).TokenForceStop)
                {
                    //if (!EvalState.Board.ContentsAt(CurrentPiece.Location).TokensAllowEndHere)
                    //{
                    //    CurrentPiece.Momentum = 1;
                    //}
                    //else
                    //{
                    //    RecordAction(new GameActionStop(CurrentPiece, CurrentPiece.Location, StopType.TOKEN));
                    //    if (!EvalState.Board.ContentsAt(CurrentPiece.Location).ContainsPiece)
                    //    {
                    //        EvalState.PieceStopsOnSpace(CurrentPiece);

                    //        EvalState.Board.AddPiece(CurrentPiece.Piece, CurrentPiece.Location);
                    //        continue;
                    //    }
                    //    if (EvalState.Board.ContentsAt(CurrentPiece.Location).ContainsPiece
                    //        && EvalState.Board.ContentsAt(CurrentPiece.Location).ActivePiece.UniqueId == CurrentPiece.UniqueId)
                    //    {
                    //        EvalState.PieceStopsOnSpace(CurrentPiece);
                    //        continue;
                    //    }
                    //}


                }

                //Find its desired direction. Where do you want to go little Fourzy?
                if (CurrentPiece.Location.OnBoard(EvalState.Board))
                {
                    CurrentPiece.Direction = EvalState.Board.ContentsAt(CurrentPiece.Location).ProcessDirection(CurrentPiece);
                } 
                
                //if (EvalState.Board.ContentsAt(CurrentPiece.Location).TokenAffectsMovement)
                //{
                //    CurrentPiece.Direction = EvalState.Board.ContentsAt(CurrentPiece.Location).ProcessDirection(CurrentPiece);
                //}

                //Find out where it wants to move based on direction.
                BoardLocation NextLocation = GetNextMovingLocation(CurrentPiece);

                //If next move is off board. Stop.
                //We discussed possibility of having some edges a piece can fall off.
                //Such as a mountain cliff.
                //It might be useful to define wall properties, or tokens
                //instead of having an 'edge of board' calculation.
                if (!NextLocation.OnBoard(EvalState.Board))
                {
                    EvalState.PieceBumpsIntoLocation(CurrentPiece, CurrentPiece.Location);
                    EvalState.PieceStopsOnSpace(CurrentPiece);

                    RecordAction(new GameActionStop(CurrentPiece, CurrentPiece.Location, StopType.WALL));
                    

                    if (!EvalState.Board.ContentsAt(CurrentPiece.Location).ContainsPieceId(CurrentPiece.UniqueId))
                    {
                        if (EvalState.Board.ContentsAt(CurrentPiece.Location).ContainsPiece) throw new Exception("Already a piece on this space.");
                        EvalState.Board.AddPiece(CurrentPiece.Piece, CurrentPiece.Location);
                    }
                    continue;
                }

                //Look at the space it wants to go. 
                //If the new space is empty, let's try moving there.
                //Empty = no token, no pieces.
                if (EvalState.Board.ContentsAt(NextLocation).Empty)
                {
                    foreach (MovingPiece p in MovePieceToLocation(CurrentPiece, NextLocation, CurrentPiece.Direction))
                    {
                        ActivePieces.Push(p);
                    }
                    continue;
                }

                //If the piece is not allowed to move into this space, maybe unmovable object or token filling space
                //No More Pieces might mean: 
                //   1. a token is set to prevent entry
                //   2. a piece is on the space and isMovable = false
                //Trigger a bump
                if (EvalState.Board.ContentsAt(NextLocation).NoMorePieces)
                {
                    EvalState.PieceBumpsIntoLocation(CurrentPiece, NextLocation);
                    EvalState.PieceStopsOnSpace(CurrentPiece);
                    if (!EvalState.Board.ContentsAt(CurrentPiece.Location).ContainsPieceId(CurrentPiece.UniqueId))
                    {
                        if (EvalState.Board.ContentsAt(CurrentPiece.Location).ContainsPiece) throw new Exception("Already a piece on this space.");
                        EvalState.Board.AddPiece(CurrentPiece.Piece, CurrentPiece.Location);
                    }
                    continue;
                }

                //If there is the piece is allowed to enter the space, but it might be depend on other conditions
                //  we need to recurse and move the next piece.
                //  if we're allowed to do this, let's initiate a move into the space.
                if (EvalState.Board.ContentsAt(NextLocation).CanMoveInto(CurrentPiece, CurrentPiece.Direction))
                {
                        foreach (MovingPiece p in MovePieceToLocation(CurrentPiece, NextLocation, CurrentPiece.Direction))
                        {
                            ActivePieces.Push(p);
                        }
                }
                //else if (EvalState.Board.ContentsAt(NextLocation).TokensAllowPushing && EvalState.Board.ContentsAt(NextLocation).CanMoveInto(CurrentPiece, CurrentPiece.Direction, -1, true))
                //{
                //       CurrentPiece.AddCondition(PieceConditionType.STRAIGHT);
                //    foreach (MovingPiece p in MovePieceToLocation(CurrentPiece, NextLocation, CurrentPiece.Direction, true))
                //    {
                //        ActivePieces.Push(p);
                //    }
                //}
                else
                {
                    EvalState.PieceBumpsIntoLocation(CurrentPiece, NextLocation);
                    EvalState.PieceStopsOnSpace(CurrentPiece);
                    if (!EvalState.Board.ContentsAt(CurrentPiece.Location).ContainsPieceId(CurrentPiece.UniqueId))
                    {
                        if (EvalState.Board.ContentsAt(CurrentPiece.Location).ContainsPiece) throw new Exception("Already a piece on this space.");
                        EvalState.Board.AddPiece(CurrentPiece.Piece, CurrentPiece.Location);
                    }
                }
            }

            //WE CAN USE THIS CODE IF WE IMPLEMENT A SPRINGY BLOCKER
            if (ActivePieces.Count == 0)
            {

                foreach (KeyValuePair<BoardLocation, Piece> p in EvalState.Board.Pieces)
                {
                    if (p.Value.HasCondition(PieceConditionType.PUSHED_UP))
                    { p.Value.ClearCondition(PieceConditionType.PUSHED_UP); ActivePieces.Push(new MovingPiece(p.Value, p.Key, Direction.UP, 10)); }
                    else if (p.Value.HasCondition(PieceConditionType.PUSHED_DOWN))
                    { p.Value.ClearCondition(PieceConditionType.PUSHED_DOWN); ActivePieces.Push(new MovingPiece(p.Value, p.Key, Direction.DOWN, 10)); }
                    else if (p.Value.HasCondition(PieceConditionType.PUSHED_LEFT))
                    { p.Value.ClearCondition(PieceConditionType.PUSHED_LEFT); ActivePieces.Push(new MovingPiece(p.Value, p.Key, Direction.LEFT, 10)); }
                    else if (p.Value.HasCondition(PieceConditionType.PUSHED_RIGHT))
                    { p.Value.ClearCondition(PieceConditionType.PUSHED_RIGHT); ActivePieces.Push(new MovingPiece(p.Value, p.Key, Direction.RIGHT, 10)); }
                }
            }

            return true;
        }

        public static BoardLocation GetNextMovingLocation(MovingPiece Piece)
        {
            BoardLocation NextLocation = new BoardLocation(Piece.Location);

            //Should this be here.  It might need to be somewhere else.
            //// does piece have momentum?
            //// does piece have friction?
            //// does piece have a direction?
            //if (Piece.Direction == Direction.NONE ||
            //    Piece.Momentum == 0 ||
            //    Piece.Friction > 1) return NextLocation;

            NextLocation = Piece.Location.Neighbor(Piece.Direction);

            return NextLocation;
        }
                
        public static BoardLocation FirstLocation(SimpleMove Move, GameBoard Board)
        {
            return new MovingPiece(Move, Board).Location;
        }

        public List<MovingPiece> SendPieceOnBoard(SimpleMove Move)
        {
            MovingPiece NewPiece = new MovingPiece(Move, EvalState.Board);
            //Message Game State to indicate new piece in play.
            //PieceEntersBoard(NewPiece);
            NewPiece.Pushed = true;
            return MovePieceToLocation(NewPiece, NewPiece.Location, Move.Direction);
          
        }
        
        //Added Direction to compensate for wind.  A moving piece may have momentum in a direction, but get pushed into a space but keep it's original direction.
        public List<MovingPiece> MovePieceToLocation(MovingPiece Piece, BoardLocation NextLocation, Direction Direction, bool IgnoreDirectionChange = false)
        {
            //Return the updated piece after a move or the pushed piece if a piece pushes a piece.
            MovingPiece EndPiece = null;
            BoardLocation StartLocation = Piece.Location;

            if (StartLocation.OnBoard(EvalState.Board)) EvalState.Board.PieceLeavesSpace(Piece);

            if (!Piece.Pushed)
            {
                EvalState.Board.ContentsAt(Piece.Location).RemovePieceFrom();
            }
            Piece.Pushed = false;

            GameAction MoveAction = new GameActionMove(Piece, StartLocation, NextLocation);
            RecordAction(MoveAction);

            //PieceLeavesSpace(Piece);
            EndPiece = EvalState.Board.ContentsAt(NextLocation).MovePieceOn(Piece, IgnoreDirectionChange);
            //PieceEntersSpace(Piece);
            
            if (EndPiece.Pushed)
            {
                GameAction PushAction = new GameActionPush(Piece, EndPiece, Piece.Location);
                RecordAction(PushAction);

                //reset push flag for continuing the move.
            }

            return new List<MovingPiece>() { EndPiece };
        }

        //Moves a piece one space
        public List<MovingPiece> ContinueMovement(MovingPiece Piece)
        {
               
            //Return the updated piece after a move or the pushed piece if a piece pushes a piece.
                //MovingPiece EndPiece = null;
                if (EvalState.Board.ContentsAt(Piece.Location).ContainsPieceId(Piece.Piece.UniqueId))
                {
                    EvalState.Board.ContentsAt(Piece.Location).RemovePieceFrom();
                }

                BoardLocation NextLocation = TurnEvaluator.GetNextMovingLocation(Piece);
                return MovePieceToLocation(Piece, NextLocation, Piece.Direction);
        }
        #endregion

        #region "Spells"
        public bool ProcessSpell(ISpell Spell)
        {
            foreach (IGameEffect e in EvalState.GameEffects)
            {
                if (e.Type == GameEffectType.VOID) {
                    RecordAction(new GameActionSpell(Spell));
                    return false;
                    }
            }

            if (EvalState.Players[Spell.PlayerId].Magic >= Spell.Cost)
            {
                EvalState.Players[Spell.PlayerId].Magic -= Spell.Cost;
                if (Spell.Cast(EvalState, out _))
                {
                    RecordAction(new GameActionSpell(Spell));
                    return true;
                }
                else
                {
                    RecordAction(new GameActionSpellFizzle(Spell));
                }
            }
            else
            {
                RecordAction(new GameActionSpellFizzle(Spell, SpellFailureType.NOT_ENOUGH_MAGIC));
            }
            return false;
        }

        public bool ProcessPower(IBossPower Power)
        {
            if (Power.Activate(EvalState))
            {
                RecordAction(new GameActionBossPower(Power));
                return true;
            }

            return false;
        }

        #endregion

        #region "Win Evaluation"

        public bool EvaluateWinner()
        {
            int count = 0;
            foreach(int p in EvalState.Players.Keys)
            {
                //if (DidPlayerWinAndFindWinningLocations(p)) { count++; };
                if (DidPlayerWinRevised(p)) { DidPlayerWinAndFindWinningLocations(p);  count++; };
            }

            //draw because both sides have a winning condition.
            if (count > 1)
            {
                //check for wins of multiple lengths.

                int[] winspacecount = new int[3];
                winspacecount[1] = 0;
                winspacecount[2] = 0;

                EvalState.WinningLocations.Clear();
                foreach (int p in EvalState.Players.Keys)
                {
                    foreach (BoardLocation l in GetWinningLocations(p))
                    {
                        winspacecount[p]++;
                        EvalState.WinningLocations.Add(l);
                    }
                    //EvalState.WinningLocations.AddRange(GetWinningLocations(p));
                }
                if (winspacecount[1] == winspacecount[2]) EvalState.WinnerId = 0;
                else EvalState.WinnerId = (winspacecount[1] > winspacecount[2] ? 1 : 2);
            }
            return (count > 0);
        }

        public List<BoardLocation> GetWinningLocations(int PlayerId)
        {
            List<BoardLocation> WinSpots = new List<BoardLocation>();
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
                                if (EvalState.Board.Columns - col < 4) continue;
                                if (row < EvalState.Board.Rows - 4 - 1) continue; break;
                            case WinDirection.DIAGONAL_NW_SE:
                                if (EvalState.Board.Columns - col < 4) continue;
                                if (EvalState.Board.Rows - row < 4) continue; break;
                        }
                        List<BoardLocation> WinTest = CheckForWin(new BoardLocation(row, col), Direction, PlayerId);
                        if (WinTest != null)
                        {
                            return WinTest;
                        }
                    }
                }
            }
            return WinSpots;
        }

        public bool PossibleToWin(int PlayerId)
        {
            if (EvalState.Board.FindPieces(PlayerId).Count < 4) return false;

            int[] columns_count = new int[EvalState.Board.Columns];
            int[] row_count = new int[EvalState.Board.Rows];
            int[] d1_count = new int[EvalState.Board.Rows + EvalState.Board.Columns];
            int[] d2_count = new int[EvalState.Board.Rows + EvalState.Board.Columns];

            for (int i = 0; i < EvalState.Board.Columns; i++) columns_count[i] = 0;
            for (int i = 0; i < EvalState.Board.Rows; i++) columns_count[i] = 0;
            for (int i = 0; i < EvalState.Board.Rows  + EvalState.Board.Columns; i++) d1_count[i] = 0;
            for (int i = 0; i < EvalState.Board.Rows  + EvalState.Board.Columns; i++) d2_count[i] = 0;
         
            foreach (BoardLocation l in EvalState.Board.Pieces.Keys)
            {
                if (EvalState.Board.Pieces[l].PlayerId != PlayerId) continue;
                columns_count[l.Column]++;
                row_count[l.Row]++;
                if (l.Row <= EvalState.Board.Rows-4 && l.Column <= EvalState.Board.Columns-4) d1_count[EvalState.Board.Rows + l.Row - l.Column]++;
                if (l.Row >= EvalState.Board.Rows - 4 && l.Column >= EvalState.Board.Columns - 4) d2_count[l.Row + l.Column]++;
            }

            foreach (int c in columns_count) if (c > 0) return true;
            foreach (int r in row_count) if (r > 0) return true;
            foreach (int d in d1_count) if (d > 0) return true;
            foreach (int d in d2_count) if (d > 0) return true;

            return false;
        }

        public bool DidPlayerWinRevised(int PlayerId)
        {
            //Dictionary<BoardLocation, Piece> Pieces = EvalState.Board.Pieces;
            //if (Pieces.Count < 4) return false;
            List<BoardLocation> Control = EvalState.Board.FindControl(PlayerId);
            if (Control.Count < 4) return false;

            BitArray[] columns = new BitArray[EvalState.Board.Columns];
            BitArray[] rows = new BitArray[EvalState.Board.Rows];
            BitArray[] diag1 = new BitArray[EvalState.Board.Rows + EvalState.Board.Columns];
            BitArray[] diag2 = new BitArray[EvalState.Board.Rows + EvalState.Board.Columns];

            for (int i = 0; i < EvalState.Board.Columns; i++) columns[i] = new BitArray(EvalState.Board.Rows);
            for (int i = 0; i < EvalState.Board.Rows; i++) rows[i] = new BitArray(EvalState.Board.Columns); 
            for (int i = 0; i < EvalState.Board.Rows + EvalState.Board.Columns; i++) diag1[i] = new BitArray(Math.Max(EvalState.Board.Rows, EvalState.Board.Columns));
            for (int i = 0; i < EvalState.Board.Rows + EvalState.Board.Columns; i++) diag2[i] = new BitArray(Math.Max(EvalState.Board.Rows, EvalState.Board.Columns));

            
            //Pieces = Pieces.Where(kvp => kvp.Value.PlayerId == PlayerId).ToDictionary(i => i.Key, i => i.Value);
            //foreach (BoardLocation l in Pieces.Keys)

            foreach(BoardLocation l in Control)
            {
                //if (EvalState.Board.Pieces[l].PlayerId != PlayerId) continue;
                //if (EvalState.Board.ContentsAt(l).ActivePiece.PlayerId != PlayerId) continue;
                columns[l.Column][l.Row] = true;
                rows[l.Row][l.Column] = true;
                //This is broken. Review these if statements.
                //if (l.Row <= EvalState.Board.Rows - 4 && l.Column <= EvalState.Board.Columns - 4) diag1[EvalState.Board.Rows + l.Row - l.Column][l.Row]=true;
                //if (l.Row >= EvalState.Board.Rows - 5 && l.Column >= EvalState.Board.Columns - 5) diag2[l.Row + l.Column][l.Row]=true;
                diag1[EvalState.Board.Rows + l.Row - l.Column][l.Row] = true;
                diag2[l.Row + l.Column][l.Row] = true;

            }

            //foreach (int c in columns_count) if (c > 0) return true;
            foreach (BitArray c in columns)
            {
                if (c.Count < 4) continue;
                int count = 0;
                foreach (bool b in c)
                {
                    if (b) count++; else { count = 0; }
                    if (count == 4) return true;
                }
            }
            //foreach (int r in row_count) if (r > 0) return true;
            foreach (BitArray r in rows)
            {
                if (r.Count < 4) continue;
                int count = 0;
                foreach (bool b in r)
                {
                    if (b) count++; else { count = 0; }
                    if (count == 4) return true;
                }
            }

            //foreach (int d in d1_count) if (d > 0) return true;
         
            foreach (BitArray d in diag1)
            {
                if (d.Count < 4) continue;
                int count = 0;
                foreach (bool b in d)
                {
                    if (b) count++; else { count = 0; }
                    if (count == 4) return true;
                }
            }
            //foreach (int d in d2_count) if (d > 0) return true;
            foreach (BitArray d in diag2)
            {
                if (d.Count < 4) continue;
                int count = 0;
                foreach (bool b in d)
                {
                    if (b) count++; else { count = 0; }
                    if (count == 4) return true;
                }
            }
            return false;
        }

        public bool DidPlayerWinAndFindWinningLocations(int PlayerId)
        {
            if (EvalState.Board.FindPieces(PlayerId).Count < 4) return false;
            //if (!PossibleToWin(PlayerId)) return false;

            for (var row = 0; row < EvalState.Board.Rows; row++)
            {
                for (var col = 0; col < EvalState.Board.Columns; col++)
                {
                    foreach (WinDirection Direction in Enum.GetValues(typeof(WinDirection))){
                        switch (Direction)
                        {
                            case WinDirection.HORIZONTAL:
                                if (EvalState.Board.Columns - col < 4) continue; break;
                            case WinDirection.VERTICAL:
                                if (EvalState.Board.Rows - row < 4) continue; break;
                            case WinDirection.DIAGONAL_NE_SW:
                                if (EvalState.Board.Columns - col < 4) continue;
                                if (row < EvalState.Board.Rows - 4 - 1) continue; break;
                            case WinDirection.DIAGONAL_NW_SE:
                                if (EvalState.Board.Columns - col < 4) continue;
                                if (EvalState.Board.Rows - row < 4) continue; break;

                        }
                        List<BoardLocation> WinningLocations = CheckForWin(new BoardLocation(row, col), Direction, PlayerId);
                        if (WinningLocations != null)
                        {
                            EvalState.WinningLocations = new List<BoardLocation>();
                            foreach(BoardLocation l in WinningLocations)
                            {
                                EvalState.WinningLocations.Add(l);
                            }
                            EvalState.WinnerId = PlayerId;
                            
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool EvaluateDraw()
        {
            int count = 0;
            foreach (int p in EvalState.Players.Keys)
            {
                if (EvaluateDraw(p)) count++;
            }

            //draw because both sides have a winning condition.
            if (count > 1)
            {
                EvalState.WinnerId = 0;
            }
            return (count > 0);
        }

        public bool EvaluateDraw(int PlayerId)
        {
            if (!IsAvailableSimpleMove(PlayerId)) return true;
            return false;
        }

        public List<BoardLocation> CheckForWin(BoardLocation Location, WinDirection Direction, int PlayerId)
        {
            int count = 0;
            int i = 0;
            BoardLocation loc; 
            List<BoardLocation> WinningLocations = new List<BoardLocation>();
            switch (Direction)
                {
                    case WinDirection.HORIZONTAL:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while ( loc.OnBoard(EvalState.Board)){

                        if (EvalState.Board.ContentsAt(loc).Control == PlayerId){
                            count++;
                            i++;
                            WinningLocations.Add(loc);
                            loc = new BoardLocation(Location.Row, Location.Column + i);
                        } else
                        {
                            if (count < 4) return null; 
                            if (count >= 4) return WinningLocations;
                        }
                    }
                    break;

                    case WinDirection.VERTICAL:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(EvalState.Board))
                    {

                        if (EvalState.Board.ContentsAt(loc).Control == PlayerId){
                            count++;
                            i++;
                            WinningLocations.Add(loc);
                            loc = new BoardLocation(Location.Row + i, Location.Column);
                        }
                        else
                        {
                            if (count < 4) return null;
                            if (count >= 4) return WinningLocations;
                        }
                    }

                    break;
                    case WinDirection.DIAGONAL_NE_SW:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(EvalState.Board))
                    {

                        if (EvalState.Board.ContentsAt(loc).Control == PlayerId)
                        {
                            count++;
                            i++;
                            WinningLocations.Add(loc);
                            loc = new BoardLocation(Location.Row - i, Location.Column +i );
                        }
                        else
                        {
                            if (count < 4) return null;
                            if (count >= 4) return WinningLocations;
                        }
                    }
                    break;


                case WinDirection.DIAGONAL_NW_SE:
                    loc = new BoardLocation(Location.Row, Location.Column);
                    while (loc.OnBoard(EvalState.Board))
                    {

                        if (EvalState.Board.ContentsAt(loc).Control == PlayerId)
                        {
                            count++;
                            i++;
                            WinningLocations.Add(loc);
                            loc = new BoardLocation(Location.Row + i, Location.Column + i);
                        }
                        else
                        {
                            if (count < 4) return null;
                            if (count >= 4) return WinningLocations;
                        }
                    }
                    break;

            }

            if (count >= 4) return WinningLocations;
            return null;
        }
            
        
        #endregion

        public void RecordAction(GameAction Action)
        {
            if (ResultActions == null) ResultActions = new List<GameAction>();
            ResultActions.Add(Action);
        }
    }
}
