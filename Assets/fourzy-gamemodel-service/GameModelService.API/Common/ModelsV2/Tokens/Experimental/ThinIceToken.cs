using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class ThinIceToken : IToken
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
                return TokenConstants.ThinIce;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.ThinIce.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.ThinIce.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.TERRAIN;
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
                return TokenConstants.COMPLEXITY_MEDIUM;
            }
        }

        public bool DisruptsWin
        {
            get
            {
                return false;
            }
        }

        public bool Triggered { get; set; }

        public ThinIceToken()
        {
            StandardTokenInit();
            this.Type = TokenType.THIN_ICE;
            this.Triggered = false;
            this.isMoveable = true;
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


        public void FallAway()
        {
            Triggered = true;
            Visible = false;
            Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.FALLAWAY, this, new PitToken()));

            Space.AddToken(new PitToken());
            Space.RemoveTokens(TokenType.FALLAWAY_FLOOR);

        }

        public Direction GetDirection(MovingPiece Piece)
        {
            return Orientation;
        }

        public void ApplyElement(ElementType Element)
        {
            switch (Element)
            {
                case ElementType.FIRE:
                    Delete = true;
                    break;
                case ElementType.WATER:
                    Delete = true;
                    break;
            }
        }

        public void EndOfTurn(int PlayerId)
        {
        }

        public void PieceEntersBoard(MovingPiece Piece)
        {

        }

        public void PieceEntersSpace(MovingPiece Piece)
        {
            if (!Triggered && Piece.Location.Equals(Space.Location)) FallAway();
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
