////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


namespace SA.Foundation.Config {
	
	public class SA_Config  {

		public const string STANS_ASSETS_SUPPORT_EMAIL 				= "support@stansassets.com";
        public const string STANS_ASSETS_CEO_EMAIL                  = "ceo@stansassets.com";
        public const string STANS_ASSETS_WEBSITE_ROOT_URL           = "https://stansassets.com/";



        public const string STANS_ASSETS_PLUGINS_PATH          = "Assets/Plugins/StansAssets/";
        public const string STANS_ASSETS_THIRD_PARTY_NOTICES = STANS_ASSETS_PLUGINS_PATH + "Third-PartyNotices.txt";


        public const string STANS_ASSETS_CROSS_PLATFORM_PLUGINS_PATH = STANS_ASSETS_PLUGINS_PATH + "CrossPlatform/";
        public const string STANS_ASSETS_NATIVE_PLUGINS_PATH = STANS_ASSETS_PLUGINS_PATH + "NativePlugins/";
        public const string STANS_ASSETS_PRODUCTIVITY_PLUGINS_PATH = STANS_ASSETS_PLUGINS_PATH + "Productivity/";
        public const string STANS_ASSETS_SERVICES_PLUGINS_PATH = STANS_ASSETS_PLUGINS_PATH + "Services/";

        


        public const string STANS_ASSETS_FOUNDATION_PATH       = STANS_ASSETS_PLUGINS_PATH + "Foundation/";
        public const string STANS_ASSETS_FOUNDATION_API_PATH = STANS_ASSETS_FOUNDATION_PATH + "API/";

        public const string STANS_ASSETS_FOUNDATION_API_MODULES_PATH = STANS_ASSETS_FOUNDATION_PATH + "APIModules/";
        public const string STANS_ASSETS_FOUNDATION_API_MODULES_PATH_PUBLIC = STANS_ASSETS_FOUNDATION_API_MODULES_PATH + "Public/";
        public const string STANS_ASSETS_FOUNDATION_API_MODULES_PATH_PRIVATE = STANS_ASSETS_FOUNDATION_API_MODULES_PATH + "Private/";
        public const string STANS_ASSETS_FOUNDATION_API_MODULES_PATH_THIRD_PARTY = STANS_ASSETS_FOUNDATION_API_MODULES_PATH + "ThirdParty/";



        public const string STANS_ASSETS_SETTINGS_ROOT_PATH = STANS_ASSETS_PLUGINS_PATH + "Settings/";

        public const string STANS_ASSETS_CACHE_PATH             = STANS_ASSETS_SETTINGS_ROOT_PATH + "Cache/Resources/";
		public const string STANS_ASSETS_SETTINGS_PATH          = STANS_ASSETS_SETTINGS_ROOT_PATH + "Resources/";


        public const string STANS_ASSETS_EDITOR_SETTINGS_PATH = STANS_ASSETS_SETTINGS_ROOT_PATH + "Editor/";
        public const string STANS_ASSETS_EDITOR_SETTINGS_RESOURCES_PATH  = STANS_ASSETS_EDITOR_SETTINGS_PATH + "Resources/";


        public const string UNITY_IOS_PLUGINS_PATH 	 = "Assets/Plugins/IOS/";
        public const string UNITY_ANDROID_PLUGINS_PATH = "Assets/Plugins/Android/";


        public const string EDITOR_MENU_ROOT = "Stan's Assets/";
        public const string EDITOR_FOUNDATION_LIB_MENU_ROOT = EDITOR_MENU_ROOT + "Foundation/";
        public const string EDITOR_ANALYTICS_MENU_ROOT = EDITOR_MENU_ROOT + "Analytics/";
        public const string EDITOR_PRODUCTIVITY_MENU_ROOT = EDITOR_MENU_ROOT + "Productivity/";

        public const string EDITOR_PRODUCTIVITY_NATIVE_UTILITY_MENU_ROOT = EDITOR_PRODUCTIVITY_MENU_ROOT + "Native Utility/";


        public const int PRODUCTIVITY_MENU_INDEX = 500;
        public const int PRODUCTIVITY_NATIVE_UTILITY_MENU_INDEX = 600;

        public const int FOUNDATION_MENU_INDEX = 1000;




		public const string STANS_ASSETS_EDITOR_ART = STANS_ASSETS_FOUNDATION_API_MODULES_PATH + "Public/Editor/Art/";
		public const string STANS_ASSETS_EDITOR_ICONS = STANS_ASSETS_EDITOR_ART + "Icons/";
        public const string STANS_ASSETS_EDITOR_FONTS = STANS_ASSETS_EDITOR_ART + "Fonts/";



        public const string STANS_ASSETS_EDITOR_CONTENT = STANS_ASSETS_FOUNDATION_PATH + "EditorContent/";
        public const string STANS_ASSETS_EDITOR_GRAPHIC = STANS_ASSETS_EDITOR_CONTENT + "SAGraphic/";

		
		private static PluginVersionHandler s_foundationVersion;

		public static PluginVersionHandler FoundationVersion {
			get {
				if (s_foundationVersion == null) {
					s_foundationVersion = new PluginVersionHandler(STANS_ASSETS_FOUNDATION_PATH);
				}
				return s_foundationVersion;
			}
		}

	}

}
