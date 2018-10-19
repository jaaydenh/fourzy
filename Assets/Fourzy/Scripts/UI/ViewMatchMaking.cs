using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Core;
using GameSparks.Api.Responses;
using System.Linq;
using TMPro;
using mixpanel;

namespace Fourzy
{
    public class ViewMatchMaking : UIView
    {
        public static ViewMatchMaking instance;
        public TextMeshProUGUI userFacingMessage;
        public TextMeshProUGUI timerText;
        [SerializeField] GameObject cancelButton;
        public bool isRealtime;
        private int findChallengerErrorCount = 0;
        private int joinChallengeErrorCount = 0;
        private int getChallengeError = 0;
        private string challengeIdToJoin;
        private float elapsedTime;
        private float minimumTimerTime = 3f;

        private List<string> matchMakingStrings = new List<string>();

        void Start()
        {
            instance = this;
            keepHistory = false;

            Button btn = cancelButton.GetComponent<Button>();
            btn.onClick.AddListener(CancelMatchmaking);

            matchMakingStrings = GameStringsLoader.Instance.GetMatchMakingWaitingStrings();
        }

        void OnEnable() 
        {
            RealtimeManager.OnRealtimeReady += StartRealtimeGame;
            RealtimeManager.OnRealtimeMatchNotFound += RealtimeMatchNotFound;
        }

        void OnDisable() 
        {
            RealtimeManager.OnRealtimeReady -= StartRealtimeGame;
            RealtimeManager.OnRealtimeMatchNotFound -= RealtimeMatchNotFound;
        }

        public override void Show()
        {
            base.Show();

            userFacingMessage.text = "Finding Match...";
            // backButton.SetActive(false);
            if (isRealtime) {
                Mixpanel.StartTimedEvent("Find Realtime Match");
                RealtimeManager.Instance.FindPlayers();
            } else {
                ChallengeManager.Instance.FindRandomChallenge(FindChallengeSuccess, FindChallengeError);
            }

            this.StartCoroutine(UpdateElapsedTimeRoutine());
            this.StartCoroutine(ShowRandomTextRoutine());
        }

        public override void Hide()
        {
            base.Hide();
            timerText.text = string.Empty;

            this.StopAllCoroutines();
        }

