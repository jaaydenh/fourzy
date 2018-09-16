using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Fourzy
{
    public class TokenBoardLoader : Singleton<TokenBoardLoader>
    {
        private int randomGeneratedBoardPercentage = 70;

        private TokenBoardData[] LoadTokenBoardData()
        {
            //string filePath = Application.streamingAssetsPath + gameDataProjectFilePath;
            //Debug.Log("filepath: " + filePath);
            // if (File.Exists (filePath)) {
            //     string dataAsJson = File.ReadAllText (filePath);
            //     tokenBoardData = JsonHelper.getJsonArray<TokenBoardInfo> (dataAsJson);
            // } else {
            //     tokenBoardData[0] = GetDefaultTokenBoard();
            // }

            TokenBoardData[] tokenBoardData = new TokenBoardData[0];
            TextAsset dataAsJson = Resources.Load<TextAsset>("TokenBoards");
            if (dataAsJson) {
                tokenBoardData = JsonHelper.getJsonArray<TokenBoardData> (dataAsJson.text);
            } else {
                tokenBoardData[0] = GetDefaultTokenBoard();
            }

            return tokenBoardData;
        }

        private GamePieceData[] LoadGamePieceData()
        {
            GamePieceData data = new GamePieceData();
            data.OutlineColorWrapper = Color.red;
            data.SecondaryColorWrapper = new Vector3(1.0f, 0.0f, 0.3f);
            string json = JsonUtility.ToJson(data);


            GamePieceData[] gamePieceData = new GamePieceData[0];
            TextAsset dataAsJson = Resources.Load<TextAsset>("GamePieces");
            if (dataAsJson)
            {
                gamePieceData = JsonHelper.getJsonArray<GamePieceData>(dataAsJson.text);
            }
            else
            {
                gamePieceData[0] = GetDefaultGamePiece();
            }

            return gamePieceData;
        }

        private TokenData[] LoadTokenData()
        {
            TokenData[] tokenData = new TokenData[0];
            TextAsset dataAsJson = Resources.Load<TextAsset>("Tokens");
            if (dataAsJson)
            {
                tokenData = JsonHelper.getJsonArray<TokenData>(dataAsJson.text);
            }
            else
            {
                AnalyticsManager.LogError("load_token_json_error");
            }

            return tokenData;
        }

        public TokenBoard GetRandomTokenBoard()
        {
            if (UnityEngine.Random.Range(0, 100) < randomGeneratedBoardPercentage) {
                return RandomBoardGenerator.GenerateBoard();
            } 

            TokenBoardData[] tokenBoardCollection = LoadTokenBoardData();

            var tokenBoardData = tokenBoardCollection
                .Where(t => t.Enabled == true)
                .OrderBy(t => UnityEngine.Random.Range(0, int.MaxValue))
            .FirstOrDefault();

            TokenBoard tokenboard = new TokenBoard(tokenBoardData.TokenData.ToArray(), tokenBoardData.ID, tokenBoardData.Name, tokenBoardData.InitialMoves, tokenBoardData.InitialGameBoard.ToArray(), true);

            return tokenboard;
        }

        public TokenBoard GetRandomTokenBoardByIndex(int index) {
            TokenBoardData[] tokenBoardCollection = LoadTokenBoardData();

            var tokenBoardData = tokenBoardCollection[index];

            TokenBoard tokenboard = new TokenBoard(tokenBoardData.TokenData.ToArray(), tokenBoardData.ID, tokenBoardData.Name, tokenBoardData.InitialMoves, tokenBoardData.InitialGameBoard.ToArray(), true);

            return tokenboard;
        }

        public TokenBoard GetTokenBoard(string id) {
            TokenBoardData[] tokenBoardCollection = LoadTokenBoardData();

            var tokenBoardData = tokenBoardCollection
                .Where(t => t.ID == id)
            .FirstOrDefault();

            TokenBoard tokenboard = new TokenBoard(tokenBoardData.TokenData.ToArray(), tokenBoardData.ID, tokenBoardData.Name, tokenBoardData.InitialMoves, tokenBoardData.InitialGameBoard.ToArray(), true);

            return tokenboard;
        }

        public TokenBoard[] GetTokenBoardsForBoardSelection()
        {
            TokenBoardData[] tokenBoardCollection = LoadTokenBoardData();
            IEnumerable<TokenBoardData> enabledTokenBoards = tokenBoardCollection
                .Where(t => t.EnabledGallery == true);

            TokenBoard[] tokenBoards = new TokenBoard[enabledTokenBoards.Count()];
            int i = 0;
            foreach (var tokenBoardData in enabledTokenBoards)
            {
                TokenBoard tokenboard = new TokenBoard(tokenBoardData.TokenData.ToArray(), tokenBoardData.ID, tokenBoardData.Name, tokenBoardData.InitialMoves, tokenBoardData.InitialGameBoard.ToArray(), false);
                tokenBoards[i] = tokenboard;
                i++;
            }
            return tokenBoards;
        }

        public GamePieceData[] GetAllGamePieces()
        {
            GamePieceData[] gamepieceCollection = LoadGamePieceData();
            IEnumerable<GamePieceData> enabledGamePieces = gamepieceCollection
                .Where(t => t.Enabled == true);

            return enabledGamePieces.ToArray();
        }

        public TokenData[] GetAllTokens()
        {
            TokenData[] tokenCollection = LoadTokenData();
            IEnumerable<TokenData> enabledTokens = tokenCollection
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
            gamePieceData.ID = "01";
            gamePieceData.Name = "Test Piece 1";
            gamePieceData.Enabled = true;

            return gamePieceData;
        }
    }
}
