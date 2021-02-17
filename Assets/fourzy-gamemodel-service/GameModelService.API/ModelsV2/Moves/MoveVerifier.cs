using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class MoveVerifier
    {
        Dictionary<BoardLocation, int> PieceVerification;

        public MoveVerifier()
        {
            PieceVerification = new Dictionary<BoardLocation, int>();
        }

        public void AddPieceCheck(BoardLocation Location, int PlayerId)
        {
            PieceVerification.Add(Location, PlayerId);
        }

        public void AddPieceCheck(int Row, int Column, int PlayerId)
        {
            PieceVerification.Add(new BoardLocation(Row,Column), PlayerId);
        }

        public bool VerifyState(GameState GameState)
        {
            foreach (BoardLocation loc in PieceVerification.Keys)
            {
                if (GameState.Board.ContentsAt(loc).Control != PieceVerification[loc]) return false;
            }
            return true;
        }

    }
}
