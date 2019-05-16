
using SA.Foundation.Tests;
using SA.iOS.Social;
using SA.Foundation.Utility;

namespace SA.iOS.Tests.Social
{
    public class ISN_InstagramShare_Test : SA_BaseTest
    {
        public override void Test() {
            SA_ScreenUtil.TakeScreenshot((screenshot) => {
                ISN_Instagram.Post(screenshot, (result) => {
                    SetAPIResult(result);
                });
            });
        }
    }
}