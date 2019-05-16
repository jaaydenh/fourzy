using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Tests;
using SA.Foundation.Templates;

using SA.iOS.UIKit;
using SA.iOS.Utilities;

namespace SA.iOS.Tests.UIKit
{
    public class ISN_UIImagePickerController_Test : SA_BaseTest
    {

        public override void Test() {
            var picker = new ISN_UIImagePickerController();
            picker.SourceType = ISN_UIImagePickerControllerSourceType.Album;
            picker.MediaTypes = new List<string>() { ISN_UIMediaType.IMAGE };

            picker.MaxImageSize = 512;
            picker.ImageCompressionFormat = ISN_UIImageCompressionFormat.JPEG;
            picker.ImageCompressionRate = 0.8f;

            picker.Present((result) => {
                if (result.IsSucceeded) {
                    if(!result.MediaType.Equals(ISN_UIMediaType.IMAGE)) {
                        SetResult(SA_TestResult.WithError("Wrong media type in callback"));
                        return;
                    }

                    if (result.Image == null) {
                        SetResult(SA_TestResult.WithError("No image"));
                        return;
                    }

                    if(string.IsNullOrEmpty(result.ImageURL)) {
                        SetResult(SA_TestResult.WithError("No image"));
                        return;
                    }
                    TestVideo();
                }  else {
                    SetAPIResult(result);
                }
               
            });
        }





        private void TestVideo() {
            var picker = new ISN_UIImagePickerController();
            picker.SourceType = ISN_UIImagePickerControllerSourceType.Album;
            picker.MediaTypes = new List<string>() { ISN_UIMediaType.MOVIE };

            picker.MaxImageSize = 512;
            picker.ImageCompressionFormat = ISN_UIImageCompressionFormat.JPEG;
            picker.ImageCompressionRate = 0.8f;

            picker.Present((result) => {
                if (result.IsSucceeded) {
                    if (!result.MediaType.Equals(ISN_UIMediaType.MOVIE)) {
                        SetResult(SA_TestResult.WithError("Wrong media type in callback"));
                        return;
                    }

                    if (result.Image == null) {
                        SetResult(SA_TestResult.WithError("No image"));
                        return;
                    }

                    if (string.IsNullOrEmpty(result.ImageURL)) {
                        SetResult(SA_TestResult.WithError("No image"));
                        return;
                    }

                }

                SetAPIResult(result);
            });
        }
    }
}