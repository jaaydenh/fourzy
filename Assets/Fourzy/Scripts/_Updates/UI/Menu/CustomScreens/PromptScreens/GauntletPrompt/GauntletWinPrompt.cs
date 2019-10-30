//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.GameplayScene;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GauntletWinPrompt : PromptScreen
    {
        public void _Prompt(IClientFourzy game)
        {
            if (game._Mode != GameMode.GAUNTLET) return;

            Prompt(
                LocalizationManager.Value("complete"),
                string.Format(LocalizationManager.Value("gauntlet_complete"), game.puzzleData.gauntletStatus.FourzyCount),
                "OK",
                null,
                () => GamePlayManager.instance.BackButtonOnClick(),
                () => GamePlayManager.instance.BackButtonOnClick());
        }
    }
}