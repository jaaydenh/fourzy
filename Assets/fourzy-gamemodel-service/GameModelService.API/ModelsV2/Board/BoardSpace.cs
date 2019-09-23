using System;
using System.Collections.Generic;
using System.Linq;

namespace FourzyGameModel.Model
{
    // A board Square is a reference to a particular square on a board.
    public class BoardSpace
    {
        #region "Internal Data"

        public Dictionary<int, IToken> Tokens { get; set; }
        public List<Piece> Pieces { get; set; }
        public BoardLocation Location { get; set; }
        public GameBoard Parent { get; set; }
        public RandomTools Random { get { return Parent.Parent.Random; } }

        // ActivePiece is the only piece that matters for now.
        // The list datastructure may potentially help in the future.
        public Piece ActivePiece
        {
            get
            {
                if (Pieces.Count > 0) return Pieces.First();
                return null;
            }
        }

        #endregion

        #region "Properties"

        public BoardSpaceData SerializeData()
        {
            BoardSpaceData data = new BoardSpaceData();
            data.T = new List<string>();

            if (ActivePiece != null)
            {
                data.P = ActivePiece.Notation;
            }
            else
            {
                data.P = "0";
            }

            foreach (IToken token in Tokens.Values)
            {
                data.T.Add(token.Notation);
            }

            return data;
        }

        // Helper Properties for legibility
        public int PieceCount { get { return Pieces.Count; } }
        public int TokenCount { get { return Tokens.Count; } }
        public bool Empty { get { return (Tokens.Count == 0 && Pieces.Count == 0); } }
        public bool NotEmpty { get { return (Tokens.Count > 0 || Pieces.Count > 0); } }
        public bool ContainsToken { get { return (Tokens.Count > 0); } }
        public bool ContainsPiece { get { return (Pieces.Count > 0); } }

        public bool ContainsTerrain
        {
            get
            {
                //default is false, if any tokens provide is movable, then entire square is movable.
                foreach (IToken t in Tokens.Values)
                {
                    if (t.Classification == TokenClassification.TERRAIN) return true;
                }
                return false;
            }
        }

        public bool ContainsSpell
        {
            get
            {
                //default is false, if any tokens provide is movable, then entire square is movable.
                foreach (IToken t in Tokens.Values)
                {
                    if (t.Classification == TokenClassification.SPELL) return true;
                }
                return false;
            }
        }

        public bool ContainsHex
        {
            get
            {
                //default is false, if any tokens provide is movable, then entire square is movable.
                foreach (IToken t in Tokens.Values)
                {
                    if (t.Type == TokenType.HEX) return true;
                }
                return false;
            }
        }
        public bool ContainsOnlyTerrain
        {
            get
            {
                bool terrain = true;

                foreach (IToken t in Tokens.Values)
                {
                    if (t.Classification != TokenClassification.TERRAIN) 
                        return false;
                }
                return terrain;
            }
        }

        public void ApplyElement(ElementType Element)
        {
            foreach (IToken t in Tokens.Values)
            {
                t.ApplyElement(Element);
            }
        }

        public int CountTokenClass(TokenClassification TokenClass)
        {
            int count = 0;
            foreach (IToken t in Tokens.Values)
            {
                if (t.Classification == TokenClass) count++;
            }
            return count;
        }


