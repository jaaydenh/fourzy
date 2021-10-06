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

        private SkillzGameScreen skillzGameScreen;
        private List<SkillzProgressionScreenPointsEntry> pointsEntries = new List<SkillzProgressionScreenPointsEntry>();

        public void OpenSkillzprogressionPrompt()
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

            //clear prev
            foreach (SkillzProgressionScreenPointsEntry entry in pointsEntries)
            {
                Destroy(entry.gameObject);
            }
            pointsEntries.Clear();

            //display points
            if (!SkillzGameController.Instance.HaveNextGame)
            {
                AddPointsWidget("Game 1", SkillzGameController.Instance.GamesPlayed[0].Points);
                AddPointsWidget("Game 2", SkillzGameController.Instance.GamesPlayed[1].Points);
            }
            foreach (PointsEntry pointsEntry in SkillzGameController.Instance.GamesPlayed.Last().pointsEntries)
            {
                AddPointsWidget(LocalizationManager.Value(pointsEntry.name), pointsEntry.amount);
            }
            AddPointsWidget(SkillzGameController.Instance.HaveNextGame ? "" : "Total", SkillzGameController.Instance.HaveNextGame ? SkillzGameController.Instance.GamesPlayed.Last().Points : SkillzGameController.Instance.Points)
                .SetSize(48)
                .SetColor(Color.green);

            pointsParent.gameObject.SetActive(pointsEntries.Count > 0);

            //open screen
            Prompt("Game Complete", null, null, SkillzGameController.Instance.HaveNextGame ? LocalizationManager.Value("continue") : LocalizationManager.Value("submit_score"));
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
                SkillzCrossPlatform.ReturnToSkillz();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            skillzGameScreen = menuController.GetScreen<SkillzGameScreen>();
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
