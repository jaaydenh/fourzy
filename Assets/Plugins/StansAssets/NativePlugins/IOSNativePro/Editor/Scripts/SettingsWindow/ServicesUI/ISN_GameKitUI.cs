using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;

using SA.iOS.GameKit;

namespace SA.iOS
{
    public class ISN_GameKitUI : ISN_ServiceSettingsUI
    {

     

        public override void OnAwake() {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://unionassets.com/ios-native-pro/getting-started-657");
            AddFeatureUrl("Authentication", "https://unionassets.com/ios-native-pro/connecting-to-game-cneter-658");
            AddFeatureUrl("Player Photo", "https://unionassets.com/ios-native-pro/connecting-to-game-cneter-658#player-photo");
            AddFeatureUrl("Server-side Auth", "https://unionassets.com/ios-native-pro/connecting-to-game-cneter-658#third-party-server-authentication");
            AddFeatureUrl("Game Center UI", "https://unionassets.com/ios-native-pro/game-center-ui-661");
            AddFeatureUrl("Leaderboards", "https://unionassets.com/ios-native-pro/leaderboards-660");
            AddFeatureUrl("Achievements", "https://unionassets.com/ios-native-pro/achievements-659");
            AddFeatureUrl("Saving A Game", "https://unionassets.com/ios-native-pro/saving-a-game-662");
        }


        public override string Title {
            get {
                return "Game Kit";
            }
        }

        public override string Description {
            get {
                return "GameKit offers features that you can use to create great social games."; 
            }
        }

        protected override Texture2D Icon {
            get {
                return SA_EditorAssets.GetTextureAtPath(ISN_Skin.ICONS_PATH + "GameKit_icon.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return ISN_Preprocessor.GetResolver<ISN_GameKitResolver>();
            }
        }


        protected override void OnServiceUI() {
           

            using (new SA_WindowBlockWithSpace(new GUIContent("Achievement"))) {

                if (ISN_Settings.Instance.Achievements.Count == 0) {
                    EditorGUILayout.HelpBox("Use this menu to list your game achievements. " +
                                            "This step is not required, and only designed for your" +
                                            "convinience, in case you are making custom in-game achievements view.", MessageType.Info);
                }
                DrawAchievementsList();
            }

            using (new SA_WindowBlockWithSpace(new GUIContent("Saving A Game"))) {
                EditorGUILayout.HelpBox("The saves API will allow you to provide your player an ability to save & load " +
                                        "game progress at any time.", MessageType.Info);

                ISN_Settings.Instance.SavingAGame = SA_EditorGUILayout.ToggleFiled("API State", ISN_Settings.Instance.SavingAGame, SA_StyledToggle.ToggleType.EnabledDisabled);
               
            }
        }



        static GUIContent AchievementIdDLabel = new GUIContent("Achievement Id[?]:", "A unique identifier that will be used for reporting. It can be composed of letters and numbers.");
        static GUIContent AchievementNameLabel = new GUIContent("Achievement Name[?]:", "The name of the achievement. This is the editor only field.");
        public static void DrawAchievementsList() {

            SA_EditorGUILayout.ReorderablList(ISN_Settings.Instance.Achievements, GetAchievementDisplayName, DrawAchievementContent, () => {
                ISN_Settings.Instance.Achievements.Add(new ISN_GKAchievement("my.new.achievement.id"));
            });

        }


        private static string GetAchievementDisplayName(ISN_GKAchievement achievement) {
            return achievement.Name + "(" + achievement.Identifier +  ")";
        }


        private static void DrawAchievementContent(ISN_GKAchievement achievement) {
            achievement.Identifier = SA_EditorGUILayout.TextField(AchievementIdDLabel, achievement.Identifier);
            achievement.Name = SA_EditorGUILayout.TextField(AchievementNameLabel, achievement.Name);
           
        }

    }
}