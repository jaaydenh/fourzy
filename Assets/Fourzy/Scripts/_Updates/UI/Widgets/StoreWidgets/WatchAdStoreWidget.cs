﻿//@vadym udod

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
        private ButtonExtended button;

        protected void OnEnable()
        {
            MoPubManager.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
            MoPubManager.OnInterstitialFailedEvent += OnInterstitialFailedEvent;
        }

        protected void OnDisable()
        {
            MoPubManager.OnInterstitialLoadedEvent -= OnInterstitialLoadedEvent;
            MoPubManager.OnInterstitialFailedEvent -= OnInterstitialFailedEvent;
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
                    MoPubBridge.instance.RequestRewardedVideo();

                    //disable button
                    button.SetState(false);

                    break;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            storeScreen = GetComponentInParent<StorePromptScreen>();
            button = GetComponent<ButtonExtended>();
        }
        private void OnInterstitialLoadedEvent(string adUnitId)
        {
            button.SetActive(true);
        }

        private void OnInterstitialFailedEvent(string adUnitId, string error)
        {
            button.SetActive(true);
        }
    }
}