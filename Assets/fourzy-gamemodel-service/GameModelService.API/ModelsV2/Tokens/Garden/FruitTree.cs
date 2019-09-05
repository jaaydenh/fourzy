using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class FruitTreeToken : IToken
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
                return TokenConstants.FruitTree;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.FruitTree.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.FruitTree.ToString() };
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
                return TokenConstants.COMPLEXITY_MEDIUM;
            }
        }

        public FruitTreeToken()
        {

            StandardTokenInit();

            this.Type = TokenType.FRUIT_TREE;
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
                BoardLocation TargetLocation = Space.Location.Neighbor(Piece.Direction);
                //check to see if the target is on the board.
                if (!TargetLocation.OnBoard(Space.Parent)) return;

                BoardSpace Target = Space.Parent.ContentsAt(TargetLocation);
                                
                if (Target.ContainsTokenType(TokenType.FRUIT_TREE))
                {
                    Space.Parent.PieceBumpsIntoLocation(Piece, Target.Location);
                    return;
                }

                if (Target.TokensAllowEnter && !Target.ContainsTokenType(TokenType.STICKY) && !Target.ContainsTokenType(TokenType.FRUIT))
                {
                    if (!Target.ContainsPiece)
                    {
                        FruitToken fruitToken = new FruitToken();
                        fruitToken.Space = Target;

                        Space.Parent.RecordGameAction(new GameActionTokenDrop(fruitToken, TransitionType.DROP_FRUIT, Space.Location, Target.Location));
                        Target.AddToken(fruitToken);
                    }
                    else
                    {
                        StickyToken stickyToken = new StickyToken();
                        stickyToken.Space = Target;

                        Space.Parent.RecordGameAction(new GameActionTokenDrop(stickyToken, TransitionType.DROP_FRUIT_SQUISHED, Space.Location, Target.Location));
                        Target.AddToken(stickyToken);
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
