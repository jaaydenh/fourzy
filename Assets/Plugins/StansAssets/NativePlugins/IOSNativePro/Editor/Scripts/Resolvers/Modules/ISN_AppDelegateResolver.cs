using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.iOS.XCode;
using SA.iOS.UIKit;


namespace SA.iOS
{
    public class ISN_AppDelegateResolver : ISN_APIResolver
    {

        protected override ISN_XcodeRequirements GenerateRequirements() {
            var requirements = new ISN_XcodeRequirements();

            if (ISN_Settings.Instance.ShortcutItems.Count > 0 ) {

                ISD_PlistKey UIApplicationShortcutItems = new ISD_PlistKey();
                UIApplicationShortcutItems.Name = "UIApplicationShortcutItems";
                UIApplicationShortcutItems.Type = ISD_PlistKeyType.Array;

                requirements.AddInfoPlistKey(UIApplicationShortcutItems);

                foreach (var shortcut in ISN_Settings.Instance.ShortcutItems) {
                    var ShortcutItem = new ISD_PlistKey();
                    ShortcutItem.Type = ISD_PlistKeyType.Dictionary;
                    UIApplicationShortcutItems.AddChild(ShortcutItem);

                    var ShortcutItemTitle = new ISD_PlistKey();
                    ShortcutItemTitle.Name = "UIApplicationShortcutItemTitle";
                    ShortcutItemTitle.StringValue = shortcut.Title;
                    ShortcutItem.AddChild(ShortcutItemTitle);

                    var ShortcutItemSubtitle = new ISD_PlistKey();
                    ShortcutItemSubtitle.Name = "UIApplicationShortcutItemSubtitle";
                    ShortcutItemSubtitle.StringValue = shortcut.Subtitle;
                    ShortcutItem.AddChild(ShortcutItemSubtitle);


                    var ShortcutItemType = new ISD_PlistKey();
                    ShortcutItemType.Name = "UIApplicationShortcutItemType";
                    ShortcutItemType.StringValue = shortcut.Type;
                    ShortcutItem.AddChild(ShortcutItemType);

                }
            }


            if (ISN_Settings.Instance.UrlTypes.Count > 0) {

                ISD_PlistKey CFBundleURLTypes = new ISD_PlistKey();
                CFBundleURLTypes.Name = "CFBundleURLTypes";
                CFBundleURLTypes.Type = ISD_PlistKeyType.Array;

                requirements.AddInfoPlistKey(CFBundleURLTypes);


                foreach (ISN_UIUrlType url in ISN_Settings.Instance.UrlTypes) {
                    ISD_PlistKey URLTypeHolder = new ISD_PlistKey();
                    URLTypeHolder.Type = ISD_PlistKeyType.Dictionary;
                    CFBundleURLTypes.AddChild(URLTypeHolder);


                    ISD_PlistKey CFBundleURLName = new ISD_PlistKey();
                    CFBundleURLName.Type = ISD_PlistKeyType.String;
                    CFBundleURLName.Name = "CFBundleURLName";
                    CFBundleURLName.StringValue = url.Identifier;
                    URLTypeHolder.AddChild(CFBundleURLName);


                    ISD_PlistKey CFBundleURLSchemes = new ISD_PlistKey();
                    CFBundleURLSchemes.Type = ISD_PlistKeyType.Array;
                    CFBundleURLSchemes.Name = "CFBundleURLSchemes";
                    URLTypeHolder.AddChild(CFBundleURLSchemes);

                    foreach (string scheme in url.Schemes) {
                        ISD_PlistKey Scheme = new ISD_PlistKey();
                        Scheme.Type = ISD_PlistKeyType.String;
                        Scheme.StringValue = scheme;
                        CFBundleURLSchemes.AddChild(Scheme);
                    }

                }
            }


            return requirements;
        }


        public override bool IsSettingsEnabled {
            get { return ISN_Settings.Instance.AppDelegate; }
            set { 
                ISN_Settings.Instance.AppDelegate = value;
                if(!ISN_Settings.Instance.AppDelegate) {
                    ISN_Settings.Instance.UserNotifications = false;
                }
            }
        }
        protected override string LibFolder { get { return "AppDelegate/"; } }
        public override string DefineName { get { return "APP_DELEGATE_ENABLED"; } }


       

        public override void RunAdditionalPreprocess() {

            if (!IsSettingsEnabled) { return; }

            var unResolver = ISN_Preprocessor.GetResolver<ISN_UserNotificationsResolver>();

            //We assume App Delegate is always avaliable with User notifications
            string ISN_UIApplicationDelegate_mm = ISN_Settings.IOS_NATIVE_XCODE + LibFolder +"ISN_UIApplicationDelegate.mm";
            ISN_Preprocessor.ChangeFileDefine(ISN_UIApplicationDelegate_mm, unResolver.DefineName, unResolver.IsSettingsEnabled);

        }
    }
}