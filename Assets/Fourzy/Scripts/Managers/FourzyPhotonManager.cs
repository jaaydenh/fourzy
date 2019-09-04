//@vadym udod

using ExitGames.Client.Photon;
using Photon;
using System;
using UnityEngine;

namespace Fourzy
{
    public class FourzyPhotonManager : PunBehaviour
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
        public static Action onJoinRandomRoomFailed;
        public static Action<string> onRoomCreated;
        public static Action<string> onRoomJoined;
        public static Action<PhotonPlayer> onPlayerConnected;
        public static Action<PhotonPlayer> onPlayerDisconnected;
        public static Action<Hashtable> onRoomCustomPropertiesChanged;

        public static bool DEBUG = false;

        private static FourzyPhotonManager instance;

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

            //initializing this will also connect to photon cloud
            PhotonNetwork.ConnectUsingSettings(Application.version);

            DontDestroyOnLoad(go);
        }

        #region Callbacks

        public override void OnConnectedToPhoton()
        {
            base.OnConnectedToPhoton();

            if (DEBUG) Debug.Log($"Connected to photon.");
        }

        /// <summary>
        /// Only called when autoJoinLobby == false
        /// </summary>
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();

            if (DEBUG) Debug.Log($"Connected to master.");

            onConnectedToMaster?.Invoke();

            //join default lobby
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();

            if (DEBUG) Debug.Log($"Joined lobby: {(string.IsNullOrEmpty(PhotonNetwork.lobby.Name) ? "DEFAULT" : PhotonNetwork.lobby.Name)}.");

            //update name
            PhotonNetwork.player.NickName = UserManager.Instance.userName;

            onJoinedLobby?.Invoke(PhotonNetwork.lobby.Name);
        }

        public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
        {
            base.OnPhotonCreateRoomFailed(codeAndMsg);

            if (DEBUG) Debug.Log($"Failed to create new Room.");

            onCreateRoomFailed?.Invoke();
        }

        public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
        {
            base.OnPhotonJoinRoomFailed(codeAndMsg);

            if (DEBUG) Debug.Log($"Failied to join room.");

            onJoinRoomFailed?.Invoke();
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            base.OnPhotonRandomJoinFailed(codeAndMsg);

            if (DEBUG) Debug.Log($"Failied to join random room.");

            onJoinRandomRoomFailed?.Invoke();
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();

            if (DEBUG) Debug.Log($"Room created: {PhotonNetwork.room.Name}.");

            onRoomCreated?.Invoke(PhotonNetwork.room.Name);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            if (DEBUG) Debug.Log($"Room joined: {PhotonNetwork.room}.");

            onRoomJoined?.Invoke(PhotonNetwork.room.Name);
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            base.OnPhotonPlayerConnected(newPlayer);

            if (DEBUG) Debug.Log($"Player connected: {newPlayer.NickName}.");

            onPlayerConnected?.Invoke(newPlayer);
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            base.OnPhotonPlayerDisconnected(otherPlayer);

            if (DEBUG) Debug.Log($"Player disconnected: {otherPlayer.NickName}.");

            onPlayerDisconnected?.Invoke(otherPlayer);
        }

        public override void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
        {
            base.OnPhotonCustomRoomPropertiesChanged(propertiesThatChanged);

            if (DEBUG) Debug.Log($"New/Changed room properties arrived.");

            onRoomCustomPropertiesChanged?.Invoke(propertiesThatChanged);
        }

        #endregion

        public static void SetClientReady()
        {
            if (PhotonNetwork.room == null) return;

            PhotonNetwork.room.SetCustomProperties(new Hashtable()
            {
                [PhotonNetwork.isMasterClient ? Constants.PLAYER_1_READY : Constants.PLAYER_2_READY] = true,
            });
        }

        public static T GetRoomProperty<T>(string key, T defaultValue)
        {
            if (PhotonNetwork.room == null) return defaultValue;

            if (PhotonNetwork.room.CustomProperties.ContainsKey(key))
                return (T)PhotonNetwork.room.CustomProperties[key];
            else
                return defaultValue;
        }

        public static bool CheckPlayersReady() =>
            (PhotonNetwork.isMasterClient && GetRoomProperty(Constants.PLAYER_2_READY, false)) ||
            (!PhotonNetwork.isMasterClient && GetRoomProperty(Constants.PLAYER_1_READY, false));
    }
}
