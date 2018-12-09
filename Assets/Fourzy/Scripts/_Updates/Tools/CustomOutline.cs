//@vadym udod

using Fourzy._Updates.Tween;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.Tools
{
    [ExecuteInEditMode]
    public class CustomOutline : RoutinesBase
    {
        [Range(0f, 3f)]
        public float intensity = 1f;
        [Range(0f, 3f)]
        public float size = 1.15f;
        [Range(0f, .2f)]
        public float blueSize = .03f;
        public Color outlineColor = Color.black;

        public Image image { get; private set; }
        public ValueTween tween { get; private set; }

        private bool initialized = false;
        private Material material;
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
                new Vector4((transform.position.x - canvas.transform.position.x) / canvas.transform.localScale.x,
                (transform.position.y - canvas.transform.position.y) / canvas.transform.localScale.y));
            image.materialForRendering.SetFloat("_Intensity", intensity);
            image.materialForRendering.SetFloat("_OutlineBorder", size);
            image.materialForRendering.SetFloat("_BlurSize", blueSize);
            image.materialForRendering.SetColor("_OutlineColor", outlineColor);
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
            material = new Material(Shader.Find("Custom/Outline"));
            material.hideFlags = HideFlags.HideAndDontSave;

            image.material = material;
        }

        public void Animate(float from, float to, float time, bool repeat)
        {
            if (!tween)
            {
                float tan45 = Mathf.Tan(Mathf.Deg2Rad * 45);

                tween = gameObject.AddComponent<ValueTween>();
                tween.curve = new AnimationCurve(new Keyframe(0f, 0f, tan45, tan45), new Keyframe(.5f, 1f), new Keyframe(1f, 0f, tan45, tan45));

                tween._onProgress += (value) => { intensity = value; };
            }

            tween.from = from;
            tween.to = to;
            tween.playbackTime = time;
            tween.repeat = repeat ? RepeatType.ZERO_TO_ONE : RepeatType.NONE;

            tween.PlayForward(true);
        }

        public void StopAnimation()
        {
            tween.repeat = RepeatType.NONE;
        }

        public void HideOutline()
        {
            intensity = 0f;

            if (tween)
                tween.StopTween(false);
        }
    }
}