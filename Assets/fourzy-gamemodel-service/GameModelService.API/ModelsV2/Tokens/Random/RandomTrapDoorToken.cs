using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class RandomTrapDoorToken : IToken
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
                return TokenConstants.TrapDoor;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.TrapDoor.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.TrapDoor.ToString() };
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
                return TokenConstants.COMPLEXITY_MEDIUM;
            }
        }

        public bool DisruptsWin
        {
            get
            {
                return false;
            }
        }

        public bool Open { get; set; }
        public TokenColor Color { get; set; }
        public int ChanceOpen { get; set; }

        public RandomTrapDoorToken(TokenColor Color, bool Open = false, int ChanceOpen=5)
        {
            StandardTokenInit();

            this.Type = TokenType.TRAPDOOR;
            this.Color = Color;
            this.Open = Open;
            this.ChanceOpen = ChanceOpen;
        }

        public RandomTrapDoorToken(string Notation)
        {
            throw new System.NotImplementedException();
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

        public void TriggerSwitch(TokenColor Color)
        {
            if (Color == this.Color) Activate();
        }

        public void Activate()
        {
            if (!this.Open) OpenTrapDoor();
            else CloseTrapDoor();
        }

        public void OpenTrapDoor()
        {
            this.Open = true;
            if (Space.ContainsPiece && this.Open)
            {
                Space.Parent.RecordGameAction(new GameActionDestroyed(Space.ActivePiece, Space.Location, DestroyType.FALLING));
                Space.Pieces.Clear();
            }
        }

        public void CloseTrapDoor()
        {
            this.Open = false;
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
            if (Space.ContainsPiece && this.Open)
            {
                Space.Parent.RecordGameAction(new GameActionDestroyed(Space.ActivePiece, Space.Location, DestroyType.FALLING));
                Space.Pieces.Clear();
            }
        }

        public string Print()
        {
            return "";
        }

        public void StartOfTurn(int PlayerId)
        {
            if (!Open && Space.Parent.Random.Chance(ChanceOpen))
            {
                Activate();
            }
        }
    }
}
