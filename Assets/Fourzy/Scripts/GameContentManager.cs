using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene, false)]
    public class GameContentManager : UnitySingleton<GameContentManager>
    {
        [SerializeField]
        private List<GamePiece> gamePiecePrefabs = new List<GamePiece>();

        [SerializeField]
        private List<TokenView> tokenPrefabs = new List<TokenView>();

        [SerializeField]
        private List<Sprite> gamePieceSprites = new List<Sprite>();

        [SerializeField]
        private List<Sprite> tokenSprites = new List<Sprite>();

        private GamePieceData[] gamePieceData = new GamePieceData[0];
        private TokenData[] tokenData = new TokenData[0];
        private Dictionary<int, TokenData> inGameTokensData = new Dictionary<int, TokenData>();
        private Dictionary<Token, TokenView> sortedTokenPrefabs = new Dictionary<Token, TokenView>();

        private void Awake()
        {
            foreach (TokenView token in tokenPrefabs)
            {
                sortedTokenPrefabs[token.tokenType] = token;
            }
        }

        public void UpdateContentData()
        {
            gamePieceData = TokenBoardLoader.instance.GetAllGamePieces();
            tokenData = TokenBoardLoader.instance.GetAllTokens();

            for (int i = 0; i < gamePiecePrefabs.Count; i++)
            {
                var piece = gamePieceData[i];
                gamePiecePrefabs[i].View.OutlineColor = piece.OutlineColorWrapper;
                gamePiecePrefabs[i].SecondaryColor = piece.SecondaryColorWrapper;
            }

            foreach(TokenData t in tokenData)
            {
                foreach(int tokenType in t.InGameTokenTypes)
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
            return gamePieceSprites.Count;
        }

        public Sprite GetGamePieceSprite(int gamePieceId)
        {
            return gamePieceSprites[gamePieceId];
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

        static int test = 0;

        public GameObject GetGamePiecePrefab(int gamePieceId)
        {
            test = (test + 1) % gamePiecePrefabs.Count;
            return gamePiecePrefabs[test].gameObject;

            if (gamePieceId < gamePiecePrefabs.Count && gamePieceId >= 0)
            {
                return gamePiecePrefabs[gamePieceId].gameObject;
            }
            else
            {
                return gamePiecePrefabs[0].gameObject;
            }

        }

        public GameObject GetTokenPrefab(Token tokenType, bool justForDisplaying = false)
        {
            TokenView tokenView;
            sortedTokenPrefabs.TryGetValue(tokenType, out tokenView);
            if (tokenView)
            {
                tokenView.justDisplaying = justForDisplaying;
                return tokenView.gameObject;
            }
            else
            {
                return null;
            }
        }
    }
}


