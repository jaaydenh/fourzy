
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.iOS.XCode;

namespace SA.iOS
{
    public class ISN_PhotosResolver : ISN_APIResolver
    {

        protected override ISN_XcodeRequirements GenerateRequirements() {
            var requirements = new ISN_XcodeRequirements();
            requirements.AddFramework(new ISD_Framework(ISD_iOSFramework.Photos));
            return requirements;
        }


        protected override string LibFolder { get { return "Photos/"; } }
        public override bool IsSettingsEnabled {
            get { return ISN_Settings.Instance.Photos; }
            set { ISN_Settings.Instance.Photos = value; }

        }
        public override string DefineName { get { return "PHOTOS_API_ENABLED"; } }
    }
}

