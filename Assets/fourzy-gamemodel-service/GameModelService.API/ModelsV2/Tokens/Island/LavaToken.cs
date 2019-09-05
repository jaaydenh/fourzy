using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class LavaToken : IToken
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

        public bool Hot { get; set; }
        public int TurnsUntilCool { get; set; }
        public int TurnsUntilFlow { get; set; }
        
        public char TokenCharacter
        {
            get
            {
                return TokenConstants.Lava;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Lava.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Lava.ToString() };
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
                return true;
            }
        }

        public int Complexity
        {
            get
            {
                return TokenConstants.COMPLEXITY_BASIC;
            }
        }

        public LavaToken()
        {
            StandardTokenInit();

            this.Type = TokenType.LAVA;
            this.isMoveable = true;
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

        public void Flow()
        {
            //Sends lava to a random adjacent space.
            //Lava will destroy many tokens on the destination space.

            List<BoardLocation> Targets = new List<BoardLocation>() { };
            foreach (Direction d in Direction.GetValues(typeof(Direction)))
            {
                BoardLocation LavaTarget = Space.Location.Neighbor(d);

                //For now, empty is ok, but we should have it consume stuff.
                if (LavaTarget.OnBoard(Space.Parent) && Space.Parent.ContentsAt(LavaTarget).Empty)
                {
                    Targets.Add(LavaTarget);
                }
            }
            if (Targets.Count > 0 )
            {
                Space.Parent.AddToken(new LavaToken(), Space.Random.RandomLocation(Targets));
            }
        }

        public void Harden()
        {
            Hot = false;

            //Not sure what other changes we might want. Maybe some friction?
        }
        
        public void ApplyElement(ElementType Element)
        {
            switch (Element)
            {
                case ElementType.FIRE:
                    if (Hot) TurnsUntilFlow--;
                    break;
                case ElementType.COLD:
                    Harden();
                    break;
                case ElementType.WATER:
                    Harden();
                    break;
                case ElementType.HEAT:
                    if (Hot) TurnsUntilFlow--;
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
            if (Hot && Piece.Location.Equals(Space.Location)) Piece.Piece.Conditions.Add(PieceConditionType.FIERY);
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
