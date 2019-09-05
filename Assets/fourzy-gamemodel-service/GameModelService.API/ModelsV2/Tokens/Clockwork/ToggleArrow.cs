using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class ToggleArrowToken : IToken
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

        public Rotation RotationDirection {get; set;}

        public char TokenCharacter
        {
            get
            {
                return TokenConstants.ToggleArrow;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.ToggleArrow.ToString()
                        + TokenConstants.DirectionString(Orientation)
                        + TokenConstants.NotateRotation(RotationDirection);
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.ToggleArrow.ToString(),
                                            TokenConstants.DirectionString(Orientation),
                                            TokenConstants.NotateRotation(RotationDirection)};
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
                return true;
            }
        }

        public int Complexity
        {
            get
            {
                return TokenConstants.COMPLEXITY_CHALLENGE;
            }
        }

        public ToggleArrowToken(Direction Orientation, Rotation Direction)
        {
            StandardTokenInit();

            this.Orientation = Orientation;
            this.RotationDirection = Direction;
            this.Type = TokenType.TOGGLE_ROTATE;
            this.changePieceDirection = true;
        }

        public ToggleArrowToken(string Notation)
        {
            StandardTokenInit();

            if (Notation.Length > 1)
            {
                this.Orientation = TokenConstants.GetOrientation(Notation[1]);
            }

            if (Notation.Length > 2)
            {
                this.RotationDirection = TokenConstants.GetRotation(Notation[2]);
            }
            this.Type = TokenType.TOGGLE_ROTATE;
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
            return Orientation;
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
            Orientation = BoardLocation.Rotate(Orientation, RotationDirection);
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
