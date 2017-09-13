using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Fourzy
{

    public class TokenBoardLoader : MonoBehaviour
    {
        private string gameDataProjectFilePath = "/TokenBoards.json";
        private TokenBoardInfo[] allTokenBoards;

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

        private TokenBoardInfo[] LoadTokenBoardData()
        {
            //string filePath = Application.streamingAssetsPath + gameDataProjectFilePath;
            //Debug.Log("filepath: " + filePath);
            // if (File.Exists (filePath)) {
            //     string dataAsJson = File.ReadAllText (filePath);
            //     tokenBoardData = JsonHelper.getJsonArray<TokenBoardInfo> (dataAsJson);
            // } else {
            //     tokenBoardData[0] = GetDefaultTokenBoard();
            // }

            TokenBoardInfo[] tokenBoardData = new TokenBoardInfo[0];
            TextAsset dataAsJson = Resources.Load<TextAsset>("TokenBoards");
            if (dataAsJson) {
                tokenBoardData = JsonHelper.getJsonArray<TokenBoardInfo> (dataAsJson.text);
            } else {
                tokenBoardData[0] = GetDefaultTokenBoard();
            }

            return tokenBoardData;
        }

        public TokenBoard GetTokenBoard()
        {
            TokenBoardInfo[] tokenBoardCollection = LoadTokenBoardData();

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
            TokenBoardInfo[] tokenBoardCollection = LoadTokenBoardData();

            TokenBoard[] tokenBoards = new TokenBoard[tokenBoardCollection.Length];
            int i = 0;
            foreach (var tokenBoardInfo in tokenBoardCollection)
            {
                TokenBoard tokenboard = new TokenBoard(tokenBoardInfo.TokenData.ToArray(), tokenBoardInfo.ID, tokenBoardInfo.Name, false);
                tokenBoards[i] = tokenboard;
                i++;
            }
            return tokenBoards;
        }

        public TokenBoardInfo GetDefaultTokenBoard()
        {
            TokenBoardInfo tokenBoardInfo = new TokenBoardInfo();
            tokenBoardInfo.ID = "1000";
            tokenBoardInfo.Name = "The Basic Game";
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
    }
}
