using UnityEngine;

namespace Fourzy {
    public interface IToken 
    {
        int Row { get; set; }
        int Column { get; set; }
        bool canEnter { get; set; }
        bool canEvaluateWithoutEntering { get; set; }
        bool canEndMove { get; set; }
        bool isMoveable { get; set; }
        bool mustStop { get; set; }
        float chanceDestroyOnEnd { get; set; }
        bool changePieceDirection { get; set; }
        bool useCurrentDirection { get; set; }
        bool isReplacable { get; set; }
        IToken replacedToken { get; set; }
        Direction newPieceDirection { get; set;}
        Token tokenType { get; set; }
        void UpdateBoard(GameBoard board, bool swapPiece);
    }

    public class UpArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public UpArrowToken(int row, int column) {
            this.Row = row;
            this.Column = column;
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
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public DownArrowToken(int row, int column) {
            this.Row = row;
            this.Column = column;
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
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public LeftArrowToken(int row, int column) {
            this.Row = row;
            this.Column = column;
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
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public RightArrowToken(int row, int column) {
            this.Row = row;
            this.Column = column;
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
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public EmptyToken (int row, int column) {
            this.Row = row;
            this.Column = column;
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
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public BlockerToken (int row, int column) {
            this.Row = row;
            this.Column = column;
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
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public GhostToken (int row, int column) {
            this.Row = row;
            this.Column = column;
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
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public StickyToken(int row, int column) {
            this.Row = row;
            this.Column = column;
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
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public IceSheetToken(int row, int column) {
            this.Row = row;
            this.Column = column;
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
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public PitToken (int row, int column) {
            this.Row = row;
            this.Column = column;
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
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public NinetyRightArrowToken(int row, int column) {
            this.Row = row;
            this.Column = column;
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
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public NinetyLeftArrowToken(int row, int column) {
            this.Row = row;
            this.Column = column;
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

    public class BumperToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public BumperToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            canEnter = true;
            canEvaluateWithoutEntering = true;
            canEndMove = false;
            isMoveable = false;
            mustStop = false;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = true;
            useCurrentDirection = true;
            newPieceDirection = Direction.REVERSE;
            tokenType = Token.BUMPER;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class CoinToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public CoinToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            canEnter = true;
            canEndMove = true;
            isMoveable = false;
            mustStop = false;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.COIN;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class FruitToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public FruitToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            canEnter = true;
            canEndMove = true;
            isMoveable = false;
            mustStop = false;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = false;
            isReplacable = true;
            replacedToken = new StickyToken(row, column);
            newPieceDirection = Direction.NONE;
            tokenType = Token.FRUIT;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class FruitTreeToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public FruitTreeToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            canEnter = true;
            canEndMove = true;
            isMoveable = false;
            mustStop = false;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.FRUIT_TREE;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class WebToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public WebToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            canEnter = true;
            canEndMove = true;
            isMoveable = false;
            mustStop = false;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = false;
            isReplacable = true;
            newPieceDirection = Direction.NONE;
            tokenType = Token.WEB;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class SpiderToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool canEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool canEndMove { get; set; }
        public bool isMoveable { get; set; }
        public bool mustStop { get; set; }
        public float chanceDestroyOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool isReplacable { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public SpiderToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            canEnter = true;
            canEndMove = true;
            isMoveable = false;
            mustStop = false;
            chanceDestroyOnEnd = 0.0f;
            changePieceDirection = false;
            isReplacable = true;
            newPieceDirection = Direction.NONE;
            tokenType = Token.SPIDER;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }
}