        public bool TokensAllowPushing
        {
            get
            {
                //default is false, if any tokens provide is movable, then entire square is movable.
                foreach (IToken t in Tokens.Values)
                {
                    if (t.isMoveable) return true;
                }
                return false;
            }
        }
        public bool TokensAllowEndHere
        {
            get
            {
                //default is false, if any tokens provide is movable, then entire square is movable.
                foreach (IToken t in Tokens.Values)
                {
                    if (!t.pieceCanEndMoveOn) return false;
                }
                return true;
            }
        }
        public bool TokensAllowEnter
        {
            get
            {
                //HEX Will override blockers.
                if (ContainsHex) return true;

                //default is false, if any tokens provide is movable, then entire square is movable.
                foreach (IToken t in Tokens.Values)
                {
                    if (!t.pieceCanEnter) return false;
                }
                return true;
            }
        }
        public bool TokenSetMomentum
        {
            get
            {

                if (ContainsHex) return false;

                //default is false, if any tokens provide is movable, then entire square is movable.
                foreach (IToken t in Tokens.Values)
                {
                    if (t.setMomentum >= 0) return true;
                }
                return false;
            }
        }
        public int TokenSetMomentumValue
        {
            get
            {
                //default is false, if any tokens provide is movable, then entire square is movable.
                int value = -1;
                foreach (IToken t in Tokens.Values)
                {
                    if (t.setMomentum >= 0)
                    {
                        if (value < 0) value = t.setMomentum;
                        else
                        {
                            if (t.setMomentum < value)
                            {
                                value = t.setMomentum;
                            }
                        }
                    }
                }
                return value;
            }
        }
        public int TokenAdjustMomentumValue
        {
            get
            {
                //default is false, if any tokens provide is movable, then entire square is movable.
                int value = -1;

                if (ContainsHex) return value;

                foreach (IToken t in Tokens.Values)
                {
                    if (t.adjustMomentum >= 0)
                    {
                        if (value < 0) value = t.adjustMomentum;
                        else
                        {
                            if (t.adjustMomentum < value)
                            {
                                value = t.adjustMomentum;
                            }
                        }
                    }
                }
                return value;
            }
        }
        public bool TokenAdjustFriction
        {
            get
            {
                if (ContainsHex) return false;

                //default is false, if any tokens provide is movable, then entire square is movable.
                foreach (IToken t in Tokens.Values)
                {
                    if (t.addFriction != 0) return true;
                }
                return false;
            }
        }
        public int TokenAdjustFrictionAmount
        {
            get
            {
                if (ContainsHex) return 0;

                //default is false, if any tokens provide is movable, then entire square is movable.
                int value = -1;
                foreach (IToken t in Tokens.Values)
                {
                    if (t.addFriction >= 0)
                    {
                        if (value < 0) value = t.addFriction;
                        else
                        {
                            if (t.addFriction > value)
                            {
                                value = t.addFriction;
                            }
                        }
                    }
                }
                return value;
            }
        }
        public bool TokenForceStop
        {
            get
            {
                if (ContainsHex) return false;

                foreach (IToken t in Tokens.Values)
                {
                    if (t.pieceMustStopOn) return true;
                }
                return false;
            }
        }

        //blocked if a token prevents piece entry or a piece exists and token prevent movement.
        public bool NoMorePieces
        {
            get
            {
                if (!TokensAllowEnter) return true;
                if (ContainsPiece && !TokensAllowPushing) return true;
                return false;
            }
        }
        public bool TokenAffectsMovement
        {
            get
            {
                if (ContainsHex) return false;

                //default is false, if any tokens provide is movable, then entire square is movable.
                foreach (IToken t in Tokens.Values)
                {
                    if (t.changePieceDirection) return true;
                }
                return false;
            }
        }

        #endregion

        #region "Constructors"

        public BoardSpace(BoardLocation Location, GameBoard Parent = null)
        {
            this.Location = Location;
            Tokens = new Dictionary<int, IToken>();
            Pieces = new List<Piece>();
            this.Parent = Parent;

        }

        public BoardSpace(int Row, int Column, GameBoard Parent = null)
        {
            this.Location = new BoardLocation(Row, Column);
            Tokens = new Dictionary<int, IToken>();
            Pieces = new List<Piece>();
            this.Parent = Parent;
        }

        public BoardSpace(int Row, int Column, BoardSpaceData data, GameBoard Parent = null)
        {
            this.Location = new BoardLocation(Row, Column);
            this.Tokens = new Dictionary<int, IToken>();

            if (data.T != null)
            {
                for (int i = 0; i < data.T.Count; i++)
                {
                    this.AddToken(TokenFactory.Create(data.T[i]), 0);
                }
            }

            this.Pieces = new List<Piece>();

            if (data.P != null && data.P != "0")
            {
                this.AddPiece(new Piece(data.P));
            }

            this.Parent = Parent;
        }

        public BoardSpace(BoardSpace SpaceToCopy)
        {
            this.Location = SpaceToCopy.Location;
            this.Pieces = new List<Piece>();
            foreach (Piece p in SpaceToCopy.Pieces)
            {
                this.Pieces.Add(new Piece(p));
            }
            this.Tokens = new Dictionary<int, IToken>();
            foreach (int k in SpaceToCopy.Tokens.Keys)
            {
                this.AddToken(SpaceToCopy.Tokens[k], k);
            }
        }

        public BoardSpace(BoardLocation Location, List<Piece> Pieces, Dictionary<int, IToken> Tokens, GameBoard Parent = null)
        {
            this.Location = Location;
            this.Pieces = new List<Piece>();
            foreach (Piece p in Pieces)
            {
                this.Pieces.Add(p);
            }
            this.Tokens = new Dictionary<int, IToken>();
            foreach (int k in Tokens.Keys)
            {
                this.Tokens.Add(k, Tokens[k]);
            }

            Parent = null;
        }

