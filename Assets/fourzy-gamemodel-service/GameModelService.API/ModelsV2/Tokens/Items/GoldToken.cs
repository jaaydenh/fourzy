using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class GoldToken : IToken
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

        int Amount = 1;

        public char TokenCharacter
        {
            get
            {
                return TokenConstants.Gold;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Gold.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Gold.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.ITEM;
            }
        }

        public GoldToken(int Amount = 1)
        {
            StandardTokenInit();

            this.Amount = Amount;
            this.Type = TokenType.GOLD;
        }

        public GoldToken(string Notation)
        {
            StandardTokenInit();

            if (Notation.Length > 1)
            {
                this.Amount = int.Parse(Notation[1].ToString());
            }
            else
            {
                this.Amount = 1;
            }

            this.Type = TokenType.GOLD;
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
            if (Space.ContainsPiece && Amount > 0)
            {
                Space.Parent.RecordGameAction(new GameActionCollect(Space.ActivePiece, Space.Location, this));
                Amount = 0;
                Visible = false;
            }
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
