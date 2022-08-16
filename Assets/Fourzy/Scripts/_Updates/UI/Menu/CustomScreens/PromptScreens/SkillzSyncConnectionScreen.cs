//@vadym udod

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SkillzSyncConnectionScreen : PromptScreen
    {
        public static bool WaitingForConnection;

        public override void Close(bool animate = true)
        {
            base.Close(animate);

            WaitingForConnection = false;
        }

        private void OnEnable()
        {
            FourzyPhotonManager.onConnectedToMaster += OnConnectedToMaster;
            FourzyPhotonManager.onJoinedLobby += OnJoinedLobby;
            FourzyPhotonManager.onJoinedRoom += OnJoinedRoom;
            FourzyPhotonManager.onConnectionTimeOut += OnConnectionTimeout;
        }

        private void OnDisable()
        {
            FourzyPhotonManager.onConnectedToMaster -= OnConnectedToMaster;
            FourzyPhotonManager.onJoinedLobby -= OnJoinedLobby;
            FourzyPhotonManager.onJoinedRoom -= OnJoinedRoom;
            FourzyPhotonManager.onConnectionTimeOut -= OnConnectionTimeout;
        }

        private void OnConnectedToMaster()
        {
            if (!WaitingForConnection)
            {
                return;
            }

            if (!isOpened)
            {
                menuController.OpenScreen(this);

                promptText.text = "Connected to server";
            }
        }

        private void OnJoinedLobby(string lobbyName)
        {
            if (isOpened)
            {
                promptText.text = "Joined to lobby";
            }
        }

        private void OnJoinedRoom(string roomName)
        {
            if (isOpened)
            {
                CloseSelf();
            }
        }

        private void OnConnectionTimeout()
        {
            if (isOpened)
            {
                CloseSelf();
            }
        }
    }
}
