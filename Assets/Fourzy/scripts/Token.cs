using UnityEngine;

namespace Fourzy {
    public interface IToken 
    {
        int Row { get; set; }
        int Column { get; set; }
        bool canEnter { get; set; }
        bool canEndMove { get; set; }
        bool isMoveable { get; set; }
        bool mustStop { get; set; }
        float chanceDestroyOnEnd { get; set; }
        bool changePieceDirection { get; set; }
        bool useCurrentDirection { get; set; }
        Direction newPieceDirection { get; set;}
        Token tokenType { get; set; }
        void UpdateBoard(GameBoard board, bool swapPiece);
    }

    public class UpArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public UpArrowToken() {
            canEnter = true;
            canEndMove = true;
            isMoveable = false;
            mustStop = false;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = true;
            newPieceDirection = Direction.UP;
            tokenType = Token.UP_ARROW;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class DownArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public DownArrowToken() {
            canEnter = true;
            canEndMove = true;
            isMoveable = false;
            mustStop = false;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = true;
            newPieceDirection = Direction.DOWN;
            tokenType = Token.DOWN_ARROW;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class LeftArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public LeftArrowToken() {
            canEnter = true;
            canEndMove = true;
            isMoveable = false;
            mustStop = false;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = true;
            newPieceDirection = Direction.LEFT;
            tokenType = Token.LEFT_ARROW;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class RightArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public RightArrowToken() {
            canEnter = true;
            canEndMove = true;
            isMoveable = false;
            mustStop = false;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = true;
            newPieceDirection = Direction.RIGHT;
            tokenType = Token.RIGHT_ARROW;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class EmptyToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public EmptyToken () {
            canEnter = true;
            canEndMove = true;
            isMoveable = false;
            mustStop = false;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.EMPTY;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class BlockerToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public BlockerToken () {
            canEnter = false;
            canEndMove = false;
            isMoveable = false;
            mustStop = false;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.BLOCKER;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            Debug.Log("BLOCKER TOKEN");
            //Do nothing as the blocker token prevents the piece from moving here
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class GhostToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public GhostToken () {
            canEnter = true;
            canEndMove = false;
            isMoveable = false;
            mustStop = false;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.GHOST;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class StickyToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public StickyToken() {
            canEnter = true;
            canEndMove = true;
            isMoveable = true;
            mustStop = true;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.STICKY;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class IceSheetToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public IceSheetToken() {
            canEnter = true;
            canEndMove = true;
            isMoveable = true;
            mustStop = false;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.ICE_SHEET;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class PitToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public PitToken () {
            canEnter = true;
            canEndMove = true;
            isMoveable = false;
            mustStop = true;
            chanceDestroyOnEnd = 100.0f;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.PIT;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class NinetyRightArrowToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public NinetyRightArrowToken()
        {
            canEnter = true;
            canEndMove = true;
            isMoveable = false;
            mustStop = false;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = true;
            useCurrentDirection = true;
            newPieceDirection = Direction.RIGHT;
            tokenType = Token.NINETY_RIGHT_ARROW;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class NinetyLeftArrowToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public NinetyLeftArrowToken()
        {
            canEnter = true;
            canEndMove = true;
            isMoveable = false;
            mustStop = false;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = true;
            useCurrentDirection = true;
            newPieceDirection = Direction.LEFT;
            tokenType = Token.NINETY_LEFT_ARROW;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }
}