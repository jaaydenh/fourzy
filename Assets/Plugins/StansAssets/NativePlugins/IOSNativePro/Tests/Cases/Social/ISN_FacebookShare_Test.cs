using SA.Foundation.Tests;
using SA.iOS.Social;
using SA.Foundation.Utility;

namespace SA.iOS.Tests.Social
{
    public class ISN_FacebookShare_Test : SA_BaseTest
    {
        public override void Test() {
            SA_ScreenUtil.TakeScreenshot((screenshot) => {
                ISN_Facebook.Post("share text", screenshot, (result) => {
                    SetAPIResult(result);
                });
            });
        }
    }
}