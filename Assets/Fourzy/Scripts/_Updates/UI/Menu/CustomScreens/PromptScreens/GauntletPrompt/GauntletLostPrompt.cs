//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.UI.Helpers;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GauntletLostPrompt : PromptScreen
    {
        public ButtonExtended watchAdButton;

        private IClientFourzy game;
        private int price;

        protected override void Awake()
        {
            base.Awake();

            MoPubBridge.onAdFailed += OnRewardedVideoFailed;
            MoPubBridge.onAdCanceled += OnRewardedVideoFailed;
            MoPubBridge.onAdPlayed += OnRewardedVideoClosedEvent;
        }

        protected void OnDestroy()
        {
            MoPubBridge.onAdFailed -= OnRewardedVideoFailed;
            MoPubBridge.onAdCanceled -= OnRewardedVideoFailed;
            MoPubBridge.onAdPlayed -= OnRewardedVideoClosedEvent;
        }

        public void _Prompt(IClientFourzy game)
        {
            if (game._Mode != GameMode.GAUNTLET) return;

            this.game = game;

            price = GamePlayManager.Instance.GetGauntletRechargePrice();

            acceptButton.SetState(UserManager.Instance.gems >= price);
            acceptButton.SetLabel(price + "", "price");

            Prompt(
                LocalizationManager.Value("gauntlet_failed"),
                LocalizationManager.Value("gauntlet_failed_message"),
                LocalizationManager.Value("continue_question"),
                LocalizationManager.Value("no"),
                () =>
                {
                    UserManager.Instance.gems -= price;
                    menuController.CloseCurrentScreen();
                    GamePlayManager.Instance.RechargeByGem();
                },
                () => GamePlayManager.Instance.BackButtonOnClick());
        }

        public void WathcAdToContinue()
        {
            MoPubBridge.instance.ShowRewardedVideoAd();

            //disable button
            watchAdButton.SetState(false);
        }

        private void OnRewardedVideoFailed(string adUnitId) => watchAdButton.SetState(true);

        private void OnRawardedVideoCalceled(string adUnitId) => watchAdButton.SetState(true);

        private void OnRewardedVideoClosedEvent(string adUnitId)
        {
            watchAdButton.SetState(true);

            menuController.CloseCurrentScreen();
            GamePlayManager.Instance.RechargeByAd();
        }
    }
}