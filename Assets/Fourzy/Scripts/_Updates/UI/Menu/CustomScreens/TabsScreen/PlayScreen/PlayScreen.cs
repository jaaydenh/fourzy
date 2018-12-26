//@vadym udod

using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using mixpanel;
using System.Collections.Generic;
using System.Linq;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PlayScreen : MenuScreen
    {
        public PlayerInfoWidget playerInfoIwdget;
        public ButtonExtended turnPlayButton;

        private MatchmakingScreen matchmakingScreen;

        protected override void Awake()
        {
            base.Awake();

            GameManager.OnUpdateGames += GameManager_OnUpdateGames;
        }

        protected override void Start()
        {
            base.Start();

            matchmakingScreen = menuController.GetScreen<MatchmakingScreen>();
        }

        protected void OnDestroy()
        {
            GameManager.OnUpdateGames -= GameManager_OnUpdateGames;
        }

        public override void Open()
        {
            base.Open();

            playerInfoIwdget.UpdateWidget();
        }

        public void StartTurnGame()
        {
            Game game = GameManager.Instance.GetNextActiveGame();

            var props = new Value();
            if (game != null)
            {
                props["HasGameWithMove"] = "True";
                GameManager.Instance.OpenGame(game);
            }
            else
            {
                props["HasGameWithMove"] = "False";

                matchmakingScreen.isRealtime = false;
                menuController.OpenScreen(matchmakingScreen);
            }
            Mixpanel.Track("Turn Play Button Press", props);
        }

        public void StartRealtimeGame()
        {
            System.DayOfWeek today = System.DateTime.Now.DayOfWeek;
            int hour = System.DateTime.UtcNow.Hour;
            bool isDLS = System.DateTime.UtcNow.IsDaylightSavingTime();

            if (today == System.DayOfWeek.Monday || today == System.DayOfWeek.Wednesday || today == System.DayOfWeek.Friday)
            {
                if ((!isDLS && hour == 1) || (isDLS && hour == 0))
                {
                    var props = new Value();
                    props["Source"] = "Home Screen";
                    Mixpanel.Track("Start Realtime Matchmaking", props);

                    matchmakingScreen.isRealtime = true;
                    menuController.OpenScreen(matchmakingScreen);
                }
                else
                {
                    Mixpanel.Track("Open Realtime Confirmation");
                    menuController.GetScreen<PromptScreen>().Prompt(
                        "Fourzy is in Beta",
                        "During beta, realtime matchmaking is scheduled for Mon, Wed, and Fri from 9-9:30pm. There are limited players outside of these times. Do you want to continue?",
                        "Yes",
                        "No",
                        () =>
                        {
                            menuController.CloseCurrentScreen();

                            matchmakingScreen.isRealtime = true;
                            menuController.OpenScreen(matchmakingScreen);
                        });
                }
            }
            else
            {
                Mixpanel.Track("Open Realtime Confirmation");
                menuController.GetScreen<PromptScreen>().Prompt(
                    "Fourzy is in Beta",
                    "During beta, realtime matchmaking is scheduled for Mon, Wed, and Fri from 9-9:30pm. There are limited players outside of these times. Do you want to continue?",
                    "Yes",
                    "No",
                    () =>
                    {
                        menuController.CloseCurrentScreen();

                        matchmakingScreen.isRealtime = true;
                        menuController.OpenScreen(matchmakingScreen);
                    });
            }
        }

        private void GameManager_OnUpdateGames(List<Game> games)
        {
            UpdateTurnGamesCount();
        }

        private void UpdateTurnGamesCount()
        {
            turnPlayButton.GetBadge().badge.SetValue(
                GameManager.Instance.Games.Where(game => game.gameState.isCurrentPlayerTurn == true || (game.didViewResult == false && game.gameState.IsGameOver == true)).Count());
        }
    }
}