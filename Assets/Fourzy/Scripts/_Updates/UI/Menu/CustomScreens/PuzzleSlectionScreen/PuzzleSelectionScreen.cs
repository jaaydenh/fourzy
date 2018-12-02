//@vadym udod

using Fourzy._Updates.UI.Widgets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PuzzleSelectionScreen : MenuScreen
    {
        public PuzzlePackWidget puzzlePackPrefab;
        public GridLayoutGroup gridLayoutGroup;
        public TMP_Text completeText;

        public int totalCount { get; private set; }
        public int completedCount { get; private set; }

        public override void Open()
        {
            base.Open();

            LoadPuzzlePacks();
        }

        private void LoadPuzzlePacks()
        {
            //remove old one
            foreach (Transform child in gridLayoutGroup.transform)
                Destroy(child.gameObject);

            totalCount = 0;
            completedCount = 0;

            foreach (PuzzlePack puzzlePack in PuzzleChallengeLoader.Instance.GetPuzzlePacks())
            {
                completedCount += puzzlePack.completedCount;
                totalCount += puzzlePack.PuzzleChallengeLevels.Count;

                PuzzlePackWidget puzzlePackObject = Instantiate(puzzlePackPrefab, gridLayoutGroup.transform);
                puzzlePackObject.InitPuzzlePack(puzzlePack, completedCount);
            }

            completeText.text = completedCount.ToString() + " / " + totalCount.ToString();
        }
    }
}
