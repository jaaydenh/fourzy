//@vadym udod

using Fourzy._Updates.UI.Menu.Screens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class GamePieceWidgetMedium : GamePieceWidgetSmall
    {
        public Image borderImage;
        public GameObject infoFrame;
        public GameObject notFound;
        public RectTransform selectedFrame;
        public TMP_Text pieceName;
        public TMP_Text numberOfChampions;
        public Slider starsSlider;
        public Material greyscaleMaterial;

        public override WidgetBase SetData(GamePieceData data)
        {
            base.SetData(data);

            notFound.SetActive(data.State == GamePieceState.NotFound);

            if (data.State == GamePieceState.NotFound)
                gamePiece.SetMaterial(greyscaleMaterial);
            else
                gamePiece.SetMaterial(null);

            switch (data.State)
            {
                case GamePieceState.FoundAndLocked:
                case GamePieceState.NotFound:
                    infoFrame.SetActive(false);
                    layoutElement.preferredHeight = 180;

                    //gamePiece.colorTween.SetColor(Color.grey);
                    gamePiece.Sleep();

                    break;

                case GamePieceState.FoundAndUnlocked:
                    infoFrame.SetActive(true);
                    layoutElement.preferredHeight = 330;
                    
                    //gamePiece.colorTween.SetColor(Color.white);
                    gamePiece.WakeUp();

                    //set data
                    pieceName.text = data.name;
                    borderImage.color = data.borderColor;

                    //stars slider
                    starsSlider.value = data.Champions;

                    break;
            }

            return this;
        }

        public void SetSelectedState(bool state) => selectedFrame.gameObject.SetActive(state);

        public void OnTap() => menuScreen.menuController.GetOrAddScreen<UpgradeGamePiecePromptScreen>().Prompt(data);
    }
}