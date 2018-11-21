//@vadym udod

using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GameFinishedScreen : MenuScreen
    {
        public TextMeshProUGUI label;
        public GameObject rewardButton;

        public void Open(string text, bool showRewardButton)
        {
            label.text = text;

            rewardButton.SetActive(showRewardButton);
        }
    }
}