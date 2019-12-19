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
            MoPubManager.OnRewardedVideoLoadedEvent += OnRewardedVideoLoadedEvent;
            MoPubManager.OnRewardedVideoFailedEvent += OnRewardedVideoFailedEvent;
            MoPubManager.OnRewardedVideoFailedToPlayEvent += OnRewardedVideoFailedToPlayEvent;
            MoPubManager.OnRewardedVideoClosedEvent += OnRewardedVideoClosedEvent;
        }

        protected void OnDisable()
        {
            MoPubManager.OnRewardedVideoLoadedEvent -= OnRewardedVideoLoadedEvent;
            MoPubManager.OnRewardedVideoFailedEvent -= OnRewardedVideoFailedEvent;
            MoPubManager.OnRewardedVideoFailedToPlayEvent -= OnRewardedVideoFailedToPlayEvent;
            MoPubManager.OnRewardedVideoClosedEvent -= OnRewardedVideoClosedEvent;
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
                    titleLabel.text = "Watch ad to receive 1 Hint";

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

        private void OnRewardedVideoLoadedEvent(string adUnitId) { }

        private void OnRewardedVideoFailedEvent(string adUnitId, string error)
        {
            button.SetState(true);
        }

        private void OnRewardedVideoFailedToPlayEvent(string adUnitId, string error)
        {
            button.SetState(true);
        }

        private void OnRewardedVideoClosedEvent(string adUnitId)
        {
            button.SetState(true);

            int reward = 1;

            PersistantOverlayScreen.instance.AnimateReward(true, RewardType.HINTS, reward, Vector2.one * .5f);
            UserManager.Instance.hints += reward;
        }
    }
}