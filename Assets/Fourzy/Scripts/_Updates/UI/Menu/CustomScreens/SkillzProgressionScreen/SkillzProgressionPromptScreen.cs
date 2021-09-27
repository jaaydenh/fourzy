//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SkillzProgressionPromptScreen : PromptScreen
    {
        [SerializeField]
        private RectTransform progressContainer;
        [SerializeField]
        private VSGamePromptProgressionWidget progressPrefab;
        [SerializeField]
        private SkillzProgressionScreenPointsEntry pointsEntryPrefab;
        [SerializeField]
        private RectTransform pointsParent;

        [SerializeField]
        private Sprite gameWonSprite;
        [SerializeField]
        private Sprite gameLostSprite;
        [SerializeField]
        private Sprite right;
        [SerializeField]
        private Sprite middle;
        [SerializeField]
        private Sprite left;
        [SerializeField]
        private Sprite single;

        private List<SkillzProgressionScreenPointsEntry> pointsEntries = new List<SkillzProgressionScreenPointsEntry>();

        public void OpenSkillzprogressionPrompt(Action onClose = null)
        {
            //clear prev widgets
            foreach (WidgetBase widget in GetWidgets<VSGamePromptProgressionWidget>())
            {
                Destroy(widget.gameObject);
                widgets.Remove(widget);
            }

            for (int gameIndex = 0; gameIndex < SkillzGameController.Instance.GamesToPlay; gameIndex++)
            {
                Sprite sprite;

                //pick sprite
                if (SkillzGameController.Instance.GamesToPlay == 1)
                {
                    sprite = single;
                }
                else
                {
                    if (gameIndex == 0)
                    {
                        sprite = left;
                    }
                    else if (gameIndex == SkillzGameController.Instance.GamesToPlay - 1)
                    {
                        sprite = right;
                    }
                    else
                    {
                        sprite = middle;
                    }
                }

                AddProgressionWidget().SetSprite(sprite).SetChecked(gameIndex < SkillzGameController.Instance.GamesPlayed.Count ? 
                    (SkillzGameController.Instance.GamesPlayed[gameIndex].state ? gameWonSprite : gameLostSprite) : 
                    null);
            }

            //clera prev
            foreach (SkillzProgressionScreenPointsEntry entry in pointsEntries)
            {
                Destroy(entry.gameObject);
            }
            pointsEntries.Clear();

            //display points
            foreach (PointsEntry pointsEntry in SkillzGameController.Instance.GamesPlayed.Last().pointsEntries)
            {
                AddPointsWidget(LocalizationManager.Value(pointsEntry.name), pointsEntry.amount);
            }
            AddPointsWidget("Total", SkillzGameController.Instance.Points)
                .SetSize(48)
                .SetColor(Color.green);

            pointsParent.gameObject.SetActive(pointsEntries.Count > 0);

            //open screen
            Prompt("Game Complete", null, null, onClose);
        }

        public override void Decline(bool force = false)
        {
            if (SkillzGameController.Instance.HaveNextGame)
            {
                GamePlayManager.Instance.Rematch();
            }
            else
            {
                SkillzGameController.Instance.CloseGameOnBack = true;
                SkillzGameController.Instance.OnMatchFinished();
                SkillzCrossPlatform.ReturnToSkillz();
            }
        }

        private VSGamePromptProgressionWidget AddProgressionWidget()
        {
            VSGamePromptProgressionWidget widget = Instantiate(progressPrefab, progressContainer);
            widget.gameObject.SetActive(true);
            widgets.Add(widget);

            return widget;
        }

        private SkillzProgressionScreenPointsEntry AddPointsWidget(string name, int amount)
        {
            SkillzProgressionScreenPointsEntry _entry = Instantiate(pointsEntryPrefab, pointsParent);
            _entry.gameObject.SetActive(true);
            _entry.SetValues(name, amount);
            pointsEntries.Add(_entry);

            return _entry;
        }
    }
}
