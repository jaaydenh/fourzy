using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class CrossBombToken : IToken
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
                return TokenConstants.CrossBomb;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.CrossBomb.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.CrossBomb.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.ITEM;
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

        public int Fuse { get; set; }

        public bool Active { get; set; }

        public CrossBombToken(int Fuse = 0, bool Active = true)
        {
            StandardTokenInit();

            this.Type = TokenType.CROSS_BOMB;
            this.Fuse = Fuse;
            this.Active = true;
        }

        public CrossBombToken(string notation)
        {
            StandardTokenInit();

            this.Type = TokenType.CROSS_BOMB;
            this.Fuse = Fuse;
            this.Active = true;

            if (notation.Length > 1)
            {
                this.Fuse = int.Parse(notation[1].ToString());
            }
            else
            {
                this.Fuse = Fuse;
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
            return Piece.Direction;
        }

        public void Explode()
        {
            if (Space.ContainsPiece && Active)
            {
                GameActionExplosion Boom = new GameActionExplosion(Space.Location, new List<BoardLocation>() { Space.Location });

                Space.Parent.RecordGameAction(new GameActionDestroyed(Space.ActivePiece, Space.Location, DestroyType.BOMB));
                Space.ActivePiece.Conditions.Add(PieceConditionType.DEAD);
                foreach (BoardLocation loc in Space.Location.GetColumn(Space.Parent))
                {
                    Boom.Explosion.Add(loc);
                    if (Space.Parent.ContentsAt(loc).ContainsPiece)
                    {
                        Space.Parent.RecordGameAction(new GameActionDestroyed(Space.Parent.ContentsAt(loc).ActivePiece, loc, DestroyType.BOMB));
                        Space.Parent.ContentsAt(loc).Pieces.Clear();
                    }
                }
                foreach (BoardLocation loc in Space.Location.GetRow(Space.Parent))
                {
                    Boom.Explosion.Add(loc);
                    if (Space.Parent.ContentsAt(loc).ContainsPiece)
                    {
                        Space.Parent.RecordGameAction(new GameActionDestroyed(Space.Parent.ContentsAt(loc).ActivePiece, loc, DestroyType.BOMB));
                        Space.Parent.ContentsAt(loc).Pieces.Clear();
                    }
                }
                Space.Parent.RecordGameAction(Boom);

                Active = false;
                Delete = true;
            }
        }

        public void ApplyElement(ElementType Element)
        {
            switch (Element)
            {
                case ElementType.FIRE:
                    break;
                case ElementType.COLD:
                    break;
                case ElementType.WATER:
                    break;
                case ElementType.HEAT:
                    break;
            }
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
            if (Piece.Location.Equals(Space.Location) && Active)
            {
                Explode();
            }
        }

        public string Print()
        {
            return "";
        }

        public void StartOfTurn(int PlayerId)
        {
            if (Fuse > 0 && Active)
            {
                if (Fuse-- == 0) Explode();
            }
        }
    }
}
