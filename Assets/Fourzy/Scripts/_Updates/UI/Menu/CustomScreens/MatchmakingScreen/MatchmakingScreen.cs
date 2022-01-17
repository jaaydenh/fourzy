//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

#if !MOBILE_SKILLZ
using Photon.Pun;
#endif

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class MatchmakingScreen : MenuScreen
    {
        public TextAsset strings;
        public TMP_Text messageLabel;
        public TMP_Text timerLabel;
        public GameObject backButton;

        private MatchmakingScreenState state;
        private LoadingPromptScreen _prompt;
        private List<string> matchMakingStrings;
        private float roomCreatedTime;
        private bool useBotMatch;

        public AlphaTween alphaTween { get; private set; }
        private bool IsOpened => isOpened && MenuController.activeMenu == menuController;

        protected override void Awake()
        {
            base.Awake();

            alphaTween = timerLabel.GetComponent<AlphaTween>();

            matchMakingStrings = JsonUtility.FromJson<GameStrings>(strings.text).values;

#if !MOBILE_SKILLZ
            FourzyPhotonManager.onJoinRandomFailed += OnJoinRandomFailed;
            FourzyPhotonManager.onCreateRoom += OnRoomCreated;
            FourzyPhotonManager.onCreateRoomFailed += OnCreateRoomFailed;
            FourzyPhotonManager.onPlayerEnteredRoom += OnPlayerEnteredRoom;
            FourzyPhotonManager.onJoinedRoom += OnRoomJoined;
            FourzyPhotonManager.onConnectionTimeOut += OnConnectionTimeOut;
            FourzyPhotonManager.onRoomLeft += OnRoomLeft;
#endif
        }

        protected void OnDestroy()
        {
#if !MOBILE_SKILLZ
            FourzyPhotonManager.onJoinRandomFailed -= OnJoinRandomFailed;
            FourzyPhotonManager.onCreateRoom -= OnRoomCreated;
            FourzyPhotonManager.onCreateRoomFailed -= OnCreateRoomFailed;
            FourzyPhotonManager.onPlayerEnteredRoom -= OnPlayerEnteredRoom;
            FourzyPhotonManager.onJoinedRoom -= OnRoomJoined;
            FourzyPhotonManager.onConnectionTimeOut -= OnConnectionTimeOut;
            FourzyPhotonManager.onRoomLeft -= OnRoomLeft;
#endif
        }

        public void BackButtonPressed()
        {
            OnBack();

            AnalyticsManager.Instance.LogEvent(
                "realtimeMatchmakingAbandoned",
                AnalyticsManager.AnalyticsProvider.ALL,
                new KeyValuePair<string, object>("playfabPlayerId", LoginManager.masterAccountId),
                new KeyValuePair<string, object>("area", ((Area)PlayerPrefsWrapper.GetCurrentArea()).ToString()),
                new KeyValuePair<string, object>("matchmakingTimeElapsed", Time.time - roomCreatedTime)
                );
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _prompt = PersistantMenuController.Instance
                .GetOrAddScreen<LoadingPromptScreen>()
                .SetType(LoadingPromptScreen.LoadingPromptType.BASIC);
        }

        public override void Close(bool animate = true)
        {
            base.Close(animate);

            CancelRoutine("botMatch");
            CancelRoutine("randomText");

            state = MatchmakingScreenState.NONE;
            timerLabel.text = string.Empty;

            FourzyPhotonManager.TryLeaveRoom();
        }

        public override void OnBack()
        {
            if (inputBlocked) return;

            base.OnBack();

            CloseSelf();
        }

        public void OpenRealtime()
        {
            state = MatchmakingScreenState.REALTIME;
            menuController.OpenScreen(this);
        }

        public override void Open()
        {
            base.Open();

            BlockInput();
            backButton.SetActive(false);

            switch (state)
            {
                case MatchmakingScreenState.REALTIME:
                    if (!FourzyPhotonManager.ConnectedAndReady && !GameManager.NetworkAccess)
                    {
                        ReadyToClose();
                        timerLabel.text = LocalizationManager.Value("no_connection");

                        return;
                    }

                    messageLabel.text = "Retrieving player stats...";

                    //first get stats
                    UserManager.GetPlayerRating(rating =>
                    {
                        messageLabel.text = LocalizationManager.Value("searching_for_opponent");

                        roomCreatedTime = Time.time;

                        AnalyticsManager.Instance.LogEvent(
                            "realtimeMatchmakingStarted",
                            AnalyticsManager.AnalyticsProvider.ALL,
                            new KeyValuePair<string, object>("playfabPlayerId", LoginManager.masterAccountId),
                            new KeyValuePair<string, object>("area", ((Area)PlayerPrefsWrapper.GetCurrentArea()).ToString()));

                        bool _needRoom = true;
                        //check for tutorial board
                        ResourceItem botGameBoardFile = GameContentManager.Instance.GetRealtimeBotBoard(UserManager.Instance.realtimeGamesComplete);
                        if (botGameBoardFile != null)
                        {
                            FTUEGameBoardDefinition _board = JsonConvert.DeserializeObject<FTUEGameBoardDefinition>(
                                botGameBoardFile.Load<TextAsset>().text);

                            if (_board.aiProfile >= 0)
                            {
                                _needRoom = false;
                                StartBotMatch();
                            }
                        }

                        if (_needRoom)
                        {
                            FourzyPhotonManager.JoinRandomRoom();
                        }
                    }, () =>
                    {
                        messageLabel.text = "Failed retrieving players' stats";

                        ReadyToClose();
                    });

                    break;
            }

            StartRoutine("randomText", ShowRandomTextRoutine(3.5f));
        }

        internal void Timeout()
        {
            if (useBotMatch)
            {
                StartBotMatch();
            }
            else
            {
                CloseSelf();

                //timed out
                PersistantMenuController.Instance.GetOrAddScreen<PromptScreen>().Prompt(
                    LocalizationManager.Value("timed_out_title"),
                    LocalizationManager.Value("timed_out_text"),
                    null,
                    LocalizationManager.Value("back"),
                    null,
                    null);
            }
        }

        internal void StartBotMatch()
        {
            //get bot profile from players' rating
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

                StartMatch(GameTypeLocal.REALTIME_BOT_GAME);

                FourzyPhotonManager.TryLeaveRoom();
            },
            (error) =>
            {
                CloseSelf();
                GameManager.Instance.ReportPlayFabError(error.ErrorMessage);

                Debug.LogError(error.ErrorMessage);
            });
        }

