using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class TokenUI : MonoBehaviour
    {
        //public delegate void SetGamePiece(string gamePieceId);
        //public static event SetGamePiece OnSetGamePiece;
        public string id;
        public string tokenName;
        public string description;
        public string arenaName;
        public bool isEnabled;
        public bool isLocked;
        public bool showBackgroundTile;
        public Text tokenNameText;
        public Image tokenImage;
        public GameObject tileBGImage;
        public GameObject selectButton;
        private GameObject tokenPopup;
        private TokenPopupUI tokenPopupUI;

        void Start()
        {
            tokenPopup = GameManager.instance.tokenPopupUI;
            tokenPopupUI = tokenPopup.GetComponent<TokenPopupUI>();

            Button btn = selectButton.GetComponent<Button>();
            btn.onClick.AddListener(OpenTokenPopup);
        }

        public void OpenTokenPopup() {
            Debug.Log("OpenTokenPopup:tokenName: " + tokenName);
            tokenPopupUI.Open();
            tokenPopupUI.tokenName.text = tokenName;
            tokenPopupUI.tokenArena.text = arenaName;
            tokenPopupUI.description.text = description;
            tokenPopupUI.tokenImage.sprite = tokenImage.sprite;
            if (showBackgroundTile)
            {
                tokenPopupUI.tileBGImage.SetActive(true);
            } else {
                tokenPopupUI.tileBGImage.SetActive(false);
            }

        }
    }
}
