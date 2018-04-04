using UnityEngine;

namespace Fourzy
{
    public class tokenSticky : MonoBehaviour
    {
        public AudioClip soundEffect;

        void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.tag == "GamePiece")
            {
                SoundManager.instance.PlayRandomizedSfx(soundEffect);
            }
        }
    }

}
