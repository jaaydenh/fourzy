﻿//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
using GameSparks.Api.Responses;
using mixpanel;
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

        private int findChallengerErrorCount = 0;
        private int joinChallengeErrorCount = 0;
        private int getChallengeError = 0;
        private string challengeIdToJoin;
        private float elapsedTime;
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
        }

        public override void Open()
        {
            base.Open();

            challengeIdToJoin = "";
            BlockInput();
            backButton.SetActive(false);

            RealtimeManager.OnRealtimeReady += StartRealtimeGame;
            RealtimeManager.OnRealtimeMatchNotFound += RealtimeMatchNotFound;

            messageLabel.text = "Finding Match";

            if (isRealtime)
            {
                Mixpanel.StartTimedEvent("Find Realtime Match");
                RealtimeManager.Instance.FindPlayers();
            }
            else
            {
                Area selectedArea = GameContentManager.Instance.currentTheme.themeID;
                Debug.Log("GameContentManager.Instance.currentTheme.themeID: " + GameContentManager.Instance.currentTheme.themeID);
                ChallengeManager.Instance.CreateTurnBasedGame("", selectedArea, CreateTurnBasedGameSuccess, CreateTurnBasedGameError);
            }

            StartCoroutine(UpdateElapsedTimeRoutine());
            StartRoutine("randomText", ShowRandomTextRoutine());
        }

        public override void Close(bool animate = true)
        {
            base.Close(animate);

            RealtimeManager.OnRealtimeReady -= StartRealtimeGame;
            RealtimeManager.OnRealtimeMatchNotFound -= RealtimeMatchNotFound;

            StopRoutine("randomText", false);
            timerLabel.text = string.Empty;
        }

        public override void OnBack()
        {
            if (!inputBlocked)
            {
                base.OnBack();

                menuController.CloseCurrentScreen();
            }
        }

        public void OpenTurnbased()
        {
            isRealtime = false;

            menuController.OpenScreen(this);
        }

        public void OpenRealtime()
        {
            isRealtime = true;

            menuController.OpenScreen(this);
        }

        public void CancelMatchmaking()
        {
            Close();

            // var props = new Value();
            // props["Status"] = "Cancel";
            // Mixpanel.Track("Find Realtime Match", props);

            // RealtimeManager.Instance.CancelMatchmakingRequest();
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

            StartRoutine("openGame", 1.5f, () => {
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
            if (!isOpened)
                return;

            Debug.Log("***** Error Creating Turn based game: " + response.Errors.JSON);
            AnalyticsManager.LogError("create_turn_based_error", response.Errors.JSON);

            messageLabel.text = "Failed to create new game...";
            timerLabel.text = response.Errors.JSON;

            SetInteractable(true);
            backButton.SetActive(true);

            StopRoutine("randomText", false);
        }

        // private void FindChallengeSuccess(FindChallengeResponse response)
        // {
        //     if (!isOpened)
        //         return;

        //     challengeIdToJoin = "";

        //     GSEnumerable<FindChallengeResponse._Challenge> challengeInstances = response.ChallengeInstances;

        //     if (challengeInstances.Count() > 0)
        //     {
        //         List<string> challengeInstanceIds = new List<string>();

        //         //for every object in the challenges array, get the challengeId field and push to challengeInstanceId[]
        //         foreach (var chalInstance in challengeInstances)
        //         {
        //             challengeInstanceIds.Add(chalInstance.ChallengeId);
        //         }

        //         int randNum = UnityEngine.Random.Range(0, challengeInstanceIds.Count - 1);

        //         //reference the id at that random numbers location
        //         challengeIdToJoin = challengeInstanceIds[randNum];

        //         // For now players are joined to a random challenge
        //         ChallengeManager.Instance.JoinChallenge(challengeIdToJoin, JoinChallengeSuccess, JoinChallengeError);
        //     }
        //     else
        //     {
        //         if (elapsedTime < MINIMAL_TIMER_TIME)
        //         {
        //             Invoke("OpenNewMultiplayerGame", MINIMAL_TIMER_TIME - elapsedTime);
        //             Invoke("Close", MINIMAL_TIMER_TIME - elapsedTime);
        //         }
        //         else
        //         {
        //             //Send player to Game Screen to make the first move
        //             ChallengeManager.Instance.OpenNewMultiplayerGame();
        //             Close();
        //         }
        //     }
        // }

        private void StartRealtimeGame(int firstPlayerPeerID, int seed)
        {
            var props = new Value();
            props["Status"] = "Found";
            Mixpanel.Track("Find Realtime Match", props);

            ChallengeManager.Instance.OpenNewRealtimeGame(firstPlayerPeerID, seed);
            Close();
        }

        private void RealtimeMatchNotFound()
        {
            var props = new Value();
            props["Status"] = "Not Found";
            Mixpanel.Track("Find Realtime Match", props);

            messageLabel.text = "No one is playing now, try again later.";
            timerLabel.text = "";

            StopAllCoroutines();
        }

        // private void OpenNewMultiplayerGame()
        // {
        //     if (!isOpened)
        //         return;

        //     ChallengeManager.Instance.OpenNewMultiplayerGame();
        // }

        // private void FindChallengeError(FindChallengeResponse response)
        // {
        //     if (!isOpened)
        //         return;

        //     findChallengerErrorCount++;
        //     Debug.Log("***** Error Finding Random Challenge: " + response.Errors.JSON);
        //     AnalyticsManager.LogError("find_challenge_request_error", response.Errors.JSON);

        //     if (findChallengerErrorCount < 2)
        //     {
        //         ChallengeManager.Instance.FindRandomChallenge(FindChallengeSuccess, FindChallengeError);
        //     }
        //     else
        //     {
        //         messageLabel.text = response.Errors.JSON;
        //         timerLabel.text = "";

        //         StopAllCoroutines();
        //     }
        // }

        // private void JoinChallengeSuccess(JoinChallengeResponse response)
        // {
        //     if (!isOpened)
        //         return;

        //     //Send Player to Game Screen to make a move
        //     ChallengeManager.Instance.GetChallenge(challengeIdToJoin, GetChallengeSuccess, GetChallengeError);
        // }

        // private void JoinChallengeError(JoinChallengeResponse response)
        // {
        //     if (!isOpened)
        //         return;

        //     joinChallengeErrorCount++;
        //     Debug.Log("***** Error Joining Challenge: " + response.Errors.JSON);
        //     AnalyticsManager.LogError("join_challenge_request_error", response.Errors.JSON);

        //     if (joinChallengeErrorCount < 2)
        //     {
        //         ChallengeManager.Instance.JoinChallenge(challengeIdToJoin, JoinChallengeSuccess, JoinChallengeError);
        //     }
        //     else
        //     {
        //         messageLabel.text = response.Errors.JSON;
        //         timerLabel.text = "";

        //         StopAllCoroutines();
        //     }
        // }

        // private void GetChallengeSuccess(GetChallengeResponse response)
        // {
        //     if (!isOpened)
        //         return;

        //     var challenge = response.Challenge;
        //     GSData scriptData = response.ScriptData;

        //     ChallengeManager.Instance.challenge = challenge;

        //     if (elapsedTime < MINIMAL_TIMER_TIME)
        //     {
        //         Invoke("OpenJoinedMultiplayerGame", MINIMAL_TIMER_TIME - elapsedTime);
        //         Invoke("Hide", MINIMAL_TIMER_TIME - elapsedTime);
        //     }
        //     else
        //     {
        //         ChallengeManager.Instance.OpenJoinedMultiplayerGame();
        //         Close();
        //     }
        // }

        // private void OpenJoinedMultiplayerGame()
        // {
        //     if (!isOpened)
        //         return;

        //     ChallengeManager.Instance.OpenJoinedMultiplayerGame();
        // }

        // private void GetChallengeError(GetChallengeResponse response)
        // {
        //     if (!isOpened)
        //         return;

        //     getChallengeError++;
        //     Debug.Log("***** Error Getting Challenge: " + response.Errors);
        //     AnalyticsManager.LogError("get_challenge_request_error", response.Errors.JSON);

        //     if (getChallengeError < 2)
        //     {
        //         ChallengeManager.Instance.GetChallenge(challengeIdToJoin, GetChallengeSuccess, GetChallengeError);
        //     }
        //     else
        //     {
        //         messageLabel.text = response.Errors.JSON;
        //         timerLabel.text = "";

        //         StopAllCoroutines();
        //     }
        // }

        private IEnumerator UpdateElapsedTimeRoutine()
        {
            float startTime = Time.time;
            while (true)
            {
                elapsedTime = Time.time - startTime;
                yield return null;
            }
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
    }
}
