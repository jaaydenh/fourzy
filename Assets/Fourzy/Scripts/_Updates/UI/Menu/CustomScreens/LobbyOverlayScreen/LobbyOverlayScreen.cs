//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Helpers;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class LobbyOverlayScreen : MenuScreen
    {
        public static LobbyOverlayScreen Instance;

        public RectTransform playerOneParent;
        public RectTransform playerTwoParent;
        public GameObject empty;
        public GameObject overlay;
        public ButtonExtended lobbyButton;

        private LoadingPromptScreen leavingPrompt;
        private PromptScreen wantToLeavePrompt;
        private LobbyOverlayState state = LobbyOverlayState.NONE;
        private float lobbyCreatedAt;

        private GamePieceView playerOneView;
        private GamePieceView playerTwoView;

        private RoomType displayable = RoomType.DIRECT_INVITE | RoomType.LOBBY_ROOM;
        private GamePieceView playerGamepiece => GameContentManager.Instance.piecesDataHolder
                    .GetGamePiecePrefabData(UserManager.Instance.gamePieceID).player1Prefab;
        private GamePieceView opponentGamepiece => GameContentManager.Instance.piecesDataHolder
                    .GetGamePiecePrefabData(FourzyPhotonManager.GetOpponentProperty(
                        Constants.REALTIME_ROOM_GAMEPIECE_KEY,
                        Constants.REALTIME_DEFAULT_GAMEPIECE_KEY)).player1Prefab;

        protected override void Awake()
        {
            base.Awake();

            Instance = this;

            FourzyPhotonManager.onCreateRoom += OnRoomCreated;
            FourzyPhotonManager.onJoinedRoom += OnJoinedRoom;
            FourzyPhotonManager.onPlayerEnteredRoom += OnPlayerEnteredRoom;
            FourzyPhotonManager.onPlayerLeftRoom += OnPlayerLeftRoom;
            FourzyPhotonManager.onJoinedLobby += OnJoinedLobby;
            FourzyPhotonManager.onRoomLeft += OnRoomLeft;
            FourzyPhotonManager.onConnectionTimeOut += OnConnectionTimedOut;
        }

        protected void OnDestroy()
        {
            FourzyPhotonManager.onCreateRoom -= OnRoomCreated;
            FourzyPhotonManager.onJoinedRoom -= OnJoinedRoom;
            FourzyPhotonManager.onPlayerEnteredRoom -= OnPlayerEnteredRoom;
            FourzyPhotonManager.onPlayerLeftRoom -= OnPlayerLeftRoom;
            FourzyPhotonManager.onJoinedLobby -= OnJoinedLobby;
            FourzyPhotonManager.onRoomLeft -= OnRoomLeft;
            FourzyPhotonManager.onConnectionTimeOut -= OnConnectionTimedOut;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            leavingPrompt = PersistantMenuController.Instance
                .GetOrAddScreen<LoadingPromptScreen>()
                .SetType(LoadingPromptScreen.LoadingPromptType.BASIC);
        }

        /// <summary>
        /// Called by button
        /// </summary>
        public void OnActivate()
        {
            wantToLeavePrompt = menuController
                .GetOrAddScreen<PromptScreen>()
                .Prompt("Leave room?", "You sure you want to leave online room?", () =>
                {
                    FourzyPhotonManager.TryLeaveRoom();

                    state = LobbyOverlayState.LEAVING_ROOM;
                    leavingPrompt
                        .Prompt("Leaving room...", "", null, () => state = LobbyOverlayState.NONE)
                        .CloseOnDecline();
                }, null)
                .CloseOnDecline()
                .CloseOnAccept();
        }

        private void SetData(GamePieceView one, GamePieceView two = null)
        {
            if (!isOpened)
            {
                Open();
            }

            bool isOne = one != null;
            bool isTwo = two != null;

            //remove prev
            if (playerOneView)
            {
                Destroy(playerOneView.gameObject);
            }
            if (playerTwoView)
            {
                Destroy(playerTwoView.gameObject);
            }

            if (isOne)
            {
                playerOneView = Instantiate(one, playerOneParent);
                playerOneView.StartBlinking();

                if (!isTwo)
                {
                    lobbyButton.GetBadge().badge.SetColor(Color.white);
                    lobbyButton.GetBadge().badge.SetValue(LocalizationManager.Value("waiting_for_opponent"));
                }
            }

            if (isTwo)
            {
                lobbyButton.GetBadge().badge.SetColor(Color.green);
                lobbyButton.GetBadge().badge.SetValue(LocalizationManager.Value("match_found"));

                playerTwoView = Instantiate(two, playerTwoParent);
                playerTwoView.StartBlinking();
            }

            empty.SetActive(!isTwo);
            overlay.SetActive(isTwo);
        }

        private void OnJoinedRoom(string roomName)
        {
            if ((FourzyPhotonManager.GetRoomProperty(Constants.REALTIME_ROOM_TYPE_KEY, RoomType.NONE)
                & displayable) == 0) return;

            lobbyCreatedAt = Time.time;

            if (PhotonNetwork.IsMasterClient)
            {
                SetData(playerGamepiece);
            }
            else
            {
                SetData(playerGamepiece, opponentGamepiece);
                GameManager.Instance.RealtimeOpponent = new OpponentData(PhotonNetwork.PlayerListOthers[0]);

                //load game
                StartRoutine("load_game", InternalSettings.Current.LOBBY_GAME_LOAD_DELAY, StartGame, null);
                state = LobbyOverlayState.LOADING_GAME;
            }

            string password = FourzyPhotonManager.GetRoomProperty(Constants.REALTIME_ROOM_PASSWORD, "");
            lobbyButton.GetBadge("code").badge.SetValue(password);
        }

        private void OnPlayerEnteredRoom(Photon.Realtime.Player other)
        {
            if ((FourzyPhotonManager.GetRoomProperty(Constants.REALTIME_ROOM_TYPE_KEY, RoomType.NONE)
                & displayable) == 0) return;

            if (isOpened)
            {
                SetData(playerGamepiece, opponentGamepiece);
                GameManager.Instance.RealtimeOpponent = new OpponentData(other);

                //load game
                StartRoutine("load_game", InternalSettings.Current.LOBBY_GAME_LOAD_DELAY, StartGame, null);
                state = LobbyOverlayState.LOADING_GAME;

                AnalyticsManager.Instance.LogOtherJoinedLobby(
                    GameManager.Instance.RealtimeOpponent.Id,
                    Time.time - lobbyCreatedAt);

                CancelRoutine("startBotMatch");
            }
        }

        private void OnPlayerLeftRoom(Photon.Realtime.Player other)
        {
            switch (state)
            {
                case LobbyOverlayState.LOADING_BOT_GAME:
                case LobbyOverlayState.LOADING_GAME:
                    CancelRoutine("load_game");

                    break;
            }
        }

        private void OnJoinedLobby(string lobbyName)
        {
            if (state == LobbyOverlayState.LOADING_BOT_GAME)
            {
                return;
            }

            if (isOpened)
            {
                Close();
                CancelRoutine("startBotMatch");
            }
        }

        private void OnRoomLeft()
        {
            if (state == LobbyOverlayState.LOADING_BOT_GAME)
            {
                return;
            }

            if (isOpened)
            {
                Close();
                CancelRoutine("startBotMatch");
            }

            AnalyticsManager.Instance.LogEvent(
                "lobbyAbandoned",
                new Dictionary<string, object>()
                {
                    ["playerId"] = LoginManager.playfabId,
                    ["timer"] = SettingsManager.Get(SettingsManager.KEY_REALTIME_TIMER),
                    ["area"] = ((Area)PlayerPrefsWrapper.GetCurrentArea()).ToString(),
                    ["isMagicEnabled"] = SettingsManager.Get(SettingsManager.KEY_REALTIME_MAGIC),
                    ["isPrivate"] = !string.IsNullOrEmpty(FourzyPhotonManager.PASSWORD),
                    ["complexityScore"] = "",
                    ["timeSinceGameCreated"] = Time.time - lobbyCreatedAt,
                });

            switch (state)
            {
                case LobbyOverlayState.LEAVING_ROOM:
                    if (leavingPrompt.isOpened)
                    {
                        leavingPrompt.Decline(true);
                    }

                    break;

                case LobbyOverlayState.LOADING_GAME:
                    CancelRoutine("load_game");

                    break;
            }
        }

        private void OnRoomCreated(string roomName)
        {
            if ((FourzyPhotonManager.GetRoomProperty(Constants.REALTIME_ROOM_TYPE_KEY, RoomType.NONE)
                & displayable) == 0) return;

            AnalyticsManager.Instance.LogEvent(
                "lobbyCreated",
                new Dictionary<string, object>()
                {
                    ["playerId"] = LoginManager.playfabId,
                    ["timer"] = SettingsManager.Get(SettingsManager.KEY_REALTIME_TIMER),
                    ["area"] = ((Area)PlayerPrefsWrapper.GetCurrentArea()).ToString(),
                    ["isMagicEnabled"] = SettingsManager.Get(SettingsManager.KEY_REALTIME_MAGIC),
                    ["isPrivate"] = !string.IsNullOrEmpty(FourzyPhotonManager.PASSWORD),
                    ["complexityScore"] = "",
                });

            StartBotRoutine();
        }

        private void StartBotRoutine()
        {
            float waitTime = InternalSettings.Current.BOT_SETTINGS.randomMatchAfter;

            if (waitTime <= 0f) return;

            StartRoutine("startBotMatch", waitTime, () =>
            {
                //get bot profile from players' rating
                PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                {
                    FunctionName = "botDataFromRating",
                    GeneratePlayStreamEvent = true,
                },
                (result) =>
                {
                    BotSettings botSettings = 
                        JsonConvert.DeserializeObject<BotSettings>(result.FunctionResult.ToString());

                    Debug.Log("starting ai match for " + botSettings.AIProfile);
                    if (wantToLeavePrompt && wantToLeavePrompt.isOpened)
                    {
                        wantToLeavePrompt.CloseSelf();
                    }

                    state = LobbyOverlayState.LOADING_BOT_GAME;

                    GameManager.Instance.RealtimeOpponent =
                        new OpponentData(
                            "bot",
                            botSettings.r,
                            Constants.GAMES_BEFORE_RATING_DISPLAYED);
                    GameManager.Instance.Bot = new FourzyGameModel.Model.Player(
                        2,
                        UserManager.CreateNewPlayerName(),
                        botSettings.AIProfile)
                    {
                        HerdId = GameContentManager.Instance.piecesDataHolder.random.data.ID,
                        PlayerString = "2",
                    };

                    SetData(playerGamepiece,
                        GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(
                            GameManager.Instance.Bot.HerdId).player1Prefab);

                    AnalyticsManager.Instance.LogOtherJoinedLobby(
                        GameManager.Instance.Bot.Profile.ToString(),
                        Time.time - lobbyCreatedAt);

                    StartRoutine("load_game", InternalSettings.Current.LOBBY_GAME_LOAD_DELAY, StartGame);

                    FourzyPhotonManager.TryLeaveRoom();
                    FourzyPhotonManager.Instance.JoinLobby();
                },
                (error) => { Debug.LogError(error.ErrorMessage); });
            },
            null);


        }

        private void OnConnectionTimedOut()
        {
            if (leavingPrompt.isOpened)
            {
                leavingPrompt.Close();
                CancelRoutine("startBotMatch");
            }
        }

        private void StartGame()
        {
            switch (state)
            {
                case LobbyOverlayState.LOADING_GAME:
                    GameManager.Instance.StartGame(GameTypeLocal.REALTIME_LOBBY_GAME);

                    break;

                case LobbyOverlayState.LOADING_BOT_GAME:
                    GameManager.Instance.StartGame(GameTypeLocal.REALTIME_BOT_GAME);

                    break;
            }

            if (isOpened)
            {
                Close();
            }

            state = LobbyOverlayState.NONE;
        }

        private enum LobbyOverlayState
        {
            NONE,
            LEAVING_ROOM,
            LOADING_GAME,
            LOADING_BOT_GAME,
        }
    }
}