//modded @vadym udod

using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Serialized;
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
        public PuzzlePacksDataHolder puzzlePacksDataHolder;
        public GamePiecesDataHolder piecesDataHolder;
        public AIPlayersDataHolder aiPlayersDataHolder;
        public TokensDataHolder tokensDataHolder;
        public ThemesDataHolder themesDataHolder;
        public PassAndPlayDataHolder passAndPlayDataHolder;
        public MiscBoardsDataHolder miscBoardsDataHolder;
        [List]
        public PrefabsCollection typedPrefabs;
        [List]
        public TutorialsCollection tutorials;
        [List]
        public ScreensCollection screens;

        public Dictionary<PrefabType, PrefabTypePair> typedPrefabsFastAccess { get; private set; }

        public ThemesDataHolder.GameTheme currentTheme
        {
            get => themesDataHolder.currentTheme;

            set => themesDataHolder.currentTheme = value;
        }

        public List<PuzzlePacksDataHolder.PuzzlePack> puzzlePacks => puzzlePacksDataHolder.puzzlePacks.list;

        public List<ThemesDataHolder.GameTheme> themes => themesDataHolder.themes.list;

        public List<ThemesDataHolder.GameTheme> enabledThemes => themesDataHolder.themes.list.Where(theme => theme.enabled).ToList();

        public List<TokensDataHolder.TokenData> tokens => tokensDataHolder.tokens.list;

        public List<TokensDataHolder.TokenData> enabledTokens => tokensDataHolder.tokens.list.Where(token => token.enabled).ToList();

        public List<GameBoardDefinition> passAndPlayGameboards => passAndPlayDataHolder.gameboards;

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
            puzzlePacksDataHolder.Initialize();
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
            #endregion

            BOARD_HINT_BOX = 40,
        }
    }
}


