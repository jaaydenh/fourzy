using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SA.Foundation.Editor;

namespace SA.iOS
{
    public class ISN_SettingsTab : SA_GUILayoutElement
    {

        GUIContent Info = new GUIContent("Info[?]:", "Full comunication logs between Native plugin part");
        GUIContent Warnings = new GUIContent("Warnings[?]:", "Warnings");
        GUIContent Errors = new GUIContent("Errors[?]:", "Errors");

        public override void OnGUI() {

            using (new SA_WindowBlockWithSpace(new GUIContent("Log Level"))) {
                EditorGUILayout.HelpBox("We recommend you to keep full loggin level while your project in development mode. " +
                                        "Full comunication logs between Native plugin part & " +
                                        "Unity side will be only avalibale with Info loggin level enabled. \n" +
                                        "Disabling the error logs isn't recommended", MessageType.Info);


                using (new SA_GuiBeginHorizontal()) {

                    var logLevel = ISN_Settings.Instance.LogLevel;

                    logLevel.Info = GUILayout.Toggle(logLevel.Info, Info, GUILayout.Width(80));
                    logLevel.Warning = GUILayout.Toggle(logLevel.Warning, Warnings, GUILayout.Width(100));
                    logLevel.Error = GUILayout.Toggle(logLevel.Error, Errors, GUILayout.Width(100));
                }
            }

            using (new SA_WindowBlockWithSpace(new GUIContent("Debug"))) {
                EditorGUILayout.HelpBox("API Resolver's are normally launched with build pre-process stage", MessageType.Info);
                using (new SA_GuiBeginHorizontal()) {

                    bool pressed = GUILayout.Button("Start API Resolvers");
                    if (pressed) {
                        SA_PluginsEditor.DisableLibstAtPath(ISN_Settings.IOS_NATIVE_XCODE_SOURCE);
                        ISN_Preprocessor.Resolve(forced: true);
                    }
                }

                EditorGUILayout.HelpBox("Action will reset all of the plugin settings to default.", MessageType.Info);
                using (new SA_GuiBeginHorizontal()) {
                    bool pressed = GUILayout.Button("Reset To Defaults");
                    if (pressed) {
                        ISN_Preprocessor.DropToDefault();
                    }
                }
            }
        }
    }
}