        private IEnumerator UpdateElapsedTimeRoutine()
        {
            float startTime = Time.time;
            while(true)
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

            while(true)
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

                    yield return this.StartCoroutine(FadeTimerTextRoutine(0.0f));

                    int randIndex = Random.Range(0, indices.Count);
                    timerText.text = matchMakingStrings[indices[randIndex]];
                    indices.Remove(randIndex);
                    time = 0;

                    yield return this.StartCoroutine(FadeTimerTextRoutine(1.0f));
                }
                yield return null;
            }
        }

        private IEnumerator FadeTimerTextRoutine(float newAlpha)
        {
            const float fadeTime = 0.5f;
            float oldAlpha = timerText.alpha;
            for (float t = 0.0f; t < fadeTime; t += Time.deltaTime)
            {
                timerText.alpha = Mathf.Lerp(oldAlpha, newAlpha, t / fadeTime);
                yield return null;
            }
            timerText.alpha = newAlpha;
        }

        public void CancelMatchmaking()
        {
            var props = new Value();
            props["Status"] = "Cancel";
            Mixpanel.Track("Find Realtime Match", props);
            RealtimeManager.Instance.CancelMatchmakingRequest();
            ViewController.instance.ChangeView(ViewController.instance.view3);
            Hide();
            ViewController.instance.ShowTabView();
        }

        private void FindChallengeSuccess(FindChallengeResponse response)
        {
            // Debug.Log("ViewMatchMaking : FindChallengeSuccess");
            challengeIdToJoin = "";

            GSEnumerable<FindChallengeResponse._Challenge> challengeInstances = response.ChallengeInstances;
            //GSData scriptData = response.ScriptData; 

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
                //each time you run this code, a different id is set in the scriptdata
                //Spark.setScriptData("challenge to join", randomChallengeId);

                // For now players are joined to a random challenge
                ChallengeManager.Instance.JoinChallenge(challengeIdToJoin, JoinChallengeSuccess, JoinChallengeError);
            }
            else
            {
                if (elapsedTime < minimumTimerTime) {
                    Invoke("OpenNewMultiplayerGame", minimumTimerTime - elapsedTime);
                    Invoke("Hide", minimumTimerTime - elapsedTime);
                } else {
                    //Send player to Game Screen to make the first move
                    ChallengeManager.Instance.OpenNewMultiplayerGame();
                    Hide();
                }
            }
        }

        private void StartRealtimeGame(int firstPlayerPeerId, int seed) 
        {
            var props = new Value();
            props["Status"] = "Found";
            Mixpanel.Track("Find Realtime Match", props);
            ChallengeManager.Instance.OpenNewRealtimeGame(firstPlayerPeerId, seed);
            Hide();
        }

        private void RealtimeMatchNotFound() 
        {
            var props = new Value();
            props["Status"] = "Not Found";
            Mixpanel.Track("Find Realtime Match", props);
            userFacingMessage.text = "No one is playing now, try again later.";
            timerText.text = "";
            //backButton.SetActive(true);
            this.StopAllCoroutines();
        }

        private void OpenNewMultiplayerGame() 
        {
            ChallengeManager.Instance.OpenNewMultiplayerGame();
        }

        private void FindChallengeError(FindChallengeResponse response)
        {
            findChallengerErrorCount++;
            Debug.Log("***** Error Finding Random Challenge: " + response.Errors.JSON);
            AnalyticsManager.LogError("find_challenge_request_error", response.Errors.JSON);

            if (findChallengerErrorCount < 2) {
                ChallengeManager.Instance.FindRandomChallenge(FindChallengeSuccess, FindChallengeError);
            } else {
                userFacingMessage.text = response.Errors.JSON;
                timerText.text = "";
                //backButton.SetActive(true);
                this.StopAllCoroutines();
            }
        }

        private void JoinChallengeSuccess(JoinChallengeResponse response) {
            // Debug.Log("ViewMatchMaking : JoinChallengeSuccess");
            // GameManager.Instance.challengeInstanceId = challengeIdToJoin;
            //Send Player to Game Screen to make a move
            ChallengeManager.Instance.GetChallenge(challengeIdToJoin, GetChallengeSuccess, GetChallengeError);
        }

        private void JoinChallengeError(JoinChallengeResponse response)
        {
            joinChallengeErrorCount++;
            Debug.Log("***** Error Joining Challenge: " + response.Errors.JSON);
            AnalyticsManager.LogError("join_challenge_request_error", response.Errors.JSON);

            if (joinChallengeErrorCount < 2) {
                ChallengeManager.Instance.JoinChallenge(challengeIdToJoin, JoinChallengeSuccess, JoinChallengeError);
            } else {
                userFacingMessage.text = response.Errors.JSON;
                timerText.text = "";
                //backButton.SetActive(true);
                this.StopAllCoroutines();
            }
        }

        private void GetChallengeSuccess(GetChallengeResponse response) {
            // Debug.Log("ViewMatchMaking : GetChallengeSuccess");
            var challenge = response.Challenge;
            GSData scriptData = response.ScriptData;

            ChallengeManager.Instance.challenge = challenge;

            if (elapsedTime < minimumTimerTime)
            {
                Invoke("OpenJoinedMultiplayerGame", minimumTimerTime - elapsedTime);
                Invoke("Hide", minimumTimerTime - elapsedTime);
            } else {
                ChallengeManager.Instance.OpenJoinedMultiplayerGame();
                Hide();
            }
        }

        private void OpenJoinedMultiplayerGame() 
        {
            ChallengeManager.Instance.OpenJoinedMultiplayerGame();
        }

        private void GetChallengeError(GetChallengeResponse response) 
        {
            getChallengeError++;
            Debug.Log("***** Error Getting Challenge: " + response.Errors);
            AnalyticsManager.LogError("get_challenge_request_error", response.Errors.JSON);

            if (getChallengeError < 2) {
                ChallengeManager.Instance.GetChallenge(challengeIdToJoin, GetChallengeSuccess, GetChallengeError);
            } else {
                userFacingMessage.text = response.Errors.JSON;
                timerText.text = "";
                //backButton.SetActive(true);
                this.StopAllCoroutines();
            }
        }
    }
}
