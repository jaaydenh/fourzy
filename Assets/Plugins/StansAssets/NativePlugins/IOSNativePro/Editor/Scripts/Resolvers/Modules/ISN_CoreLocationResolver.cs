
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.iOS.XCode;

namespace SA.iOS
{
    public class ISN_CoreLocationResolver : ISN_APIResolver
    {

        protected override ISN_XcodeRequirements GenerateRequirements() {
            var requirements = new ISN_XcodeRequirements();
            requirements.AddFramework(new ISD_Framework(ISD_iOSFramework.CoreLocation));

            var NSLocationWhenInUseUsageDescription = new ISD_PlistKey();
            NSLocationWhenInUseUsageDescription.Name = "NSLocationWhenInUseUsageDescription";
            NSLocationWhenInUseUsageDescription.StringValue = ISN_Settings.Instance.LocationWhenInUseUsageDescription;
            NSLocationWhenInUseUsageDescription.Type = ISD_PlistKeyType.String;
            requirements.AddInfoPlistKey(NSLocationWhenInUseUsageDescription);

            var NSLocationAlwaysAndWhenInUseUsageDescription = new ISD_PlistKey();
            NSLocationAlwaysAndWhenInUseUsageDescription.Name = "NSLocationAlwaysAndWhenInUseUsageDescription";
            NSLocationAlwaysAndWhenInUseUsageDescription.StringValue = ISN_Settings.Instance.LocationAlwaysAndWhenInUseUsageDescription;
            NSLocationAlwaysAndWhenInUseUsageDescription.Type = ISD_PlistKeyType.String;
            requirements.AddInfoPlistKey(NSLocationAlwaysAndWhenInUseUsageDescription);

            return requirements;
        }


        protected override string LibFolder { get { return "CoreLocation/"; } }
        public override bool IsSettingsEnabled {
            get { return ISN_Settings.Instance.CoreLocation; }
            set { ISN_Settings.Instance.CoreLocation = value; }

        }

        public override string DefineName { get { return "CORE_LOCATION_API_ENABLED"; } }

    }
}

