using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class LureToken : IToken
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
                return TokenConstants.Lure;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Lure.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Lure.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.ITEM;
            }
        }
        public bool Eaten { get; set; }

        public LureToken(bool Eaten = false)
        {
            StandardTokenInit();
            this.Eaten = Eaten;
            this.Type = TokenType.LURE;
            this.pieceMustStopOn = true;
            this.addFriction = 100;
        }

        public LureToken(string Notation)
        {
            StandardTokenInit();
            this.Type = TokenType.LURE;
            this.pieceMustStopOn = true;
            this.addFriction = 100;

            if (Notation.Length > 1)
            {
                this.Eaten = Convert.ToBoolean(Notation[1].ToString());
            }
            else
            {
                this.Eaten = false;
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

        public void Eat()
        {
            Eaten = true; ;
            Visible = false;

            //We may want to remove just one lure? or will a fourzy eat all lures??
            Space.RemoveOneToken(TokenType.LURE);
            Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.EAT_LURE, this, null));
            Space.Parent.RecordGameAction(new GameActionCollect(Space.ActivePiece, Space.Location, this));
        }

        public Direction GetDirection(MovingPiece Piece)
        {
            return Orientation;
        }


        public void Burn()
        {
            Eaten = true; ;
            Visible = false;

            //We may want to remove just one lure? or will a fourzy eat all lures??
            Space.RemoveOneToken(TokenType.LURE);
            Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.BURNING, this, null));

        }

        public void ApplyElement(ElementType Element)
        {
            switch (Element)
            {
                case ElementType.FIRE:
                    Delete = true;
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
            if (Piece.Location.Equals(Space.Location)) return;

            foreach (Direction d in TokenConstants.GetDirections())
            {
                foreach (BoardLocation l in Piece.Location.Look(Space.Parent, d))
                {
                    if (!Space.Parent.ContentsAt(l).TokensAllowEnter) break;
                    if (l.Equals(Space.Location))
                    {
                        Piece.Direction = d;
                        Piece.AddCondition(PieceConditionType.LURED);
                    }
                }
            }

            //foreach (BoardLocation l in  Piece.Location.GetColumn(Space.Parent))
            //{
            //    if (l.Equals(Space.Location))
            //    {
            //        if (Piece.Location.Row < Space.Location.Row) Piece.Direction = Direction.DOWN;
            //        else Piece.Direction = Direction.UP;
            //        Piece.AddCondition(PieceConditionType.LURED);
            //    }
            //}

            //foreach (BoardLocation l in Piece.Location.GetRow(Space.Parent))
            //{
            //    if (l.Equals(Space.Location))
            //    {
            //        if (Piece.Location.Column < Space.Location.Column) Piece.Direction = Direction.RIGHT;
            //        else Piece.Direction = Direction.LEFT;
            //        Piece.AddCondition(PieceConditionType.LURED);
            //    }
            //}

        }

        public void PieceLeavesSpace(MovingPiece Piece)
        {
        }

        public void PieceBumpsIntoLocation(MovingPiece Piece, BoardLocation Location)
        {
        }

        public void PieceStopsOnSpace(MovingPiece Piece)
        {
            if (Space.ContainsPiece && !Eaten)
            {
                Eat();
            }
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
