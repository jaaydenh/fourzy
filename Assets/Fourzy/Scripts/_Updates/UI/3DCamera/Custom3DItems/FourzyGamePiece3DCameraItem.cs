//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.UI.Camera3D
{
    public class FourzyGamePiece3DCameraItem : Camera3DItem
    {
        public Transform pieceParent;

        [HideInInspector]
        public GamePiece current;

        public void SetGamePiece(GamePiece gamePiece)
        {
            if (current)
                Destroy(current.gameObject);
            
            current = Instantiate(gamePiece, pieceParent);
            current.transform.localPosition = Vector3.zero;

            current.gameObject.SetActive(true);
        }
    }
}