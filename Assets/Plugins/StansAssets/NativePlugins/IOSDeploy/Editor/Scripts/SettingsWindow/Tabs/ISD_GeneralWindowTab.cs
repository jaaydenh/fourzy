using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;
using SA.Foundation.Localization;

namespace SA.iOS.XCode
{
    public class ISD_GeneralWindowTab : SA_GUILayoutElement
    {


        [SerializeField] bool IsDefFrameworksOpen = false;
        [SerializeField] bool IsDefLibrariesOpen = false;


        private SA_PluginActiveTextLink m_entitlementsLink;

        public override void OnAwake() {
            base.OnAwake();
            m_entitlementsLink = new SA_PluginActiveTextLink("[?] Read More");
        }


        public override void OnGUI() {



#if (UNITY_IOS || UNITY_TVOS)
            Settings();
#else
            EditorGUILayout.HelpBox("XCode Build Post-Process isn't avaliable on current platfrom", MessageType.Error);
#endif




            Texture2D buildSettings_icon    = SA_EditorAssets.GetTextureAtPath (ISD_Skin.ICONS_PATH + "BuildSettings.png"); 
            using(new SA_WindowBlockWithIndent(new GUIContent("Build Properties", buildSettings_icon))) {
                foreach (var property in ISD_Settings.Instance.BuildProperties) {
                    property.Value = SA_EditorGUILayout.StringValuePopup(property.Name, property.Value, property.Options);
                }
                EditorGUILayout.Space();
            }

            Frameworks();
            DrawEmbeddedBlock();

            BuildFlags();
            Languages();

            CopyFilestoXCodeBuildPhasee();
            ShellScriptsBuildPhasee();

            Entitlements();


        }

        private void Settings() {
            Texture2D settings_icon = SA_EditorAssets.GetTextureAtPath(ISD_Skin.ICONS_PATH + "isdSettings.png");
            using (new SA_WindowBlockWithIndent(new GUIContent("Settings", settings_icon))) {
                bool isEnabled = ISD_Settings.PostProcessEnabled;
                EditorGUI.BeginChangeCheck();
                isEnabled = SA_EditorGUILayout.ToggleFiled("Build PostProcess", isEnabled, SA_StyledToggle.ToggleType.YesNo);
                if (EditorGUI.EndChangeCheck()) {

                    EditorUtility.DisplayDialog("Configuration", "Changes will be applide after scripts re-recompilation is completed", "Okay");
                    if (isEnabled) {
                        SA_EditorDefines.RemoveCompileDefine("ISD_DISABLED", BuildTarget.iOS);
                    } else {
                        SA_EditorDefines.AddCompileDefine("ISD_DISABLED", BuildTarget.iOS);

                    }
                }

                if (!ISD_Settings.PostProcessEnabled) {
                    EditorGUILayout.HelpBox("Build PostProcess is curreently disabled. " +
                                            "The XCode project will not be modifayed.", MessageType.Error);
                }
            }
        }

