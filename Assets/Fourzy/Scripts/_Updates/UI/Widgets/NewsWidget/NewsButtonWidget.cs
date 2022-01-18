//@vadym udod

using Fourzy._Updates.UI.Helpers;

namespace Fourzy._Updates.UI.Widgets
{
    public class NewsButtonWidget : WidgetBase
    {
#if !MOBILE_SKILLZ
        protected override void Awake()
        {
            base.Awake();

            GameManager.onNewsFetched += OnNewsFetched;
        }

        protected void OnDestroy()
        {
            GameManager.onNewsFetched -= OnNewsFetched;
        }

        public override void _Update()
        {
            base._Update();

            OnNewsFetched();
        }

        private void OnNewsFetched()
        {
            int allNewsCount = GameManager.Instance.latestNews.Count;
            int unreadNews = GameManager.Instance.unreadNews.Count;

            button.GetBadge().badge.SetValue(unreadNews);
            button.SetState(allNewsCount > 0);
        }
#endif
    }
}