        #endregion

        #region "MoveHelperFunctions"

        public bool EdgeOfBoard(Direction Direction)
        {
            switch (Direction)
            {
                case Direction.UP:
                    return (Location.Row == 0);
                case Direction.DOWN:
                    return (Location.Row == Parent.Rows - 1);
                case Direction.LEFT:
                    return (Location.Column == 0);
                case Direction.RIGHT:
                    return (Location.Column == Parent.Columns - 1);
            }
            return false;
        }

        public bool ContainsPieceId(string UniqueId)
        {
            if (PieceCount > 0)
            {
                if (ActivePiece.UniqueId == UniqueId) return true;
            }
            return false;
        }

        public void SwapPiece()
        {
            if (PieceCount >0)
            {
                ActivePiece.PlayerId = Parent.Parent.Opponent(ActivePiece.PlayerId);
            }
        }


        public bool ContainsTokenType(TokenType Token)
        {
            //default is false, if any tokens provide is movable, then entire square is movable.
            foreach (IToken t in Tokens.Values)
            {
                if (t.Type == Token) return true;
            }
            return false;
        }

        public bool CanMoveInto(MovingPiece Piece, Direction Direction, int PushMomentum = -1, bool IgnoreDirection = false)
        {
            if (Empty) return true;

            //Space is filled or not movable
            if (NoMorePieces) return false;

            ////On the Edge of the board in the direction
            //if (EdgeOfBoard(Direction)) return false;

            //A piece needs at least 1 momentum to move.
            if (Piece.Momentum <= 0) return false;

            //Handle Spaces with Walls.
            if (ContainsTokenType(TokenType.WALL))
            {
                foreach (IToken t in Tokens.Values)
                {
                    if (t.Type == TokenType.WALL)
                    {
                        if (((WallToken)t).HasWall(Direction)) return false;
                        break;
                    }
                }
            }

            if (PushMomentum == -1) { PushMomentum = 100; }
            else PushMomentum--;
            if (PushMomentum == 0) return false;

            //Check if I can move into my neighbor.
            if (ContainsPiece && TokensAllowPushing)
            {
                MovingPiece PushedPiece = null;
                if (IgnoreDirection)
                    PushedPiece = new MovingPiece(this.Pieces.First(), this.Location, Piece.Direction, Piece.Momentum);
                else
                    PushedPiece = new MovingPiece(this.Pieces.First(), this.Location, ProcessDirection(Piece), Piece.Momentum);

                //check the neighbor square in the correct direction.
                //ask it if this piece can move into it.
                if (!this.Location.Neighbor(PushedPiece.Direction).OnBoard(this.Parent)) return false;

                bool CanMoveInto = Parent.ContentsAt(this.Location.Neighbor(PushedPiece.Direction)).CanMoveInto(PushedPiece, PushedPiece.Direction, PushMomentum);
                //if (!CanMoveInto & Parent.ContentsAt(this.Location.Neighbor(PushedPiece.Direction)).TokensAllowPushing)
                //{
                //    CanMoveInto = Parent.ContentsAt(this.Location.Neighbor(PushedPiece.Direction)).CanMoveInto(PushedPiece, PushedPiece.Direction, PushMomentum, true);
                //}
                return CanMoveInto;
            }

            if (!TokensAllowEndHere)
            {
                if (!this.Location.Neighbor(ProcessDirection(Piece)).OnBoard(this.Parent)) return false;
                return Parent.ContentsAt(this.Location.Neighbor(ProcessDirection(Piece))).CanMoveInto(Piece, ProcessDirection(Piece));
            }

            return true;
        }

        /// <summary>The player Id of the piece that controls this space</summary>
        public int Control
        {
            get
            {
                if (ContainsPiece) return ActivePiece.PlayerId;
                return 0;
            }
        }

        public int ProcessMomentum(MovingPiece Piece)
        {
            // if (TokenSetMomentum) return TokenSetMomentumValue;
            //if (TokenSetMomentum) for (int i = 1; i < TokenSetMomentumValue; i++) Piece.Piece.Conditions.Add(PieceConditionType.MOMENTUM);
            return Piece.Momentum += TokenAdjustMomentumValue;
        }

        public float ProcessFriction(MovingPiece Piece)
        {
            if (ContainsHex) return Piece.Friction;

            if (!TokenAdjustFriction) return Piece.Friction;
            return Piece.Friction += TokenAdjustFrictionAmount;
        }

