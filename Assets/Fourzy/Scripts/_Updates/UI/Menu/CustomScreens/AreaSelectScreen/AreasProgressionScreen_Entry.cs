//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Helpers;
using FourzyGameModel.Model;
using PlayFab.ClientModels;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Fourzy._Updates.Serialized;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class AreasProgressionScreen_Entry : MonoBehaviour
    {
        [SerializeField]
        private ButtonExtended unlockFourzyButton;
        [SerializeField]
        private Image gamepieceImage;
        [SerializeField]
        private ButtonExtended unlockTokenButton;
        [SerializeField]
        private Image tokenImage;
        [SerializeField]
        private ButtonExtended unlockAreaButton;
        [SerializeField]
        private Image areaImage;
        [SerializeField]
        private TMP_Text gamesToPlayLabel;

        public RectTransform rectTransform { get; private set; }
        public float FillAmount { get; private set; }

        private void Awake()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
        }

        public void Initialize(int targetValue, float fillAmount, CatalogItem item)
        {
            FillAmount = fillAmount;
            gamesToPlayLabel.text = targetValue + "";

            switch (item.ItemClass)
            {
                case Constants.PLAYFAB_TOKEN_CLASS:
                    unlockTokenButton.SetActive(true);
                    unlockTokenButton.SetState(fillAmount == 1f);

                    TokensDataHolder.TokenData tokenData = GameContentManager.Instance.GetTokenData((TokenType)Enum.Parse(typeof(TokenType), item.ItemId));
                    unlockTokenButton.SetLabel($"<color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}>Unlock Token</color>\n{LocalizationManager.Value(tokenData.name)}", "value");
                    tokenImage.sprite = tokenData.GetTokenSprite();

                    break;

                case Constants.PLAYFAB_GAMEPIECE_CLASS:
                    unlockFourzyButton.SetActive(true);
                    unlockFourzyButton.SetState(fillAmount == 1f);

                    GamePieceData _data = GameContentManager.Instance.piecesDataHolder.gamePiecesFastAccess[item.ItemId];
                    unlockFourzyButton.SetLabel($"<color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}>Unlock Fourzy</color>\n{_data.name}", "value");
                    Sprite gamepieceSprite = _data?.profilePicture;
                    if (gamepieceSprite)
                    {
                        gamepieceImage.rectTransform.pivot = _data.ProfilePicturePivot;
                        gamepieceImage.rectTransform.anchoredPosition = Vector2.zero;
                        gamepieceImage.sprite = gamepieceSprite;
                    }
                    else
                    {
                        gamepieceImage.color = Color.clear;
                        GamePieceView gamePiece = Instantiate(_data.player1Prefab, gamepieceImage.transform);

                        gamePiece.transform.localPosition = Vector3.zero;
                        gamePiece.StartBlinking();
                    }

                    break;

                case Constants.PLAYFAB_AREA_CLASS:
                    unlockAreaButton.SetActive(true);
                    unlockAreaButton.SetState(fillAmount == 1f);

                    AreasDataHolder.GameArea areaData = GameContentManager.Instance.areasDataHolder[(Area)Enum.Parse(typeof(Area), item.ItemId)];
                    areaImage.sprite = areaData._16X9;
                    unlockAreaButton.SetLabel($"<color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}>{(fillAmount == 1f ? "" : "Unlock ")}New World</color> {areaData.Name}", "value");

                    break;
            }
        }
    }
}
