//modded @vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using Fourzy._Updates._Tutorial;
using Fourzy._Updates.UI.Camera3D;
using Fourzy._Updates.UI.Menu;
using FourzyGameModel.Model;
using StackableDecorator;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class GameContentManager : UnitySingleton<GameContentManager>
    {
        public List<Camera3dItemProgressionMap> progressionMaps;
        public GamePiecesDataHolder piecesDataHolder;
        public AIPlayersDataHolder aiPlayersDataHolder;
        public TokensDataHolder tokensDataHolder;
        public ThemesDataHolder themesDataHolder;
        public PassAndPlayDataHolder passAndPlayDataHolder;
        public MiscBoardsDataHolder miscBoardsDataHolder;
        public MiscGameContentHolder miscGameDataHolder;
        [List]
        public PrefabsCollection typedPrefabs;
        [List]
        public ScreensCollection screens;

        private Dictionary<string, ResourceItem> fastPuzzles = new Dictionary<string, ResourceItem>();

        public Dictionary<string, BasicPuzzlePack> externalPuzzlePacks { get; private set; }

        public Dictionary<PrefabType, PrefabTypePair> typedPrefabsFastAccess { get; private set; }

        public ThemesDataHolder.GameTheme currentTheme
        {
            get => themesDataHolder.currentTheme;

            set => themesDataHolder.currentTheme = value;
        }

        public List<ThemesDataHolder.GameTheme> themes => themesDataHolder.themes.list;

        public List<ThemesDataHolder.GameTheme> enabledThemes => themesDataHolder.themes.list.Where(theme => theme.enabled).ToList();

        public List<TokensDataHolder.TokenData> tokens => tokensDataHolder.tokens.list;

        public List<TokensDataHolder.TokenData> enabledTokens => tokensDataHolder.tokens.list.Where(token => token.enabled).ToList();

        public List<GameBoardDefinition> passAndPlayGameboards => passAndPlayDataHolder.gameboards;

        public int finishedFastPuzzlesCount => fastPuzzles.Keys.Where(id => PlayerPrefsWrapper.GetFastPuzzleComplete(id)).Count();

        public List<Camera3dItemProgressionMap> existingProgressionMaps { get; private set; } = new List<Camera3dItemProgressionMap>();

        protected override void Awake()
        {
            base.Awake();

            if (InstanceExists) return;

            typedPrefabsFastAccess = new Dictionary<PrefabType, PrefabTypePair>();
            foreach (PrefabTypePair prefabTypePair in typedPrefabs.list)
                if (!typedPrefabsFastAccess.ContainsKey(prefabTypePair.prefabType))
                    typedPrefabsFastAccess.Add(prefabTypePair.prefabType, prefabTypePair);

            piecesDataHolder.Initialize();
            passAndPlayDataHolder.Initialize();
            miscBoardsDataHolder.Initialize();

            //packsDataHolders.ForEach(packsData => packsData.Initialize());

            LoadAllFastPuzzles();
            LoadPuzzlePacks();
        }

        public GameBoardDefinition GetMiscBoard(string boardID) => miscBoardsDataHolder.gameboards.Find(board => board.ID == boardID);

        public GameBoardDefinition GetMiscBoardByName(string boardName) => miscBoardsDataHolder.gameboards.Find(board => board.BoardName == boardName);

        public GameBoardDefinition GetPassAndPlayBoard(string boardID) => passAndPlayDataHolder.gameboards.Find(board => board.ID == boardID);

        public GameBoardDefinition GetPassAndPlayBoardByName(string boardName) => passAndPlayDataHolder.gameboards.Find(board => board.BoardName == boardName);

        public TokenView GetTokenPrefab(TokenType tokenType, Area theme) => tokensDataHolder.GetToken(tokenType, theme);

        public TokenView GetTokenPrefab(TokenType tokenType) => GetTokenPrefab(tokenType, themes[0].themeID);

        public TokensDataHolder.TokenData GetTokenData(TokenType tokenType) => tokensDataHolder.GetTokenData(tokenType);

        public List<ThemesDataHolder.GameTheme> GetTokenThemes(TokenType tokenType) => GetTokenData(tokenType)?.GetTokenThemes(themesDataHolder) ?? null;

        public List<string> GetTokenThemeNames(TokenType tokenType) => GetTokenData(tokenType)?.GetThemeNames(themesDataHolder) ?? null;

        public ClientFourzyPuzzle GetNextFastPuzzle(string id = "")
        {
            List<string> ids = new List<string>(fastPuzzles.Keys);

            if (string.IsNullOrEmpty(id))
            {
                //get random one
                ids.Shuffle();
                string _id = "";
                
                foreach (string __id in ids)
                    if (!PlayerPrefsWrapper.GetFastPuzzleComplete(__id))
                    {
                        _id = __id;
                        break;
                    }

                if (string.IsNullOrEmpty(_id))
                    return new ClientFourzyPuzzle(new ClientPuzzleData(fastPuzzles[ids[0]]).Initialize());
                else
                    return new ClientFourzyPuzzle(new ClientPuzzleData(fastPuzzles[_id]).Initialize());
            }
            else
            {
                int idIndex = ids.FindIndex(ids.IndexOf(id), __id => !PlayerPrefsWrapper.GetFastPuzzleComplete(__id));

                if (idIndex > -1 && idIndex < ids.Count - 1)
                    return new ClientFourzyPuzzle(new ClientPuzzleData(fastPuzzles[ids[idIndex + 1]]).Initialize());
                else
                    return GetNextFastPuzzle();
            }
        }

        public ClientFourzyPuzzle GetFastPuzzle(string id) => new ClientFourzyPuzzle(new ClientPuzzleData(fastPuzzles[id]).Initialize());

        public void ResetFastPuzzles()
        {
            foreach (string id in fastPuzzles.Keys) PlayerPrefsWrapper.SetFastPuzzleComplete(id, false);
        }

        public void ResetPuzzlePacks()
        {
            //foreach (PuzzlePacksDataHolder pack in packsDataHolders)
            //    pack.ResetPlayerPrefs();

            foreach (Camera3dItemProgressionMap progressionMap in progressionMaps)
                (existingProgressionMaps.Find(__map => __map.mapID == progressionMap.mapID) ?? progressionMap).ResetPlayerPrefs();

            foreach (BasicPuzzlePack pack in externalPuzzlePacks.Values)
                pack.ResetPlayerPrefs();
        }

        public BasicPuzzlePack GetExternalPuzzlePack(string folderName) => externalPuzzlePacks[folderName];

        private void LoadAllFastPuzzles()
        {
            foreach (ResourceItem item in ResourceDB.GetFolder(Constants.PUZZLES_ROOT_FOLDER).GetChilds("", ResourceItem.Type.Asset))
            {
                if (item.Ext != "json") continue;
                fastPuzzles.Add(item.GetIDFromPuzzleDataFile(), item);
            }

            UnityEngine.Debug.Log($"Loaded {fastPuzzles.Count} fast puzzles from resources");
        }

        /// <summary>
        /// Load puzzle packs from folder (external puzzle packs)
        /// </summary>
        private void LoadPuzzlePacks()
        {
            externalPuzzlePacks = new Dictionary<string, BasicPuzzlePack>();

            foreach (ResourceItem @event in ResourceDB.GetFolder(Constants.PUZZLE_PACKS_ROOT_FOLDER).GetChilds("", ResourceItem.Type.Folder))
            {
                //get puzzlepack file
                BasicPuzzlePack puzzlePack = new BasicPuzzlePack(@event.GetChilds("", ResourceItem.Type.Asset).First());

                int puzzleIndex = 0;
                //get puzzle descriptions file
                foreach (ResourceItem puzzleDataFile in @event.GetChild("puzzles").GetChilds("", ResourceItem.Type.Asset))
                {
                    ClientPuzzleData puzzleData = new ClientPuzzleData(puzzleDataFile);

                    if (puzzlePack.allRewards.ContainsKey(puzzleIndex))
                        puzzleData.rewards = puzzlePack.allRewards[puzzleIndex].ToArray();
                    else
                        puzzleData.rewards = new RewardsManager.Reward[0];

                    puzzleData.pack = puzzlePack;

                    puzzlePack.puzzlesData.Add(puzzleData);
                    /*if (puzzleData.Enabled) */
                    puzzlePack.enabledPuzzlesData.Add(puzzleData);
                    if (puzzleData.rewards.Length > 0) puzzlePack.rewardPuzzles.Add(puzzleData);

                    puzzleIndex++;
                }

                externalPuzzlePacks.Add(@event.Name, puzzlePack);
            }
        }

        [ContextMenu("ResetOnboarding")]
        public void ResetOnboarding()
        {
            for (int tutorialIndex = 0; tutorialIndex < HardcodedTutorials.tutorials.Count; tutorialIndex++)
            {
                PlayerPrefs.DeleteKey(PlayerPrefsWrapper.kTutorial + HardcodedTutorials.tutorials[tutorialIndex].name);
                PlayerPrefs.DeleteKey(PlayerPrefsWrapper.kTutorialOpened + HardcodedTutorials.tutorials[tutorialIndex].name);
            }
        }

        public static GameObject GetPrefab(PrefabType type)
        {
            if (!Instance.typedPrefabsFastAccess.ContainsKey(type))
                return null;

            return Instance.typedPrefabsFastAccess[type].prefab;
        }

        public static T GetPrefab<T>(PrefabType type) => GetPrefab(type).GetComponent<T>();

        public static T InstantiatePrefab<T>(PrefabType type, Transform parent) where T : Component
        {
            if (!Instance.typedPrefabsFastAccess.ContainsKey(type))
                return null;

            //need this object to have component(T) specified
            if (!Instance.typedPrefabsFastAccess[type].prefab.GetComponent<T>())
                return null;

            GameObject result = Instantiate(Instance.typedPrefabsFastAccess[type].prefab, parent);
            result.transform.localScale = Vector3.one;

            return result.GetComponent<T>();
        }

        [Serializable]
        public class ScreensCollection
        {
            public List<Screen> list;
        }

        [Serializable]
        public class Screen
        {
            [HideInInspector]
            public string _name;

            [StackableField]
            [ShowIf("#Check")]
            public MenuScreen prefab;

            public bool Check()
            {
                _name = prefab ? prefab.name : "No Prefab";

                return true;
            }
        }

        [Serializable]
        public class PrefabsCollection
        {
            public List<PrefabTypePair> list;
        }

        [Serializable]
        public class PrefabTypePair
        {
            [HideInInspector]
            public string _name;

            [StackableField]
            [ShowIf("#Check")]
            public PrefabType prefabType;
            public GameObject prefab;

            public bool Check()
            {
                _name = prefabType.ToString();

                return true;
            }
        }

        public enum PrefabType
        {
            NONE = 0,

            GAME_PIECE_SMALL = 5,
            MINI_GAME_BOARD = 7,
            GAME_PIECE_MEDIUM = 8,
            TOKEN_SMALL = 9,

            #region Rewards
            REWARDS_COIN = 10,
            REWARDS_TICKET = 11,
            REWARDS_GEM = 12,
            REWARDS_GAME_PIECE = 13,
            REWARDS_PORTAL_POINTS = 14,
            REWARDS_RARE_PORTAL_POINTS = 15,
            REWARDS_XP = 16,
            REWARDS_PACK_COMPLETE = 17,
            REWARDS_OPEN_PORTAL = 18,
            REWARDS_OPEN_RARE_PORTAL = 19,

            REWARDS_CUSTOM = 39,
            #endregion

            BOARD_HINT_BOX = 40,
            PUZZLE_PACK_WIDGET = 41,
        }
    }
}


