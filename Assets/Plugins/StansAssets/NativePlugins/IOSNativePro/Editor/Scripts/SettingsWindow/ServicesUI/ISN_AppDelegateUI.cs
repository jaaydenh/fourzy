using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;
using System.Collections.Generic;

using SA.iOS.UIKit;
using System;

namespace SA.iOS
{
    public class ISN_AppDelegateUI : ISN_ServiceSettingsUI
    {

        GUIContent m_note = new GUIContent("Note: Disabling App Delegate, will also disable User Notifications.");


        public override void OnAwake() {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://unionassets.com/ios-native-pro/getting-started-632");
            AddFeatureUrl("System Events", "https://unionassets.com/ios-native-pro/system-events-640");
            AddFeatureUrl("Force Touch Menu", "https://unionassets.com/ios-native-pro/force-touch-menu-641");
            AddFeatureUrl("External URL Calls", "https://unionassets.com/ios-native-pro/external-url-calls-642");
            AddFeatureUrl("Universal Links", "https://unionassets.com/ios-native-pro/universal-links-643");
        }


        public override string Title {
            get {
                return "App Delegate";
            }
        }

        public override string Description {
            get {
                return "A set of methods that are called in response to important events in the lifetime of your app.";
            }
        }

        protected override Texture2D Icon {
            get {
                return SA_EditorAssets.GetTextureAtPath(ISN_Skin.ICONS_PATH + "AppDelegate_icon.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return ISN_Preprocessor.GetResolver<ISN_AppDelegateResolver>();
            }
        }

        protected override IEnumerable<string> SupportedPlatforms {
            get {
                return new List<string>() { "iOS" };
            }
        }

        protected override void GettingStartedBlock() {
            base.GettingStartedBlock();
            using (new SA_GuiBeginHorizontal()) {
                GUILayout.Space(15);
                GUILayout.Label(m_note, SA_PluginSettingsWindowStyles.AssetLabel);
            }
        }


        protected override void OnServiceUI() {
            UrlTypesSettings();
            ForceTouchItemsBlock();
        }

        private void UrlTypesSettings() {
            using (new SA_WindowBlockWithSpace(new GUIContent("URL Types"))) {

                SA_EditorGUILayout.ReorderablList(ISN_Settings.Instance.UrlTypes,
                (ISN_UIUrlType url) => {
                    return url.Identifier;
                },
                (ISN_UIUrlType url) => {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Identifier");
                    url.Identifier = EditorGUILayout.TextField(url.Identifier);
                    EditorGUILayout.EndHorizontal();

                    for (int i = 0; i < url.Schemes.Count; i++) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Scheme " + i.ToString());
                        url.Schemes[i] = EditorGUILayout.TextField(url.Schemes[i]);

                        bool plus = GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20));
                        if (plus) {
                            url.AddSchemes("url_sheme");
                        }

                        bool rem = GUILayout.Button("-", EditorStyles.miniButtonLeft, GUILayout.Width(20));
                        if (rem) {
                            url.Schemes.Remove(url.Schemes[i]);
                            return;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                },
                () => {
                    ISN_UIUrlType newUlr = new ISN_UIUrlType(Application.identifier);
                    newUlr.AddSchemes("url_sheme");
                    ISN_Settings.Instance.UrlTypes.Add(newUlr);
                });

            }
        }


        private void ForceTouchItemsBlock() {
            using (new SA_WindowBlockWithSpace(new GUIContent("Force Touch Items"))) {

               SA_EditorGUILayout.ReorderablList(ISN_Settings.Instance.ShortcutItems,
               (ISN_UIApplicationShortcutItem item) => {
                   return item.Title;
               },
               (ISN_UIApplicationShortcutItem item) => {

                   EditorGUILayout.BeginHorizontal();
                   EditorGUILayout.LabelField("Title");
                   item.Title = EditorGUILayout.TextField(item.Title);
                   EditorGUILayout.EndHorizontal();

                   EditorGUILayout.BeginHorizontal();
                   EditorGUILayout.LabelField("Subtitle");
                   item.Subtitle = EditorGUILayout.TextField(item.Subtitle);
                   EditorGUILayout.EndHorizontal();

                   EditorGUILayout.BeginHorizontal();
                   EditorGUILayout.LabelField("Type");
                   item.Type = EditorGUILayout.TextField(item.Type);
                   EditorGUILayout.EndHorizontal();
               },
               () => {
                   ISN_UIApplicationShortcutItem newItem = new ISN_UIApplicationShortcutItem(string.Empty);
                   newItem.Title = "New Item";
                   ISN_Settings.Instance.ShortcutItems.Add(newItem);

               });
            }
        }
    }

}