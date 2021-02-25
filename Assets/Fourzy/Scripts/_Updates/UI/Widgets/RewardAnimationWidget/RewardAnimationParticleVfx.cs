//@vadym udod

using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Widgets;
using UnityEngine;

namespace Fourzy._Updates.Mechanics._Vfx
{
    public class RewardAnimationParticleVfx : Vfx
    {
        public float controlScale = 400f;
        public RectTransform iconParent;

        private BezierPositionTween bezierPosition;
        private ScaleTween vfxScaleTween;

        protected void Awake()
        {
            bezierPosition = GetComponent<BezierPositionTween>();
            vfxScaleTween = iconParent.GetComponent<ScaleTween>();
        }

        protected override void Update()
        {
            base.Update();

            if (isActive && durationLeft != 0f)
            {
                if (durationLeft >= vfxScaleTween.playbackTime && durationLeft - Time.deltaTime < vfxScaleTween.playbackTime) vfxScaleTween.PlayForward(true);
            }
        }

        public void SetData(Transform parent, RewardType type, Vector2 from, Vector2 to, float angle)
        {
            GameObject iconInstance = Instantiate(GameContentManager.GetPrefab<RewardsScreenWidget>(type.AsPrefabType()).icon, iconParent);
            iconInstance.transform.localPosition = Vector3.zero;
            vfxScaleTween.OnReset();

            bezierPosition.from = from;
            bezierPosition.to = to;
            bezierPosition.control = /*from + */angle.VectorFromAngle() * controlScale;

            bezierPosition.PlayForward(true);

            StartVfx(parent, Vector2.zero, 0f);

            transform.position = from;
        }
    }
}