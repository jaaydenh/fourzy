namespace Fourzy
{
    public class Opponent
    {
        public string opponentId = "";
        public string opponentName = "";
        public string opponentFBId = "";
        public string opponentLeaderboardRank = "";
        public int gamePieceId = 0;

        public Opponent(string opponentId, string opponentName, string opponentFBId) {
            this.opponentId = opponentId;
            this.opponentName = opponentName;
            this.opponentFBId = opponentFBId;
        }
    }
}
