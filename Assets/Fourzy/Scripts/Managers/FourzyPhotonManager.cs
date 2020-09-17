﻿//@vadym udod

using ExitGames.Client.Photon;
using Fourzy._Updates.Managers;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    public class FourzyPhotonManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public static FourzyPhotonManager Instance
        {
            get
            {
                if (instance == null) Initialize();

                return instance;
            }
        }

        //callback events
        public static Action onConnectedToMaster;
        public static Action onDisconnectedFromServer;
        public static Action<string> onJoinedLobby;
        public static Action onCreateRoomFailed;
        public static Action onJoinRoomFailed;
        public static Action onJoinRandomFailed;
        public static Action<string> onCreateRoom;
        public static Action<string> onJoinedRoom;
        public static Action<Player> onPlayerEnteredRoom;
        public static Action<Player> onPlayerLeftRoom;
        public static Action<Hashtable> onRoomPropertiesUpdate;
        public static Action<EventData> onEvent;
        public static Action onConnectionTimeOut;
        public static Action<List<FriendInfo>> onFriendsUpdated;
        public static Action<List<RoomInfo>> onRoomsListUpdated;

        public static bool DEBUG = false;

        private static FourzyPhotonManager instance;
        private static string lastTask;
        private static Action lastTaskAction;
        private static TypedLobby quickmatchLobby = new TypedLobby("QuickmatchLobby", LobbyType.AsyncRandomLobby);

        public List<RoomInfo> roomsInfo = new List<RoomInfo>();

        private Coroutine connectionTimedOutRoutine;

        public static void Initialize(bool DEBUG = true)
        {
            FourzyPhotonManager.DEBUG = DEBUG;

            Initialize();
        }

        public static void Initialize()
        {
            if (instance != null) return;

            GameObject go = new GameObject("FourzyPhotonManager");
            go.transform.SetParent(null);
            instance = go.AddComponent<FourzyPhotonManager>();

            DontDestroyOnLoad(go);
        }

        public static void Connect()
        {
            instance.ConnectUsingSettings(false);
        }

        #region Callbacks

        public override void OnEnable()
        {
            base.OnEnable();

            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            base.OnDisable();

            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public override void OnConnected()
        {
            base.OnConnected();

            if (DEBUG) Debug.Log($"Connected to photon.");
        }

        public override void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            base.OnFriendListUpdate(friendList);

            onFriendsUpdated?.Invoke(friendList);

            if (DEBUG) Debug.Log("Photon friends list updated");
        }

        /// <summary>
        /// Only called when autoJoinLobby == false
        /// </summary>
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();

            if (DEBUG) Debug.Log($"Connected to master.");

            UpdatePlayerGamepiece(UserManager.Instance.gamePieceID);

            onConnectedToMaster?.Invoke();

            switch (lastTask)
            {
                case "joinRandomRoom":
                    JoinLobby(quickmatchLobby);

                    break;

                default:
                    JoinLobby();

                    break;
            }
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();

            if (connectionTimedOutRoutine != null) StopCoroutine(connectionTimedOutRoutine);

            if (DEBUG) Debug.Log($"Joined lobby: {(string.IsNullOrEmpty(PhotonNetwork.CurrentLobby.Name) ? "DEFAULT" : PhotonNetwork.CurrentLobby.Name)}.");

            //update name
            PhotonNetwork.NickName = UserManager.Instance.userName;

            onJoinedLobby?.Invoke(PhotonNetwork.CurrentLobby.Name);

            switch (lastTask)
            {
                case "joinRandomRoom":
                    if (PhotonNetwork.CurrentLobby.IsDefault)
                        JoinLobby(quickmatchLobby);
                    else
                    {
                        if (DEBUG) Debug.Log($"Last task set to {lastTask}");
                        lastTaskAction();
                    }

                    break;

                case "createRoom":
                    if (PhotonNetwork.CurrentLobby.IsDefault)
                    {
                        if (DEBUG) Debug.Log($"Last task set to {lastTask}");
                        lastTaskAction();
                    }
                    else
                        JoinLobby();

                    break;
            }
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);

            if (DEBUG) Debug.Log($"Failed to create new Room.");

            onCreateRoomFailed?.Invoke();

            if (connectionTimedOutRoutine != null) StopCoroutine(connectionTimedOutRoutine);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);

            if (DEBUG) Debug.Log($"Failied to join room: {message}");

            onJoinRoomFailed?.Invoke();

            if (connectionTimedOutRoutine != null) StopCoroutine(connectionTimedOutRoutine);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            if (DEBUG) Debug.Log($"Failied to join random room. {returnCode} code {message} message.");

            onJoinRandomFailed?.Invoke();

            if (connectionTimedOutRoutine != null) StopCoroutine(connectionTimedOutRoutine);
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();

            if (DEBUG) Debug.Log($"Room created: {PhotonNetwork.CurrentRoom.Name}.");

            onCreateRoom?.Invoke(PhotonNetwork.CurrentRoom.Name);

            if (connectionTimedOutRoutine != null) StopCoroutine(connectionTimedOutRoutine);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);

            roomsInfo = roomList;
            onRoomsListUpdated?.Invoke(roomList);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            if (DEBUG) Debug.Log($"Room joined: {PhotonNetwork.CurrentRoom.Name}.");

            onJoinedRoom?.Invoke(PhotonNetwork.CurrentRoom.Name);

            if (connectionTimedOutRoutine != null) StopCoroutine(connectionTimedOutRoutine);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            if (DEBUG) Debug.Log($"Player connected: {newPlayer.NickName}.");

            onPlayerEnteredRoom?.Invoke(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            if (DEBUG) Debug.Log($"Player disconnected: {otherPlayer.NickName}.");

            onPlayerLeftRoom?.Invoke(otherPlayer);
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);
            if (DEBUG) Debug.Log($"New/Changed room properties arrived.");

            onRoomPropertiesUpdate?.Invoke(propertiesThatChanged);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);

            if (DEBUG) Debug.Log($"Disconnected from server.");

            onDisconnectedFromServer?.Invoke();
        }

        #endregion

        public static void SetClientReady()
        {
            if (PhotonNetwork.CurrentRoom == null) return;

            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable()
            {
                [PhotonNetwork.IsMasterClient ? Constants.REALTIME_PLAYER_1_READY : Constants.REALTIME_PLAYER_2_READY] = true,
            });
        }
        public static void SetClientRematchReady()
        {
            if (PhotonNetwork.CurrentRoom == null) return;

            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable()
            {
                [PhotonNetwork.IsMasterClient ? Constants.REALTIME_PLAYER_1_REMATCH : Constants.REALTIME_PLAYER_2_REMATCH] = true,
            });
        }

        public static void TryLeaveRoom()
        {
            if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.NetworkClientState != Photon.Realtime.ClientState.Leaving)
                PhotonNetwork.LeaveRoom();
        }

        public static T GetRoomProperty<T>(string key, T defaultValue)
        {
            if (PhotonNetwork.CurrentRoom == null) return defaultValue;

            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key))
                return (T)PhotonNetwork.CurrentRoom.CustomProperties[key];
            else
                return defaultValue;
        }

        public static bool CheckPlayersReady() =>
            (GetRoomProperty(Constants.REALTIME_PLAYER_2_READY, false)) && (GetRoomProperty(Constants.REALTIME_PLAYER_1_READY, false));

        public static bool CheckPlayersRematchReady() =>
            GetRoomProperty(Constants.REALTIME_PLAYER_1_REMATCH, false) && GetRoomProperty(Constants.REALTIME_PLAYER_2_REMATCH, false);

        public static bool CheckPlayerRematchReady() =>
            PhotonNetwork.IsMasterClient ? GetRoomProperty(Constants.REALTIME_PLAYER_1_REMATCH, false) : GetRoomProperty(Constants.REALTIME_PLAYER_2_REMATCH, false);

        public static void ResetRematchState()
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable()
            {
                [Constants.REALTIME_PLAYER_1_REMATCH] = false,
                [Constants.REALTIME_PLAYER_2_REMATCH] = false,
            });
        }

        public static void JoinRandomRoom()
        {
            lastTask = "joinRandomRoom";
            lastTaskAction = JoinRandomRoom;

            if (!PhotonNetwork.IsConnected)
            {
                instance.ConnectUsingSettings();
                return;
            }
            else if (PhotonNetwork.CurrentLobby == null || PhotonNetwork.CurrentLobby.IsDefault)
            {
                instance.JoinLobby(quickmatchLobby);
                return;
            }

            lastTask = "";
            lastTaskAction = null;

            PhotonNetwork.JoinRandomRoom();
            instance.RunTimeoutRoutine();
        }

        public static void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        public static void CreateRoom(RoomType type, string expectedUser = "")
        {
            lastTask = "createRoom";

            lastTaskAction = () => CreateRoom(type, expectedUser);

            if (!PhotonNetwork.IsConnected)
            {
                instance.ConnectUsingSettings();
                return;
            }
            else if (PhotonNetwork.CurrentLobby == null)
            {
                switch (type)
                {
                    case RoomType.QUICKMATCH:
                        instance.JoinLobby(quickmatchLobby);

                        break;

                    default:
                        instance.JoinLobby();

                        break;
                }

                return;
            }

            lastTask = "";
            lastTaskAction = null;

            Hashtable properties = new ExitGames.Client.Photon.Hashtable()
            {
                [Constants.REALTIME_TIMER_KEY] = SettingsManager.Get(SettingsManager.KEY_REALTIME_TIMER),
                [Constants.REALTIME_GAMEPIECE_KEY] = UserManager.Instance.gamePieceID,
            };
            string roomName;
            List<string> customProperties = new List<string>() { Constants.REALTIME_GAMEPIECE_KEY, };
            List<string> expectedUsers = new List<string>();

            switch (type)
            {
                case RoomType.LOBBY_ROOM:
                    roomName = $"{UserManager.Instance.userName}'s room";
                    customProperties.Add(Constants.REALTIME_ROOM_TYPE_KEY);
                    properties.Add(Constants.REALTIME_ROOM_TYPE_KEY, (int)type);

                    break;

                case RoomType.DIRECT_INVITE:
                    roomName = $"{UserManager.Instance.userName} challenged you!";

                    if (!string.IsNullOrEmpty(expectedUser)) expectedUsers.Add(expectedUser);

                    break;

                default:
                    roomName = Guid.NewGuid().ToString();
                    //isVisible = false;

                    break;
            }

            //create new room
            PhotonNetwork.CreateRoom(
                roomName,
                new Photon.Realtime.RoomOptions
                {
                    MaxPlayers = 2,
                    CustomRoomProperties = properties,
                    CustomRoomPropertiesForLobby = customProperties.ToArray(),
                },
                null,
                expectedUsers.ToArray());

            instance.RunTimeoutRoutine();
        }

        public static void UpdatePlayerGamepiece(string value)
        {
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable()
                {
                    [Constants.REALTIME_GAMEPIECE_KEY] = value,
                });
        }

        public static string GetOpponentGamepiece()
        {
            Hashtable _porps = PhotonNetwork.PlayerListOthers[0].CustomProperties;
            return _porps.ContainsKey(Constants.REALTIME_GAMEPIECE_KEY) ? _porps[Constants.REALTIME_GAMEPIECE_KEY].ToString() : Constants.REALTIME_DEFAULT_GAMEPIECE_KEY;
        }

        public void OnEvent(EventData photonEvent)
        {
            onEvent?.Invoke(photonEvent);
        }

        private void ConnectUsingSettings(bool runTimeOutRoutine = true)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";

            if (runTimeOutRoutine) RunTimeoutRoutine();
        }

        private void JoinLobby(TypedLobby lobby = null)
        {
            PhotonNetwork.JoinLobby(lobby);

            RunTimeoutRoutine();
        }

        private void RunTimeoutRoutine()
        {
            if (connectionTimedOutRoutine != null) StopCoroutine(connectionTimedOutRoutine);
            connectionTimedOutRoutine = StartCoroutine(OnConnectionTimedOut());
        }

        private System.Collections.IEnumerator OnConnectionTimedOut()
        {
            yield return new WaitForSeconds(Constants.PHOTON_CONNECTION_WAIT_TIME);

            PhotonNetwork.Disconnect();
            onConnectionTimeOut?.Invoke();

            connectionTimedOutRoutine = null;
        }
    }

    public enum RoomType
    {
        QUICKMATCH,
        DIRECT_INVITE,
        LOBBY_ROOM,
    }
}
