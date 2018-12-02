namespace Fourzy
{
    using System.Collections.Generic;

    [System.Serializable]
    public class PuzzlePack
    {
        public string ID;
        public string Name;
        public int Level;
        public int ActiveLevel;
        public int CompletedToUnlock;
        public bool Enabled  = false;
        public List<PuzzleChallengeLevel> PuzzleChallengeLevels = new List<PuzzleChallengeLevel>();
        
        private int _completedCount = -1;

        public bool locked { get { return completedCount < CompletedToUnlock; } }
        public int completedCount
        {
            get
            {
                if (_completedCount == -1)
                {
                    _completedCount = 0;

                    foreach (PuzzleChallengeLevel level in PuzzleChallengeLevels)
                        if (PlayerPrefsWrapper.IsPuzzleChallengeCompleted(level.ID))
                            _completedCount++;
                }

                return _completedCount;
            }
        }
    }
}