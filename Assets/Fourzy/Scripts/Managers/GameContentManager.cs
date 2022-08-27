//modded @vadym udod

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Camera3D;
using Fourzy._Updates.UI.Menu;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if !MOBILE_SKILLZ
using PlayFab;
using PlayFab.ClientModels;
#endif

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
        public MiscGameContentHolder miscGameDataHolder;
        public SkillzMissonRewardsDataHolder skillzMissionRewardsDataHolder;
        [ListDrawerSettings(ListElementLabelName = "type", NumberOfItemsPerPage = 6)]
        public List<PrefabTypePair> typedPrefabs;
        [ListDrawerSettings(NumberOfItemsPerPage = 10)]
        public List<MenuScreen> screens;

        internal Dictionary<string, ResourceItem> fastPuzzles = new Dictionary<string, ResourceItem>();
        internal Dictionary<string, BasicPuzzlePack> externalPuzzlePacks { get; private set; }
        internal Dictionary<string, GameObject> typedPrefabsFastAccess { get; private set; }
        internal List<ResourceItem> instructionBoards { get; private set; }
        internal List<ResourceItem> realtimeBotBoards { get; private set; }
        internal List<GameBoardDefinition> passAndPlayBoards { get; private set; }
        internal List<ResourceItem> skillzBoards { get; private set; }
        internal List<ResourceItem> miscBoards { get; private set; }

        internal AreasDataHolder.GameArea currentArea
        {
            get => areasDataHolder.currentAreaData;
            set => areasDataHolder.currentAreaData = value;
        }

        internal List<TokensDataHolder.TokenData> tokens => tokensDataHolder.tokens;

        internal List<string> bundlesInPlayerInventory { get; set; } = new List<string>();

#if !MOBILE_SKILLZ
        internal Dictionary<string, CatalogItem> allItemsInfo;
#endif

        internal IEnumerable<TokensDataHolder.TokenData> unlockedTokensData
        {
            get
            {
                var unlockedTokens = PlayerPrefsWrapper.GetUnlockedTokens().Select(_data => _data.tokenType);

                return tokensDataHolder.tokens.Where(_tokenData => unlockedTokens.Contains(_tokenData.tokenType));
            }
        }

        internal int finishedFastPuzzlesCount =>
            fastPuzzles.Keys.Where(id => PlayerPrefsWrapper.GetFastPuzzleComplete(id)).Count();

        internal List<Camera3dItemProgressionMap> existingProgressionMaps { get; private set; } =
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

            //load fast puzzles
            switch (GameManager.Instance.buildIntent)
            {
                case BuildIntent.MOBILE_REGULAR:
                    LoadAllFastPuzzles();
                    LoadPuzzlePacks();
                    LoadTutorialBotGames();
                    break;

                case BuildIntent.MOBILE_INFINITY:
                    LoadPuzzlePacks();
                    break;

                case BuildIntent.MOBILE_SKILLZ:
                    LoadSkillzBoards();
                    break;
            }

            LoadInstructionBoards();
            LoadMiscBoards();
            if (GameManager.Instance.buildIntent != BuildIntent.MOBILE_SKILLZ)
            {
                LoadPassAndPlayBoards();
            }
        }

        public IEnumerable<GamePieceData> SkillzMissionRewardsGamePieces()
        {
            return skillzMissionRewardsDataHolder.Groups
                .SelectMany(group => group.pieces)
                .Select(gamePieceId => piecesDataHolder.GetGamePieceData(gamePieceId));
        }

        public void GetItemsCataloge()
        {
#if !MOBILE_SKILLZ
            PlayFabClientAPI.GetCatalogItems(
                new GetCatalogItemsRequest(),
                result =>
                {
                    allItemsInfo = new Dictionary<string, CatalogItem>();

                    if (Application.isEditor || Debug.isDebugBuild)
                    {
                        Debug.Log($"Pulled {result.Catalog.Count} items from server");
                    }

                    foreach (CatalogItem item in result.Catalog)
                    {
                        allItemsInfo.Add(item.ItemId, item);

                        switch (item.ItemClass)
                        {
                            case Constants.PLAYFAB_GAMEPIECE_CLASS:
                                GamePieceData data = piecesDataHolder.GetGamePieceData(item.ItemId);
                                if (data == null)
                                {
                                    Debug.LogError($"Unknown gamepiece {item.ItemId}");
                                }
                                else
                                {
                                    JObject _customData = JObject.Parse(item.CustomData);
                                    data.PiecesToUnlock = _customData.Value<int>("toUnlock");
                                }

                                break;
                        }
                    }

                    UserManager.Instance.AddPlayfabValueLoaded(PlayfabValuesLoaded.CATALOG_INFO_RECEIVED);
                },
                error =>
                {
                    GameManager.Instance.ReportPlayFabError(error.ErrorMessage);
                });
#endif
        }

#if !MOBILE_SKILLZ
        public CatalogItem GetCatalogItem(string itemId)
        {
            if (allItemsInfo.ContainsKey(itemId))
            {
                return allItemsInfo[itemId];
            }
            else
            {
                return null;
            }
        }

        public CatalogItem GetFirstInBundle(string bundleId)
        {
            if (allItemsInfo.ContainsKey(bundleId) &&
                allItemsInfo[bundleId].Bundle != null && 
                allItemsInfo[bundleId].Bundle.BundledItems != null && 
                allItemsInfo[bundleId].Bundle.BundledItems.Count > 0)
            {
                return GetCatalogItem(allItemsInfo[bundleId].Bundle.BundledItems[0]);
            }
            else
            {
                return null;
            }
        }
