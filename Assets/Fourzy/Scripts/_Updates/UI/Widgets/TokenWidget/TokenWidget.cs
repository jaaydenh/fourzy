//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class TokenWidget : WidgetBase
    {
        public Image tokenImage;
        [HideInInspector]
        public TokensDataHolder.TokenData tokenData;

        public bool IsGrayedOut { get; private set; }

        public WidgetBase SetData(TokensDataHolder.TokenData tokenData)
        {
            Initialize();

            this.tokenData = tokenData;

            button.SetLabel(LocalizationManager.Value(tokenData.name));
            tokenImage.sprite = GameContentManager.Instance.tokensDataHolder.GetTokenSprite(tokenData);

            return this;
        }

        /// <summary>
        /// Invoked from button
        /// </summary>
        public void OnTap()
        {
            bool filterTokens = false;

            switch (GameManager.Instance.buildIntent)
            {
                case BuildIntent.MOBILE_REGULAR:
                    filterTokens = true;

                    break;
            }

            PersistantMenuController.Instance.GetOrAddScreen<TokenPrompt>().Prompt(
                tokenData,
                filterTokens ? PlayerPrefsWrapper.GetUnlockedTokens().Select(_data => _data.tokenType).Contains(tokenData.tokenType) : true,
                false);
        }

        /// <summary>
        /// Either grayed out or normal
        /// </summary>
        public void SetState(bool state)
        {
            IsGrayedOut = !state;

            button.SetState(state);
            button.interactable = true;
        }
    }
}
