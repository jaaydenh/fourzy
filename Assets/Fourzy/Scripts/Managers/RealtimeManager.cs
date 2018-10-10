using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Api.Messages;
using GameSparks.Core;
using GameSparks.Api.Responses;
using GameSparks.RT;
using System;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class RealtimeManager : UnitySingleton<RealtimeManager> {

        public delegate void RealtimeReady(int firstPlayerPeerId, int tokenBoardIndex);
        public static event RealtimeReady OnRealtimeReady;

        public static Action OnRealtimeMatchNotFound;
        public static Action<long> OnReceiveTimeStamp;
        public static Action<Move> OnReceiveMove;

        private int currentPlayerPeerId;
        public int timeDelta;
        private int latency, roundTrip;
        private GameSparksRTUnity gameSparksRTUnity;

        public GameSparksRTUnity GetRTSession(){
            return gameSparksRTUnity;
        }

        private RTSessionInfo rtSessionInfo;

        public RTSessionInfo GetSessionInfo(){
            return rtSessionInfo;
        }

        void Start () {
            MatchNotFoundMessage.Listener += OnMatchNotFound;
            MatchFoundMessage.Listener += OnMatchFound;
        }

        void OnMatchNotFound (MatchNotFoundMessage _message) {
            GameManager.Instance.ShowInfoBanner("No Match Found");
            OnRealtimeMatchNotFound();
        }

        void OnMatchFound (MatchFoundMessage _message) {
            GameManager.Instance.ShowInfoBanner("Match Found");
            rtSessionInfo = new RTSessionInfo(_message);
            StartNewRTSession(_message);
        }

        /// <summary>
        /// This will request a match between as many players you have set in the match.
        /// When the max number of players is found each player will receive the MatchFound message
        /// </summary>
        public void FindPlayers() {
            Debug.Log ("GSM| Attempting Matchmaking...");
            new GameSparks.Api.Requests.MatchmakingRequest ()
                .SetMatchShortCode ("matchRanked") // set the shortCode to be the same as the one we created in the first tutorial
                .SetSkill (0) // in this case we assume all players have skill level zero and we want anyone to be able to join so the skill level for the request is set to zero
                .Send ((response) => {
                    if(response.HasErrors){ // check for errors
                        Debug.LogError("GSM| MatchMaking Error \n" + response.Errors.JSON);
                    }
                });
        }

        public void StartNewRTSession(MatchFoundMessage message){
            Debug.Log ("GSM| Creating New RT Session Instance...");

            gameSparksRTUnity = this.gameObject.AddComponent<GameSparksRTUnity>(); // Adds the RT script to the game

            gameSparksRTUnity.Configure(message, 
                (peerId) =>  {    OnPlayerConnectedToGame(peerId);  },
                (peerId) => {    OnPlayerDisconnected(peerId);    },
                (ready) => {    OnRTReady(ready);    },
                (packet) => {    OnPacketReceived(packet);    });
            gameSparksRTUnity.Connect(); // when the config is set, connect the game
        }

        private void OnPlayerConnectedToGame(int _peerId){
            Debug.Log ("GSM| Player Connected, " + _peerId);
            currentPlayerPeerId = _peerId;
        }

        private void OnPlayerDisconnected(int _peerId){
            Debug.Log ("GSM| Player Disconnected, " + _peerId);
        }

        private void OnRTReady(bool _isReady){
            if (_isReady) {
                Debug.Log ("GSM| RT Session Connected...");
            }
        }

        private void OnPacketReceived(RTPacket _packet){

            switch (_packet.OpCode) 
            {
                // op-code 1 refers to any chat-messages being received by a player //
                // from here, we'll send them to the chat-manager //
                case 1:
                    // if (chatManager == null) { // if the chat manager is not yet setup, then assign the reference in the scene
                    //     chatManager = GameObject.Find ("Chat Manager").GetComponent<ChatManager> ();
                    // }
                    // chatManager.OnMessageReceived (_packet); // send the whole packet to the chat-manager
                    OnMessageReceived(_packet); // send the whole packet to the chat-manager
                    break;
                case 2:
                    ProcessOpponentMove(_packet);
                    break;
                case 100:
                    Debug.Log("All players joined realtime session");
                    int firstPlayerPeerId = _packet.Data.GetInt(1).Value;
                    int tokenBoardIndex = _packet.Data.GetInt(2).Value;
                    OnRealtimeReady(firstPlayerPeerId, tokenBoardIndex);
                    break;
                case 101:
                    CalculateTimeDelta(_packet);
                    break;
                case 102:
                    if (OnReceiveTimeStamp != null)
                    {
                        OnReceiveTimeStamp(_packet.Data.GetLong(1).Value);
                    }
                    break;
            }
        }

        public void SendRealTimeMove(Move move) {
            using (RTData data = RTData.Get ()) {  // we put a using statement here so that we can dispose of the RTData objects once the packet is sent
                data.SetInt(1, Utility.GetMoveLocation(move));
                data.SetInt(2, move.direction.GetHashCode());
                data.SetInt(3, move.player.GetHashCode());
                GetRTSession().SendData(2, GameSparks.RT.GameSparksRT.DeliveryIntent.RELIABLE, data);
            }
        }

        public void ProcessOpponentMove(RTPacket _packet) {
            
            int location = _packet.Data.GetInt(1).Value;
            Direction direction = (Direction)_packet.Data.GetInt(2).Value;
            PlayerEnum player = (PlayerEnum)_packet.Data.GetInt(3).Value;
            Move move = new Move(location, direction, player);
            if (OnReceiveMove != null)
            {
                OnReceiveMove(move);
            }
        }

        public void SendTimeStamp() {
            // send a packet with our current time first //
            using (RTData data = RTData.Get ()) {
                data.SetLong (1, (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds); // get the current time as unix timestamp
                GetRTSession ().SendData (101, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE, data, new int[]{ 0 }); // send to peerId -> 0, which is the server
            }
        }

        /// <summary>
        /// Calculates the time-difference between the client and server
        /// </summary>
        public void CalculateTimeDelta(RTPacket _packet){
            // calculate the time taken from the packet to be sent from the client and then for the server to return it //
            roundTrip = (int)((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds - _packet.Data.GetLong (1).Value);
            latency = roundTrip / 2; // the latency is half the round-trip time
            // calculate the server-delta from the server time minus the current time
            int serverDelta = (int)(_packet.Data.GetLong (2).Value - (long)(DateTime.UtcNow - new DateTime (1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
            timeDelta = serverDelta + latency; // the time-delta is the server-delta plus the latency
        }

        private void SendChatMessage(string message) {
            if (message != string.Empty) { // first check to see if there is any message to send
                // for all RT-data we are sending, we use an instance of the RTData object //
                // this is a disposable object, so we wrap it in this using statement to make sure it is returned to the pool //
                using (RTData data = RTData.Get ()) {
                    data.SetString(1, message); // we add the message data to the RTPacket at key '1', so we know how to key it when the packet is receieved
                    data.SetString(2, DateTime.Now.ToString()); // we are also going to send the time at which the user sent this message

                    Debug.Log("Sending Message to All Players... \n" + message);
                    GetRTSession().SendData(1, GameSparks.RT.GameSparksRT.DeliveryIntent.RELIABLE, data);
                }
            } else {
                Debug.Log ("No Chat Message To Send...");
            }
        }

        /// <summary>
        /// This is called from the GameSparksManager class.
        /// It send any packets with op-code '1' to the chat manager to the chat manager can parse the nessesary details out for display in the chat log window
        /// </summary>
        /// <param name="_data">Data.</param>
        public void OnMessageReceived(RTPacket _packet){
            Debug.Log ("Message Received...\n"+_packet.Data.GetString(1)); // the RTData we sent the message with used an index '1' so we can parse the data back using the same index
            // we need the display name of the sender. We get this by using the packet sender id and comparing that to the peerId of the player //
            foreach (RTSessionInfo.RTPlayer player in GetSessionInfo().GetPlayerList()) {
                if (player.peerId == _packet.Sender) {
                    // we want to get the message and time and print those to the local users chat-log //
                     GamePlayManager.Instance.alertUI.Open(player.displayName + ": " + _packet.Data.GetString (1) + "(" +_packet.Data.GetString (2)+ ")");
                    // UpdateChatLog (player.displayName, _packet.Data.GetString (1), _packet.Data.GetString (2));
                }
            }
        }
    }
}
