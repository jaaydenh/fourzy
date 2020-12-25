//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Tools;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class FastPuzzlesLeaderboardPlayerWidget : WidgetBase
    {
        public TMP_Text playerNameLabel;
        public TMP_Text positionLabel;
        public TMP_Text valueLabel;
        public RectTransform iconParent;

        public FastPuzzlesLeaderboardPlayerWidget SetData(PlayerLeaderboardEntry entry)
        {
            playerNameLabel.text = entry.DisplayName;
            positionLabel.text = (entry.Position + 1) + "";
            valueLabel.text = entry.StatValue + "";

            if (entry.PlayFabId == LoginManager.playerMasterAccountID)
            {
                positionLabel.color = Color.green;
                AddGamepieceView(UserManager.Instance.gamePieceID);
            }
            else
                AddGamepieceView(entry.Profile.AvatarUrl);

            return this;
        }

        private GamePieceView AddGamepieceView(string pieceID = "")
        {
            GamePieceView gamePieceView = string.IsNullOrEmpty(pieceID) ?
                Instantiate(
                    GameContentManager.Instance.piecesDataHolder.gamePieces.list.Random().player1Prefab, 
                    iconParent) :
                Instantiate(
                    GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(pieceID).player1Prefab, 
                    iconParent);

            gamePieceView.transform.localPosition = Vector3.zero;
            gamePieceView.StartBlinking();

            return gamePieceView;
        }
    }
}