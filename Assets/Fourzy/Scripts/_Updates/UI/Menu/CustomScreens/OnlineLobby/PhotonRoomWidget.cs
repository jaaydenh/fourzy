//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Menu.Screens;
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
        private InputFieldPrompt passwordScreen;
        private string password = null;

        public string roomName => data.Name;

        public PhotonRoomWidget SetData(RoomInfo data)
        {
            this.data = data;

            if (data.CustomProperties.ContainsKey(Constants.REALTIME_GAMEPIECE_KEY))
            {
                GamePieceView _gamePiece = Instantiate(
                    GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(data.CustomProperties[Constants.REALTIME_GAMEPIECE_KEY].ToString()).player1Prefab, 
                    gamepieceParent);

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

        public void _Destroy()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Invoked via button
        /// </summary>
        public void JoinGame()
        {
            if (string.IsNullOrEmpty(password))
            {
                _menuScreen.JoinRoom(data.Name);
            }
            else
            {
                passwordScreen = menuScreen.menuController
                    .GetOrAddScreen<InputFieldPrompt>()
                    ._Prompt(
                        CheckPassword, 
                        LocalizationManager.Value("enter_room_code"), 
                        "",
                        LocalizationManager.Value("join"),
                        LocalizationManager.Value("close"));

                passwordScreen.CloseOnDecline();
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
            {
                if (passwordScreen.isOpened)
                {
                    passwordScreen.CloseSelf();
                }

                _menuScreen.JoinRoom(data.Name);
            }
            else
            {
                passwordScreen
                    .SetText("Wrong password", 1f)
                    .SetTextColor(Color.red, 1f);
            }
        }
    }
}