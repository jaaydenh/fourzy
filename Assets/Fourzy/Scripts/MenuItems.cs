//@vadym udod

#if UNITY_EDITOR

using System;
using Fourzy;
using Fourzy._Updates;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEditor;
using UnityEngine;

public class MenuItems : MonoBehaviour
{
    [UnityEditor.MenuItem("Fourzy/Take screenshot")]
    static void Screenshot()
    {
        ScreenCapture.CaptureScreenshot("../" + Guid.NewGuid() + ".png");
    }

    [MenuItem("Fourzy/Reset Player")]
    public static void ResetPlayer()
    {
        DeleteAllPlayerPrefs();
        ClearInventory();
        ClearAreaProgress();
        ResetPlayerStats();
    }

    [MenuItem("Fourzy/DeleteAllPlayerPrefs")]
    public static void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Fourzy/Clear Inventory")]
    public static void ClearInventory()
    {
        if (!IsRuntime()) return;

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "resetPlayerInventory",
            GeneratePlayStreamEvent = true,
        },
        (value) =>
        {
            PlayerPrefsWrapper.RemoveAllButDefault();
            GameContentManager.Instance.piecesDataHolder.ResetPieces();
            GameContentManager.Instance.bundlesInPlayerInventory.Clear();
            UserManager.Instance.UnlockToken(FourzyGameModel.Model.TokenType.NONE, Fourzy._Updates.Serialized.TokenUnlockType.AREA_PROGRESS);

            Debug.Log("Inventory cleared");
        },
        null);
    }

    [MenuItem("Fourzy/Clear Area Progress")]
    public static void ClearAreaProgress()
    {
        if (!IsRuntime()) return;

        Debug.Log($"Reset games counter on all areas for player.");

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "resetAreaProgression",
            GeneratePlayStreamEvent = true,
        },
        result =>
        {
            UserManager.Instance.SetAreaProgression(Area.TRAINING_GARDEN, 0);
            UserManager.Instance.SetAreaProgression(Area.ENCHANTED_FOREST, 0);
            UserManager.Instance.SetAreaProgression(Area.SANDY_ISLAND, 0);
            UserManager.Instance.SetAreaProgression(Area.ICE_PALACE, 0);

            Debug.Log("Areas progress reset");
        },
        (error) =>
        {
            Debug.LogError(error.ErrorMessage);

            GameManager.Instance.ReportPlayFabError(error.ErrorMessage);
        });
    }

    [MenuItem("Fourzy/Reset Player Stats")]
    public static void ResetPlayerStats()
    {
        if (!IsRuntime()) return;

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "resetPlayerStats",
            GeneratePlayStreamEvent = true,
        },
       result =>
       {
           UserManager.GetMyStats();

           Debug.Log("Player Stats cleared");
       },
       (error) =>
       {
           Debug.LogError(error.ErrorMessage);

           GameManager.Instance.ReportPlayFabError(error.ErrorMessage);
       });
    }

    [MenuItem("Fourzy/Start Bot Match(from MainMenu)")]
    public static void StartBotMatch()
    {
        if (!IsRuntime()) return;

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "botDataFromRating",
            GeneratePlayStreamEvent = true,
        },
        (result) =>
        {
            BotSettings botSettings =
                JsonConvert.DeserializeObject<BotSettings>(result.FunctionResult.ToString());

            GameManager.Instance.RealtimeOpponent =
                new OpponentData(
                    "bot",
                    botSettings.r,
                    InternalSettings.Current.GAMES_BEFORE_RATING_USED);
            GameManager.Instance.Bot = new Player(
                2,
                CharacterNameFactory.GenerateBotName(AIDifficulty.Medium),
                botSettings.AIProfile)
            {
                HerdId = GameContentManager.Instance.piecesDataHolder.random.Id,
                PlayerString = "2",
            };

            GameManager.Instance.StartGame(GameTypeLocal.REALTIME_BOT_GAME);

            if (FourzyPhotonManager.IsQMLobby)
            {
                FourzyPhotonManager.TryLeaveRoom();
            }
        },
        (error) =>
        {
            GameManager.Instance.ReportPlayFabError(error.ErrorMessage);

            Debug.LogError(error.ErrorMessage);
        });
    }

    [MenuItem("Fourzy/Add Realtime Game Complete")]
    public static void AddRealtimeGameComplete()
    {
        if (!IsRuntime()) return;

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "addRealtimeGameComplete",
            GeneratePlayStreamEvent = true,
        },
        (value) => {
            UserManager.Instance.realtimeGamesComplete += 1;
        },
        null);
    }

    [MenuItem("Fourzy/Add Progress Training Garden")]
    public static void AddProgressTrainingGarden()
    {
        if (!IsRuntime()) return;

        GameManager.Instance.ReportAreaProgression(Area.TRAINING_GARDEN);
    }

    [MenuItem("Fourzy/Add Progress Enchanted Forest")]
    public static void AddProgressEnchantedForest()
    {
        if (!IsRuntime()) return;

        GameManager.Instance.ReportAreaProgression(Area.ENCHANTED_FOREST);
    }

    [MenuItem("Fourzy/Add Progress Sandy Island")]
    public static void AddProgressSandyIsland()
    {
        if (!IsRuntime()) return;

        GameManager.Instance.ReportAreaProgression(Area.SANDY_ISLAND);
    }

    [MenuItem("Fourzy/Add Progress Ice Palace")]
    public static void AddProgressIcePalace()
    {
        if (!IsRuntime()) return;

        GameManager.Instance.ReportAreaProgression(Area.ICE_PALACE);
    }

    [MenuItem("Fourzy/Place Player1 Piece [P]")]
    public static void PlacePlayer1Piece()
    {
        Debug.Log("Hover over cell you want to place gamepiece at and press [P] on your keyboard");
    }

    [MenuItem("Fourzy/Place Player2 Piece [O]")]
    public static void PlacePlayer2Piece()
    {
        Debug.Log("Hover over cell you want to place gamepiece at and press [O] on your keyboard");
    }

    private static bool IsRuntime()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Playmode only");
            return false;
        }

        return true;
    }
}

#endif
