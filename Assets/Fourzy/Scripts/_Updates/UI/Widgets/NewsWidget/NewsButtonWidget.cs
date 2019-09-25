//@vadym udod

using Fourzy._Updates.UI.Helpers;

namespace Fourzy._Updates.UI.Widgets
{
    public class NewsButtonWidget : WidgetBase
    {
        protected ButtonExtended button;

        protected override void Awake()
        {
            base.Awake();

            GameManager.onNewsFetched += OnNewsFetched;
        }

        public override void _Update()
        {
            base._Update();

            OnNewsFetched();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            button = GetComponent<ButtonExtended>();
        }

        private void OnNewsFetched()
        {
            int allNewsCount = GameManager.Instance.latestNews.Count;
            int unreadNews = GameManager.Instance.unreadNews.Count;

            button.GetBadge().badge.SetValue(unreadNews);
            button.SetState(allNewsCount > 0);
        }
    }
}
