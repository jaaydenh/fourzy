////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Deploy
// @author Stanislav Osipov (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

#if (UNITY_IOS || UNITY_TVOS) && !ISD_DISABLED

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

using SA.Foundation.Utility;
using SA.Foundation.UtilitiesEditor;

namespace SA.iOS.XCode
{

    public class ISD_PostProcess
    {


        [PostProcessBuild(100)]
        public static void OnPostprocessBuild(BuildTarget target, string projectPath) {
            
            var pbxProjPath = PBXProject.GetPBXProjectPath(projectPath); 


            PBXProject proj = new PBXProject();
            proj.ReadFromFile(pbxProjPath);
            string targetGuid = proj.TargetGuidByName("Unity-iPhone");


            RegisterAppLanguages();

            AddFlags(proj, targetGuid);
            AddLibraries(proj, targetGuid);
            AddFrameworks(proj, targetGuid, target);
            AddEmbededFrameworks(proj, targetGuid);
            AddPlistVariables(proj, projectPath, targetGuid);
            ApplyBuildSettings(proj, targetGuid);
            CopyAssetFiles(proj, projectPath, targetGuid);
            AddShellScriptBuildPhase(proj, targetGuid);
                    
            proj.WriteToFile(pbxProjPath);


			var capManager = new ProjectCapabilityManager(pbxProjPath, ISD_Settings.ENTITLEMENTS_FILE_NAME, "Unity-iPhone");
			AddCapabilities(proj, targetGuid, capManager);
			capManager.WriteToFile();
            


            //Some API simply does not work so on this block we are applying a workaround
            //after Unity deploy scrips has stopped working

            //Addons EmbededFrameworks
            foreach (var framework in ISD_Settings.Instance.EmbededFrameworks) {
                string contents = File.ReadAllText(pbxProjPath);
                string pattern = "(?<=Embed Frameworks)(?:.*)(\\/\\* " + framework.FileName + "\\ \\*\\/)(?=; };)";
                string oldText = "/* " + framework.FileName + " */";
                string updatedText = "/* " + framework.FileName + " */; settings = {ATTRIBUTES = (CodeSignOnCopy, ); }";
                contents = Regex.Replace(contents, pattern, m => m.Value.Replace(oldText, updatedText));
                File.WriteAllText(pbxProjPath, contents);
            }


            var entitlementsPath = projectPath + "/" + ISD_Settings.ENTITLEMENTS_FILE_NAME;
            if(ISD_Settings.Instance.EntitlementsMode == ISD_EntitlementsGenerationMode.Automatic) {
                if (ISD_Settings.Instance.Capability.iCloud.Enabled) {

                    if (ISD_Settings.Instance.Capability.iCloud.keyValueStorage && !ISD_Settings.Instance.Capability.iCloud.iCloudDocument) {
                        var entitlements = new PlistDocument();
                        entitlements.ReadFromFile(entitlementsPath);

                        var plistVariable = new PlistElementArray();
                        entitlements.root["com.apple.developer.icloud-container-identifiers"] = plistVariable;

                        entitlements.WriteToFile(entitlementsPath);
                    }
                }
            } else {
                if(ISD_Settings.Instance.EntitlementsFile != null) {
                    var entitlementsContentPath = SA_AssetDatabase.GetAbsoluteAssetPath(ISD_Settings.Instance.EntitlementsFile);
                    string contents = File.ReadAllText(entitlementsContentPath);
                    File.WriteAllText(entitlementsPath, contents);
                } else {
                    Debug.LogWarning("ISD: EntitlementsMode set to Manual but no file provided");
                }
            }





        }

