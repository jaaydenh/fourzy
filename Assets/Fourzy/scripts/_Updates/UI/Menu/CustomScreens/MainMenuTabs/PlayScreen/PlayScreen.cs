//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using mixpanel;
using System.Collections.Generic;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PlayScreen : MenuTab
    {
        public PlayerInfoWidget playerInfoIwdget;
        public ButtonExtended turnPlayButton;

        private MatchmakingScreen matchmakingScreen;

        protected override void Awake()
        {
            base.Awake();

            ChallengeManager.OnChallengesUpdate += OnChallengesUpdate;
            ChallengeManager.OnChallengeUpdate += OnChallengeUpdate;
            ChallengeManager.OnChallengeUpdateLocal += OnChallengeUpdate;
        }

        protected override void Start()
        {
            base.Start();

            matchmakingScreen = menuController.GetScreen<MatchmakingScreen>();

            turnPlayButton.GetBadge("games").badge.SetValue(ChallengeManager.Instance.NextChallenges.Count);
        }

        public void ContinueStartTurnBasedGame()
        {
            List<ChallengeData> next = ChallengeManager.Instance.NextChallenges;

            //open game
            if (next.Count > 0)
                GameManager.Instance.StartGame(next[0].GetGameForPreviousMove());
            else
                StartTurnGame();
        }

        public void StartTurnGame()
        {
            var props = new Value();
            props["HasGameWithMove"] = "False";
            Mixpanel.Track("Turn Play Button Press", props);

            matchmakingScreen.OpenTurnbased();
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

        private void OnChallengesUpdate(List<ChallengeData> data) => turnPlayButton.GetBadge("games").badge.SetValue(ChallengeManager.Instance.NextChallenges.Count);

        private void OnChallengeUpdate(ChallengeData data) => turnPlayButton.GetBadge("games").badge.SetValue(ChallengeManager.Instance.NextChallenges.Count);
    }
}