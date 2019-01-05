//@vadym udod

using Fourzy._Updates.UI.Helpers;
using mixpanel;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    [RequireComponent(typeof(ButtonExtended))]
    public class PuzzlePackWidget : WidgetBase
    {
        public PuzzlePack puzzlePack { get; private set; }
        public ButtonExtended button { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            button = GetComponent<ButtonExtended>();
        }

        public void InitPuzzlePack(PuzzlePack puzzlePack, int toUnlock)
        {
            this.puzzlePack = puzzlePack;
            this.puzzlePack.ActiveLevel = GetNextLevel();

            button.SetLabel(puzzlePack.Name);

            if (toUnlock >= puzzlePack.CompletedToUnlock)
            {
                button.GetBadge().badge.SetValue("");
                button.SetLabel(puzzlePack.completedCount + " / " + puzzlePack.PuzzleChallengeLevels.Count.ToString(), "completed");
                button.interactable = true;
            }
            else
            {
                button.GetBadge().badge.SetValue(puzzlePack.CompletedToUnlock.ToString());
                button.SetLabel("", "completed");

                alphaTween.AtProgress(.75f);
            }
        }

        public void OpenPuzzlePack()
        {
            var props = new Value();
            props["ID"] = puzzlePack.ID;
            props["Name"] = puzzlePack.Name;
            Mixpanel.Track("Open Puzzle Pack", props);

            GameManager.Instance.SetActivePuzzlePack(puzzlePack);
            GameManager.Instance.OpenPuzzleChallengeGame("open");
        }

        private int GetNextLevel()
        {
            for (int i = 0; i < puzzlePack.PuzzleChallengeLevels.Count; i++)
                if (!PlayerPrefsWrapper.IsPuzzleChallengeCompleted(puzzlePack.PuzzleChallengeLevels[i].ID))
                    return i + 1;

            return 1;
        }
    }
}
