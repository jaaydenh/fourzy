//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics._GamePiece;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class MovesLeftWidget : WidgetBase
    {
        public RectTransform container;
        public GameObject gamePieceHolderPrefab;
        public Material colorBlendMaterial;

        private List<GameObject> gamePieceHolders = new List<GameObject>();
        private ClientFourzyPuzzle puzzle;

        public MovesLeftWidget SetData(ClientFourzyPuzzle puzzle)
        {
            this.puzzle = puzzle;

            _UpdateVisible();

            //clear
            foreach (GameObject go in gamePieceHolders)
            {
                Destroy(go);
            }
            gamePieceHolders.Clear();

            //spawn
            for (int moveIndex = 0; moveIndex < puzzle.MoveLimit; moveIndex++)
            {
                GameObject gmHolder = Instantiate(gamePieceHolderPrefab, container);
                gmHolder.SetActive(true);

                GamePieceView gamePiece = Instantiate(puzzle.myGamePiece, gmHolder.transform);

                gamePiece.transform.localPosition = Vector3.zero;
                gamePiece.transform.localScale = Vector3.one * 70f;

                gamePieceHolders.Add(gmHolder);
            }

            return this;
        }

        public void UpdateMovesLeft()
        {
            _UpdateVisible();

            for (int moveIndex = 0; moveIndex < puzzle.MoveLimit; moveIndex++)
            {
                if (moveIndex > puzzle.MoveLimit - puzzle._playerTurnRecord.Count - 1)
                {
                    gamePieceHolders[moveIndex].GetComponentInChildren<GamePieceView>().SetMaterial(colorBlendMaterial);
                }
            }
        }

        public void _UpdateVisible()
        {
            if (puzzle.MoveLimit < 1)
            {
                if (alphaTween._value > 0f)
                {
                    Hide(.3f);
                }

                return;
            }

            if (alphaTween._value < 1f)
            {
                Show(.3f);
            }
        }
    }
}