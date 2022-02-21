//@vadym udod

using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    [RequireComponent(typeof(TMP_Text))]
    public class SkillzRulesMovesCounterDown : MonoBehaviour, ISKillzRulesPageComponent
    {
        [SerializeField]
        private SkillzRulesBoard board;
        [SerializeField]
        private int countFrom = 5;

        private TMP_Text label;

        private void Awake()
        {
            label = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            if (!board)
            {
                enabled = false;

                return;
            }

            board.onMoveEnded += OnMoveEnded;
        }

        public void OnPageOpened()
        {
            SetLabelCount(countFrom);
        }

        public void OnPageClosed() { }

        private void OnMoveEnded(int movesCount)
        {
            SetLabelCount(countFrom - movesCount);
        }

        private void SetLabelCount(int count)
        {
            label.text = count + "";
        }
    }
}
