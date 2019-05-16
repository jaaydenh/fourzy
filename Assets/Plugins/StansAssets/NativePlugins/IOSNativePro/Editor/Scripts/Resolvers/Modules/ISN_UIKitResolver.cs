using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.iOS.XCode;


namespace SA.iOS 
{
    public class ISN_UIKitResolver : ISN_LSApplicationQueriesSchemesResolver
    {

        protected override ISN_XcodeRequirements GenerateRequirements() {
            var requirements = new ISN_XcodeRequirements();


            var property = new ISD_BuildProperty("GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
            requirements.AddBuildProperty(property);



            if (ISN_Settings.Instance.ApplicationQueriesSchemes.Count > 0) {
                ISD_PlistKey LSApplicationQueriesSchemes = new ISD_PlistKey();
                LSApplicationQueriesSchemes.Name = "LSApplicationQueriesSchemes";
                LSApplicationQueriesSchemes.Type = ISD_PlistKeyType.Array;

                requirements.AddInfoPlistKey(LSApplicationQueriesSchemes);


                foreach (var scheme in ISN_Settings.Instance.ApplicationQueriesSchemes) {
                    ISD_PlistKey schemeName = new ISD_PlistKey();
                    schemeName.StringValue = scheme.Identifier;
                    schemeName.Type = ISD_PlistKeyType.String;
                    LSApplicationQueriesSchemes.AddChild(schemeName);
                }

            }



            var NSCameraUsageDescription = new ISD_PlistKey();
            NSCameraUsageDescription.Name = "NSCameraUsageDescription";
            NSCameraUsageDescription.StringValue = ISN_Settings.Instance.CameraUsageDescription;
            NSCameraUsageDescription.Type = ISD_PlistKeyType.String;
            requirements.AddInfoPlistKey(NSCameraUsageDescription);

            
            var NSPhotoLibraryUsageDescription = new ISD_PlistKey();
            NSPhotoLibraryUsageDescription.Name = "NSPhotoLibraryUsageDescription";
            NSPhotoLibraryUsageDescription.StringValue = ISN_Settings.Instance.PhotoLibraryUsageDescription;
            NSPhotoLibraryUsageDescription.Type = ISD_PlistKeyType.String;
            requirements.AddInfoPlistKey(NSPhotoLibraryUsageDescription);



            var NSPhotoLibraryAddUsageDescription = new ISD_PlistKey();
            NSPhotoLibraryAddUsageDescription.Name = "NSPhotoLibraryAddUsageDescription";
            NSPhotoLibraryAddUsageDescription.StringValue = ISN_Settings.Instance.PhotoLibraryAddUsageDescription;
            NSPhotoLibraryAddUsageDescription.Type = ISD_PlistKeyType.String;
            requirements.AddInfoPlistKey(NSPhotoLibraryAddUsageDescription);


            var NSMicrophoneUsageDescription = new ISD_PlistKey();
            NSMicrophoneUsageDescription.Name = "NSMicrophoneUsageDescription";
            NSMicrophoneUsageDescription.StringValue = ISN_Settings.Instance.MicrophoneUsageDescription;
            NSMicrophoneUsageDescription.Type = ISD_PlistKeyType.String;
            requirements.AddInfoPlistKey(NSMicrophoneUsageDescription);



            return requirements;
        }
       


        public override bool IsSettingsEnabled { get { return true; } set { } }
        protected override string LibFolder { get { return string.Empty; } }
        public override string DefineName { get { return string.Empty; } }
    }
}

