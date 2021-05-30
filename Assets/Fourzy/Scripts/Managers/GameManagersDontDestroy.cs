﻿//@vadym udod

using Fourzy._Updates;
using Fourzy._Updates.Serialized;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace Fourzy
{
    public class GameManagersDontDestroy : MonoBehaviour
    {
        private static bool activeGameManagers;

        private void Awake()
        {
            if (activeGameManagers)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                activeGameManagers = true;
                DontDestroyOnLoad(this.gameObject);
            }
        }

#if UNITY_EDITOR
        [Button]
        public void StartBotMatch()
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

        [Button]
        public void CompleteNextFTUEBoard()
        {
            if (!IsRuntime()) return;

            int boardsComplete = PlayerPrefsWrapper.GetTutorialRealtimeBotGamesPlayed();

            if (boardsComplete < GameContentManager.Instance.realtimeBotBoards.Count)
            {
                PlayerPrefsWrapper.AddTutorialRealtimeBotGamePlayed();
                Debug.Log($"FTUE boards complete {PlayerPrefsWrapper.GetTutorialRealtimeBotGamesPlayed()}");
            }
            else
            {
                Debug.Log($"All FTUE boards complete");
            }
        }

        [Button]
        public void UnlockAllTokensLocal()
        {
            if (!IsRuntime()) return;

            PlayerPrefsWrapper.AddUnlockedTokens(
                GameContentManager.Instance.tokens.Select(_tokenData => _tokenData.tokenType), 
                TokenUnlockType.AREA_PROGRESS);
        }

        [Button]
        public void AddProgressTrainingGarden()
        {
            if (!IsRuntime()) return;

            GameManager.Instance.ReportAreaProgression(Area.TRAINING_GARDEN);
        }

        [Button]
        public void AddProgressEnchantedForest()
        {
            if (!IsRuntime()) return;

            GameManager.Instance.ReportAreaProgression(Area.ENCHANTED_FOREST);
        }

        [Button]
        public void AddProgressSandyIsland()
        {
            if (!IsRuntime()) return;

            GameManager.Instance.ReportAreaProgression(Area.SANDY_ISLAND);
        }

        [Button]
        public void AddProgressIcePalace()
        {
            if (!IsRuntime()) return;

            GameManager.Instance.ReportAreaProgression(Area.ICE_PALACE);
        }

        [Button]
        public void ResetAreaProgress()
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
            },
            (error) =>
            {
                Debug.LogError(error.ErrorMessage);

                GameManager.Instance.ReportPlayFabError(error.ErrorMessage);
            });
        }

        [Button]
        public void StartFastPuzzleMatch()
        {
            if (!IsRuntime()) return;

            GameManager.Instance.StartGame(GameContentManager.Instance.GetNextFastPuzzle(), GameTypeLocal.LOCAL_GAME);
        }

        private bool IsRuntime()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Playmode only");
                return false;
            }

            return true;
        }
#endif
    }
}
