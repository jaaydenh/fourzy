//@vadym udod

using Fourzy._Updates.Managers;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    [RequireComponent(typeof(LocalizedText))]
    public class SkillzRulesReadValue : MonoBehaviour
    {
        [SerializeField]
        private SkillzValues value;
        [SerializeField]
        private Color color;

        private LocalizedText localizedText;
        private string originalKey;

        private void Awake()
        {
            localizedText = GetComponent<LocalizedText>();
            originalKey = localizedText.key;
        }

        private void Start()
        {
            localizedText.UpdateLocale($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{GetValue()}</color>" + " {" + originalKey + "}");

            //check if this rule needs to be disabled
            if (Constants.SKILLZ_DEFAULT_GAMES_COUNT == 1 && value == SkillzValues.BIG_WIN_POINTS)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        public int GetValue()
        {
            switch (value)
            {
                case SkillzValues.WINNER_POINTS:
                    if (SkillzGameController.Instance.WinPoints == -1) {
                        return Constants.SKILLZ_WIN_POINTS;
                    } else {
                        return SkillzGameController.Instance.WinPoints;
                    }

                case SkillzValues.POINTS_PER_MOVE_LEFT_WIN:
                    if (SkillzGameController.Instance.PointsPerMoveLeftWin == -1) {
                        return Constants.SKILLZ_POINTS_PER_MOVE_LEFT_WIN;
                    } else {
                        return SkillzGameController.Instance.PointsPerMoveLeftWin;
                    }

                case SkillzValues.POINTS_PER_MOVE_LEFT_LOSE:
                    if (SkillzGameController.Instance.PointsPerMoveLeftLose == -1) {
                        return Constants.SKILLZ_POINTS_PER_MOVE_LEFT_LOSE;
                    } else {
                        return SkillzGameController.Instance.PointsPerMoveLeftLose;
                    }

                case SkillzValues.POINTS_PER_SECOND_LEFT:
                    if (SkillzGameController.Instance.PointsPerSecond == -1) {
                        return Constants.SKILLZ_POINTS_PER_SECOND_REMAINING;
                    } else {
                        return SkillzGameController.Instance.PointsPerSecond;
                    }

                case SkillzValues.DRAW_POINTS:
                    if (SkillzGameController.Instance.DrawPoints == -1) {
                        return Constants.SKILLZ_DRAW_POINTS;
                    } else {
                        return SkillzGameController.Instance.DrawPoints;
                    }

                case SkillzValues.BIG_WIN_POINTS:
                    if (SkillzGameController.Instance.WinAllGamesBonus == -1) {
                        return Constants.SKILLZ_WIN_ALL_GAMES_BONUS;
                    } else {
                        return SkillzGameController.Instance.WinAllGamesBonus;
                    }
            }

            return 0;
        }
    }

    public enum SkillzValues
    {
        WINNER_POINTS,
        POINTS_PER_MOVE_LEFT_WIN,
        POINTS_PER_MOVE_LEFT_LOSE,
        POINTS_PER_SECOND_LEFT,
        DRAW_POINTS,
        BIG_WIN_POINTS,
    }
}
