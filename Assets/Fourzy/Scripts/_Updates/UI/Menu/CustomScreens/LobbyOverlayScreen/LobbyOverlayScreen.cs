//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Helpers;
using FourzyGameModel.Model;
using Photon.Pun;
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
                    .GetGamePieceData(UserManager.Instance.gamePieceId).player1Prefab;
        private GamePieceView opponentGamepiece => GameContentManager.Instance.piecesDataHolder
                    .GetGamePieceData(FourzyPhotonManager.GetOpponentProperty(
                        Constants.REALTIME_ROOM_GAMEPIECE_KEY,
                        Constants.REALTIME_DEFAULT_GAMEPIECE_KEY)).player1Prefab;

        protected override void Awake()
        {
            base.Awake();

            Instance = this;

#if !MOBILE_SKILLZ
            FourzyPhotonManager.onCreateRoom += OnRoomCreated;
            FourzyPhotonManager.onJoinedRoom += OnJoinedRoom;
            FourzyPhotonManager.onPlayerEnteredRoom += OnPlayerEnteredRoom;
            FourzyPhotonManager.onPlayerLeftRoom += OnPlayerLeftRoom;
            FourzyPhotonManager.onJoinedLobby += OnJoinedLobby;
            FourzyPhotonManager.onRoomLeft += OnRoomLeft;
            FourzyPhotonManager.onConnectionTimeOut += OnConnectionTimedOut;
#endif
        }

        protected void OnDestroy()
        {
#if !MOBILE_SKILLZ
            FourzyPhotonManager.onCreateRoom -= OnRoomCreated;
            FourzyPhotonManager.onJoinedRoom -= OnJoinedRoom;
            FourzyPhotonManager.onPlayerEnteredRoom -= OnPlayerEnteredRoom;
            FourzyPhotonManager.onPlayerLeftRoom -= OnPlayerLeftRoom;
            FourzyPhotonManager.onJoinedLobby -= OnJoinedLobby;
            FourzyPhotonManager.onRoomLeft -= OnRoomLeft;
            FourzyPhotonManager.onConnectionTimeOut -= OnConnectionTimedOut;
#endif
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
                        .Prompt("Leaving room...", "", null, () => state = LobbyOverlayState.NONE);
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
                    lobbyButton.GetBadge().badge.SetValue(LocalizationManager.Value("invite_code"));
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

#if !MOBILE_SKILLZ
        private void OnJoinedRoom(string roomName)
        {
            if (GameManager.Instance.RejoinAbandonedGame) return;
            if ((FourzyPhotonManager.GetRoomProperty(Constants.REALTIME_ROOM_TYPE_KEY, RoomType.NONE) & displayable) == 0) return;

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
#endif

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
            if (state != LobbyOverlayState.LEAVING_ROOM)
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
                    ["playfabPlayerId"] = LoginManager.playfabId,
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
                        leavingPrompt.CloseSelf();
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
                    ["playfabPlayerId"] = LoginManager.playfabId,
                    ["timer"] = SettingsManager.Get(SettingsManager.KEY_REALTIME_TIMER),
                    ["area"] = ((Area)PlayerPrefsWrapper.GetCurrentArea()).ToString(),
                    ["isMagicEnabled"] = SettingsManager.Get(SettingsManager.KEY_REALTIME_MAGIC),
                    ["isPrivate"] = !string.IsNullOrEmpty(FourzyPhotonManager.PASSWORD),
                    ["complexityScore"] = "",
                });

            //StartBotRoutine();
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
