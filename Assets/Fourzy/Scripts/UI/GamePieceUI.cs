using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class GamePieceUI : MonoBehaviour
    {
        public delegate void SetGamePiece(string gamePieceId);
        public static event SetGamePiece OnSetGamePiece;
        public string id;
        public string gamePieceName;
        public bool isEnabled;
        public bool isLocked;
        public bool isSelected;
        public GameObject selector;
        public Image gamePieceImage;

        public void SetAlternateColor(bool isAlternate) {
            Image rend = GetComponent<Image>();
            if (isAlternate) {
                rend.material.SetVector("_HSVAAdjust", new Vector4(0.3f, 0, 0, 0));
            } else {
                rend.material.SetVector("_HSVAAdjust", new Vector4(0, 0, 0, 0));    
            }
        }

        public void PieceSelect() {
            Toggle toggle = this.GetComponent<Toggle>();

            if (toggle.isOn)
            {
                //Debug.Log("GAME PIECE SELECTED ON");
                selector.SetActive(true);
                if (OnSetGamePiece != null)
                    OnSetGamePiece(id);
            }
            else
            {
                //Debug.Log("GAME PIECE SELECTED OFF");
                selector.SetActive(false);
            }
        }

        public void ActivateSelector() {
            Toggle toggle = this.GetComponent<Toggle>();
            toggle.isOn = true;
            selector.SetActive(true);
        }
    }
}
