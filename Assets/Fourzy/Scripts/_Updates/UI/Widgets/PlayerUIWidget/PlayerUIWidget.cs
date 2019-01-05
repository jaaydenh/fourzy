//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class PlayerUIWidget : WidgetBase
    {
        public TextMeshProUGUI playerName;
        public RectTransform pieceParent;
        public int playerNameMaxSize = 9;
        
        private GamePieceView current;

        public void SetupPlayerName(string name)
        {
            if (name.Length > playerNameMaxSize)
                playerName.text = name.Substring(0, playerNameMaxSize - 1) + "...";
            else
                playerName.text = name;
        }

        public void InitPlayerIcon(GamePieceView gamePiecePrefab)
        {
            current = Instantiate(gamePiecePrefab, pieceParent);
            current.transform.localPosition = Vector3.zero;
            current.transform.localScale = Vector3.one * 94f;

            current.gameObject.SetActive(true);
        }

        public void ShowPlayerTurnAnimation()
        {
            current.ShowTurnAnimation();
        }

        public void StopPlayerTurnAnimation()
        {
            current.StopTurnAnimation();
        }

        public void StartWinJumps()
        {
            current.ShowUIWinAnimation();
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}