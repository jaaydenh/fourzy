//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using StackableDecorator;
using System;
using System.Collections.Generic;
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

            if (_camera.aspect < .51f)
                value = BackgroundConfiguration.IPHONEX;
            else if (_camera.aspect < .655f)
                value = BackgroundConfiguration.IPHONE;
            else
                value = BackgroundConfiguration.IPAD;

            return GetTheme(area).configurations.list.Find(configuration => configuration.value == value);
        }

        public string GetThemeName(Area area) => GetTheme(area)?.name ?? "theme_not_found";

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

            [StackableField]
            [ShowIf("#Check")]
            public string name;
            public string description;
            public int levelToUnlock;
            public bool enabled = true;
            public Area themeID;
            public AudioTypes bgAudio;
            public Sprite preview;
            [List]
            public GamepieceViewsCollection gamepieces;
            [List]
            public TokenViewsCollection tokens;
            [List]
            public AreaUnlockRequirementsCollection unclockRequirements;
            [List]
            public BackgroundConfigurations configurations;

            public bool Check()
            {
                _name = $"{name}: {(enabled ? "Enabled" : "Disabled")}";

                return true;
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
        public class TokenView_ThemesDataHolder
        {
            [HideInInspector]
            public string _name;

            [StackableField]
            [ShowIf("#Check")]
            public TokenView prefab;

            public bool Check()
            {
                if (!prefab)
                    return true;

                _name = prefab.name;

                return true;
            }
        }

        [Serializable]
        public class TokenViewsCollection
        {
            public List<TokenView_ThemesDataHolder> list;
        }

        [Serializable]
        public class AreaUnlockRequirement
        {
            [HideInInspector]
            public string _name;

            [StackableField]
            [ShowIf("#Check")]
            public CurrencyWidget.CurrencyType type;
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

            public AreaUnlockRequirement GetRequirement(CurrencyWidget.CurrencyType type) => list.Find(unlockRequirement => unlockRequirement.type == type);
        }

        public enum BackgroundConfiguration
        {
            NONE,
            IPHONEX,
            IPHONE,
            IPAD,
        }
    }
}