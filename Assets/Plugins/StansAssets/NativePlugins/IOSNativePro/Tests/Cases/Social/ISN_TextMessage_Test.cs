
using SA.Foundation.Tests;
using SA.iOS.Social;
using SA.Foundation.Utility;

namespace SA.iOS.Tests.Social
{
    public class ISN_TextMessage_Test : SA_BaseTest
    {
        public override void Test() {
            SA_ScreenUtil.TakeScreenshot((screenshot) => {
                ISN_TextMessage.Send("Hello Google", "+18773555787", screenshot, (result) => {
                    SetResult(SA_TestResult.OK);
                });
            });
        }
    }
}