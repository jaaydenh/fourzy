//@vadym udod

using Sirenix.OdinInspector;
using UnityEngine;

namespace Fourzy._Updates.UI.Camera3D
{
    public class Camera3dItemProgressionMapChunk : MonoBehaviour
    {
        public Vector2 size;
        public SpriteRenderer bg;

        public float left { get; set; }
        public float right { get; set; }
        

        //editor thingis
        [Button("Size to BG")]
        public void ResetSizeToBG()
        {
            if (!bg || !bg.sprite) return;

            size = bg.bounds.size;
        }
    }
}