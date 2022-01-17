//@vadym udod

using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Toasts;
using Fourzy._Updates.UI.Widgets;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class LobbyScreen : MenuScreen
    {
        public PhotonRoomWidget widgetPrefab;
        public RectTransform widgetsParent;
        public ButtonExtended createGameButton;

        protected List<PhotonRoomWidget> rooms = new List<PhotonRoomWidget>();

        private LoadingPromptScreen _prompt;
        private LobbyState state;

        protected override void Awake()
        {
            base.Awake();

#if !MOBILE_SKILLZ
            FourzyPhotonManager.onRoomsListUpdated += OnRoomsUpdated;
#endif
            FourzyPhotonManager.onJoinedRoom += OnJoinedRoom;
            FourzyPhotonManager.onJoinedLobby += OnJoinedLobby;

            FourzyPhotonManager.onJoinRoomFailed += OnJoinRoomFailed;
            FourzyPhotonManager.onConnectionTimeOut += OnConnectionTimerOut;
            FourzyPhotonManager.onConnectedToMaster += OnConnectedToMaster;

            _prompt = PersistantMenuController.Instance
                .GetOrAddScreen<LoadingPromptScreen>()
                .SetType(LoadingPromptScreen.LoadingPromptType.BASIC);
        }

        protected void OnDestroy()
        {
#if !MOBILE_SKILLZ
            FourzyPhotonManager.onRoomsListUpdated -= OnRoomsUpdated;
#endif
            FourzyPhotonManager.onJoinedRoom -= OnJoinedRoom;
            FourzyPhotonManager.onJoinedLobby -= OnJoinedLobby;

            FourzyPhotonManager.onJoinRoomFailed -= OnJoinRoomFailed;
            FourzyPhotonManager.onConnectionTimeOut -= OnConnectionTimerOut;
            FourzyPhotonManager.onConnectedToMaster -= OnConnectedToMaster;
        }

        public override void OnBack()
        {
            base.OnBack();

            CloseSelf();
        }

        public override void Close(bool animate = true)
        {
            base.Close(animate);

            state = LobbyState.NONE;
        }

        public override void Open()
        {
            base.Open();

#if !MOBILE_SKILLZ
            OnRoomsUpdated(FourzyPhotonManager.Instance.cachedRooms);
#endif

            if (HeaderScreen.Instance)
            {
                HeaderScreen.Instance.Close();
            }
        }

        public bool CheckLobby()
        {
#if !MOBILE_SKILLZ
            if (PhotonNetwork.CurrentRoom != null)
            {
                menuController
                    .GetOrAddScreen<PromptScreen>()
                    .Prompt(
                        LocalizationManager.Value("leave_game"), 
                        LocalizationManager.Value("enter_lobby_warning"), () =>
                    {
                        state = LobbyState.LEAVING_ROOM;
                        FourzyPhotonManager.TryLeaveRoom();

                        _prompt
                            .Prompt(
                                LocalizationManager.Value("leaving_room"), 
                                "", 
                                null, 
                                LocalizationManager.Value("back"),
                                null, 
                                () => state = LobbyState.NONE)
                            .CloseOnDecline();
                    }, null)
                    .CloseOnDecline()
                    .CloseOnAccept();

                return false;
            }
            else if (FourzyPhotonManager.ConnectedAndReady)
            {
                if (!isCurrent)
                {
                    menuController.OpenScreen(this);
                }

                return true;
            }
            else
            {
                state = LobbyState.JOINING_LOBBY;

                _prompt
                    .Prompt(
                        LocalizationManager.Value("joining_lobby"), 
                        "", 
                        null,
                        LocalizationManager.Value("back"),
                        null, 
                        () => state = LobbyState.NONE)
                    .CloseOnDecline();

                FourzyPhotonManager.Instance.JoinLobby();

                return false;
            }
#else
            return false;
#endif
        }

        /// <summary>
        /// Invoke by Create Room button
        /// </summary>
        public void CreateRoom()
        {
            PersistantMenuController.Instance.GetOrAddScreen<CreateRealtimeGamePrompt>().Prompt();
        }

        public void JoinRoom(string name)
        {
            _prompt
                .Prompt(LocalizationManager.Value("getting_players_stats"), null, null, null, null, null)
                .CloseOnDecline();

            UserManager.GetPlayerRating(rating =>
            {
                FourzyPhotonManager.JoinRoom(name);

                if (_prompt.isOpened)
                {
                    _prompt.promptTitle.text = $"{LocalizationManager.Value("joining_room")} \n{name}";
                }
            }, () =>
            {
                if (_prompt.isOpened)
                {
                    _prompt.Decline(true);
                }
            });
        }

#if !MOBILE_SKILLZ
        private void OnRoomsUpdated(List<RoomInfo> _rooms)
        {
            foreach (RoomInfo roomData in _rooms)
            {
                PhotonRoomWidget entry = rooms.Find(_widget => _widget.SameName(roomData));

                //remove
                if (roomData.RemovedFromList)
                {
                    if (entry)
                    {
                        entry._Destroy();
                        rooms.Remove(entry);
                    }
                }
                //add
                else
                {
                    if (entry == null)
                    {
                        rooms.Add(Instantiate(widgetPrefab, widgetsParent).SetData(roomData));
                    }
                }
            }
        }
#endif

        private void OnJoinedRoom(string roomName)
        {
            if (isOpened)
            {
                CloseSelf();
            }

            if (_prompt.isOpened)
            {
                _prompt.Decline(true);
            }
        }

        private void OnJoinRoomFailed(string error)
        {
            if (isOpened)
            {
                GamesToastsController.ShowTopToast($"{LocalizationManager.Value("failed")}: {error}");
            }

            if (_prompt.isOpened)
            {
                _prompt.Decline(true);
            }
        }

        private void OnConnectionTimerOut()
        {
            if (isOpened && state != LobbyState.NONE)
            {
                CloseSelf();
            }

            if (_prompt.isOpened)
            {
                _prompt.Decline(true);
            }
        }

        private void OnJoinedLobby(string lobbyName)
        {
            if (state != LobbyState.JOINING_LOBBY) return;

            if (_prompt.isOpened)
            {
                _prompt.Decline(true);
            }

            CheckLobby();
        }

        private void OnConnectedToMaster()
        {
            if (state != LobbyState.LEAVING_ROOM) return;

            CheckLobby();
        }

        private enum LobbyState
        {
            NONE,
            JOINING_LOBBY,
            LEAVING_ROOM,
        }
    }
}