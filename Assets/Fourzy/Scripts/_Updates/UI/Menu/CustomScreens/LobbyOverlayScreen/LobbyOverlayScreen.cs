//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Photon.Pun;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class LobbyOverlayScreen : MenuScreen
    {
        public static LobbyOverlayScreen Instance;

        public RectTransform playerOneParent;
        public RectTransform playerTwoParent;

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
            FourzyPhotonManager.onJoinedLobby += OnJoinedLobby;
            FourzyPhotonManager.onRoomLeft += OnRoomLeft;
            FourzyPhotonManager.onConnectionTimeOut += OnConnectionTimedOut;
        }

        protected void OnDestroy()
        {
            FourzyPhotonManager.onJoinedRoom -= OnJoinedRoom;
            FourzyPhotonManager.onPlayerEnteredRoom -= OnPlayerEnteredRoom;
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

            if (playerOneView) Destroy(playerOneView.gameObject);
            if (one != null)
            {
                playerOneView = Instantiate(GameContentManager.Instance.piecesDataHolder
                    .GetGamePiecePrefabData(UserManager.Instance.gamePieceID).player1Prefab, playerOneParent);

                playerOneView.StartBlinking();
            }

            if (playerTwoView) Destroy(playerTwoView.gameObject);
            if (two != null)
            {
                playerTwoView = Instantiate(GameContentManager.Instance.piecesDataHolder
                    .GetGamePiecePrefabData(FourzyPhotonManager.GetOpponentGamepiece()).player1Prefab, playerTwoParent);

                playerTwoView.StartBlinking();

                //load game
                GameManager.Instance.StartGame(GameTypeLocal.REALTIME_LOBBY_GAME);

                Close();
            }
        }

        private void OnJoinedRoom(string roomName)
        {
            if ((FourzyPhotonManager.GetRoomProperty(Constants.REALTIME_ROOM_TYPE_KEY, RoomType.NONE) & displayable) != 0) return;

            if (PhotonNetwork.IsMasterClient) SetData(PhotonNetwork.LocalPlayer);
            else SetData(PhotonNetwork.PlayerListOthers[0], PhotonNetwork.LocalPlayer);
        }

        private void OnPlayerEnteredRoom(Photon.Realtime.Player other)
        {
            if ((FourzyPhotonManager.GetRoomProperty(Constants.REALTIME_ROOM_TYPE_KEY, RoomType.NONE) & displayable) != 0) return;

            SetData(PhotonNetwork.LocalPlayer, other);
        }

        private void OnJoinedLobby(string lobbyName)
        {
            if (isOpened) Close();
        }

        private void OnRoomLeft()
        {
            if (isOpened) Close();

            if (state == LobbyOverlayState.LEAVING_ROOM)
            {
                if (_prompt.isOpened) _prompt.Decline(true);
            }
        }

        private void OnConnectionTimedOut()
        {
            if (_prompt.isOpened) _prompt.CloseSelf();
        }

        private enum LobbyOverlayState
        {
            NONE,
            LEAVING_ROOM,
        }
    }
}