using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Api.Messages;
using GameSparks.Core;
using System.Linq;
using DG.Tweening;
using Lean;
using UnityEngine.Analytics.Experimental;

namespace Fourzy
{
    public class GameManager : MonoBehaviour
    {
        public float dropTime = 1.0f;
        // GameSparks Challenge Instance Id
        public string challengeInstanceId = null;
        public GameObject gamePiecePrefab;
        public GameObject cornerSpot;

        // ---------- Token Views ----------
        public GameObject upArrowToken;
        public GameObject downArrowToken;
        public GameObject leftArrowToken;
        public GameObject rightArrowToken;
        public GameObject stickyToken;
        public GameObject blockerToken;
        public GameObject ghostToken;
        public GameObject iceSheetToken;
        public GameObject pitToken;
        // ---------- Token Views ----------

        public GameObject moveArrowLeft;
        public GameObject moveArrowRight;
        public GameObject moveArrowDown;
        public GameObject moveArrowUp;
        public GameBoardView gameBoardView;
        public GameState gameState { get; set; }
        //public List<GSData> moveList;
        public List<GameObject> tokenViews;
        public List<ActiveGame> activeGames;
        public Button rematchButton;
        public Button nextGameButton;
        public Button createGameButton;
        public GameObject CreateGameScreen;
        public Text gameStatusText;
        public Sprite playerOneSprite;
        public Sprite playerTwoSprite;
        public string bluePlayerWonText = "Blue Player Won!";
        public string redPlayerWonText = "Red Player Won!";
        public string bluePlayerMoveText = "Blue Player's Move";
        public string redPlayerMoveText = "Red Player's Move";
        public string drawText = "Draw!";

        public Color bluePlayerColor = new Color(0f/255f, 176.0f/255f, 255.0f/255.0f);
        public Color redPlayerColor = new Color(254.0f/255.0f, 40.0f/255.0f, 81.0f/255.0f);

        public Shader lineShader = null;

        // ---------- UI References ----------
        public GameObject gamePieces;
        public GameObject tokens;
        public GameObject gameScreen;
        public GameObject ErrorPanel;
        private GameObject UIScreen;
        public Text playerNameLabel;
        public Image playerProfilePicture;
        public Text opponentNameLabel;
        public Image opponentProfilePicture;
        // ---------- UI References ----------

        public delegate void MoveAction();
        public static event MoveAction OnMoved;

        public bool isMultiplayer = false;
        public bool isNewChallenge = false;
        public bool isNewRandomChallenge = false;
        public bool isAiActive;
        public bool isCurrentPlayer_PlayerOne;
        public string challengedUserId;
        public string winner;
        public string opponentFacebookId;
        public bool isLoading = true;
        bool isDropping = false;
        bool updatingViews = false;
        bool replayedLastMove = false;
        public bool isAnimating = false;
        public float gameScreenFadeInTime = 0.6f;
        //int spacing = 1; //100
        //int offset = 0; //4
        public AudioClip clipMove;
        public AudioClip clipWin;
        private AudioSource audioMove;
        private AudioSource audioWin;
        public CreateGame createGameScript;
        public AiPlayer aiPlayer;

        //Singleton
        private static GameManager _instance;
        public static GameManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<GameManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        void OnApplicationPause(bool paused) {
            if (!paused)
            {
                if (ChallengeManager.instance)
                {
                    ChallengeManager.instance.GetChallenges();
                }
            } else {
                //createGameScript.ResetFindMatchButton();
                new EndSessionRequest()
                    .Send((response) => {
                        if (response.HasErrors) {
                            Debug.Log("***** EndSessionRequest:Error: " + response.Errors.JSON);
                        }
                    });
            }
        }

        private void TransitionToGameScreen() {
            UserInputHandler.inputEnabled = false;
            UIScreen.SetActive(false);
            CreateGameScreen.SetActive(false);
            gameScreen.SetActive(true);
            FadeGameScreen(1.0f, gameScreenFadeInTime);
            StartCoroutine(WaitToEnableInput());
        }

        public void TransitionToGamesListScreen() {
            UserInputHandler.inputEnabled = false;
            UIScreen.SetActive(true);
            gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
            gameScreen.SetActive(false);
            challengeInstanceId = null;
            winner = null;

        }

        public void TransitionToCreateGameScreen() {
            BoardSelectionManager.instance.LoadMiniBoards();
            UIScreen.SetActive(false);
            CreateGameScreen.SetActive(true);
            gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
            gameScreen.SetActive(false);
        }

        private void FadeGameScreen(float alpha, float fadeTime) {
            gameScreen.GetComponent<CanvasGroup>().DOFade(alpha, 0.5f).OnComplete(()=>FadeTokens(alpha, fadeTime));
        }

        private void WinLineSetActive(bool active) {
            LineRenderer[] winLines = gameScreen.GetComponentsInChildren<LineRenderer>(true);

            foreach (var line in winLines)
            {
                if (line) {
                    line.gameObject.SetActive(active);
                }
            }
        }

