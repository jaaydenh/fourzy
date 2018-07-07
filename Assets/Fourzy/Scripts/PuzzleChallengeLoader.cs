using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Fourzy
{
    public class PuzzleChallengeLoader : Singleton<PuzzleChallengeLoader>
    {
        private PuzzleChallengeLevel[] LoadChallengeData()
        {
            PuzzleChallengeLevel[] challengeData = new PuzzleChallengeLevel[0];
            TextAsset dataAsJson = Resources.Load<TextAsset>("PuzzleChallenges");
            if (dataAsJson) {
                challengeData = JsonHelper.getJsonArray<PuzzleChallengeLevel> (dataAsJson.text);
            } else {
                Debug.Log("default challenge");
                challengeData[0] = GetDefaultChallenge();
            }

            return challengeData;
        }

        public PuzzlePack[] GetPuzzlePacks() {
            TextAsset[] dataAsJson = Resources.LoadAll<TextAsset>("PuzzlePacks");
            PuzzlePack[] puzzlePacks = new PuzzlePack[dataAsJson.Length];
            if (dataAsJson.Length > 0) {
                for (int i = 0; i < dataAsJson.Length; i++)
                {
                    puzzlePacks[i] = JsonUtility.FromJson<PuzzlePack>(dataAsJson[i].text);
                }
            }
            Debug.Log("puzzlePacks.length" +  puzzlePacks.Length);
            return puzzlePacks;
        }

        public PuzzlePack GetPuzzlePack(string fileName) {
            TextAsset dataAsJson = Resources.Load<TextAsset>("PuzzlePacks\\" + fileName);
            PuzzlePack puzzlePack = new PuzzlePack();
            if (dataAsJson) {
                puzzlePack = JsonUtility.FromJson<PuzzlePack>(dataAsJson.text);
            }

            return puzzlePack;
        }

        public PuzzleChallengeLevel GetChallenge()
        {
            PuzzleChallengeLevel[] challengeCollection = LoadChallengeData();
            Debug.Log("puzzleChallengeLevel: " + PlayerPrefs.GetInt("puzzleChallengeLevel"));
            int puzzleChallengeLevel = PlayerPrefs.GetInt("puzzleChallengeLevel");
            IEnumerable<PuzzleChallengeLevel> enabledChallenges = challengeCollection
                .Where(t => t.Enabled == true && t.Level == puzzleChallengeLevel+1);

            
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
                //return enabledChallenges.ElementAtOrDefault(puzzleChallengeLevel);
            }
            Debug.Log("Return null puzzle challenge");
            return null;
        }

        private PuzzleChallengeLevel GetDefaultChallenge()
        {
            PuzzleChallengeLevel challengeInfo = new PuzzleChallengeLevel();
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
