using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class GrassToken : IToken
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

        public int Height { get; set; }

        public char TokenCharacter
        {
            get
            {
                return TokenConstants.Grass;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Grass.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Grass.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.TERRAIN;
            }
        }

        public GrassToken(int Height = 1)
        {
            StandardTokenInit();
            this.Height= Height;
            this.Type = TokenType.GRASS;
            this.isMoveable = false;
            this.addFriction = 34;
        }

        public GrassToken(string Notation)
        {
            StandardTokenInit();
            this.Type = TokenType.GRASS;
            this.isMoveable = false;
            this.addFriction = 34;

            if (Notation.Length > 1)
            {
                this.Height = int.Parse(Notation[1].ToString());
            }
            else
            {
                this.Height = 1;
            }
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

        public void Burn()
        {
            Visible = false;
            pieceMustStopOn = true;
            addFriction = 0;
            Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.BURNING, this, new FireToken()));

            //Do we want to add and remove tokens, or just change properties??
            Space.AddToken(new FireToken());
            Space.RemoveTokens(TokenType.GRASS);
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
