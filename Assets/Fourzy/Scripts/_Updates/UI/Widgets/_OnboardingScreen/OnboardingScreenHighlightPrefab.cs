//@vadym udod

using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Mechanics.GameplayScene;
using FourzyGameModel.Model;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenHighlightPrefab : WidgetBase
    {
        public void SetArea(Rect area)
        {
            GameboardView board = GamePlayManager.Instance.BoardView;

            if (board == null)
                return;

            Vector2 shapeSize = menuScreen.menuController.WorldToCanvasSize(new Vector2(board.step.x * area.width, board.step.y * area.height));
            rectTransform.sizeDelta = shapeSize;

            rectTransform.anchoredPosition = menuScreen.menuController.WorldToCanvasPoint((board.transform.position + (Vector3)board.BoardLocationToVec2(new BoardLocation((int)area.y, (int)area.x))))
                - new Vector2(-(area.width - 1) * (shapeSize.x / area.width), (area.height - 1) * (shapeSize.y / area.height)) * .5f;
        }
    }
}
