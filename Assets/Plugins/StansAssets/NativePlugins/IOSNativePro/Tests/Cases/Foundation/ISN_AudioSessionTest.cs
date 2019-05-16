using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Async;

using SA.iOS.AVFoundation;


namespace SA.iOS.Tests.Foundation
{
    public class ISN_AudioSessionTest : SA_BaseTest
    {

        public override void Test() {


            ISN_AVAudioSession.SetActive(true);

            var category = ISN_AVAudioSession.Category;

            ISN_AVAudioSession.SetCategory(ISN_AVAudioSessionCategory.Playback);
            ISN_AVAudioSession.SetCategory(category);

            ISN_AVAudioSession.SetActive(false);

            SetResult(SA_TestResult.OK);
        }
    }
}