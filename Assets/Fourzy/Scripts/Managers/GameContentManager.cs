﻿//modded @vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Board;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class GameContentManager : UnitySingleton<GameContentManager>
    {
        public List<GamePiece> gamePiecePrefabs = new List<GamePiece>();
        public List<TokenView> tokenPrefabs = new List<TokenView>();
        public List<Sprite> tokenSprites = new List<Sprite>();
        public List<GameTheme> gameThemes = new List<GameTheme>();
        [Header("Typed Prefabs")]
        public PrefabTypePair[] typedPrefabs;

        private GamePieceData[] gamePieceData = new GamePieceData[0];
        private TokenData[] tokenData = new TokenData[0];
        private Dictionary<int, TokenData> inGameTokensData = new Dictionary<int, TokenData>();
        private Dictionary<Token, TokenView> sortedTokenPrefabs = new Dictionary<Token, TokenView>();
        public Dictionary<PrefabType, PrefabTypePair> typedPrefabsFastAccess { get; private set; }

        private int currentTheme;

        public int CurrentTheme
        {
            get
            {
                return currentTheme;
            }
            set
            {
                currentTheme = value;
                PlayerPrefsWrapper.SetCurrentGameTheme(currentTheme);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            foreach (TokenView token in tokenPrefabs)
                sortedTokenPrefabs[token.tokenType] = token;

            typedPrefabsFastAccess = new Dictionary<PrefabType, PrefabTypePair>();
            foreach (PrefabTypePair prefabTypePair in typedPrefabs)
                if (!typedPrefabsFastAccess.ContainsKey(prefabTypePair.prefabType))
                    typedPrefabsFastAccess.Add(prefabTypePair.prefabType, prefabTypePair);

            currentTheme = PlayerPrefsWrapper.GetCurrentTheme();
        }

        public void UpdateContentData()
        {
            gamePieceData = TokenBoardLoader.Instance.GetAllGamePieces();
            tokenData = TokenBoardLoader.Instance.GetAllTokens();

            for (int i = 0; i < gamePiecePrefabs.Count; i++)
            {
                var piece = gamePieceData[i];

                gamePiecePrefabs[i].gamePieceID = i;
                //gamePiecePrefabs[i].gamePieceView.OutlineColor = piece.OutlineColor;
                //gamePiecePrefabs[i].gamePieceView.SecondaryColor = piece.SecondaryColor;
            }

            foreach (TokenData token in tokenData)
                foreach (int tokenType in token.InGameTokenTypes)
                    inGameTokensData[tokenType] = token;
        }

        public TokenData[] GetAllTokens()
        {
            return tokenData;
        }

        public GamePieceData[] GetAllGamePieces()
        {
            return gamePieceData;
        }

        public int GetGamePieceCount()
        {
            return gamePiecePrefabs.Count;
        }

        public Sprite GetGamePieceSprite(int gamePieceId)
        {
            if (gamePieceId > gamePiecePrefabs.Count - 1)
                return gamePiecePrefabs[0].gamePieceIcon;

            return gamePiecePrefabs[gamePieceId].gamePieceIcon;
        }

        public Sprite GetTokenSprite(int tokenID)
        {
            return tokenSprites[tokenID];
        }

        public TokenData GetTokenData(int tokenId)
        {
            return tokenData[tokenId];
        }

        public TokenData GetTokenDataWithType(Token tokenType)
        {
            return inGameTokensData[(int)tokenType];
        }

        public string GetGamePieceName(int gamePieceId)
        {
            if (gamePieceId >= gamePieceData.Length)
                return string.Empty;

            return gamePieceData[gamePieceId].Name;
        }

        public GamePiece GetGamePiecePrefab(int gamePieceId)
        {
            if (gamePieceId < gamePiecePrefabs.Count && gamePieceId >= 0)
                return gamePiecePrefabs[gamePieceId];
            else
                return gamePiecePrefabs[0];
        }

        public TokenView GetTokenPrefab(Token tokenType, bool justForDisplaying = false)
        {
            TokenView tokenView = null;
            sortedTokenPrefabs.TryGetValue(tokenType, out tokenView);

            if (tokenView != null)
                tokenView.justDisplaying = justForDisplaying;

            return tokenView;
        }

        public GameTheme GetCurrentTheme()
        {
            return gameThemes[currentTheme];
        }

        public static GameObject GetPrefab(PrefabType type)
        {
            if (!Instance.typedPrefabsFastAccess.ContainsKey(type))
                return null;

            return Instance.typedPrefabsFastAccess[type].prefab;
        }

        public static T GetPrefab<T>(PrefabType type)
        {
            return GetPrefab(type).GetComponent<T>();
        }

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

        public static T InstantiatePrefab<T>(PrefabType type) where T : Component
        {
            return InstantiatePrefab<T>(type, null);
        }

        [Serializable]
        public class GameTheme
        {
            public string Name;
            public Sprite Preview;
            public Sprite GameBackground;
            public Sprite GameBackgroundWide;
            public Sprite GameBoard;
        }

        [Serializable]
        public class PrefabTypePair
        {
            public GameObject prefab;
            public PrefabType prefabType;
        }

        public enum PrefabType
        {
            NONE = 0,

            #region UI Prefabs
            COINS_WIDGET_SMALL = 1,
            //
            GAME_PIECE_SMALL = 5,
            MINI_GAME_BOARD_PIECE = 6,
            MINI_GAME_BOARD = 7,
            GAME_PIECE_MEDIUM = 8,
            TOKEN_SMALL = 9,

            PROMPT_SCREEN_GENERIC = 35,
            PROMPT_SCREEN_CAHNGE_NAME = 36,
            PROMPT_SCREEN_TOKEN_INSTUCTION = 37,
            #endregion

            BOARD_HINT_BOX = 40,
        }
    }
}