        // A Fourzy will default in its current direction unless adjusted;
        public Direction ProcessDirection(MovingPiece Piece)
        {
            if (ContainsHex) return Piece.Direction;

            if (Piece.HasCondition(PieceConditionType.STRAIGHT))
            {
                Piece.RemoveCondition(PieceConditionType.STRAIGHT);
                return Piece.Direction;
            }

            Direction NewDirection = Piece.Direction;
            if (TokenAffectsMovement)
            {
                foreach (KeyValuePair<int, IToken> t in Tokens.OrderBy(key => key.Key))
                {
                    if (t.Value.changePieceDirection)
                    {
                        NewDirection = t.Value.GetDirection(Piece);
                    }
                }
            }

            return NewDirection;
        }

        public MovingPiece MovePieceOff(Direction Direction, int Momentum)
        {
            MovingPiece Piece = new MovingPiece(this.Pieces.First(), this.Location, Direction, Momentum);
            Parent.PieceLeavesSpace(Piece);
            this.Pieces.Clear();
            return Piece;
        }

        public MovingPiece MovePieceOn(MovingPiece NewPiece, bool IgnoreDirectionChange = false)
        {
            NewPiece.Location = Location;

            MovingPiece PushedPiece = null;
            if (Pieces.Count > 0)
            {
                if (TokenAffectsMovement && BoardLocation.Reverse(ProcessDirection(NewPiece)) == NewPiece.Direction && !IgnoreDirectionChange)
                {
                    PushedPiece = new MovingPiece(this.Pieces.First(), this.Location, BoardLocation.Reverse(NewPiece.Direction), NewPiece.Momentum);
                }
                else
                {
                    //A piece that is stopped, loses inertia
                    NewPiece.RemoveCondition(PieceConditionType.INERTIA);
                    if (IgnoreDirectionChange)
                    {
                        PushedPiece = new MovingPiece(this.Pieces.First(), this.Location, NewPiece.Direction, NewPiece.Momentum);
                        PushedPiece.AddCondition(PieceConditionType.STRAIGHT);
                    }
                    else
                        PushedPiece = new MovingPiece(this.Pieces.First(), this.Location, ProcessDirection(NewPiece), NewPiece.Momentum);

                }

                //PushedPiece.Momentum = Parent.Random.RandomInteger(Constants.MinimumMomentum, Constants.MaximumMomentum);
                PushedPiece.Momentum = ProcessMomentum(PushedPiece);
                PushedPiece.Pushed = true;
            }


            //Reset Wind if Current Space has no wind.  
            if (NewPiece.Piece.ConditionCount(PieceConditionType.WIND) > 0 && !ContainsTokenType(TokenType.WIND))
            {
                NewPiece.Piece.ClearCondition(PieceConditionType.WIND);
            }

            NewPiece.Momentum = ProcessMomentum(NewPiece);
            NewPiece.Friction = ProcessFriction(NewPiece);
            Pieces.Clear();
            Pieces.Add(NewPiece.Piece);
            Parent.PieceEntersSpace(NewPiece);

            if (PushedPiece != null) return PushedPiece;
            return NewPiece;
        }

        public void AddToken(IToken Token, int Layer = 0)
        {
            if (Layer == 0)
            {
                if (Tokens.Count == 0)
                {
                    Layer = 1;
                }
                else
                {
                    Layer = Tokens.Keys.Max() + 1;
                }
            }


            if (Tokens.Keys.Contains(Layer)) Layer = Tokens.Keys.Max() + 1;
            IToken NewToken = TokenFactory.Create(Token.Notation);
            NewToken.Layer = Layer;
            NewToken.Space = this;

            Tokens.Add(Layer, NewToken);
        }

        public void RemoveOneToken(TokenType Type)
        {
            foreach (KeyValuePair<int, IToken> t in Tokens.OrderBy(key => key.Key))
            {
                if (t.Value.Type == Type)
                {
                    Tokens.Remove(t.Key);
                    return;
                }
            }
        }

        public void RemoveTokens(TokenType Type)
        {
            foreach (KeyValuePair<int, IToken> t in Tokens.OrderBy(key => key.Key))
            {
                if (t.Value.Type == Type)
                {
                    Tokens.Remove(t.Key);
                }
            }
        }

        public void RemoveAllTokens()
        {
            Tokens.Clear();
        }

