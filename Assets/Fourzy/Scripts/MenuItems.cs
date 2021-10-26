﻿//@vadym udod

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
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

    [MenuItem("Fourzy/Set For Mobile Regular")]
    public static void SetMobileRegular()
    {
        SetBuildIntent(BuildIntent.MOBILE_REGULAR);
    }

    [MenuItem("Fourzy/Set For Mobile Skillz")]
    public static void SetMobileSkillz()
    {
        SetBuildIntent(BuildIntent.MOBILE_SKILLZ);
    }

    [MenuItem("Fourzy/Set For Desktop")]
    public static void SetMobileDesktop()
    {
        SetBuildIntent(BuildIntent.DESKTOP_REGULAR);
    }

    private static void SetBuildIntent(BuildIntent intent)
    {
        GameManagersDontDestroy managers = AssetDatabase.LoadAssetAtPath<GameManagersDontDestroy>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("GameManagers")[0]));

        if (managers)
        {
            GameManager gameManager = managers.GetComponentInChildren<GameManager>();

            if (gameManager)
            {
                SerializedObject so = new SerializedObject(gameManager);
                so.FindProperty("buildIntent").enumValueIndex = (int)intent;
                so.ApplyModifiedProperties();

                SetScenesFromIntent(intent);
                AssetDatabase.SaveAssets();
            }
        }
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

            FourzyPhotonManager.TryLeaveRoom();
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

    public static void SetScenesFromIntent(BuildIntent intent)
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();

        //fix scenes
        switch (intent)
        {
            case BuildIntent.DESKTOP_REGULAR:
                scenes.Add(new EditorBuildSettingsScene(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(Fourzy.Constants.LOGO_SCENE_NAME)[0]), true));
                scenes.Add(new EditorBuildSettingsScene(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(Fourzy.Constants.MAIN_MENU_L_SCENE_NAME)[0]), true));
                scenes.Add(new EditorBuildSettingsScene(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(Fourzy.Constants.GAMEPLAY_SCENE_NAME)[0]), true));

                break;

            case BuildIntent.MOBILE_REGULAR:
                scenes.Add(new EditorBuildSettingsScene(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(Fourzy.Constants.LOGO_SCENE_NAME)[0]), true));
                scenes.Add(new EditorBuildSettingsScene(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(Fourzy.Constants.MAIN_MENU_P_SCENE_NAME)[0]), true));
                scenes.Add(new EditorBuildSettingsScene(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(Fourzy.Constants.GAMEPLAY_SCENE_NAME)[0]), true));

                break;

            case BuildIntent.MOBILE_SKILLZ:
                scenes.Add(new EditorBuildSettingsScene(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(Fourzy.Constants.LOGO_SKILLZ_SCENE_NAME)[0]), true));
                scenes.Add(new EditorBuildSettingsScene(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(Fourzy.Constants.MAIN_MENU_SKILLZ_SCENE_NAME)[0]), true));
                scenes.Add(new EditorBuildSettingsScene(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(Fourzy.Constants.GAMEPLAY_SCENE_NAME)[0]), true));

                break;
        }

        EditorBuildSettings.scenes = scenes.ToArray();
    }
}

#endif
