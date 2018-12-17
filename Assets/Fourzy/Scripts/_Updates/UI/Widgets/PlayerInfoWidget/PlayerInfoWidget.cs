//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class PlayerInfoWidget : WidgetBase
    {
        public TMP_Text playerNameLabel;
        public TMP_Text pieceNameLabel;
        public Badge ratingLabel;
        public Slider starsSlider;
        public Image playerPieceIcon;
        public CircleProgressUI circleProgressUI;

        protected override void Awake()
        {
            base.Awake();

            UserManager.OnUpdateUserInfo += UserManager_OnUpdateUserInfo;
            UserManager.OnUpdateUserGamePieceID += UserManager_OnUpdateUserGamePieceID;
        }

        protected void OnDestroy()
        {
            UserManager.OnUpdateUserInfo -= UserManager_OnUpdateUserInfo;
            UserManager.OnUpdateUserGamePieceID -= UserManager_OnUpdateUserGamePieceID;
        }

        public void UpdateGamePieceIcon(int gamePieceID)
        {
            playerPieceIcon.enabled = false;
            foreach (Transform t in playerPieceIcon.transform)
                Destroy(t.gameObject);
            
            GamePiece pieceIcon = Instantiate(GameContentManager.Instance.GetGamePiecePrefab(gamePieceID), playerPieceIcon.transform);
            pieceIcon.transform.localPosition = Vector3.zero;
            pieceIcon.transform.localScale = Vector3.one * 100f;
            pieceIcon.gameObject.SetLayerRecursively(playerPieceIcon.gameObject.layer);
            pieceIcon.gamePieceView.StartBlinking();
        }

        public void UpdateWidget()
        {
            UserManager user = UserManager.Instance;

            UserManager_OnUpdateUserInfo();
            UserManager_OnUpdateUserGamePieceID(user.gamePieceId);
        }

        private void UserManager_OnUpdateUserInfo()
        {
            UserManager user = UserManager.Instance;

            ratingLabel.SetValue(user.ratingElo.ToString());

            if (!string.IsNullOrEmpty(user.userName))
                playerNameLabel.text = user.userName;
        }

        private void UserManager_OnUpdateUserGamePieceID(int gamePieceID)
        {
            UpdateGamePieceIcon(gamePieceID);
            pieceNameLabel.text = GameContentManager.Instance.GetGamePieceName(gamePieceID);
        }
    }
}
