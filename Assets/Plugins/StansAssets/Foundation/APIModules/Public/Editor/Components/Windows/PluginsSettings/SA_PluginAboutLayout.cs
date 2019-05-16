using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Config;

namespace SA.Foundation.Editor
{
	public class SA_PluginAboutLayout : SA_GUILayoutElement
	{
        //What we do
        [SerializeField] SA_HyperLabel m_games;
        [SerializeField] SA_HyperLabel m_plugins;
        [SerializeField] SA_HyperLabel m_team;

        //How to get in touch
        [SerializeField] SA_HyperLabel m_linkedIn;
        [SerializeField] SA_HyperLabel m_twitter;
        [SerializeField] SA_HyperLabel m_facebook;


        [SerializeField] SA_HyperLabel m_youtube;
        [SerializeField] SA_HyperLabel m_google;
        [SerializeField] SA_HyperLabel m_twitch;

        [SerializeField] SA_HyperLabel m_support_mail;
        [SerializeField] SA_HyperLabel m_ceo_mail;

        [SerializeField] SA_HyperLabel web_siteLabel;

        private  const int LABEL_WIDTH = 100;
        private const int SOCIAL_LABEL_WIDTH = 70;



		GUIContent m_whoWeAre = new GUIContent("Stan’s Assets is a team of Unity developers " +
            "with more than 6 years of experience that are " +
            "committed to develop high quality and " +
            "engaging entertainment software.");



		GUIContent m_whatWeDo = new GUIContent("Game development our main direction. But we do everything that is " +
                                                  "connected to Unity. Games, Plugins, VR, AR " +
                                                  "and even enterprise applications with 3D elemnets. " +
                                                  "And of course we are always looking forward to a new challenging projects.");


		public override void OnLayoutEnable() {
			m_games = CreateAboutLabel(" Our Games",  "game.png");
			m_plugins = CreateAboutLabel(" Our Plugins", "plugin.png");
			m_team = CreateAboutLabel(" Our Team", "team.png");


            m_linkedIn = CreateSocialLabel("LinkedIn", "linkedin.png");
            m_twitter = CreateSocialLabel("Twitter", "twitter.png");
            m_facebook = CreateSocialLabel("Facebook", "facebook.png");


            m_youtube = CreateSocialLabel("Youtube", "youtube.png");
            m_google = CreateSocialLabel("Google+", "google-plus.png");
            m_twitch = CreateSocialLabel("Twitch", "twitch.png");


            m_support_mail = CreateTextLabel(SA_Config.STANS_ASSETS_SUPPORT_EMAIL);
            m_ceo_mail = CreateTextLabel(SA_Config.STANS_ASSETS_CEO_EMAIL);


            web_siteLabel = new SA_HyperLabel(new GUIContent(SA_CompanyGUILayout.Logo), SA_EditorStyles.DescribtionLabelStyle);
            web_siteLabel.SetMouseOverColor(SA_PluginSettingsWindowStyles.SelectedElementColor);
        }

