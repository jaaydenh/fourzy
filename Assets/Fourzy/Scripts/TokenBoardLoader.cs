using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class TokenBoardLoader : UnitySingleton<TokenBoardLoader>
    {
        private TokenBoardData[] tokenBoards = new TokenBoardData[0];
        private GamePieceData[] gamePieces = new GamePieceData[0];
        private TokenData[] tokens = new TokenData[0];

        protected override void Awake()
        {
            base.Awake();

            LoadData();
        }

        public void LoadData()
        {
            LoadTokenBoardData();
            LoadGamePieceData();
            LoadTokenData();
        }

        private void LoadTokenBoardData()
        {
            TextAsset dataAsJson = Resources.Load<TextAsset>("TokenBoards");
            if (dataAsJson)
            {
                tokenBoards = JsonHelper.getJsonArray<TokenBoardData>(dataAsJson.text);
            }
            else
            {
                tokenBoards[0] = GetDefaultTokenBoard();
            }
        }

        private void LoadGamePieceData()
        {
            TextAsset dataAsJson = Resources.Load<TextAsset>("GamePieces");
            if (dataAsJson)
            {
                gamePieces = JsonHelper.getJsonArray<GamePieceData>(dataAsJson.text);
            }
            else
            {
                gamePieces[0] = GetDefaultGamePiece();
            }
        }

        private void LoadTokenData()
        {
            TextAsset dataAsJson = Resources.Load<TextAsset>("Tokens");
            if (dataAsJson)
            {
                tokens = JsonHelper.getJsonArray<TokenData>(dataAsJson.text);
            }
            else
            {
                AnalyticsManager.LogError("load_token_json_error");
            }
        }

        public TokenBoard GetRandomTokenBoard(GameType gameType, int seed = 0)
        {
            if (seed != 0) {
                UnityEngine.Random.InitState(seed);
            }
            
            if (UnityEngine.Random.Range(0, 100) < Constants.randomGeneratedBoardPercentage) {
                return BoardGeneratorTools.GenerateBoard(seed);
            }

            if (gameType == GameType.REALTIME) {
                var tokenBoardData = tokenBoards
                    .Where(t => t.EnabledRealtime == true)
                    .OrderBy(t => UnityEngine.Random.Range(0, int.MaxValue))
                .FirstOrDefault();

                TokenBoard tokenboard = new TokenBoard(tokenBoardData.TokenData.ToArray(), tokenBoardData.ID, tokenBoardData.Name, tokenBoardData.InitialMoves, tokenBoardData.InitialGameBoard.ToArray(), true);

                return tokenboard;
            } else {
                var tokenBoardData = tokenBoards
                    .Where(t => t.Enabled == true)
                    .OrderBy(t => UnityEngine.Random.Range(0, int.MaxValue))
                .FirstOrDefault();

                TokenBoard tokenboard = new TokenBoard(tokenBoardData.TokenData.ToArray(), tokenBoardData.ID, tokenBoardData.Name, tokenBoardData.InitialMoves, tokenBoardData.InitialGameBoard.ToArray(), true);

                return tokenboard;
            }

            //TokenBoard tokenboard = new TokenBoard(tokenBoardData.TokenData.ToArray(), tokenBoardData.ID, tokenBoardData.Name, tokenBoardData.InitialMoves, tokenBoardData.InitialGameBoard.ToArray(), true);

            //return tokenboard;
        }

        public TokenBoard GetRandomTokenBoardByIndex(int index) 
        {
            // var tokenBoardData = tokenBoards[index];
            var enabledTokenBoards = tokenBoards
                .Where(t => t.Enabled == true);
            var tokenBoardData = enabledTokenBoards.ElementAt<TokenBoardData>(index);

            TokenBoard tokenboard = new TokenBoard(tokenBoardData.TokenData.ToArray(), tokenBoardData.ID, tokenBoardData.Name, tokenBoardData.InitialMoves, tokenBoardData.InitialGameBoard.ToArray(), true);

            return tokenboard;
        }

        public TokenBoard GetTokenBoard(string id) 
        {
            var tokenBoardData = tokenBoards
                .Where(t => t.ID == id)
            .FirstOrDefault();

            TokenBoard tokenboard = new TokenBoard(tokenBoardData.TokenData.ToArray(), tokenBoardData.ID, tokenBoardData.Name, tokenBoardData.InitialMoves, tokenBoardData.InitialGameBoard.ToArray(), true);

            return tokenboard;
        }

        public TokenBoard[] GetTokenBoardsForBoardSelection()
        {
            IEnumerable<TokenBoardData> enabledTokenBoards = tokenBoards.Where(t => t.EnabledGallery == true);

            TokenBoard[] selectedTokenBoards = new TokenBoard[enabledTokenBoards.Count()];
            int i = 0;
            foreach (var tokenBoardData in enabledTokenBoards)
            {
                TokenBoard tokenboard = new TokenBoard(tokenBoardData.TokenData.ToArray(), tokenBoardData.ID, tokenBoardData.Name, tokenBoardData.InitialMoves, tokenBoardData.InitialGameBoard.ToArray(), false);
                selectedTokenBoards[i] = tokenboard;
                i++;
            }
            return selectedTokenBoards;
        }

        public GamePieceData[] GetAllGamePieces()
        {
            IEnumerable<GamePieceData> enabledGamePieces = gamePieces
                .Where(t => t.Enabled == true);

            return enabledGamePieces.ToArray();
        }

        public TokenData[] GetAllTokens()
        {
            IEnumerable<TokenData> enabledTokens = tokens
                .Where(t => t.Enabled == true);

            return enabledTokens.ToArray();
        }

        private TokenBoardData GetDefaultTokenBoard()
        {
            TokenBoardData tokenBoardData = new TokenBoardData();
            tokenBoardData.ID = "1000";
            tokenBoardData.Name = "The Basic Game";
            tokenBoardData.Enabled = true;
            tokenBoardData.TokenData = new List<int> {
                6,0,0,0,0,0,0,6,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                6,0,0,0,0,0,0,6};

            return tokenBoardData;
        }

        private GamePieceData GetDefaultGamePiece()
        {
            GamePieceData gamePieceData = new GamePieceData();
            gamePieceData.ID = 1;
            gamePieceData.Name = "Test Piece 1";
            gamePieceData.Enabled = true;
            gamePieceData.OutlineColor = Color.blue;
            gamePieceData.SecondaryColor = new Vector4(0.3f, 0, 0, 0);

            return gamePieceData;
        }
    }
}
