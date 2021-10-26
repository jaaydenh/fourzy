//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GamesScreen : MenuTab
    {
        public static GamesScreen instance;

        public static float maxPullDistance = -100f;

        public ActiveGameWidget activeGameWidgetPrefab;

        public RectTransform contentsParent;
        public RefreshIndicatorWidget refreshIndicator;
        public ScrollRect scrollRect;

        public Transform yourMoveGameGrid;
        public Transform theirMoveGameGrid;
        public Transform completedGameGrid;
        public Transform resultsGameGrid;

        private List<ActiveGameWidget> challengesViews;
        private bool pulledToRefresh;

        protected override void Awake()
        {
            base.Awake();

            instance = this;
            challengesViews = new List<ActiveGameWidget>();

            //ChallengeManager.OnChallengesUpdate += OnChallengesUpdate;
            //ChallengeManager.OnChallengeUpdateLocal += OnChallengeUpdate;
            //ChallengeManager.OnChallengeUpdate += OnChallengeUpdate;
        }

        public void ViewContentScrollRectOnValueChanged(Vector2 pos)
        {
            if (pulledToRefresh) return;

            if (contentsParent.anchoredPosition.y < 0f)
                refreshIndicator.SetAlpha(Mathf.Clamp01(contentsParent.anchoredPosition.y / maxPullDistance));

            if (contentsParent.anchoredPosition.y < maxPullDistance)
            {
                pulledToRefresh = true;

                refreshIndicator.SetAnimationState(true);

                // ChallengeManager.Instance.GetChallengesRequest();
            }
        }

        public void ReloadGamesOnClick()
        {
            // ChallengeManager.Instance.GetChallengesRequest();
        }

        private void OnChallengesUpdate(List<ChallengeData> data)
        {
            //find if new challenge was added
            string[] challengeIDs = challengesViews.Select(widget => widget.data.challengeInstanceId).ToArray();
            foreach (ChallengeData challengeData in data)
                if (!challengeIDs.Contains(challengeData.challengeInstanceId))
                    AddChallengeWidget(challengeData);

            if (pulledToRefresh)
            {
                refreshIndicator.SetAlpha(0f);
                refreshIndicator.SetAnimationState(false);
                pulledToRefresh = false;
                scrollRect.normalizedPosition = Vector2.up;
            }
        }

        public void UpdateChallenge(ChallengeData data)
        {
            //check if updated game finished
            //if so, check if active game is the one that got update, so it can be set as viewed
            if (GameManager.Instance.activeGame != null && GameManager.Instance.activeGame._Type == GameType.TURN_BASED)
            {
                if (GameManager.Instance.activeGame.BoardID == data.challengeInstanceId && data.lastTurnGame.IsOver)
                    PlayerPrefsWrapper.SetGameViewed(data.challengeInstanceId);
            }

            UpdateChallengeView(data);
        }

        private void ReloadGameViews()
        {
            ClearPreviousGames();

            // foreach (ChallengeData data in ChallengeManager.Instance.Challenges)
                // AddChallengeWidget(data);
        }

        private ActiveGameWidget AddChallengeWidget(ChallengeData data)
        {
            ActiveGameWidget challengeWidget = Instantiate(activeGameWidgetPrefab);
            challengeWidget.menuScreen = this;

            UpdateChallengeView(challengeWidget, data);

            challengesViews.Add(challengeWidget);

            return challengeWidget;
        }

        #region Challenge got updated, update view

        private void OnChallengeUpdate(ChallengeData data) => UpdateChallengeView(data);

        public void UpdateChallengeView(ChallengeData data) => UpdateChallengeView(challengesViews.Find(widget => widget.data.challengeInstanceId == data.challengeInstanceId), data);

        public void UpdateChallengeView(ActiveGameWidget widget, ChallengeData data)
        {
            ClientFourzyGame game = data.lastTurnGame;

            if (game.IsOver)
                widget.transform.SetParent(PlayerPrefsWrapper.GetGameViewed(data.challengeInstanceId) ? completedGameGrid : resultsGameGrid);
            else
                widget.transform.SetParent(game.isMyTurn ? yourMoveGameGrid.transform : theirMoveGameGrid.transform);

            completedGameGrid.gameObject.SetActive(completedGameGrid.childCount > 1);
            resultsGameGrid.gameObject.SetActive(resultsGameGrid.childCount > 1);

            widget.transform.localScale = Vector3.one;
            widget.SetData(this, data, game);
        }

        #endregion

        private void ClearPreviousGames()
        {
            foreach (ActiveGameWidget gameWidget in challengesViews) Destroy(gameWidget.gameObject);

            challengesViews.Clear();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            //if (ChallengeManager.Instance.Challenges.Count > 0) OnChallengesUpdate(ChallengeManager.Instance.Challenges);
        }
    }
}
