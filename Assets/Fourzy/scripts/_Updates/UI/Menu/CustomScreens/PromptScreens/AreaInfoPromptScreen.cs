//@vadym udod

using Fourzy._Updates.UI.Helpers;
using FourzyGameModel.Model;
using UnityEngine;
using Fourzy._Updates.Serialized;
using System.Collections.Generic;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.UI.Widgets;
using Fourzy._Updates.UI.Toasts;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class AreaInfoPromptScreen : PromptScreen
    {
        public Image icon;
        public GameObject gamePiecesRoot;
        public GameObject gamePieceHolder;
        public RectTransform gamePiecesParent;
        public GameObject tokensRoot;
        public RectTransform tokensParent;
        public RectTransform buttonsParent;

        public ButtonExtended selectButton;
        public ButtonExtended buyButton;

        private List<GameObject> gamePieces = new List<GameObject>();
        private List<GameObject> tokens = new List<GameObject>();
        private List<ButtonExtended> buyButtons = new List<ButtonExtended>();

        private ThemesDataHolder.GameTheme currentOpenedTheme;

        public void Prompt(Area theme)
        {
            ThemesDataHolder.GameTheme _themeData = GameContentManager.Instance.themesDataHolder.GetTheme(theme);

            if (theme == Area.NONE || _themeData == null) return;
            
            Prompt(_themeData.name, _themeData.description, null);

            //get buy buttons
            if (!PlayerPrefsWrapper.GetThemeUnlocked((int)theme))
            {
                foreach (ButtonExtended button in buyButtons) Destroy(button.gameObject);
                buyButtons.Clear();

                foreach (ThemesDataHolder.AreaUnlockRequirement unlockRequirement in _themeData.unclockRequirements.list)
                {
                    ButtonExtended buttonInstance = Instantiate(buyButton, buttonsParent);
                    buttonInstance.GetBadge(unlockRequirement.type.ToString()).badge.SetValue(unlockRequirement.quantity);
                    buttonInstance.events.AddListener(() => Purchase(unlockRequirement.type));

                    buyButtons.Add(buttonInstance);
                }
            }

            SetData(_themeData);
        }

        public void SetData(ThemesDataHolder.GameTheme themeData)
        {
            if (themeData.themeID == Area.NONE) return;

            //refresh buttons
            if (PlayerPrefsWrapper.GetThemeUnlocked((int)themeData.themeID) || themeData.unclockRequirements.list.Count == 0)
            {
                buyButtons.ForEach(button => button.SetActive(false));

                selectButton.SetActive(true);
            }
            else
            {
                buyButtons.ForEach(button => button.SetActive(true));

                selectButton.SetActive(false);
            }

            if (themeData == currentOpenedTheme) return;

            icon.sprite = themeData.preview;

            foreach (GameObject gamePiece in gamePieces) Destroy(gamePiece);
            gamePieces.Clear();

            gamePiecesRoot.SetActive(themeData.gamepieces.list.Count > 0);
            foreach (ThemesDataHolder.GamePieceView_ThemesDataHolder gamePiecePrefab in themeData.gamepieces.list)
            {
                GameObject gamePieceHolderInstance = Instantiate(gamePieceHolder, gamePiecesParent);
                gamePieceHolderInstance.SetActive(true);

                GamePieceView gamePieceInstance = Instantiate(gamePiecePrefab.prefab, gamePieceHolderInstance.transform);
                gamePieceInstance.transform.localScale = Vector3.one * 70f;
                gamePieceInstance.StartBlinking();

                gamePieces.Add(gamePieceHolderInstance);
            }

            foreach (GameObject tokenView in tokens) Destroy(tokenView);
            tokens.Clear();

            tokensRoot.SetActive(themeData.tokens.list.Count > 0);

            foreach (TokenType tokenType in themeData.tokens.list)
            {
                GameObject tokenHolderInstance = Instantiate(gamePieceHolder, tokensParent);
                tokenHolderInstance.SetActive(true);

                TokensDataHolder.TokenData tokenData = GameContentManager.Instance.GetTokenData(tokenType);

                Image bg = tokenHolderInstance.GetComponentInChildren<Image>(true);
                bg.gameObject.SetActive(tokenData.showBackgroundTile);
                bg.color = tokenData.backgroundTileColor;

                TokenView tokenInstance = Instantiate(GameContentManager.Instance.GetTokenPrefab(tokenType, themeData.themeID), tokenHolderInstance.transform);
                tokenInstance.transform.localScale = Vector3.one * (tokenData.showBackgroundTile ? 55f : 70f);

                tokens.Add(tokenHolderInstance);
            }

            currentOpenedTheme = themeData;
        }

        public override void Accept()
        {
            base.Accept();

            GameContentManager.Instance.currentTheme = currentOpenedTheme;
            menuController.CloseCurrentScreen(true);
        }

        public void Purchase(CurrencyType currency)
        {
            ThemesDataHolder.AreaUnlockRequirement unlockRequirement = currentOpenedTheme.unclockRequirements.GetRequirement(currency);

            //check currency
            switch (currency)
            {
                case CurrencyType.COINS:
                    if (UserManager.Instance.coins < unlockRequirement.quantity)
                    {
                        GamesToastsController.ShowToast(GamesToastsController.ToastStyle.ACTION_WARNING, "Not enought coins");
                        return;
                    }

                    //deduct
                    UserManager.Instance.coins -= unlockRequirement.quantity;
                    break;

                case CurrencyType.GEMS:
                    if (UserManager.Instance.gems < unlockRequirement.quantity)
                    {
                        GamesToastsController.ShowToast(GamesToastsController.ToastStyle.ACTION_WARNING, "Not enought gems");
                        return;
                    }

                    //deduct
                    UserManager.Instance.gems -= unlockRequirement.quantity;
                    break;


                case CurrencyType.TICKETS:
                    if (UserManager.Instance.gems < unlockRequirement.quantity)
                    {
                        GamesToastsController.ShowToast(GamesToastsController.ToastStyle.ACTION_WARNING, "Not enought tickets");
                        return;
                    }

                    //deduct
                    UserManager.Instance.tickets -= unlockRequirement.quantity;
                    break;
            }

            //unlock
            PlayerPrefsWrapper.SetThemeUnlocked((int)currentOpenedTheme.themeID, true);

            SetData(currentOpenedTheme);
        }
    }
}