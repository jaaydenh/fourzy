using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class MovingCloudToken : IToken
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
                return TokenConstants.MovingCloud;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.MovingCloud.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.MovingCloud.ToString() };
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

        public bool DisruptsWin
        {
            get
            {
                return true;
            }
        }

        public MovingCloudToken(Direction Orientation = Direction.DOWN)
        {
            StandardTokenInit();

            this.Type = TokenType.MOVING_CLOUD;
            this.pieceCanEnter = true;
            this.pieceMustStopOn = false;
            this.pieceCanEndMoveOn = false;
            this.adjustMomentum = 0;
            this.Orientation = Orientation;
        }

        public MovingCloudToken(string Notation)
        {
            StandardTokenInit();

            this.Type = TokenType.MOVING_CLOUD;
            this.pieceCanEnter = true;
            this.pieceMustStopOn = false;
            this.pieceCanEndMoveOn = false;
            this.adjustMomentum = 0;

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

        public void Move()
        {
            Space.Parent.ContentsAt(Space.Location.Neighbor(Orientation)).AddToken(new SharkToken(Orientation));
            Space.Parent.RecordGameAction(new GameActionTokenMovement(this, TransitionType.SHARK_SWIM, Space.Location, Space.Location.Neighbor(Orientation)));
            Space.RemoveTokens(TokenType.SHARK);
        }
        public void Spin()
        {
            Orientation = Space.Random.RandomDirection();
        }

        public void Rain()
        {
            //Maybe Puddle, then water??
            Space.AddToken(new WaterToken());
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
            if (Space.Empty) Rain();
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
            Spin();
            if (Space.Parent.ContentsAt(Space.Location.Neighbor(Orientation)).Empty) Move();
        }
    }
}
