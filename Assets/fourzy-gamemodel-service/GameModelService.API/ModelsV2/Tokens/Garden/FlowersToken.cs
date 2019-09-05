using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class FlowersToken : IToken
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

        public int MagicBonus { get; set; }

        public char TokenCharacter
        {
            get
            {
                return TokenConstants.Flowers;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Flowers.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Flowers.ToString() };
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
                return TokenConstants.COMPLEXITY_BASIC;
            }
        }

        public FlowersToken(int MagicBonus = Constants.DefaultFlowerMagic)
        {
            StandardTokenInit();
            this.MagicBonus = MagicBonus;
            this.Type = TokenType.FLOWERS;
        }

        public FlowersToken(string Notation)
        {
            StandardTokenInit();

            if (Notation.Length > 1)
            {
                this.MagicBonus = int.Parse(Notation[1].ToString());
            }
            else
            {
                this.MagicBonus = Constants.DefaultFlowerMagic;
            }

            this.Type = TokenType.FLOWERS;
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
            return Piece.Direction;
        }

        public void Sing()
        {

        }

        public void Burn()
        {

        }

        public void ApplyElement(ElementType Element)
        {
            switch (Element)
            {
                case ElementType.FIRE:
                    Burn();
                    break;
                case ElementType.COLD:
                    break;
                case ElementType.WATER:
                    break;
                case ElementType.HEAT:
                    break;
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
            if (Piece.Location.Equals(Space.Location))
            {
                Space.Parent.Parent.Players[Piece.PlayerId].AddMagic(this.MagicBonus);
            }
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
