using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class WispToken : IToken
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
                return TokenConstants.Wisp;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Wisp.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Wisp.ToString() };
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

        public int Chance;

        public WispToken(int ChanceToMove = TokenConstants.WispMovePercentage)
        {
            StandardTokenInit();

            this.Type = TokenType.WISP;
            this.pieceCanEnter = true;
            this.pieceMustStopOn = false;
            this.pieceCanEndMoveOn = false;
        }

        public WispToken(string Notation)
        {
            StandardTokenInit();

            this.Type = TokenType.WISP;
            this.pieceCanEnter = true;
            this.pieceMustStopOn = false;
            this.pieceCanEndMoveOn = false;
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

        public void Move()
        {
            BoardLocation Destination = Space.Random.RandomLocation(Space.Location.Look(Space.Parent,Orientation));
            Space.Parent.ContentsAt(Destination).AddToken(new WispToken(Chance));
            //Need a GameAction.

            Space.RemoveTokens(TokenType.WISP);
        }
        public void Spin()
        {
            Orientation = Space.Random.RandomDirection();
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
            if (Space.Random.Chance(Chance))
            {
                Spin();
                Move();
            }
        }
    }
}
