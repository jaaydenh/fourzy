//@vadym udod

using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class PlayerUIWidget : WidgetBase
    {
        public TextMeshProUGUI playerName;
        public RectTransform pieceParent;
        
        private GamePiece current;

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
            current = Instantiate(gamePiecePrefab, pieceParent);
            current.transform.localPosition = Vector3.zero;
            current.transform.localScale = Vector3.one * 94f;

            current.gameObject.SetActive(true);
        }

        public void ShowPlayerTurnAnimation()
        {
            current.gamePieceView.ShowTurnAnimation();
        }

        public void StopPlayerTurnAnimation()
        {
            current.gamePieceView.StopTurnAnimation();
        }

        public void StartWinJumps()
        {
            current.gamePieceView.ShowUIWinAnimation();
        }
    }
}