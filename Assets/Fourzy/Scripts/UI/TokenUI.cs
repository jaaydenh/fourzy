using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class TokenUI : MonoBehaviour
    {
        public TokenData tokenData;

        public Text tokenNameText;
        public Image tokenImage;
        public GameObject tileBGImage;
        public GameObject selectButton;

        void Start()
        {
            Button btn = selectButton.GetComponent<Button>();
            btn.onClick.AddListener(OpenTokenPopup);
        }

        public void InitWithTokenData(TokenData token)
        {
            this.tokenData = token;

            tokenNameText.text = token.Name;
            tokenImage.sprite = GameContentManager.Instance.GetTokenSprite(token.ID);

            if (token.showBackgroundTile)
            {
                tileBGImage.SetActive(true);
            }
        }

        public void OpenTokenPopup() 
        {
            PopupManager.Instance.GetPopup<TokenPopupUI>().tokenData = tokenData;
            PopupManager.Instance.OpenPopup<TokenPopupUI>();
        }
    }
}
