//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
using Newtonsoft.Json;
// using GameSparks.Api.Responses;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class MatchmakingScreen : MenuScreen
    {
        private const float MINIMAL_TIMER_TIME = 3f;

        public TextAsset strings;
        public TMP_Text messageLabel;
        public TMP_Text timerLabel;
        public GameObject backButton;

        private string challengeIdToJoin;
        private string challengedID = "";
        private MatchmakingScreenState state;
        private LoadingPromptScreen _prompt;
        private List<string> matchMakingStrings;
        private float roomCreatedTime;
        private bool useBotMatch;

        public AlphaTween alphaTween { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            alphaTween = timerLabel.GetComponent<AlphaTween>();

            matchMakingStrings = JsonUtility.FromJson<GameStrings>(strings.text).values;

            // ChallengeManager.OnChallengesUpdate += OnChallengesUpdate;
            // ChallengeManager.OnChallengeUpdate += OnChallengeUpdate;

            FourzyPhotonManager.onJoinRandomFailed += OnJoinRandomFailed;
            FourzyPhotonManager.onCreateRoom += OnRoomCreated;
            FourzyPhotonManager.onCreateRoomFailed += OnCreateRoomFailed;
            FourzyPhotonManager.onPlayerEnteredRoom += OnPlayerEnteredRoom;
            FourzyPhotonManager.onJoinedRoom += OnRoomJoined;
            FourzyPhotonManager.onConnectionTimeOut += OnConnectionTimeOut;
            FourzyPhotonManager.onRoomLeft += OnRoomLeft;
        }

        protected void OnDestroy()
        {
            // ChallengeManager.OnChallengesUpdate -= OnChallengesUpdate;
            // ChallengeManager.OnChallengeUpdate -= OnChallengeUpdate;

            FourzyPhotonManager.onJoinRandomFailed -= OnJoinRandomFailed;
            FourzyPhotonManager.onCreateRoom -= OnRoomCreated;
            FourzyPhotonManager.onCreateRoomFailed -= OnCreateRoomFailed;
            FourzyPhotonManager.onPlayerEnteredRoom -= OnPlayerEnteredRoom;
            FourzyPhotonManager.onJoinedRoom -= OnRoomJoined;
            FourzyPhotonManager.onConnectionTimeOut -= OnConnectionTimeOut;
            FourzyPhotonManager.onRoomLeft -= OnRoomLeft;
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

            StopRoutine("botMatch", false);
            StopRoutine("randomText", false);

            state = MatchmakingScreenState.NONE;
            timerLabel.text = string.Empty;
        }

        public override void OnBack()
        {
            if (inputBlocked) return;

            base.OnBack();

            CloseSelf();

            if (FourzyPhotonManager.IsQMLobby)
            {
                FourzyPhotonManager.TryLeaveRoom();
            }
        }

        public void OpenTurnbased()
        {
            state = MatchmakingScreenState.TURNBASED;
            challengedID = "";

            menuController.OpenScreen(this);
        }

        public void StartVSPlayer(string playerID)
        {
            state = MatchmakingScreenState.TURNBASED;
            challengedID = playerID;

            menuController.OpenScreen(this);
        }

        public void OpenRealtime()
        {
            if (FourzyPhotonManager.GetRoomProperty(Constants.REALTIME_ROOM_TYPE_KEY, RoomType.NONE) ==
                RoomType.LOBBY_ROOM)
            {
                menuController
                    .GetOrAddScreen<PromptScreen>()
                    .Prompt("Leave room?", "To enter the quickmatch you have to leave the room you'r currently in.", () =>
                    {
                        state = MatchmakingScreenState.WAITTING_ROOM_LEFT;
                        FourzyPhotonManager.TryLeaveRoom();

                        _prompt
                            .Prompt(
                            "Leaving room...",
                            "",
                            null,
                            "Back",
                            null,
                            () => state = MatchmakingScreenState.NONE)
                            .CloseOnDecline();
                    }, null)
                    .CloseOnDecline()
                    .CloseOnAccept();
            }
            else
            {
                state = MatchmakingScreenState.REALTIME;
                challengedID = "";

                menuController.OpenScreen(this);
            }
        }

        public override void Open()
        {
            base.Open();

            challengeIdToJoin = "";
            BlockInput();
            backButton.SetActive(false);

            switch (state)
            {
                case MatchmakingScreenState.REALTIME:
                    messageLabel.text = "Retrieving player stats...";

                    //first get stats
                    UserManager.GetPlayerRating(rating =>
                    {
                        messageLabel.text = "Searching for opponent...";

                        roomCreatedTime = Time.time;
                        FourzyPhotonManager.JoinRandomRoom();

                        AnalyticsManager.Instance.LogEvent(
                            "realtimeMatchmakingStarted",
                            AnalyticsManager.AnalyticsProvider.ALL,
                            new KeyValuePair<string, object>("playfabPlayerId", LoginManager.masterAccountId),
                            new KeyValuePair<string, object>("area", ((Area)PlayerPrefsWrapper.GetCurrentArea()).ToString()));
                    }, () =>
                    {
                        messageLabel.text = "Failed retrieving players' stats";

                        ReadyToClose();
                    });

                    break;

                case MatchmakingScreenState.TURNBASED:
                    messageLabel.text = "Searching for opponent...";

                    Area selectedArea = GameContentManager.Instance.currentArea.areaID;
                    Debug.Log("GameContentManager.Instance.currentTheme.themeID: " + GameContentManager.Instance.currentArea.areaID);
                    // ChallengeManager.Instance.CreateTurnBasedGame(challengedID/*"5ca27b6b4cd5b739c01cbd21"*/, selectedArea, CreateTurnBasedGameSuccess, CreateTurnBasedGameError);

                    break;
            }

            StartRoutine("randomText", ShowRandomTextRoutine());
        }

        #region Photon callbacks

        private void OnJoinRandomFailed()
        {
            if (!isOpened) return;

            Debug.Log("Failed to join random room, creating new one...");

            FourzyPhotonManager.CreateRoom(RoomType.QUICKMATCH);
        }

        private void OnRoomCreated(string roomName)
        {
            if (!isOpened) return;

            messageLabel.text = "Searching for opponent...";

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
                StartBotMatch,
                null);
        }

        private void OnCreateRoomFailed(string error)
        {
            if (!isOpened) return;

            messageLabel.text = $"Failed to create new room. {error}";

            ReadyToClose();
        }

        private void ReadyToClose()
        {
            //unblock input
            SetInteractable(true);
            backButton.SetActive(true);

            StopRoutine("randomText", false);
            timerLabel.text = string.Empty;
        }

        private void OnPlayerEnteredRoom(Photon.Realtime.Player otherPlayer)
        {
            if (!isOpened) return;

            GameManager.Instance.RealtimeOpponent = new OpponentData(otherPlayer);
            //other player connected, switch to gameplay scene
            StartMatch(GameTypeLocal.REALTIME_QUICKMATCH);

            CloseSelf();
        }

        private void OnRoomJoined(string roomName)
        {
            if (!isOpened) return;

            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                GameManager.Instance.RealtimeOpponent = new OpponentData(PhotonNetwork.PlayerListOthers[0]);

                //open gameplay scene
                StartMatch(GameTypeLocal.REALTIME_QUICKMATCH);

                CloseSelf();
            }
        }

        private void OnRoomLeft()
        {
            if (state != MatchmakingScreenState.WAITTING_ROOM_LEFT) return;

            if (_prompt.isOpened) _prompt.Decline();
            OpenRealtime();
        }

        #endregion

        private void StartMatch(GameTypeLocal type)
        {
            //play GAME_FOUND sfx
            AudioHolder.instance.PlaySelfSfxOneShotTracked(Serialized.AudioTypes.GAME_FOUND);
            GameManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.Success);
            timerLabel.text = "Match Found";

            GameManager.Instance.StartGame(type);
        }

        private void StartBotMatch()
        {
            OnBack();

            if (useBotMatch)
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

                    Debug.Log("starting ai match for " + botSettings.AIProfile);

                    GameManager.Instance.RealtimeOpponent =
                        new OpponentData(
                            "bot",
                            botSettings.r,
                            InternalSettings.Current.GAMES_BEFORE_RATING_USED);
                    GameManager.Instance.Bot = new Player(
                        2,
                        UserManager.CreateNewPlayerName(),
                        botSettings.AIProfile)
                    {
                        HerdId = GameContentManager.Instance.piecesDataHolder.random.data.ID,
                        PlayerString = "2",
                    };

                    StartMatch(GameTypeLocal.REALTIME_BOT_GAME);
                },
                (error) =>
                {
                    Debug.LogError(error.ErrorMessage);

                    GameManager.Instance.ReportPlayFabError(error.ErrorMessage);
                });
            }
            else
            {
                //timed out
                PersistantMenuController.Instance.GetOrAddScreen<PromptScreen>().Prompt(
                    LocalizationManager.Value("timed_out_title"),
                    LocalizationManager.Value("timed_out_text"), null,
                    LocalizationManager.Value("back"), null, null);
            }
        }

        private void OnConnectionTimeOut()
        {
            //unblock input
            SetInteractable(true);
            backButton.SetActive(true);

            OnBack();

            //open prompt screen
            PersistantMenuController.Instance.GetOrAddScreen<PromptScreen>().Prompt(
                "Connection failed.",
                "Failed to connect to multiplayer server :(", null,
                LocalizationManager.Value("back"), null, null);
        }

        private IEnumerator ShowRandomTextRoutine()
        {
            const float timeToShow = 3.5f;

            int elementIndex = 0;
            while (true)
            {
                alphaTween.PlayForward(true);

                timerLabel.text = matchMakingStrings[elementIndex].RemoveLastChar();

                yield return new WaitForSeconds(timeToShow - alphaTween.playbackTime);

                alphaTween.PlayBackward(true);

                yield return new WaitForSeconds(alphaTween.playbackTime);

                if (elementIndex + 1 < matchMakingStrings.Count)
                    elementIndex++;
                else
                    elementIndex = 0;
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
