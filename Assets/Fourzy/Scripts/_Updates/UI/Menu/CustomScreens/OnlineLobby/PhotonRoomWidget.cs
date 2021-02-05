//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Menu.Screens;
using FourzyGameModel.Model;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class PhotonRoomWidget : WidgetBase
    {
        public TMP_Text roomNameLabel;
        public TMP_Text userRatingLabel;
        public TMP_Text spellsStateLabel;
        public TMP_Text areaNameLabel;
        public RectTransform gamepieceParent;
        public GameObject lockImage;
        public GameObject timerImage;

        protected RoomInfo data;

        private LobbyScreen _menuScreen;
        private InputFieldPrompt passwordScreen;
        private string password = null;
        private Image _image;

        public string roomName => data.Name;

        public PhotonRoomWidget SetData(RoomInfo data)
        {
            this.data = data;

            if (data.CustomProperties.ContainsKey(Constants.REALTIME_ROOM_GAMEPIECE_KEY))
            {
                GamePieceView _gamePiece = Instantiate(
                    GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(data.CustomProperties[Constants.REALTIME_ROOM_GAMEPIECE_KEY].ToString()).player1Prefab, 
                    gamepieceParent);

                _gamePiece.transform.localPosition = Vector3.zero;
                _gamePiece.StartBlinking();
            }

            roomNameLabel.text = data.Name;
            
            //set rating
            bool displayRating = FourzyPhotonManager.GetRoomProperty(
                data.CustomProperties,
                Constants.REALTIME_ROOM_GAMES_TOTAL_KEY,
                0) >= Constants.GAMES_BEFORE_RATING_DISPLAYED;

            if (displayRating)
            {
                int rating = FourzyPhotonManager.GetRoomProperty(
                    data.CustomProperties,
                    Constants.REALTIME_ROOM_RATING_KEY,
                    int.MinValue);

                userRatingLabel.text = rating == int.MinValue ? "-||-" : (rating + "");
            }
            else
            {
                userRatingLabel.text = "Apprentice";
            }
            _image.color = displayRating ? Color.white : new Color(.7f, .7f, .7f, 1f);

            password = FourzyPhotonManager.GetRoomProperty(
                data.CustomProperties,
                Constants.REALTIME_ROOM_PASSWORD,
                "");
            lockImage.SetActive(!string.IsNullOrEmpty(password));

            bool magicState = FourzyPhotonManager.GetRoomProperty(
                data.CustomProperties,
                Constants.REALTIME_ROOM_MAGIC_KEY,
                0) == 2;
            spellsStateLabel.text = magicState ? "Spells\nOn" : "Spells\nOff";

            areaNameLabel.text = GameContentManager.Instance.areasDataHolder[
                (Area)FourzyPhotonManager.GetRoomProperty(
                    data.CustomProperties,
                    Constants.REALTIME_ROOM_AREA,
                    (int)Constants.DEFAULT_AREA)].name;

            bool timer = FourzyPhotonManager.GetRoomProperty(
                data.CustomProperties,
                Constants.REALTIME_ROOM_TIMER_KEY,
                false);

            timerImage.SetActive(timer);

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
            _image = GetComponent<Image>();
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