using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Fourzy
{
    public class PuzzlePackUI : MonoBehaviour {

        public PuzzlePack PuzzlePack { get; private set; }
        public Text puzzlePackName;
        public Text completedAndTotalCount;
        public Text starsToUnlockText;
        public GameObject lockInfo;
        private int puzzlesCompletedCount;
        private bool isLocked = true;
        public Image packImage;

        void Start () {
            Button btn = this.GetComponent<Button>();
            btn.onClick.AddListener(OpenPuzzlePack);
            // packImage = this.GetComponent<Image>();
        }

        // private void OnEnable()
        // {
        //     GamePlayManager.OnPuzzleCompleted += UpdatePuzzlePack;
        // }

        // private void OnDisable()
        // {
        //     GamePlayManager.OnPuzzleCompleted -= UpdatePuzzlePack;
        // }

        public void InitPuzzlePack(PuzzlePack puzzlePack, int puzzlesCompletedCount) {
            // Debug.Log("InitPuzzlePack");
            
            this.PuzzlePack = puzzlePack;
            //TODO: set active level based off of levels already completed
            this.PuzzlePack.ActiveLevel = GetNextLevel();
            this.puzzlesCompletedCount = puzzlesCompletedCount;

            // Debug.Log("puzzlePack.Name: " + puzzlePack.Name);
            puzzlePackName.text = puzzlePack.Name;
            // Debug.Log("puzzlePack.PuzzleChallengeLevels.Count: " + puzzlePack.PuzzleChallengeLevels.Count);

            // Debug.Log("puzzlesCompletedCount: " + puzzlesCompletedCount);

            if (puzzlesCompletedCount >= puzzlePack.CompletedToUnlock) {
                lockInfo.SetActive(false);
                isLocked = false;
                completedAndTotalCount.enabled = true;
                completedAndTotalCount.text = GetCompletedCount() + " / " + puzzlePack.PuzzleChallengeLevels.Count.ToString();
            } else {
                lockInfo.SetActive(true);
                isLocked = true;
                starsToUnlockText.text = puzzlePack.CompletedToUnlock.ToString();
                completedAndTotalCount.enabled = false;

                Color c = new Color(1f,1f,1f,0.75f);
                packImage.color = c;
            }
        }

        void OpenPuzzlePack() {
            if (!isLocked) {
                ViewController.instance.viewPuzzleSelection.Hide();
                GameManager.Instance.SetActivePuzzlePack(PuzzlePack);
                GameManager.Instance.OpenPuzzleChallengeGame("open");
            }
        }

        // void UpdatePuzzlePack(PuzzleChallengeLevel puzzleChallengeLevel) {
        //     Debug.Log("UpdatePuzzlePack: GameManager.Instance.ActivePuzzlePack.ID: " + GameManager.Instance.ActivePuzzlePack.ID);
        //     // Debug.Log("UpdatePuzzlePack: PuzzlePack.ID: " + PuzzlePack.ID);
        //     if (PuzzlePack == null) {
        //         Debug.Log("Puzzle Pack is null");
        //     }
        //     if (GameManager.Instance.ActivePuzzlePack.ID == PuzzlePack.ID) {
        //         completedAndTotalCount.text = GetCompletedCount() + " / " + PuzzlePack.PuzzleChallengeLevels.Count.ToString();
        //     }
        // }

        int GetCompletedCount() {
            int completedCount = 0;
            foreach (var level in PuzzlePack.PuzzleChallengeLevels)
            {
                if (PlayerPrefsWrapper.IsPuzzleChallengeCompleted(level.ID))
                {
                    completedCount++;
                }
            }

            return completedCount;
        }

        int GetNextLevel() {
            // Determine the next level the player should play based on the levels they have completed
            // When opening the puzzle pack the lowest level that the player has not yet completed should load
            int highestCompletedLevel = 1;
            for (int i = 0; i < PuzzlePack.PuzzleChallengeLevels.Count; i++)
            {
                Debug.Log("PuzzlePackID: " + PuzzlePack.ID + ",GetNextLevel: " + PuzzlePack.PuzzleChallengeLevels[i].ID);
                if (PlayerPrefsWrapper.IsPuzzleChallengeCompleted(PuzzlePack.PuzzleChallengeLevels[i].ID))
                {
                    highestCompletedLevel++;
                }
                else
                {
                    break;
                }
            }
            // If player completed all levels reset to 1st level
            if (highestCompletedLevel > PuzzlePack.PuzzleChallengeLevels.Count) {
                highestCompletedLevel = 1;
            }
            Debug.Log("PuzzlePackID: " + PuzzlePack.ID + " GetNextLevel: "  + highestCompletedLevel);
            return highestCompletedLevel;
        }
    }
}

