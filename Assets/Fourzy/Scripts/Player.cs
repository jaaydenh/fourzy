namespace Fourzy
{
    public class PlayerData
    {
        public string opponentName = "";
        public string opponentFBId = "";

        public PlayerData(string opponentName, string opponentFBId) {
            this.opponentName = opponentName;
            this.opponentFBId = opponentFBId;
        }
    }
}
