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
                case GameType.PRESENTATION:
                    base.Open();

                    break;

                default:
                    if (isOpened) Close();

                    break;
            }
        }

        public void _Update()
        {

        }

        public void GameComplete()
        {

        }

        public void OnBGTap()
        {
            //leave game
            menuController.GetOrAddScreen<GameplayScreen>().OnBack();
        }
    }
}