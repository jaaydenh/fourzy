﻿//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Serialized;
using FourzyGameModel.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class AIPlayerUIWidget : WidgetBase
    {
        public Image bg;
        public RectTransform iconParent;

        public AIPlayersDataHolder.AIPlayerData aiPlayerData;

        public void SetData(AIPlayersDataHolder.AIPlayerData aiPlayerData)
        {
            this.aiPlayerData = aiPlayerData;

            bg.sprite = aiPlayerData.background;

            GamePieceView icon = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(aiPlayerData.gamePieceID).player1Prefab, iconParent);
            icon.transform.localScale = new Vector3(100f, 100f, 1f);
            icon.StartBlinking();

            button.SetLabel(aiPlayerData.name);
        }

        public void OnTap()
        {
            AIPlayersDataHolder.SELECTED = aiPlayerData;

            GameManager.Instance.StartGame(new ClientFourzyGame(
                GameContentManager.Instance.currentTheme.themeID,
                UserManager.Instance.meAsPlayer,
                new Player(2, aiPlayerData.name, aiPlayerData.profile)
                {
                    HerdId = aiPlayerData.gamePieceID + "",
                    BossType = aiPlayerData.bossProfile,
                },
                1
            )
            { _Type = GameType.AI }, GameTypeLocal.LOCAL_GAME);
        }
    }
}