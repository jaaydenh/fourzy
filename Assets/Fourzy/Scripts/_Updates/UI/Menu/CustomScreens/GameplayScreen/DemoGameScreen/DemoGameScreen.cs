//@vadym udod

using Fourzy._Updates.ClientModel;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class DemoGameScreen : MenuScreen
    {
        public void Open(IClientFourzy game)
        {
            switch (game._Type)
            {
                case GameType.DEMO:
                    base.Open();

                    break;
            }
        }

        public void GameComplete()
        {

        }

        public void OnBGTap()
        {
            //leave game
            menuController.GetScreen<GameplayScreen>().OnBack();
        }
    }
}