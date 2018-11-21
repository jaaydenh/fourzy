//@vadym udod

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class PlayerUIWidget : WidgetBase
    {
        public TextMeshProUGUI playerName;
        public Transform playerIconParent;

        private GamePiece playerIcon;

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
            if (playerIcon != null)
                Destroy(playerIcon.gameObject);

            playerIcon = Instantiate(gamePiecePrefab);
            playerIcon.gameObject.SetActive(true);
            playerIcon.CachedTransform.parent = playerIconParent;
            playerIcon.CachedTransform.localPosition = new Vector3(0, 0, 10);
            playerIcon.transform.localScale = gamePiecePrefab.transform.localScale;
        }

        public void ShowPlayerTurnAnimation()
        {
            playerIcon.View.ShowTurnAnimation();
        }

        public void StopPlayerTurnAnimation()
        {
            playerIcon.View.StopTurnAnimation();
        }

        public void StartWinJumps()
        {
            playerIcon.View.ShowUIWinAnimation();
        }
    }
}