//@vadym udod

using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using UnityEngine;
using static Fourzy._Updates.UI.Menu.Screens.FastPuzzlesScreen;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class RankingScreen : MenuTab
    {
        public Badge loadingIndicator;

        public FastPuzzlesLeaderboardPlayerWidget leaderboardWidgetPrefab;
        public RectTransform widgetsParent;

        private bool loadingLeaderboard = false;

        protected override void Awake()
        {
            base.Awake();

            GameManager.onNetworkAccess += NetworkAccess;
        }

        protected void OnDestroy()
        {
            GameManager.onNetworkAccess -= NetworkAccess;
        }

        public override void Open()
        {
            bool _opened = isOpened;
            base.Open();

            if (!_opened) StartRoutine("showLB", ShowLeaderboard());
        }

        public override void Close(bool animate)
        {
            base.Close(animate);

            CancelLoading();
        }

        public void ReloadLeaderboard()
        {
            if (loadingLeaderboard) return;

            StartRoutine("showLB", ShowLeaderboard());
        }

        private void ClearEntries()
        {
            foreach (WidgetBase widget in widgets) Destroy(widget.gameObject);
            widgets.Clear();
        }

        private void NetworkAccess(bool state)
        {
            if (!state) CancelLoading();
        }

        private void CancelLoading()
        {
            if (loadingLeaderboard)
            {
                loadingLeaderboard = false;
                CancelRoutine("showLB");
            }
        }

        private void OnError(PlayFabError error)
        {
            loadingLeaderboard = false;
            loadingIndicator.SetValue(LocalizationManager.Value(error.ErrorMessage));

            Debug.Log(error.ErrorMessage);
        }

        private void OnLeaderboardLoaded(ExecuteCloudScriptResult result)
        {
            if (!loadingLeaderboard) return;

            loadingLeaderboard = false;
            loadingIndicator.SetState(false);

            LeaderboardRequestResult leaderboard =
                JsonConvert.DeserializeObject<LeaderboardRequestResult>(result.FunctionResult.ToString());

            foreach (PlayerLeaderboardEntry entry in leaderboard.s1)
                widgets.Add(Instantiate(leaderboardWidgetPrefab, widgetsParent).SetData(entry));

            if (leaderboard.s2 != null)
                foreach (PlayerLeaderboardEntry entry in leaderboard.s2)
                    widgets.Add(Instantiate(leaderboardWidgetPrefab, widgetsParent).SetData(entry));
        }

        private IEnumerator ShowLeaderboard()
        {
            ClearEntries();

            loadingLeaderboard = true;
            float timer = 0f;
            //wait 10 seconds
            while (
                (string.IsNullOrEmpty(LoginManager.playerMasterAccountID) || !GameManager.NetworkAccess) &&
                timer < 10f)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (timer >= 10f || !isOpened) yield break;

            loadingIndicator.SetValue(LocalizationManager.Value("loading"));

            //PlayFabClientAPI.GetLeaderboardAroundPlayer

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "getLeaderboard",
                FunctionParameter = new { maxCount = 6, tableName = "All Time Rating" }
            }, OnLeaderboardLoaded, OnError);
        }
    }
}