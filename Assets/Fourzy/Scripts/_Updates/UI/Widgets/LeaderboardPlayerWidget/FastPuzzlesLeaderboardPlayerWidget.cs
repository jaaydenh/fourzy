﻿//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
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

        private GamePieceView currentGamepiece;
        private PlayerLeaderboardEntry data;

        public FastPuzzlesLeaderboardPlayerWidget SetData(PlayerLeaderboardEntry entry)
        {
            data = entry;

            playerNameLabel.text = entry.DisplayName;
            positionLabel.text = (entry.Position + 1) + "";
            valueLabel.text = entry.StatValue + "";

            if (entry.PlayFabId == LoginManager.playfabId)
            {
                positionLabel.color = Color.green;
                AddGamepieceView(UserManager.Instance.gamePieceID);
            }
            else
            {
                positionLabel.color = Color.white;
                AddGamepieceView(entry.Profile.AvatarUrl);
            }

            return this;
        }

        public float GetHeight() => rectTransform ? rectTransform.rect.height : 0f;

        private GamePieceView AddGamepieceView(string pieceID = "")
        {
            if (currentGamepiece)
                Destroy(currentGamepiece.gameObject);

            pieceID = string.IsNullOrEmpty(pieceID) ? Constants.DEFAULT_GAME_PIECE : pieceID;
            currentGamepiece = Instantiate(
                    GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(pieceID).player1Prefab,
                    iconParent);

            currentGamepiece.transform.localPosition = Vector3.zero;
            currentGamepiece.StartBlinking();

            return currentGamepiece;
        }
    }
}