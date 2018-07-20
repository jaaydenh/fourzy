using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class ViewPuzzleSelection : UIView
    {
        //Instance
        public static ViewPuzzleSelection instance;
        public GameObject puzzlePackGrid;
        public GameObject puzzlePackPrefab;
        public Text completedAndTotalCount;
        private int puzzlesCompletedCount = 0;
        private int puzzlesTotalCount = 0;

        // Use this for initialization
        void Start()
        {
            instance = this;
            keepHistory = true;
        }

        public override void Show()
        {
            base.Show();
            LoadPuzzlePacks();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void BackButton()
        {
            ViewController.instance.ChangeView(ViewController.instance.viewTraining);
            //ViewController.instance.ShowTabView();
        }

        public void LoadPuzzlePacks() {
            Debug.Log("LoadPuzzlePacks");
            puzzlesCompletedCount = 0;
            puzzlesTotalCount = 0;

            PuzzlePack[] puzzlePacks = PuzzleChallengeLoader.instance.GetPuzzlePacks();

            if (puzzlePackGrid.transform.childCount > 0) {
                for (int i = puzzlePackGrid.transform.childCount-1; i >= 0; i--) {
                    Transform puzzlePack = puzzlePackGrid.transform.GetChild(i);
                    DestroyImmediate(puzzlePack.gameObject);
                    // Transform piece = createGameGameboardGrid.transform.GetChild(i);
                    //Lean.LeanPool.Despawn(piece.gameObject);
                }
            }

            GameObject puzzlePackObject = Instantiate(puzzlePackPrefab) as GameObject;

            foreach (var puzzlePack in puzzlePacks)
            {
                puzzlesCompletedCount += GetCompletedCount(puzzlePack);
                puzzlesTotalCount += puzzlePack.PuzzleChallengeLevels.Count;

                GameObject go = Instantiate(puzzlePackPrefab) as GameObject;
                PuzzlePackUI puzzlePackUI = go.GetComponent<PuzzlePackUI>();
                Debug.Log("LoadPuzzlePacks: puzzlePack.ID: " + puzzlePack.ID);
                puzzlePackUI.InitPuzzlePack(puzzlePack);

                go.gameObject.transform.SetParent(puzzlePackGrid.transform);
            }

            completedAndTotalCount.text = puzzlesCompletedCount.ToString() + " / " + puzzlesTotalCount.ToString();

            // createGameGameboardGrid.transform.Translate(1900f,0,0);
        }

        int GetCompletedCount(PuzzlePack pack ) {
            int completedCount = 0;
            foreach (var level in pack.PuzzleChallengeLevels)
            {
                if (PlayerPrefs.GetInt("PuzzleChallengeID:" + level.ID) == 1) {
                    completedCount++;
                }
            }

            return completedCount;
        }
    }
}
