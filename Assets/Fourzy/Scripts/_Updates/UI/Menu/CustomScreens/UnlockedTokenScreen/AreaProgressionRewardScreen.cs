//@vadym udod

using Fourzy._Updates.Serialized;
using FourzyGameModel.Model;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class AreaProgressionRewardScreen : MenuScreen
    {
        [SerializeField]
        private Image tokenImage;
        [SerializeField]
        private TMP_Text tokenName;

        private Action _onClose;

        public override void OnBack()
        {
            base.OnBack();

            _onClose?.Invoke();
            _onClose = null;

            CloseSelf();
        }

        internal void ShowReward(TokenType token, Action _onClose)
        {
            TokensDataHolder.TokenData _data = GameContentManager.Instance.GetTokenData(token);
            tokenImage.sprite = _data.GetTokenSprite();
            tokenName.text = LocalizationManager.Value(_data.name);

            HeaderScreen.Instance.Close();

            this._onClose = _onClose;

            menuController.OpenScreen(this);
        }

        public void TryButtonPress()
        {

        }
    }
}