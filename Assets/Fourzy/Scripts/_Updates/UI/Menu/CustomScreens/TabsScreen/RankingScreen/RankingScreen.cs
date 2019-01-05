//@vadym udod

using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class RankingScreen : MenuScreen
    {
        public TMP_Text noPlayerText;
        public LeaderboardPlayerWidget playerLeaderboardWidgetPrefab;
        public RectTransform widgetsParent;

        private bool loadingLeaderboard = false;

        public void GetWinsLeaderboard()
        {
            if (loadingLeaderboard)
                return;

            loadingLeaderboard = true;

            //SetInteractable(false);

            LoginManager.Instance.GetWinsLeaderboard(GetLeaderboardCallback);
        }

        public void GetTrophiesLeaderboard()
        {
            if (loadingLeaderboard)
                return;

            loadingLeaderboard = true;

            //SetInteractable(false);

            LoginManager.Instance.GetCoinsEarnedLeaderboard(GetLeaderboardCallback);
        }

        private void GetLeaderboardCallback(List<Leaderboard> leaderboards, string errorMessage)
        {
            loadingLeaderboard = false;

            //remove old ones
            foreach (Transform playerWidget in widgetsParent)
                Destroy(playerWidget.gameObject);

            //SetInteractable(true);

            if (leaderboards == null)
            {
                noPlayerText.text = errorMessage;
                noPlayerText.gameObject.SetActive(true);
                return;
            }

            noPlayerText.gameObject.SetActive(leaderboards.Count == 0);

            foreach (Leaderboard leaderboard in leaderboards)
            {
                LeaderboardPlayerWidget leaderboardPlayerWidget = Instantiate(playerLeaderboardWidgetPrefab, widgetsParent);

                leaderboardPlayerWidget.SetData(leaderboard);
            }
        }
    }
}