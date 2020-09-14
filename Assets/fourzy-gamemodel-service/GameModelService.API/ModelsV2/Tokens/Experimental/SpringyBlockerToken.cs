using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class SpringyBlockerToken : IToken
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
                return TokenConstants.SpringyBlocker;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.SpringyBlocker.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.SpringyBlocker.ToString() };
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
                return false;
            }
        }

        public int Complexity
        {
            get
            {
                return TokenConstants.COMPLEXITY_NORMAL;
            }
        }

        public bool DisruptsWin
        {
            get
            {
                return false;
            }
        }
        
        public SpringyBlockerToken()
        {

            StandardTokenInit();

            this.Type = TokenType.SPRINGY_BLOCKER;
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
            if (Location.Equals(Space.Location))
            {
                BoardSpace Target = Space.Parent.ContentsAt(Space.Location.Neighbor(Piece.Direction));

                //check to see if the target is on the board.
                if (!Target.Location.OnBoard(Space.Parent)) return;

                if (Target.ContainsPiece)
                {
                    Space.Parent.PieceBumpsIntoLocation(Piece, Target.Location);
                    switch (Piece.Direction)
                    {
                        case Direction.UP:
                            Target.ActivePiece.Conditions.Add(PieceConditionType.PUSHED_UP);
                            break;
                        case Direction.DOWN:
                            Target.ActivePiece.Conditions.Add(PieceConditionType.PUSHED_DOWN);
                            break;
                        case Direction.LEFT:
                            Target.ActivePiece.Conditions.Add(PieceConditionType.PUSHED_LEFT);
                            break;
                        case Direction.RIGHT:
                            Target.ActivePiece.Conditions.Add(PieceConditionType.PUSHED_RIGHT);
                            break;


                    }

                }
              
            }
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
