using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public class GameState
    {
        //INTERNAL DATA
        public GameBoard Board;
        public List<IGameEffect> GameEffects;
        public Dictionary<int, Player> Players;
        public Dictionary<int, Herd> Herds;
        public Dictionary<int, ISpell> ActiveSpells;
        public bool ProcessStartOfTurn = false;
        public GameOptions Options;

        [JsonIgnore]
        public List<BoardLocation> ActiveSpaces;

        //INITAL DATA AND PREFERENCES
        //A unique identifier used for repeating a random generation
        public string GameSeed { get; set; }

        //A unique guid associated with a particular game
        public string UniqueId { get; private set; }

        //Is the game realtime or not.
        public Boolean RealTime;

        //ACTIVE DATA 
        //Amount of Time Left
        public Dictionary<int, int> Time;

        //Expecting a move of this player
        public int ActivePlayerId { get; set; }

        //How many turns have been taken.
        public int TurnCount { get; set; }

        //Winner will be set to 0 if there is a draw.
        public int WinnerId { get; set; }

        //What locations are the winning pieces
        public List<BoardLocation> WinningLocations { get; set; }

        //This might include some other things eventually including GameEffects, ActiveSpells
        public string StateString { get { return Board.ContentString; } }

        public string CompressedString { get { return Compression.CompressString(StateString); } }

        public RandomTools Random { get; set; }

        [JsonIgnore]
        public Action<GameAction> RecordGameAction;

        public GameStateData SerializeData()
        {
            GameStateData data = new GameStateData
            {
                GameSeed = this.GameSeed,
                Herds = this.Herds,
                Players = this.Players,
                WinnerId = this.WinnerId,
                WinningLocations = this.WinningLocations,
                GameBoardData = this.Board.SerializeData(),
                GameEffects = this.GameEffects,
                ActivePlayerId = this.ActivePlayerId
            };

            return data;
        }

        //Constructors and Initialization

        #region "Constructors"
        public GameState(GameStateData gameStateData)
        {
            this.GameSeed = gameStateData.GameSeed;
            this.Herds = gameStateData.Herds;

            //need a copy of players dictionary
            this.Players = new Dictionary<int, Player>();
            foreach (KeyValuePair<int, Player> record in gameStateData.Players)
                this.Players.Add(record.Key, new Player(record.Value));

            this.WinnerId = gameStateData.WinnerId;
            this.WinningLocations = gameStateData.WinningLocations;

            this.Board = new GameBoard(gameStateData.GameBoardData);
            this.GameEffects = gameStateData.GameEffects;

            this.Board.Parent = this;
            // activePlayerId of -1 means the first player is not yet known
            this.ActivePlayerId = gameStateData.ActivePlayerId;
            this.RealTime = false;
            this.UniqueId = Guid.NewGuid().ToString();
            this.Time = new Dictionary<int, int>();

            this.Random = new RandomTools(this);
            this.ActiveSpaces = new List<BoardLocation>();

            this.Options = new GameOptions();
        }

        public GameState(GameState OriginalState)
        {
            this.Board = new GameBoard(OriginalState.Board);
            //this.Board.RecordGameAction += RecordGameAction;

            this.GameEffects = new List<IGameEffect>();
            foreach (IGameEffect e in OriginalState.GameEffects)
            {
                e.Parent = this;
                this.GameEffects.Add(e);
            }
            this.Players = new Dictionary<int, Player>();
            foreach (int p in OriginalState.Players.Keys)
            {
                this.Players.Add(p, new Player(OriginalState.Players[p]));
            }
            this.Herds = new Dictionary<int, Herd>();
            foreach (int key in OriginalState.Herds.Keys)
            {
                this.Herds.Add(key, new Herd(OriginalState.Herds[key]));
            }
            this.RealTime = OriginalState.RealTime;
            this.Time = new Dictionary<int, int>();
            foreach (int p in OriginalState.Time.Keys)
            {
                this.Time.Add(p, OriginalState.Time[p]);
            }
            this.GameSeed = OriginalState.GameSeed;
            this.Random = OriginalState.Random;
            this.ActivePlayerId = OriginalState.ActivePlayerId;
            this.UniqueId = Guid.NewGuid().ToString();
            this.ActivePlayerId = OriginalState.ActivePlayerId;
            this.WinnerId = OriginalState.WinnerId;
            this.ActiveSpaces = new List<BoardLocation>();
            foreach (BoardLocation l in OriginalState.ActiveSpaces)
            {
                ActiveSpaces.Add(l);
            }

            this.Options = OriginalState.Options;
        }

        public GameState()
        {
            this.Board = BoardFactory.CreateDefaultBoard(8, 8);
            this.Board.Parent = this;
            this.ActivePlayerId = 1;
            this.RealTime = false;
            Initialize();
            //this.Players.Add(1, new Player(1, "One"));
            //this.Players.Add(2, new Player(2, "Two"));
        }

        public GameState(int Rows, int Columns, int FirstPlayerId = 1)
        {
            this.Board = BoardFactory.CreateDefaultBoard(Rows, Columns);
            this.Board.Parent = this;
            this.RealTime = false;
            Initialize();
            //this.Players.Add(1, new Player(1, "One"));
            //this.Players.Add(2, new Player(2, "Two"));

            this.ActivePlayerId = FirstPlayerId;
        }

        public GameState(GameBoard Board, GameOptions Options, int FirstPlayerId = 1)
        {
            this.Board = Board;
            this.Board.Parent = this;

            this.RealTime = false;
            Initialize();
            this.Board.Random = this.Random;
            this.Options = Options;
            this.ActivePlayerId = FirstPlayerId;
        }

        public GameState(GameBoardDefinition definition, int FirstPlayerId = 1)
        {
            this.Board = new GameBoard(definition);
            this.Board.Parent = this;
            this.RealTime = false;
            Initialize();
            this.Board.Random = this.Random;

            this.ActivePlayerId = FirstPlayerId;
        }

        //TO DO: Build a method to take a string and build a game state.
        //public GameState(string GameStateString, bool Compressed = false)
        //{
        //    if (Compressed) GameStateString = Compression.DecompressString(GameStateString);

        //    //Need to write code to parse GameStateString

        //    //this.GameSeed = gameStateData.GameSeed;
        //    //this.Herds = gameStateData.Herds;
        //    //this.Players = gameStateData.Players;
        //    //this.WinnerId = gameStateData.WinnerId;
        //    //this.WinningLocations = gameStateData.WinningLocations;

        //    //this.Board = new GameBoard(gameStateData.GameBoardData);
        //    //this.GameEffects = gameStateData.GameEffects;

        //    //this.Board.Parent = this;
        //    //// activePlayerId of -1 means the first player is not yet known
        //    //this.ActivePlayerId = gameStateData.ActivePlayerId;
        //    //this.RealTime = false;
        //    //this.UniqueId = Guid.NewGuid().ToString();
        //    //this.Time = new Dictionary<int, int>();

        //    //this.Random = new RandomTools(this);
        //}


        private void Initialize()
        {
            this.WinnerId = -1;
            this.GameEffects = new List<IGameEffect>();
            this.Players = new Dictionary<int, Player>();
            this.Time = new Dictionary<int, int>();
            this.GameSeed = Guid.NewGuid().ToString();
            this.UniqueId = Guid.NewGuid().ToString();
            this.Random = new RandomTools(this);
            this.ActiveSpaces = new List<BoardLocation>();
            this.Herds = new Dictionary<int, Herd>();
            this.Options = new GameOptions();
        }

        public void InitializeHerd(int PlayerId, int HerdId, int HerdCount)
        {
            Herds[PlayerId] = new Herd(HerdId, HerdCount);
        }

        public void InitializeHerd(int PlayerId, int HerdCount)
        {
            int HerdId = 1;
            try
            {
                HerdId = int.Parse(Players[PlayerId].HerdId);
            }
            catch { }
            Herds[PlayerId] = new Herd(HerdId, HerdCount);
        }

        #endregion "Constructors"

        public PlayerTurnResult TakeTurn(PlayerTurn Turn)
        {
            TurnEvaluator ME = new TurnEvaluator(this);
            PlayerTurnResult result = new PlayerTurnResult();
            result.GameState = ME.EvaluateTurn(Turn);
            result.Activity = ME.ResultActions;

            return result;
        }


        #region "Events"

        //Event Recorder
        public void SetActionRecorder(Action<GameAction> ActionRecorder)
        {
            this.RecordGameAction += ActionRecorder;
            this.Board.RecordGameAction += ActionRecorder;
        }

        public void StartOfTurn(int PlayerId)
        {
            foreach (IGameEffect e in GameEffects) e.StartOfTurn(PlayerId);
            Board.StartOfTurn(PlayerId);
            this.ProcessStartOfTurn = true;
            this.ActiveSpaces.Clear();
        }

        public void PieceBumpsIntoLocation(MovingPiece Piece, BoardLocation Location)
        {
            foreach (IGameEffect e in GameEffects) e.PieceBumpsIntoLocation(Piece, Location);

            Board.PieceBumpsIntoLocation(Piece, Location);
        }

        public void PieceStopsOnSpace(MovingPiece Piece)
        {
            foreach (IGameEffect e in GameEffects) e.PieceStopsOnSpace(Piece);
            Piece.RemoveCondition(PieceConditionType.INERTIA);
            Board.PieceStopsOnSpace(Piece);
            if (Piece.PlayerId == ActivePlayerId) ActiveSpaces.Add(Piece.Location);
        }

        public void EndOfTurn(int PlayerId)
        {
            foreach (IGameEffect e in GameEffects) e.EndOfTurn(PlayerId);
            Board.EndOfTurn(PlayerId);
            var newActivePlayerId = this.Opponent(this.ActivePlayerId);
            this.ActivePlayerId = newActivePlayerId;
            this.ProcessStartOfTurn = false;
        }

        #endregion

        #region "Helper Functions"

        public int Opponent(int PlayerId)
        {
            foreach (var player in Players.Values)
            {
                if (player.PlayerId != PlayerId) return player.PlayerId;
            }
            return 0;
        }

        #endregion "Helper Functions"
    }
}
