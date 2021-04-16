//@vadym udod

using ExitGames.Client.Photon;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Rewards;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.ClientModel
{
    public class ClientFourzyGame : FourzyGame, IClientFourzy
    {
        public Action<int, int> onMagic { get; set; }

        public GameState _State
        {
            get => State;
            set => State = value;
        }
        public GameState _FirstState { get; set; }

        public List<PlayerTurn> _playerTurnRecord => playerTurnRecord;
        public List<PlayerTurn> _allTurnRecord { get; set; }
        public Dictionary<int, int> magic { get; set; }

        public bool isBoardRandom { get; private set; } = false;

        private int _originalHerdCount;

        public Area _Area
        {
            get
            {
                if ((int)State.Board.Area > 1)
                {
                    return State.Board.Area;
                }
                else
                {
                    return InternalSettings.Current.DEFAULT_AREA;
                }
            }

            set { }
        }

        public RealtimeGameStateData toGameStateData => new RealtimeGameStateData(State.SerializeData());

        public GameType _Type
        {
            get => _type;

            set
            {
                _type = value;

                //force modify opponent HerdID for passplay games
                switch (_Type)
                {
                    case Fourzy.GameType.PASSANDPLAY:
                    case Fourzy.GameType.PRESENTATION:
                        switch (GameManager.Instance.characterType)
                        {
                            case GameManager.PassPlayCharactersType.SELECTED_RANDOM:
                                if (string.IsNullOrEmpty(opponent.HerdId))
                                {
                                    opponent.HerdId = GameContentManager.Instance.piecesDataHolder.random.Id;
                                }

                                break;

                            case GameManager.PassPlayCharactersType.RANDOM:
                                if (string.IsNullOrEmpty(me.HerdId))
                                {
                                    me.HerdId = GameContentManager.Instance.piecesDataHolder.random.Id;
                                }
                                if (string.IsNullOrEmpty(opponent.HerdId))
                                {
                                    opponent.HerdId = GameContentManager.Instance.piecesDataHolder.random.Id;
                                }

                                break;
                        }

                        break;
                }

                //adjust active player ID
                switch (_Type)
                {
                    case Fourzy.GameType.PASSANDPLAY:
                        //random active player
                        SetRandomActivePlayer();

                        break;
                }
            }
        }

        public GameMode _Mode
        {
            get
            {
                if (puzzleData)
                {
                    if (puzzleData.pack)
                    {
                        if (puzzleData.gauntletStatus != null)
                        {
                            return GameMode.GAUNTLET;
                        }
                        else
                        {
                            switch (puzzleData.pack.packType)
                            {
                                case PackType.AI_PACK:
                                    return GameMode.AI_PACK;

                                case PackType.BOSS_AI_PACK:
                                    return GameMode.BOSS_AI_PACK;
                            }
                        }
                    }
                }

                return GameMode.VERSUS;
            }

            set { }
        }

        public float initializedTime { get; set; }
        public string BoardID
        {
            get
            {
                if (string.IsNullOrEmpty(gameID))
                {
                    gameID = Guid.NewGuid().ToString();
                }

                return gameID;
            }

            set => gameID = value;
        }

        public bool isFourzyPuzzle => false;

        public List<Creature> myMembers => State.Herds[me.PlayerId].Members;

        public int BossMoves { get; set; }

        public int LoseStreak { get; set; }

        public GamePieceData playerOnePrefabData { get; set; }
        public GamePieceData playerTwoPrefabData { get; set; }

        private string gameID;
        private GameType _type;

        public List<SimpleMove> initialMoves { get; private set; } = new List<SimpleMove>();

        public List<PlayerTurn> InitialTurns
        {
            get
            {
                List<PlayerTurn> result = new List<PlayerTurn>();

                foreach (SimpleMove move in initialMoves)
                {
                    result.Add(new PlayerTurn(move));
                }

                return result;
            }
        }

        //turn base data
        public ChallengeData challengeData { get; set; }

        //realtime data
        public long createdEpoch { get; private set; }

        public List<RewardsManager.Reward> collectedItems { get; set; }

        public GamePieceView playerOneGamepiece
        {
            get
            {
                ClientFourzyHelper.AssignPrefabs(this);

                return playerOnePrefabData.player1Prefab;
            }
        }

        public GamePieceView playerTwoGamepiece
        {
            get
            {
                ClientFourzyHelper.AssignPrefabs(this);

                return playerOnePrefabData.Id == playerTwoPrefabData.Id ?
                    playerTwoPrefabData.player2Prefab :
                    playerTwoPrefabData.player1Prefab;
            }
        }

        public int Rows => State.Board.Rows;

        public int Columns => State.Board.Columns;

        public bool isOver =>
            State.WinnerId > 0 ||
            State.WinningLocations != null ||
            (puzzleData && puzzleData.pack && puzzleData.pack.gauntletStatus != null && myMembers.Count == 0);

        public bool draw { get; set; }

        public bool hideOpponent { get; set; } = false;

        public Piece activePlayerPiece => ClientFourzyHelper.ActivePlayerPiece(this);

        public Piece playerPiece => ClientFourzyHelper.PlayerPiece(this);

        public Piece opponentPiece => ClientFourzyHelper.OpponentPiece(this);

        public List<BoardSpace> boardContent => ClientFourzyHelper.BoardContent(this);

        public GamePieceView myGamePiece =>
            me.PlayerId == 1 ? playerOneGamepiece : playerTwoGamepiece;

        public GamePieceView opponentGamePiece =>
            opponent.PlayerId == 1 ? playerOneGamepiece : playerTwoGamepiece;

        public GamePieceView activePlayerGamePiece =>
            State.ActivePlayerId == 1 ? playerOneGamepiece : playerTwoGamepiece;

        public Player me => State.Players[1].PlayerString == UserManager.Instance.userId ?
                    State.Players[1] : State.Players[2];

        public Player opponent => ClientFourzyHelper.Other(this, me);

        public Player activePlayer => isMyTurn ? me : opponent;

        public Player unactivePlayer => isMyTurn ? opponent : me;

        public Player player1 => State.Players[(_FirstState == null ? _State : _FirstState).ActivePlayerId];

        public Player player2 => ClientFourzyHelper.Other(this, player1);

        public ClientPuzzleData puzzleData { get; set; }

        public bool isMyTurn => me.PlayerId == State.ActivePlayerId;

        public bool haveMoves => State.TurnCount > 0;

        public TurnEvaluator turnEvaluator => new TurnEvaluator(State);

        public float gameDuration => Time.time - initializedTime;

        public ClientFourzyGame asFourzyGame => this;

        public ClientFourzyPuzzle asFourzyPuzzle => null;

        public ClientFourzyGame() : base()
        {
            Initialize();
        }

        public ClientFourzyGame(GameState gameState) : base(gameState)
        {
            Initialize();
        }

        public ClientFourzyGame(GameStateData gameStateData) : base(gameStateData)
        {
            Initialize();

            //gamestateepoch means its realtime game
            if (gameStateData.GetType() == typeof(RealtimeGameStateData))
            {
                createdEpoch = (gameStateData as RealtimeGameStateData).createdEpoch;
                _Type = Fourzy.GameType.REALTIME;
            }
        }

        public ClientFourzyGame(Player Player1, Player Player2, GameOptions Options = null)
            : base(Player1, Player2, Options)
        {
            Initialize();

            isBoardRandom = true;
        }

        public ClientFourzyGame(
            Area Area,
            Player Player1,
            Player Player2,
            int FirstPlayerId,
            GameOptions Options = null) : base(Area, Player1, Player2, FirstPlayerId, Options)
        {
            Initialize();

            isBoardRandom = true;
        }

        public ClientFourzyGame(
            GameBoardDefinition definition,
            Player Player1,
            Player Player2) : base(definition, Player1, Player2)
        {
            initialMoves = new List<SimpleMove>(definition.InitialMoves);
            BoardID = definition.ID;

            Initialize();
        }

        public ClientFourzyGame(string boardJson) : base(boardJson)
        {
            Initialize();
        }

        public ClientFourzyGame(Area Area, BossType Boss, Player Player) : base(Area, Boss, Player)
        {
            Initialize();

            isBoardRandom = true;
        }

        public ClientFourzyGame(GameBoardDefinition definition, BossType aiBoss, Player player)
            : base(definition, aiBoss, player)
        {
            Initialize();

            _Type = Fourzy.GameType.AI;
        }

        public ClientFourzyGame(GameBoardDefinition definition, AIProfile aiProfile, Player player)
            : base(definition, aiProfile, player)
        {
            Initialize();

            _Type = Fourzy.GameType.AI;
        }

        public ClientFourzyGame(
            Player Player1,
            int GauntletLevel,
            Area CurrentArea = Area.NONE,
            int DifficultModifier = -1,
            int membersCount = 999/*GauntletStatus Status = null*/,
            GameOptions Options = null)
            : base(Player1, GauntletLevel, CurrentArea, DifficultModifier, /*Status*/membersCount, Options)
        {
            Initialize();

            _Type = Fourzy.GameType.AI;
        }

        public bool IsWinner(Player player) => State.WinnerId == player.PlayerId;

        public bool IsWinner() => me.PlayerId == State.WinnerId;

        public override PlayerTurnResult TakeTurn(
            PlayerTurn Turn,
            bool ReturnStartOfNextTurn = false,
            bool TriggerEndOfTurn = true)
        {
            if (State.Players.ContainsKey(State.ActivePlayerId))
            {
                Turn.PlayerString = State.Players[State.ActivePlayerId].PlayerString ?? "";
            }
            else
            {
                Turn.PlayerString = "Null";
                Debug.Log("Active PlayerId is not present in Player Dictionary: " +
                    State.ActivePlayerId + " GameSeed: " + State.GameSeed);
            }

            _allTurnRecord.Add(Turn);

            return base.TakeTurn(Turn, ReturnStartOfNextTurn, TriggerEndOfTurn);
        }

        public override PlayerTurnResult TakeAITurn(bool ReturnStartOfNextTurn = false)
        {
            PlayerTurnResult result = base.TakeAITurn(ReturnStartOfNextTurn);

            _allTurnRecord.Add(result.Turn);

            switch (_Type)
            {
                case Fourzy.GameType.REALTIME:
                    switch (GameManager.Instance.ExpectedGameType)
                    {
                        case GameTypeLocal.REALTIME_LOBBY_GAME:
                        case GameTypeLocal.REALTIME_QUICKMATCH:
                            var eventOptions = new Photon.Realtime.RaiseEventOptions();
                            eventOptions.Flags.HttpForward = true;
                            eventOptions.Flags.WebhookFlags = Photon.Realtime.WebFlags.HttpForwardConst;
                            var aiTurnEvenResult = PhotonNetwork.RaiseEvent(
                                Constants.TAKE_TURN,
                                JsonConvert.SerializeObject(result.Turn),
                                eventOptions,
                                SendOptions.SendReliable);
                            Debug.Log("Photon AI take turn event result: " + aiTurnEvenResult);

                            break;
                    }

                    break;
            }

            return result;
        }

        public void SetInitialTime(float value) => ClientFourzyHelper.SetInitialTime(this, value);

        public void AddPlayerMagic(int playerId, int value) =>
            ClientFourzyHelper.AddPlayerMagic(this, playerId, value);

        public void OnVictory() => ClientFourzyHelper.OnVictory(this);

        public void OnDraw() => ClientFourzyHelper.OnDraw(this);

        public void CheckLost() => ClientFourzyHelper.CheckLost(this);

        public void RemoveMember() => ClientFourzyHelper.RemoveMember(this);

        public void AddMembers(int count) => ClientFourzyHelper.AddMembers(this, count);

        public void UpdateFirstState()
        {
            _FirstState = new GameState(State);

            if (State.Herds.Count > 0)
            {
                _originalHerdCount = myMembers.Count;
                int myID = me.PlayerId;

                State.Herds[myID] = new Herd(_FirstState.Herds[myID].HerdId, _originalHerdCount);
            }
        }

        public IClientFourzy Next()
        {
            if (puzzleData)
            {
                if (puzzleData.pack)
                {
                    return puzzleData.pack.Next(this);
                }
                else
                {
                    return GameContentManager.Instance.GetNextFastPuzzle(puzzleData.ID);
                }
            }

            switch (_Type)
            {
                case Fourzy.GameType.PASSANDPLAY:
                    GameBoardDefinition _gameBoardDefinition = null;

                    //get next board
                    int index = GameContentManager.Instance.passAndPlayBoards.IndexOf(
                        GameContentManager.Instance.passAndPlayBoards.Find(board =>
                        board.ID == BoardID));

                    if (index < GameContentManager.Instance.passAndPlayBoards.Count && index >= 0)
                    {
                        _gameBoardDefinition =
                            GameContentManager.Instance.passAndPlayBoards[index + 1];
                    }
                    else
                    {
                        _gameBoardDefinition = GameContentManager.Instance.passAndPlayBoards[0];
                    }

                    return new ClientFourzyGame(
                        _gameBoardDefinition,
                        UserManager.Instance.meAsPlayer,
                        new Player(2, "Player Two"))
                    {
                        _Type = Fourzy.GameType.PASSANDPLAY
                    };
            }

            return null;
        }

        public PlayerTurnResult StartTurn() => StartTurn(_State);

        public GameState _Reset(bool resetMembers = false)
        {
            int myID = me.PlayerId;
            int currentHerdCount = 0;

            if (State.Herds.Count > 0 && !resetMembers)
            {
                currentHerdCount = State.Herds[myID].Members.Count;
            }
            State = new GameState(_FirstState);

            if (State.Herds.Count > 0)
            {
                if (resetMembers)
                {
                    State.Herds[myID] = new Herd(_FirstState.Herds[myID].HerdId, _originalHerdCount);
                }
                else
                {
                    State.Herds[myID] = new Herd(_FirstState.Herds[myID].HerdId, currentHerdCount);
                }
            }

            LoseStreak++;
            playerTurnRecord = new List<PlayerTurn>();

            Initialize(false);

            //need to reassign values, lazy fix
            _Type = _type;
            ClientFourzyHelper.AssignPrefabs(this, true);

            return State;
        }

        public void SetPlayer2ID(string id)
        {
            _FirstState.Players[2].PlayerString = id;
            _State.Players[2].PlayerString = id;
        }

        public static ClientFourzyGame FromPuzzleData(ClientPuzzleData puzzleData, IClientFourzy current)
        {
            ClientFourzyGame game;
            Player me = UserManager.Instance.meAsPlayer;
            me.Magic = puzzleData.startingMagic;

            switch (puzzleData.pack.packType)
            {
                case PackType.AI_PACK:
                    //gauntlet game
                    if (puzzleData.gauntletStatus != null)
                    {
                        game = new ClientFourzyGame(
                            me,
                            puzzleData.puzzleIndex,
                            membersCount: (current != null ?
                                    current.myMembers.Count :
                                    InternalSettings.Current.GAUNTLET_DEFAULT_MOVES_COUNT));
                    }
                    //ai puzzle pack
                    else
                    {
                        game = new ClientFourzyGame(puzzleData.gameBoardDefinition, puzzleData.aiProfile, me);

                        if (!string.IsNullOrEmpty(puzzleData.aiPlayerName))
                        {
                            game.opponent.DisplayName = puzzleData.aiPlayerName;
                        }
                    }

                    break;

                default:
                    game = new ClientFourzyGame(puzzleData.gameBoardDefinition, puzzleData.aiBoss, me);
                    game.BoardID = puzzleData.gameBoardDefinition.ID;

                    break;
            }

            game.puzzleData = puzzleData;
            game._State.ActivePlayerId = puzzleData.firstTurn < 1 ? game.me.PlayerId : puzzleData.firstTurn;
            game.opponent.HerdId = puzzleData.PuzzlePlayer.HerdId;

            //update games' state
            game.UpdateFirstState();

            return game;
        }

        internal void SetRandomActivePlayer()
        {
            State.ActivePlayerId = UnityEngine.Random.value > .5f ? 1 : 2;
        }

        private void Initialize(bool resetFirstState = true)
        {
            if (resetFirstState)
            {
                UpdateFirstState();
            }

            collectedItems = new List<RewardsManager.Reward>();
            _allTurnRecord = new List<PlayerTurn>();

            draw = false;
            //assign magic values
            magic = new Dictionary<int, int>();
            foreach (KeyValuePair<int, Player> player in State.Players)
            {
                magic.Add(player.Key, player.Value.Magic);
            }
        }
    }
}
