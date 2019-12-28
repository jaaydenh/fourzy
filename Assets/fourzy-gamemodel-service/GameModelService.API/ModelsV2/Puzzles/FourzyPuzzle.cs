using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class FourzyPuzzle
    {
        public GameState FirstState;
        public GameState LastState;
        public GameState State;
        public AIProfile AIProfile;

        public List<GameAction> GameActions;
        public List<PlayerTurn> playerTurnRecord;

        //  public enum PuzzleGoalType { WIN, SURVIVE, COVER}
        //  WIN = make moves to win in X turns
        //  SURVIVE = make moves to not lose or draw in X turns
        //  COVER = make moves to place a fourzy in a specific location
        
        public PuzzleGoalType GoalType;

        // If the player does not succeed in this many turns, the challenge is a failure.
        public int MoveLimit;

        // A list of available hints.  Each challenge may have different amounts and types of hints.
        public List<PuzzleHint> AvailableHints;

        //Information on what the computer should do at each stage
        // CurrentState, Instructions
        public Dictionary<string, PuzzleTurnInstructions> AILibrary { get; set; }
        
        public PuzzleStatus Status { get; set; }

        public FourzyPuzzle(PuzzleData Data, Player Player1)
        {
            GameOptions go = new GameOptions();
            go.Rows = Data.InitialGameBoard.Rows;
            go.Columns= Data.InitialGameBoard.Columns;

            GameBoard gameBoard = new GameBoard(Data.InitialGameBoard);

            this.State = new GameState(gameBoard, go);
            this.State.Players.Add(1, Player1);
            this.State.Players.Add(2, Data.PuzzlePlayer);

            this.FirstState = State;

            this.GoalType = Data.GoalType;
            this.MoveLimit = Data.MoveLimit;
            foreach (PuzzleHint h in Data.AvailableHints)
            {
                AvailableHints.Add(h);
            }
            this.Status = PuzzleStatus.ACTIVE;

            initialize();
        }

        private void initialize()
        {
            playerTurnRecord = new List<PlayerTurn>();
        }


        public virtual PlayerTurnResult TakeTurn(PlayerTurn Turn, bool ReturnStartOfNextTurn = false)
        {

            if (Turn.PlayerId == 0 && Turn.PlayerString.Length > 0)
            {
                foreach (Player p in State.Players.Values)
                {
                    if (Turn.PlayerString == p.PlayerString) { Turn.PlayerId = p.PlayerId; break; }
                }
            }

            LastState = State;
            TurnEvaluator ME = new TurnEvaluator(State);
            State = ME.EvaluateTurn(Turn);
            GameActions = ME.ResultActions;
            playerTurnRecord.Add(Turn);

            if (State.WinnerId < 0 && playerTurnRecord.Count >= MoveLimit)
            {
                Status = PuzzleStatus.FAILED;
                GameActions.Add(new GameActionPuzzleStatus(PuzzleStatus.FAILED, PuzzleEvent.NOMOREMOVES));
            }

            if (ReturnStartOfNextTurn && State.WinnerId < 0)
            {
                ME = new TurnEvaluator(State);
                GameState StartState = ME.EvaluateStartOfTurn();
                GameActions.AddRange(ME.ResultActions);
                return new PlayerTurnResult(StartState, GameActions);
            }

            if (State.WinnerId > 0 && State.WinnerId == State.Opponent(Turn.PlayerId))
            {
                Status = PuzzleStatus.FAILED;
                GameActions.Add(new GameActionPuzzleStatus(PuzzleStatus.FAILED, PuzzleEvent.LOSS));
            }

            if (State.WinnerId > 0 && State.WinnerId == Turn.PlayerId)
            {
                Status = PuzzleStatus.SUCCESS;
                GameActions.Add(new GameActionPuzzleStatus(PuzzleStatus.SUCCESS, PuzzleEvent.VICTORY));
            }

            return new PlayerTurnResult(State, GameActions, Turn);
        }

        public virtual PlayerTurnResult TakeAITurn(bool ReturnStartOfNextTurn = false)
        {
            PlayerTurnResult AIResult = new PlayerTurnResult();

            LastState = State;
            TurnEvaluator turnEvaluator = new TurnEvaluator(State);

            AIPlayer AI = AI = new PuzzleAI(State);
            turnEvaluator = new TurnEvaluator(State);

            AIResult.Turn = AI.GetTurn();
            AIResult.GameState = turnEvaluator.EvaluateTurn(AIResult.Turn);
            AIResult.Activity = turnEvaluator.ResultActions;
            State = AIResult.GameState;
            GameActions = turnEvaluator.ResultActions;

            if (State.WinnerId < 0 && ReturnStartOfNextTurn)
            {
                turnEvaluator = new TurnEvaluator(State);
                GameState StartState = turnEvaluator.EvaluateStartOfTurn();
                GameActions.AddRange(turnEvaluator.ResultActions);
                AIResult.GameState = StartState;
                AIResult.Activity = GameActions;
            }

            //if (State.WinnerId > 0 && State.WinnerId != State.Opponent(AIResult.Turn.PlayerId))
            //{
            //    Status = PuzzleStatus.FAILED;
            //    GameActions.Add(new GameActionPuzzleStatus(PuzzleStatus.FAILED, PuzzleEvent.LOSS));
            //}

            //if (State.WinnerId > 0 && State.WinnerId == State.Opponent(AIResult.Turn.PlayerId))
            //{ Status = PuzzleStatus.SUCCESS;
            //    GameActions.Add(new GameActionPuzzleStatus(PuzzleStatus.SUCCESS, PuzzleEvent.VICTORY));
            //}
            
            return AIResult;
        }

        //public virtual PlayerTurnResult TakeTurn(PlayerTurn Turn)
        //{

        //    MoveEvaluator ME = new MoveEvaluator(State);
        //    GameState PlayerState = ME.EvaluateTurn(Turn);
        //    List<GameAction> PlayerGameActions = ME.ResultActions;

        //    PlayerTurnResult PuzzleResult = new PlayerTurnResult();
        //    AIMoveEvaluator AI = new AIMoveEvaluator(PlayerState);
        //    PuzzleResult.Turn = new PlayerTurn(AI.GetRandomOkMove());

        //    State = PlayerState;

        //    ME = new MoveEvaluator(State);
        //    PuzzleResult.GameState = ME.EvaluateTurn(PuzzleResult.Turn);
        //    PuzzleResult.Activity = ME.ResultActions;

        //    State = PuzzleResult.GameState;

        //    return PuzzleResult;
        //}

        public PlayerTurnResult StartTurn(GameState GameState)
        {
            TurnEvaluator ME = new TurnEvaluator(GameState);
            GameState StartState = ME.EvaluateStartOfTurn();

            return new PlayerTurnResult(StartState, ME.ResultActions);
        }

        public virtual GameState Reset()
        {
            State = FirstState;
            initialize();
            this.Status = PuzzleStatus.ACTIVE;

            return State;
        }

    }
}
