using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.iOS.XCode;
using SA.iOS.UIKit;

namespace SA.iOS 
{
    public class ISN_SocialResolver : ISN_LSApplicationQueriesSchemesResolver
    {

        protected override ISN_XcodeRequirements GenerateRequirements() {
            var requirements = new ISN_XcodeRequirements();

            requirements.AddFramework(new ISD_Framework(ISD_iOSFramework.Accounts));
            requirements.AddFramework(new ISD_Framework(ISD_iOSFramework.Social));
            requirements.AddFramework(new ISD_Framework(ISD_iOSFramework.MessageUI));



            ISD_PlistKey LSApplicationQueriesSchemes = new ISD_PlistKey();
            LSApplicationQueriesSchemes.Name = "LSApplicationQueriesSchemes";
            LSApplicationQueriesSchemes.Type = ISD_PlistKeyType.Array;


            requirements.AddInfoPlistKey(LSApplicationQueriesSchemes);

            ISD_PlistKey instagram = new ISD_PlistKey();
            instagram.StringValue = "instagram";
            instagram.Type = ISD_PlistKeyType.String;
            LSApplicationQueriesSchemes.AddChild(instagram);

            ISD_PlistKey whatsapp = new ISD_PlistKey();
            whatsapp.StringValue = "whatsapp";
            whatsapp.Type = ISD_PlistKeyType.String;
            LSApplicationQueriesSchemes.AddChild(whatsapp);


            return requirements;
        }


        protected override string LibFolder { get { return "Social/"; } }
        public override bool IsSettingsEnabled {
            get { return ISN_Settings.Instance.Social; }
            set { ISN_Settings.Instance.Social = value; }
        }
        public override string DefineName { get { return "SOCIAL_API_ENABLED"; } }
    }
}

