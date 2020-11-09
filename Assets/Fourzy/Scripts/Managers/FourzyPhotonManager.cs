//@vadym udod

using ExitGames.Client.Photon;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Tools;
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
        public static Action<string> onCreateRoomFailed;
        public static Action<string> onJoinRoomFailed;
        public static Action onJoinRandomFailed;
        public static Action<string> onCreateRoom;
        public static Action<string> onJoinedRoom;
        public static Action<Player> onPlayerEnteredRoom;
        public static Action onRoomLeft;
        public static Action<Player> onPlayerLeftRoom;
        public static Action<Hashtable> onRoomPropertiesUpdate;
        public static Action<EventData> onEvent;
        public static Action onConnectionTimeOut;
        public static Action<List<FriendInfo>> onFriendsUpdated;
        public static Action<List<RoomInfo>> onRoomsListUpdated;

        public static bool DEBUG = false;
        public static TypedLobby quickmatchLobby = new TypedLobby("QuickmatchLobby", LobbyType.AsyncRandomLobby);

        private static FourzyPhotonManager instance;
        private static StackModified<KeyValuePair<string, Action>> tasks = new StackModified<KeyValuePair<string, Action>>();

        public List<RoomInfo> roomsInfo = new List<RoomInfo>();

        private Coroutine connectionTimedOutRoutine;

        public static bool IsQMLobby => PhotonNetwork.CurrentLobby != null && !PhotonNetwork.CurrentLobby.IsDefault;

        public static bool IsDefaultLobby => PhotonNetwork.CurrentLobby != null && PhotonNetwork.CurrentLobby.IsDefault;

        /// <summary>
        /// False when inside any room
        /// </summary>
        public static bool InDefaultLobby => PhotonNetwork.InLobby && PhotonNetwork.CurrentLobby.IsDefault;

        /// <summary>
        /// False when inside any room
        /// </summary>
        public static bool InQMLobby => PhotonNetwork.InLobby && !PhotonNetwork.CurrentLobby.IsDefault;

        public static bool ConnectedAndReady => PhotonNetwork.IsConnectedAndReady;

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

            if (tasks.Count > 0)
            {
                switch (tasks.Peek().Key)
                {
                    case "joinRandomRoom":
                        JoinLobby(quickmatchLobby);

                        break;

                    case "joinLobby":
                        tasks.Pop().Value();

                        break;

                    default:
                        JoinLobby();

                        break;
                }
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

            if (tasks.Count > 0)
            {
                switch (tasks.Peek().Key)
                {
                    case "joinRandomRoom":
                        //if (DEBUG) Debug.Log($"Last task set to {tasks.Peek().Key}");

                        if (PhotonNetwork.CurrentLobby.IsDefault)
                            JoinLobby(quickmatchLobby);
                        else
                        {
                            tasks.Pop().Value();
                        }

                        break;

                    case "createRoom":
                        if (PhotonNetwork.CurrentLobby.IsDefault)
                            tasks.Pop().Value();
                        else
                            JoinLobby();

                        break;
                }
            }
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);

            if (DEBUG) Debug.Log($"Failed to create new Room. {message}");

            onCreateRoomFailed?.Invoke(message);

            if (connectionTimedOutRoutine != null) StopCoroutine(connectionTimedOutRoutine);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);

            if (DEBUG) Debug.Log($"Failied to join room: {message}");

            onJoinRoomFailed?.Invoke(message);

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

            foreach (RoomInfo info in roomList)
                if (info.RemovedFromList)
                {
                    roomsInfo.RemoveAll(_room => _room.Name == info.Name);
                    if (DEBUG) Debug.Log($"Room removed: {info.Name}.");
                }
                else
                {
                    if (roomsInfo.Find(_room => _room.Name == info.Name) == null)
                        roomsInfo.Add(info);
                }

            if (DEBUG) Debug.Log($"Rooms list updated: {roomsInfo.Count}.");
            onRoomsListUpdated?.Invoke(roomsInfo);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            if (DEBUG) Debug.Log($"Room joined: {PhotonNetwork.CurrentRoom.Name}.");

            onJoinedRoom?.Invoke(PhotonNetwork.CurrentRoom.Name);

            if (connectionTimedOutRoutine != null) StopCoroutine(connectionTimedOutRoutine);

            //if room is to be removed, remove it yourself
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                roomsInfo.RemoveAll(_room => _room.Name == PhotonNetwork.CurrentRoom.Name);
                if (DEBUG) Debug.Log($"Room removed: {PhotonNetwork.CurrentRoom.Name}.");
            }
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

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();

            if (DEBUG) Debug.Log($"Room left.");

            onRoomLeft?.Invoke();
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
            if (PhotonNetwork.CurrentRoom == null) return;

            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable()
            {
                [Constants.REALTIME_PLAYER_1_REMATCH] = false,
                [Constants.REALTIME_PLAYER_2_REMATCH] = false,
            });
        }

        public static void JoinRandomRoom()
        {
            tasks.Push(new KeyValuePair<string, Action>("joinRandomRoom", JoinRandomRoom));

            if (!PhotonNetwork.IsConnected)
            {
                instance.ConnectUsingSettings();

                return;
            }
            else if (!InQMLobby)
            {
                instance.JoinLobby(quickmatchLobby);

                return;
            }

            tasks.Pop();

            PhotonNetwork.JoinRandomRoom();
            instance.RunTimeoutRoutine();
        }

        public static void JoinRoom(string roomName)
        {
            tasks.Push(new KeyValuePair<string, Action>("joinRoom", () => JoinRoom(roomName)));

            if (!PhotonNetwork.IsConnected)
            {
                instance.ConnectUsingSettings();
                return;
            }
            else if (PhotonNetwork.CurrentLobby == null)
            {
                instance.JoinLobby();
                return;
            }
            else if (
                PhotonNetwork.NetworkClientState == ClientState.Authenticating ||
                PhotonNetwork.NetworkClientState == ClientState.Leaving)
            {
                return;
            }

            tasks.Pop();

            PhotonNetwork.JoinRoom(roomName);
        }

        public static void CreateRoom(RoomType type, string expectedUser = "")
        {
            tasks.Push(new KeyValuePair<string, Action>("createRoom", () => CreateRoom(type, expectedUser)));

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

            tasks.Pop();

            Hashtable properties = new ExitGames.Client.Photon.Hashtable()
            {
                [Constants.REALTIME_TIMER_KEY] = SettingsManager.Get(SettingsManager.KEY_REALTIME_TIMER),
                [Constants.REALTIME_GAMEPIECE_KEY] = UserManager.Instance.gamePieceID,
            };
            string roomName;
            List<string> customProperties = new List<string>() { Constants.REALTIME_GAMEPIECE_KEY, };
            List<string> expectedUsers = new List<string>();

            if (!string.IsNullOrEmpty(expectedUser)) expectedUsers.Add(expectedUser);

            customProperties.Add(Constants.REALTIME_ROOM_TYPE_KEY);
            properties.Add(Constants.REALTIME_ROOM_TYPE_KEY, (int)type);

            switch (type)
            {
                case RoomType.LOBBY_ROOM:
                    roomName = $"{UserManager.Instance.userName}'s room";

                    break;

                case RoomType.DIRECT_INVITE:
                    roomName = $"{UserManager.Instance.userName} challenged you!";

                    break;

                case RoomType.QUICKMATCH:
                default:
                    roomName = Guid.NewGuid().ToString();

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

        public void ConnectUsingSettings(bool runTimeOutRoutine = true)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";

            if (runTimeOutRoutine) RunTimeoutRoutine();
        }

        public void JoinLobby(TypedLobby lobby = null)
        {
            TypedLobby _copy = lobby;
            tasks.Push(new KeyValuePair<string, Action>("joinLobby", () => JoinLobby(_copy)));

            if (!PhotonNetwork.IsConnected)
            {
                ConnectUsingSettings();

                return;
            }
            else if (
                PhotonNetwork.NetworkClientState == ClientState.Authenticating ||
                PhotonNetwork.NetworkClientState == ClientState.Leaving ||
                PhotonNetwork.NetworkClientState == ClientState.PeerCreated)

                return;

            tasks.Pop();
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

    [Flags]
    public enum RoomType
    {
        NONE = 0,
        QUICKMATCH = 1,
        DIRECT_INVITE = 2,
        LOBBY_ROOM = 4,

    }
}
