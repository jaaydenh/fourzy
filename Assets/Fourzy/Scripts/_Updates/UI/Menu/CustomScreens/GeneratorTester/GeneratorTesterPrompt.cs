//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GeneratorTesterPrompt : MenuScreen
    {
        public ScrollRect areasContainer;
        public PracticeScreenAreaSelectWidget areaWidgetPrefab;
        public GeneratorTesterTokenView generatorTesterTokenView;
        public RectTransform tokensParent;
        public Slider low;
        public Slider high;
        public ButtonExtended toggleMagicButton;
        public ButtonExtended toggleRandomElementsButton;
        public ButtonExtended toggleDynamicElementsButton;

        public PracticeScreenAreaSelectWidget currentAreaWidget { get; private set; }
        public bool AllowMagic { get; private set; } = true;
        public bool AllowRandomElements { get; private set; } = true;
        public bool AllowDynamicElements { get; private set; } = true;

        public void OnLowValueChange(float value)
        {
            if (value > high.value)
            {
                high.value = value + 1;
            }
        }

        public void OnHighValueChange(float value)
        {
            if (value < low.value)
            {
                low.value = value - 1;
            }
        }
        
        public void ToggleMagic()
        {
            AllowMagic = !AllowMagic;

            toggleMagicButton.GetBadge().badge.SetState(AllowMagic);
        }

        public void ToggleRandomElements()
        {
            AllowRandomElements = !AllowRandomElements;

            toggleRandomElementsButton.GetBadge().badge.SetState(AllowRandomElements);
        }

        public void ToggleDynamicElements()
        {
            AllowDynamicElements = !AllowDynamicElements;

            toggleDynamicElementsButton.GetBadge().badge.SetState(AllowDynamicElements);
        }

        public void GenerateAndTry()
        {

        }

        protected void OnAreaWidgetTap(PracticeScreenAreaSelectWidget widget)
        {
            if (currentAreaWidget)
            {
                currentAreaWidget.Deselect();

                if (currentAreaWidget == widget)
                {
                    currentAreaWidget = null;
                }
                else
                {
                    currentAreaWidget = widget;
                }
            }
            else
            {
                currentAreaWidget = widget;
            }

            low.transform.parent.gameObject.SetActive(currentAreaWidget == null);
            high.transform.parent.gameObject.SetActive(currentAreaWidget == null);

            if (currentAreaWidget)
            {
                currentAreaWidget.Select();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            //areas
            bool first = true;
            foreach (AreasDataHolder.GameArea areaData in GameContentManager.Instance.enabledAreas)
            {
                if (first)
                {
                    first = false;
                    OnAreaWidgetTap(AddAreaWidget(areaData.areaID));
                }
                else
                {
                    AddAreaWidget(areaData.areaID);
                }
            }

            //tokens
            foreach (TokensDataHolder.TokenData data in GameContentManager.Instance.enabledTokens)
            {
                Instantiate(generatorTesterTokenView, tokensParent)
                    .SetData(data.tokenType);
            }
        }

        protected PracticeScreenAreaSelectWidget AddAreaWidget(Area area)
        {
            PracticeScreenAreaSelectWidget instance = Instantiate(areaWidgetPrefab, areasContainer.content)
                .SetData(area);
            instance.button.onTap += data => OnAreaWidgetTap(instance);

            return instance;
        }
    }
}