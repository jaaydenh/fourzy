//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Widgets;
using System;

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

        public void Prompt(BasicPuzzlePack puzzlePack, Action onDeny = null, Action onAccept = null)
        {
            if (this.puzzlePack == null || this.puzzlePack != puzzlePack)
            {
                this.puzzlePack = puzzlePack;

                progressWidget.SetData(puzzlePack);

                widgets.Clear();
                widgets.Add(progressWidget);
                widgets.AddRange(progressWidget.widgets);
            }

            promptText.gameObject.SetActive(!string.IsNullOrEmpty(puzzlePack.description));

            Prompt(puzzlePack.name, puzzlePack.description,
            onAccept ?? (() =>
            {
                menuController.CloseCurrentScreen(false);
                puzzlePack.StartNextUnsolvedPuzzle();
            }),
            onDeny ?? (() => menuController.CloseCurrentScreen()));
        }
    }
}