using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class SharkToken : IToken
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

        public bool onSurface { get; set; }

        public char TokenCharacter
        {
            get
            {
                return TokenConstants.Shark;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Shark.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Shark.ToString() };
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

        public SharkToken(Direction Orientation, bool OnSurface = false)
        {
            StandardTokenInit();

            this.Orientation = Orientation;
            this.onSurface = OnSurface;

            this.Type = TokenType.SHARK;

            this.pieceCanEnter = true;
            this.pieceMustStopOn = true;
            this.pieceCanEndMoveOn = true;
   
        }

        public SharkToken(string Notation)
        {
            StandardTokenInit();

            this.Type = TokenType.SHARK;

            this.pieceCanEnter = true;
            this.pieceMustStopOn = true;
            this.pieceCanEndMoveOn = true;


            if (Notation.Length > 1)
            {
                this.Orientation = TokenConstants.IdentifyDirection(Notation[1].ToString());
            }
            else
            {
                this.Orientation = Direction.DOWN;
            }

            if (Notation.Length > 2)
            {
                this.onSurface = bool.Parse(Notation[2].ToString());
            }
            else
            {
                this.onSurface = true;
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

        public void Rise()
        {

        }

        public void Sink()
        {

        }

        public void Swim()
        {
            Space.Parent.ContentsAt(Space.Location.Neighbor(Orientation)).AddToken(new GuardToken(Orientation));
            Space.Parent.RecordGameAction(new GameActionTokenMovement(this, TransitionType.SHARK_SWIM , Space.Location, Space.Location.Neighbor(Orientation)));
            Space.RemoveTokens(TokenType.SHARK);
        }

        public void AboutFace()
        {
            GuardToken G_0 = new GuardToken(Orientation);
            Orientation = BoardLocation.Reverse(Orientation);
            Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.SHARK_SPIN, G_0, this));
        }

        public void Eat()
        {
            Space.Parent.RecordGameAction(new GameActionDestroyed(Space.ActivePiece, Space.Location, DestroyType.EATEN_BY_SHARK));
            Space.Pieces.Clear();
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
            if (Space.ContainsPiece) Eat();
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
            if (Space.Parent.ContentsAt(Space.Location.Neighbor(Orientation)).TokensAllowEnter) Swim();
            else AboutFace();
        }
    }
}
