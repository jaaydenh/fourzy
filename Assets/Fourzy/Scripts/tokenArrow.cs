using Fourzy._Updates.Audio;
using Fourzy._Updates.Serialized;
using UnityEngine;

namespace Fourzy
{
    public class tokenArrow : MonoBehaviour
    {
        public AudioClip soundEffect;
        public AudioTypes clipType;
        public float volume = 1f;

        void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.tag == "GamePiece")
            {
                AudioHolder.instance.PlaySelfSfxOneShotTracked(clipType, volume);
            }
        }
    }
}

