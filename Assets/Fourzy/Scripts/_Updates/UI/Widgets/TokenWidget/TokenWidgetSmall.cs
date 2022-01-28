//@vadym udod

using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using System.Linq;
using UnityEngine.UI;
using static Fourzy._Updates.Serialized.TokensDataHolder;

namespace Fourzy._Updates.UI.Widgets
{
    public class TokenWidgetSmall : WidgetBase
    {
        public Image tokenIcon;

        private TokenData data;

        public TokenWidgetSmall SetData(TokenData data)
        {
            this.data = data;
            tokenIcon.sprite = GameContentManager.Instance.tokensDataHolder.GetTokenSprite(data);

            return this;
        }

        public void OnClick() => PersistantMenuController.Instance.GetOrAddScreen<TokenPrompt>().Prompt(
            data, 
            PlayerPrefsWrapper.GetUnlockedTokens().Select(_data => _data.tokenType).Contains(data.tokenType), 
            false,
            false);
    }
}