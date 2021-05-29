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
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class GameContentManager : UnitySingleton<GameContentManager>
    {
        public static Action onBundlesLoaded;

        public List<Camera3dItemProgressionMap> progressionMaps;
        public GamePiecesDataHolder piecesDataHolder;
        public AIPlayersDataHolder aiPlayersDataHolder;
        public TokensDataHolder tokensDataHolder;
        public AreasDataHolder areasDataHolder;
        public MiscGameContentHolder miscGameDataHolder;
        [ListDrawerSettings(ListElementLabelName = "type", NumberOfItemsPerPage = 6)]
        public List<PrefabTypePair> typedPrefabs;
        [ListDrawerSettings(NumberOfItemsPerPage = 10)]
        public List<MenuScreen> screens;

        internal Dictionary<string, ResourceItem> fastPuzzles = new Dictionary<string, ResourceItem>();
        internal Dictionary<string, BasicPuzzlePack> externalPuzzlePacks { get; private set; }
        internal Dictionary<string, GameObject> typedPrefabsFastAccess { get; private set; }
        internal List<GameBoardDefinition> miscBoards { get; private set; }
        internal List<ResourceItem> realtimeBotBoards { get; private set; }
        internal List<GameBoardDefinition> passAndPlayBoards { get; private set; }

        internal AreasDataHolder.GameArea currentArea
        {
            get => areasDataHolder.currentAreaData;
            set => areasDataHolder.currentAreaData = value;
        }

        internal List<AreasDataHolder.GameArea> enabledAreas =>
            areasDataHolder.areas.Where(theme => theme.enabled).ToList();

        internal List<TokensDataHolder.TokenData> tokens => tokensDataHolder.tokens;

        internal List<string> bundlesInPlayerInventory { get; set; } = new List<string>();

        internal Dictionary<string, BundleInfo> allBundlesInfo;

        internal IEnumerable<TokensDataHolder.TokenData> unlockedTokensData
        {
            get
            {
                var unlockedTokens = PlayerPrefsWrapper.GetUnlockedTokens().Select(_data => _data.tokenType);

                return tokensDataHolder.tokens.Where(_tokenData => unlockedTokens.Contains(_tokenData.tokenType));
            }
        }

        internal Dictionary<Area, AreaProgression> defaultAreaProgression;

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

            LoadAllFastPuzzles();
            LoadPuzzlePacks();
            LoadTutorialBotGames();
            LoadAreasProgression();
            LoadMiscBoards();
            LoadPassAndPlayBoards();
        }

        public void GetBundlesInfo()
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "getBundlesData",
                GeneratePlayStreamEvent = true,
            },
            result =>
            {
                allBundlesInfo = new Dictionary<string, BundleInfo>();

                foreach (BundleInfo info in JsonConvert.DeserializeObject<BundleInfo[]>(result.FunctionResult.ToString()))
                {
                    allBundlesInfo.Add(info.BundleId, info);
                }

                if (Application.isEditor || Debug.isDebugBuild)
                {
                    Debug.Log($"Pulled {allBundlesInfo.Count} bundles from server");
                }

                onBundlesLoaded?.Invoke();

                UserManager.Instance.AddPlayfabValueLoaded(PlayfabValuesLoaded.BUNDLES_INFO_RECEIVED);
            },
            error =>
            {
                GameManager.Instance.ReportPlayFabError(error.ErrorMessage);
            });
        }

        public GameBoardDefinition GetMiscBoard(string boardID) => miscBoards.Find(board => board.ID == boardID);

        public void StartTryItBoard(TokenType token)
        {
            ResourceItem _boardTextFile =  ResourceDB
                .GetFolder(Constants.TRYI_IT_BOARDS_FOLDER)
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

        private void LoadTutorialBotGames()
        {
            realtimeBotBoards = new List<ResourceItem>(ResourceDB
                .GetFolder(Constants.REALTIME_BOT_BOARDS_FOLDER)
                .GetChilds("", ResourceItem.Type.Asset)
                .Where(_file => _file.Ext == "json")
                .OrderBy(_file => int.Parse(_file.Name.Split(' ')[0])));

            Debug.Log($"Loaded {realtimeBotBoards.Count} tutorial bot games.");
        }

        private void LoadAreasProgression()
        {
            defaultAreaProgression = new Dictionary<Area, AreaProgression>();

            foreach (ResourceItem resourceItem in ResourceDB
                .GetFolder(Constants.AREAS_PROGRESSION_FOLDER)
                .GetChilds("", ResourceItem.Type.Asset)
                .Where(_file => _file.Ext == "json"))
            {
                defaultAreaProgression.Add(
                    (Area)Enum.Parse(typeof(Area), resourceItem.Name),
                    AreaProgression.FromJsonString(resourceItem.Load<TextAsset>().text));
            }
        }

        private void LoadMiscBoards()
        {
            miscBoards = new List<GameBoardDefinition>(ResourceDB
                .GetFolder(Constants.MISC_BOARDS_FOLDER)
                .GetChilds("", ResourceItem.Type.Asset)
                .Where(_file => _file.Ext == "json")
                .Select(_file => JsonConvert.DeserializeObject<GameBoardDefinition>(_file.Load<TextAsset>().text)));

            if (Application.isEditor || Debug.isDebugBuild)
            {
                Debug.Log($"Loaded {miscBoards.Count} misc boards");
            }
        }

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
                Debug.Log($"Loaded {miscBoards.Count} pass and play boards");
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
    /// Playdfab bundles info
    /// </summary>
    [Serializable]
    public class BundleInfo
    {
        public string BundleId;

        public BundleItem[] Items;

        public BundleItem GetFirstItem() => Items.First(
            _item => _item.ItemClass == Constants.PLAYFAB_GAMEPIECE_CLASS ||
            _item.ItemClass == Constants.PLAYFAB_TOKEN_CLASS);
    }

    [Serializable]
    public class BundleItem
    {
        public string ItemId;
        public string ItemClass;
    }

    /// <summary>
    /// As result of GameManager.ReportAreaProgression call
    /// </summary>
    [Serializable]
    public class ProgressionReward
    {
        public string itemId;
        public string itemClass;
    }
}


