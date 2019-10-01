//@vadym udod

using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Mechanics.GameplayScene;
using FourzyGameModel.Model;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenPointer : WidgetBase
    {
        public RectTransform pointerGraphics;

        protected override void Awake()
        {
            base.Awake();

            SetInteractable(false);
            BlockRaycast(false);

            Hide(0f);
        }

        public void PointAt(BoardLocation location)
        {
            SetAnchors(Vector2.zero);

            GameboardView board = GamePlayManager.instance.board;

            if (board == null) return;

            pointerGraphics.anchoredPosition = menuScreen.menuController.WorldToCanvasPoint((board.transform.position + (Vector3)board.BoardLocationToVec2(location)));
        }
    }
}