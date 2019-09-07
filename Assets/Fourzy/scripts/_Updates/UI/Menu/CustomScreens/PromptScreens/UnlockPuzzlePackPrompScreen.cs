//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Toasts;
using Fourzy._Updates.UI.Widgets;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class UnlockPuzzlePackPrompScreen : PromptScreen
    {
        public PuzzlePackWidget widget;

        private PuzzlePacksDataHolder.PuzzlePack puzzlePack;

        public void Prompt(PuzzlePacksDataHolder.PuzzlePack puzzlePack)
        {
            this.puzzlePack = puzzlePack;

            widget.SetData(this.puzzlePack);

            acceptButton.GetBadge("coins").badge.SetState(false);
            acceptButton.GetBadge("gems").badge.SetState(false);
            switch (puzzlePack.unlockRequirement)
            {
                case UnlockRequirementsEnum.COINS:
                    acceptButton.GetBadge("coins").badge.SetValue(puzzlePack.quantity);
                    break;

                case UnlockRequirementsEnum.GEMS:
                    acceptButton.GetBadge("gems").badge.SetValue(puzzlePack.quantity);
                    break;
            }

            Prompt(puzzlePack.name, "", "Unlock", "", null);
        }

        public override void Accept()
        {
            onAccept?.Invoke();
            onDecline = null;

            switch (puzzlePack.unlockRequirement)
            {
                case UnlockRequirementsEnum.COINS:
                    if (UserManager.Instance.coins >= puzzlePack.quantity)
                    {
                        //deduct coins

                        //unlock
                        PlayerPrefsWrapper.SetPuzzlePackUnlocked(puzzlePack.packID, true);
                        menuController.CloseCurrentScreen();
                    }
                    else
                    {
                        GamesToastsController.ShowTopToast("Not enough coins");
                        return;
                    }
                    break;

                case UnlockRequirementsEnum.GEMS:
                    if (UserManager.Instance.gems >= puzzlePack.quantity)
                    {
                        //deduct gems

                        //unlock
                        PlayerPrefsWrapper.SetPuzzlePackUnlocked(puzzlePack.packID, true);
                        menuController.CloseCurrentScreen();
                    }
                    else
                    {
                        GamesToastsController.ShowTopToast("Not enough gems");
                        return;
                    }
                    break;
            }
        }
    }
}