        static void AddPlistVariables(PBXProject proj, string pathToBuiltProject, string targetGUID) {
            var infoPlist = new PlistDocument();
            var infoPlistPath = pathToBuiltProject + "/Info.plist";
            infoPlist.ReadFromFile(infoPlistPath);

            foreach(var variable in ISD_Settings.Instance.PlistVariables) {

               PlistElement plistVariable = null;
               switch (variable.Type) {
                    case ISD_PlistKeyType.String:
                        plistVariable = new PlistElementString(variable.StringValue);
                        break;
                    case ISD_PlistKeyType.Integer:
                        plistVariable = new PlistElementInteger(variable.IntegerValue);
                        break;
                    case ISD_PlistKeyType.Boolean:
                        plistVariable = new PlistElementBoolean(variable.BooleanValue);
                        break;
                    case ISD_PlistKeyType.Array:
                        plistVariable = CreatePlistArray(variable);
                        break;
                    case ISD_PlistKeyType.Dictionary:
                        plistVariable = CreatePlistDict(variable);
                        break;


                }

                infoPlist.root[variable.Name] = plistVariable;
            }

            infoPlist.WriteToFile(infoPlistPath);



           

        }


        static PlistElementArray CreatePlistArray(ISD_PlistKey variable, PlistElementArray array = null) {

            if(array == null) {
                array = new PlistElementArray();
            }

            foreach (string variableUniqueIdKey in variable.ChildrensIds) {
                ISD_PlistKey v = ISD_Settings.Instance.getVariableById(variableUniqueIdKey);

                switch(v.Type) {
                    case ISD_PlistKeyType.String:
                        array.AddString(v.StringValue);
                        break;
                    case ISD_PlistKeyType.Boolean:
                        array.AddBoolean(v.BooleanValue);
                        break;
                    case ISD_PlistKeyType.Integer:
                        array.AddInteger(v.IntegerValue);
                        break;
                    case ISD_PlistKeyType.Array:
                        CreatePlistArray(v, array.AddArray());
                        break;
                    case ISD_PlistKeyType.Dictionary:
                        CreatePlistDict(v, array.AddDict());
                        break;
                }
            }

            return array;
        }

        static PlistElementDict CreatePlistDict(ISD_PlistKey variable, PlistElementDict dict = null) {

            if (dict == null) {
                dict = new PlistElementDict();
            }

            foreach (string variableUniqueIdKey in variable.ChildrensIds) {
                ISD_PlistKey v = ISD_Settings.Instance.getVariableById(variableUniqueIdKey);

                switch (v.Type) {
                    case ISD_PlistKeyType.String:
                        dict.SetString(v.Name, v.StringValue);
                        break;
                    case ISD_PlistKeyType.Boolean:
                        dict.SetBoolean(v.Name, v.BooleanValue);
                        break;
                    case ISD_PlistKeyType.Integer:
                        dict.SetInteger(v.Name, v.IntegerValue);
                        break;
                    case ISD_PlistKeyType.Array:
                        var array = dict.CreateArray(v.Name);
                        CreatePlistArray(v, array);
                        break;
                    case ISD_PlistKeyType.Dictionary:
                        var d = dict.CreateDict(v.Name);
                        CreatePlistDict(v, d);
                        break;
                }
            }

            return dict;
        }




        static void ApplyBuildSettings(PBXProject proj, string targetGUID) {
            foreach(var property in ISD_Settings.Instance.BuildProperties) {
                proj.SetBuildProperty(targetGUID, property.Name, property.Value);
            }
        }

        static void AddFlags(PBXProject proj, string targetGuid) {
         
            foreach(var flag in ISD_Settings.Instance.Flags) {
                if(flag.Type == ISD_FlagType.LinkerFlag) {
                    proj.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", flag.Name);
                }

                if (flag.Type == ISD_FlagType.LinkerFlag) {
                    proj.AddBuildProperty(targetGuid, "OTHER_CFLAGS", flag.Name);
                }
            }
            
           
           
        }


        static void RegisterAppLanguages() {

            //We have nothing to register, no point to add en empty CFBundleLocalizations key.
            if(ISD_Settings.Instance.Languages.Count == 0) {
                return;
            }

            var CFBundleLocalizations = new ISD_PlistKey();
            CFBundleLocalizations.Name = ISD_Settings.CF_LOCLIZATIONS_PLIST_KEY;
            CFBundleLocalizations.Type = ISD_PlistKeyType.Array;

            foreach (var lang in ISD_Settings.Instance.Languages) {
                var langItem = new ISD_PlistKey();
                langItem.Type = ISD_PlistKeyType.String;
                langItem.StringValue = lang.Name;
                CFBundleLocalizations.AddChild(langItem);
            }

            ISD_API.SetInfoPlistKey(CFBundleLocalizations);
        }


