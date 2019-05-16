using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class WindToken : IToken
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
                return TokenConstants.Wind;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Wind.ToString() + TokenConstants.DirectionString(Orientation);
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Wind.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.EFFECT;
            }
        }

        //Wind Strength will push a Fourzy this many spaces in the Orienation Direction.
        public int Strength { get; set; }

        public WindToken(Direction Orientation, int Strength = 1)
        {
            StandardTokenInit();

            this.Orientation = Orientation;
            this.Strength = Strength;
            this.Visible = true;

            this.Type = TokenType.WIND;
            this.changePieceDirection = true;
        }

        public WindToken(string Notation)
        {
            StandardTokenInit();
            this.Visible = true;
            this.Type = TokenType.WIND;
            this.changePieceDirection = true;

            if (Notation.Length > 1)
            {
                this.Orientation = TokenConstants.IdentifyDirection(Notation[1].ToString());
            }
            else
            {
                this.Orientation = Direction.NONE;
            }

            if (Notation.Length > 2)
            {
                this.Strength = int.Parse(Notation[2].ToString());
            }
            else
            {
                this.Strength = 1;
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

        public void ApplyElement(ElementType Element)
        {
        }

        public void FuelFlames(MovingPiece Piece)
        {
            if (Piece.Piece.Conditions.Contains(PieceConditionType.FIERY))
            {
                Piece.Piece.Conditions.Add(PieceConditionType.FIERY);
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
            if (Space.Location.Equals(Piece.Location) && Piece.Direction != Orientation)
            {
                Piece.Piece.Conditions.Add(PieceConditionType.WIND);

                FuelFlames(Piece);

                BoardLocation WindPushLocation = Space.Location.Neighbor(Orientation);

                if (WindPushLocation.OnBoard(Space.Parent) && Space.Parent.ContentsAt(WindPushLocation).CanMoveInto(Piece, Orientation))
                {
                    if (!Space.Parent.ContentsAt(WindPushLocation).ContainsPiece)
                    {
                        if (Piece.Direction == BoardLocation.Reverse(Orientation) || Piece.Piece.ConditionCount(PieceConditionType.WIND) <= Strength)
                        {
                            Piece.Location = WindPushLocation.Neighbor(Orientation);
                            Space.Parent.ContentsAt(WindPushLocation).MovePieceOn(Piece);
                            Space.Parent.PieceEntersSpace(Piece);
                        }
                    }
                    Space.RemovePieceFrom();
                }
            }

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
