//@vadym udod

using Fourzy._Updates;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.OdinInspector;
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
            if (!Application.isPlaying)
            {
                Debug.LogError("Playmode only");
                return;
            }

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
#endif
    }
}
