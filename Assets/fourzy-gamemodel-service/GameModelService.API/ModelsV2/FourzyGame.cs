using System;
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

        #region "Constructors"

        //Primarily Used for Testing.  Should Phase this Out.
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

        //For 2 Player Games        
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

        // Select a Area and create a random board.
        // Active Player Id is set to -1 as the first player is not known when the game is created
        public FourzyGame(Area Area, Player Player1, Player Player2, int FirstPlayerId = -1, GameOptions Options = null)
        {
            if (Options == null) Options = new GameOptions();

            GameBoard Board = BoardFactory.CreateRandomBoard(Options, Player1, Player2, Area);

            switch (Player2.DisplayName)
            {
                case "Easy":
                    Player2.Profile = AIPlayerFactory.RandomProfile(AIDifficulty.Easy);
                    this.State = new GameState(Board, Options, FirstPlayerId);
                    break;

                case "Normal":
                    Player2.Profile = AIPlayerFactory.RandomProfile(AIDifficulty.Medium);
                    this.State = new GameState(Board, Options, FirstPlayerId);
                    break;

                case "Hard":
                    Player2.Profile = AIPlayerFactory.RandomProfile(AIDifficulty.Hard);
                    this.State = new GameState(Board, Options, FirstPlayerId);
                    break;

                case "Doctor":
                    Player2.Profile = AIProfile.DoctorBot;
                    this.State = new GameState(Board, Options, FirstPlayerId);
                    break;

                case "Practice":
                    Player2.Profile = AIProfile.PassBot;
                    this.State = new GameState(Board, Options, FirstPlayerId);
                    break;

                case "Entryway":
                    BossType MyBoss = BossType.EntryWay;
                    Board = BossFactory.CreateBoard(MyBoss);
                    this.State = new GameState(Board, Options, FirstPlayerId);
                    IBoss Boss = BossFactory.Create(MyBoss);
                    Boss.StartGame(this.State);
                    Player2.BossType = MyBoss;
                    Player2.Profile = AIProfile.BossAI;
                    Player2.SpecialAbilityCount = 1;
                    break;

                case "DirectionMaster":
                    BossType MyBoss2 = BossType.DirectionMaster;
                    Board = BossFactory.CreateBoard(MyBoss2);
                    this.State = new GameState(Board, Options, FirstPlayerId);
                    IBoss Boss2 = BossFactory.Create(MyBoss2);
                    Boss2.StartGame(this.State);
                    Player2.BossType = MyBoss2;
                    Player2.Profile = AIProfile.BossAI;
                    Player2.SpecialAbilityCount = 1;
                    break;

                case "LordOfGoop":
                    BossType MyBoss3 = BossType.LordOfGoop;
                    Board = BossFactory.CreateBoard(MyBoss3);
                    this.State = new GameState(Board, Options, FirstPlayerId);
                    IBoss Boss3 = BossFactory.Create(MyBoss3);
                    Boss3.StartGame(this.State);
                    Player2.BossType = MyBoss3;
                    Player2.Profile = AIProfile.BossAI;
                    Player2.SpecialAbilityCount = 1;
                    break;

                default:
                    this.State = new GameState(Board, Options, FirstPlayerId);
                    break;

            }


            //else if (Player2.DisplayName == "UFO")
            //{
            //    BossType MyBoss = BossType.Necrodancer;
            //    Board = BossFactory.CreateBoard(MyBoss);
            //    this.State = new GameState(Board, Options, FirstPlayerId);
            //    IBoss Boss = BossFactory.Create(MyBoss);
            //    Boss.StartGame(this.State);
            //    Player2.BossType = MyBoss;
            //    Player2.Profile = AIProfile.BossAI;
            //    Player2.SpecialAbilityCount = 1;
            //}

       

            this.State.Players.Add(1, Player1);
            this.State.Players.Add(2, Player2);

            this.playerTurnRecord = new List<PlayerTurn>();
            this.GameType = GameType.STANDARD;
        }

        // Pass in a Board
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

        //Pass in a Board Definition.
        public FourzyGame(GameBoardDefinition definition, Player Player1, Player Player2, int FirstPlayerId = 1, GameOptions Options = null)
        {
            this.State = new GameState(definition);
            this.State.ActivePlayerId = FirstPlayerId;
            this.playerTurnRecord = new List<PlayerTurn>();
            this.State.Players.Add(1, Player1);
            this.State.Players.Add(2, Player2);
            this.GameType = GameType.STANDARD;
            if (Options != null) this.State.Options = Options;
        }

        //Pass in a Json String.
        public FourzyGame(string boardJson)
        {
            GameBoardDefinition gbd = JsonConvert.DeserializeObject<GameBoardDefinition>(boardJson);
            this.State = new GameState(gbd);
            this.playerTurnRecord = new List<PlayerTurn>();
            this.State.Players.Add(1, new Player(1, "First"));
            this.State.Players.Add(2, new Player(2, "Second"));
            this.GameType = GameType.STANDARD;
        }
        
        //BOSSES

        //Create a boss match in an selected Area.
        public FourzyGame(Area Area, BossType Boss, Player Player, GameOptions Options = null)
        {
            this.State = BossGameFactory.CreateBossGame(Area, Boss, Player, Options);
            this.playerTurnRecord = new List<PlayerTurn>();
            this.GameType = GameType.AI;
        }

        //Create a boss match with a random area board.
        public FourzyGame(Area Area, AIProfile Profile, Player Player, int FirstPlayerId=1, GameOptions Options = null)
        {
            if (Options == null) Options = new GameOptions();
            Player AI = new Player(2, Profile.ToString(), Profile);

            GameBoard Board = BoardFactory.CreateRandomBoard(Options, Player, AI, Area);
            this.State = new GameState(Board, Options, FirstPlayerId);
            this.State.Players.Add(1, Player);
            this.State.Players.Add(2, AI);

            this.playerTurnRecord = new List<PlayerTurn>();
            this.GameType = GameType.AI;
        }

        //Create a boss match with a specific board.
        public FourzyGame(GameBoardDefinition definition, BossType Boss, Player Player, GameOptions Options = null)
        {
            this.State = BossGameFactory.CreateBossGame(definition, Boss, Player, Options);
            this.playerTurnRecord = new List<PlayerTurn>();
            this.GameType = GameType.AI;
        }

        //AI GAMES

        public FourzyGame(GameBoardDefinition definition, AIProfile Profile, Player Player1, int FirstPlayerId = 1, Player AI = null)
        {
            this.State = new GameState(definition);
            this.State.ActivePlayerId = FirstPlayerId;
            this.playerTurnRecord = new List<PlayerTurn>();
            this.State.Players.Add(1, Player1);
            if (AI == null)
                AI = new Player(2, Profile.ToString(), Profile);
            this.State.Players.Add(2, AI);
            this.GameType = GameType.STANDARD;
        }

        //THE GAUNTLET

        public FourzyGame(Player Human, int GauntletLevel, Area CurrentArea = Area.NONE, int DifficultModifier = -1, int membersCount = 999, GameOptions Options = null)
        {
            string SeedString = Guid.NewGuid().ToString();


            //this.State = BossGameFactory.CreateBossGame(CurrentArea, BossType.DirectionMaster, new Player(1,"Player"), Options);
            //this.playerTurnRecord = new List<PlayerTurn>();
            //this.GameType = GameType.AI;


            this.State = GauntletFactory.Create(Human, GauntletLevel, /*Status*/membersCount, CurrentArea, DifficultModifier, Options, SeedString);
            this.playerTurnRecord = new List<PlayerTurn>();
            this.GameType = GameType.AI;
        }


        #endregion "Constructors"



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
