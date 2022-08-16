//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.UI.Widgets;
using System.Collections;
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
        private bool openFinalScreen = false;

        public override void Close(bool animate = true)
        {
            base.Close(animate);

            declineButton.GetBadge("timer").badge.SetState(false);
            CancelRoutine("timer");
        }

        public void OpenSkillzprogressionPrompt()
        {
            //clear prev widgets
            foreach (WidgetBase widget in GetWidgets<VSGamePromptProgressionWidget>())
            {
                Destroy(widget.gameObject);
                widgets.Remove(widget);
            }

            int gamesToPlay = SkillzGameController.Instance.GamesToPlay;
            string titleText = LocalizationManager.Value("game_complete");
            string buttonText = LocalizationManager.Value("next");

            if (gamesToPlay > 1)
            {
                progressContainer.gameObject.SetActive(true);

                // final screen option
                if (!SkillzGameController.Instance.HaveNextGame)
                {
                    openFinalScreen = true;
                }

                // init games progress widget
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

                    AddProgressionWidget()
                        .SetSprite(sprite)
                        .SetChecked(gameIndex < SkillzGameController.Instance.GamesPlayed.Count ?
                        (SkillzGameController.Instance.GamesPlayed[gameIndex].state ? gameWonSprite : gameLostSprite) :
                        null);
                }
            }
            else
            {
                progressContainer.gameObject.SetActive(false);

                titleText = LocalizationManager.Value("final_score");
                buttonText = LocalizationManager.Value("submit_score");
            }

            ClearPointsEntries();

            SkillzGameResult lastGameResult = SkillzGameController.Instance.GamesPlayed.FindLast(entries => entries.Points > 0);
            if (lastGameResult != null)
            {
                //display points
                foreach (PointsEntry pointsEntry in lastGameResult.pointsEntries)
                {
                    AddPointsWidget(pointsEntry.name, pointsEntry.amount);
                }
                AddPointsWidget("", lastGameResult.Points)
                    .SetSize(48)
                    .SetColor(Color.green);
            }

            pointsParent.gameObject.SetActive(pointsEntries.Count > 0);

            //start timer routine
            StartRoutine("timer", TimerRoutine(Constants.SKILLZ_PROGRESSION_POPUP_WAIT_TIME), () => Decline(), null);

            //open screen
            Prompt(titleText, null, null, buttonText);
        }

        public override void Decline(bool force = false)
        {
            if (openFinalScreen)
            {
                StartCoroutine(CloseAndOpenTotal());
            }
            else
            {
                GamePlayManager.Instance.StartNextGame();
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

        private void ClearPointsEntries()
        {
            //clear prev
            foreach (SkillzProgressionScreenPointsEntry entry in pointsEntries)
            {
                Destroy(entry.gameObject);
            }
            pointsEntries.Clear();
        }

        private void AddTotalToPrompt()
        {
            for (int gameIndex = 0; gameIndex < SkillzGameController.Instance.GamesPlayed.Count; gameIndex++)
            {
                AddPointsWidget($"{LocalizationManager.Value("game")} {gameIndex + 1}", SkillzGameController.Instance.GamesPlayed[gameIndex].Points);
            }

            AddPointsWidget(LocalizationManager.Value("total"), SkillzGameController.Instance.Points)
                .SetSize(48)
                .SetColor(Color.green);
        }

        private IEnumerator TimerRoutine(int time)
        {
            declineButton.GetBadge("timer").badge.SetValue(time);
            for (int seconds = time; seconds >= 0; seconds--)
            {
                yield return new WaitForSeconds(1f);
                declineButton.GetBadge("timer").badge.SetValue(seconds);
            }
        }

        private IEnumerator CloseAndOpenTotal()
        {
            SetInteractable(false);
            Close(true);

            yield return new WaitForSeconds(.4f);

            promptTitle.text = LocalizationManager.Value("final_score");
            UpdateDeclineButton(LocalizationManager.Value("submit_score"));

            SetInteractable(true);
            ClearPointsEntries();
            AddTotalToPrompt();
            Open();

            //start timer routine
            StartRoutine("timer", TimerRoutine(Constants.SKILLZ_PROGRESSION_POPUP_FINAL_WAIT_TIME), () => Decline());

            openFinalScreen = false;
        }
    }
}
