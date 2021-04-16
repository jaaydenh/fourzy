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
        public Image tileBGImage;

        public WidgetBase SetData(TokensDataHolder.TokenData tokenData)
        {
            this.tokenData = tokenData;

            button.SetLabel(LocalizationManager.Value(tokenData.name));
            tokenImage.sprite = GameContentManager.Instance.tokensDataHolder.GetTokenSprite(tokenData);

            tileBGImage.enabled = tokenData.showBackground;
            tileBGImage.color = tokenData.backgroundTileColor;

            return this;
        }

        public void OnTap() => PersistantMenuController.Instance.GetOrAddScreen<TokenPrompt>().Prompt(tokenData);
    }
}
