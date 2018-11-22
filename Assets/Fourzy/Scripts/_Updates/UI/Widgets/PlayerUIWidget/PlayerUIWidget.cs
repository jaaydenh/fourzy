//@vadym udod

using Fourzy._Updates.UI.Camera3D;
using TMPro;

namespace Fourzy._Updates.UI.Widgets
{
    public class PlayerUIWidget : WidgetBase
    {
        public TextMeshProUGUI playerName;
        public Camera3DItemToImage pieceRenderer;

        private FourzyGamePiece3DCameraItem camera3dItem;

        protected override void Awake()
        {
            base.Awake();

            camera3dItem = pieceRenderer.item as FourzyGamePiece3DCameraItem;
        }

        public void SetupPlayerName(string name)
        {
            playerName.text = name;
        }

        public string GetPlayerName()
        {
            return playerName.text;
        }

        public void InitPlayerIcon(GamePiece gamePiecePrefab)
        {
            camera3dItem.SetGamePiece(gamePiecePrefab);
        }

        public void ShowPlayerTurnAnimation()
        {
            camera3dItem.current.View.ShowTurnAnimation();
        }

        public void StopPlayerTurnAnimation()
        {
            camera3dItem.current.View.StopTurnAnimation();
        }

        public void StartWinJumps()
        {
            camera3dItem.current.View.ShowUIWinAnimation();
        }
    }
}