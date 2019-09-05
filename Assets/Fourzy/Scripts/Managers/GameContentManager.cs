//modded @vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Camera3D;
using Fourzy._Updates.UI.Menu;
using FourzyGameModel.Model;
using StackableDecorator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class GameContentManager : UnitySingleton<GameContentManager>
    {
        [Sirenix.OdinInspector.BoxGroup("Fast Puzzles")]
        public string fastPuzzlesFolder = "/PuzzlePool/";
        [Sirenix.OdinInspector.BoxGroup("Fast Puzzles")]
        public string puzzlePacksRootPath = "/PuzzlePacks/";

        public List<PuzzlePacksDataHolder> packsDataHolders;
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
        public TutorialsCollection tutorials;
        [List]
        public ScreensCollection screens;

        private Dictionary<string, string> fastPuzzles = new Dictionary<string, string>();

        public Dictionary<PrefabType, PrefabTypePair> typedPrefabsFastAccess { get; private set; }

        public List<PuzzlePacksDataHolder.BasicPuzzlePack> externalPuzzlePacks { get; private set; }

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

        public PuzzlePacksDataHolder puzzlePacksDataHolder => packsDataHolders[0];

        protected override void Awake()
        {
            base.Awake();

            if (InstanceExists) return;

            typedPrefabsFastAccess = new Dictionary<PrefabType, PrefabTypePair>();
            foreach (PrefabTypePair prefabTypePair in typedPrefabs.list)
                if (!typedPrefabsFastAccess.ContainsKey(prefabTypePair.prefabType))
                    typedPrefabsFastAccess.Add(prefabTypePair.prefabType, prefabTypePair);

            //init tutorials
            foreach (Tutorial t in tutorials.list)
                t.Initialize();

            piecesDataHolder.Initialize();
            passAndPlayDataHolder.Initialize();
            miscBoardsDataHolder.Initialize();

            packsDataHolders.ForEach(packsData => packsData.Initialize());

            //LoadAllFastPuzzles();
            //LoadPuzzlePacks();
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

        public Tutorial GetTutorial(string name) => tutorials.list.Find(_tutorial => _tutorial.data.tutorialName == name);

        public ClientFourzyPuzzle GetFastPuzzle(string id = "", bool unfinished = true)
        {
            List<string> ids = new List<string>(fastPuzzles.Keys);

            if (string.IsNullOrEmpty(id))
            {
                //get random one
                ids.Shuffle();
                string _id = "";

                if (unfinished)
                    foreach (string __id in ids)
                        if (!PlayerPrefsWrapper.GetFastPuzzleComplete(__id))
                        {
                            _id = __id;
                            break;
                        }
                else
                    _id = ids[0];

                if (string.IsNullOrEmpty(_id))
                    return GetFastPuzzle(id, false);
                else
                    return new ClientFourzyPuzzle(new ClientPuzzleData(Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(fastPuzzles[_id]))));
            }
            else
            {
                int idIndex = ids.FindIndex(ids.IndexOf(id), __id => !PlayerPrefsWrapper.GetFastPuzzleComplete(__id));

                if (idIndex > -1 && idIndex < ids.Count - 1)
                    return new ClientFourzyPuzzle(new ClientPuzzleData(Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(fastPuzzles[ids[idIndex + 1]]))));
                else
                    return GetFastPuzzle();
            }
        }

        public void ResetPuzzlePacks()
        {
            foreach (PuzzlePacksDataHolder pack in packsDataHolders)
                pack.ResetPlayerPrefs();

            //reset progression maps
            foreach (Camera3dItemProgressionMap progressionMap in progressionMaps)
                progressionMap.ResetPlayerPrefs();

            //reset external packs
            foreach (PuzzlePacksDataHolder.BasicPuzzlePack pack in externalPuzzlePacks)
                pack.ResetPlayerPrefs();
        }

        private void LoadAllFastPuzzles()
        {
            Stopwatch _sw = new Stopwatch();
            _sw.Start();
            foreach (string file in Directory.EnumerateFiles(Application.streamingAssetsPath + fastPuzzlesFolder))
            {
                if (Path.GetExtension(file).ToLower() != ".json" || !Path.GetFileNameWithoutExtension(file).ToLower().Contains("puzzle")) continue;
                fastPuzzles.Add(File.ReadAllText(file).Substring(7, 36), file);
            }
            _sw.Stop();
            UnityEngine.Debug.Log($"Took {_sw.ElapsedMilliseconds}ms to find/pparse {fastPuzzles.Count}");
        }

        /// <summary>
        /// Load puzzle packs from folder (external puzzle packs)
        /// </summary>
        private void LoadPuzzlePacks()
        {
            externalPuzzlePacks = new List<PuzzlePacksDataHolder.BasicPuzzlePack>();

            Stopwatch _sw = new Stopwatch();
            _sw.Start();

            //load external puzzle packs
            foreach (string packFolder in Directory.EnumerateDirectories(Application.streamingAssetsPath + puzzlePacksRootPath))
            {
                List<ClientPuzzleData> puzzles = new List<ClientPuzzleData>();

                PuzzlePacksDataHolder.BasicPuzzlePack puzzlePack = new PuzzlePacksDataHolder.BasicPuzzlePack();

                string filename = Path.GetFileName(packFolder);

                foreach (string file in Directory.EnumerateFiles(packFolder))
                {
                    if (Path.GetExtension(file).ToLower() != ".json" || !Path.GetFileNameWithoutExtension(file).ToLower().Contains("puzzle")) continue;

                    ClientPuzzleData puzzleData = new ClientPuzzleData(filename + "_" + File.ReadAllText(file).Substring(7, 36), file);
                    puzzleData.pack = puzzlePack;
                    puzzleData.PackID = filename;

                    puzzles.Add(puzzleData);
                }

                if (puzzles.Count == 0) continue;

                puzzlePack.name = filename;
                puzzlePack.packID = filename;
                puzzlePack.packType = PuzzlePacksDataHolder.PackType.PUZZLE_PACK;

                puzzlePack.Initialize();

                //parse puzzles
                for (int puzzleIndex = 0; puzzleIndex < puzzles.Count; puzzleIndex++)
                {
                    puzzlePack.puzzlesData.Add(puzzles[puzzleIndex]);
                    puzzlePack.puzzlesData.Add(puzzles[puzzleIndex]);
                    puzzlePack.enabledPuzzlesData.Add(puzzles[puzzleIndex]);
                }

                externalPuzzlePacks.Add(puzzlePack);
            }

            _sw.Stop();
            UnityEngine.Debug.Log($"Loaded external puzzle packs {externalPuzzlePacks.Count}");
        }

        [ContextMenu("ResetOnboarding")]
        public void ResetOnboarding()
        {
            for (int tutorialIndex = 0; tutorialIndex < tutorials.list.Count; tutorialIndex++)
            {
                PlayerPrefs.DeleteKey(PlayerPrefsWrapper.kTutorial + tutorials.list[tutorialIndex].data.name);
                PlayerPrefs.DeleteKey(PlayerPrefsWrapper.kTutorialOpened + tutorials.list[tutorialIndex].data.name);
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
        public class TutorialsCollection
        {
            public List<Tutorial> list;
        }

        [Serializable]
        public class Tutorial
        {
            [HideInInspector]
            public string _name;
            [HideInInspector]
            public bool previousState;

            [StackableField]
            [ShowIf("#Check")]
            public OnboardingDataHolder data;

            public bool wasFinishedThisSession => !previousState && PlayerPrefsWrapper.GetTutorialFinished(data);

            public void Initialize()
            {
                if (!data) return;

                previousState = PlayerPrefsWrapper.GetTutorialFinished(data);
            }

            public bool Check()
            {
                _name = data ? data.tutorialName : "No data specified.";

                return true;
            }
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
            #endregion

            BOARD_HINT_BOX = 40,
            PUZZLE_PACK_WIDGET = 41,
        }
    }
}


