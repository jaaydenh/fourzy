//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Tools
{
    [RequireComponent(typeof(LineRenderer))]
    [ExecuteInEditMode]
    public class LineRendererWave : MonoBehaviour
    {
        [Range(5, 20)]
        public int positionCount = 10;
        [Range(.5f, 10f)]
        public float height = 1f;
        [Range(0f, 1f)]
        public float animationTimeOffset;
        [Range(0f, 3f)]
        public float animationSpeed;

        public AnimationCurve animationCurve;

        private LineRenderer lineRenderer;
        private float maxOffset;

        protected void Update()
        {
            UpdateShape();
            Animate();
        }

        public void UpdateShape()
        {
            if (!lineRenderer)
                lineRenderer = GetComponent<LineRenderer>();

            lineRenderer.positionCount = positionCount;
        }

        public void Animate()
        {
            if (!lineRenderer)
                return;

            float step = height / positionCount;
            for (int positionIndex = 0; positionIndex < positionCount; positionIndex++)
            {
                float relativeIndex = (float)positionIndex / positionCount;
                float offsetValue = animationCurve.Evaluate(relativeIndex + Time.time * animationSpeed + animationTimeOffset);

                if (offsetValue > maxOffset)
                    maxOffset = offsetValue;

                lineRenderer.SetPosition(positionIndex,
                    new Vector3(
                        transform.position.x + (offsetValue - maxOffset * .5f) * relativeIndex,
                        transform.position.y + (positionIndex - (positionCount * .5f)) * step,
                        transform.position.z));
            }
        }
    }
}