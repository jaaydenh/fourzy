//@vadym udod

using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    [RequireComponent(typeof(Image))]
    public class MiniGameboardPiece : MonoBehaviour
    {
        public Sprite gamePieceBlue;
        public Sprite gamePieceRed;

        public Image image { get; private set; }

        protected void Awake()
        {
            image = GetComponent<Image>();
        }

        public void SetGamePiece(Piece piece)
        {
            if (piece == Piece.BLUE)
                image.sprite = gamePieceBlue;
            else if (piece == Piece.RED)
                image.sprite = gamePieceRed;
        }
    }
}
