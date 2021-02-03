//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Helpers;
using Photon.Pun;
using Photon.Realtime;
using System;
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

        private LoadingPromptScreen _prompt;
        private LobbyOverlayState state = LobbyOverlayState.NONE;

        private GamePieceView playerOneView;
        private GamePieceView playerTwoView;

        private RoomType displayable = RoomType.DIRECT_INVITE | RoomType.LOBBY_ROOM;

        protected override void Awake()
        {
            base.Awake();

            Instance = this;

            FourzyPhotonManager.onJoinedRoom += OnJoinedRoom;
            FourzyPhotonManager.onPlayerEnteredRoom += OnPlayerEnteredRoom;
            FourzyPhotonManager.onPlayerLeftRoom += OnPlayerLeftRoom;
            FourzyPhotonManager.onJoinedLobby += OnJoinedLobby;
            FourzyPhotonManager.onRoomLeft += OnRoomLeft;
            FourzyPhotonManager.onConnectionTimeOut += OnConnectionTimedOut;
        }

        protected void OnDestroy()
        {
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

            _prompt = PersistantMenuController.instance
                .GetOrAddScreen<LoadingPromptScreen>()
                .SetType(LoadingPromptScreen.LoadingPromptType.BASIC);
        }

        public void OnActivate()
        {
            menuController
                .GetOrAddScreen<PromptScreen>()
                .Prompt("Leave room?", "You sure you want to leave online room?", () =>
                {
                    FourzyPhotonManager.TryLeaveRoom();

                    state = LobbyOverlayState.LEAVING_ROOM;
                    _prompt.Prompt("Leaving room...", "", null, () => state = LobbyOverlayState.NONE).CloseOnDecline();
                }, null)
                .CloseOnDecline()
                .CloseOnAccept();
        }

        public void SetData(Photon.Realtime.Player one, Photon.Realtime.Player two = null)
        {
            if (!isOpened) Open();

            bool isOne = one != null;
            bool isTwo = two != null;

            if (playerOneView) Destroy(playerOneView.gameObject);
            if (isOne)
            {
                playerOneView = Instantiate(GameContentManager.Instance.piecesDataHolder
                    .GetGamePiecePrefabData(UserManager.Instance.gamePieceID).player1Prefab, playerOneParent);

                playerOneView.StartBlinking();

                if (!isTwo)
                {
                    lobbyButton.GetBadge().badge.SetColor(Color.white);
                    lobbyButton.GetBadge().badge.SetValue(LocalizationManager.Value("waiting_for_opponent"));
                }
            }

            if (playerTwoView) Destroy(playerTwoView.gameObject);
            if (isTwo)
            {
                lobbyButton.GetBadge().badge.SetColor(Color.green);
                lobbyButton.GetBadge().badge.SetValue(LocalizationManager.Value("match_found"));

                playerTwoView = Instantiate(GameContentManager.Instance.piecesDataHolder
                    .GetGamePiecePrefabData(FourzyPhotonManager.GetOpponentProperty(
                        Constants.REALTIME_GAMEPIECE_KEY, 
                        Constants.REALTIME_DEFAULT_GAMEPIECE_KEY)).player1Prefab, playerTwoParent);

                playerTwoView.StartBlinking();

                //load game
                StartRoutine("load_game", InternalSettings.Current.LOBBY_GAME_LOAD_DELAY, StartGame);
                state = LobbyOverlayState.LOADING_GAME;
            }

            empty.SetActive(!isTwo);
            overlay.SetActive(isTwo);
        }

        private void OnJoinedRoom(string roomName)
        {
            if ((FourzyPhotonManager.GetRoomProperty(Constants.REALTIME_ROOM_TYPE_KEY, RoomType.NONE) 
                & displayable) == 0) return;

            if (PhotonNetwork.IsMasterClient)
            {
                SetData(PhotonNetwork.LocalPlayer);
            }
            else
            {
                SetData(PhotonNetwork.PlayerListOthers[0], PhotonNetwork.LocalPlayer);
                GameManager.Instance.CurrentOpponent = PhotonNetwork.PlayerListOthers[0].UserId;
            }

            string password = FourzyPhotonManager.GetRoomProperty(Constants.REALTIME_ROOM_PASSWORD, "");
            lobbyButton.GetBadge("code").badge.SetValue(password);
        }

        private void OnPlayerEnteredRoom(Photon.Realtime.Player other)
        {
            if ((FourzyPhotonManager.GetRoomProperty(Constants.REALTIME_ROOM_TYPE_KEY, RoomType.NONE) 
                & displayable) == 0) return;

            SetData(PhotonNetwork.LocalPlayer, other);
            GameManager.Instance.CurrentOpponent = other.UserId;
        }

        private void OnPlayerLeftRoom(Player other)
        {
            switch (state)
            {
                case LobbyOverlayState.LOADING_GAME:
                    CancelRoutine("load_game");

                    break;
            }
        }

        private void OnJoinedLobby(string lobbyName)
        {
            if (isOpened) Close();
        }

        private void OnRoomLeft()
        {
            if (isOpened) Close();

            switch (state)
            {
                case LobbyOverlayState.LEAVING_ROOM:
                    if (_prompt.isOpened) _prompt.Decline(true);

                    break;

                case LobbyOverlayState.LOADING_GAME:
                    CancelRoutine("load_game");

                    break;
            }
        }

        private void OnConnectionTimedOut()
        {
            if (_prompt.isOpened) _prompt.CloseSelf();
        }

        private void StartGame()
        {
            if (state != LobbyOverlayState.LOADING_GAME) return;

            GameManager.Instance.StartGame(GameTypeLocal.REALTIME_LOBBY_GAME);

            if (isOpened) Close();
        }

        private enum LobbyOverlayState
        {
            NONE,
            LEAVING_ROOM,
            LOADING_GAME,
        }
    }
}