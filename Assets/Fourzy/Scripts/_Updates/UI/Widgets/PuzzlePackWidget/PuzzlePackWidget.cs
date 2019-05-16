//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using Fourzy._Updates.UI.Toasts;
using mixpanel;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    [RequireComponent(typeof(ButtonExtended))]
    public class PuzzlePackWidget : WidgetBase
    {
        public Image bgImage;

        public PuzzlePacksDataHolder.PuzzlePack puzzlePack { get; private set; }
        public ButtonExtended button { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            button = GetComponent<ButtonExtended>();
        }

        public void SetData(PuzzlePacksDataHolder.PuzzlePack puzzlePack)
        {
            this.puzzlePack = puzzlePack;

            bgImage.sprite = puzzlePack.packBG;

            button.GetLabel().label.fontSharedMaterial = GameContentManager.Instance.puzzlePacksDataHolder.GetPuzzlePackFontMaterial(puzzlePack.outlineColor);
            button.SetLabel(puzzlePack.name);

            button.GetBadge("starsLocked").badge.SetState(false);
            button.GetBadge("coinsLocked").badge.SetState(false);
            button.GetBadge("gemsLocked").badge.SetState(false);

            switch (puzzlePack.unlockRequirement)
            {
                case PuzzlePacksDataHolder.UnlockRequirementsEnum.NONE:
                    DisplayProgression(puzzlePack);
                    break;

                case PuzzlePacksDataHolder.UnlockRequirementsEnum.STARS:
                    if (GameContentManager.Instance.puzzlePacksDataHolder.totalPuzzlesCompleteCount >= puzzlePack.quantity)
                        DisplayProgression(puzzlePack);
                    else
                        button.GetBadge("starsLocked").badge.SetValue(puzzlePack.quantity);
                    break;

                case PuzzlePacksDataHolder.UnlockRequirementsEnum.COINS:
                    if (PlayerPrefsWrapper.PuzzlePackUnlocked(puzzlePack.name))
                        DisplayProgression(puzzlePack);
                    else
                    {
                        //can be unlocked
                        button.interactable = true;
                        button.GetBadge("coinsLocked").badge.SetValue(puzzlePack.quantity);
                    }
                    break;

                case PuzzlePacksDataHolder.UnlockRequirementsEnum.GEMS:
                    if (PlayerPrefsWrapper.PuzzlePackUnlocked(puzzlePack.name))
                        DisplayProgression(puzzlePack);
                    else
                    {
                        //can be unlocked
                        button.interactable = true;
                        button.GetBadge("gemsLocked").badge.SetValue(puzzlePack.quantity);
                    }
                    break;
            }
        }

        public void OpenPuzzlePack()
        {
            //try to unlock
            switch (puzzlePack.unlockRequirement)
            {
                case PuzzlePacksDataHolder.UnlockRequirementsEnum.COINS:
                case PuzzlePacksDataHolder.UnlockRequirementsEnum.GEMS:
                    if (!PlayerPrefsWrapper.PuzzlePackUnlocked(puzzlePack.name))
                    {
                        //show unlock popup
                        menuScreen.menuController.GetScreen<UnlockPuzzlePackPrompScreen>().Prompt(puzzlePack);
                        return;
                    }
                    break;
            }

            var props = new Value();
            props["Name"] = puzzlePack.name;
            Mixpanel.Track("Open Puzzle Pack", props);

            ClientFourzyPuzzle puzzle = puzzlePack.nextUnsolvedPuzzle;

            if (puzzle != null)
                GameManager.Instance.StartGame(puzzle);
            else
                GamesToastsController.ShowTopToast("Empty puzzle pack");
        }

        private void DisplayProgression(PuzzlePacksDataHolder.PuzzlePack puzzlePack)
        {
            button.interactable = true;
            button.GetBadge("completeCounter").badge.SetValue($"{puzzlePack.puzzlesCompleteCount} / {puzzlePack.puzzlesEnabled.Count}");

            //finished
            button.GetBadge("finished").badge.SetState(puzzlePack.complete);

            //is opened
            button.GetBadge("new").badge.SetState(!PlayerPrefsWrapper.PuzzlePackOpened(puzzlePack.packID));
        }
    }
}
