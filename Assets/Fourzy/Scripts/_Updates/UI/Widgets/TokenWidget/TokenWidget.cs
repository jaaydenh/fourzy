//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Helpers;
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

        public ButtonExtended button { get; private set; }
        public MenuScreen menuScree { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            button = GetComponent<ButtonExtended>();
            menuScree = GetComponentInParent<MenuScreen>();
        }

        public void SetData(TokensDataHolder.TokenData tokenData)
        {
            this.tokenData = tokenData;

            button.SetLabel(tokenData.name);
            tokenImage.sprite = GameContentManager.Instance.tokensDataHolder.GetTokenSprite(tokenData);

            tileBGImage.enabled = tokenData.showBackgroundTile;
        }

        public void OpenTokenPrompt()
        {
            menuScree.menuController.GetScreen<TokenPrompt>().Prompt(tokenData);
        }
    }
}
