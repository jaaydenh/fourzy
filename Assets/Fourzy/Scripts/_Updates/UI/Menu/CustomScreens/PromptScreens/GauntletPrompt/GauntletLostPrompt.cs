//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.UI.Helpers;
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
                "Replay?",
                LocalizationManager.Value("no"),
                () =>
                {
                    UserManager.Instance.gems--;
                    menuController.CloseCurrentScreen();
                    GamePlayManager.instance.Rematch(true);
                },
                () => GamePlayManager.instance.BackButtonOnClick());
        }

        public override void Open()
        {
            base.Open();

            acceptButton.SetState(UserManager.Instance.gems > 0);
        }
    }
}