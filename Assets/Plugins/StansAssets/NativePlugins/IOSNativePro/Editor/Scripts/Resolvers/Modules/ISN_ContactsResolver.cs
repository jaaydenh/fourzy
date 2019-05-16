using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.iOS.XCode;

namespace SA.iOS 
{
    public class ISN_ContactsResolver : ISN_APIResolver
    {

        protected override ISN_XcodeRequirements GenerateRequirements() {
            var requirements = new ISN_XcodeRequirements();
            requirements.AddFramework(new ISD_Framework(ISD_iOSFramework.Contacts));
            requirements.AddFramework(new ISD_Framework(ISD_iOSFramework.ContactsUI));

            var NSContactsUsageDescription = new ISD_PlistKey();
            NSContactsUsageDescription.Name = "NSContactsUsageDescription";
            NSContactsUsageDescription.StringValue = ISN_Settings.Instance.ContactsUsageDescription;
            NSContactsUsageDescription.Type = ISD_PlistKeyType.String;

            requirements.AddInfoPlistKey(NSContactsUsageDescription);
           

            return requirements;
        }
       

        protected override string LibFolder { get { return "Contacts/"; } }
        public override bool IsSettingsEnabled {
            get { return ISN_Settings.Instance.Contacts; }
            set { ISN_Settings.Instance.Contacts = value; }
        }
        public override string DefineName { get { return "CONTACTS_API_ENABLED"; } }
    }
}

