﻿//@vadym udod

using ExitGames.Client.Photon;
using Fourzy._Updates.Managers;
using Photon.Pun;
using Photon.Realtime;
using System;
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
        public static Action<string> onJoinedLobby;
        public static Action onCreateRoomFailed;
        public static Action onJoinRoomFailed;
        public static Action onJoinRandomFailed;
        public static Action<string> onCreateRoom;
        public static Action<string> onJoinedRoom;
        public static Action<Player> onPlayerEnteredRoom;
        public static Action<Player> onPlayerLeftRoom;
        public static Action<Hashtable> onRoomPropertiesUpdate;
        public static Action onDisconnectedFromServer;
        public static Action<EventData> onEvent;
        public static Action onConnectionTimeOut;

        public static bool DEBUG = false;

        private static FourzyPhotonManager instance;
        private static string lastTask;

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

            instance.ConnectUsingSettings(false);

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

        /// <summary>
        /// Only called when autoJoinLobby == false
        /// </summary>
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();

            if (DEBUG) Debug.Log($"Connected to master.");

            UpdatePlayerGamepiece(UserManager.Instance.gamePieceID);

            onConnectedToMaster?.Invoke();

            JoinDefaultLobby();
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
                    if (DEBUG) Debug.Log($"Last task set to {lastTask}");
                    JoinRandomRoom();

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

            if (DEBUG) Debug.Log($"Failied to join room.");

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
            (PhotonNetwork.IsMasterClient && GetRoomProperty(Constants.REALTIME_PLAYER_2_READY, false)) ||
            (!PhotonNetwork.IsMasterClient && GetRoomProperty(Constants.REALTIME_PLAYER_1_READY, false));

        public static void JoinRandomRoom()
        {
            lastTask = "joinRandomRoom";

            switch (PhotonNetwork.NetworkClientState)
            {
                case ClientState.JoinedLobby: break;

                case ClientState.Authenticated:
                    instance.JoinDefaultLobby();

                    return;

                default:
                    instance.ConnectUsingSettings();

                    return;
            }

            lastTask = "";
            PhotonNetwork.JoinRandomRoom();
            instance.RunTimeoutRoutine();
        }

        public static void CreateRoom()
        {
            var options = new Photon.Realtime.RoomOptions
            {
                MaxPlayers = 2,
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
                {
                    [Constants.REALTIME_TIMER_KEY] = SettingsManager.Get(SettingsManager.KEY_REALTIME_TIMER)
                }
            };

            //create new room
            PhotonNetwork.CreateRoom(
                Guid.NewGuid().ToString(),
                options,
                null);

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

        private void JoinDefaultLobby()
        {
            PhotonNetwork.JoinLobby();

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
}