        private void Entitlements() {
            Texture2D entitlements = SA_EditorAssets.GetTextureAtPath(ISD_Skin.ICONS_PATH + "entitlements.png");
            using (new SA_WindowBlockWithIndent(new GUIContent("Entitlements", entitlements))) {
                EditorGUILayout.HelpBox("Entitlements confer specific capabilities or security permissions to your iOS or macOS app.\n" +
                    "By default Entitlements file is generated based on your deploy settings, " +
                    "but you may alos provide own version of the entitlements file by using manual mode", MessageType.Info);
                using (new SA_GuiBeginHorizontal()) {
                    GUILayout.FlexibleSpace();
                    bool click = m_entitlementsLink.DrawWithCalcSize();
                    if (click) {
                        Application.OpenURL("https://developer.apple.com/documentation/uikit/core_app/protecting_the_user_s_privacy?language=objc");
                    }
                }

                EditorGUILayout.Space();

                ISD_Settings.Instance.EntitlementsMode = (ISD_EntitlementsGenerationMode) SA_EditorGUILayout.EnumPopup("Generation Mode", ISD_Settings.Instance.EntitlementsMode);
                if(ISD_Settings.Instance.EntitlementsMode == ISD_EntitlementsGenerationMode.Manual) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Entitlements File:");
                    ISD_Settings.Instance.EntitlementsFile = EditorGUILayout.ObjectField(ISD_Settings.Instance.EntitlementsFile, typeof(UnityEngine.Object), false);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void CopyFilestoXCodeBuildPhasee() {
            Texture2D folder = SA_EditorAssets.GetTextureAtPath(ISD_Skin.ICONS_PATH + "folder.png");
            using (new SA_WindowBlockWithIndent(new GUIContent("Copy Files to XCode Build Phasee", folder))) {
                SA_EditorGUILayout.ReorderablList(ISD_Settings.Instance.Files,
                   (ISD_AssetFile file) => {
                       return file.XCodeRelativePath;
                   },
                   (ISD_AssetFile file) => {

                       EditorGUILayout.BeginHorizontal();
                       EditorGUILayout.LabelField("Asset: ");
                       file.Asset = EditorGUILayout.ObjectField(file.Asset, typeof(UnityEngine.Object), false);
                       EditorGUILayout.EndHorizontal();

                       EditorGUILayout.BeginHorizontal();
                       EditorGUILayout.LabelField("XCode Path:");
                       file.XCodePath = EditorGUILayout.TextField(file.XCodePath);
                       EditorGUILayout.EndHorizontal();
                   },
                   () => {
                       ISD_Settings.Instance.Files.Add(new ISD_AssetFile());
                   }
                );

                EditorGUILayout.Space();
            }
        }

        private void ShellScriptsBuildPhasee() {
            Texture2D plistVariables_icon = SA_EditorAssets.GetTextureAtPath(ISD_Skin.ICONS_PATH + "plistVariables.png");
            using (new SA_WindowBlockWithIndent(new GUIContent("Shell Scripts Build Phasee", plistVariables_icon))) {
                SA_EditorGUILayout.ReorderablList(ISD_Settings.Instance.ShellScripts,

                   (ISD_ShellScript script) => {
                       return script.Name;
                   },
                    (ISD_ShellScript script) => {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Name: ");
                        script.Name = EditorGUILayout.TextField(script.Name);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Shell: ");
                        script.Shell = EditorGUILayout.TextField(script.Shell);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Script");
                        script.Script = EditorGUILayout.TextField(script.Script);
                        EditorGUILayout.EndHorizontal();
                    },
                    () => {
                        ISD_Settings.Instance.ShellScripts.Add(new ISD_ShellScript());
                    }
                );
                EditorGUILayout.Space();
            }
        }

        public void BuildFlags() {
            
            Texture2D linkerFlags = SA_EditorAssets.GetTextureAtPath(ISD_Skin.ICONS_PATH + "linkerFlags.png");
            using (new SA_WindowBlockWithIndent(new GUIContent("Linker & Compiler Flags", linkerFlags))) {
                SA_EditorGUILayout.ReorderablList(ISD_Settings.Instance.Flags,
                    (ISD_Flag flag) => {
                        if (flag.Type.Equals(ISD_FlagType.CompilerFlag)) {
                            return flag.Name + "     (Compiler)";
                        } else {
                            return flag.Name + "     (Linker)";
                        }
                    },
                    (ISD_Flag flag) => {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Full Name:", GUILayout.Width(100));
                        flag.Name = EditorGUILayout.TextField(flag.Name, GUILayout.ExpandWidth(true));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Type:", GUILayout.Width(100));
                    //flag.Type     = EditorGUILayout.TextField(flag.Type, GUILayout.ExpandWidth(true));
                    flag.Type = (ISD_FlagType)EditorGUILayout.EnumPopup(flag.Type);
                        EditorGUILayout.EndHorizontal();
                    },
                    () => {
                        ISD_Flag newFlag = new ISD_Flag();
                        newFlag.Name = "New Flag";
                        ISD_Settings.Instance.Flags.Add(newFlag);
                    }
                 );
            }

            EditorGUILayout.Space();
        }


        private int s_newLangindex = 0;
        public void Languages() {

            EditorGUI.BeginChangeCheck();

            Texture2D languages_icon      = SA_EditorAssets.GetTextureAtPath (ISD_Skin.ICONS_PATH + "languages.png");
            using (new SA_WindowBlockWithIndent(new GUIContent("Languages", languages_icon))) {
                SA_EditorGUILayout.ReorderablList(ISD_Settings.Instance.Languages,
                   (SA_ISOLanguage lang) => {
                       return lang.Code.ToUpper() + "     (" + lang.Name + ")";
                   }
               );

                EditorGUILayout.BeginHorizontal();
                s_newLangindex = EditorGUILayout.Popup(s_newLangindex, SA_LanguagesUtil.ISOLanguagesList.Names.ToArray());
                if (GUILayout.Button("Add Language", EditorStyles.miniButton, GUILayout.Width(100))) {
                    var lang = SA_LanguagesUtil.ISOLanguagesList.Languages[s_newLangindex];
                    ISD_Settings.Instance.Languages.Add(lang);
                }
                EditorGUILayout.EndHorizontal();
            }

            if(EditorGUI.EndChangeCheck()) {
                if(ISD_Settings.Instance.Languages.Count == 0) {
                    ISD_API.RemoveInfoPlistKey(ISD_Settings.CF_LOCLIZATIONS_PLIST_KEY);
                }
            }
        }



        public int NewBaseFrameworkIndex = 0;
        public int NewLibraryIndex = 0;
        public void Frameworks() {


            Texture2D framework_icon = SA_EditorAssets.GetTextureAtPath(ISD_Skin.ICONS_PATH + "frameworks.png");
            using (new SA_WindowBlockWithIndent(new GUIContent("Frameworks", framework_icon))) {

                IsDefFrameworksOpen = EditorGUILayout.Foldout(IsDefFrameworksOpen, 
                                                              "Show Default Unity Frameworks (" +ISD_FrameworkHandler.DefaultFrameworks.Count +  "Enabled)");
                if (IsDefFrameworksOpen) {

                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 0;
                    using (new SA_GuiBeginHorizontal()) {
                        GUILayout.Space(30);

                        using (new SA_GuiBeginVertical(GUI.skin.box)) {
                            foreach (ISD_Framework framework in ISD_FrameworkHandler.DefaultFrameworks) {
                                SA_EditorGUILayout.SelectableLabel(framework.Type.ToString() + ".framework", "");
                            }
                        }
                    }

                    EditorGUI.indentLevel = indentLevel;
                    EditorGUILayout.Space();
                }

   
                IsDefLibrariesOpen = EditorGUILayout.Foldout(IsDefLibrariesOpen, "Default Unity Libraries (2 Enabled)");
                if (IsDefLibrariesOpen) {
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 0;
                    using (new SA_GuiBeginHorizontal()) {
                        GUILayout.Space(30);

                        using (new SA_GuiBeginVertical(GUI.skin.box)) {
                            SA_EditorGUILayout.SelectableLabel("libiPhone-lib.a", "");
                            SA_EditorGUILayout.SelectableLabel("libiconv.2.dylib", "");
                        }
                    }
                    EditorGUI.indentLevel = indentLevel;


                    EditorGUILayout.Space();
                }

                //Frameworks List
                SA_EditorGUILayout.ReorderablList(ISD_Settings.Instance.Frameworks,
                   (ISD_Framework framework) => {
                       if (framework.IsOptional && framework.IsEmbeded) {
                           return framework.Name + "       (Optional & Embeded)";
                       } else if (framework.IsOptional) {
                           return framework.Name + "        (Optional)";
                       } else if (framework.IsEmbeded) {
                           return framework.Name + "        (Embeded)";
                       } else {
                           return framework.Name;
                       }
                   },
                    (ISD_Framework framework) => {
                        framework.IsOptional = SA_EditorGUILayout.ToggleFiled("Optional", framework.IsOptional, SA_StyledToggle.ToggleType.YesNo);
                        framework.IsEmbeded = SA_EditorGUILayout.ToggleFiled("Embeded", framework.IsEmbeded, SA_StyledToggle.ToggleType.YesNo);
                    }
                 );

                //Libraries List
                SA_EditorGUILayout.ReorderablList(ISD_Settings.Instance.Libraries,
                  (ISD_Library lib) => {
                      if (lib.IsOptional) {
                          return lib.Name + "    (Optional)";
                      } else {
                          return lib.Name;
                      }
                  },
                   (ISD_Library lib) => {
                       lib.IsOptional = SA_EditorGUILayout.ToggleFiled("Optional", lib.IsOptional, SA_StyledToggle.ToggleType.YesNo);
                   }
                );



                //Add New Framework
                EditorGUILayout.BeginHorizontal();
                NewBaseFrameworkIndex = EditorGUILayout.Popup(NewBaseFrameworkIndex, ISD_FrameworkHandler.BaseFrameworksArray());

                if (GUILayout.Button("Add Framework", EditorStyles.miniButton, GUILayout.Width(100))) {
                    var type = ISD_FrameworkHandler.BaseFrameworksArray()[NewBaseFrameworkIndex];
                    NewBaseFrameworkIndex = 0;

                    ISD_Framework f = new ISD_Framework(type);
                    ISD_API.AddFramework(f);
                }
                EditorGUILayout.EndHorizontal();


                //Add New Library
                EditorGUILayout.BeginHorizontal();
                NewLibraryIndex = EditorGUILayout.Popup(NewLibraryIndex, ISD_LibHandler.BaseLibrariesArray());

                if (GUILayout.Button("Add Library", EditorStyles.miniButton, GUILayout.Width(100))) {
                    ISD_iOSLibrary type = (ISD_iOSLibrary)ISD_LibHandler.enumValueOf(ISD_LibHandler.BaseLibrariesArray()[NewLibraryIndex]);
                    NewLibraryIndex = 0;
                    ISD_API.AddLibrary(type);
                }

                EditorGUILayout.EndHorizontal();



            }


        }


        private static void DrawEmbeddedBlock() {

            Texture2D libraries_icon = SA_EditorAssets.GetTextureAtPath(ISD_Skin.ICONS_PATH + "libraries.png");
            using (new SA_WindowBlockWithIndent(new GUIContent("Embedded Binaries", libraries_icon))) {
                SA_EditorGUILayout.ReorderablList(ISD_Settings.Instance.EmbededFrameworks,
                (ISD_EmbedFramework framework) => {
                    return framework.FileName;
                },
                (ISD_EmbedFramework freamwork) => {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Asset: ");
                    freamwork.Asset = EditorGUILayout.ObjectField(freamwork.Asset, typeof(UnityEngine.Object), false);
                    EditorGUILayout.EndHorizontal();
                },
                () => {
                    ISD_Settings.Instance.EmbededFrameworks.Add(new ISD_EmbedFramework());
                }
             );
            }

        }
    }
}
