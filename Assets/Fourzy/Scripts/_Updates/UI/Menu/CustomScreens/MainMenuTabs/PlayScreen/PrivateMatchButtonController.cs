//@vadym udod

using Fourzy._Updates.UI.Helpers;
using UnityEngine;

namespace Fourzy._Updates.UI
{
    public class PrivateMatchButtonController : MonoBehaviour
    {
        private ButtonExtended button;

        private void Awake()
        {
            button = GetComponent<ButtonExtended>();

            FourzyPhotonManager.onJoinedLobby += OnLobbyJoined;
            FourzyPhotonManager.onJoinedRoom += OnRoomJoined;
        }

        private void Start()
        {
            if (!FourzyPhotonManager.InDefaultLobby)
            {
                button.SetState(false);
            }
        }

        private void OnDestroy()
        {
            FourzyPhotonManager.onJoinedLobby -= OnLobbyJoined;
            FourzyPhotonManager.onJoinedRoom -= OnRoomJoined;
        }

        private void OnLobbyJoined(string lobby)
        {
            if (FourzyPhotonManager.InDefaultLobby)
            {
                button.SetState(true);
            }
            else
            {
                button.SetState(false);
            }
        }

        private void OnRoomJoined(string roomName)
        {
            button.SetState(false);
        }
    }
}
