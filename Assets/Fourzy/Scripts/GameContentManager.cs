using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    [System.Serializable]
    public class GameTheme
    {
        public string Name;
        public Sprite Preview;
        public Sprite GameBackground;
        public GameBoardView GameBoardView;
        public Sprite GameBoard;
    }

    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class GameContentManager : UnitySingleton<GameContentManager>
    {
        [SerializeField] List<GamePiece> gamePiecePrefabs = new List<GamePiece>();
        [SerializeField] List<TokenView> tokenPrefabs = new List<TokenView>();
        [SerializeField] List<Sprite> tokenSprites = new List<Sprite>();
        [SerializeField] List<GameTheme> gameThemes = new List<GameTheme>();

        private GamePieceData[] gamePieceData = new GamePieceData[0];
        private TokenData[] tokenData = new TokenData[0];
        private Dictionary<int, TokenData> inGameTokensData = new Dictionary<int, TokenData>();
        private Dictionary<Token, TokenView> sortedTokenPrefabs = new Dictionary<Token, TokenView>();

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

        private int currentTheme;

        protected override void Awake()
        {
            base.Awake();

            foreach (TokenView token in tokenPrefabs)
            {
                sortedTokenPrefabs[token.tokenType] = token;
            }

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
                gamePiecePrefabs[i].View.OutlineColor = piece.OutlineColor;
                gamePiecePrefabs[i].View.SecondaryColor = piece.SecondaryColor;
            }

            foreach (TokenData t in tokenData)
            {
                foreach (int tokenType in t.InGameTokenTypes)
                {
                    inGameTokensData[tokenType] = t;
                }
            }
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
            {
                return gamePiecePrefabs[0].gamePieceIcon;
            }
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
            return gamePieceData[gamePieceId].Name;
        }

        public GamePiece GetGamePiecePrefab(int gamePieceId)
        {
            if (gamePieceId < gamePiecePrefabs.Count && gamePieceId >= 0)
            {
                return gamePiecePrefabs[gamePieceId];
            }
            else
            {
                return gamePiecePrefabs[0];
            }
        }

        public GameObject GetTokenPrefab(Token tokenType, bool justForDisplaying = false)
        {
            TokenView tokenView;
            sortedTokenPrefabs.TryGetValue(tokenType, out tokenView);
            GameObject token = null;
            if (tokenView != null)
            {
                tokenView.justDisplaying = justForDisplaying;
                token = tokenView.gameObject;
            }
            return token;
        }

        public GameTheme GetCurrentTheme()
        {
            return gameThemes[currentTheme];
        }
    }
}


