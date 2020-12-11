//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Menu.Screens;
using Fourzy._Updates.UI.Toasts;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class PhotonRoomWidget : WidgetBase
    {
        public TMP_Text roomNameLabel;
        public RectTransform gamepieceParent;
        public GameObject lockImage;

        protected RoomInfo data;

        private LobbyScreen _menuScreen;
        private string password = null;

        public string roomName => data.Name;

        public PhotonRoomWidget SetData(RoomInfo data)
        {
            this.data = data;

            if (data.CustomProperties.ContainsKey(Constants.REALTIME_GAMEPIECE_KEY))
            {
                GamePieceView _gamePiece = Instantiate(GameContentManager.Instance.piecesDataHolder
                    .GetGamePiecePrefabData(data.CustomProperties[Constants.REALTIME_GAMEPIECE_KEY].ToString()).player1Prefab, gamepieceParent);

                _gamePiece.transform.localPosition = Vector3.zero;
                _gamePiece.StartBlinking();
            }

            password = FourzyPhotonManager.GetRoomProperty(
                data.CustomProperties, 
                Constants.REALTIME_ROOM_PASSWORD, 
                "");
            lockImage.SetActive(!string.IsNullOrEmpty(password));

            roomNameLabel.text = data.Name;

            return this;
        }

        public void JoinGame()
        {
            if (string.IsNullOrEmpty(password))
                _menuScreen.JoinRoom(data.Name);
            else
            {
                InputFieldPrompt screen = menuScreen.menuController.GetOrAddScreen<InputFieldPrompt>();
                screen._Prompt(CheckPassword, "Enter Password", "Password:", "Join", "Close");
                screen.CloseOnAccept().CloseOnDecline();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _menuScreen = menuScreen as LobbyScreen;
        }

        private void CheckPassword(string value)
        {
            if (value.Equals(password))
                _menuScreen.JoinRoom(data.Name);
            else
                GamesToastsController.ShowTopToast("Wrong password");
        }
    }
}