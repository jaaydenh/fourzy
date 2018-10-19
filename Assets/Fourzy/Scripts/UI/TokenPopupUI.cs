using UnityEngine;
using UnityEngine.UI;
using TMPro;
using mixpanel;

namespace Fourzy
{
    public class TokenPopupUI : MonoBehaviour, IPopup
    {
        public TokenData tokenData;

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

            PopupManager.Instance.ClosePopup();
        }

        public bool IsOpen()
        {
            return gameObject.activeSelf;
        }

        void IPopup.Open()
        {
            gameObject.SetActive(true);
            var props = new Value();
            props["Token Type"] = tokenData.Name;
            Mixpanel.Track("Open Token Detail", props);
            
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


