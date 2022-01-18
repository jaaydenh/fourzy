//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using TMPro;
using UnityEngine;

#if !MOBILE_SKILLZ
using PlayFab.ClientModels;
#endif

namespace Fourzy._Updates.UI.Widgets
{
    public class FastPuzzlesLeaderboardPlayerWidget : WidgetBase
    {
        public TMP_Text playerNameLabel;
        public TMP_Text positionLabel;
        public TMP_Text valueLabel;
        public RectTransform iconParent;

        private GamePieceView currentGamepiece;
#if !MOBILE_SKILLZ
        private PlayerLeaderboardEntry data;
#endif

#if !MOBILE_SKILLZ
        public FastPuzzlesLeaderboardPlayerWidget SetData(PlayerLeaderboardEntry entry)
        {
            data = entry;

            playerNameLabel.text = entry.DisplayName;
            positionLabel.text = (entry.Position + 1) + "";
            valueLabel.text = entry.StatValue + "";

            if (entry.PlayFabId == LoginManager.playfabId)
            {
                positionLabel.color = Color.green;
                AddGamepieceView(UserManager.Instance.gamePieceId);
            }
            else
            {
                positionLabel.color = Color.white;
                AddGamepieceView(entry.Profile.AvatarUrl);
            }

            return this;
        }
#endif

        public float GetHeight() => rectTransform ? rectTransform.rect.height : 0f;

        private GamePieceView AddGamepieceView(string pieceID = "")
        {
            if (currentGamepiece)
                Destroy(currentGamepiece.gameObject);

            pieceID = string.IsNullOrEmpty(pieceID) ? Constants.DEFAULT_GAME_PIECE : pieceID;
            currentGamepiece = Instantiate(
                    GameContentManager.Instance.piecesDataHolder.GetGamePieceData(pieceID).player1Prefab,
                    iconParent);

            currentGamepiece.transform.localPosition = Vector3.zero;
            currentGamepiece.StartBlinking();

            return currentGamepiece;
        }
    }
}