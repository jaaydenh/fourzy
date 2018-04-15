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
        public bool isEnabled;
        public bool isLocked;
        public Text tokenNameText;
        public Image tokenImage;
    }
}
