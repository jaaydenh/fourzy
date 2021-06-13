//@vadym udod


using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.GameplayScene;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenBG : WidgetBase
    {
        public bool shown = false;

        [SerializeField]
        private RectTransform piecesParent;

        private List<GamePieceView> gamepieces = new List<GamePieceView>();

        public void _Hide()
        {
            if (alphaTween._value > 0f)
            {
                Hide(.3f);
            }

            SetInteractable(false);
            BlockRaycast(false);

            shown = false;
        }

        public void _Show(bool show)
        {
            if (show)
            {
                Show(.3f);
            }

            SetInteractable(true);
            BlockRaycast(true);

            shown = true;
        }

        public void ShowGamepieces()
        {
            foreach (GamePieceView view in gamepieces)
            {
                Destroy(view.gameboard);
            }

            List<GamePieceView> toSpawn = GamePlayManager.Instance.board.gamePieces.Select(_piece => _piece as GamePieceView).ToList();

            //move them to ui layer
            for (int index = 0; index < toSpawn.Count; index++)
            {
                GamePieceView gamepieceView = Instantiate(toSpawn[index], piecesParent);

                Vector2 viewportPoint = Camera.main.WorldToViewportPoint(toSpawn[index].transform.position);

                gamepieceView.SetAnchor(viewportPoint);
                gamepieceView.transform.localScale = (GameManager.Instance.Landscape ? 42f : 33f) * Vector3.one;

                gamepieces.Add(gamepieceView);
            }
        }

        public void HideGamepieces()
        {
            foreach (GamePieceView gamepiece in gamepieces)
            {
                Destroy(gamepiece.gameboard);
            }
        }
    }
}
