using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    public static class PlayerPrefsWrapper
    {
        static string kInstructionPopupDisplayed = "kInstructionPopupDisplayed_";

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
    }
}


