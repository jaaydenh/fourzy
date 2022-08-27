//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

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
        public static Action<Player> onPlayerLeftRoom;
        public static Action<Hashtable> onRoomPropertiesUpdate;
        public static Action<EventData> onEvent;
        public static Action<List<FriendInfo>> onFriendsUpdated;
        public static Action<List<RoomInfo>> onRoomsListUpdated;
        public static Action<Player, Hashtable> onPlayerPpopertiesUpdate;
        public List<RoomInfo> cachedRooms = new List<RoomInfo>();
        public static Action onRoomLeft;
        public static Action onConnectionTimeOut;

        public static bool DEBUG = false;
        public static string PASSWORD = "";

        private static FourzyPhotonManager instance;
        private static StackModified<KeyValuePair<string, Action>> tasks = new StackModified<KeyValuePair<string, Action>>();


        private Coroutine connectionTimedOutRoutine;

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

            if (DEBUG)
            {
                Debug.Log($"Connected to photon.");
            }
        }

        public override void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            base.OnFriendListUpdate(friendList);

            if (DEBUG)
            {
                Debug.Log("Photon friends list updated");
            }

            onFriendsUpdated?.Invoke(friendList);
        }

        /// <summary>
        /// Only called when autoJoinLobby == false
        /// </summary>
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();

            if (DEBUG)
            {
                Debug.Log($"Connected to master.");
            }

            SetMyProperty(Constants.REALTIME_ROOM_GAMEPIECE_KEY, UserManager.Instance.gamePieceId);
            SetMyProperty(Constants.REALTIME_MID_KEY, LoginManager.masterAccountId);

            onConnectedToMaster?.Invoke();

            if (tasks.Count > 0)
            {
                switch (tasks.Peek().Key)
                {
                    case "joinLobby":
                        tasks.Pop().Value();

                        break;
                }
            }
            else
            {
                if (PhotonNetwork.NetworkClientState != ClientState.JoiningLobby)
                {
                    JoinLobby();
                }
            }
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();

            if (connectionTimedOutRoutine != null)
            {
                StopCoroutine(connectionTimedOutRoutine);
            }

            if (DEBUG)
            {
                Debug.Log($"Joined lobby: {(string.IsNullOrEmpty(PhotonNetwork.CurrentLobby.Name) ? "DEFAULT" : PhotonNetwork.CurrentLobby.Name)}.");
            }

            //update name
            PhotonNetwork.NickName = UserManager.Instance.userName;
            onJoinedLobby?.Invoke(PhotonNetwork.CurrentLobby.Name);

            if (tasks.Count > 0)
            {
                switch (tasks.Peek().Key)
                {
                    case "joinRandomRoom":
                    case "createRoom":
                        tasks.Pop().Value();

                        break;
                }
            }

            //clear all rooms
            foreach (RoomInfo room in cachedRooms)
            {
                room.RemovedFromList = true;
            }
            onRoomsListUpdated?.Invoke(cachedRooms);
            cachedRooms.Clear();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);

            if (DEBUG)
            {
                Debug.Log($"Failed to create new Room. {message}");
            }

            onCreateRoomFailed?.Invoke(message);

            if (connectionTimedOutRoutine != null)
            {
                StopCoroutine(connectionTimedOutRoutine);
            }

            AnalyticsManager.Instance.LogEvent(
                "photonError",
                AnalyticsManager.AnalyticsProvider.ALL,
                new KeyValuePair<string, object>("code", returnCode),
                new KeyValuePair<string, object>("message", message));
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);

            if (DEBUG)
            {
                Debug.Log($"Failied to join room: {message}");
            }

            onJoinRoomFailed?.Invoke(message);

            if (connectionTimedOutRoutine != null)
            {
                StopCoroutine(connectionTimedOutRoutine);
            }

            AnalyticsManager.Instance.LogEvent(
               "photonError",
               AnalyticsManager.AnalyticsProvider.ALL,
               new KeyValuePair<string, object>("code", returnCode),
               new KeyValuePair<string, object>("message", message));
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);

            if (DEBUG)
            {
                Debug.Log($"Failied to join random room. {returnCode} code {message} message.");
            }

            onJoinRandomFailed?.Invoke();

            //ignore "No match found" error
            if (returnCode != 32760)
            {
                AnalyticsManager.Instance.LogEvent(
                    "photonError",
                    AnalyticsManager.AnalyticsProvider.ALL,
                    new KeyValuePair<string, object>("code", returnCode),
                    new KeyValuePair<string, object>("message", message));
            }

            if (connectionTimedOutRoutine != null)
            {
                StopCoroutine(connectionTimedOutRoutine);
            }
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();

            if (DEBUG)
            {
                Debug.Log($"Room created: {PhotonNetwork.CurrentRoom.Name}.");
            }

            onCreateRoom?.Invoke(PhotonNetwork.CurrentRoom.Name);

            if (connectionTimedOutRoutine != null)
            {
                StopCoroutine(connectionTimedOutRoutine);
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);

            List<RoomInfo> toRemove = new List<RoomInfo>();
            foreach (RoomInfo info in roomList)
            {
                RoomInfo entry = cachedRooms.Find(_room => _room.Name == info.Name);

                if (entry != null)
                {
                    if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                    {
                        toRemove.Add(entry);
                        entry.RemovedFromList = true;
                    }
                }
                else
                {
                    cachedRooms.Add(info);
                }
            }

            onRoomsListUpdated?.Invoke(cachedRooms);

            foreach (RoomInfo _toRemove in toRemove)
            {
                RoomInfo ___toRemove = cachedRooms.Find(_info => _info.Name == _toRemove.Name);

                if (___toRemove != null)
                {
                    cachedRooms.Remove(___toRemove);
                    if (DEBUG)
                    {
                        Debug.Log($"Room removed: {___toRemove.Name}.");
                    }
                }
            }
            toRemove.Clear();

            if (DEBUG)
            {
                Debug.Log($"Rooms list updated: {cachedRooms.Count}.");
            }
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            if (DEBUG)
            {
                Debug.Log($"Room joined: {PhotonNetwork.CurrentRoom.Name}.");
            }

            onJoinedRoom?.Invoke(PhotonNetwork.CurrentRoom.Name);

            if (connectionTimedOutRoutine != null)
            {
                StopCoroutine(connectionTimedOutRoutine);
            }
        }

        public override void OnLeftLobby()
        {
            base.OnLeftLobby();

            if (DEBUG)
            {
                Debug.Log($"Lobby left");
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);

            if (DEBUG)
            {
                Debug.Log($"Player connected: {newPlayer.NickName}.");
            }

            onPlayerEnteredRoom?.Invoke(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            if (DEBUG)
            {
                Debug.Log($"Player disconnected: {otherPlayer.NickName}.");
            }

            onPlayerLeftRoom?.Invoke(otherPlayer);
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);

            if (DEBUG)
            {
                Debug.Log($"New/Changed room properties arrived. {propertiesThatChanged.ToStringFull()}");
            }

            onRoomPropertiesUpdate?.Invoke(propertiesThatChanged);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);

            if (DEBUG)
            {
                Debug.Log($"Disconnected from server.");
            }

            onDisconnectedFromServer?.Invoke();
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();

            if (DEBUG)
            {
                Debug.Log($"Room left.");
            }

            PASSWORD = "";
            onRoomLeft?.Invoke();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

            onPlayerPpopertiesUpdate?.Invoke(targetPlayer, changedProps);
        }

