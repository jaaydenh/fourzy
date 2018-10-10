using UnityEngine;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Api.Messages;
using GameSparks.Core;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;
using mixpanel;
using System;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class GameManager : UnitySingleton<GameManager>
    {
        private List<Game> games = new List<Game>();

        public List<Game> Games
        {
            get
            {
                return games;
            }
            set
            {
                games = value;
                if (OnUpdateGames != null)
                {
                    OnUpdateGames(games);
                }
            }
        }

        public Game activeGame;
        public PuzzlePack ActivePuzzlePack { get; private set; }
        public bool isOnboardingActive = false;
        public bool shouldLoadOnboarding = false;

        public Onboarding onboardingScreen;

        public GameObject infoBannerPrefab;

        public static event Action<List<Game>> OnUpdateGames;
        public static event Action<Game> OnUpdateGame;

        protected override void Awake()
        {
            base.Awake();

            DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
        }

        private void OnEnable()
        {
            // GS.GameSparksAvailable += CheckConnectionStatus;
            ChallengeManager.OnReceivedOpponentGamePiece += SetOpponentGamePiece;
            LoginManager.OnLoginMessage += ShowInfoBanner;
            GamePlayManager.OnGamePlayMessage += ShowInfoBanner;
            ChallengeManager.OnOpponentTurnTakenDelegate += OpponentTurnHandler;
            ChallengeManager.OnChallengeJoinedDelegate += ChallengeJoinedHandler;
            ChallengeManager.OnChallengeWonDelegate += ChallengeWonHandler;
            ChallengeManager.OnChallengeLostDelegate += ChallengeLostHandler;
            ChallengeManager.OnChallengeDrawnDelegate += ChallengeDrawnHandler;
            ChallengeManager.OnChallengeIssuedDelegate += ChallengeIssuedHandler;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            // GS.GameSparksAvailable -= CheckConnectionStatus;
            ChallengeManager.OnReceivedOpponentGamePiece -= SetOpponentGamePiece;
            LoginManager.OnLoginMessage -= ShowInfoBanner;
            GamePlayManager.OnGamePlayMessage -= ShowInfoBanner;
            ChallengeManager.OnOpponentTurnTakenDelegate -= OpponentTurnHandler;
            ChallengeManager.OnChallengeJoinedDelegate -= ChallengeJoinedHandler;
            ChallengeManager.OnChallengeWonDelegate -= ChallengeWonHandler;
            ChallengeManager.OnChallengeLostDelegate -= ChallengeLostHandler;
            ChallengeManager.OnChallengeDrawnDelegate -= ChallengeDrawnHandler;
            ChallengeManager.OnChallengeIssuedDelegate -= ChallengeIssuedHandler;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void Start()
        {
            GS.GameSparksAvailable += CheckConnectionStatus;

            Mixpanel.Track("Game Started");

            ChallengeAcceptedMessage.Listener = (message) =>
            {
                var gsChallenge = message.Challenge;
                Debug.Log("ChallengeAcceptedMessage");

                if (UserManager.Instance.userId == gsChallenge.NextPlayer)
                {
                    Debug.Log("UserManager.Instance.userId == gsChallenge.NextPlayer");
                }
            };

            ChallengeStartedMessage.Listener = (message) =>
            {
                var gsChallenge = message.Challenge;
                Debug.Log("ChallengeStartedMessage");

                if (UserManager.Instance.userId == gsChallenge.NextPlayer)
                {
                    Debug.Log("UserManager.Instance.userId == gsChallenge.NextPlayer");
                }
            };

    #if UNITY_IOS
            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
            UnityEngine.iOS.NotificationServices.ClearRemoteNotifications();
    #endif
        }

        private void OpponentTurnHandler(ChallengeTurnTakenMessage message) {
            //Debug.Log("OpponentTurnHandler: active scene: " + SceneManager.GetActiveScene().name);
            var gsChallenge = message.Challenge;
            
            // Only do this for the opponent, not the player who just made the move
            if (UserManager.Instance.userId == gsChallenge.NextPlayer)
            {
                // Only Replay the last move if the player is viewing the game screen for that game
                // replayedLastMove is a workaround to avoid having the opponents move being replayed more than once
                if (SceneManager.GetActiveScene().name == "gamePlay")
                {
                    if (gsChallenge.ChallengeId == activeGame.challengeId) {
                        List<GSData> moveList = gsChallenge.ScriptData.GetGSDataList("moveList");
                        GSData lastMove = moveList.Last();
                        int currentPlayerMove = gsChallenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
                        PlayerEnum opponent = currentPlayerMove == 1 ? PlayerEnum.TWO : PlayerEnum.ONE;
                        int position = lastMove.GetInt("position").GetValueOrDefault();
                        Direction direction = (Direction)lastMove.GetInt("direction").GetValueOrDefault();
                        Move move = new Move(position, direction, opponent);
                        StartCoroutine(GamePlayManager.Instance.ReplayIncomingOpponentMove(move));
                    } else {
                       GamePlayManager.Instance.SetActionButton();
                    }
                }
                else
                {
                    var currentGame = games
                        .Where(t => t.challengeId == gsChallenge.ChallengeId)
                        .FirstOrDefault();
                    if (currentGame != null)
                    {
                        GameSparksChallenge challenge = new GameSparksChallenge(gsChallenge);
                        // TODO: update with correct gameType
                        GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, GameType.RANDOM, challenge.isPlayerOneTurn, true, challenge.isGameOver, challenge.tokenBoard, PlayerEnum.EMPTY, ChallengeManager.Instance.ConvertMoveList(challenge.moveList), challenge.previousGameboardData);
                        currentGame.gameState = newGameState;

                        if (OnUpdateGame != null)
                        {
                            OnUpdateGame(currentGame);
                        }
                    }
                }
            }
        }

        private void ChallengeJoinedHandler(ChallengeJoinedMessage message) {
            var gsChallenge = message.Challenge;
            string opponentId = gsChallenge.Challenged.FirstOrDefault().Id;
            string opponentName = gsChallenge.Challenged.FirstOrDefault().Name;
            string opponentFBId = gsChallenge.Challenged.FirstOrDefault().ExternalIds.GetString("FB");
            Opponent opponent = new Opponent(opponentId, opponentName, opponentFBId);

            ChallengeManager.Instance.GetOpponentGamePiece(opponentId, gsChallenge.ChallengeId);

            var currentGame = games
                .Where(t => t.challengeId == gsChallenge.ChallengeId)
                .FirstOrDefault();
            if (currentGame != null)
            {
                currentGame.opponent = opponent;
                currentGame.InitGame();

                if (OnUpdateGame != null)
                {
                    OnUpdateGame(currentGame);
                }
            }

            if (SceneManager.GetActiveScene().name == "gamePlay" && gsChallenge.ChallengeId == activeGame.challengeId)
            {
                GamePlayManager.Instance.UpdateOpponentUI(opponent);
            }
        }

        private void ChallengeDrawnHandler(ChallengeDrawnMessage message) {
            GameSparksChallenge challenge = new GameSparksChallenge(message.Challenge);
            GameCompletedHandler(challenge);
        }

        private void ChallengeWonHandler(ChallengeWonMessage message) {
            GameSparksChallenge challenge = new GameSparksChallenge(message.Challenge);
            GameCompletedHandler(challenge);
        }

        private void ChallengeLostHandler(ChallengeLostMessage message) {
            GameSparksChallenge challenge = new GameSparksChallenge(message.Challenge);
            GameCompletedHandler(challenge);
        }

        private void GameCompletedHandler(GameSparksChallenge challenge) {
            PlayerEnum player = challenge.currentPlayerMove == 1 ? PlayerEnum.ONE : PlayerEnum.TWO;

            if (challenge.challengeId == activeGame.challengeId)
            {
                //ChallengeManager.Instance.GetRatingDelta(activeGame.challengeId);
            } 

            // Only for the player who didn't make the last move
            if (UserManager.Instance.userId != challenge.nextPlayer) {
                var currentGame = games
                .Where(t => t.challengeId == challenge.challengeId)
                .FirstOrDefault();
                if (currentGame != null)
                {
                    //TODO: update with correct game type
                    GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, GameType.RANDOM, challenge.isPlayerOneTurn, true, challenge.isGameOver, challenge.tokenBoard, player, ChallengeManager.Instance.ConvertMoveList(challenge.moveList), challenge.previousGameboardData);
                    currentGame.gameState = newGameState;
                    currentGame.challengerRatingDelta = challenge.challengerRatingDelta;
                    currentGame.challengedRatingDelta = challenge.challengedRatingDelta;

                    if (OnUpdateGame != null)
                    {
                        OnUpdateGame(currentGame);
                    }
                }

                // Only Replay the last move if the player is viewing the game screen for that game
                if (SceneManager.GetActiveScene().name == "gamePlay")
                {
                    if (challenge.challengeId == activeGame.challengeId) {
                        GSData lastMove = challenge.moveList.Last();

                        int position = lastMove.GetInt("position").GetValueOrDefault();
                        Direction direction = (Direction)lastMove.GetInt("direction").GetValueOrDefault();
                        Move move = new Move(position, direction, player);

                        StartCoroutine(GamePlayManager.Instance.ReplayIncomingOpponentMove(move));
                    } 

                    GamePlayManager.Instance.SetActionButton();
                }
            }

            // Update players coins and rating when a game is completed
            UserManager.Instance.UpdateInformation();
        }

        private void ChallengeIssuedHandler(ChallengeIssuedMessage message) {
            var gsChallenge = message.Challenge;
            Debug.Log("ChallengeIssuedHandler");
            GameSparksChallenge challenge = new GameSparksChallenge(gsChallenge);

            GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, GameType.RANDOM, challenge.isPlayerOneTurn, true, challenge.isGameOver, challenge.tokenBoard, PlayerEnum.EMPTY, ChallengeManager.Instance.ConvertMoveList(challenge.moveList), challenge.previousGameboardData);
            string opponentFBId = gsChallenge.Challenger.ExternalIds.GetString("FB");
            Opponent opponent = new Opponent(gsChallenge.Challenger.Id, gsChallenge.Challenger.Name, opponentFBId);

            Game game = new Game(gsChallenge.ChallengeId, newGameState, false, false, false, opponent, ChallengeState.RUNNING, ChallengeType.STANDARD, challenge.challengerGamePieceId, challenge.challengedGamePieceId, null, null, null, null, true);
            game.gameState.SetRandomGuid(gsChallenge.ChallengeId);
            games.Add(game);

            if (OnUpdateGames != null)
            {
                OnUpdateGames(games);
            }

            if (SceneManager.GetActiveScene().name == "gamePlay") {
                GamePlayManager.Instance.SetActionButton();
            }
        }

        void OnApplicationPause(bool paused)
        {
            if (!paused)
            {
                if (ChallengeManager.Instance)
                {
                    ChallengeManager.Instance.GetChallengesRequest();
                }
            }
            else
            {
                new EndSessionRequest()
                    .Send((response) =>
                    {
                        if (response.HasErrors)
                        {
                            Debug.Log("***** EndSessionRequest:Error: " + response.Errors.JSON);
                        }
                    });
            }
        }

        public Game GetNextActiveGame() 
        {
            Game game = null;
            for (int i = 0; i < games.Count(); i++)
            {
                if (games[i].gameState.isCurrentPlayerTurn == true || (games[i].didViewResult == false && games[i].gameState.IsGameOver == true))
                {
                    game = games[i];
                    break;
                }
            }

            return game;
        }

        public void UpdateGame(Game game) {
            var currentGame = games
                .Where(t => t.challengeId == game.challengeId)
                .FirstOrDefault();
            if (currentGame != null)
            {
                currentGame.gameState = game.gameState;
            }
        }

        public void VisitedGameResults(Game game) {
            var currentGame = games
                .Where(t => t.challengeId == game.challengeId).FirstOrDefault();
            if (currentGame != null)
            {
                if (!currentGame.didViewResult && game.gameState.IsGameOver)
                {
                    ChallengeManager.Instance.SetViewedCompletedGame(game.challengeId);
                    currentGame.didViewResult = true;
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
        {
            SceneManager.SetActiveScene(scene);
        }

        public void OpenGame(Game game)
        {
            activeGame = game;
            SceneManager.LoadScene("gamePlay", LoadSceneMode.Additive);
        }

        public Game GetRandomGame()
        {
            TokenBoard tokenBoard = TokenBoardLoader.Instance.GetRandomTokenBoard();
            GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, GameType.PASSANDPLAY, true, true, tokenBoard, tokenBoard.initialGameBoard, false, null);
            Game newGame = new Game(null, newGameState, true, false, false, null, ChallengeState.NONE, ChallengeType.NONE, UserManager.Instance.gamePieceId.ToString(), null, null, null, null, null, false);
            return newGame;
        }

        public void OpenNewGame(GameType gameType, Opponent opponent, bool displayIntroUI = true, string tokenBoardId = null)
        {
            Debug.Log("GameManager OpenNewGame tokenboardId: " + tokenBoardId);

            TokenBoard tokenBoard = ChallengeManager.Instance.GetTokenBoard(tokenBoardId);
            Debug.Log("OpenNewGame: tokenboard name: " + tokenBoard.name);
            //If we initiated the challenge, we get to be player 1
            GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, gameType, true, true, tokenBoard, tokenBoard.initialGameBoard, false, null);
            Game newGame = new Game(null, newGameState, true, false, false, opponent, ChallengeState.NONE, ChallengeType.NONE, UserManager.Instance.gamePieceId.ToString(), null, null, null, null, null, displayIntroUI);
            OpenGame(newGame);

            AnalyticsManager.LogOpenGame(newGame);
        }

        public void OpenPuzzleChallengeGame(string type = "open") {
            //PuzzleChallengeLevel puzzleChallengeLevel = PuzzleChallengeLoader.instance.GetChallenge();

            PuzzleChallengeLevel puzzleChallengeLevel = ActivePuzzlePack.PuzzleChallengeLevels[ActivePuzzlePack.ActiveLevel-1];

            // if (puzzleChallengeLevel == null) {
            //     if (ViewController.instance.GetCurrentView() != null) {
            //         ViewController.instance.ChangeView(ViewController.instance.GetCurrentView());
            //         if (ViewController.instance.GetCurrentView() != ViewController.instance.viewTraining)
            //         {
            //             ViewController.instance.ShowTabView();
            //         }
            //     } else if (ViewController.instance.GetPreviousView() != null) {
            //         ViewController.instance.ChangeView(ViewController.instance.GetPreviousView());
            //     }
            //     // no more puzzle challenges
            //     PlayerPrefs.DeleteKey("puzzleChallengeLevel");
            //     alertUI.Open(LocalizationManager.Instance.GetLocalizedValue("all_challenges_completed"));
                
            // } else {
                string subtitle = "";
                if (puzzleChallengeLevel.MoveGoal > 1) {
                    subtitle = LocalizationManager.Instance.GetLocalizedValue("puzzle_challenge_win_instructions_plural");
                } else {
                    subtitle = LocalizationManager.Instance.GetLocalizedValue("puzzle_challenge_win_instructions_singular");
                }
                TokenBoard tokenBoard = new TokenBoard(puzzleChallengeLevel.InitialTokenBoard.ToArray(), "", "", null, null, true);
                GameState gameState = new GameState(Constants.numRows, Constants.numColumns, GameType.PUZZLE, true, true, tokenBoard, puzzleChallengeLevel.InitialGameBoard.ToArray(), false, null);
                Game newGame = new Game(null, gameState, true, false, false, null, ChallengeState.NONE, ChallengeType.NONE, null, null, puzzleChallengeLevel, null, puzzleChallengeLevel.Name, subtitle.Replace("%1", puzzleChallengeLevel.MoveGoal.ToString()), true);
                OpenGame(newGame);    

                Dictionary<string, object> customAttributes = new Dictionary<string, object>();
                customAttributes.Add("id", puzzleChallengeLevel.ID);
                customAttributes.Add("level", puzzleChallengeLevel.Level);
                AnalyticsManager.LogCustom(type + "_puzzle_challenge", customAttributes);
            // }
        }

        public void SetActivePuzzlePack(PuzzlePack puzzlePack) {
            ActivePuzzlePack = puzzlePack;
        }

        public void SetNextActivePuzzleLevel() {
            if (ActivePuzzlePack.ActiveLevel < ActivePuzzlePack.PuzzleChallengeLevels.Count) {
                ActivePuzzlePack.ActiveLevel++;
            } else {
                ActivePuzzlePack.ActiveLevel = 1;
            }
        }

        public void OpenNextGame()
        {
            SceneManager.UnloadSceneAsync("gamePlay");
            
            for (int i = 0; i < games.Count; i++)
            {
                if (games[i].challengeId == activeGame.challengeId) {
                    var game = games[i];
                    games.RemoveAt(i);
                    games.Add(game);
                    break;
                }
            }

            for (int i = 0; i < games.Count; i++)
            {
                if ((games[i].gameState.isCurrentPlayerTurn == true || (games[i].didViewResult == false && games[i].gameState.IsGameOver == true)) && games[i].challengeId != activeGame.challengeId)
                {
                    if (games[i] != null)
                    {
                        OpenGame(games[i]);
                        AnalyticsManager.LogCustom("next_game");
                        break;
                    }
                }
            }
        }

        private void SetOpponentGamePiece(string gamePieceId, string challengeId) 
        {
            if (challengeId != "") {
                var game = games
                    .Where(t => t.challengeId == challengeId)
                    .FirstOrDefault();

                if (game != null)
                {
                    game.challengedGamePieceId = int.Parse(gamePieceId);
                }

                if (game.challengedGamePieceId > GameContentManager.Instance.GetGamePieceCount() - 1)
                {
                    game.challengedGamePieceId = 0;
                }
            }
        }

        private void CheckConnectionStatus(bool connected)
        {
            // TODO: inform the player they dont have a connection when connected is false
            //Debug.Log("CheckConnectionStatus: " + connected);
            if (connected) {
                ShowInfoBanner("Connected Successfully");
            } else {
                ShowInfoBanner("Error connecting to server");
            }
        }

        public void ShowInfoBanner(string message)
        {
            Debug.Log("ShowInfoBanner");
            GameObject infoBannerObject = Instantiate(infoBannerPrefab) as GameObject;
            InfoBanner infoBanner = infoBannerObject.GetComponent<InfoBanner>();
            StartCoroutine(infoBanner.ShowText(message));
        }

        public void AddGame(Game game) 
        {
            game.gameState.SetRandomGuid(game.challengeId);
            games.Add(game);
        }

        public void CallMovePiece(Move move, bool replayMove, bool updatePlayer)
        {
            StartCoroutine(GamePlayManager.Instance.MovePiece(move, replayMove, updatePlayer));
        }
    }
}