#endif

        public GameBoardDefinition GetInstructionBoard(string boardFileName)
        {
            ResourceItem _file = instructionBoards.Find(file => file.Name == boardFileName);

            if (_file != null)
            {
                return JsonConvert.DeserializeObject<GameBoardDefinition>(_file.Load<TextAsset>().text);
            }
            else
            {
                return null;
            }
        }

        public GameBoardDefinition GetMiscBoard(string boardFileName)
        {
            ResourceItem _file = miscBoards.Find(file => file.Name == boardFileName);

            if (_file != null)
            {
                return JsonConvert.DeserializeObject<GameBoardDefinition>(_file.Load<TextAsset>().text);
            }
            else
            {
                return null;
            }
        }

        public GameBoardDefinition GetRandomSkillzBoard()
        {
            ResourceItem _file = skillzBoards[UnityEngine.Random.Range(0, skillzBoards.Count-1)];
            return JsonConvert.DeserializeObject<GameBoardDefinition>(_file.Load<TextAsset>().text);
        }

        public void StartTryItBoard(TokenType token)
        {
            ResourceItem _boardTextFile = ResourceDB
                .GetFolder(Constants.TRY_IT_BOARDS_FOLDER)
                .GetChild(token.ToString(), ResourceItem.Type.Asset);

            if (_boardTextFile == null)
            {
                Debug.LogError($"Failed to find Try It board for {token}");
                return;
            }
            else
            {
                ClientFourzyGame _game = new ClientFourzyGame
                    (JsonConvert.DeserializeObject<GameBoardDefinition>(_boardTextFile.Load<TextAsset>().text),
                    UserManager.Instance.meAsPlayer,
                    new Player(2, "Bot", AIProfile.BadBot))
                {
                    _Type = GameType.TRY_TOKEN,
                };
                _game.UpdateFirstState();

                GameManager.Instance.StartGame(_game, GameTypeLocal.LOCAL_GAME);
            }
        }

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
                .GetFolder(Constants.FAST_PUZZLES_FOLDER)
                .GetChilds("", ResourceItem.Type.Asset)
                .Where(_file => _file.Ext == "json"))
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
                .GetFolder(Constants.PUZZLE_PACKS_FOLDER)
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

        /// <summary>
        /// Tutorial boards for realtime games
        /// </summary>
        private void LoadTutorialBotGames()
        {
            realtimeBotBoards = new List<ResourceItem>(ResourceDB
                .GetFolder(Constants.REALTIME_BOT_BOARDS_FOLDER)
                .GetChilds("", ResourceItem.Type.Asset)
                .Where(_file => _file.Ext == "json")
                .OrderBy(_file => int.Parse(_file.Name.Split(' ')[0])));

            Debug.Log($"Loaded {realtimeBotBoards.Count} tutorial bot games.");
        }

        /// <summary>
        /// Token instruction boards
        /// </summary>
        private void LoadInstructionBoards()
        {
            instructionBoards = new List<ResourceItem>(ResourceDB
                .GetFolder(Constants.INSTRUCTION_BOARDS_FOLDER)
                .GetChilds("", ResourceItem.Type.Asset)
                .Where(_file => _file.Ext == "json"));

            if (Application.isEditor || Debug.isDebugBuild)
            {
                Debug.Log($"Loaded {instructionBoards.Count} instruction boards");
            }
        }

        /// <summary>
        /// Onboarding boards
        /// </summary>
        private void LoadMiscBoards()
        {
            miscBoards = new List<ResourceItem>(ResourceDB
                .GetFolder(Constants.MISC_BOARDS_FOLDER)
                .GetChilds("", ResourceItem.Type.Asset)
                .Where(_file => _file.Ext == "json"));

            if (Application.isEditor || Debug.isDebugBuild)
            {
                Debug.Log($"Loaded {miscBoards.Count} misc boards");
            }
        }

        /// <summary>
        /// Pass and play boards
        /// </summary>
        private void LoadPassAndPlayBoards()
        {
            passAndPlayBoards = new List<GameBoardDefinition>(ResourceDB
                .GetFolder(Constants.PASS_AND_PLAY_BOARDS_FOLDER)
                .GetChild("used", ResourceItem.Type.Folder)
                .GetChilds("", ResourceItem.Type.Asset)
                .Where(_file => _file.Ext == "json")
                .Select(_file => JsonConvert.DeserializeObject<GameBoardDefinition>(_file.Load<TextAsset>().text)));

            if (Application.isEditor || Debug.isDebugBuild)
            {
                Debug.Log($"Loaded {passAndPlayBoards.Count} pass and play boards");
            }
        }

        /// <summary>
        /// Skillz boards
        /// </summary>
        private void LoadSkillzBoards()
        {
            skillzBoards = new List<ResourceItem>(ResourceDB
                .GetFolder(Constants.SKILLZ_BOARDS_FOLDER)
                .GetChild("Z2", ResourceItem.Type.Folder)
                .GetChilds("", ResourceItem.Type.Asset)
                .Where(_file => _file.Ext == "json"));

            if (Application.isEditor || Debug.isDebugBuild)
            {
                Debug.Log($"Loaded {skillzBoards.Count} skillz boards");
            }
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

    /// <summary>
    /// As result of GameManager.ReportAreaProgression call
    /// </summary>
    [Serializable]
    public class ProgressionReward
    {
        public string itemId;
        public string itemClass;
        public int count;
    }
}