        static void AddCapabilities(PBXProject proj, string targetGuid, ProjectCapabilityManager capManager) {



            var capability = ISD_Settings.Instance.Capability;

            if (capability.iCloud.Enabled) {

                if (capability.iCloud.iCloudDocument || capability.iCloud.customContainers.Count > 0) {
                    capManager.AddiCloud(capability.iCloud.keyValueStorage, capability.iCloud.iCloudDocument, capability.iCloud.customContainers.ToArray());   
                } else {
                    capManager.AddiCloud(capability.iCloud.keyValueStorage, false, null);
                }


            }

            if(capability.PushNotifications.Enabled) {
                capManager.AddPushNotifications(capability.PushNotifications.development);
            }

            if(capability.GameCenter.Enabled) {
                capManager.AddGameCenter();
            }

            if(capability.Wallet.Enabled) {
                capManager.AddWallet(capability.Wallet.passSubset.ToArray());
            }

            if(capability.Siri.Enabled) {
                capManager.AddSiri();
            }

            if(capability.ApplePay.Enabled) {
                capManager.AddApplePay(capability.ApplePay.merchants.ToArray());
            }

            if(capability.InAppPurchase.Enabled) {
                capManager.AddInAppPurchase();
            }

            if (capability.Maps.Enabled) {
                var options = MapsOptions.None;
                foreach(var opt in capability.Maps.options) {
                    MapsOptions opt2 = (MapsOptions) opt;
                    options |= opt2;
                }
                capManager.AddMaps(options);
            }

            if (capability.PersonalVPN.Enabled) {
                capManager.AddPersonalVPN();
            }

            if (capability.BackgroundModes.Enabled) {
                var options = BackgroundModesOptions.None;
                foreach (var opt in capability.BackgroundModes.options) {
                    BackgroundModesOptions opt2 = (BackgroundModesOptions)opt;
                    options |= opt2;
                }
                capManager.AddBackgroundModes(options);
            }


            if (capability.InterAppAudio.Enabled) {
                capManager.AddInterAppAudio();
            }

            if (capability.KeychainSharing.Enabled) {
                capManager.AddKeychainSharing(capability.KeychainSharing.accessGroups.ToArray());
            }

            if (capability.AssociatedDomains.Enabled) {
                capManager.AddAssociatedDomains(capability.AssociatedDomains.domains.ToArray());
            }

            if (capability.AppGroups.Enabled) {
                capManager.AddAppGroups(capability.AppGroups.groups.ToArray());
            }

            if (capability.DataProtection.Enabled) {
                capManager.AddDataProtection();
            }

            if (capability.HomeKit.Enabled) {
                capManager.AddHomeKit();
            }

            if (capability.HealthKit.Enabled) {
                capManager.AddHealthKit();
            }

            if (capability.WirelessAccessoryConfiguration.Enabled) {
                capManager.AddWirelessAccessoryConfiguration();
            }

            /*


            if (ISD_Settings.Instance.Capabilities.Count == 0) {
                return;
            }


            foreach (var cap in ISD_Settings.Instance.Capabilities) {
                switch(cap.CapabilityType) {  
                 

                     case ISD_CapabilityType.InAppPurchase:
                        capManager.AddInAppPurchase();
                        break;
                    case ISD_CapabilityType.GameCenter:
                        capManager.AddGameCenter();
                        break;
                    case ISD_CapabilityType.PushNotifications:
                        var pushSettings = ISD_Settings.Instance.PushNotificationsCapabilitySettings;
                        capManager.AddPushNotifications(pushSettings.Development);
                        break;

                    default:
                        var capability = ISD_PBXCapabilityTypeUtility.ToPBXCapability(cap.CapabilityType);
                        string entitlementsFilePath = null;
                        if(!string.IsNullOrEmpty(cap.EntitlementsFilePath)) {
                            entitlementsFilePath = cap.EntitlementsFilePath;
                        } 


                        proj.AddCapability(targetGuid, capability, entitlementsFilePath, cap.AddOptionalFramework); 
                        break;
                }
            }
            */


        }


