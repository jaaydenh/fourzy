using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class GuardToken : IToken
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
                return TokenConstants.Guard;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Guard.ToString() + TokenConstants.DirectionString(Orientation);
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Guard.ToString(), TokenConstants.DirectionString(Orientation) };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.CREATURE;
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
                return TokenConstants.COMPLEXITY_HARD;
            }
        }


        public GuardToken(Direction Orientation = Direction.DOWN)
        {
            StandardTokenInit();

            this.Type = TokenType.GUARD;
            this.pieceCanEnter = false;
            this.pieceMustStopOn = false;
            this.pieceCanEndMoveOn = false;
            this.Orientation = Orientation;
        }

        public GuardToken(string Notation)
        {
            StandardTokenInit();

            this.Type = TokenType.GUARD;
            this.pieceCanEnter = false;
            this.pieceMustStopOn = false;
            this.pieceCanEndMoveOn = false;

            if (Notation.Length > 1)
            {
                this.Orientation = TokenConstants.IdentifyDirection(Notation[1].ToString());
            }
            else
            {
                this.Orientation = Direction.DOWN;
            }
        }

        public void StandardTokenInit()
        {
            this.Layer = -1;
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
            this.Orientation = Direction.NONE;
        }

        public void March()
        {
            Space.Parent.ContentsAt(Space.Location.Neighbor(Orientation)).AddToken(new GuardToken(Orientation));
            Space.Parent.RecordGameAction(new GameActionTokenMovement(this, TransitionType.GUARD_MARCH, Space.Location, Space.Location.Neighbor(Orientation)));
            Space.RemoveTokens(TokenType.GUARD);
        }
        public void AboutFace()
        {
            GuardToken G_0 = new GuardToken(Orientation);
            Direction OrigOrientation = Orientation;
            Orientation = BoardLocation.Reverse(Orientation);
            Space.Parent.RecordGameAction(new GameActionTokenRotation(this, TransitionType.GUARD_ABOUTFACE,Rotation.CLOCKWISE, OrigOrientation, Orientation));
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
            if (Space.Parent.ContentsAt(Space.Location.Neighbor(Orientation)).TokensAllowEnter) March();
            else AboutFace();
        }
    }
}
