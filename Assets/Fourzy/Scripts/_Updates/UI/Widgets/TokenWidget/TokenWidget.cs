//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class TokenWidget : WidgetBase
    {
        [HideInInspector]
        public TokensDataHolder.TokenData tokenData;

        public Image tokenImage;

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
        public void OnTap() => PersistantMenuController.Instance.GetOrAddScreen<TokenPrompt>().Prompt(tokenData);

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
