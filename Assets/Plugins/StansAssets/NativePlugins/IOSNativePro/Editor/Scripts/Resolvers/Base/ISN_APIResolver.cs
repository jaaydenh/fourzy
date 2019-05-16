using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;
using SA.Foundation.Utility;
using SA.Foundation.UtilitiesEditor;

using SA.iOS.XCode;


namespace SA.iOS
{
    public abstract class ISN_APIResolver : SA_iAPIResolver
    {
        private ISN_XcodeRequirements m_requirements;


        //--------------------------------------
        // Virtual
        //--------------------------------------

        public virtual void RunAdditionalPreprocess() {}


        //--------------------------------------
        // Abstract
        //--------------------------------------


        protected abstract string LibFolder { get; }
        public abstract string DefineName { get; }

        //Method is executed when Resolver is created. 
        //Resolvers can be refreshed from the editor, and GenerateRequirements will be triggered again
        //Those requirement will apply / remove only on build stage 
        protected abstract ISN_XcodeRequirements GenerateRequirements();


        public abstract bool IsSettingsEnabled { get; set; }
        public ISN_XcodeRequirements XcodeRequirements {
             get {
               
                if(m_requirements == null) {
                    m_requirements = GenerateRequirements();
                }

                return m_requirements;
             }
        }


        //--------------------------------------
        // Public Methods
        //--------------------------------------

        public void Run(bool plgingVersionUpdated = false) {
            if ((IsSettingsEnabled && !IsAPIEnabled) || (IsSettingsEnabled && plgingVersionUpdated)) {
                Enable();
            }

            if (!IsSettingsEnabled && IsAPIEnabled) {
                Disable();
            }


            //We want to run Xcode Requirement's every time on build preprocess
            //RemoveXcodeRequirements only excepted once if API appears to be turned off 
            if(IsSettingsEnabled) {
                AddXcodeRequirements();
            }

            //Defines & custom postprocess should be executed every time
            ChangeDefines(IsSettingsEnabled);
        }


        //--------------------------------------
        // Get / Set
        //--------------------------------------


        public bool IsAPIEnabled {
            get {
                if(string.IsNullOrEmpty(LibFolder)) {
                    return true;
                }
                return SA_AssetDatabase.IsDirectoryExists(LibFolderPath);
            }
        }

        public string LibFolderPath {
            get {
                if(string.IsNullOrEmpty(LibFolder)) {
                    return string.Empty;
                } else {
                    return ISN_Settings.IOS_NATIVE_XCODE + LibFolder;
                }
            }
        }

        //--------------------------------------
        // Private Methods
        //--------------------------------------

        private void Enable() {
            
            //There is no additional lib dependensies for this API
            if(string.IsNullOrEmpty(LibFolder)) {
                return;
            }

            string source = ISN_Settings.IOS_NATIVE_XCODE_SOURCE + LibFolder;
            string destination = LibFolderPath;

            SA_PluginsEditor.InstallLibFolder(source, destination);
        }


       

        private void Disable() {
            string destination = LibFolderPath;
            SA_PluginsEditor.UninstallLibFolder(destination);
           
            RemoveXcodeRequirements();
        }


        protected virtual void RemoveXcodePlistKey(ISD_PlistKey key) {
            ISD_API.RemoveInfoPlistKey(key);
        }


        //Method is executed on post process build only 
        protected virtual void RemoveXcodeRequirements() {
            foreach (var freamwork in XcodeRequirements.Frameworks) {
                ISD_API.RemoveFramework(freamwork.Type);
            }

            foreach (var lib in XcodeRequirements.Libraries) {
                ISD_API.RemoveLibrary(lib.Type);
            }

            foreach (var property in XcodeRequirements.Properties) {
                ISD_API.RemoveBuildProperty(property);
            }


            foreach (var key in XcodeRequirements.PlistKeys) {
                RemoveXcodePlistKey(key);
            }
           
        }


        //Method is executed on post process build only 
        protected virtual void AddXcodeRequirements() {
            foreach (var freamwork in XcodeRequirements.Frameworks) {
                ISD_API.AddFramework(freamwork);
            }

            foreach (var lib in XcodeRequirements.Libraries) {
                ISD_API.AddLibrary(lib);
            }

            foreach (var property in XcodeRequirements.Properties) {
                ISD_API.SetBuildProperty(property);
            }

            foreach (var key in XcodeRequirements.PlistKeys) {
                AddXcodePlistKey(key);
            }
        }

        protected virtual void AddXcodePlistKey(ISD_PlistKey key) {
            ISD_API.SetInfoPlistKey(key);
        }

       
        private void ChangeDefines(bool IsEnabled) {

            if(IsEnabled) {
                SA_EditorDefines.AddCompileDefine(DefineName, BuildTarget.iOS);
            } else {
                SA_EditorDefines.RemoveCompileDefine(DefineName, BuildTarget.iOS);
            }
           
        }


    }
}