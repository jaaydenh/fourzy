using UnityEngine;
using UnityEngine.UI;
using HedgehogTeam.EasyTouch;

namespace Fourzy
{
    public class View5 : UIView
    {
        //Instance
        public static View5 instance;
        public Text winTabText;
        public Text coinsEarnedTabText;

        void Start()
        {
            instance = this;
            keepHistory = true;
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void ShowAnimated(AnimationDirection sourceDirection)
        {
            ViewController.instance.SetActiveView(TotalView.view5);
            base.ShowAnimated(sourceDirection);
            LeaderboardManager.instance.GetWinsLeaderboard();
        }

        public override void HideAnimated(AnimationDirection getAwayDirection)
        {
            base.HideAnimated(getAwayDirection);
        }

        public void WinsButton() 
        {
            LeaderboardManager.instance.GetWinsLeaderboard();
            coinsEarnedTabText.fontSize = 35;
            winTabText.fontSize = 40;
            winTabText.GetComponent<Outline>().enabled = true;
            coinsEarnedTabText.GetComponent<Outline>().enabled = false;
        }

        public void CoinsEarnedButton() 
        {
            LeaderboardManager.instance.GetCoinsEarnedLeaderboard();
            coinsEarnedTabText.fontSize = 40;
            winTabText.fontSize = 35;
            winTabText.GetComponent<Outline>().enabled = false;
            coinsEarnedTabText.GetComponent<Outline>().enabled = true;
        }
    }
}
