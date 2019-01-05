//modded @vadym udod

using System;
using UnityEngine;

namespace Fourzy
{
    public static class PlayerPrefsWrapper
    {
        public static Action<int> onThemeChanged;

        static string kInstructionPopupDisplayed = "kInstructionPopupDisplayed_";
        static string kPuzzleChallengeCompleted = "PuzzleChallengeID:";
        static string kCurrentTheme = "kCurrentTheme";

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

        public static void SetCurrentGameTheme(int currentTheme)
        {
            PlayerPrefs.SetInt(kCurrentTheme, currentTheme);

            if (onThemeChanged != null)
                onThemeChanged.Invoke(currentTheme);
        }

        public static int GetCurrentTheme()
        {
            return PlayerPrefs.GetInt(kCurrentTheme, 0);
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }
    }
}


