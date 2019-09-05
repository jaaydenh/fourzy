using System.Collections.Generic;
using System.Linq;

namespace FourzyGameModel.Model
{
    public class SpiderToken : IToken
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

        public bool Hiding { get; set; }

        public char TokenCharacter
        {
            get
            {
                return TokenConstants.Spider;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Spider.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Spider.ToString() };
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

        public SpiderToken(bool Hiding = true)
        {

            StandardTokenInit();

            this.Hiding = Hiding;
            this.Type = TokenType.SPIDER;
            this.pieceCanEnter = false;
            this.pieceMustStopOn = false;
            this.pieceCanEndMoveOn = false;
            this.Orientation = Orientation;
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

        public void TrackPrey()
        {
            List<BoardLocation> WebLocations = Space.Parent.FindTokenLocations(TokenType.WEB);
            List<BoardLocation> Targets = new List<BoardLocation>();
            BoardLocation Target = new BoardLocation(0, 0);

            foreach (BoardLocation l in WebLocations)
            {
                if (!Space.Parent.ContentsAt(l).ContainsPiece) Targets.Add(l);
            }

            if (Targets.Count == 0) return;
            if (Targets.Count == 1) Target = Targets.First();
            else Target = Space.Parent.Random.RandomLocation(Targets);

            //Find Shortest Path Between two spaces, a biding by blockers.
            // For now, we'll create a simple test


            Direction VDirection = Direction.NONE;
            Direction HDirection = Direction.NONE;
            Direction MoveDirection = Direction.NONE;

            if (Target.Row > Space.Location.Row) VDirection = Direction.DOWN;
            else if (Target.Row < Space.Location.Row) VDirection = Direction.UP;

            if (Target.Column> Space.Location.Column) HDirection = Direction.RIGHT;
            else if (Target.Column < Space.Location.Column) HDirection = Direction.LEFT;

            if (VDirection != Direction.NONE)
                if (Space.Parent.ContentsAt(Space.Location.Neighbor(VDirection)).TokensAllowEndHere
                    && !Space.Parent.ContentsAt(Space.Location.Neighbor(VDirection)).ContainsPiece) MoveDirection = VDirection;
            else if (HDirection != Direction.NONE)
                if (Space.Parent.ContentsAt(Space.Location.Neighbor(HDirection)).TokensAllowEndHere
                    && !Space.Parent.ContentsAt(Space.Location.Neighbor(HDirection)).ContainsPiece) MoveDirection = HDirection;

            if (MoveDirection!= Direction.NONE)
            {
                Space.Parent.RecordGameAction(new GameActionTokenMovement(this, TransitionType.SPIDER_HUNT, Space.Location, Space.Location.Neighbor(Orientation)));
                Space.Parent.RecordGameAction(new GameActionTokenDrop(new SpiderWebToken(), TransitionType.SPIDER_WEAVE, Space.Location.Neighbor(Orientation), Space.Location ));
                Space.RemoveTokens(TokenType.SPIDER);
                Space.Parent.AddToken(new SpiderWebToken(), Space.Location, AddTokenMethod.ALWAYS);
                Space.Parent.AddToken(new SpiderToken(), Space.Location.Neighbor(HDirection), AddTokenMethod.ALWAYS);
            }
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
            //Spider is squished.
            if (Piece.Location.Equals(Space.Location))
            {
                Space.Parent.RecordGameAction(new GameActionTokenRemove(Space.Location, TransitionType.SPIDER_SQUISH, this));
                Space.RemoveTokens(TokenType.SPIDER);
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

        public void StartOfTurn(int PlayerId)
        {
            //Move closer to the web.
            //Calculate path.  Move around obstacles or wait.
            
        }
    }
}
