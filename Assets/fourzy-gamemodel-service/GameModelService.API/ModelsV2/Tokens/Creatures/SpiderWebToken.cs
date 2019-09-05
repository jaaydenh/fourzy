using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class SpiderWebToken : IToken
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

        public TokenClassification Classification { get
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
                return TokenConstants.COMPLEXITY_BASIC;
            }
        }

        public char TokenCharacter
        {
            get
            {
                return TokenConstants.SpiderWeb;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.SpiderWeb.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.SpiderWeb.ToString() };
            }
        }

        public SpiderWebToken()
        {
            StandardTokenInit();

            this.Type = TokenType.WEB;
            this.pieceMustStopOn = true;
            this.isMoveable = true;
            this.addFriction = 100;
            this.adjustMomentum = -1;
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
            this.addFriction = 100;
            this.adjustMomentum = -1;
            this.setMomentum = -1;
            this.changePieceDirection = false;
            this.Space = null;
        }

        public void Burn()
        {
            if (Space.ContainsPiece) Space.ActivePiece.Conditions.Add(PieceConditionType.FIERY);
            this.pieceMustStopOn = false;
            this.addFriction = 0;
            Delete = true;
        }

        public Direction GetDirection(MovingPiece Piece)
        {
            return Piece.Direction;
        }

        public void ApplyElement(ElementType Element)
        {
            if (Element == ElementType.FIRE)
            {
                Burn();
            }
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
