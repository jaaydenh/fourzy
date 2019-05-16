using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.iOS.XCode;

namespace SA.iOS 
{
    public class ISN_ReplayKitResolver : ISN_APIResolver
    {

        protected override ISN_XcodeRequirements GenerateRequirements() {
            var requirements = new ISN_XcodeRequirements();
            requirements.AddFramework(new ISD_Framework(ISD_iOSFramework.ReplayKit));
            return requirements;
        }
       

        protected override string LibFolder { get { return "ReplayKit/"; } }
        public override bool IsSettingsEnabled {
            get { return ISN_Settings.Instance.ReplayKit; }
            set { ISN_Settings.Instance.ReplayKit = value; }
        }
        public override string DefineName { get { return "REPLAY_KIT_API_ENABLED"; } }
    }
}

