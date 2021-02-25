//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Menu.Screens;
using TMPro;

namespace Fourzy._Updates.UI.Widgets
{
    public class SelectPackWidget : WidgetBase
    {
        public TMP_Text label;

        private BasicPuzzlePack pack;

        public SelectPackWidget SetData(BasicPuzzlePack pack)
        {
            this.pack = pack;
            label.text = pack.name;

            return this;
        }

        public void OnTap()
        {
            if (!pack) return;

            switch (pack.packType)
            {
                case PackType.PUZZLE_PACK:
                    menuScreen.menuController.GetOrAddScreen<PrePackPrompt>().Prompt(pack);

                    break;

                default:
                    menuScreen.menuController.GetOrAddScreen<VSGamePrompt>().Prompt(pack);

                    break;
            }
        }
    }
}