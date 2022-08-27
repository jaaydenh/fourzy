//@vadym udod

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class GamePieceWidgetMedium : GamePieceWidgetSmall
    {
        [SerializeField]
        private Image borderImage;
        [SerializeField]
        private GameObject infoFrame;
        [SerializeField]
        private TMP_Text pieceName;
        [SerializeField]
        private TMP_Text numberOfChampions;
        [SerializeField]
        private Slider starsSlider;

        public override WidgetBase SetData(GamePieceData data)
        {
            base.SetData(data);

            SetAsHidden(data.State == GamePieceState.NotFound);

            switch (data.State)
            {
                case GamePieceState.FoundAndLocked:
                case GamePieceState.NotFound:
                    infoFrame.SetActive(false);
                    layoutElement.preferredHeight = 180;

                    //gamePiece.colorTween.SetColor(Color.grey);
                    GamePiece.Sleep();

                    break;

                case GamePieceState.FoundAndUnlocked:
                    infoFrame.SetActive(true);
                    layoutElement.preferredHeight = 330;
                    
                    //gamePiece.colorTween.SetColor(Color.white);
                    GamePiece.WakeUp();

                    //set data
                    pieceName.text = data.name;
                    borderImage.color = data.borderColor;

                    //stars slider
                    starsSlider.value = 0f;

                    break;
            }

            return this;
        }
    }
}