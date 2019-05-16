using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.iOS.XCode;


using SA.Foundation.Editor;
using SA.Foundation.Utility;
using SA.Foundation.UtilitiesEditor;

namespace SA.iOS
{
    public class ISN_MediaPlayerResolver : ISN_APIResolver
    {
        protected override ISN_XcodeRequirements GenerateRequirements() {
            var requirements = new ISN_XcodeRequirements();
            requirements.AddFramework(new ISD_Framework(ISD_iOSFramework.MediaPlayer));

          
            return requirements;
        }


        public override bool IsSettingsEnabled {
            get { return ISN_Settings.Instance.MediaPlayer; }
            set { 
                ISN_Settings.Instance.MediaPlayer = value;
            }
        }



        protected override string LibFolder { get { return "MediaPlayer/"; } }
        public override string DefineName { get { return "MEDIA_PLAYER_API_ENABLED"; } }

    
    }
}
