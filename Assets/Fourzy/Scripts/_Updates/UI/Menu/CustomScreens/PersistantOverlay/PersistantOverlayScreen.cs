//@vadym udod

using Fourzy._Updates.Mechanics._Vfx;
using Fourzy._Updates.UI.Widgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PersistantOverlayScreen : MenuScreen
    {
        public static PersistantOverlayScreen instance;

        public RewardAnimationWidget animationWidgetPrefab;

        protected override void Awake()
        {
            base.Awake();

            if (!instance) instance = this;
        }

        public string AnimateReward(bool spawnOrigin, RewardType type, int amount, Vector2 viewportPoint)
        {
            HeaderScreen.instance.SneakABit();
            string animationID = Random.value + "";

            StartRoutine(animationID, AnimateRewardRoutine(
                spawnOrigin,
                type,
                amount, new Vector2(viewportPoint.x * menuController.size.x, viewportPoint.y * menuController.size.y)));

            return animationID;
        }

        public void CancelAnimationReward(string animationID) => CancelRoutine(animationID);

        private IEnumerator AnimateRewardRoutine(bool spawnOrigin, RewardType type, int amount, Vector2 from)
        {
            int maxParticles = 40;
            float spawnDuration = .3f;
            float step = spawnDuration / amount;

            //spawn 'origin'
            RewardAnimationWidget origin = null;
            if (spawnOrigin)
            {
                origin = Instantiate(animationWidgetPrefab, transform);
                origin.
                    PlayAnimationDelayed(.8f).
                    SetData(type, amount).
                    SetPosition(from);
            }

            if (spawnOrigin) yield return new WaitForSeconds(1.3f);
            else yield return new WaitForSeconds(.5f);

            Vector2 to = HeaderScreen.instance.GetCurrencyWidget(type).transform.position;

            int max = Mathf.Min(amount, maxParticles);
            for (int _index = 0; _index < max; _index++)
            {
                RewardAnimationParticleVfx vfx = VfxHolder.instance.GetVfx<RewardAnimationParticleVfx>(VfxType.UI_REWARD_ANIMATION_PARTICLE);
                vfx.SetData(
                    transform, 
                    type, 
                    from, 
                    to + new Vector2(Random.Range(-10f, 10f), Random.Range(-5f, 5f)),
                    Random.Range(0, 360f));

                yield return new WaitForSeconds(step);
            }

            yield return new WaitForSeconds(.5f);

            if (spawnOrigin)
            {
                //yield return new WaitForSeconds(.3f);
                Destroy(origin.gameObject);
            }
        }
    }
}
