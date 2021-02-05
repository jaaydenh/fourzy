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
        [ListDrawerSettings(NumberOfItemsPerPage = 2, ListElementLabelName = "name")]
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

        public BackgroundConfigurationData GetCurrentAreaBGConfiguration(Camera _camera)
        {
            return GetAreaBGConfiguration(currentAreaData.areaID, _camera);
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
                //portrait
                if (_camera.aspect > .7f)
                {
                    value = BackgroundConfiguration.IPAD;
                }
                else if (_camera.aspect >= .5f)
                {
                    value = BackgroundConfiguration.IPHONE;
                }
                else
                {
                    value = BackgroundConfiguration.IPHONEX;
                }
            }

            return this[area].configurations.Find(configuration => configuration.type == value);
        }

        public bool IsAreaEnabled(Area area) => this[area]?.enabled ?? false;

        [Serializable]
        public class GameArea
        {
            public string name;
            public bool enabled = true;
            [ShowIf("enabled")]
            public Color areaColor;
            [ShowIf("enabled")]
            public string description;
            [ShowIf("enabled")]
            public int levelToUnlock;
            [ShowIf("enabled")]
            public Area areaID;
            [ShowIf("enabled")]
            public AudioTypes bgAudio;
            [ShowIf("enabled"), PreviewField(70)]
            public Sprite _16X9;
            [ShowIf("enabled"), PreviewField(70)]
            public Sprite _4X3;
            [ShowIf("enabled"), PreviewField(70)]
            public Sprite square;

            [ShowIf("enabled"), ListDrawerSettings(NumberOfItemsPerPage = 5)]
            public List<GamePieceView> gamepieces;
            [ShowIf("enabled"), ListDrawerSettings(NumberOfItemsPerPage = 5)]
            public List<TokenType> tokens;
            [ShowIf("enabled"), ListDrawerSettings(NumberOfItemsPerPage = 5, ListElementLabelName = "type")]
            public List<AreaUnlockRequirement> unlockRequirements;
            [ShowIf("enabled"), ListDrawerSettings(NumberOfItemsPerPage = 5, ListElementLabelName = "type")]
            public List<BackgroundConfigurationData> configurations;

            public AreaUnlockRequirement GetRequirement(CurrencyType type) =>
                unlockRequirements.Find(unlockRequirement => unlockRequirement.type == type);
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