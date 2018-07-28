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
    }
}