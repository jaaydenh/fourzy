//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Mechanics.GameplayScene;
using FourzyGameModel.Model;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultAreasDataHolder", menuName = "Create Areas Data Holder")]
    public class AreasDataHolder : ScriptableObject
    {
        public static Action<Area, bool> onAreaUnlockChanged;

        [ListDrawerSettings(NumberOfItemsPerPage = 10, ListElementLabelName = "name")]
        public List<GameArea> areas;

        public GameArea currentAreaData
        {
            get => this[PlayerPrefsWrapper.GetCurrentArea()];
            set => PlayerPrefsWrapper.SetCurrentArea((int)value.areaID);
        }

        public GameArea this[Area area]
        {
            get => areas.Find((_areaData) => _areaData.areaID == area);
        }

        public GameArea this[int value]
        {
            get => areas.Find((_areaData) => _areaData.areaID == (Area)value);
        }

        public BackgroundConfigurationData GetAreaBGConfiguration(Area area, Camera _camera)
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
                //ipad
                if (_camera.aspect > .74f)
                {
                    value = BackgroundConfiguration.IPAD;
                }
                //iphone
                else if (_camera.aspect > .55f)
                {
                    value = BackgroundConfiguration.IPHONE;
                }
                //iphonex
                else
                {
                    value = BackgroundConfiguration.IPHONEX;
                }
            }

            return this[area].configurations.Find(configuration => configuration.type == value);
        }

        public void SetAreaUnlockedState(Area area, bool state)
        {
            this[area].SetUnlockedState(state);

            onAreaUnlockChanged?.Invoke(area, state);
        }

        [Serializable]
        public class GameArea
        {
            public string name;
            [NonSerialized]
            public bool unlocked;
            public Color areaColor;
            public string description;
            public int levelToUnlock;
            public Area areaID;
            public string bgAudio;
            public Sprite _16X9;
            [PreviewField(70)]
            public Sprite _4X3;
            [PreviewField(70)]
            public Sprite square;

            [ListDrawerSettings(NumberOfItemsPerPage = 5)]
            public List<GamePieceView> gamepieces;
            [ListDrawerSettings(NumberOfItemsPerPage = 5)]
            public List<TokenType> tokens;
            [ListDrawerSettings(NumberOfItemsPerPage = 5, ListElementLabelName = "type")]
            public List<AreaUnlockRequirement> unlockRequirements;
            [ListDrawerSettings(NumberOfItemsPerPage = 5, ListElementLabelName = "type")]
            public List<BackgroundConfigurationData> configurations;

            /// <summary>
            /// Localized
            /// </summary>
            public string Name => LocalizationManager.Value(name);

            public AreaUnlockRequirement GetRequirement(CurrencyType type) =>
                unlockRequirements.Find(unlockRequirement => unlockRequirement.type == type);

            public void SetUnlockedState(bool state)
            {
                unlocked = state;
            }
        }

        [Serializable]
        public class AreaUnlockRequirement
        {
            public CurrencyType type;
            public int quantity;
        }

        [Serializable]
        public class BackgroundConfigurationData
        {
            public BackgroundConfiguration type;
            public GameplayBG backgroundPrefab;
            public GameboardView gameboardPrefab;
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