        public void AddPiece(Piece Piece)
        {
            if (Pieces.Count > 0) throw new Exception("Space is not empty. Cannot add new piece.");
            Pieces.Add(Piece);
        }

        // Removes the piece from the space
        public Piece RemovePieceFrom()
        {
            Piece PieceOnSpace = null;
            if (Pieces.Count > 0) PieceOnSpace = this.Pieces.First();
            Pieces.Clear();
            return PieceOnSpace;
        }

        public List<IToken> FindTokens(TokenType Type)
        {
            List<IToken> FoundTokens = new List<IToken>() { };

            foreach (IToken t in Tokens.Values)
            {
                if (t.Type == Type) FoundTokens.Add(t);
            }

            return FoundTokens;
        }

        #endregion

        #region "UtilityFunction"

        public string Print()
        {
            string t_string = "";
            string p_string = "0";
            if (PieceCount > 0) p_string = ActivePiece.Notation;
            foreach (IToken t in Tokens.Values)
            {
                if (t.Notation != null) t_string += t.Notation;
            }
            if (t_string.Length == 0) t_string = " ";
            return string.Format("P:{0},T:{1}", p_string, t_string);
        }

        #endregion

        #region "MoveEventProcessing"

        // Need to Code TriggerSwitch
        public void TriggerSwitch(TokenColor Color)
        {
            List<IToken> TList = new List<IToken>();
            foreach (IToken t in Tokens.Values) TList.Add(t);
            //foreach (IToken t in TList) t.TriggerSwitch(Color);
        }

        public void StartOfTurn(int PlayerId)
        {
            foreach (Piece p in Pieces) p.StartOfTurn(PlayerId);

            List<IToken> TList = new List<IToken>();
            foreach (IToken t in Tokens.Values) TList.Add(t);
            foreach (IToken t in TList) t.StartOfTurn(PlayerId);
        }

        public void PieceEntersBoard(MovingPiece Piece)
        {
            List<IToken> TList = new List<IToken>();
            foreach (IToken t in Tokens.Values) TList.Add(t);

            foreach (IToken t in TList) t.PieceEntersBoard(Piece);
        }

        public void PieceEntersSpace(MovingPiece Piece)
        {
            List<IToken> TList = new List<IToken>();
            foreach (IToken t in Tokens.Values) TList.Add(t);

            foreach (IToken t in TList) t.PieceEntersSpace(Piece);
        }

        public void PieceBumpsIntoLocation(MovingPiece Piece, BoardLocation Location)
        {
            foreach (IToken t in Tokens.Values) t.PieceBumpsIntoLocation(Piece, Location);
            if (ContainsPiece && Location.Equals(this.Location))
            {
                foreach (Piece p in Pieces) p.PieceBumpsIntoLocation(Piece, Location);
                if (Location.Neighbor(Piece.Direction).OnBoard(Parent))
                    Parent.PieceBumpsIntoLocation(new MovingPiece(ActivePiece, Location.Neighbor(Piece.Direction), Piece.Direction, 1), Location.Neighbor(Piece.Direction));
            }
        }

        public void PieceStopsOnSpace(MovingPiece Piece)
        {
            List<IToken> TList = new List<IToken>();
            foreach (IToken t in Tokens.Values) TList.Add(t);
            foreach (IToken t in TList) t.PieceStopsOnSpace(Piece);
        }

        public void PieceLeavesSpace(MovingPiece Piece)
        {
            List<IToken> TList = new List<IToken>();
            foreach (IToken t in Tokens.Values) TList.Add(t);

            foreach (IToken t in TList) t.PieceLeavesSpace(Piece);
        }

        public void EndOfTurn(int PlayerId)
        {
            if (ContainsPiece)
            {
                foreach (Piece p in Pieces)
                {
                    p.EndOfTurn(PlayerId);
                }
                if (ActivePiece.Conditions.Contains(PieceConditionType.DEAD)) Pieces.Clear();
            }

            if (ContainsToken)
            {
                //Delete objects before end of turn logic.
                foreach (var key in Tokens.Keys.ToArray().Where(v => Tokens[v].Delete == true)) Tokens.Remove(key);

                List<IToken> TList = new List<IToken>();
                foreach (IToken t in Tokens.Values) TList.Add(t);
                foreach (IToken t in TList) t.EndOfTurn(PlayerId);

                //In case a end of turn logic deletes a piece, check for additional deletes.
                foreach (var key in Tokens.Keys.ToArray().Where(v => Tokens[v].Delete == true)) Tokens.Remove(key);
            }
        }

        #endregion
    }
}