#endregion

        public static void SetClientReadyState(bool state)
        {
            SetRoomProperty(PhotonNetwork.IsMasterClient ? Constants.REALTIME_ROOM_PLAYER_1_READY : Constants.REALTIME_ROOM_PLAYER_2_READY, state);
        }

        public static void SetClientsReadyState(bool state)
        {
            SetRoomProperty(Constants.REALTIME_ROOM_PLAYER_1_READY, state);
            SetRoomProperty(Constants.REALTIME_ROOM_PLAYER_2_READY, state);
        }

        public static void SetClientRematchReady()
        {
            SetRoomProperty(PhotonNetwork.IsMasterClient ? Constants.REALTIME_ROOM_PLAYER_1_REMATCH : Constants.REALTIME_ROOM_PLAYER_2_REMATCH, true);
        }

        public static void SetRoomProperty<T>(string key, T value)
        {
            if (PhotonNetwork.CurrentRoom == null) return;

            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable()
            {
                [key] = value,
            });
        }

        public static void TryLeaveRoom()
        {
            if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.NetworkClientState != ClientState.Leaving)
            {
                PhotonNetwork.LeaveRoom();
            }
        }

        public static T GetRoomProperty<T>(string key, T defaultValue)
        {
            if (PhotonNetwork.CurrentRoom == null) return defaultValue;

            return GetRoomProperty(PhotonNetwork.CurrentRoom.CustomProperties, key, defaultValue);
        }

        public static T GetRoomProperty<T>(Hashtable values, string key, T defaultValue)
            => values.ContainsKey(key) ? (T)values[key] : defaultValue;

        public static bool CheckPlayersReady() =>
               GetRoomProperty(Constants.REALTIME_ROOM_PLAYER_2_READY, false) && 
               GetRoomProperty(Constants.REALTIME_ROOM_PLAYER_1_READY, false);

        public static bool CheckPlayersRematchReady() =>
            GetRoomProperty(Constants.REALTIME_ROOM_PLAYER_1_REMATCH, false) && 
            GetRoomProperty(Constants.REALTIME_ROOM_PLAYER_2_REMATCH, false);

        public static bool CheckPlayerRematchReady() =>
            PhotonNetwork.IsMasterClient ? 
                GetRoomProperty(Constants.REALTIME_ROOM_PLAYER_1_REMATCH, false) : 
                GetRoomProperty(Constants.REALTIME_ROOM_PLAYER_2_REMATCH, false);

        public static void ResetRematchState()
        {
            if (PhotonNetwork.CurrentRoom == null) return;

            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable()
            {
                [Constants.REALTIME_ROOM_PLAYER_1_REMATCH] = false,
                [Constants.REALTIME_ROOM_PLAYER_2_REMATCH] = false,
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
            else if (PhotonNetwork.CurrentLobby == null)
            {
                instance.JoinLobby();

                return;
            }

            tasks.Pop();

            Hashtable customProperties = new Hashtable();
            customProperties.Add(Constants.REALTIME_ROOM_TYPE_KEY, RoomType.QUICKMATCH);

            PhotonNetwork.JoinRandomRoom(customProperties, 0);
            instance.RunTimeoutRoutine();
        }

        public static void JoinRoom(string roomName, bool rejoin = false)
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

            if (rejoin)
            {
                PhotonNetwork.RejoinRoom(roomName);
            }
            else
            {
                PhotonNetwork.JoinRoom(roomName);
            }
        }

        public static void CreateRoom(RoomType type, string password = "", string expectedUser = "")
        {
            tasks.Push(new KeyValuePair<string, Action>("createRoom", () => CreateRoom(type, password, expectedUser)));

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

            tasks.Pop();

            PASSWORD = password;

            Hashtable properties = new ExitGames.Client.Photon.Hashtable()
            {
                [Constants.REALTIME_ROOM_TIMER_KEY] =
                    SettingsManager.Get(SettingsManager.KEY_REALTIME_TIMER),
                [Constants.REALTIME_ROOM_MAGIC_KEY] =
                    SettingsManager.Get(SettingsManager.KEY_REALTIME_MAGIC) ? 2 : 0,
                [Constants.REALTIME_ROOM_GAMEPIECE_KEY] =
                    UserManager.Instance.gamePieceId,
                [Constants.REALTIME_ROOM_AREA] =
                    PlayerPrefsWrapper.GetCurrentArea(),
                [Constants.REALTIME_ROOM_RATING_KEY] =
                    UserManager.Instance.lastCachedRating,
                [Constants.REALTIME_ROOM_GAMES_TOTAL_KEY] =
                    UserManager.Instance.totalRatedGames,
            };
            string roomName;
            List<string> lobbyProperties = new List<string>() { 
                Constants.REALTIME_ROOM_GAMEPIECE_KEY, 
                Constants.REALTIME_ROOM_RATING_KEY,
                Constants.REALTIME_ROOM_GAMES_TOTAL_KEY,
                Constants.REALTIME_ROOM_MAGIC_KEY,
                Constants.REALTIME_ROOM_TIMER_KEY,
                Constants.REALTIME_ROOM_AREA
            };
            List<string> expectedUsers = new List<string>();

            if (!string.IsNullOrEmpty(expectedUser)) expectedUsers.Add(expectedUser);

            lobbyProperties.Add(Constants.REALTIME_ROOM_TYPE_KEY);
            properties.Add(Constants.REALTIME_ROOM_TYPE_KEY, (int)type);

            if (!string.IsNullOrEmpty(password))
            {
                lobbyProperties.Add(Constants.REALTIME_ROOM_PASSWORD);
                properties.Add(Constants.REALTIME_ROOM_PASSWORD, password);
            }

            switch (type)
            {
                case RoomType.LOBBY_ROOM:
                    roomName = password;

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
                new RoomOptions
                {
                    MaxPlayers = 2,
                    CustomRoomProperties = properties,
                    CustomRoomPropertiesForLobby = lobbyProperties.ToArray(),
                    PublishUserId = true,
                },
                null,
                expectedUsers.ToArray());

            instance.RunTimeoutRoutine();
        }

        public static T GetOpponentProperty<T>(string key, T defaultValue)
        {
            if (PhotonNetwork.PlayerListOthers == null || PhotonNetwork.PlayerListOthers.Length == 0) 
                return defaultValue;

            return GetPlayerProperty(PhotonNetwork.PlayerListOthers[0], key, defaultValue);
        }

        public static int GetOpponentTotalGames()
        {
            return
                GetOpponentProperty(Constants.REALTIME_WINS_KEY, 0) +
                GetOpponentProperty(Constants.REALTIME_LOSES_KEY, 0) +
                GetOpponentProperty(Constants.REALTIME_DRAWS_KEY, 0);
        }

        public static void SetMyProperty(string key, object value)
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { [key] = value, });
            }
        }

        public static T GetPlayerProperty<T>(Player player, string key, T defaultValue)
        {
            if (player == null) return defaultValue;
            if (!player.CustomProperties.ContainsKey(key)) return defaultValue;

            return (T)player.CustomProperties[key];
        }

        public static int GetPlayerTotalGamesCount(Player player)
        {
            return GetPlayerProperty(player, Constants.REALTIME_WINS_KEY, 0) +
                GetPlayerProperty(player, Constants.REALTIME_LOSES_KEY, 0) +
                GetPlayerProperty(player, Constants.REALTIME_DRAWS_KEY, 0);
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
            {
                return;
            }

            tasks.Pop();
            PhotonNetwork.JoinLobby(lobby);
            RunTimeoutRoutine();
        }

        public void JoinOrCreateRoom(string roomName)
        {
            Debug.Log("First time connecting to Master Server, creating and joining room");
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 2 }, default);
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
        SKILLZ_SYNC,
    }
}
