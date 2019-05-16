using SA.Foundation.Tests;
using SA.iOS.Social;
using SA.Foundation.Utility;

namespace SA.iOS.Tests.Social
{
    public class ISN_MailShare_Test : SA_BaseTest
    {
        public override void Test() {
            SA_ScreenUtil.TakeScreenshot((screenshot) => {
            ISN_Mail.Send("Mail Subject", "Mail Body", "mail@gmail.com", screenshot, (result) => {
                    SetAPIResult(result);
                });
            });
        }
    }
}