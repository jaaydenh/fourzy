using UnityEngine;
using HedgehogTeam.EasyTouch;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

namespace Fourzy
{
    public class View1 : UIView
    {
        [SerializeField] List<GameObject> gameViews = new List<GameObject>();

        [SerializeField] GameObject yourMoveGameGrid;
        [SerializeField] GameObject theirMoveGameGrid;
        [SerializeField] GameObject completedGameGrid;
        [SerializeField] GameObject resultsGameGrid;
        [SerializeField] GameObject activeGamePrefab;
        [SerializeField] GameObject inviteGrid;
        [SerializeField] GameObject invitePrefab;
        [SerializeField] GameObject NoMovesPanel;
        [SerializeField] GameObject loadingSpinner;
        [SerializeField] GameObject gamesListContainer;

        private bool pulledToRefresh;

        void Start()
        {
            keepHistory = true;

            loadingSpinner.GetComponent<Animator>().enabled = false;
            loadingSpinner.GetComponent<Image>().enabled = false;
        }

        public override void Show()
        {
            base.Show();

            GameManager.OnUpdateGames += GameManager_OnUpdateGames;
            GameManager.OnUpdateGame += GameManager_OnUpdateGame;

            this.ReloadGameViews();
        }

        public override void Hide()
        {
            GameManager.OnUpdateGames -= GameManager_OnUpdateGames;
            GameManager.OnUpdateGame -= GameManager_OnUpdateGame;

            base.Hide();
        }

        public override void ShowAnimated (AnimationDirection sourceDirection)
        {
            ViewController.instance.SetActiveView(TotalView.view1);
            base.ShowAnimated (sourceDirection);
        }

        public override void HideAnimated (AnimationDirection getAwayDirection)
        {  
            base.HideAnimated (getAwayDirection);
        }

        public void ReloadGamesOnClick()
        {
            ChallengeManager.Instance.GetChallengesRequest();
        }

        public void EditButtonOnClick()
        {
            
        }

        public void ViewContentScrollRectOnValueChanged(Vector2 pos)
        {
            if (!pulledToRefresh && pos.y > 1.06)
            {
                pulledToRefresh = true;
                loadingSpinner.GetComponent<Animator>().enabled = true;
                loadingSpinner.GetComponent<Image>().enabled = true;
                gamesListContainer.GetComponent<VerticalLayoutGroup>().padding.top = 250;
                ChallengeManager.Instance.GetChallengesRequest();
            }
            //if (!gettingChallenges && pos.y <= 1.015)
            //{
            //    pulledToRefresh = false;
            //}
        }

        void GameManager_OnUpdateGames(List<Game> games)
        {
            this.ReloadGameViews();

            if (pulledToRefresh)
            {
                StartCoroutine(WaitRoutine());
            }
        }

        void GameManager_OnUpdateGame(Game game)
        {
            this.ReloadGameViews();
        }

        private void ReloadGameViews()
        {
            for (int i = 0; i < gameViews.Count; i++)
            {
                Destroy(gameViews[i]);
            }
            gameViews.Clear();

            foreach (var game in GameManager.Instance.Games)
            {
                GameObject go = Instantiate(activeGamePrefab) as GameObject;
                GameUI gameUI = go.GetComponent<GameUI>();
                gameUI.game = game;

                if (!game.gameState.IsGameOver)
                {
                    if (game.gameState.isCurrentPlayerTurn)
                    {
                        go.transform.SetParent(yourMoveGameGrid.transform);
                    }
                    else
                    {
                        go.transform.SetParent(theirMoveGameGrid.transform);
                    }
                }
                else if (game.gameState.IsGameOver && game.isVisible == true && game.didViewResult == true)
                {
                    go.transform.SetParent(completedGameGrid.transform);
                }
                else if (game.gameState.IsGameOver && game.isVisible == true && game.didViewResult == false)
                {
                    go.transform.SetParent(resultsGameGrid.transform);
                }

                gameUI.transform.localScale = Vector3.one;
                gameViews.Add(go);
            }
        }

        IEnumerator WaitRoutine()
        {
            yield return new WaitForSeconds(0.8f);
            loadingSpinner.GetComponent<Animator>().enabled = false;
            loadingSpinner.GetComponent<Image>().enabled = false;
            gamesListContainer.GetComponent<VerticalLayoutGroup>().padding.top = 170;
            gamesListContainer.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();

            pulledToRefresh = false;
        }
    }
}
