//@vadym udod

using Fourzy._Updates.Mechanics.GameplayScene;
using UnityEngine;

namespace Fourzy._Updates.Tools
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineRendererWave : MonoBehaviour, IGameplayBGPart
    {
        [Range(5, 20)]
        public int positionCount = 10;
        [Range(.5f, 10f)]
        public float height = 1f;
        [Range(0f, 3f)]
        public float animationSpeed;
        [Range(5, 60)]
        public int fps = 30;

        public AnimationCurve animationCurve;

        private LineRenderer lineRenderer;
        private float maxOffset;
        private float originalHeight;
        private float originalWidth;
        private float step;
        private float timer = 0;
        private bool initialized = false;
        private float randomStart;

        protected void Awake()
        {
            Initialize();
        }

        protected void Update()
        {
            if ((timer += Time.deltaTime) >= step)
            {
                timer -= step;
                Animate();
            }
        }

        public void Initialize()
        {
            if (initialized) return;

            if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();

            lineRenderer.positionCount = positionCount;
            originalHeight = height;
            originalWidth = lineRenderer.widthMultiplier;
            step = 1f / fps;
            timer = 0f;
            randomStart = Random.value;

            initialized = true;
        }

        public void Animate()
        {
            if (!lineRenderer) return;

            float step = height / positionCount;
            for (int positionIndex = 0; positionIndex < positionCount; positionIndex++)
            {
                float relativeIndex = (float)positionIndex / positionCount;
                float offsetValue = animationCurve.Evaluate(relativeIndex + randomStart + Time.time * animationSpeed);

                if (offsetValue > maxOffset)
                    maxOffset = offsetValue;

                lineRenderer.SetPosition(positionIndex,
                    new Vector3(
                        transform.position.x + (offsetValue - maxOffset * .5f) * relativeIndex,
                        transform.position.y + (positionIndex - (positionCount * .5f)) * step,
                        transform.position.z));
            }
        }

        public void OnScale(float value)
        {
            Initialize();

            lineRenderer.widthMultiplier = originalWidth * value;
            height = originalHeight * value;
        }
    }
}