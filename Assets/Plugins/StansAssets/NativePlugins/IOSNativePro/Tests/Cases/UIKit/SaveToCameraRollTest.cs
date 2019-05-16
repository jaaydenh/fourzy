using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Tests;
using SA.Foundation.Templates;

using SA.iOS.UIKit;
using SA.iOS.Utilities;
using SA.Foundation.Utility;

namespace SA.iOS.Tests.UIKit
{
    public class SaveToCameraRollTest : SA_BaseTest {

        public override void Test() {
            SA_ScreenUtil.TakeScreenshot((tex) => {
                ISN_UIImagePickerController.SaveTextureToCameraRoll(tex, (result) => {
                    SetAPIResult(result); 
                });
            });
        }
    }
}