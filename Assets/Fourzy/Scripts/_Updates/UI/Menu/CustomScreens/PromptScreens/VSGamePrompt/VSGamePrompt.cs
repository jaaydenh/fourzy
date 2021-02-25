//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Widgets;
using System;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class VSGamePrompt : PromptScreen
    {
        public Color defaultLeftColor;
        public Color defaultRightColor;

        public VSGamePromptProfileWidget playerProfileWidget;
        public VSGamePromptProfileWidget aiProfileWidget;

        public RectTransform progressContainer;
        public VSGamePromptProgressionWidget progressPrefab;

        public Sprite right;
        public Sprite middle;
        public Sprite left;

        public BasicPuzzlePack puzzlePack { get; private set; }

        public void Prompt(int gauntletLevels) => Prompt(new BasicPuzzlePack(gauntletLevels));

        public void Prompt(BasicPuzzlePack puzzlePack, Action onDeny = null, Action onAccept = null)
        {
            this.puzzlePack = puzzlePack;

            //spawn UI elements
            playerProfileWidget
                .SetProfile(UserManager.Instance.meAsPlayer)
                .SetColor(defaultLeftColor);

            aiProfileWidget
                .SetProfile(puzzlePack.puzzlePlayer)
                .SetColor(defaultRightColor);

            foreach (WidgetBase widget in GetWidgets<VSGamePromptProgressionWidget>()) Destroy(widget.gameObject);
            widgets.Clear();

            for (int puzzleIndex = 0; puzzleIndex < puzzlePack.enabledPuzzlesData.Count; puzzleIndex++)
            {
                Sprite sprite = null;

                //pick sprite
                if (puzzleIndex == 0) sprite = left;
                else if (puzzleIndex == puzzlePack.enabledPuzzlesData.Count - 1 && puzzlePack.allRewards.Count == 0) sprite = right;
                else sprite = middle;

                AddWidget()
                    .SetSprite(sprite)
                    .SetData(puzzlePack, puzzlePack.enabledPuzzlesData[puzzleIndex]);
            }

            if (puzzlePack.allRewards.Count > 0)
            {
                AddWidget()
                    .SetSprite(right)
                    .SetReward(puzzlePack, puzzlePack.enabledPuzzlesData.Last(), puzzlePack.enabledPuzzlesData.Last().rewards[0]);
            }

            Prompt(
                $"{LocalizationManager.Value("defeat")} {puzzlePack.aiPlayerName}",
                null,
                onAccept ?? (() =>
                {
                    CloseSelf();
                    puzzlePack.StartNextUnsolvedPuzzle(/*GameManager.Instance.activeGame*/);
                }),
                onDeny ?? (() => menuController.CloseCurrentScreen()));
        }

        private VSGamePromptProgressionWidget AddWidget()
        {
            VSGamePromptProgressionWidget widget = Instantiate(progressPrefab, progressContainer);
            widget.gameObject.SetActive(true);
            widgets.Add(widget);

            return widget;
        }
    }
}