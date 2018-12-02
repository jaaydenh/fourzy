//@vadym udod

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class GamePieceWidgetMedium : GamePieceWidgetSmall
    {
        public static Action<string> onSelected;

        public Image borderImage;
        public GameObject infoFrame;
        public GameObject notFound;
        public TMP_Text pieceName;
        public TMP_Text numberOfChampions;
        public Slider starsSlider;

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

            switch (data.state)
            {
                case GamePieceState.FoundAndLocked:
                case GamePieceState.FoundAndUnlocked:
                    pieceName.text = data.Name;
                    numberOfChampions.text = string.Format("{0:000}/{1:000}", data.NumberOfChampions, data.TotalNumberOfChampions);
                    borderImage.color = data.BorderColor;
                    gamePieceIcon.material = null;

                    //stars slider
                    starsSlider.value = data.NumberOfStars;
                    break;

                case GamePieceState.NotFound:
                    infoFrame.SetActive(false);
                    notFound.SetActive(true);
                    layoutElement.preferredHeight = 200f;
                    break;
            }

            //switch toggle
            if (toggle)
                toggle.isOn = data.ID == UserManager.Instance.gamePieceId;
        }

        public void OnToggle()
        {
            if (toggle.isOn)
            {
                if (onSelected != null)
                    onSelected(data.ID.ToString());
            }
        }
    }
}