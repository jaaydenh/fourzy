//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Tools
{
    public class SpriteRendererOutline : OutlineBase
    {
        public SpriteRenderer spriteRenderer { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            Initialize();
        }

        protected void Update()
        {
            if (!initialized)
                Initialize();

            spriteRenderer.sharedMaterial.SetFloat("_Intensity", intensity);
            spriteRenderer.sharedMaterial.SetFloat("_OutlineBorder", size);
            spriteRenderer.sharedMaterial.SetFloat("_BlurSize", blueSize);
            spriteRenderer.sharedMaterial.SetColor("_OutlineColor", outlineColor);
        }

        protected void OnDestroy()
        {
            DestroyImmediate(material);

            spriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        protected void Initialize()
        {
            if (initialized)
                return;

            initialized = true;

            spriteRenderer = GetComponent<SpriteRenderer>();

            //assign outline material
            material = new Material(Shader.Find("Custom/HSVRangeOutlineShader"));
            material.hideFlags = HideFlags.HideAndDontSave;

            spriteRenderer.sharedMaterial = material;
        }
    }
}