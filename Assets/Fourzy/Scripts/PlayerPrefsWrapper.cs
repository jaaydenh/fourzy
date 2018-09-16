using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    public static class PlayerPrefsWrapper
    {
        static string kInstructionPopupDisplayed = "kInstructionPopupDisplayed_";
        static string kPuzzleChallengeCompleted = "PuzzleChallengeID:";

        public static bool InstructionPopupWasDisplayed(int tokenId)
        {
            int defaultValue = 0;
            return PlayerPrefs.GetInt(kInstructionPopupDisplayed + tokenId, defaultValue) != defaultValue;
        }

        public static void SetInstructionPopupDisplayed(int tokenId, bool isDisplayed)
        {
            int value = isDisplayed ? 1 : 0;
            PlayerPrefs.SetInt(kInstructionPopupDisplayed + tokenId, value);
        }

        public static bool IsPuzzleChallengeCompleted(string id)
        {
            int defaultValue = 0;
            return PlayerPrefs.GetInt(kPuzzleChallengeCompleted + id, defaultValue) != defaultValue;
        }

        public static void SetPuzzleChallengeCompleted(string id, bool completed)
        {
            int value = completed ? 1 : 0;
            PlayerPrefs.SetInt(kPuzzleChallengeCompleted + id, value);
        }
    }
}


