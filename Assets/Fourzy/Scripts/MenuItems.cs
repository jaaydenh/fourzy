using UnityEngine;

public class MenuItems : MonoBehaviour {


#if UNITY_EDITOR
    [UnityEditor.MenuItem("Fourzy/Take screenshot")]
    static void Screenshot()
    {
        ScreenCapture.CaptureScreenshot("../test.png");
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel0")]
    static void SetPuzzleToLevel1()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 0);
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel5")]
    static void SetPuzzleToLevel5()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 5);
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel10")]
    static void SetPuzzleToLevel10()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 10);
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel15")]
    static void SetPuzzleToLevel15()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 15);
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel20")]
    static void SetPuzzleToLevel20()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 20);
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel25")]
    static void SetPuzzleToLevel25()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 25);
    }

    [UnityEditor.MenuItem("Fourzy/SetPuzzleToLevel30")]
    static void SetPuzzleToLevel30()
    {
        PlayerPrefs.SetInt("puzzleChallengeLevel", 30);
    }
#endif

}
