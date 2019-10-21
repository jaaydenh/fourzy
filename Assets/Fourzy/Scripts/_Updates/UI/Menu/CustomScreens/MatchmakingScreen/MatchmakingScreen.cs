//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
using GameSparks.Api.Responses;
using System;
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
        private List<string> matchMakingStrings;

        public bool isRealtime { get; set; }
        public AlphaTween alphaTween { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            alphaTween = timerLabel.GetComponent<AlphaTween>();

            matchMakingStrings = JsonUtility.FromJson<GameStrings>(strings.text).values;

            ChallengeManager.OnChallengesUpdate += OnChallengesUpdate;
            ChallengeManager.OnChallengeUpdate += OnChallengeUpdate;

            FourzyPhotonManager.onJoinRandomRoomFailed += OnJoinRandomRoomFailed;
            FourzyPhotonManager.onRoomCreated += OnRoomCreated;
            FourzyPhotonManager.onCreateRoomFailed += OnCreateRoomFailed;
            FourzyPhotonManager.onPlayerConnected += OnPlayerConnected;
            FourzyPhotonManager.onRoomJoined += OnRoomJoined;
        }

        protected void OnDestroy()
        {
            ChallengeManager.OnChallengesUpdate -= OnChallengesUpdate;
            ChallengeManager.OnChallengeUpdate -= OnChallengeUpdate;

            FourzyPhotonManager.onJoinRandomRoomFailed -= OnJoinRandomRoomFailed;
            FourzyPhotonManager.onRoomCreated -= OnRoomCreated;
            FourzyPhotonManager.onCreateRoomFailed -= OnCreateRoomFailed;
            FourzyPhotonManager.onPlayerConnected -= OnPlayerConnected;
            FourzyPhotonManager.onRoomJoined -= OnRoomJoined;
        }

        public override void Close(bool animate = true)
        {
            base.Close(animate);

            if (isRealtime) PhotonNetwork.LeaveRoom();

            StopRoutine("randomText", false);
            timerLabel.text = string.Empty;
        }

        public override void OnBack()
        {
            if (inputBlocked) return;

            base.OnBack();

            menuController.CloseCurrentScreen();
        }

        public void OpenTurnbased()
        {
            isRealtime = false;
            challengedID = "";

            menuController.OpenScreen(this);
        }

        public void StartVSPlayer(string playerID)
        {
            isRealtime = false;
            challengedID = playerID;

            menuController.OpenScreen(this);
        }

        public void OpenRealtime()
        {
            isRealtime = true;
            challengedID = "";

            menuController.OpenScreen(this);
        }

        public override void Open()
        {
            base.Open();

            challengeIdToJoin = "";
            BlockInput();
            backButton.SetActive(false);

            if (isRealtime)
            {
                messageLabel.text = "Finding Match";

                //try join random room
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                messageLabel.text = "Finding Match";

                Area selectedArea = GameContentManager.Instance.currentTheme.themeID;
                Debug.Log("GameContentManager.Instance.currentTheme.themeID: " + GameContentManager.Instance.currentTheme.themeID);
                ChallengeManager.Instance.CreateTurnBasedGame(challengedID/*"5ca27b6b4cd5b739c01cbd21"*/, selectedArea, CreateTurnBasedGameSuccess, CreateTurnBasedGameError);
            }

            StartRoutine("randomText", ShowRandomTextRoutine());
        }

        private void OnChallengesUpdate(List<ChallengeData> challenges) => OnChallengeUpdate(challenges.Find(_challenge => _challenge.challengeInstanceId == challengeIdToJoin));

        private void OnChallengeUpdate(ChallengeData challenge)
        {
            if (!isOpened || challenge == null || challenge.challengeInstanceId != challengeIdToJoin)
                return;

            Debug.Log($"Game starts {challenge.challengeInstanceId}");
            challengeIdToJoin = "";

            //cancel extra check
            CancelRoutine("newChallengeExtraCheck");

            //play GAME_FOUND sfx
            AudioHolder.instance.PlaySelfSfxOneShotTracked(Serialized.AudioTypes.GAME_FOUND);
            GameManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.Success);
            timerLabel.text = "Match Found";

            StartRoutine("openGame", 1.5f, () =>
            {
                menuController.CloseCurrentScreen();
                GameManager.Instance.StartGame(challenge.lastTurnGame);
            });
        }

        private void CreateTurnBasedGameSuccess(LogEventResponse response)
        {
            challengeIdToJoin = response.ScriptData.GetString("challengeInstanceId");
            Debug.Log($"Game created {challengeIdToJoin}");

            //start routine to check for this challenge in 5 seconds if OnChallengeStarted wasnt triggered on ChallengeManager
            StartRoutine("newChallengeExtraCheck", 5f, () => ChallengeManager.Instance.GetChallengeRequest(challengeIdToJoin), null);
        }

        private void CreateTurnBasedGameError(LogEventResponse response)
        {
            if (!isOpened) return;

            Debug.Log("***** Error Creating Turn based game: " + response.Errors.JSON);
            AnalyticsManager.Instance.LogError(response.Errors.JSON, AnalyticsManager.AnalyticsErrorType.create_turn_base_game);

            messageLabel.text = "Failed to create new game...";
            timerLabel.text = response.Errors.JSON;

            //unblock input
            SetInteractable(true);
            backButton.SetActive(true);

            StopRoutine("randomText", false);
        }

        #region Photon callbacks

        private void OnJoinRandomRoomFailed()
        {
            if (!isOpened) return;

            messageLabel.text = "Failed to join random room, creating new one...";
            Debug.Log("Failed to join random room, creating new one...");

            //create new room
            PhotonNetwork.CreateRoom(
                Guid.NewGuid().ToString(),
                new RoomOptions()
                {
                    MaxPlayers = 2,
                },
                null);
        }

        private void OnRoomCreated(string roomName)
        {
            if (!isOpened) return;

            messageLabel.text = "Waiting for other player to join...";

            //unblock input
            SetInteractable(true);
            backButton.SetActive(true);

            StopRoutine("randomText", false);
            timerLabel.text = string.Empty;
        }

        private void OnCreateRoomFailed()
        {
            if (!isOpened) return;

            messageLabel.text = "Failed to create new room.";

            //unblock input
            SetInteractable(true);
            backButton.SetActive(true);

            StopRoutine("randomText", false);
            timerLabel.text = string.Empty;
        }

        private void OnPlayerConnected(PhotonPlayer otherPlayer)
        {
            ////fake scene loading
            //StartRoutine("fakeSceneLoading", 4f, () => GameManager.Instance.StartGame());
            //other player connected, switch to gameplay scene
            GameManager.Instance.StartGame();
        }

        private void OnRoomJoined(string roomName)
        {
            if (PhotonNetwork.room.PlayerCount == 2)
            {
                ////fake scene loading
                //StartRoutine("fakeSceneLoading", 4f, () => GameManager.Instance.StartGame());
                //open gameplay scene
                GameManager.Instance.StartGame();
            }
        }

        #endregion

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
    }
}
