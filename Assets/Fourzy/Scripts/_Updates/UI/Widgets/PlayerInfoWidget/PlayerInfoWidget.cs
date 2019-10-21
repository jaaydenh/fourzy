//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class PlayerInfoWidget : WidgetBase
    {
        public TMP_Text playerNameLabel;
        public TMP_Text pieceNameLabel;
        public Slider starsSlider;
        public GamePieceWidgetSmall gamePieceWidget;

        protected override void Awake()
        {
            base.Awake();

            UserManager.OnUpdateUserInfo += OnUpdateUserInfo;
            UserManager.OnUpdateUserGamePieceID += OnUpdateUserGamePieceID;
        }

        protected void OnDestroy()
        {
            UserManager.OnUpdateUserInfo -= OnUpdateUserInfo;
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
            OnUpdateUserGamePieceID(user.gamePieceID);
        }

        private void OnUpdateUserInfo()
        {
            playerNameLabel.text = UserManager.Instance.userName;
        }

        private void OnUpdateUserGamePieceID(string gamePieceID)
        {
            pieceNameLabel.text = SetData(gamePieceID).pieceData.name;
        }
    }
}
