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
                    //return SkillzGameController.Instance.WinPoints;
                    return Constants.SKILLZ_WIN_POINTS;

                case SkillzValues.POINTS_PER_MOVES_LEFT:
                    //return SkillzGameController.Instance.PointsPerMoveLeftWin;
                    return Constants.SKILLZ_POINTS_PER_MOVE_LEFT_WIN;

                case SkillzValues.POINTS_PER_SECOND_LEFT:
                    //return SkillzGameController.Instance.PointsPerSecond;
                    return Constants.SKILLZ_POINTS_PER_SECOND_REMAINING;

                case SkillzValues.DRAW_POINTS:
                    return Constants.SKILLZ_DRAW_POINTS;

                case SkillzValues.BIG_WIN_POINTS:
                    return Constants.SKILLZ_WIN_ALL_GAMES_BONUS;
            }

            return 0;
        }
    }

    public enum SkillzValues
    {
        WINNER_POINTS,
        POINTS_PER_MOVES_LEFT,
        POINTS_PER_SECOND_LEFT,
        DRAW_POINTS,
        BIG_WIN_POINTS,
    }
}