        private void FadePieces(float alpha, float fadeTime) {

            GameObject[] pieces = GameObject.FindGameObjectsWithTag("GamePiece");
            if (pieces.Length > 0) {
                for (int i=0; i < pieces.Length; i++)
                {
                    if (i < pieces.Length-1) {
                        pieces[i].GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime);
                    } else {
                        pieces[i].GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime).OnComplete(()=>ReplayLastMove());
                    }
                }
            } else {
                ReplayLastMove();
            }
        }

        private void FadeTokens(float alpha, float fadeTime) {
            GameObject[] tokens = GameObject.FindGameObjectsWithTag("Token");
            for (int i=0; i < tokens.Length; i++)
            {
                if (i < tokens.Length-1) {
                    tokens[i].GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime);
                } else {
                    tokens[i].GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime).OnComplete(()=>FadePieces(alpha, fadeTime));
                }
            }
        }

        private void FadeGamesListScreen(float alpha, bool enabled, float fadeTime) {
            UIScreen.GetComponent<CanvasGroup>().DOFade(alpha, fadeTime).OnComplete(()=>UIScreenSetActive(enabled));
        }

        private void GameScreenSetActive(bool setActive) {
            gameScreen.SetActive(setActive);
        }

        private void UIScreenSetActive(bool setActive) {
            UIScreen.SetActive(setActive);
        }

        IEnumerator WaitToEnableInput() {
            yield return new WaitForSeconds(1.5f);
            UserInputHandler.inputEnabled = true;
        }

        private void OnEnable()
        {
            UserInputHandler.OnTap += ProcessPlayerInput;
            ActiveGame.OnActiveGame += TransitionToGameScreen;
            FriendEntry.OnActiveGame += TransitionToGameScreen;
            ChallengeManager.OnActiveGame += TransitionToGameScreen;
            LoginManager.OnLoginError += DisplayLoginError;
        }

        private void OnDisable()
        {
            UserInputHandler.OnTap -= ProcessPlayerInput;
            ActiveGame.OnActiveGame -= TransitionToGameScreen;
            FriendEntry.OnActiveGame -= TransitionToGameScreen;
            ChallengeManager.OnActiveGame -= TransitionToGameScreen;
            LoginManager.OnLoginError -= DisplayLoginError;
        }

        private void CheckConnectionStatus(bool connected) {
            // TODO: inform the player they dont have a connection when connected is false
            Debug.Log("CheckConnectionStatus: " + connected);
        }

        void Start() 
        {
            GS.GameSparksAvailable += CheckConnectionStatus;

            DOTween.Init(false, true, LogBehaviour.ErrorsOnly);

            audioMove = AddAudio(clipMove, false, false, 1);
            audioWin = AddAudio(clipWin, false, false, 1);

            MatchFoundMessage.Listener = (message) => {
                createGameScript.SetButtonStateWrapper(false);
                ChallengeManager.instance.GetChallenges();
            };

            ChallengeWonMessage.Listener = (message) => {
                var challenge = message.Challenge;
                if (UserManager.instance.userId != challenge.NextPlayer) {
                    // Only Replay the last move if the player is viewing the game screen for that game
                    if (challenge.ChallengeId == challengeInstanceId) {
                        List<GSData> moveList = challenge.ScriptData.GetGSDataList("moveList");
                        GSData lastMove = moveList.Last();
                        int currentPlayerMove = challenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
                        Player player = currentPlayerMove == 1 ? Player.ONE : Player.TWO;
                        StartCoroutine(ReplayIncomingOpponentMove(currentPlayerMove, lastMove, player));
                        ChallengeManager.instance.SetViewedCompletedGame(challenge.ChallengeId);
                    }
                    ChallengeManager.instance.GetChallenges();
                }
            };

            ChallengeLostMessage.Listener = (message) => {
                var challenge = message.Challenge;
                if (UserManager.instance.userId != challenge.NextPlayer) {
                    // Only Replay the last move if the player is viewing the game screen for that game
                    if (challenge.ChallengeId == challengeInstanceId) {
                        List<GSData> moveList = challenge.ScriptData.GetGSDataList("moveList");
                        GSData lastMove = moveList.Last();
                        int currentPlayerMove = challenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
                        Player player = currentPlayerMove == 1 ? Player.ONE : Player.TWO;
                        StartCoroutine(ReplayIncomingOpponentMove(currentPlayerMove, lastMove, player));
                        ChallengeManager.instance.SetViewedCompletedGame(challenge.ChallengeId);
                    }
                    ChallengeManager.instance.GetChallenges();
                }
            };

            ChallengeTurnTakenMessage.Listener = (message) => {
                var gsChallenge = message.Challenge;
                // replayedLastMove is a workaround to avoid having the opponents move being replayed more than once
                if (UserManager.instance.userId == gsChallenge.NextPlayer) {
                    // Only Replay the last move if the player is viewing the game screen for that game
                    if (gsChallenge.ChallengeId == challengeInstanceId && !replayedLastMove) {
                        replayedLastMove = true;
                        List<GSData> moveList = gsChallenge.ScriptData.GetGSDataList("moveList");
                        GSData lastMove = moveList.Last();
                        int currentPlayerMove = gsChallenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
                        Player opponent = currentPlayerMove == 1 ? Player.TWO : Player.ONE;
                        StartCoroutine(ReplayIncomingOpponentMove(currentPlayerMove, lastMove, opponent));
                    } else {
                        var activeGame = activeGames
                            .Where(t => t.challengeId == gsChallenge.ChallengeId)
                            .FirstOrDefault();
                        if (activeGame != null) {
                            Challenge challenge = new Challenge(gsChallenge);
                            GameState gameState = new GameState(Constants.numRows, Constants.numColumns, challenge, true);
                            activeGame.gameState = gameState;
                        }
                    }

                    // This resets the gameslist when the opponent makes a move
                    ChallengeManager.instance.GetChallenges();
                }
            };

            // Called when another player joins a game you created that was waiting for a 2nd player
            ChallengeJoinedMessage.Listener = (message) => {
                var gsChallenge = message.Challenge;
            };

            // Called when directly challenged by another player
            ChallengeIssuedMessage.Listener = (message) => {
                var gsChallenge = message.Challenge;
                Debug.Log("ChallengeIssuedMessage");
                Debug.Log("gsChallenge.Challenger.Id: " + gsChallenge.Challenger.Id);
                Debug.Log("gsChallenge.Challenged: " + gsChallenge.Challenged);
                if (UserManager.instance.userId == gsChallenge.NextPlayer) {
                    Debug.Log("UserManager.instance.userId == gsChallenge.NextPlayer");
                }
            };

            ChallengeAcceptedMessage.Listener = (message) => {
                var gsChallenge = message.Challenge;
                Debug.Log("ChallengeAcceptedMessage");
                Debug.Log("gsChallenge.Challenger.Id: " + gsChallenge.Challenger.Id);
                Debug.Log("gsChallenge.Challenged: " + gsChallenge.Challenged);
                if (UserManager.instance.userId == gsChallenge.NextPlayer) {
                    Debug.Log("UserManager.instance.userId == gsChallenge.NextPlayer");
                }
            };

            ChallengeStartedMessage.Listener = (message) => {
                var gsChallenge = message.Challenge;
                Debug.Log("ChallengeStartedMessage");
                Debug.Log("gsChallenge.Challenger.Id: " + gsChallenge.Challenger.Id);
                Debug.Log("gsChallenge.Challenged: " + gsChallenge.Challenged);
                if (UserManager.instance.userId == gsChallenge.NextPlayer) {
                    Debug.Log("UserManager.instance.userId == gsChallenge.NextPlayer");
                }
            };
            //gamePieces = new GameObject("GamePieces");
            //gamePieces.transform.parent = gameScreen.transform;

            tokens = new GameObject("Tokens");
            tokens.transform.parent = gameScreen.transform;

            UIScreen = GameObject.Find("UI Screen");

            rematchButton.gameObject.SetActive(false);
            gameScreen.SetActive(false);

            playerNameLabel.text = UserManager.instance.userName;

            // center camera
            Camera.main.transform.position = new Vector3((Constants.numColumns-1) / 2.0f, -((Constants.numRows-1) / 2.0f)+.15f, Camera.main.transform.position.z);
        }

        void Awake() {
            replayedLastMove = false;
        }

        public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) { 
            AudioSource newAudio = gameObject.AddComponent<AudioSource>();
            newAudio.clip = clip; 
            newAudio.loop = loop;
            newAudio.playOnAwake = playAwake;
            newAudio.volume = vol; 
            return newAudio;
        }

        private IEnumerator ReplayIncomingOpponentMove(int currentPlayerMove, GSData lastMove, Player player) {
            int position = lastMove.GetInt("position").GetValueOrDefault();
            Direction direction = (Direction)lastMove.GetInt("direction").GetValueOrDefault();
            Move move = new Move(position, direction, player);

            while(isDropping || isLoading)
                yield return null;
            StartCoroutine(MovePiece(move, false, true));
        }

        private void ReplayLastMove() {
            if (gameState.moveList != null) {
                GSData lastMove = gameState.moveList.Last();
                // Debug.Log("lastmove: " + lastMove.JSON.ToString());
                int position = lastMove.GetInt("position").GetValueOrDefault();
                Direction direction = (Direction)lastMove.GetInt("direction").GetValueOrDefault();

                Player player = Player.NONE;
                if (!gameState.isGameOver) {
                    player = gameState.isPlayerOneTurn ? Player.TWO : Player.ONE;
                } else {
                    player = gameState.isPlayerOneTurn ? Player.ONE : Player.TWO;
                }
                Move move = new Move(position, direction, player);

                StartCoroutine(MovePiece(move, true, false));
            } else {
                isLoading = false;
            }
        }

        public void SetupGameWrapper() {
            SetupGame();
        }

        public void SetupGame() {
            UpdatePlayersStatusView();
            SetGameBoardView(gameState.GetPreviousGameBoard());
            CreateTokenViews();
        }

        public void SetGameBoardView(int[,] board) {
            gameBoardView.gamePieces = new GameObject[Constants.numRows, Constants.numColumns];

            for(int row = 0; row < Constants.numRows; row++)
            {
                for(int col = 0; col < Constants.numColumns; col++)
                {
                    int piece = board[row, col];

                    if (piece == (int)Piece.BLUE)
                    {
                        GameObject pieceObject = SpawnPiece(col, row * -1, Player.ONE);
                        //TODO: Use player chosen sprite for gamepiece
                        SpriteRenderer pieceSprite = pieceObject.GetComponent<SpriteRenderer>();
                        Color c = pieceSprite.color;
                        c.a = 0.0f;
                        pieceSprite.color = c;
                        //pieceSprite.sprite = new Sprite().texture.

                        GamePiece pieceModel = pieceObject.GetComponent<GamePiece>();
                        pieceModel.player = Player.ONE;
                        gameBoardView.gamePieces[row, col] = pieceObject;
                    }
                    else if (piece == (int)Piece.RED)
                    {
                        GameObject pieceObject = SpawnPiece(col, row * -1, Player.TWO);
                        SpriteRenderer pieceSprite = pieceObject.GetComponent<SpriteRenderer>();
                        Color c = pieceSprite.color;
                        c.a = 0.0f;
                        pieceSprite.color = c;
                        //pieceSprite.sprite = new Sprite().texture.
                        GamePiece pieceModel = pieceObject.GetComponent<GamePiece>();
                        pieceModel.player = Player.TWO;
                        gameBoardView.gamePieces[row, col] = pieceObject;
                    }
                }
            }
        }

        public void CreateTokenViews() {
            tokenViews = new List<GameObject>();

            for(int row = 0; row < Constants.numRows; row++)
            {
                for(int col = 0; col < Constants.numColumns; col++)
                {
                    Token token = gameState.tokenBoard.tokens[row, col].tokenType;
                    GameObject go;
                    switch (token)
                    {
                        case Token.UP_ARROW:
                            go = Instantiate(upArrowToken, new Vector3(col, row * -1, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews.Add(go);
                            break;
                        case Token.DOWN_ARROW:
                            go = Instantiate(downArrowToken, new Vector3(col, row * -1, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews.Add(go);
                            break;
                        case Token.LEFT_ARROW:
                            go = Instantiate(leftArrowToken, new Vector3(col, row * -1, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews.Add(go);
                            break;
                        case Token.RIGHT_ARROW:
                            go = Instantiate(rightArrowToken, new Vector3(col, row * -1, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews.Add(go);
                            break;
                        case Token.STICKY:
                            go = Instantiate(stickyToken, new Vector3(col, row * -1, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews.Add(go);
                            break;
                        case Token.BLOCKER:
                            go = Instantiate(blockerToken, new Vector3(col, row * -1, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews.Add(go);
                            break;
                        case Token.GHOST:
                            go = Instantiate(ghostToken, new Vector3(col, row * -1, 5), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews.Add(go);
                            break;
                        case Token.ICE_SHEET:
                            go = Instantiate(iceSheetToken, new Vector3(col, row * -1, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews.Add(go);
                            break;
                        case Token.PIT:
                            go = Instantiate(pitToken, new Vector3(col, row * -1, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews.Add(go);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void EnableTokenAudio() {
            foreach (var item in tokenViews)
            {
                if (item.GetComponent<AudioSource>()) {
                    item.GetComponent<AudioSource>().mute = false;
                }
            }
        }

        public void UpdatePlayersStatusView() {
            //Debug.Log("UpdatePlayersStatusView:isPlayerOneTurn: " + isPlayerOneTurn);
            //Debug.Log("UpdatePlayersStatusView:isCurrentPlayerTurn: " + isCurrentPlayerTurn);
            if (isMultiplayer)
            {
                if (gameState.isCurrentPlayerTurn)
                {
                    gameStatusText.text = "Your Move";
                    gameStatusText.color = gameState.isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
                } else {
                    gameStatusText.text = "Their Move";
                    gameStatusText.color = gameState.isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
                }
            }
            else
            {
                gameStatusText.text = gameState.isPlayerOneTurn ? bluePlayerMoveText : redPlayerMoveText;
                gameStatusText.color = gameState.isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
            }
        }

        public void ResetUI() {
            playerNameLabel.color = isCurrentPlayer_PlayerOne ? bluePlayerColor : redPlayerColor;
            opponentNameLabel.color = isCurrentPlayer_PlayerOne ? redPlayerColor : bluePlayerColor;

            rematchButton.gameObject.SetActive(false);
            nextGameButton.gameObject.SetActive(false);
            createGameButton.gameObject.SetActive(false);
        }

        public void NextGame() {
            gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
            // foreach (var game in activeGames)
            // {
            //     Debug.Log("viewedResult: " + game.viewedResult);
            //     Debug.Log("gameState.isGameOver: " + game.gameState.isGameOver);
            //     Debug.Log("gameState.isCurrentPlayerTurn: " + game.gameState.isCurrentPlayerTurn);
            // }
            var activeGame = activeGames
                .Where(t => (t.gameState.isCurrentPlayerTurn == true || (t.viewedResult == false && t.gameState.isGameOver == true)) && t.challengeId != challengeInstanceId)
                .FirstOrDefault();
            if (activeGame != null) {
                activeGame.OpenGame();
            }
        }

        public void SetActionButton(){
            if (isMultiplayer) {
                //if (!gameState.isCurrentPlayerTurn) {
                    var activeGame = activeGames
                        //.Where(t => t.gameState.isCurrentPlayerTurn == true && t.challengeId != challengeInstanceId)
                        .Where(t => (t.gameState.isCurrentPlayerTurn == true || (t.viewedResult == false && t.gameState.isGameOver == true && t.gameState.isCurrentPlayerTurn == false)) && t.challengeId != challengeInstanceId)
                        .FirstOrDefault();
                    // var activeGame2 = activeGames
                    //     //.Where(t => t.gameState.isCurrentPlayerTurn == true && t.challengeId != challengeInstanceId)
                    //     .Where(t => (t.gameState.isCurrentPlayerTurn == true || (t.viewedResult == false && t.gameState.isGameOver == true && t.gameState.isCurrentPlayerTurn == false)) && t.challengeId != challengeInstanceId);
                        
                    //     foreach (var game in activeGame2)
                    //     {
                    //         Debug.Log("viewedResult: " + game.viewedResult);
                    //         Debug.Log("gameState.isGameOver: " + game.gameState.isGameOver);
                    //         Debug.Log("gameState.isCurrentPlayerTurn: " + game.gameState.isCurrentPlayerTurn);
                    //     }
                    if (activeGame != null) {
                        nextGameButton.gameObject.SetActive(true);
                        createGameButton.gameObject.SetActive(false);
                    } else {
                        createGameButton.gameObject.SetActive(true);
                        nextGameButton.gameObject.SetActive(false);
                    }
                //}
            } else {
                if (gameState.isGameOver) {
                    rematchButton.gameObject.SetActive(true);
                }
            }
        }

        public void ResetGamePiecesAndTokens() {
            Debug.Log("ResetGamePiecesAndTokens");
            if (gamePieces.transform.childCount > 0) {
                for (int i = gamePieces.transform.childCount-1; i >= 0; i--)
                {
                    Transform piece = gamePieces.transform.GetChild(i);
                    piece.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    Lean.LeanPool.Despawn(piece.gameObject);
                }
            }

            if (tokens.transform.childCount > 0) {
                for (int i = tokens.transform.childCount-1; i >= 0; i--)
                {
                    Transform token = tokens.transform.GetChild(i);
                    DestroyImmediate(token.gameObject);
                }
            }

            gameBoardView.Clear();

            // if(tokens != null)
            // {
            //     DestroyImmediate(tokens);
            // }
            // tokens = new GameObject("Tokens");
            // tokens.transform.parent = gameScreen.transform;
            // tokens.transform.localPosition = new Vector3(-375f, -501f);
        }

        public void PopulateMoveArrows() {
            // for(int col = 0; col < numColumns; col++)
            // {
            //     for(int row = 0; row < numRows; row++)
            //     {
            //         GameObject g;

            //         if (col == numColumns - 1 && row > 0 && row < numRows - 1) {
            //             g = Instantiate(moveArrowLeft, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 10), Quaternion.identity) as GameObject;
            //             g.transform.parent = gamePieces.transform;
            //         }
            //         if (col == 0 && row > 0 && row < numRows - 1) {
            //             g = Instantiate(moveArrowRight, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 10), Quaternion.identity) as GameObject;
            //             g.transform.parent = gamePieces.transform;
            //         }
            //         if (row == 0 && col > 0 && col < numColumns - 1) {
            //             if (gameBoard.GetCell(col, row) == 0) {
            //                 g = Instantiate(moveArrowDown, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 10), Quaternion.identity) as GameObject;
            //                 g.transform.parent = gamePieces.transform;
            //             }
            //         }
            //         if (row == numRows - 1 && col > 0 && col < numColumns - 1) {
            //             if (gameBoard.GetCell(col, row) == 0) {
            //                 g = Instantiate(moveArrowUp, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 10), Quaternion.identity) as GameObject;
            //                 g.transform.parent = gamePieces.transform;
            //             }
            //         }
            //     }
            // }
        }

        // public void AnimateEmptyEdgeSpots(bool animate) {
        //     foreach (EmptySpot spot in gameScreen.GetComponentsInChildren<EmptySpot>())
        //     {
        //         StartCoroutine(spot.AnimateSpot(animate));
        //     }
        // }

        /// <summary>
        /// Spawns a gamepiece at the given column and row
        /// </summary>
        /// <returns>The piece.</returns>
        GameObject SpawnPiece(float posX, float posY, Player player)
        {
            GameObject gamePiece = Lean.LeanPool.Spawn(gamePiecePrefab,
            new Vector3(Mathf.FloorToInt(posX + 0.5f), 
                Mathf.FloorToInt(posY + 0.5f), 10),
                Quaternion.identity, gamePieces.transform) as GameObject;
            
            gamePiece.transform.position = new Vector3(Mathf.FloorToInt(posX + 0.5f), 
                Mathf.FloorToInt(posY + 0.5f), 10);
            // GameObject gamePiece = Instantiate(
            //     isPlayerOneTurn ? pieceBlue : pieceRed,
            //     new Vector3(Mathf.FloorToInt(posX + 0.5f),
            //     Mathf.FloorToInt(posY + 0.5f), 10), // spawn it above the first row
            //     Quaternion.identity, gamePieces.transform) as GameObject;
            
            if (player == Player.ONE) {
                gamePiece.GetComponent<SpriteRenderer>().sprite = playerOneSprite;
                // Layer for player 1 pieces
                gamePiece.layer = 8;
            } else {
                gamePiece.GetComponent<SpriteRenderer>().sprite = playerTwoSprite;
                // Layer for player 2 pieces
                gamePiece.layer = 9;
            }
            gamePiece.GetComponent<SpriteRenderer>().enabled = true;
            
            gamePiece.GetComponent<GamePiece>().gameManager = this;

            return gamePiece;
        }

        public void RematchPassAndPlayGame() {
            isLoading = true;
            gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
            ResetGamePiecesAndTokens();

            TokenBoard tokenBoard = TokenBoardLoader.instance.GetTokenBoard();
            gameState = new GameState(Constants.numRows, Constants.numColumns, true, true, tokenBoard, null, false, null);

            CreateTokenViews();
            UpdatePlayersStatusView();
            rematchButton.gameObject.SetActive(false);
            GameManager.instance.EnableTokenAudio();
            FadeGameScreen(1.0f, gameScreenFadeInTime);
            isLoading = false;
        }

        //void Update () {
            // if(!isMultiplayer && gameState != null && gameState.isGameOver)
            // {
            //     rematchButton.gameObject.SetActive(true);
            // }
        //}

        private void DisplayLoginError() {
            ErrorPanel.SetActive(true);
        }

        private void ProcessPlayerInput(Vector3 mousePosition) {

            if (isLoading || isDropping || gameState.isGameOver) {
                return;
            }

            if(gameState.isCurrentPlayerTurn)
            {
                //if (!isDropping) {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(mousePosition);

                    // round to a grid square
                    int column = Mathf.RoundToInt(pos.x);
                    int row = Mathf.RoundToInt(pos.y * -1);
                    Position position;
                    Player player = gameState.isPlayerOneTurn ? Player.ONE : Player.TWO;

                    if (inTopRowBounds (pos.x, pos.y)) {
                        position = new Position(column, row - 1);
                        //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                        Move move = new Move(position, Direction.DOWN, player);
                        StartCoroutine(ProcessMove(move, true));
                    } else if (inBottomRowBounds(pos.x, pos.y)) {
                        position = new Position(column, row + 1);
                        Move move = new Move(position, Direction.UP, player);
                        //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                        StartCoroutine(ProcessMove(move, true));
                    } else if (inRightRowBounds(pos.x, pos.y)) {
                        position = new Position(column + 1, row);
                        Move move = new Move(position, Direction.LEFT, player);
                        //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                        StartCoroutine(ProcessMove(move, true));
                    } else if (inLeftRowBounds(pos.x, pos.y)) {
                        position = new Position(column - 1, row);
                        Move move = new Move(position, Direction.RIGHT, player);
                        //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                        StartCoroutine(ProcessMove(move, true));
                    }
                //}
            } else { 
                // TODO: inform the player it is not their turn
            }
        }

        // MovePiece
        // 1. Add the piece to moving pieces
        //   While board.movingpiece contains an active moving piece
        // in case of undo feature, save the initial board state
        // 2. Evalute at Token at row 0 col 3, empty token moves the piece to row 1 col 3, no new board state
        // 3. Evaluate token at row 1 col 3 left arrow, return a new board state, update movingpiece with a new position,
        //    and update moving piece to row 1 col 2
        // Final result
        // 1. Final board state
        // 2. List of completed moving pieces and list of positions for those pieces to be used for piece animation
        public IEnumerator ProcessMove(Move move, bool updatePlayer)
        {
            isDropping = true;

            if (gameState.CanMove(move.GetNextPosition(), gameState.tokenBoard.tokens))
            {
                // Debug.Log("isMultiplayer: " + isMultiplayer);
                // Debug.Log("isNewChallenge: " + isNewChallenge);
                // Debug.Log("isNewRandomChallenge: " + isNewRandomChallenge);
                if (isMultiplayer && !isNewChallenge && !isNewRandomChallenge)
                {
                    replayedLastMove = false;
                    StartCoroutine(MovePiece(move, false, updatePlayer));
                    Debug.Log("LogChallengeEventRequest: challengeInstanceId: " + challengeInstanceId);
                    new LogChallengeEventRequest().SetChallengeInstanceId(challengeInstanceId)
                        .SetEventKey("takeTurn") //The event we are calling is "takeTurn", we set this up on the GameSparks Portal
                        .SetEventAttribute("pos", Utility.GetMoveLocation(move)) // pos is the row or column the piece was placed at depending on the direction
                        .SetEventAttribute("direction", move.direction.GetHashCode()) // direction can be up, down, left, right
                        .SetEventAttribute("player", gameState.isPlayerOneTurn ? (int)Piece.BLUE : (int)Piece.RED)
                        .SetDurable(true)
                        .Send((response) =>
                            {
                                if (response.HasErrors)
                                {
                                    Debug.Log("***** ChallengeEventRequest failed: " + response.Errors.JSON);
                                    gameStatusText.text = "There was a problem making your move. Please try again.";
                                }
                                else
                                {
                                    // If our ChallengeEventRequest was successful we inform the player
                                    Debug.Log("ChallengeEventRequest was successful");
                                }
                            });

                    while(isDropping)
                        yield return null;
                    var activeGame = activeGames
                        .Where(t => t.challengeId == challengeInstanceId)
                        .FirstOrDefault();
                    if (activeGame != null) {
                        activeGame.gameState = gameState;
                    }
                }
                else if (isMultiplayer && isNewChallenge)
                {
                    isNewChallenge = false;
                    StartCoroutine(MovePiece(move, false, updatePlayer));
                    ChallengeManager.instance.ChallengeUser(challengedUserId, gameState, Utility.GetMoveLocation(move), move.direction);
                    // Create an ActiveGame and add it to the activeGames list
                }
                else if (isMultiplayer && isNewRandomChallenge)
                {
                    isNewRandomChallenge = false;
                    StartCoroutine(MovePiece(move, false, updatePlayer));
                    ChallengeManager.instance.ChallengeRandomUser(gameState, Utility.GetMoveLocation(move), move.direction);
                    // Create an ActiveGame and add it to the activeGames list
                } else {
                    // Used for Pass and Play Games
                    StartCoroutine(MovePiece(move, false, updatePlayer));
                }
            } else {
                // TODO: inform the player that the move is not possible
                Debug.Log("MOVE IS NOT POSSIBLE");
                isDropping = false;
            }

            yield return 0;
        }

        private IEnumerator MovePiece(Move move, bool replayMove, bool updatePlayer) {
            // GameObject[] cornerArrows = GameObject.FindGameObjectsWithTag("Arrow");
            // foreach (GameObject cornerArrow in cornerArrows) {
            //     SpriteRenderer sr = cornerArrow.GetComponentInChildren<SpriteRenderer>();
            //     sr.enabled = false;
            // }

            isDropping = true;
            audioMove.Play();

            gameState.PrintGameState("BeforeMove");
            List<MovingGamePiece> movingPieces = gameState.MovePiece(move, replayMove);
            gameState.PrintGameState("AfterMove");

            StartCoroutine(MovePieceGameView(move, movingPieces));

            while(updatingViews)
                yield return null;

            UpdateGameStatus(updatePlayer);

            isDropping = false;
            if (replayMove) {
                isLoading = false;
            }

            if (!replayMove || gameState.isGameOver) {
                SetActionButton();
            }

            if(OnMoved != null)
                OnMoved();
        }

        private void UpdateGameStatus(bool updatePlayer) {
            // Debug.Log("UpdateGameStatus");
            // Debug.Log("gameState.isCurrentPlayerTurn: " + gameState.isCurrentPlayerTurn);
            // Debug.Log("gameState.isGameOver" + gameState.isGameOver);
            var activeGame = activeGames
                .Where(t => t.challengeId == challengeInstanceId).FirstOrDefault();
            if (activeGame != null) {
                if (!activeGame.viewedResult && gameState.isGameOver) {
                    ChallengeManager.instance.SetViewedCompletedGame(challengeInstanceId);
                    // Debug.Log("activeGame.challengeId: " + activeGame.challengeId);
                    activeGame.viewedResult = true;
                }
            }

            if (gameState.isGameOver) {
                DisplayGameOverView();
            } else {
                if (updatePlayer) {
                    if (isMultiplayer || isAiActive) {
                        gameState.isCurrentPlayerTurn = !gameState.isCurrentPlayerTurn;
                    }
                    UpdatePlayersStatusView();
                }
            }
        }

        private IEnumerator MovePieceGameView(Move move, List<MovingGamePiece> movingPieces) {
            updatingViews = true;

            // Create Game Piece View
            GameObject g = SpawnPiece(move.position.column, move.position.row * -1, move.player);
            GamePiece gamePiece = g.GetComponent<GamePiece>();
            gamePiece.isMoving = true;
            gamePiece.player = move.player;
            gamePiece.column = move.position.column;
            gamePiece.row = move.position.row;

            GameObject nextPieceView = null;

            // process animations for completed moving pieces
            for (int i = 0; i < movingPieces.Count; i++)
            {   
                var movingGamePiece = movingPieces[i];
                Position startPosition = movingGamePiece.positions[0];
                Position endPosition = movingGamePiece.positions[movingGamePiece.positions.Count - 1];

                // If it is the first moving piece, the first position should not be used as the piece starts outside the board
                if (i==0) {
                    movingGamePiece.positions.RemoveAt(0);
                }

                GameObject pieceView;
                if (i == 0) {
                    pieceView = g;
                    nextPieceView = gameBoardView.gamePieces[endPosition.row,endPosition.column];
                } else {
                    pieceView = nextPieceView;
                    nextPieceView = gameBoardView.gamePieces[endPosition.row,endPosition.column];
                }

                pieceView.GetComponent<GamePiece>().positions = movingGamePiece.positions;
                StartCoroutine(AnimatePiece(pieceView, movingGamePiece));
                
                if (movingGamePiece.isDestroyed) {
                    // pieceView.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    // Lean.LeanPool.Despawn(pieceView);
                } else {
                    // Update the state of the game board views
                    gameBoardView.gamePieces[endPosition.row, endPosition.column] = pieceView;
                }

                while(this.isAnimating)
                    yield return null;
            }

            gameBoardView.PrintGameBoard();

            updatingViews = false;
        }

        private IEnumerator AnimatePiece(GameObject g, MovingGamePiece movingGamePiece) {
            isAnimating = true;
            // Debug.Log("POSITIONS: ");
            // foreach (var item in positions)
            // {
            //     Debug.Log("row: " + item.row + ", column: " + item.column);
            // }
            //Debug.Log("AnimatePiece: ROW: " + positions[positions.Count - 1].row + "COLUMN: " + positions[positions.Count - 1].column);
            //Debug.Log("AnimatePiece: ROW: " + positions[0].row + "COLUMN: " + positions[0].column);
            
            //GameObject g = gameBoardView.gamePieces[positions[0].row, positions[0].column];
            
            //Vector3 start = new Vector3(positions[1].column, positions[1].row * -1);

            List<Position> positions = movingGamePiece.positions;

            Sequence mySequence = DOTween.Sequence();
            for (int i = 0; i < positions.Count; i++)
            {
                Vector3 end = new Vector3(positions[i].column, positions[i].row * -1);
                //float distance = Vector3.Distance(start, end);

                // if (i < positions.Count) {
                //     mySequence.Append(g.transform.DOMove(end, dropTime, false).SetEase(Ease.Linear));
                // } else {
                //     mySequence.Append(g.transform.DOMove(end, dropTime, false).SetEase(Ease.Linear));
                // }

                mySequence.Append(g.transform.DOMove(end, dropTime, false).SetEase(Ease.Linear));
                //mySequence.Append(g.GetComponent<Rigidbody2D>().DOMove(end, dropTime, false).SetEase(Ease.Linear));

//                float t = 0;
//                while(t < 1)
//                {
//                    t += Time.deltaTime * dropTime;
//                    if (numRows - distance > 0) {
//                        t += (numRows - distance) / 500;
//                    }
//                    g.transform.position = Vector3.Lerp (start, end, t);
//
//                    yield return null;
//                }

                //start = end;
            }

            yield return mySequence.WaitForCompletion();

            if (movingGamePiece.animationState == PieceAnimStates.DROPPING) {
                g.transform.DOScale(0.0f, 1.0f);
            }

            GamePiece gamePiece = g.GetComponent<GamePiece>();
            gamePiece.isMoving = false;
            isAnimating = false;
        }

        public void DisplayGameOverView() {

            if (gameState.winner == Player.ONE) {
                int size = gameState.gameBoard.player1WinningPositions.Count;
                Vector3 startPos = new Vector3(gameState.gameBoard.player1WinningPositions[0].column, gameState.gameBoard.player1WinningPositions[0].row * -1, 12);
                Vector3 endPos = new Vector3(gameState.gameBoard.player1WinningPositions[size-1].column, gameState.gameBoard.player1WinningPositions[size-1].row * -1, 12);
                DrawLine(startPos, endPos, bluePlayerColor);

                if (gameState.isCurrentPlayerTurn) {
                    #if UNITY_IOS || UNITY_ANDROID
                        Handheld.Vibrate();
                    #endif
                }
            }

            if (gameState.winner == Player.TWO) {
                int size = gameState.gameBoard.player2WinningPositions.Count;
                Vector3 startPos = new Vector3(gameState.gameBoard.player2WinningPositions[0].column, gameState.gameBoard.player2WinningPositions[0].row * -1, 12);
                Vector3 endPos = new Vector3(gameState.gameBoard.player2WinningPositions[size-1].column, gameState.gameBoard.player2WinningPositions[size-1].row * -1, 12);
                DrawLine(startPos, endPos, redPlayerColor);

                if (gameState.isCurrentPlayerTurn) {
                    #if UNITY_IOS || UNITY_ANDROID
                        Handheld.Vibrate();
                    #endif
                }
            }
            WinLineSetActive(true);

            if (gameState.winner == Player.NONE || gameState.winner == Player.ALL) {
                gameStatusText.text = drawText;
            } else if (isMultiplayer) {
                if (winner != null && winner.Length > 0)
                {
                    gameStatusText.text = winner + " Won!";
                } else {
                    if (isCurrentPlayer_PlayerOne && gameState.winner == Player.ONE) {
                        audioWin.Play();
                        gameStatusText.text = UserManager.instance.userName + " Won!";
                    } else if (!isCurrentPlayer_PlayerOne && gameState.winner == Player.TWO) {
                        audioWin.Play();
                        gameStatusText.text = UserManager.instance.userName + " Won!";
                    } else {
                        gameStatusText.text = opponentNameLabel.text + " Won!";
                    }

                    gameStatusText.color = gameState.winner == Player.ONE ? bluePlayerColor : redPlayerColor;
                }
            } else {
                AnalyticsEvent.GameOver("local_game");
                audioWin.Play();
                gameStatusText.text = gameState.winner == Player.ONE ? bluePlayerWonText : redPlayerWonText;
                gameStatusText.color = gameState.winner == Player.ONE ? bluePlayerColor : redPlayerColor;
            }
        }

        void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
        {
            //Debug.Log("START X: " + start.x + " START Y: " + start.y);
            //Debug.Log("END X: " + end.x + " END Y: " + end.y);
            //Debug.Log("DRAWLINE");
            GameObject myLine = new GameObject("WinLine");
            myLine.tag = "WinLine";
            myLine.transform.parent = tokens.transform;
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = new Material(lineShader);
            lr.startColor = color;
            lr.endColor = color;
            lr.startWidth = 0.15f;
            lr.endWidth = 0.15f;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            myLine.SetActive(false);
            //GameObject.Destroy(myLine, duration);
        }

        bool inTopRowBounds(float x, float y) {
            return x > 0.5 && x < Constants.numColumns - 1.5 && y > -0.5 && y < 0.5;
        }

        bool inBottomRowBounds(float x, float y) {
            return x > 0.5 && x < Constants.numColumns - 1.5 && y > -Constants.numColumns && y < -Constants.numColumns + 1.5;
        }

        bool inLeftRowBounds(float x, float y) {
            return x > - 0.5 && x < 0.5 && y > -Constants.numColumns + 1.5 && y < -0.5;
        }

        bool inRightRowBounds(float x, float y) {
            return x > Constants.numColumns - 1.5 && x < Constants.numColumns - 0.5 && y > -Constants.numColumns + 1.5 && y < -0.5;
        }
    }
}
