//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using TMPro;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class PlayerInfoWidget : WidgetBase
    {
        public TMP_Text playerNameLabel;
        public TMP_Text pieceNameLabel;
        public TMP_Text ratingLabel;
        public Slider starsSlider;
        public GamePieceWidgetSmall gamePieceWidget;

        protected override void Awake()
        {
            base.Awake();

            UserManager.onRatingAquired += OnRatingUpate;
            UserManager.onDisplayNameChanged += OnUpdateUserInfo;
            UserManager.OnUpdateUserGamePieceID += OnUpdateUserGamePieceID;
        }

        protected void OnDestroy()
        {
            UserManager.onRatingAquired -= OnRatingUpate;
            UserManager.onDisplayNameChanged -= OnUpdateUserInfo;
            UserManager.OnUpdateUserGamePieceID -= OnUpdateUserGamePieceID;
        }

        public GamePieceView SetData(string gamePieceID)
        {
            GamePieceData data = GameContentManager.Instance.piecesDataHolder.GetGamePieceData(gamePieceID);
            gamePieceWidget.SetData(data);
            starsSlider.value = data.Champions;

            return gamePieceWidget.gamePiece;
        }

        public override void _Update()
        {
            UserManager user = UserManager.Instance;

            OnUpdateUserInfo();
            OnRatingUpate(0);
            OnUpdateUserGamePieceID(user.gamePieceID);
        }

        private void OnUpdateUserInfo()
        {
            playerNameLabel.text = UserManager.Instance.userName;
        }

        private void OnRatingUpate(int rating)
        {
            if (UserManager.Instance.lastCachedRating == -1)
                ratingLabel.text = $"Rating: ...";
            else
                ratingLabel.text = $"Rating: {UserManager.Instance.lastCachedRating}";
        }

        private void OnUpdateUserGamePieceID(string gamePieceID)
        {
            pieceNameLabel.text = SetData(gamePieceID).pieceData.name;
        }
    }
}
