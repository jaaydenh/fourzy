//@vadym udod

using GameSparks.Api.Responses;
using GameSparks.Core;
using mixpanel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class MatchmakingScreen : MenuScreen
    {
        private const float MINIMAL_TIMER_TIME = 3f;

        public TMP_Text messageLabel;
        public TMP_Text timerLabel;

        private int findChallengerErrorCount = 0;
        private int joinChallengeErrorCount = 0;
        private int getChallengeError = 0;
        private string challengeIdToJoin;
        private float elapsedTime;
        private List<string> matchMakingStrings;

        public bool isRealtime { get; set; }

        protected override void Awake()
        {
            base.Awake();

            matchMakingStrings = GameStringsLoader.Instance.GetMatchMakingWaitingStrings();
        }

        public override void Open()
        {
            base.Open();

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
                ChallengeManager.Instance.FindRandomChallenge(FindChallengeSuccess, FindChallengeError);
            }

            StartCoroutine(UpdateElapsedTimeRoutine());
            StartCoroutine(ShowRandomTextRoutine());
        }

        public override void Close()
        {
            base.Close();

            RealtimeManager.OnRealtimeReady -= StartRealtimeGame;
            RealtimeManager.OnRealtimeMatchNotFound -= RealtimeMatchNotFound;

            timerLabel.text = string.Empty;

            StopAllCoroutines();
        }

        public void CancelMatchmaking()
        {
            Close();

            var props = new Value();
            props["Status"] = "Cancel";
            Mixpanel.Track("Find Realtime Match", props);

            RealtimeManager.Instance.CancelMatchmakingRequest();
        }

        private void FindChallengeSuccess(FindChallengeResponse response)
        {
            if (!isOpened)
                return;
            
            challengeIdToJoin = "";

            GSEnumerable<FindChallengeResponse._Challenge> challengeInstances = response.ChallengeInstances;

            if (challengeInstances.Count() > 0)
            {
                List<string> challengeInstanceIds = new List<string>();

                //for every object in the challenges array, get the challengeId field and push to challengeInstanceId[]
                foreach (var chalInstance in challengeInstances)
                {
                    challengeInstanceIds.Add(chalInstance.ChallengeId);
                }

                int randNum = UnityEngine.Random.Range(0, challengeInstanceIds.Count - 1);

                //reference the id at that random numbers location
                challengeIdToJoin = challengeInstanceIds[randNum];

                // For now players are joined to a random challenge
                ChallengeManager.Instance.JoinChallenge(challengeIdToJoin, JoinChallengeSuccess, JoinChallengeError);
            }
            else
            {
                if (elapsedTime < MINIMAL_TIMER_TIME)
                {
                    Invoke("OpenNewMultiplayerGame", MINIMAL_TIMER_TIME - elapsedTime);
                    Invoke("Close", MINIMAL_TIMER_TIME - elapsedTime);
                }
                else
                {
                    //Send player to Game Screen to make the first move
                    ChallengeManager.Instance.OpenNewMultiplayerGame();
                    Close();
                }
            }
        }

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

        private void OpenNewMultiplayerGame()
        {
            if (!isOpened)
                return;

            ChallengeManager.Instance.OpenNewMultiplayerGame();
        }

        private void FindChallengeError(FindChallengeResponse response)
        {
            if (!isOpened)
                return;

            findChallengerErrorCount++;
            Debug.Log("***** Error Finding Random Challenge: " + response.Errors.JSON);
            AnalyticsManager.LogError("find_challenge_request_error", response.Errors.JSON);

            if (findChallengerErrorCount < 2)
            {
                ChallengeManager.Instance.FindRandomChallenge(FindChallengeSuccess, FindChallengeError);
            }
            else
            {
                messageLabel.text = response.Errors.JSON;
                timerLabel.text = "";

                StopAllCoroutines();
            }
        }

        private void JoinChallengeSuccess(JoinChallengeResponse response)
        {
            if (!isOpened)
                return;

            //Send Player to Game Screen to make a move
            ChallengeManager.Instance.GetChallenge(challengeIdToJoin, GetChallengeSuccess, GetChallengeError);
        }

        private void JoinChallengeError(JoinChallengeResponse response)
        {
            if (!isOpened)
                return;

            joinChallengeErrorCount++;
            Debug.Log("***** Error Joining Challenge: " + response.Errors.JSON);
            AnalyticsManager.LogError("join_challenge_request_error", response.Errors.JSON);

            if (joinChallengeErrorCount < 2)
            {
                ChallengeManager.Instance.JoinChallenge(challengeIdToJoin, JoinChallengeSuccess, JoinChallengeError);
            }
            else
            {
                messageLabel.text = response.Errors.JSON;
                timerLabel.text = "";

                StopAllCoroutines();
            }
        }

        private void GetChallengeSuccess(GetChallengeResponse response)
        {
            if (!isOpened)
                return;
            
            var challenge = response.Challenge;
            GSData scriptData = response.ScriptData;

            ChallengeManager.Instance.challenge = challenge;

            if (elapsedTime < MINIMAL_TIMER_TIME)
            {
                Invoke("OpenJoinedMultiplayerGame", MINIMAL_TIMER_TIME - elapsedTime);
                Invoke("Hide", MINIMAL_TIMER_TIME - elapsedTime);
            }
            else
            {
                ChallengeManager.Instance.OpenJoinedMultiplayerGame();
                Close();
            }
        }

        private void OpenJoinedMultiplayerGame()
        {
            if (!isOpened)
                return;

            ChallengeManager.Instance.OpenJoinedMultiplayerGame();
        }

        private void GetChallengeError(GetChallengeResponse response)
        {
            if (!isOpened)
                return;

            getChallengeError++;
            Debug.Log("***** Error Getting Challenge: " + response.Errors);
            AnalyticsManager.LogError("get_challenge_request_error", response.Errors.JSON);

            if (getChallengeError < 2)
            {
                ChallengeManager.Instance.GetChallenge(challengeIdToJoin, GetChallengeSuccess, GetChallengeError);
            }
            else
            {
                messageLabel.text = response.Errors.JSON;
                timerLabel.text = "";

                StopAllCoroutines();
            }
        }

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
            float time = 5.0f;

            List<int> indices = new List<int>();

            while (true)
            {
                time += Time.deltaTime;
                if (time > timeToShow)
                {
                    if (indices.Count == 0)
                    {
                        for (int i = 0; i < matchMakingStrings.Count; i++)
                        {
                            indices.Add(i);
                        }
                    }

                    yield return StartCoroutine(FadeTimerTextRoutine(0.0f));

                    int randIndex = Random.Range(0, indices.Count);
                    timerLabel.text = matchMakingStrings[indices[randIndex]];
                    indices.Remove(randIndex);
                    time = 0;

                    yield return StartCoroutine(FadeTimerTextRoutine(1.0f));
                }
                yield return null;
            }
        }

        private IEnumerator FadeTimerTextRoutine(float newAlpha)
        {
            const float fadeTime = 0.5f;
            float oldAlpha = timerLabel.alpha;
            for (float t = 0.0f; t < fadeTime; t += Time.deltaTime)
            {
                timerLabel.alpha = Mathf.Lerp(oldAlpha, newAlpha, t / fadeTime);
                yield return null;
            }
            timerLabel.alpha = newAlpha;
        }
    }
}
