﻿//@vadym udod

using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Toasts;
using System.Collections.Generic;
using System.Linq;

namespace Fourzy._Updates.ClientModel
{
    [System.Serializable]
    public class BasicPuzzlePack
    {
        public string name;
        public PackType packType = PackType.PUZZLE_PACK;
        public string packID;

        public List<ClientPuzzleData> puzzlesData { get; set; }
        public List<ClientPuzzleData> enabledPuzzlesData { get; set; }
        public List<ClientPuzzleData> rewardPuzzles { get; set; }
        public List<RewardsManager.Reward> allRewards { get; set; }

        public List<ClientPuzzleData> puzzlesComplete => enabledPuzzlesData.Where(puzzle => PlayerPrefsWrapper.GetPuzzleChallengeComplete(puzzle.ID)).ToList();

        public bool complete => puzzlesComplete.Count == enabledPuzzlesData.Count;

        public bool justFinished { get; set; }

        public virtual void Initialize()
        {
            puzzlesData = new List<ClientPuzzleData>();
            enabledPuzzlesData = new List<ClientPuzzleData>();
            rewardPuzzles = new List<ClientPuzzleData>();
            allRewards = new List<RewardsManager.Reward>();
        }

        public ClientPuzzleData nextUnsolvedData
        {
            get
            {
                if (enabledPuzzlesData.Count == 0) return null;

                for (int puzzleIndex = 0; puzzleIndex < enabledPuzzlesData.Count; puzzleIndex++)
                {
                    if (puzzleIndex == 0 && !PlayerPrefsWrapper.GetPuzzleChallengeComplete(enabledPuzzlesData[puzzleIndex].ID))
                        return enabledPuzzlesData[puzzleIndex];
                    else if (puzzleIndex > 0 && PlayerPrefsWrapper.GetPuzzleChallengeComplete(enabledPuzzlesData[puzzleIndex - 1].ID)
                        && !PlayerPrefsWrapper.GetPuzzleChallengeComplete(enabledPuzzlesData[puzzleIndex].ID))
                        return enabledPuzzlesData[puzzleIndex];
                }

                return enabledPuzzlesData[0];
            }
        }

        public IClientFourzy NextUnsolved()
        {
            ClientPuzzleData data = nextUnsolvedData;

            if (data)
            {
                data.Initialize();

                switch (data.pack.packType)
                {
                    case PackType.PUZZLE_PACK: return new ClientFourzyPuzzle(data);

                    default: return ClientFourzyGame.FromPuzzleData(data);
                }
            }

            return null;
        }

        public IClientFourzy Next(IClientFourzy current)
        {
            ClientPuzzleData puzzleData = enabledPuzzlesData.Next(current.puzzleData);

            puzzleData.Initialize();

            switch (puzzleData.pack.packType)
            {
                case PackType.PUZZLE_PACK: return new ClientFourzyPuzzle(puzzleData);

                default: return ClientFourzyGame.FromPuzzleData(puzzleData);
            }
        }

        public void StartNextUnsolvedPuzzle()
        {
            IClientFourzy game = NextUnsolved();

            if (game == null)
            {
                GamesToastsController.ShowTopToast("Empty puzzle pack");
                return;
            }

            GameManager.Instance.currentPuzzlePack = this;
            GameManager.Instance.StartGame(game);
        }

        public void ResetPlayerPrefs()
        {
            PlayerPrefsWrapper.SetPuzzlePackUnlocked(packID, false);
            PlayerPrefsWrapper.SetPuzzlePackOpened(packID, false);

            puzzlesData.ForEach(_data =>
            {
                PlayerPrefsWrapper.SetPuzzleChallengeComplete(_data.ID, false);
                PlayerPrefsWrapper.SetGameRewarded(_data.ID, false);

                //clear rewards
                foreach (RewardsManager.Reward reward in _data.rewards)
                {
                    PlayerPrefsWrapper.SetEventRewarded(_data.GetRewardID(reward), false);
                }
            });
        }

        public static implicit operator bool(BasicPuzzlePack pack) => pack != null;
    }

    public enum UnlockRequirementsEnum
    {
        NONE = 0,
        STARS = 1,
        COINS = 2,
        GEMS = 3,
    }

    public enum PackType
    {
        PUZZLE_PACK,
        AI_PACK,
        BOSS_AI_PACK,
    }
}