//@vadym udod

using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class BoardEdgeXMark : MonoBehaviour
    {
        protected AlphaTween alphaTween;

        private GameboardView board;
        private bool isShown;

        protected void Awake()
        {
            alphaTween = GetComponentInChildren<AlphaTween>(true);
            board = GetComponentInParent<GameboardView>();
        }

        public virtual void Hide(float time)
        {
            if (!isShown) return;
            isShown = false;

            if (alphaTween)
            {
                if (time == 0f)
                    alphaTween.SetAlpha(0f);
                else
                {
                    alphaTween.playbackTime = time;
                    alphaTween.PlayBackward(true);
                }
            }
        }

        public virtual void Show(float time)
        {
            if (alphaTween)
            {
                if (time == 0f)
                    alphaTween.SetAlpha(1f);
                else
                {
                    alphaTween.playbackTime = time;
                    alphaTween.PlayForward(true);
                }
            }
        }

        public void SetProgress(float value)
        {
            if (value > 0f) isShown = true;

            alphaTween.AtProgress(value);
        }

        public void Position(BoardLocation boardLocation) => transform.localPosition = board.BoardLocationToVec2(boardLocation);
    }
}