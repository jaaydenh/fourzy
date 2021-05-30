//@vadym udod

using Fourzy._Updates.ClientModel;
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
        private TokenType tokenReward;

        public override void Open()
        {
            if (!isOpened)
            {
                Close(false);

                CancelRoutine("open");
                BlockInput();
            }
        }

        public override void OnBack()
        {
            base.OnBack();

            _onClose?.Invoke();
            _onClose = null;

            CloseSelf();
        }

        internal void ShowReward(TokenType token, Action _onClose = null)
        {
            tokenReward = token;
            TokensDataHolder.TokenData _data = GameContentManager.Instance.GetTokenData(token);
            tokenImage.sprite = _data.GetTokenSprite();
            tokenName.text = LocalizationManager.Value(_data.name);

            this._onClose = _onClose;

            menuController.OpenScreen(this);
        }

        public void TryButtonPress()
        {
            CloseSelf();

            GameContentManager.Instance.StartTryItBoard(tokenReward);
        }

        public void ActualOpen()
        {
            base.Open();

            HeaderScreen.Instance.Close();
        }
    }
}