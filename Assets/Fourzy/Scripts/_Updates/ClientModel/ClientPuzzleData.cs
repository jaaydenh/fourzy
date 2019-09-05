//@vadym udod

using FourzyGameModel.Model;
using Fourzy._Updates.Serialized;

namespace Fourzy._Updates.ClientModel
{
    [System.Serializable]
    public class ClientPuzzleData : PuzzleData
    {
        public PuzzlePacksDataHolder.PackType packType;
        public GameBoardDefinition gameBoardDefinition;
        public AIProfile aiProfile;
        public BossType aiBoss;
    }
}