//@vadym udod

using mixpanel;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class TrainingScreen : MenuScreen
    {
        public void PassAndPlayGame()
        {
            Mixpanel.Track("Pass And Play Button Press");

            //MenuController.GetMenu("MainMenuCanvas").GetScreen<GameboardSelectionScreen>().SelectBoard(GameType.PASSANDPLAY);
        }

        public void PuzzleChallengeGame()
        {

        }

        public void AIGame()
        {
            //MenuController.GetMenu("MainMenuCanvas").GetScreen<GameboardSelectionScreen>().SelectBoard(GameType.AI);
        }
    }
}
