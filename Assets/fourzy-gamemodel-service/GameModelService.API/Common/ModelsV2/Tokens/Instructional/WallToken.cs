using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class WallToken : IToken
    {
        public int Layer { get; set; }
        public Direction Orientation { get; set; }
        public BoardSpace Space { get; set; }
        public bool Visible { get; set; }
        public bool Delete { get; set; }

        public bool pieceCanEnter { get; set; }
        public bool pieceMustStopOn { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public int addFriction { get; set; }
        public int adjustMomentum { get; set; }
        public int setMomentum { get; set; }
        public bool changePieceDirection { get; set; }
        public TokenType Type { get; set; }
        public bool WallTop { get; set; }
        public bool WallBottom { get; set; }
        public bool WallLeft { get; set; }
        public bool WallRight { get; set; }

        public char TokenCharacter
        {
            get
            {
                return TokenConstants.Wall;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Wall.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Wall.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.BLOCKER;
            }
        }

        public bool HasDynamicFeature
        {
            get
            {
                return false;
            }
        }

        public int Complexity
        {
            get
            {
                return TokenConstants.COMPLEXITY_BASIC;
            }
        }

        public bool DisruptsWin
        {
            get
            {
                return false;
            }
        }

        public WallToken(bool Top, bool Bottom, bool Left, bool Right)
        {
            StandardTokenInit();
            this.WallLeft = Left;
            this.WallRight = Right;
            this.WallTop = Top;
            this.WallBottom = Bottom;

            this.Type = TokenType.WALL;
        }

        public WallToken(string Notation)
        {

        }

        public void StandardTokenInit()
        {
            this.Layer = -1;
            this.Orientation = Direction.NONE;
            this.Visible = true;
            this.Delete = false;
            this.pieceCanEnter = true;
            this.pieceMustStopOn = false;
            this.pieceCanEndMoveOn = true;
            this.isMoveable = false;
            this.addFriction = 0;
            this.adjustMomentum = -1;
            this.setMomentum = -1;
            this.changePieceDirection = false;
            this.Space = null;
        }

        public bool HasWall(Direction MoveIntoDirection)
        {
            switch (MoveIntoDirection)
            {
                case Direction.UP:
                    return WallBottom;
                case Direction.DOWN:
                    return WallTop;
                case Direction.LEFT:
                    return WallRight;
                case Direction.RIGHT:
                    return WallLeft;
            }
            return false;
        }

        public Direction GetDirection(MovingPiece Piece)
        {
            return Direction.NONE;
        }

        public void ApplyElement(ElementType Element)
        {
        }


        public void EndOfTurn(int PlayerId)
        {
        }

        public void PieceEntersBoard(MovingPiece Piece)
        {

        }

        public void PieceEntersSpace(MovingPiece Piece)
        {
        }

        public void PieceLeavesSpace(MovingPiece Piece)
        {
        }

        public void PieceBumpsIntoLocation(MovingPiece Piece, BoardLocation Location)
        {
        }

        public void PieceStopsOnSpace(MovingPiece Piece)
        {
        }

        public string Print()
        {
            return "";
        }

        public void StartOfTurn(int PlayerId)
        {
        }
    }
}
