//@vadym udod

using Fourzy._Updates.Mechanics.Vfx;
using Fourzy._Updates.Tween;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class RewardHolderWidget : WidgetBase
    {
        [HideInInspector]
        public WidgetBase rewardWidget;
        [HideInInspector]
        public BezierPositionTween bezier;
        [HideInInspector]
        public SquishStretchTween squishTween;
        [HideInInspector]
        public Vfx starsVfx;

        protected override void Awake()
        {
            base.Awake();

            bezier = GetComponent<BezierPositionTween>();
            squishTween = GetComponent<SquishStretchTween>();
        }

        public void SetRewardWidget(Reward reward)
        {
            switch (reward.Type)
            {
                case Reward.RewardType.Coins:
                    rewardWidget = GameContentManager.InstantiatePrefab<WidgetBase>(GameContentManager.PrefabType.COINS_WIDGET_SMALL, transform);
                    ((CoinsWidgetSmall)rewardWidget).SetValue(reward.NumberOfCoins);
                    break;
                case Reward.RewardType.CollectedGamePiece:
                    rewardWidget = GameContentManager.InstantiatePrefab<WidgetBase>(GameContentManager.PrefabType.GAME_PIECE_SMALL, transform);
                    ((GamePieceWidgetSmall)rewardWidget).SetData(reward.CollectedGamePiece);
                    break;
            }

            rewardWidget.transform.localPosition = Vector3.zero;

            starsVfx = VfxHolder.instance.ShowVfx(VfxType.VFX_STARS_TRAIL, transform);
            starsVfx.transform.SetAsFirstSibling();

            ScaleTo(Vector3.zero, Vector3.one, 1f);
        }

        public void BezierMoveTo(Vector3 to, Vector3 control, float time)
        {
            bezier.playbackTime = time;
            bezier.from = rectTransform.anchoredPosition;
            bezier.to = to;
            bezier.control = control;

            bezier.PlayForward(true);
        }

        public void CancelAnimation()
        {
            bezier.AtProgress(1f);
            scaleTween.AtProgress(1f);

            starsVfx.Disable();
        }
    }
}
