//@vadym udod

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GauntletIntroScreen : PromptScreen
    {
        public void _Prompt()
        {
            Prompt(
                LocalizationManager.Value("gauntlet"),
                LocalizationManager.Value("gauntlet_desc"),
                LocalizationManager.Value("continue"),
                null,
                () =>
                {
                    menuController.CloseCurrentScreen();
                    menuController.GetScreen<VSGamePrompt>().Prompt(5);
                },
                null);
        }
    }
}