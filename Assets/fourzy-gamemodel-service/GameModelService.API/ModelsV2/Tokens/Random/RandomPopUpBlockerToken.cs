using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class RandomPopUpBlockerToken : IToken
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

        public char TokenCharacter { get; set; }

        public string Notation
        {
            get
            {
                return TokenConstants.RightTurn.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.RightTurn.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.BLOCKER;
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
                return false;
            }
        }

        public int Percentage { get; set; }
        public bool Raised { get; set; }

        public RandomPopUpBlockerToken(bool Raised, int Percentage)
        {

            StandardTokenInit();

            this.Type = TokenType.POPUP_BLOCKER;
            this.TokenCharacter = 'B';

            if (Raised)
            {
                this.pieceCanEnter = false;
                this.pieceCanEndMoveOn = false;
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
            return Direction.NONE;
        }

        public void TurnGears()
        {
            if (Space.Random.Chance(Percentage))
            {
                if (Raised) Lower();
                else Raise();
            }
        }
        public void Raise()
        {
            Space.Parent.RecordGameAction(new GameActionDestroyed(Space.ActivePiece, Space.Location, DestroyType.CRUSHED_BY_BLOCKER));
            Space.Pieces.Clear();

            this.pieceCanEnter = false;
            this.pieceCanEndMoveOn = false;
            Raised = true;
        }
        public void Lower()
        {
            this.pieceCanEnter = true;
            this.pieceCanEndMoveOn = true;
            Raised = false;
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
            TurnGears();
        }
    }
}
