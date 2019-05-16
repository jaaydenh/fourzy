using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Tests;
using SA.Foundation.Templates;

using SA.iOS.Social;
using SA.Foundation.Utility;


namespace SA.iOS.Tests.Social
{
    public class ISN_DefaultSharingDialog_Test : SA_BaseTest
    {
        public override void Test() {
            SA_ScreenUtil.TakeScreenshot((screenshot) => {

                ISN_UIActivityViewController controller = new ISN_UIActivityViewController();
                controller.SetText("share text");
                controller.AddImage(screenshot);

                controller.ExcludedActivityTypes.Add(ISN_UIActivityType.Print);
                controller.ExcludedActivityTypes.Add(ISN_UIActivityType.AssignToContact);
                controller.Present((result) => {

                    if(result.IsSucceeded) {
                        Debug.Log("Completed: " + result.Completed);
                        Debug.Log("ActivityType: " + result.ActivityType);
                    } else {
                        Debug.Log("ISN_UIActivityViewController error: " + result.Error.FullMessage);
                    }

                    SetAPIResult(result);

                });
            });
        }
    }
}