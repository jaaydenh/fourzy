using UnityEngine;
using System;

namespace Fourzy {

    public interface IToken 
    {
        int Row { get; set; }
        int Column { get; set; }
        bool pieceCanEnter { get; set; }
        bool canEvaluateWithoutEntering { get; set; }
        bool pieceCanEndMoveOn { get; set; }
        bool isMoveable { get; set; }
        bool pieceMustStopOn { get; set; }
        float chanceDestroyPieceOnEnd { get; set; }
        bool changePieceDirection { get; set; }
        bool useCurrentDirection { get; set; }
        bool hasEffect { get; set; }
        bool destroyTokenOnEnd { get; set; }
        float addFriction { get; set; }
        int setMomentum { get; set; }
        IToken replacedToken { get; set; }
        Direction newPieceDirection { get; set;}
        Token tokenType { get; set; }
        void UpdateBoard(GameBoard board, bool swapPiece);
    }
    [Serializable]
    public class UpArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public UpArrowToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            isMoveable = false;
            pieceMustStopOn = false;
            chanceDestroyPieceOnEnd = 0.0f;
            addFriction = 0f;
            changePieceDirection = true;
            newPieceDirection = Direction.UP;
            tokenType = Token.UP_ARROW;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    [Serializable]
    public class DownArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public DownArrowToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            isMoveable = false;
            pieceMustStopOn = false;
            chanceDestroyPieceOnEnd = 0.0f;
            addFriction = 0f;
            changePieceDirection = true;
            newPieceDirection = Direction.DOWN;
            tokenType = Token.DOWN_ARROW;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    [Serializable]
    public class LeftArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public LeftArrowToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            isMoveable = false;
            pieceMustStopOn = false;
            chanceDestroyPieceOnEnd = 0.0f;
            addFriction = 0f;
            changePieceDirection = true;
            newPieceDirection = Direction.LEFT;
            tokenType = Token.LEFT_ARROW;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    [Serializable]
    public class RightArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public RightArrowToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            isMoveable = false;
            pieceMustStopOn = false;
            chanceDestroyPieceOnEnd = 0.0f;
            addFriction = 0f;
            changePieceDirection = true;
            newPieceDirection = Direction.RIGHT;
            tokenType = Token.RIGHT_ARROW;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    [Serializable]
    public class EmptyToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public EmptyToken (int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            isMoveable = false;
            pieceMustStopOn = false;
            chanceDestroyPieceOnEnd = 0.0f;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.EMPTY;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    [Serializable]
    public class BlockerToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public BlockerToken (int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = false;
            pieceCanEndMoveOn = false;
            isMoveable = false;
            pieceMustStopOn = false;
            chanceDestroyPieceOnEnd = 0.0f;
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

    [Serializable]
    public class GhostToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public GhostToken (int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = false;
            isMoveable = false;
            pieceMustStopOn = false;
            chanceDestroyPieceOnEnd = 0.0f;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.GHOST;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    [Serializable]
    public class StickyToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public StickyToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            isMoveable = true;
            pieceMustStopOn = true;
            chanceDestroyPieceOnEnd = 0.0f;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.STICKY;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    [Serializable]
    public class IceSheetToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public IceSheetToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            isMoveable = true;
            pieceMustStopOn = false;
            chanceDestroyPieceOnEnd = 0.0f;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.ICE_SHEET;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    [Serializable]
    public class PitToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public PitToken (int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            pieceMustStopOn = true;
            chanceDestroyPieceOnEnd = 100.0f;
            destroyTokenOnEnd = true;
            hasEffect = true;
            replacedToken = new EmptyToken(row, column);
            newPieceDirection = Direction.NONE;
            tokenType = Token.PIT;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    [Serializable]
    public class NinetyRightArrowToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public NinetyRightArrowToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            isMoveable = false;
            pieceMustStopOn = false;
            chanceDestroyPieceOnEnd = 0.0f;
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

    [Serializable]
    public class NinetyLeftArrowToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public NinetyLeftArrowToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            isMoveable = false;
            pieceMustStopOn = false;
            chanceDestroyPieceOnEnd = 0.0f;
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

    [Serializable]
    public class BumperToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public BumperToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            canEvaluateWithoutEntering = true;
            pieceCanEndMoveOn = false;
            isMoveable = false;
            pieceMustStopOn = false;
            chanceDestroyPieceOnEnd = 0.0f;
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

    [Serializable]
    public class CoinToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public CoinToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            isMoveable = false;
            pieceMustStopOn = false;
            chanceDestroyPieceOnEnd = 0.0f;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.COIN;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    [Serializable]
    public class FruitToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public FruitToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            isMoveable = false;
            pieceMustStopOn = false;
            chanceDestroyPieceOnEnd = 0.0f;
            changePieceDirection = false;
            hasEffect = true;
            replacedToken = new StickyToken(row, column);
            newPieceDirection = Direction.NONE;
            tokenType = Token.FRUIT;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    [Serializable]
    public class FruitTreeToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public FruitTreeToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            isMoveable = false;
            pieceMustStopOn = false;
            chanceDestroyPieceOnEnd = 0.0f;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.FRUIT_TREE;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    [Serializable]
    public class WebToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public WebToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            isMoveable = false;
            pieceMustStopOn = false;
            chanceDestroyPieceOnEnd = 0.0f;
            changePieceDirection = false;
            hasEffect = true;
            newPieceDirection = Direction.NONE;
            tokenType = Token.WEB;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    [Serializable]
    public class SpiderToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public SpiderToken(int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            isMoveable = false;
            pieceMustStopOn = false;
            chanceDestroyPieceOnEnd = 0.0f;
            changePieceDirection = false;
            hasEffect = true;
            newPieceDirection = Direction.NONE;
            tokenType = Token.SPIDER;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    [Serializable]
    public class SandToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public SandToken(int row, int column)
        {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            isMoveable = true;
            pieceMustStopOn = false;
            addFriction = 0.5f;
            chanceDestroyPieceOnEnd = 0.0f;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.SAND;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    [Serializable]
    public class WaterToken : IToken
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set; }
        public Token tokenType { get; set; }

        public WaterToken(int row, int column)
        {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            isMoveable = true;
            pieceMustStopOn = true;
            chanceDestroyPieceOnEnd = 0.0f;
            changePieceDirection = false;
            setMomentum = 1;
            newPieceDirection = Direction.NONE;
            tokenType = Token.WATER;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    [Serializable]
    public class CircleBombToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool pieceCanEnter { get; set; }
        public bool canEvaluateWithoutEntering { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public bool pieceMustStopOn { get; set; }
        public float chanceDestroyPieceOnEnd { get; set; }
        public bool changePieceDirection { get; set; }
        public bool useCurrentDirection { get; set; }
        public bool hasEffect { get; set; }
        public bool destroyTokenOnEnd { get; set; }
        public float addFriction { get; set; }
        public int setMomentum { get; set; }
        public IToken replacedToken { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public CircleBombToken (int row, int column) {
            this.Row = row;
            this.Column = column;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            pieceMustStopOn = true;
            chanceDestroyPieceOnEnd = 100.0f;
            destroyTokenOnEnd = true;
            hasEffect = true;
            replacedToken = new EmptyToken(row, column);
            newPieceDirection = Direction.NONE;
            tokenType = Token.CIRCLE_BOMB;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }
}