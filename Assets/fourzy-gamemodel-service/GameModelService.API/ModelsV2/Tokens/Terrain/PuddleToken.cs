using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class PuddleToken : IToken
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

        public bool splashed { get; set; }

        public char TokenCharacter
        {
            get
            {
                return TokenConstants.Puddle;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Puddle.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Puddle.ToString() };
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

        public PuddleToken()
        {
            StandardTokenInit();

            this.Type = TokenType.PUDDLE;
            this.pieceMustStopOn = true;
            this.isMoveable = false;
            this.addFriction = 100;
            this.splashed = false;
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

        public void Freeze()
        {
            Visible = false;
            pieceMustStopOn = false;
            addFriction = 0;
            Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.WATER_FREEZE, this, new IceToken()));

            //Do we want to add and remove tokens, or just change properties??
            Space.AddToken(new IceToken());
            Space.RemoveTokens(TokenType.PUDDLE);
        }

        public void Splash()
        {
            splashed = true;
            Visible = false;
            pieceMustStopOn = true;
            addFriction = 100;
            Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.SPLASH, this, null));

            Space.RemoveTokens(TokenType.PUDDLE);
        }

        public void Evaporate()
        {
            splashed = true;
            Visible = false;
            pieceMustStopOn = true;
            addFriction = 100;
            Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.EVAPORATE, this, null));

            Space.RemoveTokens(TokenType.PUDDLE);
        }

        public void ApplyElement(ElementType Element)
        {
            switch (Element)
            {
                case ElementType.FIRE:
                    Evaporate();
                    break;
                case ElementType.COLD:
                    Freeze();
                    break;
                case ElementType.WATER:
                    break;
                case ElementType.HEAT:
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
            if (!splashed && Piece.Location.Equals(Space.Location)) Splash();
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
