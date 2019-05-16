//@vadym udod

using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.Tools
{
    public class UIOutline : OutlineBase
    {
        public Image image { get; private set; }
        private Canvas canvas;

        protected override void Awake()
        {
            base.Awake();

            Initialize();
        }

        protected void Update()
        {
            if (!initialized)
                Initialize();
            
            image.materialForRendering.SetVector("_AnchoredPosition",
                new Vector2((transform.position.x - canvas.transform.position.x) / canvas.transform.lossyScale.x,
                (transform.position.y - canvas.transform.position.y) / canvas.transform.lossyScale.y));
            image.materialForRendering.SetFloat("_Intensity", intensity);
            image.materialForRendering.SetFloat("_OutlineBorder", size);
            image.materialForRendering.SetFloat("_BlurSize", blueSize);
            image.materialForRendering.SetColor("_OutlineColor", outlineColor);
        }

        protected void OnDestroy()
        {
            DestroyImmediate(material);

            image.material = null;
        }

        protected void Initialize()
        {
            if (initialized)
                return;

            initialized = true;

            image = GetComponent<Image>();
            canvas = GetComponentInParent<Canvas>();

            //assign outline material
            material = new Material(Shader.Find("Custom/UIOutline"));
            material.hideFlags = HideFlags.HideAndDontSave;

            image.material = material;
        }
    }
}