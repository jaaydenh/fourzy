//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Helpers;
using FourzyGameModel.Model;
using PlayFab.ClientModels;
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
        private TMP_Text rewardTitle;
        [SerializeField]
        private TMP_Text rewardName;
        [SerializeField]
        private ButtonExtended okButton;
        [SerializeField]
        private ParticleSystem fgStarsParticles;

        private CatalogItem reward;
        private GamePieceView gamepiece;

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

            switch (reward.ItemClass)
            {
                case Constants.PLAYFAB_TOKEN_CLASS:
                    AnalyticsManager.Instance.LogEvent(
                        "tryItButtonPress",
                        AnalyticsManager.AnalyticsProvider.ALL,
                        new System.Collections.Generic.KeyValuePair<string, object>("token", reward.ItemId),
                        new System.Collections.Generic.KeyValuePair<string, object>("realtimeGamesCompleted", UserManager.Instance.realtimeGamesComplete),
                        new System.Collections.Generic.KeyValuePair<string, object>("skipped", true));

                    break;
            }

            CloseSelf();
        }

        internal void ShowReward(CatalogItem item)
        {
            reward = item;

            if (gamepiece)
            {
                Destroy(gamepiece.gameObject);
            }

            switch (item.ItemClass)
            {
                case Constants.PLAYFAB_TOKEN_CLASS:
                    TokensDataHolder.TokenData _data = GameContentManager.Instance.GetTokenData((TokenType)Enum.Parse(typeof(TokenType), item.ItemId));
                    tokenImage.color = Color.white;
                    tokenImage.sprite = _data.GetTokenSprite();

                    rewardTitle.text = LocalizationManager.Value("new_token");
                    rewardName.text = LocalizationManager.Value(_data.name);

                    okButton.SetLabel(LocalizationManager.Value("try_it_out"));
                    fgStarsParticles.gameObject.SetActive(true);

                    break;

                case Constants.PLAYFAB_GAMEPIECE_CLASS:
                    tokenImage.color = Color.clear;
                    GamePieceData _pieceData = GameContentManager.Instance.piecesDataHolder.GetGamePieceData(item.ItemId);
                    gamepiece = Instantiate(_pieceData.player1Prefab, tokenImage.transform);
                    gamepiece.transform.localScale = Vector3.one * .5f;
                    gamepiece.StartBlinking();

                    rewardTitle.text = LocalizationManager.Value("new_gamepiece");
                    rewardName.text = _pieceData.name;

                    okButton.SetLabel(LocalizationManager.Value("ok"));
                    fgStarsParticles.gameObject.SetActive(false);

                    break;
            }

            menuController.OpenScreen(this);
        }

        public void TryButtonPress()
        {
            CloseSelf();

            switch (reward.ItemClass)
            {
                case Constants.PLAYFAB_TOKEN_CLASS:
                    AnalyticsManager.Instance.LogEvent(
                        "tryItButtonPress",
                        AnalyticsManager.AnalyticsProvider.ALL,
                        new System.Collections.Generic.KeyValuePair<string, object>("token", reward.ItemId),
                        new System.Collections.Generic.KeyValuePair<string, object>("realtimeGamesCompleted", UserManager.Instance.realtimeGamesComplete),
                        new System.Collections.Generic.KeyValuePair<string, object>("skipped", false));

                    GameContentManager.Instance.StartTryItBoard((TokenType)Enum.Parse(typeof(TokenType), reward.ItemId));

                    break;
            }
        }

        public void ActualOpen()
        {
            base.Open();

            HeaderScreen.Instance.Close();

            switch (reward.ItemClass)
            {
                case Constants.PLAYFAB_TOKEN_CLASS:
                    AnalyticsManager.Instance.LogEvent(
                        "tokenUnlocked",
                        AnalyticsManager.AnalyticsProvider.ALL,
                        new System.Collections.Generic.KeyValuePair<string, object>("token", reward.ItemId),
                        new System.Collections.Generic.KeyValuePair<string, object>("realtimeGamesCompleted", UserManager.Instance.realtimeGamesComplete));

                    break;

                case Constants.PLAYFAB_GAMEPIECE_CLASS:
                    AnalyticsManager.Instance.LogEvent(
                        "fourzyUnlocked",
                        AnalyticsManager.AnalyticsProvider.ALL,
                        new System.Collections.Generic.KeyValuePair<string, object>("piece", reward.ItemId),
                        new System.Collections.Generic.KeyValuePair<string, object>("realtimeGamesCompleted", UserManager.Instance.realtimeGamesComplete));

                    break;
            }
        }
    }
}