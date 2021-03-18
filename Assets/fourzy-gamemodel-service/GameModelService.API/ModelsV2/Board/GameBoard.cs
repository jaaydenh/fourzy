using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public class GameBoard
    {
        #region "Properties"

        public int Rows { get; }

        public int Columns { get; }

        //This will be set by any generator code.
        public Area Area { get; set; }

        public int PieceCount
        {
            get
            {
                int count = 0;
                for (int r = 0; r < Rows; r++)
                {
                    for (int c = 0; c < Columns; c++)
                    {
                        count += Contents[r, c].PieceCount;
                    }
                }
                return count;
            }
        }

        public int TokenCount
        {
            get
            {
                int count = 0;
                for (int r = 0; r < Rows; r++)
                {
                    for (int c = 0; c < Columns; c++)
                    {
                        count += Contents[r, c].TokenCount;
                    }
                }
                return count;
            }
        }

        [JsonIgnore]
        public GameState Parent { get; set; }

        public RandomTools Random { get; set; }

        public string Recipe { get; set; }
        public string SeedString { get; set; }

        public string ContentString
        {
            get
            {
                {
                    StringBuilder output = new StringBuilder();
                    for (int r = 0; r < Rows; r++)
                    {
                        for (int c = 0; c < Columns; c++)
                        {
                            output.Append("[" + Contents[r, c].Print() + "]");
                        }
                        output.AppendLine();
                    }
                    return output.ToString();
                }
            }
        }

        public Dictionary<BoardLocation, Piece> Pieces
        {
            get
            {
                Dictionary<BoardLocation, Piece> pieces = new Dictionary<BoardLocation, Piece>();
                for (int r = 0; r < Rows; r++)
                {
                    for (int c = 0; c < Columns; c++)
                    {
                        if (Contents[r, c].ContainsPiece) pieces.Add(new BoardLocation(r, c), Contents[r, c].ActivePiece);
                    }
                }
                return pieces;
            }
        }

        [JsonIgnore]
        public List<BoardLocation> Corners
        {
            get
            {
                List<BoardLocation> Locations = new List<BoardLocation>();
                Locations.Add(GetCorner(Corner.UPLEFT));
                Locations.Add(GetCorner(Corner.UPRIGHT));
                Locations.Add(GetCorner(Corner.DOWNLEFT));
                Locations.Add(GetCorner(Corner.DOWNRIGHT));
                return Locations;
            }
        }

        [JsonIgnore]
        public List<BoardLocation> Edges
        {
            get
            {
                List<BoardLocation> Locations = new List<BoardLocation>();
                for (int r = 1; r < Rows-1; r++)
                {
                    Locations.Add(new BoardLocation(r,0));
                    Locations.Add(new BoardLocation(r,Columns-1));

                }
                for (int c = 1; c < Columns- 1; c++)
                {
                    Locations.Add(new BoardLocation(0,c));
                    Locations.Add(new BoardLocation(Rows - 1,c));

                }

                return Locations;
            }
        }

        public List<BoardLocation> Edge(Direction EdgeDirection)
        {
            List<BoardLocation> Locations = new List<BoardLocation>();
            switch (EdgeDirection)
            {
                case Direction.UP:
                    for (int r = 1; r < Rows - 1; r++)
                    {
                        Locations.Add(new BoardLocation(r, 0));
                    }
                    break;
                case Direction.DOWN:
                    for (int r = 1; r < Rows - 1; r++)
                    {
                        Locations.Add(new BoardLocation(r, Columns - 1));
                    }
                    break;
                case Direction.LEFT:
                    for (int c = 1; c < Columns - 1; c++)
                    {
                        Locations.Add(new BoardLocation(0, c));
                    }
                    break;
                case Direction.RIGHT:
                    for (int c = 1; c < Columns - 1; c++)
                    {
                        Locations.Add(new BoardLocation(Rows - 1, c));

                    }
                    break;
            }
            return Locations;
        }

        #endregion

        public BoardSpace[,] Contents;

        [JsonIgnore]
        public Action<GameAction> RecordGameAction;

        public GameBoardData SerializeData()
        {
            GameBoardData data = new GameBoardData
            {
                Rows = this.Rows,
                Columns = this.Columns,
                Area = this.Area
            };

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    data.BoardSpaceData.Add(Contents[r, c].SerializeData());
                }
            }

            return data;
        }

        #region "Constructors"

        public GameBoard(int Rows, int Columns, GameState Parent = null)
        {
            this.Rows = Rows;
            this.Columns = Columns;
            this.Parent = Parent;
            if (Parent != null)
                if (Parent.Random != null) this.Random = Parent.Random;

            Initialize();
        }

        public GameBoard(GameBoardDefinition definition, GameState Parent = null)
        {
            this.Rows = definition.Rows;
            this.Columns = definition.Columns;
            this.Contents = new BoardSpace[definition.Rows, definition.Columns];
            this.Area = definition.Area;
            List<BoardSpaceData> data = definition.BoardSpaceData;

            if (Rows * Columns != data.Count)
                throw new Exception("Invalid GameBoardDefinition");

            int i = 0;
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    Contents[r, c] = new BoardSpace(r, c, data[i]);
                    Contents[r, c].Parent = this;
                    i++;
                }
            }
            this.Parent = Parent;
            if (Parent != null)
            {
                this.Random = Parent.Random;
            }
        }

        public GameBoard(GameBoardData data)
        {
            this.Rows = data.Rows;
            this.Columns = data.Columns;
            this.Area = data.Area;

            this.Contents = new BoardSpace[data.Rows, data.Columns];
            List<BoardSpaceData> boardSpaceData = data.BoardSpaceData;

            if (Rows * Columns != boardSpaceData.Count)
                throw new Exception("Invalid GameBoardDefinition");

            int i = 0;
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    Contents[r, c] = new BoardSpace(r, c, boardSpaceData[i]);
                    Contents[r, c].Parent = this;
                    i++;
                }
            }
            this.Parent = Parent;
            if (Parent != null)
            {
                this.Random = Parent.Random;
            }
        }

        public GameBoard(GameBoard GameBoardToCopy)
        {
            this.Rows = GameBoardToCopy.Rows;
            this.Columns = GameBoardToCopy.Columns;
            this.Contents = new BoardSpace[GameBoardToCopy.Rows, GameBoardToCopy.Columns];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    Contents[r, c] = new BoardSpace(GameBoardToCopy.ContentsAt(r, c));
                    Contents[r, c].Parent = this;

                }
            }
            this.Parent = GameBoardToCopy.Parent;
            this.Random = GameBoardToCopy.Parent.Random;
            this.Area = GameBoardToCopy.Area;

            this.Recipe = GameBoardToCopy.Recipe;
            this.SeedString = GameBoardToCopy.SeedString;
        }

        #endregion

        private void Initialize()
        {
            Contents = new BoardSpace[Rows, Columns];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    Contents[r, c] = new BoardSpace(r, c, this);
                }
            }
        }

        public BoardSpace ContentsAt(int Row, int Column)
        {
            return ContentsAt(new BoardLocation(Row, Column));
        }

        public BoardSpace ContentsAt(BoardLocation Location)
        {
            if (!Location.OnBoard(this))
            {
                throw new Exception("Location Not On Board");
            }
            return Contents[Location.Row, Location.Column];
        }

        public BoardLocation GetCorner(Corner Corner)
        {
            switch (Corner)
            {
                case Corner.UPLEFT:
                    return new BoardLocation(0, 0);
                case Corner.UPRIGHT:
                    return new BoardLocation(0, Columns - 1);
                case Corner.DOWNLEFT:
                    return new BoardLocation(Rows - 1, 0);
                case Corner.DOWNRIGHT:
                    return new BoardLocation(Rows - 1, Columns - 1);
            }
            throw new Exception("No Corner Specified.");
        }

        public List<BoardLocation> GetColumn(int Column)
        {
            List<BoardLocation> Spaces = new List<BoardLocation>();
            for (int r = 0; r < Rows; r++)
                Spaces.Add(new BoardLocation(r, Columns));
            return Spaces;
        }

        public List<BoardLocation> GetRow(int Row)
        {
            List<BoardLocation> Spaces = new List<BoardLocation>();
            for (int c = 0; c < Columns; c++)
                Spaces.Add(new BoardLocation(Row, c));

            return Spaces;
        }


        public List<BoardLocation> GetEmptySpaces()
        {
            List<BoardLocation> Spaces = new List<BoardLocation>();
            foreach (BoardSpace s in Contents) if (s.Empty) Spaces.Add(s.Location);
            return Spaces;
        }

        public List<BoardLocation> GetSpacesWithOnlyTerrain()
        {
            List<BoardLocation> Spaces = new List<BoardLocation>();
            foreach (BoardSpace s in Contents) if (s.ContainsOnlyTerrain && !s.Location.IsCorner(this)) Spaces.Add(s.Location);
            return Spaces;
        }

        //No Pieces and a Piece Can End Here
        public List<BoardLocation> GetOpenSpaces()
        {
            List<BoardLocation> Spaces = new List<BoardLocation>();
            foreach (BoardSpace s in Contents) if (!s.ContainsPiece &&!s.Location.IsCorner(this) && s.TokensAllowEndHere) Spaces.Add(s.Location);
            return Spaces;
        }
        
        // If no level is passed in, the object will increment to the highest existing level + 1
        // This function is designed for setup routines, not for movement.
        public void AddToken(IToken TokenTemplate, BoardLocation Location, AddTokenMethod AddMethod = AddTokenMethod.ONLY_TERRAIN, bool ReplaceTokens = false)
        {
            if (!Location.OnBoard(this))
                return;

            if (Location.IsCorner(this) && AddMethod != AddTokenMethod.CORNERS) 
                return;

            switch (AddMethod)
            {
                case AddTokenMethod.ALWAYS:
                case AddTokenMethod.CORNERS:
                    if (ReplaceTokens && !ContentsAt(Location).Empty)
                        ContentsAt(Location).Tokens.Clear();
                    ContentsAt(Location).AddToken(TokenFactory.Create(TokenTemplate.Notation));
                    break;

                case AddTokenMethod.EMPTY:
                    if (ContentsAt(Location).Empty)
                        ContentsAt(Location).AddToken(TokenFactory.Create(TokenTemplate.Notation));
                    break;

                case AddTokenMethod.IF_NO_TOKEN_MATCH:
                    if (!ContentsAt(Location).ContainsTokenType(TokenTemplate.Type))
                    {
                        if (ReplaceTokens && !ContentsAt(Location).Empty)
                            ContentsAt(Location).Tokens.Clear();
                        ContentsAt(Location).AddToken(TokenFactory.Create(TokenTemplate.Notation));
                    }
                    break;

                case AddTokenMethod.NO_TERRAIN:
                    if (!ContentsAt(Location).ContainsTerrain)
                    {
                        if (ReplaceTokens && !ContentsAt(Location).Empty)
                            ContentsAt(Location).Tokens.Clear();
                        ContentsAt(Location).AddToken(TokenFactory.Create(TokenTemplate.Notation));
                    }
                    break;

                case AddTokenMethod.ONLY_TERRAIN:
                    if (ContentsAt(Location).Empty || ContentsAt(Location).ContainsOnlyTerrain)
                    {
                        if (ReplaceTokens && !ContentsAt(Location).Empty)
                            ContentsAt(Location).Tokens.Clear();
                        ContentsAt(Location).AddToken(TokenFactory.Create(TokenTemplate.Notation));
                    }
                    break;
            }

           // if (InsertOnlyIfEmpty)
           //     if (!ContentsAt(Location).Empty) return;

           //if (ReplaceTokens && !ContentsAt(Location).Empty) ContentsAt(Location).Tokens.Clear();
           
           // ContentsAt(Location).AddToken(Token, Layer);
        }

        public void AddToken(IToken TokenTemplate, List<BoardLocation> Locations, AddTokenMethod AddMethod = AddTokenMethod.ONLY_TERRAIN, bool ReplaceTokens = false)
        {

            foreach (BoardLocation l in Locations)
                if (l.OnBoard(this))
                    if (!l.IsCorner(this) || AddMethod == AddTokenMethod.CORNERS)
                    switch (AddMethod)
                    {
                        case AddTokenMethod.ALWAYS:
                        case AddTokenMethod.CORNERS:
                                if (ReplaceTokens && !ContentsAt(l).Empty)
                                ContentsAt(l).Tokens.Clear();
                            ContentsAt(l).AddToken(TokenFactory.Create(TokenTemplate.Notation));
                            break;

                        case AddTokenMethod.EMPTY:
                            if (ContentsAt(l).Empty)
                                ContentsAt(l).AddToken(TokenFactory.Create(TokenTemplate.Notation));
                            break;

                        case AddTokenMethod.IF_NO_TOKEN_MATCH:
                            if (!ContentsAt(l).ContainsTokenType(TokenTemplate.Type))
                            {
                                if (ReplaceTokens && !ContentsAt(l).Empty)
                                    ContentsAt(l).Tokens.Clear();
                                ContentsAt(l).AddToken(TokenFactory.Create(TokenTemplate.Notation));
                            }
                            break;

                        case AddTokenMethod.NO_TERRAIN:
                            if (!ContentsAt(l).ContainsTerrain)
                            {
                                if (ReplaceTokens && !ContentsAt(l).Empty)
                                    ContentsAt(l).Tokens.Clear();
                                ContentsAt(l).AddToken(TokenFactory.Create(TokenTemplate.Notation));
                            }
                            break;

                        case AddTokenMethod.ONLY_TERRAIN:
                            if (ContentsAt(l).Empty || ContentsAt(l).ContainsOnlyTerrain)
                            {
                                if (ReplaceTokens && !ContentsAt(l).Empty)
                                    ContentsAt(l).Tokens.Clear();
                                ContentsAt(l).AddToken(TokenFactory.Create(TokenTemplate.Notation));
                            }
                            break;
                    }
        }

        public void AddTokenToAllSpaces(IToken TokenTemplate, AddTokenMethod AddMethod = AddTokenMethod.EMPTY)
        {
            foreach (BoardSpace s in Contents)
                if (!s.Location.IsCorner(this))
            switch (AddMethod)
                {
                    case AddTokenMethod.ALWAYS:
                    case AddTokenMethod.CORNERS:
                        ContentsAt(s.Location).AddToken(TokenFactory.Create(TokenTemplate.Notation));
                        break;

                    case AddTokenMethod.EMPTY:
                        if (ContentsAt(s.Location).Empty)
                            ContentsAt(s.Location).AddToken(TokenFactory.Create(TokenTemplate.Notation));
                        break;

                    case AddTokenMethod.IF_NO_TOKEN_MATCH:
                        if (!ContentsAt(s.Location).ContainsTokenType(TokenTemplate.Type))
                            ContentsAt(s.Location).AddToken(TokenFactory.Create(TokenTemplate.Notation));
                        break;

                    case AddTokenMethod.NO_TERRAIN:
                        if (!ContentsAt(s.Location).ContainsTerrain)
                            ContentsAt(s.Location).AddToken(TokenFactory.Create(TokenTemplate.Notation));
                        break;

                    case AddTokenMethod.ONLY_TERRAIN:
                        if (ContentsAt(s.Location).Empty || ContentsAt(s.Location).ContainsOnlyTerrain)
                            ContentsAt(s.Location).AddToken(TokenFactory.Create(TokenTemplate.Notation));
                        break;
                }
        }
        

        // HEY! 
        // THIS FUNCTION NEEDS WORK.  NEED TO IMPLEMENT CLONEABLE SO THAT
        //  WE'RE NOT PASSING A REFRENCE TO THE SAME OBJECT TO EVERY SPACE.  
        //  MOSTLY IT DOESN'T MATTER, BUT.
        //  THIS COULD BE A PROBLEM FOR TOKENS THAT HAVE STATE
        public void AddTokenRegion(TokenRegion Region)
        {
            for (int r = 0; r < Region.Height; r++)
            {
                for (int c=0; c<Region.Width; c++)
                {
                    ContentsAt(Region.Origin.Row + r, Region.Origin.Column + c).AddToken(Region.Token);
                }
            }
        }

        // This function is designed for setup routines, not for movement.
        public void AddPiece(Piece Piece, BoardLocation Location)
        {
            if (!Location.OnBoard(this)) throw new Exception("Location is off the board");
            ContentsAt(Location).AddPiece(Piece);
        }

        public List<IToken> FindTokens(TokenType Type)
        {
            List<IToken> FoundTokens = new List<IToken>() { };

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    FoundTokens.AddRange(Contents[r, c].FindTokens(Type));
                }
            }

            return FoundTokens;
        }

        public List<BoardLocation> FindTokenLocations(TokenType Type)
        {
            List<BoardLocation> Found  = new List<BoardLocation>() { };

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (Contents[r, c].ContainsTokenType(Type)) Found.Add(new BoardLocation(r, c));
                    
                }
            }

            return Found;
        }
        
        public List<Piece> FindPieces(int PlayerId =-1 )
        {
            List<Piece> FoundPiece = new List<Piece>() { };

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (Contents[r,c].ContainsPiece)
                        if (PlayerId < 0 || PlayerId == Contents[r, c].ActivePiece.PlayerId)
                            FoundPiece.Add(Contents[r,c].ActivePiece);
                }
            }

            return FoundPiece;
        }

        public List<BoardLocation> FindControl(int PlayerId = -1)
        {
            List<BoardLocation> FoundPieces = new List<BoardLocation>() { };

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (Contents[r, c].Control == PlayerId)
                            FoundPieces.Add(new BoardLocation(r, c));
                }
            }

            return FoundPieces;
        }


        public List<BoardLocation> FindPieceLocations(int PlayerId = -1)
        {
            List<BoardLocation> FoundPieces = new List<BoardLocation>() { };

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (Contents[r, c].ContainsPiece)
                        if (PlayerId < 0 || PlayerId == Contents[r, c].ActivePiece.PlayerId)
                            FoundPieces.Add(new BoardLocation(r,c));
                }
            }

            return FoundPieces;
        }

        public List<List<BoardLocation>> GetFoursFromLocation(BoardLocation Location)
        {
            List<List<BoardLocation>> Fours = new List<List<BoardLocation>>();

            //horizontal
            for (int c = -3; c <0 ; c++)
            {
                List<BoardLocation> Four = new List<BoardLocation>();
                for (int i=0;i<4;i++)
                    Four.Add(new BoardLocation(Location.Row, c+i));
                Fours.Add(Four);
            }

            //vertical
            for (int r = -3; r < 0; r++)
            {
                List<BoardLocation> Four = new List<BoardLocation>();
                for (int i = 0; i < 4; i++)
                    Four.Add(new BoardLocation(r+i, Location.Column));
                Fours.Add(Four);
            }

            //Diag high right
            for (int c = -3; c < 0; c++)
            {
                List<BoardLocation> Four = new List<BoardLocation>();
                for (int i = 0; i < 4; i++)
                    Four.Add(new BoardLocation(Location.Row + i, c + i));
                Fours.Add(Four);
            }

            //Diag high left
            for (int c = -3; c < 0; c++)
            {
                List<BoardLocation> Four = new List<BoardLocation>();
                for (int i = 0; i < 4; i++)
                    Four.Add(new BoardLocation(Location.Row+3-i, c + i));
                Fours.Add(Four);
            }

            return Fours;
        }




        public void SwapPieces()
        {
            foreach (BoardLocation l in FindPieceLocations())
            {
                ContentsAt(l).SwapPiece();
            }
        }

        #region "Events"

        public void TriggerSwitch(TokenColor Color)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    Contents[r, c].TriggerSwitch(Color);
                }
            }
        }


        public void StartOfTurn(int TurnCount)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    Contents[r, c].StartOfTurn(TurnCount);
                }
            }
        }

        public void PieceEntersBoard(MovingPiece Piece)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    Contents[r, c].PieceEntersBoard(Piece);
                }
            }
        }
        public void PieceEntersSpace(MovingPiece Piece)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    Contents[r, c].PieceEntersSpace(Piece);
                }
            }

            //Check for Multiple Lures.
            if (Piece.ConditionCount(PieceConditionType.LURED) > 1)
            {
                Dictionary<Direction, int> Lures = new Dictionary<Direction, int>();
                foreach (Direction d in TokenConstants.GetDirections())
                {
                    int distance = 1;
                    foreach (BoardLocation l in Piece.Location.Look(this, d))
                    {
                        if (!ContentsAt(l).TokensAllowEnter) break;
                        if (ContentsAt(l).ContainsTokenType(TokenType.LURE))
                        {
                            if (!Lures.ContainsKey(d))
                                Lures.Add(d, distance);
                            else if (Lures[d] < distance) Lures[d] = distance;
                        }
                        distance++;
                    }
                }
                Lures = Lures.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                Piece.Direction = Lures.First().Key;
            }
            Piece.RemoveCondition(PieceConditionType.LURED);

        }
        public void PieceBumpsIntoLocation(MovingPiece Piece, BoardLocation Location)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    Contents[r, c].PieceBumpsIntoLocation(Piece, Location);
                }
            }
        }

        public void PieceStopsOnSpace(MovingPiece Piece)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    Contents[r, c].PieceStopsOnSpace(Piece);
                }
            }
        }

        public void PieceLeavesSpace(MovingPiece Piece)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    Contents[r, c].PieceLeavesSpace(Piece);
                }
            }
        }
        public void EndOfTurn(int PlayerId)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    Contents[r, c].EndOfTurn(PlayerId);
                }
            }
        }

        #endregion
    }
}
