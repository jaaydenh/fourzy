using System.Collections.Generic;
using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public class FourzyGame
    {
        public GameState LastState;
        public GameState State;
        public List<GameAction> GameActions;
        public List<PlayerTurn> playerTurnRecord;
        public GameType GameType;

        public FourzyGame()
        {
            this.State = new GameState(8, 8);
            this.playerTurnRecord = new List<PlayerTurn>();
            this.State.Players.Add(1, new Player(1, "First"));
            this.State.Players.Add(2, new Player(2, "Second"));
            this.GameType = GameType.STANDARD;
        }

        public FourzyGame(GameState gameState)
        {
            this.State = new GameState(gameState);
            this.playerTurnRecord = new List<PlayerTurn>();
            this.GameType = GameType.STANDARD;
        }

        public FourzyGame(GameStateData gameStateData)
        {
            this.State = new GameState(gameStateData);
            this.playerTurnRecord = new List<PlayerTurn>();
            this.GameType = GameType.STANDARD;
        }

        public FourzyGame(Player Player1, Player Player2, GameOptions Options = null)
        {
            if (Options == null) Options = new GameOptions();

            GameBoard Board = BoardFactory.CreateRandomBoard(Options, Player1, Player2);

            this.State = new GameState(Board, Options);
            this.playerTurnRecord = new List<PlayerTurn>();
            this.State.Players.Add(1, Player1);
            this.State.Players.Add(2, Player2);
            this.GameType = GameType.STANDARD;
        }

        // Active Player Id is set to -1 as the first player is not known when the game is created
        public FourzyGame(Area Area, Player Player1, Player Player2, int FirstPlayerId = -1, GameOptions Options = null)
        {
            if (Options == null) Options = new GameOptions();

            GameBoard Board = BoardFactory.CreateRandomBoard(Options, Player1, Player2, Area);

            if (Player2.DisplayName == "Pod")
            {
                Player2.Profile = AIProfile.BeginnerAI;
                this.State = new GameState(Board, Options, FirstPlayerId);
            }

            else if (Player2.DisplayName == "TiVo")
            {
                Player2.Profile = AIProfile.EasyAI;
                this.State = new GameState(Board, Options, FirstPlayerId);
            }

            else if (Player2.DisplayName == "UFO")
            {
                //BossType MyBoss = BossType.Necrodancer;
                //Board = BossFactory.CreateBoard(MyBoss);
                //this.State = new GameState(Board, Options, FirstPlayerId);
                //IBoss Boss = BossFactory.Create(MyBoss);
                //Boss.StartGame(this.State);
                //Player2.BossType = MyBoss;
                //Player2.Profile = AIProfile.BossAI;
                //Player2.SpecialAbilityCount = 1;

                Player2.Profile = AIProfile.SimpleAI;
                this.State = new GameState(Board, Options, FirstPlayerId);
            }
            else
            {
                this.State = new GameState(Board, Options, FirstPlayerId);
            }

            this.State.Players.Add(1, Player1);
            this.State.Players.Add(2, Player2);

            this.playerTurnRecord = new List<PlayerTurn>();
            this.GameType = GameType.STANDARD;
        }

        // Active Player Id is set to -1 as the first player is not known when the game is created
        public FourzyGame(GameBoard Board, Player Player1, Player Player2, int FirstPlayerId = -1, GameOptions Options = null)
        {
            if (Options == null) Options = new GameOptions();

            this.State = new GameState(Board, Options, FirstPlayerId);
            this.playerTurnRecord = new List<PlayerTurn>();
            this.State.Players.Add(1, Player1);
            this.State.Players.Add(2, Player2);
            this.GameType = GameType.STANDARD;
        }

        public FourzyGame(GameBoardDefinition definition, Player Player1, Player Player2, int FirstPlayerId = 1)
        {
            this.State = new GameState(definition);
            this.State.ActivePlayerId = FirstPlayerId;
            this.playerTurnRecord = new List<PlayerTurn>();
            this.State.Players.Add(1, Player1);
            this.State.Players.Add(2, Player2);
            this.GameType = GameType.STANDARD;
        }

        public FourzyGame(string boardJson)
        {
            GameBoardDefinition gbd = JsonConvert.DeserializeObject<GameBoardDefinition>(boardJson);
            this.State = new GameState(gbd);
            this.playerTurnRecord = new List<PlayerTurn>();
            this.State.Players.Add(1, new Player(1, "First"));
            this.State.Players.Add(2, new Player(2, "Second"));
            this.GameType = GameType.STANDARD;
        }
        
        //Create a boss match with a random area board.
        public FourzyGame(Area Area, BossType Boss, Player Player)
        {
            this.State = BossGameFactory.CreateBossGame(Area, Boss, Player);
            this.playerTurnRecord = new List<PlayerTurn>();
            this.GameType = GameType.AI;
        }

        //Create a boss match with a specific board.
        public FourzyGame(GameBoardDefinition definition, BossType Boss, Player Player)
        {
            this.State = BossGameFactory.CreateBossGame(definition, Boss, Player);
            this.playerTurnRecord = new List<PlayerTurn>();
            this.GameType = GameType.AI;
        }

        public FourzyGame(GameBoardDefinition definition, AIProfile Profile, Player Player1, int FirstPlayerId = 1)
        {
            this.State = new GameState(definition);
            this.State.ActivePlayerId = FirstPlayerId;
            this.playerTurnRecord = new List<PlayerTurn>();
            this.State.Players.Add(1, Player1);
            Player AI = new Player(2, Profile.ToString(), Profile);
            this.State.Players.Add(2, AI);
            this.GameType = GameType.STANDARD;
        }

        public virtual PlayerTurnResult TakeTurn(PlayerTurn Turn, bool ReturnStartOfNextTurn =false )
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

            if (ReturnStartOfNextTurn)
            {
                ME = new TurnEvaluator(State);
                GameState StartState = ME.EvaluateStartOfTurn();
                GameActions.AddRange(ME.ResultActions);
                return new PlayerTurnResult(StartState, GameActions);
            }


            return new PlayerTurnResult(State, GameActions);
        }

        public PlayerTurnResult StartTurn(GameState GameState)
        {
            TurnEvaluator ME = new TurnEvaluator(GameState);
            GameState StartState = ME.EvaluateStartOfTurn();
           
            return new PlayerTurnResult(StartState, ME.ResultActions);
        }

        public virtual PlayerTurnResult TakeAITurn(bool ReturnStartOfNextTurn = false)
        {
            PlayerTurnResult AIResult = new PlayerTurnResult();

            LastState = State;
            TurnEvaluator ME = new TurnEvaluator(State);

            AIProfile Profile = State.Players[State.ActivePlayerId].Profile;
            BossType Boss = State.Players[State.ActivePlayerId].BossType;

            AIPlayer AI = null;
            if (Boss != BossType.None)
               AI = new BossAI(State, Boss);
            else
               AI = AIPlayerFactory.Create(State, Profile);

            if (AI is null) AI = new SimpleAI(State);

            //AIPlayer AI  = new SimpleAI(State);
            //switch (State.Players[State.ActivePlayerId].DisplayName)
            //{
            //    case "Pod":
            //        AI = new TestBossAI(State, BossType.EntryWay);
            //        break;

            //    case "UFO":
            //        AI = new BetterAI(State);
            //        break;
            //}

            ME = new TurnEvaluator(State);

            AIResult.Turn = AI.GetTurn();
            AIResult.GameState = ME.EvaluateTurn(AIResult.Turn);
            AIResult.Activity = ME.ResultActions;
            State = AIResult.GameState;
            GameActions = new List<GameAction>();
            GameActions.AddRange(ME.ResultActions);

            if (ReturnStartOfNextTurn)
            {
                ME = new TurnEvaluator(State);
                GameState StartState = ME.EvaluateStartOfTurn();
                GameActions.AddRange(ME.ResultActions);
                AIResult.GameState = StartState;
            }
            AIResult.Activity = GameActions;
            playerTurnRecord.Add(AIResult.Turn);

            return AIResult;
        }


        //public AITurnResult TakeAITurn(PlayerTurn Turn)
        //{
        //    // if passed in a string, convert it to 
        //    if (Turn.PlayerId == 0 && Turn.PlayerString.Length > 0)
        //    {
        //        foreach (Player p in State.Players.Values)
        //        {
        //            if (Turn.PlayerString == p.PlayerString) { Turn.PlayerId = p.PlayerId; break; }
        //        }
        //    }

        //    AITurnResult Result = new AITurnResult();

        //    LastState = State;
        //    MoveEvaluator ME = new MoveEvaluator(State);

        //    Result.AfterPlayerMove.GameState = ME.EvaluateTurn(Turn);
        //    Result.AfterPlayerMove.Activity = ME.ResultActions;
        //    Result.PlayerTurn = Turn;

        //    SimpleAI AI = new SimpleAI(Result.AfterPlayerMove.GameState);
        //    ME = new MoveEvaluator(Result.AfterPlayerMove.GameState);
        //    Result.AITurn = AI.GetTurn();
        //    Result.AfterAIMove.GameState = ME.EvaluateTurn(Result.AITurn);
        //    Result.AfterAIMove.Activity = ME.ResultActions;
            
        //    State = Result.AfterAIMove.GameState;
        //    GameActions.Clear();
        //    GameActions.AddRange(Result.AfterPlayerMove.Activity);
        //    GameActions.AddRange(Result.AfterAIMove.Activity);
            
        //    return Result;
        //}
    }
}
