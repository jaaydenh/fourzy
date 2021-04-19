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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class GameContentManager : UnitySingleton<GameContentManager>
    {
        public List<Camera3dItemProgressionMap> progressionMaps;
        public GamePiecesDataHolder piecesDataHolder;
        public AIPlayersDataHolder aiPlayersDataHolder;
        public TokensDataHolder tokensDataHolder;
        public AreasDataHolder areasDataHolder;
        public PassAndPlayDataHolder passAndPlayDataHolder;
        public MiscBoardsDataHolder miscBoardsDataHolder;
        public MiscGameContentHolder miscGameDataHolder;
        [ListDrawerSettings(NumberOfItemsPerPage = 6)]
        public List<PrefabTypePair> typedPrefabs;
        [ListDrawerSettings(NumberOfItemsPerPage = 6)]
        public List<MenuScreen> screens;

        private Dictionary<string, ResourceItem> fastPuzzles = new Dictionary<string, ResourceItem>();

        public Dictionary<string, BasicPuzzlePack> externalPuzzlePacks { get; private set; }

        public Dictionary<string, GameObject> typedPrefabsFastAccess { get; private set; }

        public List<ResourceItem> realtimeBotBoards { get; private set; }

        public AreasDataHolder.GameArea currentArea
        {
            get => areasDataHolder.currentAreaData;
            set => areasDataHolder.currentAreaData = value;
        }

        public List<AreasDataHolder.GameArea> enabledAreas => 
            areasDataHolder.areas.Where(theme => theme.enabled).ToList();

        public List<TokensDataHolder.TokenData> tokens => 
            tokensDataHolder.tokens.list;

        public List<TokensDataHolder.TokenData> enabledTokens => 
            tokensDataHolder.tokens.list.Where(token => token.enabled).ToList();

        public List<GameBoardDefinition> passAndPlayGameboards => 
            passAndPlayDataHolder.gameboards;

        public int finishedFastPuzzlesCount => 
            fastPuzzles.Keys.Where(id => PlayerPrefsWrapper.GetFastPuzzleComplete(id)).Count();

        public List<Camera3dItemProgressionMap> existingProgressionMaps { get; private set; } = 
            new List<Camera3dItemProgressionMap>();

        protected override void Awake()
        {
            base.Awake();

            if (InstanceExists) return;

            typedPrefabsFastAccess = new Dictionary<string, GameObject>();
            foreach (PrefabTypePair prefabTypePair in typedPrefabs)
            {
                if (!typedPrefabsFastAccess.ContainsKey(prefabTypePair.type))
                {
                    typedPrefabsFastAccess.Add(prefabTypePair.type, prefabTypePair.prefab);
                }
            }

            piecesDataHolder.Initialize();
            passAndPlayDataHolder.Initialize();
            miscBoardsDataHolder.Initialize();

            LoadAllFastPuzzles();
            LoadPuzzlePacks();
            LoadTutorialBotGames();
        }

        public GameBoardDefinition GetMiscBoard(string boardID) => 
            miscBoardsDataHolder.gameboards.Find(board => board.ID == boardID);

        public GameBoardDefinition GetMiscBoardByName(string boardName) => 
            miscBoardsDataHolder.gameboards.Find(board => board.BoardName == boardName);

        public GameBoardDefinition GetPassAndPlayBoard(string boardID) => 
            passAndPlayDataHolder.gameboards.Find(board => board.ID == boardID);

        public GameBoardDefinition GetPassAndPlayBoardByName(string boardName) => 
            passAndPlayDataHolder.gameboards.Find(board => board.BoardName == boardName);

        public TokenView GetTokenPrefab(TokenType tokenType, Area theme) => 
            tokensDataHolder.GetToken(tokenType, theme);

        public TokenView GetTokenPrefab(TokenType tokenType) => 
            GetTokenPrefab(tokenType, areasDataHolder.areas[0].areaID);

        public TokensDataHolder.TokenData GetTokenData(TokenType tokenType) => 
            tokensDataHolder.GetTokenData(tokenType);

        public List<AreasDataHolder.GameArea> GetTokenThemes(TokenType tokenType) => 
            GetTokenData(tokenType)?.GetTokenAreas(areasDataHolder) ?? null;

        public List<string> GetTokenAreaNames(TokenType tokenType) => 
            GetTokenData(tokenType)?.GetAreaNames(areasDataHolder) ?? null;

        public ClientFourzyPuzzle GetNextFastPuzzle(string id = "")
        {
            List<string> ids = new List<string>(fastPuzzles.Keys);

            if (string.IsNullOrEmpty(id))
            {
                //get random one
                ids.Shuffle();
                string _id = "";
                
                foreach (string __id in ids)
                {
                    if (!PlayerPrefsWrapper.GetFastPuzzleComplete(__id))
                    {
                        _id = __id;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(_id))
                {
                    return new ClientFourzyPuzzle(new ClientPuzzleData(fastPuzzles[ids[0]]).Initialize());
                }
                else
                {
                    return new ClientFourzyPuzzle(new ClientPuzzleData(fastPuzzles[_id]).Initialize());
                }
            }
            else
            {
                int idIndex = ids.FindIndex(ids.IndexOf(id), 
                    __id => !PlayerPrefsWrapper.GetFastPuzzleComplete(__id));

                if (idIndex > -1 && idIndex < ids.Count - 1)
                {
                    return new ClientFourzyPuzzle(new ClientPuzzleData(fastPuzzles[ids[idIndex + 1]]).Initialize());
                }
                else
                {
                    return GetNextFastPuzzle();
                }
            }
        }

        public ClientFourzyPuzzle GetFastPuzzle(string id) => 
            new ClientFourzyPuzzle(new ClientPuzzleData(fastPuzzles[id]).Initialize());

        public void ResetFastPuzzles()
        {
            foreach (string id in fastPuzzles.Keys)
            {
                PlayerPrefsWrapper.SetFastPuzzleComplete(id, false);
            }
        }

        public void ResetPuzzlePacks()
        {
            foreach (BasicPuzzlePack pack in externalPuzzlePacks.Values)
            {
                pack.ResetPlayerPrefs();
            }

            foreach (Camera3dItemProgressionMap progressionMap in progressionMaps)
            {
                (existingProgressionMaps.Find(__map => __map.mapID == progressionMap.mapID) ?? progressionMap)
                    .ResetPlayerPrefs();
            }
        }

        public BasicPuzzlePack GetExternalPuzzlePack(string folderName) 
            => externalPuzzlePacks[folderName];

        private void LoadAllFastPuzzles()
        {
            foreach (ResourceItem item in ResourceDB
                .GetFolder(Constants.PUZZLES_ROOT_FOLDER)
                .GetChilds("", ResourceItem.Type.Asset))
            {
                if (item.Ext != "json") continue;

                fastPuzzles.Add(item.GetIDFromPuzzleDataFile(), item);
            }

            Debug.Log($"Loaded {fastPuzzles.Count} fast puzzles from resources");
        }

        /// <summary>
        /// Load puzzle packs from folder (external puzzle packs)
        /// </summary>
        private void LoadPuzzlePacks()
        {
            externalPuzzlePacks = new Dictionary<string, BasicPuzzlePack>();

            foreach (ResourceItem @event in ResourceDB
                .GetFolder(Constants.PUZZLE_PACKS_ROOT_FOLDER)
                .GetChilds("", ResourceItem.Type.Folder))
            {
                IEnumerable<ResourceItem> items = @event.GetChilds("", ResourceItem.Type.Asset);
                if (items.Count() == 0) continue;

                //get puzzlepack file
                BasicPuzzlePack puzzlePack = new BasicPuzzlePack(items.First());

                int puzzleIndex = 0;
                //get puzzle descriptions file
                foreach (ResourceItem puzzleDataFile in @event
                    .GetChild("puzzles")
                    .GetChilds("", ResourceItem.Type.Asset))
                {
                    ClientPuzzleData puzzleData = new ClientPuzzleData(puzzleDataFile);

                    if (puzzlePack.allRewards.ContainsKey(puzzleIndex))
                    {
                        puzzleData.rewards = puzzlePack.allRewards[puzzleIndex].ToArray();
                    }
                    else
                    {
                        puzzleData.rewards = new RewardsManager.Reward[0];
                    }

                    puzzleData.pack = puzzlePack;

                    puzzlePack.puzzlesData.Add(puzzleData);
                    if (puzzleData.Enabled)
                    {
                        puzzlePack.enabledPuzzlesData.Add(puzzleData);

                        if (puzzleData.rewards.Length > 0)
                        {
                            puzzlePack.rewardPuzzles.Add(puzzleData);
                        }
                    }

                    puzzleIndex++;
                }

                externalPuzzlePacks.Add(@event.Name, puzzlePack);
            }
        }

        private void LoadTutorialBotGames()
        {
            //get realtime bot boards
            realtimeBotBoards = new List<ResourceItem>(ResourceDB
                .GetFolder(Constants.REALTIME_BOT_BOARDS)
                .GetChilds("", ResourceItem.Type.Asset)
                .OrderBy(_board => int.Parse(_board.Name.Split(' ')[0])));

            Debug.Log($"Loaded {realtimeBotBoards.Count} tutorial bot games.");
        }

        internal ResourceItem GetRealtimeBotBoard(int index)
        {
            if (index < realtimeBotBoards.Count)
            {
                return realtimeBotBoards[index];
            }

            return null;
        }

        [ContextMenu("ResetOnboarding")]
        public void ResetOnboarding()
        {
            for (int tutorialIndex = 0; tutorialIndex < HardcodedTutorials.tutorials.Count; tutorialIndex++)
            {
                PlayerPrefs.DeleteKey("tutorial_" + HardcodedTutorials.tutorials[tutorialIndex].name);
                PlayerPrefs.DeleteKey("tutorialOpened_" + HardcodedTutorials.tutorials[tutorialIndex].name);
            }
        }

        public static GameObject GetPrefab(string type)
        {
            if (!Instance.typedPrefabsFastAccess.ContainsKey(type))
                return null;

            return Instance.typedPrefabsFastAccess[type];
        }

        public static T GetPrefab<T>(string type) => 
            GetPrefab(type).GetComponent<T>();

        public static T InstantiatePrefab<T>(string type, Transform parent) where T : Component
        {
            if (!Instance.typedPrefabsFastAccess.ContainsKey(type))
                return null;

            //need this object to have component(T) specified
            if (!Instance.typedPrefabsFastAccess[type].GetComponent<T>())
                return null;

            GameObject result = Instantiate(Instance.typedPrefabsFastAccess[type], parent);
            result.transform.localScale = Vector3.one;

            return result.GetComponent<T>();
        }

        [Serializable]
        public class PrefabsCollection
        {
            public List<PrefabTypePair> list;
        }

        [Serializable]
        public class PrefabTypePair
        {
            public string type;
            public GameObject prefab;
        }
    }
}


