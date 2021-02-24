//@vadym udod

using ExitGames.Client.Photon;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Serialized;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
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
                                    opponent.HerdId = GameContentManager.Instance.piecesDataHolder.random.data.ID;
                                }

                                break;

                            case GameManager.PassPlayCharactersType.RANDOM:
                                if (string.IsNullOrEmpty(me.HerdId))
                                {
                                    me.HerdId = GameContentManager.Instance.piecesDataHolder.random.data.ID;
                                }
                                if (string.IsNullOrEmpty(opponent.HerdId))
                                {
                                    opponent.HerdId = GameContentManager.Instance.piecesDataHolder.random.data.ID;
                                }

                                break;
                        }

                        break;
                }

                //adjust active player ID
                switch (_Type)
                {
                    case Fourzy.GameType.PASSANDPLAY:
                        SetRandomActivePlayerID();

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
                            return GameMode.GAUNTLET;
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
                else
                {
                    switch (_Type)
                    {
                        case Fourzy.GameType.PASSANDPLAY:
                        case Fourzy.GameType.PRESENTATION:
                            return GameMode.LOCAL_VERSUS;

                        case Fourzy.GameType.AI:
                            return GameMode.NONE;
                    }
                }

                return GameMode.NONE;
            }

            set { }
        }

        public float initializedTime { get; set; }
        public string BoardID
        {
            get
            {
                if (string.IsNullOrEmpty(gameID)) gameID = Guid.NewGuid().ToString();

                return gameID;
            }

            set => gameID = value;
        }

        public bool isFourzyPuzzle => false;

        public List<Creature> myMembers => State.Herds[me.PlayerId].Members;

        public int BossMoves { get; set; }

        public int LoseStreak { get; set; }

        public GamePiecePrefabData playerOnePrefabData { get; set; }
        public GamePiecePrefabData playerTwoPrefabData { get; set; }

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
                AssignPrefabs();

                return playerOnePrefabData.player1Prefab;
            }
        }

        public GamePieceView playerTwoGamepiece
        {
            get
            {
                AssignPrefabs();

                return playerOnePrefabData.data.ID == playerTwoPrefabData.data.ID ?
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

        public Piece activePlayerPiece
        {
            get
            {
                return new Piece(
                    State.ActivePlayerId,
                    string.IsNullOrEmpty(activePlayer.HerdId) ? 1 : int.Parse(activePlayer.HerdId));
            }
        }

        public Piece playerPiece
        {
            get
            {
                Player _player = me;
                return new Piece(_player.PlayerId, string.IsNullOrEmpty(_player.HerdId) ?
                    1 :
                    int.Parse(_player.HerdId));
            }
        }

        public Piece opponentPiece
        {
            get
            {
                Player _player = opponent;
                return new Piece(_player.PlayerId, string.IsNullOrEmpty(_player.HerdId) ?
                    1 :
                    int.Parse(_player.HerdId));
            }
        }

        public List<BoardSpace> boardContent
        {
            get
            {
                List<BoardSpace> result = new List<BoardSpace>();

                for (int row = 0; row < Rows; row++)
                {
                    for (int col = 0; col < Columns; col++)
                    {
                        result.Add(State.Board.ContentsAt(row, col));
                    }
                }

                return result;
            }
        }

        public void SetRandomActivePlayerID()
        {
            State.ActivePlayerId = UnityEngine.Random.value > .5f ? 1 : 2;
        }

        public GamePieceView myGamePiece => 
            me.PlayerId == 1 ? 
            playerOneGamepiece :
            playerTwoGamepiece;

        public GamePieceView opponentGamePiece => 
            opponent.PlayerId == 1 ? 
            playerOneGamepiece : 
            playerTwoGamepiece;

        public GamePieceView activePlayerGamePiece => 
            State.ActivePlayerId == 1 ?
            playerOneGamepiece :
            playerTwoGamepiece;

        public Player me => State.Players.Values
            .ToList()
            .Find(_player => _player.PlayerString == UserManager.Instance.userId) ?? State.Players[1];

        public Player opponent => State.Players[(PlayerEnum)me.PlayerId == PlayerEnum.ONE ?
                (int)PlayerEnum.TWO :
                (int)PlayerEnum.ONE];

        public Player activePlayer => isMyTurn ? me : opponent;

        public Player unactivePlayer => isMyTurn ? opponent : me;

        public Player player1 => State.Players[1];

        public Player player2 => State.Players[2];

        public ClientPuzzleData puzzleData { get; set; }

        public bool isMyTurn => me.PlayerId == State.ActivePlayerId;

        public bool haveMoves => State.TurnCount > 0;

        public TurnEvaluator turnEvaluator => new TurnEvaluator(State);

        public float gameDuration
        {
            get
            {
                switch (_Type)
                {
                    case Fourzy.GameType.TURN_BASED:
                        return 0f;

                    case Fourzy.GameType.PASSANDPLAY:
                    case Fourzy.GameType.PUZZLE:
                    case Fourzy.GameType.AI:
                        return Time.time - initializedTime;

                    default:
                        return 0f;
                }
            }
        }

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

        public void SetInitialTime(float value)
        {
            initializedTime = value;
        }

        public void AddPlayerMagic(int playerId, int value)
        {
            magic[playerId] += value;

            onMagic?.Invoke(playerId, magic[playerId]);
        }

        public void OnVictory()
        {
            if (puzzleData)
            {
                if (IsWinner())
                {
                    if (puzzleData.pack)
                    {
                        bool _complete = puzzleData.pack.complete;

                        PlayerPrefsWrapper.SetPuzzleChallengeComplete(puzzleData.ID, true);

                        if (puzzleData.pack.complete && !_complete)
                        {
                            puzzleData.pack.justFinished = true;

                            AnalyticsManager.Instance.LogEvent(AnalyticsManager.AnalyticsEvents.EVENT_COMPLETED,
                                values: new KeyValuePair<string, object>(
                                    AnalyticsManager.EVENT_ID_KEY,
                                    puzzleData.pack.packID));
                        }
                    }
                    else
                    {
                        PlayerPrefsWrapper.SetFastPuzzleComplete(puzzleData.ID, true);

                        //send new statistics to playfab
                        GameManager.UpdateFastPuzzlesStat(GameContentManager.Instance.finishedFastPuzzlesCount);
                    }

                    //assign rewards if any
                    puzzleData.AssignPuzzleRewards();
                }
            }
        }

        public void OnDraw()
        {
            draw = true;
        }

        public void CheckLost()
        {
            if (isOver)
            {
                if (IsWinner())
                {
                    LoseStreak = 0;
                }
            }
        }

        public void RemoveMember()
        {
            myMembers.RemoveAt(myMembers.Count - 1);
        }

        public void AddMembers(int count)
        {
            int playerID = me.PlayerId;

            List<Creature> addition = new List<Creature>();
            for (int index = 0; index < count; index++)
            {
                addition.Add(new Creature(playerPiece.HerdId));
            }

            State.Herds[playerID].Members.AddRange(addition);
            State.Players[playerID].HerdCount = State.Herds[playerID].Members.Count;
        }

        /// <summary>
        /// Only work with passplay boards for now
        /// </summary>
        /// <returns></returns>
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
                    int index = GameContentManager.Instance.passAndPlayDataHolder.gameboards.IndexOf(
                        GameContentManager.Instance.passAndPlayDataHolder.gameboards.Find(board =>
                        board.ID == BoardID));

                    if (index < GameContentManager.Instance.passAndPlayDataHolder.gameboards.Count && index >= 0)
                    {
                        _gameBoardDefinition =
                            GameContentManager.Instance.passAndPlayDataHolder.gameboards[index + 1];
                    }
                    else
                    {
                        _gameBoardDefinition = GameContentManager.Instance.passAndPlayDataHolder.gameboards[0];
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

            Initialize(false);

            //need to reassign values, lazy fix
            _Type = _type;
            AssignPrefabs(true);

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
                    if (puzzleData.gauntletStatus != null)
                    {
                        game = new ClientFourzyGame(
                            me,
                            puzzleData.puzzleIndex,
                            membersCount: (current != null ?
                                    current.myMembers.Count :
                                    InternalSettings.Current.GAUNTLET_DEFAULT_MOVES_COUNT));
                    }
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

                    break;
            }

            game.puzzleData = puzzleData;
            game.BoardID = puzzleData.gameBoardDefinition != null ?
                puzzleData.gameBoardDefinition.ID :
                "random_level";
            game._State.ActivePlayerId = puzzleData.firstTurn < 1 ?
                game.me.PlayerId :
                puzzleData.firstTurn;

            game.opponent.HerdId = puzzleData.PuzzlePlayer.HerdId;

            return game;
        }

        private void Initialize(bool resetFirstState = true)
        {
            if (resetFirstState)
            {
                _FirstState = new GameState(State);

                if (State.Herds.Count > 0)
                {
                    _originalHerdCount = myMembers.Count;
                    int myID = me.PlayerId;

                    State.Herds[myID] = new Herd(_FirstState.Herds[myID].HerdId, _originalHerdCount);
                }
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

        private void AssignPrefabs(bool force = false)
        {
            if (playerOnePrefabData == null || force)
            {
                string player1HerdId = "1";
                if (State.Players[1].HerdId != null)
                {
                    player1HerdId = State.Players[1].HerdId;
                }

                if (State.Players.ContainsKey(1) && !string.IsNullOrEmpty(State.Players[1].HerdId))
                {
                    playerOnePrefabData =
                        GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(player1HerdId);
                }
                else
                {
                    playerOnePrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(
                        UserManager.Instance.gamePieceID);
                }
            }

            if (playerTwoPrefabData == null || force)
            {
                string player2HerdId = "1";
                if (State.Players[2].HerdId != null)
                {
                    player2HerdId = State.Players[2].HerdId;
                }

                if (State.Players.ContainsKey(2) && !string.IsNullOrEmpty(State.Players[2].HerdId))
                {
                    playerTwoPrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(
                        player2HerdId);
                }
                else
                {
                    playerTwoPrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(
                        UserManager.Instance.gamePieceID);
                }
            }
        }
    }
}
