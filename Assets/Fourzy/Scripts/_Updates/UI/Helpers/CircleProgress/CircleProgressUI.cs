//@vadym udod

using Fourzy._Updates.Tween;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Helpers
{
    [RequireComponent(typeof(ValueTween))]
    [RequireComponent(typeof(Image))]
    public class CircleProgressUI : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float value = 0f;

        private bool initialized = false;

        private Image image;
        private Material material;

        private ValueTween valueTween;

        protected void Awake()
        {
            Initialize();
        }

        protected void Update()
        {
            if (!initialized)
                return;

            image.materialForRendering.SetFloat("_Progress", value * Mathf.PI * 2f);
        }

        protected void OnDestroy()
        {
            if (material)
                DestroyImmediate(material);
        }

        private void Initialize()
        {
            if (initialized || !Application.isPlaying)
                return;

            initialized = true;

            image = GetComponent<Image>();
            valueTween = GetComponent<ValueTween>();

            material = new Material(image.material);
            material.hideFlags = HideFlags.HideAndDontSave;

            image.material = material;
        }

        public void Animate(float from, float to, float by)
        {
            valueTween.from = from;
            valueTween.to = to;
            valueTween.playbackTime = by;

            valueTween.PlayForward(true);
        }

        public void Animate(float to, float by)
        {
            valueTween.from = valueTween.value * valueTween.to;
            valueTween.to = to;
            valueTween.playbackTime = by;

            valueTween.PlayForward(true);
        }

        public void Animate(float to)
        {
            valueTween.from = valueTween.value * valueTween.to;
            valueTween.to = to;

            valueTween.PlayForward(true);
        }

        public void SetColor(Color color)
        {
            image.color = color;
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}