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
        }

        public int GetValue()
        {
            switch (value)
            {
                case SkillzValues.WINNER_POINTS:
                    return SkillzGameController.Instance.WinPoints;

                case SkillzValues.POINTS_PER_MOVES_LEFT:
                    return SkillzGameController.Instance.PointsPerMoveLeftWin;

                case SkillzValues.POINTS_PER_SECOND_LEFT:
                    return SkillzGameController.Instance.PointsPerSecond;
            }

            return 0;
        }
    }

    public enum SkillzValues
    {
        WINNER_POINTS,
        POINTS_PER_MOVES_LEFT,
        POINTS_PER_SECOND_LEFT,
    }
}
