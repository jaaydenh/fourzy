//@vadym udod

using Fourzy._Updates.Mechanics;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class WatchAdStoreWidget : WidgetBase
    {
        public Image icon;
        public TMP_Text titleLabel;

        private Action onComplete;
        private StorePromptScreen storeScreen;

        protected void OnEnable()
        {
            MoPubBridge.onAdFailed += OnRewardedVideoFailed;
            MoPubBridge.onAdCanceled += OnRewardedVideoFailed;
            MoPubBridge.onAdPlayed += OnRewardedVideoClosedEvent;
        }

        protected void OnDisable()
        {
            MoPubBridge.onAdFailed -= OnRewardedVideoFailed;
            MoPubBridge.onAdCanceled -= OnRewardedVideoFailed;
            MoPubBridge.onAdPlayed -= OnRewardedVideoClosedEvent;
        }

        public override void _Update()
        {
            base._Update();

            if (!storeScreen) return;

            //check reward add availability
            switch (storeScreen.filter)
            {
                case StorePromptScreen.StoreItemType.HINTS:
                    //check availability

                    break;
            }

            //if available
            switch (storeScreen.filter)
            {
                case StorePromptScreen.StoreItemType.HINTS:
                    titleLabel.text = LocalizationManager.Value("watch_ad");

                    break;
            }
        }

        public void OnTap()
        {
            if (!storeScreen) return;

            switch (storeScreen.filter)
            {
                case StorePromptScreen.StoreItemType.HINTS:
                    MoPubBridge.instance.ShowRewardedVideoAd();

                    //disable button
                    button.SetState(false);

                    break;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            storeScreen = GetComponentInParent<StorePromptScreen>();
        }

        private void OnRewardedVideoFailed(string adUnitId) => button.SetState(true);

        private void OnRawardedVideoCalceled(string adUnitId) => button.SetState(true);

        private void OnRewardedVideoClosedEvent(string adUnitId)
        {
            button.SetState(true);

            int reward = 1;

            PersistantOverlayScreen.instance.AnimateReward(true, RewardType.HINTS, reward, Vector2.one * .5f);
            UserManager.Instance.hints += reward;
        }
    }
}