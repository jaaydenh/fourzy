//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Tools;
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

        protected RoomInfo data;

        private LobbyPromptScreen _menuScreen;

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

            roomNameLabel.text = data.Name;

            return this;
        }

        public void JoinGame() => _menuScreen.JoinRoom(data.Name);

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _menuScreen = menuScreen as LobbyPromptScreen;
        }
    }
}