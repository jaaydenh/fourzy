using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Fourzy
{
    public class TokenBoardLoader : MonoBehaviour
    {
        //Singleton
        private static TokenBoardLoader _instance;
        public static TokenBoardLoader instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<TokenBoardLoader>();
                }
                return _instance;
            }
        }

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

        public TokenBoard GetTokenBoard()
        {
            TokenBoardData[] tokenBoardCollection = LoadTokenBoardData();

            var tokenBoardInfo = tokenBoardCollection
                .Where(t => t.Enabled == true)
                .OrderBy(t => UnityEngine.Random.Range(0, int.MaxValue))
            .FirstOrDefault();

            //Debug.Log("tokenboard ID: " + tokenBoardInfo.ID);
            //Debug.Log("tokenboard Name: " + tokenBoardInfo.Name);
            //Debug.Log("tokenboard Enabled: " + tokenBoardInfo.Enabled);
            //Debug.Log("tokenboard TokenData: " + tokenBoardInfo.TokenData);

            TokenBoard tokenboard = new TokenBoard(tokenBoardInfo.TokenData.ToArray(), tokenBoardInfo.ID, tokenBoardInfo.Name, true);
            return tokenboard;
        }

        public TokenBoard[] GetAllTokenBoards()
        {
            TokenBoardData[] tokenBoardCollection = LoadTokenBoardData();
            IEnumerable<TokenBoardData> enabledTokenBoards = tokenBoardCollection
                .Where(t => t.Enabled == true);

            TokenBoard[] tokenBoards = new TokenBoard[enabledTokenBoards.Count()];
            int i = 0;
            foreach (var tokenBoardInfo in enabledTokenBoards)
            {
                TokenBoard tokenboard = new TokenBoard(tokenBoardInfo.TokenData.ToArray(), tokenBoardInfo.ID, tokenBoardInfo.Name, false);
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

        private TokenBoardData GetDefaultTokenBoard()
        {
            TokenBoardData tokenBoardInfo = new TokenBoardData();
            tokenBoardInfo.ID = "1000";
            tokenBoardInfo.Name = "The Basic Game";
            tokenBoardInfo.Enabled = true;
            tokenBoardInfo.TokenData = new List<int> {
                6,0,0,0,0,0,0,6,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                6,0,0,0,0,0,0,6};

            return tokenBoardInfo;
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
