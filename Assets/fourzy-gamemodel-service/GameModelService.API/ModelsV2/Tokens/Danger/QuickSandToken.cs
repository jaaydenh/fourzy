using System.Collections.Generic;
using System.Linq;

namespace FourzyGameModel.Model
{
    public class QuickSandToken : IToken
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
                return TokenConstants.Quicksand;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Quicksand.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Quicksand.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.TERRAIN;
            }
        }

        public QuickSandToken()
        {
            StandardTokenInit();
            this.Type = TokenType.QUICKSAND;
            this.isMoveable = true;
            this.addFriction = 50;
            this.Visible = false;
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

        public void Sink(int PlayerId)
        {
            this.Visible = true;
            if (Space.ActivePiece.PlayerId == PlayerId)
            {
                Space.ActivePiece.Conditions.Add(PieceConditionType.SINKING);
                Space.Parent.RecordGameAction(new GameActionPieceCondition(Space.ActivePiece, Space.Location, PieceConditionType.SINKING));

                if (Space.ActivePiece.Conditions.Count(n => n== PieceConditionType.SINKING) >= Constants.TurnsUntilSinkInQuicksand)
                {
                    Space.Parent.RecordGameAction(new GameActionDestroyed(Space.ActivePiece, Space.Location, DestroyType.QUICKSAND));
                    Space.Pieces.Clear();
                }
            }

        }

        public Direction GetDirection(MovingPiece Piece)
        {
            return Piece.Direction;
        }

        public void ApplyElement(ElementType Element)
        {

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
            //remove all sinking conditions.  //Or not? Would a piece be saved if it was a large block of quicksand?
            if (Piece.Location.Equals(this.Space.Location))
                Piece.RemoveCondition(PieceConditionType.SINKING);

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
            if (Space.ContainsPiece)
            {
                Sink(PlayerId);
            }
        }
    }
}
