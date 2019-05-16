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
        public TMP_Text pieceName;
        public TMP_Text numberOfChampions;
        public Slider starsSlider;
        public Material greyscaleMaterial;

        public Toggle toggle { get; private set; }
        public ToggleGroup toggleGroup { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            toggle = GetComponent<Toggle>();
            toggleGroup = GetComponentInParent<ToggleGroup>();
            
            if (toggleGroup && toggle)
                toggle.group = toggleGroup;
        }

        public override void SetData(GamePieceData data)
        {
            base.SetData(data);

            switch (data.State)
            {
                case GamePieceState.FoundAndLocked:
                case GamePieceState.NotFound:
                    infoFrame.SetActive(false);
                    notFound.SetActive(true);
                    layoutElement.preferredHeight = 180;

                    gamePiece.SetMaterial(greyscaleMaterial);
                    gamePiece.colorTween.SetColor(Color.grey);
                    gamePiece.Sleep();
                    break;

                case GamePieceState.FoundAndUnlocked:
                    infoFrame.SetActive(true);
                    notFound.SetActive(false);
                    layoutElement.preferredHeight = 330;
                    
                    gamePiece.SetMaterial(null);
                    gamePiece.colorTween.SetColor(Color.white);
                    gamePiece.WakeUp();

                    //set data
                    pieceName.text = data.name;
                    borderImage.color = data.borderColor;

                    //stars slider
                    starsSlider.value = data.champions;
                    break;
            }

            //switch toggle
            if (toggle)
                toggle.isOn = data.ID == UserManager.Instance.gamePieceID;
        }

        public void OnToggle()
        {
            if (!menuScreen.isCurrent)
                return;

            UserManager.Instance.UpdateSelectedGamePiece(data.ID);

            //open upgrade prompt
            menuScreen.menuController.GetScreen<UpgradeGamePiecePromptScreen>().Prompt(data);
        }
    }
}