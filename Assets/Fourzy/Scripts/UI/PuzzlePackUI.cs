﻿using System.Collections;
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

        void Start () {
            Button btn = this.GetComponent<Button>();
            btn.onClick.AddListener(OpenPuzzlePack);
        }

        // private void OnEnable()
        // {
        //     GamePlayManager.OnPuzzleCompleted += UpdatePuzzlePack;
        // }

        // private void OnDisable()
        // {
        //     GamePlayManager.OnPuzzleCompleted -= UpdatePuzzlePack;
        // }

        public void InitPuzzlePack(PuzzlePack puzzlePack) {
            Debug.Log("InitPuzzlePack");
            this.PuzzlePack = puzzlePack;
            //TODO: set active level based off of levels already completed
            this.PuzzlePack.ActiveLevel = GetNextLevel();

            // Debug.Log("puzzlePack.Name: " + puzzlePack.Name);
            puzzlePackName.text = puzzlePack.Name;
            // Debug.Log("puzzlePack.PuzzleChallengeLevels.Count: " + puzzlePack.PuzzleChallengeLevels.Count);
            completedAndTotalCount.text = GetCompletedCount() + " / " + puzzlePack.PuzzleChallengeLevels.Count.ToString();
        }

        void OpenPuzzlePack() {
            ViewController.instance.viewPuzzleSelection.Hide();
            GameManager.instance.SetActivePuzzlePack(PuzzlePack);
            GameManager.instance.OpenPuzzleChallengeGame("open");
        }

        // void UpdatePuzzlePack(PuzzleChallengeLevel puzzleChallengeLevel) {
        //     Debug.Log("UpdatePuzzlePack: GameManager.instance.ActivePuzzlePack.ID: " + GameManager.instance.ActivePuzzlePack.ID);
        //     // Debug.Log("UpdatePuzzlePack: PuzzlePack.ID: " + PuzzlePack.ID);
        //     if (PuzzlePack == null) {
        //         Debug.Log("Puzzle Pack is null");
        //     }
        //     if (GameManager.instance.ActivePuzzlePack.ID == PuzzlePack.ID) {
        //         completedAndTotalCount.text = GetCompletedCount() + " / " + PuzzlePack.PuzzleChallengeLevels.Count.ToString();
        //     }
        // }

        int GetCompletedCount() {
            int completedCount = 0;
            foreach (var level in PuzzlePack.PuzzleChallengeLevels)
            {
                if (PlayerPrefs.GetInt("PuzzleChallengeID:" + level.ID) == 1) {
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
                if (PlayerPrefs.GetInt("PuzzleChallengeID:" + PuzzlePack.PuzzleChallengeLevels[i].ID) == 1) {
                    highestCompletedLevel++;
                } else {
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