        static void AddFrameworks(PBXProject proj, string targetGuid, BuildTarget target) {
            foreach (ISD_Framework framework in ISD_Settings.Instance.Frameworks) {

                if(IsAvaliableOnPlatfrom(framework, target)) {
                    proj.AddFrameworkToProject(targetGuid, framework.Name, framework.IsOptional);
                }

            }
        }

        static bool IsAvaliableOnPlatfrom(ISD_Framework framework, BuildTarget target) {
            if(target == BuildTarget.tvOS) {
                switch(framework.Type) {
                    case ISD_iOSFramework.MessageUI:
                    case ISD_iOSFramework.Contacts:
                    case ISD_iOSFramework.ContactsUI:
                    case ISD_iOSFramework.Social:
                    case ISD_iOSFramework.Accounts:
                        return false;

                }
            }

            return true;

        }


        static void AddEmbededFrameworks(PBXProject proj, string targetGuid) {
            foreach (var framework in ISD_Settings.Instance.EmbededFrameworks) {


                string fileGuid = proj.AddFile(framework.AbsoluteFilePath, "Frameworks/" + framework.FileName, PBXSourceTree.Source);
                string embedPhase = proj.AddCopyFilesBuildPhase(targetGuid, "Embed Frameworks", "", "10");
                proj.AddFileToBuildSection(targetGuid, embedPhase, fileGuid);
#if UNITY_2017_4_OR_NEWER
                PBXProjectExtensions.AddFileToEmbedFrameworks(proj, targetGuid, fileGuid);
#endif
                proj.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
                //proj.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(SRCROOT)/PATH_TO_FRAMEWORK/");

            }
        }


        static void AddShellScriptBuildPhase(PBXProject proj, string targetGuid) {

           
            foreach (var script in ISD_Settings.Instance.ShellScripts) {

                #if UNITY_2018_3_OR_NEWER
                proj.AddShellScriptBuildPhase(targetGuid, script.Name, script.Shell, script.Script);
                #else
                MethodInfo dynMethod = proj.GetType().GetMethod("AppendShellScriptBuildPhase",
                                                 BindingFlags.NonPublic | BindingFlags.Instance, //if static AND public
                                                 null,
                                                 new[] { typeof(string), typeof(string), typeof(string), typeof(string) },//specify arguments to tell reflection which variant to look for
                                                 null);
                                  
             
                dynMethod.Invoke(proj, new object[] { targetGuid, script.Name, script.Shell, script.Script });
                #endif
            }
        }


        static void AddLibraries(PBXProject proj, string targetGuid) {
            foreach (var lib in ISD_Settings.Instance.Libraries) {
                proj.AddFrameworkToProject(targetGuid, lib.Name, lib.IsOptional);
            }
        }



        static void CopyAssetFiles(PBXProject proj, string pathToBuiltProject, string targetGUID) {
            
            foreach (ISD_AssetFile file in ISD_Settings.Instance.Files) {

                if (file.IsDirectory) {

                    foreach (var assetPath in Directory.GetFiles(file.RelativeFilePath)) {

                        string fileName = Path.GetFileName(assetPath);
                        string XCodeRelativePath = file.XCodeRelativePath + "/" + fileName;
                        CopyFile(XCodeRelativePath, assetPath, pathToBuiltProject, proj, targetGUID);
                    }

                } else {
                    CopyFile(file.XCodeRelativePath, file.RelativeFilePath, pathToBuiltProject, proj, targetGUID);
                }

            }
        }


        static void CopyFile(string XCodeRelativePath, string sourcePath, string pathToBuiltProject, PBXProject proj, string targetGUID) {

            var dstPath = Path.Combine(pathToBuiltProject, XCodeRelativePath);
            var rootPath = Path.GetDirectoryName(dstPath);

            if (!Directory.Exists(rootPath)) {
                Directory.CreateDirectory(rootPath);
            }

            File.Copy(sourcePath, dstPath);

            string name = proj.AddFile(XCodeRelativePath, XCodeRelativePath, PBXSourceTree.Source);
            proj.AddFileToBuild(targetGUID, name);
        }


    }
}
#endif