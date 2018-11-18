//@vadym udod

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Widgets
{
    public class GamePieceWidgetSmall : WidgetBase
    {
        [HideInInspector]
        public GamePieceData data;

        public Image gamePieceIcon;
        public CircleProgress progressBar;
        public TextMeshProUGUI piecesCount; 

        public void SetData(GamePieceData data)
        {
            this.data = data;

            //set icon
            //gamePieceIcon.sprite = data.icon

            progressBar.SetupNewValue(.3f);

            //set pieces count
            piecesCount.text = string.Format("{0}/{1}", data.NumberOfPieces, data.TotalNumberOfPieces);
        }

        //public void 
    }
}