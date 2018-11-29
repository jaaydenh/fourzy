//modded @vadym udod

using DG.Tweening;
using Fourzy._Updates.Audio;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using Fourzy._Updates.UI.Toasts;
using GameSparks.Api.Requests;
using mixpanel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene, true)]
    public class GamePlayManager : UnitySingleton<GamePlayManager>
    {
        public static event Action OnStartMove;
        public static event Action OnEndMove;
        public static event Action OnGameOver;
        public static event Action<string> OnGamePlayMessage;
        public static event Action<long> OnTimerUpdate;

        [HideInInspector]
        public Game game;
        
        public MenuController menuController;
        
        public GameBoardView gameBoardView;
        public WinningParticleGenerator winningParticleGenerator;

        private bool isLoading = false;
        private bool clockStarted = false;
        private bool replayedLastMove = false;
        private float gameScreenFadeInTime = 0.7f;
        private DateTime serverClock;
        private DateTime playerMoveCountdown;
        private int playerMoveTimer_InitialTime;

        [HideInInspector]
        public GameFinishedScreen gameFinishedScreen;
        [HideInInspector]
        public GameIntroScreen gameIntroScreen;
        [HideInInspector]
        public GameplayScreen gameplayScreen;

        private BGAudioManager.BGAudio bgAudio;

        public bool isDropping { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            isDropping = false;
            game = GameManager.Instance.activeGame;

            if (game == null)
            {
                GameManager.Instance.activeGame = game = GameManager.Instance.GetRandomGame();

                game.challengeId = "123999428123456";
                //game.displayIntroUI = true;
                //game.title = "Title";
                //game.subtitle = "Subtitle";
            }

            game.boardView = gameBoardView;

            switch (game.gameState.GameType)
            {
                case GameType.REALTIME:
                    StartCoroutine(SendTimeStamp());
                    break;
            }
        }

        protected IEnumerator Start()
        {
            //get screens
            gameFinishedScreen = menuController.GetScreen<GameFinishedScreen>();
            gameIntroScreen = menuController.GetScreen<GameIntroScreen>();
            gameplayScreen = menuController.GetScreen<GameplayScreen>();

            replayedLastMove = false;

            InitGamePiecePrefabs();
            gameBoardView.UpdateGameBoardSprite(GameContentManager.Instance.GetCurrentTheme().GameBoard);
            gameBoardView.CreateGamePieceViews(game.gameState.GetPreviousGameBoard(), 0.0f);
            gameBoardView.CreateTokenViews(game.gameState.PreviousTokenBoard.tokens, 0.0f);
            gameIntroScreen.Init(game);

            if (GameManager.Instance.shouldLoadOnboarding)
            {
                GameManager.Instance.shouldLoadOnboarding = false;
                GameManager.Instance.onboardingScreen.StartOnboarding();
            }

            playerMoveTimer_InitialTime = Constants.playerMoveTimer_InitialTime;

            yield return StartCoroutine(FadeGameScreen(0.0f, 1.0f, gameScreenFadeInTime));
            yield return StartCoroutine(ReplayLastMove());
            yield return StartCoroutine(ShowTokenInstructionPopupRoutine());
            yield return StartCoroutine(ShowPlayTurnWithDelay(0.3f));

            StartCoroutine(RandomGamePiecesBlinkingRoutine());
        }

        private void OnEnable()
        {
            RealtimeManager.OnReceiveTimeStamp += RealtimeManager_OnReceiveTimeStamp;
            RealtimeManager.OnReceiveMove += RealtimeManager_OnReceiveMove;

            //play BG audio
            switch (game.gameState.GameType)
            {
                case GameType.REALTIME:
                    bgAudio = BGAudioManager.instance.PlayBGAudio(_Updates.Serialized.AudioTypes.GARDEN_REALTIME, true, .9f, 3f);
                    break;
                default:
                    bgAudio = BGAudioManager.instance.PlayBGAudio(_Updates.Serialized.AudioTypes.GARDEN_TURN_BASED, true, .9f, 3f);
                    break;
            }
        }

        private void OnDisable()
        {
            RealtimeManager.OnReceiveTimeStamp -= RealtimeManager_OnReceiveTimeStamp;
            RealtimeManager.OnReceiveMove -= RealtimeManager_OnReceiveMove;

            //stop bg audio
            BGAudioManager.instance.StopBGAudio(bgAudio, 1f);
        }

        void RealtimeManager_OnReceiveTimeStamp(long milliseconds)
        {
            SyncClock(milliseconds);
        }

        void RealtimeManager_OnReceiveMove(Move move)
        {
            StartCoroutine(ReplayIncomingOpponentMove(move));
        }

        /// <summary>
        /// Syncs the local clock to server-time
        /// </summary>
        private void SyncClock(long milliseconds)
        {
            if (isDropping)
                return;

            DateTime dateNow = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc); // get the current time
            serverClock = dateNow.AddMilliseconds(milliseconds + RealtimeManager.Instance.timeDelta).ToLocalTime(); // adjust current time to match clock from server

            if (game.gameState.isCurrentPlayerTurn)
            {
                // make sure that we only calculate the endtime once
                if (!clockStarted)
                {
                    playerMoveCountdown = serverClock.AddMilliseconds(playerMoveTimer_InitialTime + RealtimeManager.Instance.timeDelta); // endtime is 60seconds plus the time-offset
                    clockStarted = true;
                }

                // set the timer each time a new update from the server comes in
                // Debug.Log("Total miliseconds: " + (playerMoveCountdown - serverClock).TotalMilliseconds);
                TimeSpan timeDifference = playerMoveCountdown - serverClock;
                if (timeDifference.TotalMilliseconds <= 0)
                {
                    if (OnTimerUpdate != null)
                        OnTimerUpdate.Invoke(0);

                    List<Move> moves = game.gameState.GetPossibleMoves();
                    Move move = moves[UnityEngine.Random.Range(0, moves.Count)];
                    StartCoroutine(ProcessMove(move));
                    clockStarted = false;
                }
                else
                {
                    if (OnTimerUpdate != null)
                        OnTimerUpdate.Invoke(timeDifference.Ticks);
                }
            }
            else
            {
                // playerMoveCountdown = serverClock;
            }
        }

        private IEnumerator ShowTokenInstructionPopupRoutine()
        {
            if (!game.displayIntroUI)
                yield break;

            yield return new WaitForSeconds(0.5f);

            TokenBoard previousTokenBoard = game.gameState.PreviousTokenBoard;
            IToken[,] previousTokenBoardTokens = previousTokenBoard.tokens;

            HashSet<TokenData> tokens = new HashSet<TokenData>();

            for (int i = 0; i < Constants.numRows; i++)
            {
                for (int j = 0; j < Constants.numColumns; j++)
                {
                    if (previousTokenBoardTokens[i, j] == null || previousTokenBoardTokens[i, j].tokenType == Token.EMPTY)
                    {
                        continue;
                    }

                    TokenData token = GameContentManager.Instance.GetTokenDataWithType(previousTokenBoardTokens[i, j].tokenType);
                    if (!PlayerPrefsWrapper.InstructionPopupWasDisplayed(token.ID))
                    {
                        tokens.Add(token);
                    }
                }
            }

            TokenPopupUI popupUI = PopupManager.Instance.GetPopup<TokenPopupUI>();

            foreach (TokenData token in tokens)
            {
                popupUI.tokenData = token;

                PopupManager.Instance.OpenPopup<TokenPopupUI>();

                yield return new WaitWhile(() => popupUI.IsOpen());

                PlayerPrefsWrapper.SetInstructionPopupDisplayed(token.ID, true);

                yield return new WaitForSeconds(0.5f);
            }
        }

        public void UpdateOpponentUI(Opponent opponent)
        {
            game.opponent = opponent;

            UpdateOpponentUI(opponent.gamePieceId);
        }

        public void UpdateOpponentUI(int gamePieceID)
        {
            game.opponent.gamePieceId = gamePieceID;

            if (gameBoardView.OpponentPiece != null)
            {
                Destroy(gameBoardView.PlayerPiece);
                Destroy(gameBoardView.OpponentPiece);
            }

            InitGamePiecePrefabs();
        }

        private void InitGamePiecePrefabs()
        {
            int playerGamePieceId = 0;
            int opponentGamePieceId = 0;

            PlayerEnum player = PlayerEnum.ONE;
            PlayerEnum opponent = PlayerEnum.TWO;

            if (game.gameState.GameType == GameType.REALTIME)
            {
                playerGamePieceId = UserManager.Instance.gamePieceId;
                opponentGamePieceId = game.opponent.gamePieceId;

                if (!game.isCurrentPlayer_PlayerOne)
                {
                    player = PlayerEnum.TWO;
                    opponent = PlayerEnum.ONE;
                }
            }
            else if (game.isCurrentPlayer_PlayerOne)
            {
                playerGamePieceId = game.challengerGamePieceId;
                opponentGamePieceId = game.challengedGamePieceId;
            }
            else
            {
                playerGamePieceId = game.challengedGamePieceId;
                opponentGamePieceId = game.challengerGamePieceId;

                player = PlayerEnum.TWO;
                opponent = PlayerEnum.ONE;
            }

            bool shouldUseSecondaryColor = (playerGamePieceId == opponentGamePieceId);

            GamePiece prefab = GameContentManager.Instance.GetGamePiecePrefab(playerGamePieceId);
            gameBoardView.PlayerPiece = Instantiate(prefab);
            gameBoardView.PlayerPiece.player = player;
            gameBoardView.PlayerPiece.gameBoardView = gameBoardView;
            gameBoardView.PlayerPiece.View.UseSecondaryColor(player == PlayerEnum.TWO && shouldUseSecondaryColor);
            gameBoardView.PlayerPiece.gameObject.SetActive(false);

            prefab = GameContentManager.Instance.GetGamePiecePrefab(opponentGamePieceId);
            gameBoardView.OpponentPiece = Instantiate(prefab);
            gameBoardView.OpponentPiece.player = opponent;
            gameBoardView.OpponentPiece.gameBoardView = gameBoardView;
            gameBoardView.OpponentPiece.View.UseSecondaryColor(opponent == PlayerEnum.TWO && shouldUseSecondaryColor);
            gameBoardView.OpponentPiece.gameObject.SetActive(false);

            gameplayScreen.InitUI(game);
        }

        private IEnumerator FadeGameScreen(float startAlpha, float alpha, float fadeTime)
        {
            gameBoardView.SetAlpha(startAlpha);
            gameBoardView.Fade(alpha, fadeTime);

            yield return new WaitForSeconds(fadeTime);

            yield return StartCoroutine(FadeTokens(alpha, fadeTime));
            yield return StartCoroutine(FadePieces(alpha, fadeTime));
        }

        private IEnumerator FadeTokens(float alpha, float fadeTime)
        {
            GameObject[] tokenArray = GameObject.FindGameObjectsWithTag("Token");
            for (int i = 0; i < tokenArray.Length; i++)
                tokenArray[i].GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime);

            yield return new WaitForSeconds(fadeTime);
        }

        private IEnumerator FadePieces(float alpha, float fadeTime)
        {
            var pieces = gameBoardView.GetGamePiecesList();
            for (int i = 0; i < pieces.Count; i++)
                pieces[i].View.Fade(alpha, fadeTime);

            if (pieces.Count > 0)
                yield return new WaitForSeconds(fadeTime);
        }

        private IEnumerator ReplayLastMove()
        {
            Debug.Log("ReplayLastMove");
            if (game.gameState.MoveList != null)
            {
                Debug.Log("ReplayLastMove: has move");
                Move lastMove = game.gameState.MoveList.Last();

                PlayerEnum player = PlayerEnum.NONE;
                if (!game.gameState.IsGameOver)
                {
                    player = game.gameState.IsPlayerOneTurn ? PlayerEnum.TWO : PlayerEnum.ONE;
                }
                else
                {
                    player = game.gameState.IsPlayerOneTurn ? PlayerEnum.ONE : PlayerEnum.TWO;
                }

                lastMove.player = player;

                yield return StartCoroutine(MovePiece(lastMove, true, false));
            }
            else if (game.gameState.TokenBoard.initialMoves.Count > 0)
            {
                //StartCoroutine(PlayInitialMoves());
            }
            else
                isLoading = false;
        }

        public IEnumerator ReplayIncomingOpponentMove(Move move)
        {
            while (isDropping || isLoading)
                yield return null;

            StartCoroutine(MovePiece(move, false, true));
        }

        public void RealtimeResignButtonOnClick()
        {
            Mixpanel.Track("Resign Button Press");
            BackButtonOnClick();
        }

        public void BackButtonOnClick()
        {
            if (ViewController.instance.GetCurrentView() != null)
            {
                ViewController.instance.ChangeView(ViewController.instance.GetCurrentView());
                if (ViewController.instance.GetCurrentView() != ViewController.instance.viewTraining && ViewController.instance.GetCurrentView() != ViewController.instance.viewPuzzleSelection)
                {
                    ViewController.instance.ShowTabView();
                }
            }
            else if (ViewController.instance.GetPreviousView() != null)
            {
                ViewController.instance.ChangeView(ViewController.instance.GetPreviousView());
            }

            //ChallengeManager.Instance.ReloadGames();

            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            if (game.gameState.GameType == GameType.REALTIME)
            {
                RealtimeManager.Instance.GetRTSession().Disconnect();
            }
        }

        public void UnloadGamePlayScreen()
        {
            SceneManager.UnloadSceneAsync(Constants.GAMEPLAY_SCENE_NAME);
        }

        public void ResignButtonOnClick()
        {
            ChallengeManager.Instance.Resign(game.challengeId);
        }

        public void NextGameButtonOnClick()
        {
            gameBoardView.backgroundImage.SetAlpha(0.0f);
            GameManager.Instance.OpenNextGame();
        }

        public void CreateGameButtonOnClick()
        {
            Scene uiScene = SceneManager.GetSceneByName(Constants.MAIN_MENU_SCENE_NAME);
            SceneManager.SetActiveScene(uiScene);
            SceneManager.UnloadSceneAsync("Constants.GAMEPLAY_SCENE_NAME");

            ViewController.instance.ChangeView(ViewController.instance.viewMatchMaking);
        }

        public void RematchPassAndPlayGameButtonOnClick()
        {
            Scene uiScene = SceneManager.GetSceneByName(Constants.MAIN_MENU_SCENE_NAME);
            SceneManager.SetActiveScene(uiScene);
            SceneManager.UnloadSceneAsync(Constants.GAMEPLAY_SCENE_NAME);
            ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            ViewGameBoardSelection.instance.TransitionToViewGameBoardSelection(game.gameState.GameType);
            AnalyticsManager.LogCustom("rematch_pnp_game");
        }

        public void RetryPuzzleChallengeButtonOnClick()
        {
            Scene uiScene = SceneManager.GetSceneByName(Constants.MAIN_MENU_SCENE_NAME);
            SceneManager.SetActiveScene(uiScene);
            SceneManager.UnloadSceneAsync(Constants.GAMEPLAY_SCENE_NAME);
            GameManager.Instance.OpenPuzzleChallengeGame("retry");
        }

        public void NextPuzzleChallengeButtonOnClick()
        {
            Scene uiScene = SceneManager.GetSceneByName(Constants.MAIN_MENU_SCENE_NAME);
            SceneManager.SetActiveScene(uiScene);
            SceneManager.UnloadSceneAsync(Constants.GAMEPLAY_SCENE_NAME);
            GameManager.Instance.OpenPuzzleChallengeGame("next");
        }

        private IEnumerator ShowPlayTurnWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            gameplayScreen.UpdatePlayerTurn();
        }

        public IEnumerator PlayInitialMoves()
        {
            List<MoveInfo> initialMoves = game.gameState.TokenBoard.initialMoves;

            for (int i = 0; i < initialMoves.Count; i++)
            {
                Move move = new Move(initialMoves[i].Location, (Direction)initialMoves[i].Direction, i % 2 == 0 ? PlayerEnum.ONE : PlayerEnum.TWO);
                StartCoroutine(MovePiece(move, false, false));
                yield return new WaitWhile(() => isDropping == true);
            }

            isLoading = false;
        }

        public void ProcessPlayerInput(Vector3 mousePosition)
        {
            Debug.Log("Gamemanager: disableInput: " + GameBoardView.disableInput + ", isLoading: " + isLoading + ", isLoadingUI: " + ",isDropping: " + isDropping + ", numPiecesAnimating: " + gameBoardView.NumPiecesAnimating + ", isGameOver: " + game.gameState.IsGameOver);

            if (GameBoardView.disableInput || isLoading|| isDropping || game.gameState.IsGameOver || gameBoardView.NumPiecesAnimating > 0)
            {
                Debug.Log("returned in process player input");
                return;
            }

            //Vector3 pos = Camera.main.ScreenToWorldPoint(mousePosition);
            //Position position = gameBoardView.Vec3ToPosition(pos);
            Position position = gameBoardView.Vec3ToPosition(mousePosition);

            if (game.gameState.isCurrentPlayerTurn)
            {
                PlayerEnum player = game.gameState.IsPlayerOneTurn ? PlayerEnum.ONE : PlayerEnum.TWO;
                Debug.Log("ProcessPlayerInput: column: " + position.column + "   row = " + position.row);
                if (position.IsTopRow())
                {
                    position.row -= 1;
                    Move move = new Move(position, Direction.DOWN, player);
                    StartCoroutine(ProcessMove(move));
                }
                else if (position.IsBottomRow())
                {
                    position.row += 1;
                    Move move = new Move(position, Direction.UP, player);
                    StartCoroutine(ProcessMove(move));
                }
                else if (position.IsRightColumn())
                {
                    position.column += 1;
                    Move move = new Move(position, Direction.LEFT, player);
                    StartCoroutine(ProcessMove(move));
                }
                else if (position.IsLeftColumn())
                {
                    position.column -= 1;
                    Move move = new Move(position, Direction.RIGHT, player);
                    StartCoroutine(ProcessMove(move));
                }
            }
            else
            {
                Debug.Log("Not isCurrentPlayerTurn: challengeInstanceId: " + game.challengeId);
                if (game.challengeId != null)
                {
                    if (position.IsTopRow() || position.IsBottomRow() || position.IsLeftColumn() || position.IsRightColumn())
                        GamesToastsController.ShowToast(GamesToastsController.ToastStyle.ACTION_WARNING, LocalizationManager.Instance.GetLocalizedValue("not_your_turn"));
                }
            }
        }

        public IEnumerator ProcessMove(Move move)
        {
            if (!game.gameState.CanMove(move.GetNextPosition(), game.gameState.TokenBoard.tokens))
            {
                GamesToastsController.ShowToast(GamesToastsController.ToastStyle.ACTION_WARNING, "Nope, not possible");
                yield break;
            }

            isDropping = true;
            bool updatePlayer = true;

            //Debug.Log("game.challengeId: " + game.challengeId);
            if (game.challengeId != null && (game.gameState.GameType == GameType.RANDOM
                                             || game.gameState.GameType == GameType.FRIEND
                                             || game.gameState.GameType == GameType.LEADERBOARD))
            {
                replayedLastMove = false;
                StartCoroutine(MovePiece(move, false, updatePlayer));
                Debug.Log("LogChallengeEventRequest: challengeInstanceId: " + game.challengeId);
                new LogChallengeEventRequest().SetChallengeInstanceId(game.challengeId)
                    .SetEventKey("takeTurn") //The event we are calling is "takeTurn", we set this up on the GameSparks Portal
                    .SetEventAttribute("pos", Utility.GetMoveLocation(move)) // pos is the row or column the piece was placed at depending on the direction
                    .SetEventAttribute("direction", move.direction.GetHashCode()) // direction can be up, down, left, right
                    .SetEventAttribute("player", game.gameState.IsPlayerOneTurn ? (int)Piece.BLUE : (int)Piece.RED)
                    .SetDurable(true)
                    .Send((response) =>
                    {
                        if (response.HasErrors)
                        {
                            Debug.Log("***** ChallengeEventRequest failed: " + response.Errors.JSON);
                            // alertUI.Open("Server Error: " + response.Errors.JSON);
                            OnGamePlayMessage("Error: " + response.Errors.JSON);
                        }
                        else
                        {
                            Debug.Log("ChallengeEventRequest was successful");
                            OnGamePlayMessage("Move was successful");
                        }
                    });

                while (isDropping)
                    yield return null;
            }
            else if (game.gameState.GameType == GameType.REALTIME)
            {
                replayedLastMove = false;
                StartCoroutine(MovePiece(move, false, updatePlayer));
                move.player = game.isCurrentPlayer_PlayerOne ? PlayerEnum.ONE : PlayerEnum.TWO;

                RealtimeManager.Instance.SendRealTimeMove(move);
                while (isDropping)
                    yield return null;

                playerMoveTimer_InitialTime = (int)(playerMoveCountdown - serverClock).TotalMilliseconds + Constants.playerMoveTimer_AdditionalTime;

                TimeSpan ts = playerMoveCountdown - serverClock;
                TimeSpan ts2 = ts.Add(TimeSpan.FromMilliseconds(Constants.playerMoveTimer_AdditionalTime));

                if (OnTimerUpdate != null)
                    OnTimerUpdate(ts2.Ticks);

                // timerText.text = (playerMoveCountDown_LastTime/1000).ToString();
                clockStarted = false;
            }
            else if (game.gameState.GameType == GameType.FRIEND || game.gameState.GameType == GameType.LEADERBOARD)
            {
                StartCoroutine(MovePiece(move, false, updatePlayer));
                ChallengeManager.Instance.ChallengeUser(game, Utility.GetMoveLocation(move), move.direction);
            }
            else if (game.gameState.GameType == GameType.RANDOM)
            {
                StartCoroutine(MovePiece(move, false, updatePlayer));
                ChallengeManager.Instance.ChallengeRandomUser(game, Utility.GetMoveLocation(move), move.direction);
            }
            else if (game.gameState.GameType == GameType.AI)
            {
                StartCoroutine(MovePiece(move, false, updatePlayer));

                if (game.gameState.IsGameOver)
                    yield break;

                while (isDropping)
                    yield return null;

                isDropping = true;

                AiPlayer aiPlayer = new AiPlayer(AIPlayerSkill.LEVEL1);

                StartCoroutine(aiPlayer.MakeMove(move));
            }
            else
            {
                StartCoroutine(MovePiece(move, false, updatePlayer));
                if (game.gameState.GameType == GameType.PUZZLE && !game.gameState.IsGameOver)
                {

                    // yield return new WaitWhile(() => isDropping == true);
                    while (isDropping && gameBoardView.NumPiecesAnimating > 0)
                        yield return null;

                    isDropping = true;
                    // yield return new WaitForSeconds(0.5f);
                    PuzzlePlayer puzzlePlayer = new PuzzlePlayer();

                    StartCoroutine(puzzlePlayer.MakeMove(game));
                }
            }

            yield return null;
        }

        public IEnumerator MovePiece(Move move, bool replayMove, bool updatePlayer)
        {
            isDropping = true;

            if (OnStartMove != null)
                OnStartMove();

            AudioHolder.instance.PlaySelfSfxOneShotTracked(_Updates.Serialized.AudioTypes.GAME_PIECE_MOVE);

            List<IToken> activeTokens;

            game.gameState.PrintGameState("BeforeMove");
            List<MovingGamePiece> movingPieces = game.gameState.MovePiece(move, replayMove, out activeTokens);
            game.gameState.PrintGameState("AfterMove");

            gameBoardView.MoveGamePieceViews(move, movingPieces, activeTokens);
            gameBoardView.PrintGameBoard();

            yield return new WaitWhile(() => gameBoardView.NumPiecesAnimating > 0);

            if (!replayMove || game.gameState.IsGameOver)
                gameplayScreen.SetActionButton();

            if (game.gameState.GameType == GameType.PUZZLE && game.gameState.IsGameOver)
            {
                if (game.gameState.IsPuzzleChallengePassed)
                {
                    string title = LocalizationManager.Instance.GetLocalizedValue("challenge_completed_title");
                    string subtitle = LocalizationManager.Instance.GetLocalizedValue("challenge_completed_subtitle");
                    //DisplayIntroUI(title, subtitle, false);
                }
                else
                {
                    string title = LocalizationManager.Instance.GetLocalizedValue("challenge_failed_title");
                    string subtitle = LocalizationManager.Instance.GetLocalizedValue("challenge_failed_subtitle");
                    //DisplayIntroUI(title, subtitle, false);
                }

                if (game.gameState.IsPuzzleChallengePassed)
                {
                    if (PlayerPrefsWrapper.IsPuzzleChallengeCompleted(game.puzzleChallengeInfo.ID))
                    {
                        ChallengeManager.Instance.SubmitPuzzleCompleted();
                    }

                    PlayerPrefsWrapper.SetPuzzleChallengeCompleted(game.puzzleChallengeInfo.ID, true);

                    GameManager.Instance.SetNextActivePuzzleLevel();
                    AnalyticsManager.LogPuzzleChallenge(game.puzzleChallengeInfo, true, game.gameState.Player1MoveCount);
                }
                else
                {
                    AnalyticsManager.LogPuzzleChallenge(game.puzzleChallengeInfo, false, game.gameState.Player1MoveCount);
                }
            }

            UpdateGameStatus(updatePlayer);

            isDropping = false;

            if (replayMove)
                isLoading = false;

            // Retrieve the current game from the list of games and update its state with the changes from the current move
            if (game.gameState.GameType == GameType.RANDOM
                || game.gameState.GameType == GameType.FRIEND
                || game.gameState.GameType == GameType.LEADERBOARD)
            {
                GameManager.Instance.UpdateGame(game);
            }

            if (game.gameState.IsGameOver)
            {
                if (OnGameOver != null)
                    OnGameOver();
            }

            if (OnEndMove != null)
                OnEndMove();

            yield return true;
        }

        private void UpdateGameStatus(bool updatePlayer)
        {
            // Debug.Log("UpdateGameStatus:isExpired: " + game.isExpired);
            if (game.gameState.IsGameOver || game.isExpired)
                StartCoroutine(DisplayGameOverView());
            else
            {
                if (updatePlayer)
                {
                    if (game.gameState.GameType == GameType.REALTIME
                        || game.gameState.GameType == GameType.RANDOM
                        || game.gameState.GameType == GameType.FRIEND
                        || game.gameState.GameType == GameType.LEADERBOARD
                        || game.gameState.GameType == GameType.AI)
                    {
                        game.gameState.isCurrentPlayerTurn = !game.gameState.isCurrentPlayerTurn;
                    }
                }
                //UpdatePlayerTurn();
                gameplayScreen.UpdatePlayerTurn();
            }
        }

        private bool IsPlayerWinner()
        {
            if (game.gameState.Winner == PlayerEnum.NONE || game.gameState.Winner == PlayerEnum.ALL)
                return false;

            bool isPlayerWinner = false;
            GameType gameType = game.gameState.GameType;

            if (gameType == GameType.PASSANDPLAY)
                isPlayerWinner = true;
            else if (gameType == GameType.PUZZLE)
                isPlayerWinner = game.gameState.IsPuzzleChallengePassed;
            else
            {
                if (game.isCurrentPlayer_PlayerOne && game.gameState.Winner == PlayerEnum.ONE)
                    isPlayerWinner = true;
                else if (!game.isCurrentPlayer_PlayerOne && game.gameState.Winner == PlayerEnum.TWO)
                    isPlayerWinner = true;
            }

            return isPlayerWinner;
        }

        private void PlayWinnerSound()
        {
            if (!IsPlayerWinner())
                return;

            AudioHolder.instance.PlaySelfSfxOneShotTracked(_Updates.Serialized.AudioTypes.GAME_WON);
        }

        private void ShowWinnerParticles()
        {
            if (!IsPlayerWinner())
                return;

            winningParticleGenerator.ShowParticles();
        }

        public void ShowRewardsScreen()
        {
            GameManager.Instance.VisitedGameResults(game);

            bool isCurrentPlayerWinner = this.IsPlayerWinner();
            PlayerEnum currentPlayer = PlayerEnum.NONE;

            if (game.isCurrentPlayer_PlayerOne)
                currentPlayer = PlayerEnum.ONE;
            else
                currentPlayer = PlayerEnum.TWO;

            if (gameFinishedScreen.isOpened)
                menuController.CloseCurrentScreen(false);

            RewardScreen rewardScreen = menuController.GetScreen<RewardScreen>();
            rewardScreen.OpenScreen(new Reward[] {
                    new Reward()
                    {
                        Type = Reward.RewardType.Coins,
                        NumberOfCoins = 10
                    },
                    new Reward()
                    {
                        Type = Reward.RewardType.CollectedGamePiece,
                        CollectedGamePiece = new GamePieceData()
                    }
                });
        }

        private void LogGameWinner()
        {
            switch (game.gameState.GameType)
            {
                case GameType.FRIEND:
                    AnalyticsManager.LogGameOver("friend", game.gameState.Winner, game.gameState.TokenBoard);
                    break;
                case GameType.LEADERBOARD:
                    AnalyticsManager.LogGameOver("leaderboard", game.gameState.Winner, game.gameState.TokenBoard);
                    break;
                case GameType.AI:
                    AnalyticsManager.LogGameOver("AI", game.gameState.Winner, game.gameState.TokenBoard);
                    break;
                case GameType.PASSANDPLAY:
                    AnalyticsManager.LogGameOver("pnp", game.gameState.Winner, game.gameState.TokenBoard);
                    break;
                case GameType.PUZZLE:
                    //AnalyticsManager.LogGameOver("puzzle", gameState.winner, gameState.tokenBoard);
                    break;
                default:
                    AnalyticsManager.LogGameOver("random", game.gameState.Winner, game.gameState.TokenBoard);
                    break;
            }
        }

        /// <summary>
        /// Sends a Unix timestamp in milliseconds to the server
        /// </summary>
        private IEnumerator SendTimeStamp()
        {
            while (true)
            {
                RealtimeManager.Instance.SendTimeStamp();
                yield return new WaitForSeconds(5f);
            }
        }

        private IEnumerator RandomGamePiecesBlinkingRoutine()
        {
            float blinkTime = 2.5f;
            float t = 0.0f;
            while (true)
            {
                t += Time.deltaTime;
                if (t > blinkTime)
                {
                    var piecesForBlink = gameBoardView.GetWaitingGamePiecesList();
                    if (piecesForBlink.Count > 0)
                    {
                        int randomIndex = UnityEngine.Random.Range(0, piecesForBlink.Count);
                        GamePiece gamePiece = piecesForBlink[randomIndex];
                        gamePiece.View.Blink();
                    }
                    t = 0;
                }
                yield return null;
            }
        }

        private IEnumerator DisplayGameOverView()
        {
            if (game.isExpired)
            {
                GameManager.Instance.VisitedGameResults(game);
                //gameInfo.Open(LocalizationManager.Instance.GetLocalizedValue("expired_text"), Color.white, false, !game.didViewResult);
                gameFinishedScreen.Open(LocalizationManager.Instance.GetLocalizedValue("expired_text"), !game.didViewResult);
                yield break;
            }

            Debug.Log("DisplayGameOverView gameState.winner: " + game.gameState.Winner);

            LogGameWinner();
            PlayWinnerSound();
            gameplayScreen.ShowWinnerAnimation(IsPlayerWinner());

            yield return new WaitForSeconds(1.5f);

            ShowWinnerParticles();
            ShowRewardsScreen();

#if UNITY_IOS || UNITY_ANDROID
            if (game.gameState.Winner == PlayerEnum.ONE || game.gameState.Winner == PlayerEnum.TWO)
            {
                if (game.gameState.isCurrentPlayerTurn)
                {
                    Handheld.Vibrate();
                }
            }
#endif
        }
    }
}

