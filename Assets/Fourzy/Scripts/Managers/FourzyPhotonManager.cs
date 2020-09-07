//@vadym udod

using ExitGames.Client.Photon;
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
        public static Action<EventData> onEvent;

        public static bool DEBUG = false;

        private static FourzyPhotonManager instance;
        private static string lastTask;

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

            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";

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

            onConnectedToMaster?.Invoke();

            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();

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
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);

            if (DEBUG) Debug.Log($"Failied to join room.");

            onJoinRoomFailed?.Invoke();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            if (DEBUG) Debug.Log($"Failied to join random room.");

            onJoinRandomFailed?.Invoke();
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();

            if (DEBUG) Debug.Log($"Room created: {PhotonNetwork.CurrentRoom.Name}.");

            onCreateRoom?.Invoke(PhotonNetwork.CurrentRoom.Name);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            if (DEBUG) Debug.Log($"Room joined: {PhotonNetwork.CurrentRoom.Name}.");

            onJoinedRoom?.Invoke(PhotonNetwork.CurrentRoom.Name);
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

        #endregion


        public void OnEvent(EventData photonEvent)
        {
            onEvent?.Invoke(photonEvent);
        }

        public static void SetClientReady()
        {
            if (PhotonNetwork.CurrentRoom == null) return;

            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable()
            {
                [PhotonNetwork.IsMasterClient ? Constants.PLAYER_1_READY : Constants.PLAYER_2_READY] = true,
            });
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
            (PhotonNetwork.IsMasterClient && GetRoomProperty(Constants.PLAYER_2_READY, false)) ||
            (!PhotonNetwork.IsMasterClient && GetRoomProperty(Constants.PLAYER_1_READY, false));

        public static void JoinRandomRoom()
        {
            lastTask = "joinRandomRoom";

            switch (PhotonNetwork.NetworkClientState)
            {
                case ClientState.JoinedLobby: break;

                case ClientState.Authenticated:
                    PhotonNetwork.JoinLobby();

                    return;

                default:
                    PhotonNetwork.ConnectUsingSettings();

                    return;
            }

            lastTask = "";
            PhotonNetwork.JoinRandomRoom();
        }
    }
}
