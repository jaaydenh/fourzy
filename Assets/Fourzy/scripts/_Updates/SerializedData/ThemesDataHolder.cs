//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using StackableDecorator;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultThemesDataHolder", menuName = "Create Themes Data Holder")]
    public class ThemesDataHolder : ScriptableObject
    {
        [List]
        public GameThemeList themes;

        private int currentThemeIndex = -1;

        public GameTheme currentTheme
        {
            get
            {
                if (currentThemeIndex == -1)
                    currentThemeIndex = PlayerPrefsWrapper.GetCurrentTheme();

                return themes.list[currentThemeIndex];
            }
            set
            {
                currentThemeIndex = themes.list.IndexOf(value);
                PlayerPrefsWrapper.SetCurrentGameTheme(currentThemeIndex);
            }
        }

        public GameTheme GetTheme(Area area) => themes.list.Find((_theme) => _theme.themeID == area);

        public BackgroundConfigurationData GetCurrentThemeBGConfiguration(Camera _camera) => GetThemeBGConfiguration(currentTheme.themeID, _camera);

        public BackgroundConfigurationData GetThemeBGConfiguration(Area area, Camera _camera)
        {
            BackgroundConfiguration value = BackgroundConfiguration.IPHONEX;

            //check portrait/landscape
            if (Screen.width > Screen.height)
            {
                //landscape
                value = BackgroundConfiguration.WIDESCREEN;
            }
            else
            {
                //portrait
                if (_camera.aspect > .7f)
                    value = BackgroundConfiguration.IPAD;
                else if (_camera.aspect >= .5f)
                    value = BackgroundConfiguration.IPHONE;
                else
                    value = BackgroundConfiguration.IPHONEX;
            }

            return GetTheme(area).configurations.list.Find(configuration => configuration.value == value);
        }

        public string GetThemeName(Area area) => GetTheme(area)?.id ?? "theme_not_found";

        public Area GetRandomTheme(Area exclude, bool excludeDisabled = true)
        {
            List<Area> areas = new List<Area>();
            foreach (Area area in Enum.GetValues(typeof(Area)).Cast<Area>()) if (!exclude.HasFlag(area) && IsThemeEnabled(area)) areas.Add(area);
            return areas.Random();
        }

        public bool IsThemeEnabled(Area area) => GetTheme(area)?.enabled ?? false;

        [Serializable]
        public class GameThemeList
        {
            public List<GameTheme> list;
        }

        [Serializable]
        public class GameTheme
        {
            [HideInInspector]
            public string _name;

            [ShowIf("#Check"), StackableField]
            public string id;
            public bool enabled = true;
            [ShowIf("#ShowIf"), StackableField]
            public Color themeColor;
            [ShowIf("#ShowIf"), StackableField]
            public string description;
            [ShowIf("#ShowIf"), StackableField]
            public int levelToUnlock;
            [ShowIf("#ShowIf"), StackableField]
            public Area themeID;
            [ShowIf("#ShowIf"), StackableField]
            public AudioTypes bgAudio;
            [ShowIf("#ShowIf"), StackableField]
            public Sprite preview;

            [List]
            [ShowIf("#ShowIf"), StackableField]
            public GamepieceViewsCollection gamepieces;
            [List]
            [ShowIf("#ShowIf"), StackableField]
            public TokenViewsCollection tokens;
            [List]
            [ShowIf("#ShowIf"), StackableField]
            public AreaUnlockRequirementsCollection unclockRequirements;
            [List]
            [ShowIf("#ShowIf"), StackableField]
            public BackgroundConfigurations configurations;

            public bool Check()
            {
                _name = $"{id}: {(enabled ? "Enabled" : "Disabled")}";

                return true;
            }

            public bool ShowIf()
            {
                return enabled;
            }
        }

        [Serializable]
        public class BackgroundConfigurations
        {
            public List<BackgroundConfigurationData> list;
        }

        [Serializable]
        public class BackgroundConfigurationData
        {
            [HideInInspector]
            public string _name;

            [StackableField]
            [ShowIf("#Check")]
            public BackgroundConfiguration value;
            public GameplayBG backgroundPrefab;
            public GameboardView gameboardPrefab;

            public bool Check()
            {
                _name = value.ToString();

                return true;
            }
        }

        [Serializable]
        public class GamePieceView_ThemesDataHolder
        {
            [HideInInspector]
            public string _name;
            [HideInInspector]
            public GamePieceView _prefab;

            [StackableField]
            [ShowIf("#Check")]
            public GamePieceView prefab;

            public bool Check()
            {
                if (!prefab || _prefab == prefab) return true;

                _name = prefab.pieceData.name;
                _prefab = prefab;

                return true;
            }
        }

        [Serializable]
        public class GamepieceViewsCollection
        {
            public List<GamePieceView_ThemesDataHolder> list;
        }

        [Serializable]
        public class TokenViewsCollection
        {
            public List<TokenType> list;
        }

        [Serializable]
        public class AreaUnlockRequirement
        {
            [HideInInspector]
            public string _name;

            [StackableField]
            [ShowIf("#Check")]
            public CurrencyType type;
            public int quantity;

            public bool Check()
            {
                _name = type.ToString();

                return true;
            }
        }

        [Serializable]
        public class AreaUnlockRequirementsCollection
        {
            public List<AreaUnlockRequirement> list;

            public AreaUnlockRequirement GetRequirement(CurrencyType type) => list.Find(unlockRequirement => unlockRequirement.type == type);
        }

        public enum BackgroundConfiguration
        {
            NONE,
            IPHONEX,
            IPHONE,
            IPAD,
            WIDESCREEN,
        }
    }
}