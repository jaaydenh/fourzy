//@vadym udod

using UnityEngine;
using UnityEngine.UI;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.ClientModel;
using FourzyGameModel.Model;

namespace Fourzy._Updates.UI.Widgets
{
    public class AIPlayerUIWidget : WidgetBase
    {
        public Image bg;
        public RectTransform iconParent;

        public ButtonExtended button { get; private set; }
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

        public void Activate()
        {
            AIPlayersDataHolder.SELECTED = aiPlayerData;

            ClientFourzyGame newGame = new ClientFourzyGame(
            GameContentManager.Instance.currentTheme.themeID,
            UserManager.Instance.meAsPlayer,
            new Player(2, aiPlayerData.name) { HerdId = aiPlayerData.gamePieceID + "", },
            2);

            //ClientFourzyGame newGame = new ClientFourzyGame(
            //        GameContentManager.Instance.currentTheme.themeID,
            //        UserManager.Instance.meAsPlayer, 
            //        new Player(2, aiPlayerData.name) { HerdId = aiPlayerData.gamePieceID + "", },
            //        UserManager.Instance.meAsPlayer.PlayerId);
            newGame._Type = GameType.AI;

            GameManager.Instance.StartGame(newGame);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            button = GetComponent<ButtonExtended>();
        }
    }
}