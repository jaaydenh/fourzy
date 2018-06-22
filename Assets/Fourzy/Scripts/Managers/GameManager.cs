using UnityEngine;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Api.Messages;
using GameSparks.Core;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Fourzy
{
    public class GameManager : Singleton<GameManager>
    {
        public List<Game> games = new List<Game>();
        public Game activeGame;
        public bool isOnboardingActive = false;

        [Header("Game UI")]
        public AlertUI alertUI;
        public GameObject ErrorPanel;
        public Onboarding onboardingScreen;
        public Badge homeScreenPlayBadge;
        public GameObject tokenPopupUI;
        public GameObject headerUI;

        private void OnEnable()
        {
            GameUI.OnActiveGame += TransitionToGamePlayScene;
            FriendEntry.OnActiveGame += TransitionToGamePlayScene;
            LeaderboardPlayer.OnActiveGame += TransitionToGamePlayScene;
            ChallengeManager.OnActiveGame += TransitionToGamePlayScene;
            ChallengeManager.OnReceivedOpponentGamePiece += SetOpponentGamePiece;
            LoginManager.OnLoginError += DisplayLoginError;
            Game.OnActiveGame += TransitionToGamePlayScene;
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
            GameUI.OnActiveGame -= TransitionToGamePlayScene;
            FriendEntry.OnActiveGame -= TransitionToGamePlayScene;
            LeaderboardPlayer.OnActiveGame -= TransitionToGamePlayScene;
            ChallengeManager.OnActiveGame -= TransitionToGamePlayScene;
            ChallengeManager.OnReceivedOpponentGamePiece -= SetOpponentGamePiece;
            LoginManager.OnLoginError -= DisplayLoginError;
            Game.OnActiveGame -= TransitionToGamePlayScene;
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
            ChallengeAcceptedMessage.Listener = (message) =>
            {
                var gsChallenge = message.Challenge;
                Debug.Log("ChallengeAcceptedMessage");

                if (UserManager.instance.userId == gsChallenge.NextPlayer)
                {
                    Debug.Log("UserManager.instance.userId == gsChallenge.NextPlayer");
                }
            };

            ChallengeStartedMessage.Listener = (message) =>
            {
                var gsChallenge = message.Challenge;
                Debug.Log("ChallengeStartedMessage");

                if (UserManager.instance.userId == gsChallenge.NextPlayer)
                {
                    Debug.Log("UserManager.instance.userId == gsChallenge.NextPlayer");
                }
            };

    #if UNITY_IOS
            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
            UnityEngine.iOS.NotificationServices.ClearRemoteNotifications();
    #endif
        }

        new void Awake()
        {
            base.Awake();
            GS.GameSparksAvailable += CheckConnectionStatus;
            DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
        }

        private void OpponentTurnHandler(ChallengeTurnTakenMessage message) {
            Debug.Log("OpponentTurnHandler: active scene: " + SceneManager.GetActiveScene().name);
            var gsChallenge = message.Challenge;
            
            // Only do this for the opponent, not the player who just made the move
            if (UserManager.instance.userId == gsChallenge.NextPlayer)
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
                        GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, GameType.RANDOM, challenge.isPlayerOneTurn, true, challenge.isGameOver, challenge.tokenBoard, PlayerEnum.EMPTY, ChallengeManager.instance.ConvertMoveList(challenge.moveList), challenge.previousGameboardData);
                        currentGame.gameState = newGameState;
                    }

                    ChallengeManager.instance.ReloadGames();
                }
            }
        }

        private void ChallengeJoinedHandler(ChallengeJoinedMessage message) {
            var gsChallenge = message.Challenge;
            string opponentId = gsChallenge.Challenged.FirstOrDefault().Id;
            string opponentName = gsChallenge.Challenged.FirstOrDefault().Name;
            string opponentFBId = gsChallenge.Challenged.FirstOrDefault().ExternalIds.GetString("FB");
            Opponent opponent = new Opponent(opponentId, opponentName, opponentFBId);

            ChallengeManager.instance.GetOpponentGamePiece(opponentId, gsChallenge.ChallengeId);

            var currentGame = games
                .Where(t => t.challengeId == gsChallenge.ChallengeId)
                .FirstOrDefault();
            if (currentGame != null)
            {
                currentGame.opponent = opponent;
                currentGame.InitGame();
            }

            if (SceneManager.GetActiveScene().name == "gamePlay" && gsChallenge.ChallengeId == activeGame.challengeId)
            {
                GamePlayManager.Instance.UpdateOpponentUI(opponent);
            }
            else
            {
                ChallengeManager.instance.ReloadGames();
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
                //ChallengeManager.instance.GetRatingDelta(activeGame.challengeId);
            } 

            // Only for the player who didn't make the last move
            if (UserManager.instance.userId != challenge.nextPlayer) {
                var currentGame = games
                .Where(t => t.challengeId == challenge.challengeId)
                .FirstOrDefault();
                if (currentGame != null)
                {
                    //TODO: update with correct game type
                    GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, GameType.RANDOM, challenge.isPlayerOneTurn, true, challenge.isGameOver, challenge.tokenBoard, player, ChallengeManager.instance.ConvertMoveList(challenge.moveList), challenge.previousGameboardData);
                    currentGame.gameState = newGameState;
                    currentGame.challengerRatingDelta = challenge.challengerRatingDelta;
                    currentGame.challengedRatingDelta = challenge.challengedRatingDelta;
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
                else
                {
                    ChallengeManager.instance.ReloadGames();
                }
            }

            // Update players coins and rating when a game is completed
            UserManager.instance.UpdateInformation();
        }

        private void ChallengeIssuedHandler(ChallengeIssuedMessage message) {
            var gsChallenge = message.Challenge;
            Debug.Log("ChallengeIssuedHandler");
            GameSparksChallenge challenge = new GameSparksChallenge(gsChallenge);

            GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, GameType.RANDOM, challenge.isPlayerOneTurn, true, challenge.isGameOver, challenge.tokenBoard, PlayerEnum.EMPTY, ChallengeManager.instance.ConvertMoveList(challenge.moveList), challenge.previousGameboardData);
            string opponentFBId = gsChallenge.Challenger.ExternalIds.GetString("FB");
            Opponent opponent = new Opponent(gsChallenge.Challenger.Id, gsChallenge.Challenger.Name, opponentFBId);

            Game game = new Game(gsChallenge.ChallengeId, newGameState, false, false, false, opponent, ChallengeState.RUNNING, ChallengeType.STANDARD, challenge.challengerGamePieceId, challenge.challengedGamePieceId, null, null, null, null, true);
            game.gameState.SetRandomGuid(gsChallenge.ChallengeId);
            games.Add(game);

            if (SceneManager.GetActiveScene().name == "gamePlay") {
                GamePlayManager.Instance.SetActionButton();
            } else {
                ChallengeManager.instance.ReloadGames();
            }
        }

        public void UpdatePlayButtonBadgeCount() {
            int activeGamesCount = 0;

            for (int i = 0; i < games.Count(); i++)
            {
                if (games[i].gameState.isCurrentPlayerTurn == true || (games[i].didViewResult == false && games[i].gameState.IsGameOver == true))
                {
                    activeGamesCount++;
                }
            }

            if (activeGamesCount > 0) {
                homeScreenPlayBadge.gameObject.SetActive(true);
            } else {
                homeScreenPlayBadge.gameObject.SetActive(false);    
            }

            homeScreenPlayBadge.SetGameCount(activeGamesCount);
        }

        // void Update()
        // {
        //     if (Application.platform == RuntimePlatform.Android)
        //     {
        //         if (Input.GetKeyDown(KeyCode.Escape))
        //         {
        //             if (challengeInstanceId != null)
        //             {
        //                 GameScreenBackButton();
        //             }
        //             else
        //             {
        //                 Application.Quit();
        //             }
        //         }
        //     }
        // }

        void OnApplicationPause(bool paused)
        {
            if (!paused)
            {
                if (ChallengeManager.instance)
                {
                    ChallengeManager.instance.GetChallenges();
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

        public Game GetNextActiveGame() {
            int activeGamesCount = 0;
            Game game = null;
            for (int i = 0; i < games.Count(); i++)
            {
                if (games[i].gameState.isCurrentPlayerTurn == true || (games[i].didViewResult == false && games[i].gameState.IsGameOver == true))
                {
                    activeGamesCount++;
                    if (game == null)
                    {
                        game = games[i];
                    }
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
                    ChallengeManager.instance.SetViewedCompletedGame(game.challengeId);
                    currentGame.didViewResult = true;
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            SceneManager.SetActiveScene(scene);
        }

        private void TransitionToGamePlayScene()
        {
            SceneManager.LoadScene("gamePlay", LoadSceneMode.Additive);
            headerUI.SetActive(false);
        }

        public void OpenNewGame(GameType gameType, Opponent opponent, bool displayIntroUI = true, string tokenBoardId = null)
        {
            Debug.Log("GameManager OpenNewGame tokenboardId: " + tokenBoardId);

            TokenBoard tokenBoard = ChallengeManager.instance.GetTokenBoard(tokenBoardId);

            //If we initiated the challenge, we get to be player 1
            GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, gameType, true, true, tokenBoard, tokenBoard.initialGameBoard, false, null);
            Game newGame = new Game(null, newGameState, true, false, false, opponent, ChallengeState.NONE, ChallengeType.NONE, UserManager.instance.gamePieceId.ToString(), null, null, null, null, null, displayIntroUI);
            activeGame = newGame;

            TransitionToGamePlayScene();

            AnalyticsManager.LogOpenGame(newGame);
        }

        public void OpenPuzzleChallengeGame(string type = "open") {
            PuzzleChallengeInfo puzzleChallenge = PuzzleChallengeLoader.instance.GetChallenge();
            if (puzzleChallenge == null) {
                if (ViewController.instance.GetCurrentView() != null) {
                    ViewController.instance.ChangeView(ViewController.instance.GetCurrentView());
                    if (ViewController.instance.GetCurrentView() != ViewController.instance.viewTraining)
                    {
                        ViewController.instance.ShowTabView();
                    }
                } else if (ViewController.instance.GetPreviousView() != null) {
                    ViewController.instance.ChangeView(ViewController.instance.GetPreviousView());
                }
                // no more puzzle challenges
                PlayerPrefs.DeleteKey("puzzleChallengeLevel");
                alertUI.Open(LocalizationManager.Instance.GetLocalizedValue("all_challenges_completed"));
                
            } else {
                string subtitle = "";
                if (puzzleChallenge.MoveGoal > 1) {
                    subtitle = LocalizationManager.Instance.GetLocalizedValue("puzzle_challenge_win_instructions_plural");
                } else {
                    subtitle = LocalizationManager.Instance.GetLocalizedValue("puzzle_challenge_win_instructions_singular");
                }
                TokenBoard tokenBoard = new TokenBoard(puzzleChallenge.InitialTokenBoard.ToArray(), "", "", null, null, true);
                GameState gameState = new GameState(Constants.numRows, Constants.numColumns, GameType.PUZZLE, true, true, tokenBoard, puzzleChallenge.InitialGameBoard.ToArray(), false, null);
                Game newGame = new Game(null, gameState, true, false, false, null, ChallengeState.NONE, ChallengeType.NONE, null, null, puzzleChallenge, null, puzzleChallenge.Name, subtitle.Replace("%1", puzzleChallenge.MoveGoal.ToString()), true);
                activeGame = newGame;
                TransitionToGamePlayScene();

                Dictionary<string, object> customAttributes = new Dictionary<string, object>();
                customAttributes.Add("id", puzzleChallenge.ID);
                customAttributes.Add("level", puzzleChallenge.Level);
                AnalyticsManager.LogCustom(type + "_puzzle_challenge", customAttributes);
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
                        activeGame = games[i];
                        TransitionToGamePlayScene();
                        AnalyticsManager.LogCustom("next_game");
                        break;
                    }
                }
            }
        }

        private void SetOpponentGamePiece(string gamePieceId, string challengeId) {
            // Debug.Log("GameManager: SetOpponentGamePiece: gamepieceid: " + gamePieceId);
            if (challengeId != "") {
                var game = games
                    .Where(t => t.challengeId == challengeId)
                    .FirstOrDefault();

                if (game != null)
                {
                    game.challengedGamePieceId = int.Parse(gamePieceId);
                }

                if (game.challengedGamePieceId > GamePieceSelectionManager.Instance.gamePieces.Count - 1)
                {
                    game.challengedGamePieceId = 0;
                }
            }

            // opponentPiece.sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(challengedGamePieceId);

            // GamePieceUI opponent = opponentPiece.GetComponent<GamePieceUI>();
            // if (challengerGamePieceId == challengedGamePieceId)
            // {
            //     opponent.SetAlternateColor(true);
            // }
            // else
            // {
            //     opponent.SetAlternateColor(false);
            // }
        }

        private void CheckConnectionStatus(bool connected)
        {
            // TODO: inform the player they dont have a connection when connected is false
            //Debug.Log("CheckConnectionStatus: " + connected);
        }

        private void DisplayLoginError()
        {
            ErrorPanel.SetActive(true);
        }

        public void AddGame(Game game) {
            game.gameState.SetRandomGuid(game.challengeId);
            games.Add(game);
        }

        public void CallMovePiece(Move move, bool replayMove, bool updatePlayer)
        {
            StartCoroutine(GamePlayManager.Instance.MovePiece(move, replayMove, updatePlayer));
        }
    }
}
