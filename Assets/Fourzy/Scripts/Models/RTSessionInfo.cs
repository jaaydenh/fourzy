using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Api.Messages;

namespace Fourzy {
	public class RTSessionInfo
	{
		private string hostURL;
		public string GetHostURL() { return this.hostURL; }
		private string acccessToken;
		public string GetAccessToken() { return this.acccessToken; }
		private int portID;
		public int GetPortID() { return this.portID; }
		private string matchID;
		public string GetMatchID(){    return this.matchID;    }

		private List<RTPlayer> playerList = new List<RTPlayer> ();
		public List<RTPlayer> GetPlayerList(){
			return playerList;
		}

		/// <summary>
		/// Creates a new RTSession object which is held until a new RT session is created
		/// </summary>
		/// <param name="_message">Message.</param>
		public RTSessionInfo (MatchFoundMessage _message){
			portID = (int)_message.Port;
			hostURL = _message.Host;
			acccessToken = _message.AccessToken;
			matchID = _message.MatchId;
			// we loop through each participant and get their peerId and display name //
			foreach(MatchFoundMessage._Participant p in _message.Participants){
                playerList.Add(new RTPlayer(p.DisplayName, p.Id, (int)p.PeerId));
			}
		}

		public class RTPlayer
		{
			public RTPlayer(string _displayName, string _id, int _peerId){
				this.displayName = _displayName;
				this.id = _id;
				this.peerId = _peerId;
			}

			public string displayName;
			public string id;
			public int peerId;
			public bool isOnline;
		}
	}
}
