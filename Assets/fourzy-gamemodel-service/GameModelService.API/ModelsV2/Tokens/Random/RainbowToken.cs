using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class RainbowToken : IToken
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
        public bool Triggered { get; set; }

        public char TokenCharacter
        {
            get
            {
                return TokenConstants.Rainbow;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Rainbow.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Rainbow.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
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

        public bool DisruptsWin
        {
            get
            {
                return false;
            }
        }

        public RainbowToken()
        {
            StandardTokenInit();

            this.Type = TokenType.RAINBOW;
            this.changePieceDirection = true;
            this.Triggered = false;
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

        public void Fade()
        {
        }

        public Direction GetDirection(MovingPiece Piece)
        {
            Direction NewDirection = Piece.Direction;
            List<Direction> NewDirections = new List<Direction>() { };
            //Step 1. Get Possible Directions.

            foreach (Direction d in TokenConstants.GetDirections())
            {
                if (Piece.Direction == d) continue;
                if (!Space.Location.Neighbor(d).OnBoard(Space.Parent)) continue;
                BoardSpace s = Space.Parent.ContentsAt(Space.Location.Neighbor(d));
                if (s.CanMoveInto(Piece, d)) NewDirections.Add(d);
            }

            if (NewDirections.Count > 0) NewDirection = Space.Random.RandomDirection(NewDirections);
            else NewDirection = Piece.Direction;
       
            return NewDirection;
        }

        public void ApplyElement(ElementType Element)
        {
        }

        public void EndOfTurn(int PlayerId)
        {
            if (Triggered) {
                Delete = true;
                Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.SPELL_FADE, this, null));
            }

        }

        public void PieceEntersBoard(MovingPiece Piece)
        {
        }

        public void PieceEntersSpace(MovingPiece Piece)
        {
            if (Piece.Location.Equals(Space.Location))
            {
                this.Triggered = true;
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
        }
    }
}
