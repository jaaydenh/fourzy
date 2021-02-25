//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Toasts;
using FourzyGameModel.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

        private AreasDataHolder.GameArea currentOpenedTheme;

        public void Prompt(Area area)
        {
            AreasDataHolder.GameArea _themeData = GameContentManager.Instance.areasDataHolder[area];

            if (area == Area.NONE || _themeData == null) return;

            Prompt(_themeData.name, _themeData.description, null);

            //get buy buttons
            if (!InternalSettings.Current.UNLOCKED_AREAS.Contains(area))
            {
                foreach (ButtonExtended button in buyButtons) Destroy(button.gameObject);
                buyButtons.Clear();

                foreach (AreasDataHolder.AreaUnlockRequirement unlockRequirement in 
                    _themeData.unlockRequirements)
                {
                    ButtonExtended buttonInstance = Instantiate(buyButton, buttonsParent);
                    buttonInstance
                        .GetBadge(unlockRequirement.type.ToString()).badge
                        .SetValue(unlockRequirement.quantity);
                    buttonInstance.events.AddListener(() => Purchase(unlockRequirement.type));

                    buyButtons.Add(buttonInstance);
                }
            }

            SetData(_themeData);
        }

        public void SetData(AreasDataHolder.GameArea themeData)
        {
            if (themeData.areaID == Area.NONE) return;

            //refresh buttons
            if (InternalSettings.Current.UNLOCKED_AREAS.Contains(themeData.areaID) || 
                themeData.unlockRequirements.Count == 0)
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

            icon.sprite = themeData._16X9;

            foreach (GameObject gamePiece in gamePieces) Destroy(gamePiece);
            gamePieces.Clear();

            gamePiecesRoot.SetActive(themeData.gamepieces.Count > 0);
            foreach (GamePieceView gamePiecePrefab in themeData.gamepieces)
            {
                GameObject gamePieceHolderInstance = Instantiate(gamePieceHolder, gamePiecesParent);
                gamePieceHolderInstance.SetActive(true);

                GamePieceView gamePieceInstance = Instantiate(
                    gamePiecePrefab, 
                    gamePieceHolderInstance.transform);
                gamePieceInstance.transform.localScale = Vector3.one * 70f;
                gamePieceInstance.StartBlinking();

                gamePieces.Add(gamePieceHolderInstance);
            }

            foreach (GameObject tokenView in tokens) Destroy(tokenView);
            tokens.Clear();

            tokensRoot.SetActive(themeData.tokens.Count > 0);

            foreach (TokenType tokenType in themeData.tokens)
            {
                GameObject tokenHolderInstance = Instantiate(gamePieceHolder, tokensParent);
                tokenHolderInstance.SetActive(true);

                TokensDataHolder.TokenData tokenData = GameContentManager.Instance.GetTokenData(tokenType);

                Image bg = tokenHolderInstance.GetComponentInChildren<Image>(true);
                bg.gameObject.SetActive(tokenData.showBackgroundTile);
                bg.color = tokenData.backgroundTileColor;

                TokenView tokenInstance = Instantiate(
                    GameContentManager.Instance.GetTokenPrefab(tokenType, themeData.areaID), 
                    tokenHolderInstance.transform);
                tokenInstance.transform.localScale = Vector3.one * (tokenData.showBackgroundTile ? 55f : 70f);

                tokens.Add(tokenHolderInstance);
            }

            currentOpenedTheme = themeData;
        }

        public override void Accept(bool force = false)
        {
            base.Accept(force);

            GameContentManager.Instance.currentArea = currentOpenedTheme;
            CloseSelf();
        }

        public void Purchase(CurrencyType currency)
        {
            AreasDataHolder.AreaUnlockRequirement unlockRequirement = currentOpenedTheme.GetRequirement(currency);

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
            PlayerPrefsWrapper.SetAreaUnlocked((int)currentOpenedTheme.areaID, true);

            SetData(currentOpenedTheme);
        }
    }
}