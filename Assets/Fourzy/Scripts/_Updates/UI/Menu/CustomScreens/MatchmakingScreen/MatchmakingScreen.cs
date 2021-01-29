//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
// using GameSparks.Api.Responses;
using Photon.Pun;
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

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _prompt = PersistantMenuController.instance
                .GetOrAddScreen<LoadingPromptScreen>()
                .SetType(LoadingPromptScreen.LoadingPromptType.BASIC);
        }

        public override void Close(bool animate = true)
        {
            base.Close(animate);

            StopRoutine("multiplayerTimeout", false);
            StopRoutine("randomText", false);

            state = MatchmakingScreenState.NONE;
            timerLabel.text = string.Empty;
        }

        public override void OnBack()
        {
            if (inputBlocked) return;

            base.OnBack();

            CloseSelf();

            if (FourzyPhotonManager.IsQMLobby) FourzyPhotonManager.TryLeaveRoom();
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

                        FourzyPhotonManager.JoinRandomRoom();
                    }, () =>
                    {
                        messageLabel.text = "Failed retrieving players' stats";

                        ReadyToClose();
                    });

                    break;

                case MatchmakingScreenState.TURNBASED:
                    messageLabel.text = "Searching for opponent...";

                    Area selectedArea = GameContentManager.Instance.currentTheme.themeID;
                    Debug.Log("GameContentManager.Instance.currentTheme.themeID: " + GameContentManager.Instance.currentTheme.themeID);
                    // ChallengeManager.Instance.CreateTurnBasedGame(challengedID/*"5ca27b6b4cd5b739c01cbd21"*/, selectedArea, CreateTurnBasedGameSuccess, CreateTurnBasedGameError);

                    break;
            }

            StartRoutine("randomText", ShowRandomTextRoutine());
        }

        // private void OnChallengesUpdate(List<ChallengeData> challenges) => OnChallengeUpdate(challenges.Find(_challenge => _challenge.challengeInstanceId == challengeIdToJoin));

        // private void OnChallengeUpdate(ChallengeData challenge)
        // {
        //     if (!isOpened || challenge == null || challenge.challengeInstanceId != challengeIdToJoin)
        //         return;

        //     Debug.Log($"Game starts {challenge.challengeInstanceId}");
        //     challengeIdToJoin = "";

        //     //cancel extra check
        //     CancelRoutine("newChallengeExtraCheck");

        //     //play GAME_FOUND sfx
        //     AudioHolder.instance.PlaySelfSfxOneShotTracked(Serialized.AudioTypes.GAME_FOUND);
        //     GameManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.Success);
        //     timerLabel.text = "Match Found";

        //     StartRoutine("openGame", 1.5f, () =>
        //     {
        //         menuController.CloseCurrentScreen();
        //         GameManager.Instance.StartGame(challenge.lastTurnGame);
        //     });
        // }

        // private void CreateTurnBasedGameSuccess(LogEventResponse response)
        // {
        //     challengeIdToJoin = response.ScriptData.GetString("challengeInstanceId");
        //     Debug.Log($"Game created {challengeIdToJoin}");

        //     //start routine to check for this challenge in 5 seconds if OnChallengeStarted wasnt triggered on ChallengeManager
        //     StartRoutine("newChallengeExtraCheck", 5f, () => ChallengeManager.Instance.GetChallengeRequest(challengeIdToJoin), null);
        // }

        // private void CreateTurnBasedGameError(LogEventResponse response)
        // {
        //     if (!isOpened) return;

        //     Debug.Log("***** Error Creating Turn based game: " + response.Errors.JSON);
        //     AnalyticsManager.Instance.LogError(response.Errors.JSON, AnalyticsManager.AnalyticsErrorType.create_turn_base_game);

        //     messageLabel.text = "Failed to create new game...";
        //     timerLabel.text = response.Errors.JSON;

        //     //unblock input
        //     SetInteractable(true);
        //     backButton.SetActive(true);

        //     StopRoutine("randomText", false);
        // }

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

            //start timeout timer
            StartRoutine("multiplayerTimeout", Constants.REALTIME_OPPONENT_WAIT_TIME, OnMultiplayerTimerTimedOut, null);
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

            GameManager.Instance.currentOpponent = otherPlayer.UserId;
            //other player connected, switch to gameplay scene
            StartMatch();

            CloseSelf();
        }

        private void OnRoomJoined(string roomName)
        {
            if (!isOpened) return;

            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                GameManager.Instance.currentOpponent = PhotonNetwork.PlayerListOthers[0].UserId;

                //open gameplay scene
                StartMatch();

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

        private void StartMatch()
        {
            //play GAME_FOUND sfx
            AudioHolder.instance.PlaySelfSfxOneShotTracked(Serialized.AudioTypes.GAME_FOUND);
            GameManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.Success);
            timerLabel.text = "Match Found";

            GameManager.Instance.StartGame(GameTypeLocal.REALTIME_QUICKMATCH);
        }

        private void OnMultiplayerTimerTimedOut()
        {
            //leave room and close screen
            PhotonNetwork.LeaveRoom();

            OnBack();

            //open prompt screen
            PersistantMenuController.instance.GetOrAddScreen<PromptScreen>().Prompt(
                LocalizationManager.Value("timed_out_title"),
                LocalizationManager.Value("timed_out_text"), null,
                LocalizationManager.Value("back"), null, null);
        }

        private void OnConnectionTimeOut()
        {
            //unblock input
            SetInteractable(true);
            backButton.SetActive(true);

            OnBack();

            //open prompt screen
            PersistantMenuController.instance.GetOrAddScreen<PromptScreen>().Prompt(
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
