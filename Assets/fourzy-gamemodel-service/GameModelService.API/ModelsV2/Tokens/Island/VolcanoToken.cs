using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class VolcanoToken : IToken
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


        //if Active, Volcano will erupt and send lava on the board.
        public bool Active { get; set; }
        public int DurationUntilErupt { get; set; }
               
        public char TokenCharacter
        {
            get
            {
                return TokenConstants.Volcano;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Volcano.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Volcano.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.EFFECT;
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

        public VolcanoToken()
        {

            StandardTokenInit();

            this.Type = TokenType.VOLCANO;
            this.pieceCanEnter = false;
            this.pieceMustStopOn = false;
            this.pieceCanEndMoveOn = false;
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

        public void Erupt()
        {
            //send lava on the board.
            foreach (Direction d in Direction.GetValues(typeof(Direction)))
            {
                BoardLocation LavaTarget = Space.Location.Neighbor(d);

                //For now, empty is ok, but we should have it consume stuff.
                if (LavaTarget.OnBoard(Space.Parent) && Space.Parent.ContentsAt(LavaTarget).Empty)
                {
                    Space.Parent.AddToken(new LavaToken(), LavaTarget);
                }
            }
        }
        
        public void Agitate()
        {
            //if a piece bumps the volcano, will decrease timer.
            if (Active) DurationUntilErupt--;
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
            if (Space.Location.Equals(Location)) Agitate();
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
            if (Active)
            {
                DurationUntilErupt--;
                if (DurationUntilErupt <= 0) Erupt();
            }
            

        }
    }
}
