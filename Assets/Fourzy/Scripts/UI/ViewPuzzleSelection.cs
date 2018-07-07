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
                GameObject go = Instantiate(puzzlePackPrefab) as GameObject;
                PuzzlePackUI puzzlePackUI = go.GetComponent<PuzzlePackUI>();
                puzzlePackUI.InitPuzzlePack(puzzlePack);

                go.gameObject.transform.SetParent(puzzlePackGrid.transform);
            }

            // createGameGameboardGrid.transform.Translate(1900f,0,0);
        }
    }
}
