using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor;

using SA.iOS.XCode;
using SA.Foundation.Editor;
using SA.Foundation.Utility;

#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

namespace SA.iOS
{

#if UNITY_2018_1_OR_NEWER
    public class ISN_Preprocessor : IPreprocessBuildWithReport
#else
    public class ISN_Preprocessor : IPreprocessBuild
#endif
    {

        //--------------------------------------
        // Static
        //--------------------------------------


        public static void Resolve(bool forced = false) {
            bool plgingVersionUpdated = ISN_Settings.UpdateVersion(ISN_Settings.FormattedVersion) && !SA_PluginTools.IsDevelopmentMode;

            Refresh();
            foreach (var resolver in Resolvers) {
                resolver.Run(plgingVersionUpdated || forced);
            }


            foreach (var resolver in Resolvers) {
                resolver.RunAdditionalPreprocess();
            }

        }

        public static void DropToDefault() {
            ISN_Settings.Delete();
            ISD_Settings.Delete();

            //Looks like uity bug. 
            //As always let's use the delay call magic
            EditorApplication.delayCall += () => {
                EditorApplication.delayCall += () => {
                    Refresh();
                    Resolve(forced: true);
                };
            };
           
        }

        public static void Refresh() {
            s_resolvers = null;
        }


        public static T GetResolver<T>() where T : ISN_APIResolver {
            return (T) GetResolver(typeof(T));
        }

        public static ISN_APIResolver GetResolver(Type resolverType) {
            foreach (var resolver in Resolvers) {
                if (resolver.GetType().Equals(resolverType)) {
                    return resolver;
                }
            }

            return null;
        }


        private static List<ISN_APIResolver> s_resolvers = null;
        public static List<ISN_APIResolver> Resolvers {
            get {
                if(s_resolvers == null) {
                    s_resolvers = new List<ISN_APIResolver>();

                    s_resolvers.Add(new ISN_StoreKitResolver());
                    s_resolvers.Add(new ISN_AppDelegateResolver());
                    s_resolvers.Add(new ISN_ContactsResolver());
                    s_resolvers.Add(new ISN_PhotosResolver());
                    s_resolvers.Add(new ISN_AVKitResolver());
                    s_resolvers.Add(new ISN_ReplayKitResolver());
                    s_resolvers.Add(new ISN_SocialResolver());
                    s_resolvers.Add(new ISN_UIKitResolver());
                    s_resolvers.Add(new ISN_FoundationResolver());
                    s_resolvers.Add(new ISN_GameKitResolver());
                    s_resolvers.Add(new ISN_UserNotificationsResolver());
                    s_resolvers.Add(new ISN_MediaPlayerResolver());
                    s_resolvers.Add(new ISN_CoreLocationResolver());

                    
                }
                return s_resolvers;
            }
        }




        public static void ChangeFileDefine(string file, string tag, bool IsEnabled) {
            if (SA_FilesUtil.IsFileExists(file)) {

                string defineLine = "#define " + tag;
                if (!IsEnabled) {
                    defineLine = "//" + defineLine;
                }

                string[] content = SA_FilesUtil.ReadAllLines(file);
                content[0] = defineLine;
                SA_FilesUtil.WriteAllLines(file, content);
            } else {
                Debug.LogError(file + " not found");
            }
        }


        //--------------------------------------
        // IPreprocessBuild
        //--------------------------------------


        public void OnPreprocessBuild(BuildTarget target, string path) {
            Preprocess(target);
        }


#if UNITY_2018_1_OR_NEWER
        public void OnPreprocessBuild(BuildReport report) {
            Preprocess(report.summary.platform);
        }
#endif


        public int callbackOrder  { get { return 0; }}



        //--------------------------------------
        // Private Methods
        //--------------------------------------

        private void Preprocess(BuildTarget target) {
            if (target == BuildTarget.iOS || target == BuildTarget.tvOS) {
                Resolve();
            }
        }
    }
}