#if !MOBILE_SKILLZ
        private void OnJoinRandomFailed()
        {
            if (!IsOpened) return;

            Debug.Log("Failed to join random room, creating new one...");
            FourzyPhotonManager.CreateRoom(RoomType.QUICKMATCH);
        }

        private void OnRoomCreated(string roomName)
        {
            if (!IsOpened) return;

            messageLabel.text = LocalizationManager.Value("searching_for_opponent");

            //unblock input
            SetInteractable(true);
            backButton.SetActive(true);

            useBotMatch = InternalSettings.Current.BOT_SETTINGS.randomMatchAfter > 0f;

            //start timeout timer
            StartRoutine(
                "botMatch",
                useBotMatch ?
                    InternalSettings.Current.BOT_SETTINGS.randomMatchAfter :
                    Constants.REALTIME_OPPONENT_WAIT_TIME,
                Timeout,
                null);
        }

        private void OnCreateRoomFailed(string error)
        {
            if (!IsOpened) return;

            messageLabel.text = $"Failed to create new room. {error}";

            ReadyToClose();
        }

        private void OnPlayerEnteredRoom(Photon.Realtime.Player otherPlayer)
        {
            if (!IsOpened) return;

            GameManager.Instance.RealtimeOpponent = new OpponentData(otherPlayer);
            //other player connected, open to gameplay scene
            StartMatch(GameTypeLocal.REALTIME_QUICKMATCH);
        }

        private void OnRoomJoined(string roomName)
        {
            if (!IsOpened) return;

            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                GameManager.Instance.RealtimeOpponent = new OpponentData(PhotonNetwork.PlayerListOthers[0]);
                //open gameplay scene
                StartMatch(GameTypeLocal.REALTIME_QUICKMATCH);
            }
        }

        private void OnRoomLeft()
        {
            if (state != MatchmakingScreenState.WAITTING_ROOM_LEFT) return;

            if (_prompt.isOpened)
            {
                _prompt.Decline();
            }

            OpenRealtime();
        }
#endif

        private void ReadyToClose()
        {
            //unblock input
            SetInteractable(true);
            backButton.SetActive(true);

            StopRoutine("randomText", false);
            timerLabel.text = string.Empty;
        }

        private void StartMatch(GameTypeLocal type)
        {
            state = MatchmakingScreenState.NONE;
            AudioHolder.instance.PlaySelfSfxOneShotTracked("game_found");
            GameManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.Success);
            timerLabel.text = "Match Found";

            GameManager.Instance.StartGame(type);
        }

        private void OnConnectionTimeOut()
        {
            if (!IsOpened) return;

            //unblock input
            SetInteractable(true);
            backButton.SetActive(true);

            CloseSelf();

            //open prompt screen
            PersistantMenuController.Instance.GetOrAddScreen<PromptScreen>().Prompt(
                LocalizationManager.Value("no_connection"),
                "",
                null,
                LocalizationManager.Value("back"),
                null,
                null);
        }

        private IEnumerator ShowRandomTextRoutine(float timeToShow = 3.5f)
        {
            int elementIndex = 0;
            while (true)
            {
                if (!isOpened) yield return null;

                alphaTween.PlayForward(true);

                timerLabel.text = matchMakingStrings[elementIndex].RemoveLastChar();

                yield return new WaitForSeconds(timeToShow - alphaTween.playbackTime);

                alphaTween.PlayBackward(true);

                yield return new WaitForSeconds(alphaTween.playbackTime);

                if (elementIndex + 1 < matchMakingStrings.Count)
                {
                    elementIndex++;
                }
                else
                {
                    elementIndex = 0;
                }
            }
        }

        private enum MatchmakingScreenState
        {
            NONE,
            REALTIME,
            TURNBASED,
            WAITTING_ROOM_LEFT,
        }
    }
}