		public override void OnGUI() {
                 
			using (new SA_WindowBlockWithIndent(new GUIContent("Who we are"))) {
				EditorGUILayout.LabelField(m_whoWeAre, SA_EditorStyles.DescribtionLabelStyle);
				EditorGUILayout.Space();
			}
           

			using (new SA_WindowBlockWithIndent(new GUIContent("What we do"))) {
				EditorGUILayout.LabelField(m_whatWeDo, SA_EditorStyles.DescribtionLabelStyle);
				EditorGUILayout.Space();


				EditorGUILayout.Space();
                using (new SA_GuiBeginHorizontal()) {
                    bool games =  m_games.Draw(GUILayout.Width(LABEL_WIDTH));
                    if(games) {
                        Application.OpenURL("https://stansassets.com/#portfolio");
                    }

                    bool plugins = m_plugins.Draw(GUILayout.Width(LABEL_WIDTH));
                    if (plugins) {
                        Application.OpenURL("https://assetstore.unity.com/publishers/2256");
                    }

                    EditorGUILayout.Space();
                }


                EditorGUILayout.Space();
                using (new SA_GuiBeginHorizontal()) {
                    bool team = m_team.Draw(GUILayout.Width(LABEL_WIDTH));
                    if(team) {
                        Application.OpenURL(" https://stansassets.com/#our-team");
                    }

                    EditorGUILayout.Space();
                }

            }

           
            bool clicked;
			using (new SA_WindowBlockWithIndent(new GUIContent("How to get in touch"))) {
                using (new SA_GuiBeginHorizontal()) {
                    EditorGUILayout.LabelField("If you have any technical issues or questions, do not hesitate to drop us a message at:", SA_EditorStyles.DescribtionLabelStyle);
                }

                using (new SA_GuiBeginHorizontal()) {
                    if(m_support_mail.Draw()) {
                        Application.OpenURL("mailto:" + SA_Config.STANS_ASSETS_SUPPORT_EMAIL);
                    }

                }


                EditorGUILayout.Space();
                using (new SA_GuiBeginHorizontal()) {
                    EditorGUILayout.LabelField("For a non technical and business related questions, use:", SA_EditorStyles.DescribtionLabelStyle);
                }

                using (new SA_GuiBeginHorizontal()) {
                    clicked = m_ceo_mail.Draw();
                    if(clicked) {
                        Application.OpenURL("mailto:" + SA_Config.STANS_ASSETS_CEO_EMAIL);
                    }
                }



                EditorGUILayout.Space();
                using (new SA_GuiBeginHorizontal()) {
                    EditorGUILayout.LabelField("Let's just be in touch", SA_EditorStyles.DescribtionLabelStyle);
                }

               
                EditorGUILayout.Space();
                using (new SA_GuiBeginHorizontal()) {
                    clicked = m_linkedIn.Draw(GUILayout.Width(SOCIAL_LABEL_WIDTH));
                    if(clicked) { Application.OpenURL("https://www.linkedin.com/in/lacost"); }
                    clicked = m_twitter.Draw(GUILayout.Width(SOCIAL_LABEL_WIDTH));
                    if (clicked) { Application.OpenURL("https://twitter.com/stansassets"); }
                    clicked = m_facebook.Draw(GUILayout.Width(SOCIAL_LABEL_WIDTH));
                    if (clicked) { Application.OpenURL("https://www.facebook.com/stansassets/"); }

                    EditorGUILayout.Space();
                }
              

                EditorGUILayout.Space();
                using (new SA_GuiBeginHorizontal()) {
                    clicked = m_youtube.Draw(GUILayout.Width(SOCIAL_LABEL_WIDTH));
                    if (clicked) { Application.OpenURL("https://www.youtube.com/user/stansassets/videos"); }

                    clicked = m_google.Draw(GUILayout.Width(SOCIAL_LABEL_WIDTH));
                    if (clicked) { Application.OpenURL("https://plus.google.com/+StansassetsOfficial"); }

                    clicked =  m_twitch.Draw(GUILayout.Width(SOCIAL_LABEL_WIDTH));
                    if (clicked) { Application.OpenURL("https://www.twitch.tv/stans_assets"); }

                    EditorGUILayout.Space();
                }


                EditorGUILayout.Space();


                using (new SA_GuiBeginHorizontal()) {
                    clicked = web_siteLabel.Draw();
                    if(clicked) {
                        Application.OpenURL(SA_Config.STANS_ASSETS_WEBSITE_ROOT_URL);
                    }
                }
            }

        }



	


		private SA_HyperLabel CreateAboutLabel(string title, string icon) {
            return CreateLabel(title, icon, SA_Skin.ABOUT_ICONS_PATH);
        }

        private SA_HyperLabel CreateSocialLabel(string title, string icon) {
            return CreateLabel(title, icon, SA_Skin.SOCIAL_ICONS_PATH);
        }

        private SA_HyperLabel CreateLabel(string title, string icon, string iconFolder) {

            var image = SA_EditorAssets.GetTextureAtPath(iconFolder + icon);
            var label = new SA_HyperLabel(new GUIContent(title, image), SA_EditorStyles.DescribtionLabelStyle);
            label.SetMouseOverColor(SA_PluginSettingsWindowStyles.SelectedElementColor);
            return label;
        }


        private SA_HyperLabel CreateTextLabel(string title) {
            var label = new SA_HyperLabel(new GUIContent(title), SA_PluginSettingsWindowStyles.SelectebleLabelStyle);
            label.SetMouseOverColor(SA_PluginSettingsWindowStyles.SelectedElementColor);
            return label;
        }

    }
}