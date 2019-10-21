//@vadym udod

using Fourzy;
using Fourzy._Updates.Serialized;
using FourzyGameModel.Model;
using UnityEngine;

public class MenuItems : MonoBehaviour
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Fourzy/Take screenshot")]
    static void Screenshot()
    {
        ScreenCapture.CaptureScreenshot("../test.png");
    }

    [UnityEditor.MenuItem("Fourzy/Clear Gamepieces PlayerPrefs")]
    public static void ClearGamepiecesPlayerPrefs()
    {
        GamePiecesDataHolder.ClearGamepiecesData();
    }

    [UnityEditor.MenuItem("Fourzy/DeleteAllPlayerPrefs")]
    static void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
#endif
}
