using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Fourzy
{
    public class PuzzleChallengeLoader : MonoBehaviour
    {
        //Singleton
        private static PuzzleChallengeLoader _instance;
        public static PuzzleChallengeLoader instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<PuzzleChallengeLoader>();
                }
                return _instance;
            }
        }

        private PuzzleChallengeInfo[] LoadChallengeData()
        {
            PuzzleChallengeInfo[] challengeData = new PuzzleChallengeInfo[0];
            TextAsset dataAsJson = Resources.Load<TextAsset>("PuzzleChallenges");
            if (dataAsJson) {
                challengeData = JsonHelper.getJsonArray<PuzzleChallengeInfo> (dataAsJson.text);
            } else {
                Debug.Log("default challenge");
                challengeData[0] = GetDefaultChallenge();
            }

            return challengeData;
        }

        public PuzzleChallengeInfo GetChallenge()
        {
            PuzzleChallengeInfo[] challengeCollection = LoadChallengeData();
            Debug.Log("puzzleChallengeLevel: " + PlayerPrefs.GetInt("puzzleChallengeLevel"));
            int puzzleChalLengeLevel = PlayerPrefs.GetInt("puzzleChallengeLevel");
            IEnumerable<PuzzleChallengeInfo> enabledChallenges = challengeCollection
                .Where(t => t.Enabled == true && t.Level == puzzleChalLengeLevel+1);

            // ChallengeInfo[] challenges = new TokenBoard[enabledChallenges.Count()];
            // int i = 0;
            // foreach (var tokenBoardInfo in enabledChallenges)
            // {
            //     TokenBoard tokenboard = new TokenBoard(tokenBoardInfo.TokenData.ToArray(), tokenBoardInfo.ID, tokenBoardInfo.Name, false);
            //     tokenBoards[i] = tokenboard;
            //     i++;
            // }

            if (enabledChallenges.Count() > 0) {
                return enabledChallenges.FirstOrDefault();
            }
            Debug.Log("Return null puzzle challenge");
            return null;
        }

        private PuzzleChallengeInfo GetDefaultChallenge()
        {
            PuzzleChallengeInfo challengeInfo = new PuzzleChallengeInfo();
            challengeInfo.ID = "1000";
            challengeInfo.Name = "The Basic Game";
            challengeInfo.MoveGoal = 2;
            challengeInfo.FirstMove = 0;
            challengeInfo.Enabled = true;
            challengeInfo.InitialGameBoard = new List<int> {
                6,0,0,0,0,0,0,6,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                6,0,0,0,0,0,0,6};
            challengeInfo.InitialTokenBoard = new List<int> {
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0};

            return challengeInfo;
        }
    }
}
