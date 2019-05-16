
using SA.Foundation.Tests;
using SA.iOS.Social;
using SA.Foundation.Utility;

namespace SA.iOS.Tests.Social
{
    public class ISN_TwitterShare_Test : SA_BaseTest
    {
        public override void Test() {
            SA_ScreenUtil.TakeScreenshot((screenshot) => {
                ISN_Twitter.Post("share text", screenshot, (result) => {
                    SetAPIResult(result);
                });
            });
        }
    }
}