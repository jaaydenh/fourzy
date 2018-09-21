using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Fourzy
{
    public class TokenPopupUI : MonoBehaviour
    {
        public Text tokenName;
        public Text tokenArena;
        public TextMeshProUGUI description;
        public Image tokenImage;
        public GameObject tileBGImage;

        public GameObject closeButton;

        [SerializeField]
        private TokenInstructionGameboardManager tokenInstruction;

        private GameObject cachedGO;

        void Awake()
        {
            Button btn = closeButton.GetComponent<Button>();
            btn.onClick.AddListener(Close);
        }

        public void Close()
        {
            tokenInstruction.Close();
            gameObject.SetActive(false);
        }

        public bool IsOpen()
        {
            return gameObject.activeSelf;
        }

        public void Open(int tokenId)
        {
            TokenData tokenData = GameContentManager.Instance.GetTokenData(tokenId);
            Open(tokenData);
        }

        public void Open(Token tokenType)
        {
            TokenData tokenData = GameContentManager.Instance.GetTokenDataWithType(tokenType);
            Open(tokenData);
        }

        public void Open(TokenData tokenData)
        {
            gameObject.SetActive(true);

            Sprite sprite = GameContentManager.Instance.GetTokenSprite(tokenData.ID);

            tokenName.text = tokenData.Name;
            tokenArena.text = tokenData.Arena;
            description.text = tokenData.Description;
            tokenImage.sprite = sprite;
            tileBGImage.SetActive(tokenData.showBackgroundTile);

            tokenInstruction.Init(tokenData.GameBoardInstructionID);
        }
    }
}


