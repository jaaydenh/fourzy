//@vadym udod

using Fourzy._Updates.UI.Helpers;
using Photon.Pun;
using UnityEngine;

namespace Fourzy._Updates.UI
{
    public class QuickmatchPlayButtonController : MonoBehaviour
    {
        private ButtonExtended button;

        private void Awake()
        {
            button = GetComponent<ButtonExtended>();

            FourzyPhotonManager.onJoinedLobby += OnLobbyJoined;
            FourzyPhotonManager.onJoinedRoom += OnRoomJoined;
        }

        private void OnDestroy()
        {
            FourzyPhotonManager.onJoinedLobby -= OnLobbyJoined;
            FourzyPhotonManager.onJoinedRoom -= OnRoomJoined;
        }

        private void OnLobbyJoined(string lobby)
        {
            if (PhotonNetwork.CurrentLobby == null || FourzyPhotonManager.InDefaultLobby)
            {
                button.SetState(true);
            }
        }

        private void OnRoomJoined(string roomName)
        {
            button.SetState(false);
        }
    }
}