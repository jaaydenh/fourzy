//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class FastPuzzlesScreen : MenuScreen
    {
        public LeaderboardPlayerWidget leaderboardWidgetPrefab;
        public RectTransform widgetsParent;
        public Badge noEntries;

        public void StartFastPuzzleGame() => GameManager.Instance.StartGame(GameContentManager.Instance.GetNextFastPuzzle());

        protected override void Awake()
        {
            base.Awake();

            GameManager.onNetworkAccess += NetworkAccess;
        }

        public override void Open()
        {
            base.Open();

            StartRoutine("showLB", ShowLeaderboard());

            HeaderScreen.instance.Close();
        }

        private void OnError(PlayFabError error)
        {
            Debug.Log(error.ErrorMessage);
        }

        private void OnLeaderboardLoaded(ExecuteCloudScriptResult result)
        {
            LeaderboardRequestResult leaderboard = JsonConvert.DeserializeObject<LeaderboardRequestResult>(result.FunctionResult.ToString());

            if (leaderboard.version != PlayerPrefsWrapper.GetFastPuzzlesLeaderboardVersion())
            {
                PlayerPrefsWrapper.SetFastPuzzlesLeaderboardVersion(leaderboard.version);

                //reset all
                GameContentManager.Instance.ResetFastPuzzles();
            }

            ClearEntries();

            if (leaderboard.leaderboard.Count < 2)
            {
                noEntries.SetState(true);
                return;
            }
            else
                noEntries.SetState(false);

            foreach (PlayerLeaderboardEntry entry in leaderboard.leaderboard) widgets.Add(Instantiate(leaderboardWidgetPrefab, widgetsParent).SetData(entry));
        }

        private void NetworkAccess(bool state)
        {
            if (isOpened)
            {
                if (state)
                    StartRoutine("showLB", ShowLeaderboard());
                else
                    ClearEntries();
            }
        }

        private void ClearEntries()
        {
            foreach (WidgetBase widget in widgets) Destroy(widget.gameObject);
            widgets.Clear();
        }

        private IEnumerator ShowLeaderboard()
        {
            NetworkManager.instance.FastCheck();

            float timer = 0f;
            //wait 10 seconds
            while ((string.IsNullOrEmpty(LoginManager.playerMasterAccountID) || !GameManager.NetworkAccess) && timer < 10f)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (timer >= 10f || !isOpened) yield break;

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "getFastPuzzleLB",
                FunctionParameter = new { maxCount = 6, }
            }, OnLeaderboardLoaded, OnError);
        }

        public class LeaderboardRequestResult
        {
            public List<PlayerLeaderboardEntry> leaderboard;
            public int version;
        }
    }
}