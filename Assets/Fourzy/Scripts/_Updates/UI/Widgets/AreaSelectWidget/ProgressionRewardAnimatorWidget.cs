//@vadym udod

using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
using System;
using System.Collections;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class ProgressionRewardAnimatorWidget : WidgetBase
    {
        [SerializeField]
        private ParticleSystem rewardParticles;
        [SerializeField]
        private TweenBase tween;
        [SerializeField]
        private ProgressionRewardWidget rewardPrefab;

        private ProgressionRewardWidget _currentReward;

        public void Animate(float duration)
        {
            tween.PlayForward(true);
            rewardParticles.Play(true);

            StartRoutine("animation", duration, OnComplete);
        }

        public void SetReward(BundleItem item)
        {
            CancelRoutine("set_item");
            StartRoutine("set_item", UpdateRewardRoutine(item));
        }

        public void RemoveCurrentReward()
        {
            StartRoutine("remove_current", RemoveCurrentRewardRoutine());
        }

        public void AnimateProgress()
        {
            if (_currentReward)
            {
                _currentReward.AnimateProgress();
            }
        }

        private void OnComplete()
        {
            tween.PlayBackward(true);
            rewardParticles.Stop();
        }

        private IEnumerator UpdateRewardRoutine(BundleItem item)
        {
            CancelRoutine("remove_current");
            yield return RemoveCurrentRewardRoutine();

            _currentReward = Instantiate(rewardPrefab, transform);
            _currentReward.gameObject.SetActive(true);
            switch (item.ItemClass)
            {
                case Constants.PLAYFAB_GAMEPIECE_CLASS:
                    _currentReward.ShowGamepiece(item.ItemId);

                    break;

                case Constants.PLAYFAB_TOKEN_CLASS:
                    _currentReward.ShowToken((TokenType)Enum.Parse(typeof(TokenType), item.ItemId));

                    break;
            }

            _currentReward.PlayForward();
        }

        private IEnumerator RemoveCurrentRewardRoutine()
        {
            if (!_currentReward) yield break;

            _currentReward.PlayBackward();

            yield return new WaitForSeconds(_currentReward.AnimationTime);
            Destroy(_currentReward.gameObject);
        }
    }
}