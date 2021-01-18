using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class RightTurnToken : IToken
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

        public char TokenCharacter
        {
            get
            {
                switch (this.Orientation)
                {
                    case Direction.UP:
                        return '\u2191';
                    case Direction.DOWN:
                        return '\u2193';
                    case Direction.LEFT:
                        return '\u2190';
                    case Direction.RIGHT:
                        return '\u2192';
                }
                return ' ';
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.RightTurn.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.RightTurn.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.INSTRUCTION;
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
                return TokenConstants.COMPLEXITY_NORMAL;
            }
        }

        public bool DisruptsWin
        {
            get
            {
                return false;
            }
        }

        public RightTurnToken()
        {
            StandardTokenInit();

            this.Orientation = Orientation;
            this.Type = TokenType.RIGHT_TURN;
            this.changePieceDirection = true;
        }

        public RightTurnToken(string Notation)
        {
            StandardTokenInit();

            this.Type = TokenType.RIGHT_TURN;
            this.changePieceDirection = true;
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

        public Direction GetDirection(MovingPiece Piece)
        {
            switch (Piece.Direction) {
                case Direction.UP:
                    return Direction.RIGHT;
                case Direction.DOWN:
                    return Direction.LEFT;
                case Direction.LEFT:
                    return Direction.DOWN;
                case Direction.RIGHT:
                    return Direction.UP;
            }

            return Piece.Direction;
        }

        public void ApplyElement(ElementType Element)
        {
        }

        public void EndOfTurn(int TurnCount)
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

        public void StartOfTurn(int TurnCount)
        {
        }
    }
}
