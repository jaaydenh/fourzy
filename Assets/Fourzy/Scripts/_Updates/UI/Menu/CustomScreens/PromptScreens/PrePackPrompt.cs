//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Widgets;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PrePackPrompt : PromptScreen
    {
        public PuzzlePackProgressWidget progressWidget;

        private BasicPuzzlePack puzzlePack;

        public override void OnBack()
        {
            base.OnBack();

            menuController.CloseCurrentScreen(true);
        }

        public void Prompt(BasicPuzzlePack puzzlePack)
        {
            if (this.puzzlePack == null || this.puzzlePack != puzzlePack)
            {
                this.puzzlePack = puzzlePack;

                progressWidget.SetData(puzzlePack);

                widgets.Clear();
                widgets.Add(progressWidget);
                widgets.AddRange(progressWidget.widgets);
            }

            Prompt(puzzlePack.name, null, () =>
            {
                menuController.CloseCurrentScreen(false);
                puzzlePack.StartNextUnsolvedPuzzle();
            }, () => menuController.CloseCurrentScreen());
        }
    }
}