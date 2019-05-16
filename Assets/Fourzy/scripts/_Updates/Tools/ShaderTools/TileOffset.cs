//@vadym udod

using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.Tools
{
    public class TileOffset : MonoBehaviour
    {
        public static Material material;

        [Range(.01f, .2f)]
        public float xSpeed = .03f;
        [Range(.01f, .2f)]
        public float ySpeed = .03f;

        private Image image;
        private Vector4 offset = Vector4.zero;

        protected void Awake()
        {
            image = GetComponent<Image>();
            Material _material;

            if (!material)
            {
                material = new Material(Shader.Find("UI/Default Offset"));
                material.hideFlags = HideFlags.HideAndDontSave;

                _material = material;
            }
            else
                _material = new Material(material);

            image.material = _material;
        }

        protected void Update()
        {
            if (!image)
                return;

            offset.x += xSpeed * Time.deltaTime;
            offset.y += ySpeed * Time.deltaTime;

            image.material.SetVector("_Offset", offset);
        }
    }
}
