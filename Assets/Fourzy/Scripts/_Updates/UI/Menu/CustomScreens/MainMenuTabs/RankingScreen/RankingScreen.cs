//@vadym udod

using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#if !MOBILE_SKILLZ
using PlayFab;
using PlayFab.ClientModels;
#endif

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class RankingScreen : MenuTab
    {
        public const int LOADED_WIDGETS_CAP = 50;
        public const int TO_LOAD_PER_REQUEST = 8;
        public const float TIMEOUT_TIME = 6f;

        public Badge loadingIndicator;
        public ButtonExtended reloadButton;
        public ScrollRect scrollView;
        public VerticalLayoutGroup verticalLayoutGroup;

        public FastPuzzlesLeaderboardPlayerWidget leaderboardWidgetPrefab;
        public RectTransform widgetsParent;

#if !MOBILE_SKILLZ
        private bool loadingLeaderboard = false;
        private bool checkScrolling = false;
        private bool firstLoad = true;
        private float prevScrollValue = 0f;
        private List<FastPuzzlesLeaderboardPlayerWidget> loadedWidgets = new List<FastPuzzlesLeaderboardPlayerWidget>();
        private List<PlayerLeaderboardEntry> playersData = new List<PlayerLeaderboardEntry>();

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

            if (!_opened)
            {
                StartRoutine("showLB", AddMoreResults());
            }
        }

        public override void Close(bool animate)
        {
            base.Close(animate);

            CancelLoading();
        }

        public void ReloadLeaderboard()
        {
            if (loadingLeaderboard) return;

            StartRoutine("showLB", AddMoreResults());
        }

        public void OnScrollValueChanged(Vector2 value)
        {
            if (checkScrolling && !loadingLeaderboard)
            {
                if (value.y >= 1f && (value.y > prevScrollValue || prevScrollValue == 1f))
                    StartRoutine("showLB", AddMoreResults(-1));
                else if (value.y <= 0f && (value.y < prevScrollValue || prevScrollValue == 0f))
                    StartRoutine("showLB", AddMoreResults(1));
            }

            prevScrollValue = value.y;
        }

        public void PointerCheck()
        {
            checkScrolling = true;
        }

        public void PointerUp()
        {
            checkScrolling = false;
        }

        private void ClearEntries()
        {
            foreach (WidgetBase widget in loadedWidgets) Destroy(widget.gameObject);
            loadedWidgets.Clear();

            playersData.Clear();
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
            GameManager.Instance.ReportPlayFabError(error.ErrorMessage);
        }

        private void OnValuesAdded(List<PlayerLeaderboardEntry> newValues, int direction)
        {
            if (!loadingLeaderboard) return;

            loadingLeaderboard = false;

            if (firstLoad)
            {
                if (!UserManager.Instance.ratingAssigned)
                {
                    menuController
                        .GetOrAddScreen<PromptScreen>()
                        .Prompt(
                            LocalizationManager.Value("leaderboard"),
                            string.Format(
                                LocalizationManager.Value("complete_games_to_see_rating"), 
                                InternalSettings.Current.GAMES_BEFORE_RATING_USED),
                            LocalizationManager.Value("ok"),
                            "")
                        .CloseOnAccept();
                }

                firstLoad = false;
            }

            //remove current user if rating is 0
            newValues.RemoveAll(_entry => _entry.PlayFabId == LoginManager.playfabId && _entry.StatValue == 0);

            if (newValues.Count == 0 && loadedWidgets.Count == 0)
            {
                loadingIndicator.SetValue("No entries");

                //no more entries to load in this direction
                return;
            }

            loadingIndicator.SetState(false);

            //remove copies if any
            List<PlayerLeaderboardEntry> toRemove = new List<PlayerLeaderboardEntry>(playersData
                    .Where(_entry => newValues.Any(_newEntry => _newEntry.PlayFabId == _entry.PlayFabId)));

            foreach (PlayerLeaderboardEntry entry in toRemove)
                RemoveEntry(playersData.IndexOf(entry));

            for (int newEntryIndex = 0; newEntryIndex < newValues.Count; newEntryIndex++)
            {
                int index = direction < 0 ? newEntryIndex : playersData.Count;
                AddEntry(newValues[newEntryIndex], index);
            }

            //cap values
            if (playersData.Count > LOADED_WIDGETS_CAP)
            {
                int amountToRemove = playersData.Count - LOADED_WIDGETS_CAP;
                int startIndex = direction < 0 ? LOADED_WIDGETS_CAP : 0;

                for (int removedIndex = 0; removedIndex < amountToRemove; removedIndex++)
                    RemoveEntry(startIndex);
            }

            for (int dataIndex = 0; dataIndex < playersData.Count; dataIndex++)
                loadedWidgets[dataIndex].SetData(playersData[dataIndex]);


            //
            void AddEntry(PlayerLeaderboardEntry entry, int index)
            {
                playersData.Insert(index, entry);

                FastPuzzlesLeaderboardPlayerWidget newWidget =
                    Instantiate(leaderboardWidgetPrefab, widgetsParent);

                loadedWidgets.Insert(index, newWidget);
                newWidget.transform.SetSiblingIndex(index);
            }

            void RemoveEntry(int index)
            {
                if (index < 0) return;

                Destroy(loadedWidgets[index].gameObject);
                loadedWidgets.RemoveAt(index);
                playersData.RemoveAt(index);
            }
        }

        private IEnumerator AddMoreResults(int direction = 0)
        {
            //check connection
            loadingLeaderboard = true;
            if (playersData.Count == 0) direction = 0;

            float timer = 0f;
            while (string.IsNullOrEmpty(LoginManager.playfabId) && timer < TIMEOUT_TIME)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (timer >= TIMEOUT_TIME || !isOpened)
            {
                loadingLeaderboard = false;
                loadingIndicator.SetValue("Player not logged in");

                yield break;
            }

            loadingIndicator.SetValue(LocalizationManager.Value("loading"));

            PlayerProfileViewConstraints viewConstraints = new PlayerProfileViewConstraints()
            {
                ShowAvatarUrl = true,
                ShowDisplayName = true,
            };

            if (direction == 0)
            {
                ClearEntries();
                PlayFabClientAPI.GetLeaderboardAroundPlayer(
                    new GetLeaderboardAroundPlayerRequest()
                    {
                        PlayFabId = LoginManager.playfabId,
                        StatisticName = "All Time Rating",
                        MaxResultsCount = TO_LOAD_PER_REQUEST,
                        ProfileConstraints = viewConstraints,
                    },
                    result => OnValuesAdded(result.Leaderboard, direction),
                    OnError);
            }
            else
            {
                int startIndex =
                    playersData[direction > 0 ? playersData.Count - 1 : 0].Position
                    + (direction < 0 ? -TO_LOAD_PER_REQUEST : 0)
                    + (direction > 0 ? 1 : 0);

                startIndex = Mathf.Clamp(startIndex, 0, int.MaxValue);

                PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest()
                {
                    StatisticName = "All Time Rating",
                    MaxResultsCount = TO_LOAD_PER_REQUEST,
                    StartPosition = startIndex,
                    ProfileConstraints = viewConstraints,
                },
                result => OnValuesAdded(result.Leaderboard, direction),
                OnError);
            }
        }
#endif
    }
}