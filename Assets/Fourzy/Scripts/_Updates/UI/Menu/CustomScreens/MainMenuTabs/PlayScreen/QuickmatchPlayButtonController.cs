//@vadym udod

using Fourzy._Updates.UI.Helpers;
using UnityEngine;

#if !MOBILE_SKILLZ
using Photon.Pun;
#endif

namespace Fourzy._Updates.UI
{
    public class QuickmatchPlayButtonController : MonoBehaviour
    {
#if !MOBILE_SKILLZ
        private ButtonExtended button;

        private void Awake()
        {
            button = GetComponent<ButtonExtended>();

            FourzyPhotonManager.onJoinedLobby += OnLobbyJoined;
            FourzyPhotonManager.onJoinedRoom += OnRoomJoined;
        }

        private void Start()
        {
            button.SetState(PhotonNetwork.CurrentLobby != null && PhotonNetwork.CurrentRoom == null);
        }

        private void OnDestroy()
        {
            FourzyPhotonManager.onJoinedLobby -= OnLobbyJoined;
            FourzyPhotonManager.onJoinedRoom -= OnRoomJoined;
        }

        private void OnLobbyJoined(string lobby)
        {
            button.SetState(true);
        }

        private void OnRoomJoined(string roomName)
        {
            button.SetState(false);
        }
#endif
    }
}