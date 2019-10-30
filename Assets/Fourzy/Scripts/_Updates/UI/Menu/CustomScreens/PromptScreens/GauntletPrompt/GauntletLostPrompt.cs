//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.GameplayScene;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GauntletLostPrompt : PromptScreen
    {
        public void _Prompt(IClientFourzy game)
        {
            if (game._Mode != GameMode.GAUNTLET) return;

            Prompt(
                LocalizationManager.Value("gauntlet_failed"),
                LocalizationManager.Value("gauntlet_failed_message"),
                "OK",
                null,
                () => GamePlayManager.instance.BackButtonOnClick(),
                () => GamePlayManager.instance.BackButtonOnClick());
        }
    }
}