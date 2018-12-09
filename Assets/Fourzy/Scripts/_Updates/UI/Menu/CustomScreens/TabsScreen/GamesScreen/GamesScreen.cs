//@vadym udod

using Fourzy._Updates.UI.Widgets;
using mixpanel;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GamesScreen : MenuScreen
    {
        public ActiveGameWidget activeGameWidgetPrefab;

        public Transform yourMoveGameGrid;
        public Transform theirMoveGameGrid;
        public Transform completedGameGrid;
        public Transform resultsGameGrid;
        public GameObject loadingIcon;

        private List<ActiveGameWidget> gameViews;
        private bool pulledToRefresh;

        protected override void Awake()
        {
            base.Awake();

            gameViews = new List<ActiveGameWidget>();
        }

        public override void Open()
        {
            base.Open();

            GameManager.OnUpdateGames += GameManager_OnUpdateGames;
            GameManager.OnUpdateGame += GameManager_OnUpdateGame;

            ReloadGameViews();
        }

        public override void Close()
        {
            base.Close();

            GameManager.OnUpdateGames -= GameManager_OnUpdateGames;
            GameManager.OnUpdateGame -= GameManager_OnUpdateGame;
        }

        public void ViewContentScrollRectOnValueChanged(Vector2 pos)
        {
            if (!pulledToRefresh && pos.y > 1.06)
            {
                pulledToRefresh = true;
                loadingIcon.SetActive(true);

                ChallengeManager.Instance.GetChallengesRequest();
            }
        }

        public void ReloadGamesOnClick()
        {
            Mixpanel.Track("Reload Games Button Press");
            ChallengeManager.Instance.GetChallengesRequest();
        }

        private void GameManager_OnUpdateGames(List<Game> games)
        {
            ReloadGameViews();

            if (pulledToRefresh)
            {
                StopRoutine("WaitRoutine", false);
                StartRoutine("WaitRoutine", .8f, () =>
                {
                    loadingIcon.SetActive(false);
                    pulledToRefresh = false;
                }, null);
            }
        }

        private void GameManager_OnUpdateGame(Game game)
        {
            ReloadGameViews();
        }

        private void ReloadGameViews()
        {
            foreach (ActiveGameWidget gameWidget in gameViews)
                Destroy(gameWidget.gameObject);

            gameViews.Clear();

            foreach (Game game in GameManager.Instance.Games)
            {
                ActiveGameWidget gameWidget = Instantiate(activeGameWidgetPrefab);

                if (!game.gameState.IsGameOver)
                {
                    if (game.gameState.isCurrentPlayerTurn)
                        gameWidget.transform.SetParent(yourMoveGameGrid.transform);
                    else
                        gameWidget.transform.SetParent(theirMoveGameGrid.transform);
                }
                else if (game.gameState.IsGameOver && game.isVisible == true && game.didViewResult == true)
                    gameWidget.transform.SetParent(completedGameGrid.transform);
                else if (game.gameState.IsGameOver && game.isVisible == true && game.didViewResult == false)
                    gameWidget.transform.SetParent(resultsGameGrid.transform);

                gameWidget.transform.localScale = Vector3.one;
                gameViews.Add(gameWidget);
            }
        }
    }
}
