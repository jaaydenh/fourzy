using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectFour
{
	public class BoardState
	{
		public const string InitialBoardNotation = "";
		public const string InitialMoves = "";

		public enum ChessPieceType
		{
			None, Rook, Knight, Bishop, Queen, King, Pawn
		}
		public enum FourzyPieceColor
		{
			None, Blue, Red
		}
		// The directions that a rook can move.
		public static readonly List<int[]> RookDirections = new List<int[]> { new int[2] { 0, 1 }, new int[2] { 0, -1 }, new int[2] { -1, 0 }, new int[2] { 1, 0 } };
		// The directions that a bishop can move.
		public static readonly List<int[]> BishopDirections = new List<int[]> { new int[2] { 1, 1 }, new int[2] { 1, -1 }, new int[2] { -1, 1 }, new int[2] { -1, -1 } };
		// The directions that a queen can move, as well as the translational movement of a king.
		public static readonly List<int[]> QueenDirectionsKingTranslations = new List<int[]> { new int[2] { 0, 1 }, new int[2] { 0, -1 }, new int[2] { -1, 0 }, new int[2] { 1, 0 }, new int[2] { 1, 1 }, new int[2] { 1, -1 }, new int[2] { -1, 1 }, new int[2] { -1, -1 } };
		// The distances in x,y that a rook can move.
		public static readonly List<int[]> KnightTranslations = new List<int[]> { new int[2] { 2, 1 }, new int[2] { 1, 2 }, new int[2] { -2, 1 }, new int[2] { 1, -2 }, new int[2] { 2, -1 }, new int[2] { -1, 2 }, new int[2] { -2, -1 }, new int[2] { -1, -2 } };
		// Note that the keys are all uppercase, so lowercase letters should be cast to upper to retrieve the piece type
		private static readonly Dictionary<char, ChessPieceType> CharToChessPieceType = new Dictionary<char, ChessPieceType>{
			{ 'P', ChessPieceType.Pawn   },
			{ 'N', ChessPieceType.Knight },
			{ 'B', ChessPieceType.Bishop },
			{ 'Q', ChessPieceType.Queen  },
			{ 'R', ChessPieceType.Rook   },
			{ 'K', ChessPieceType.King   }
		};
		// Indexed by the ChessPieceType enum to retrieve the capital version of the letter that describes that piece. None Type returns a space because it has no corresponding letter. Note that in some cases a pawn is implied if there is no letter provided.
		private const string ChessPieceToCapitalChar = " RNBQKP";
		private static readonly Dictionary<char, int> FileToColumn = new Dictionary<char, int>
		{
			{ 'a', 0 },
			{ 'b', 1 },
			{ 'c', 2 },
			{ 'd', 3 },
			{ 'e', 4 },
			{ 'f', 5 },
			{ 'g', 6 },
			{ 'h', 7 },
		};
		// Indexed by the column to retrieve the file letter.
		private const string ColumnToFile = "abcdefgh";

		private FourzyPiece[,] BoardGrid;
		private HashSet<ChessMove>[,] PossibleMovesGrid;

		public FourzyPieceColor TurnColor { get; private set; }
		public bool WhiteKingsideCastling { get; private set; }
		public bool WhiteQueensideCastling { get; private set; }
		public bool BlackKingsideCastling { get; private set; }
		public bool BlackQueensideCastling { get; private set; }
		public int HalfMove { get; private set; }
		public int FullMove { get; private set; }
		public Coordinate EnPassantTarget { get; private set; }
		public ChessMove PreviousMove { get; private set; }

		// Take the current state of the board in Forsyth-Edwards Notation and the Long Algebraic Notation
		// of the most recent move (the FEN should already reflect the move).
		public BoardState(string forsythEdwardsNotation, string algebraicNotation)
		{
			BoardGrid = new FourzyPiece[8, 8];
			// Default No color or type (i.e. an empty square)
			for (int row = 0; row < 8; row++)
			{
				for (int column = 0; column < 8; column++)
				{
					this.BoardGrid[row, column] = new FourzyPiece(FourzyPieceColor.None, ChessPieceType.None);
				}
			}
			ParseFEN(forsythEdwardsNotation);
			ParseLongAlgebraicNotation(algebraicNotation);
			InitializeAllPossibleMoves();
		}

		// Create Board State for a new game.
		public BoardState() : this(InitialBoardNotation, "") { }

		// Make a new board state by applying a new move to an already existing board state.
		public BoardState(BoardState previousState, ChessMove newMove, bool lookForCheck = true)
		{
			BoardGrid = new FourzyPiece[8, 8];
			if (!newMove.KingsideCastle && !newMove.QueensideCastle)
			{
				HashSet<ChessMove> previousPossibleMoves = previousState.GetPossibleMoves(newMove.From);
				if (!previousPossibleMoves.Contains(newMove))
				{
					throw new BoardStateException("Illegal move.");
				}
			}
			// Copy elements.
			for (int row = 0; row < 8; row++)
			{
				for (int column = 0; column < 8; column++)
				{
					Coordinate coordinate = new Coordinate(row, column);
					SetPieceAtCoordinate(previousState.GetPieceAtCoordinate(coordinate), coordinate);
				}
			}

			// Turn color will be flipped and fullmove/halfmove will be incremented after move is applied.
			TurnColor = previousState.TurnColor;
			FullMove = previousState.FullMove;
			HalfMove = previousState.HalfMove;

				// If en passant
				if (newMove.PieceType == ChessPieceType.Pawn)
				{
					if (previousState.EnPassantTarget.Equals(newMove.To))
					{
						if (TurnColor == FourzyPieceColor.Blue)
						{
							SetPieceAtCoordinate(new FourzyPiece(FourzyPieceColor.None, ChessPieceType.None), new Coordinate(newMove.To, -1, 0));
						}
						else
						{
							SetPieceAtCoordinate(new FourzyPiece(FourzyPieceColor.None, ChessPieceType.None), new Coordinate(newMove.To, 1, 0));
						}
					}
					// Mark if the new move triggers the possibilty of an En Passant from the following turn.

					int pawnDoubleFromRow = TurnColor == FourzyPieceColor.Blue ? 1 : 6;
					int pawnDoubleToRow = TurnColor == FourzyPieceColor.Blue ? 3 : 4;
					int enPassantTargetTargetRow = TurnColor == FourzyPieceColor.Blue ? 2 : 5;
					if (newMove.From.Row == pawnDoubleFromRow && newMove.To.Row == pawnDoubleToRow)
					{
						EnPassantTarget = new Coordinate(enPassantTargetTargetRow, newMove.From.Column);
					}
				}

				// Set square that the piece is moving from to empty, and moving to to have the piece.
				SetPieceAtCoordinate(new FourzyPiece(FourzyPieceColor.None, ChessPieceType.None), newMove.From);
				SetPieceAtCoordinate(new FourzyPiece(TurnColor, newMove.IsPromotionToQueen ? ChessPieceType.Queen : newMove.PieceType), newMove.To);


			// Set applied move to be the previous move.
			PreviousMove = newMove;

			// Increment fullMove after blacks turn;
			if (TurnColor == FourzyPieceColor.Red)
			{
				FullMove++;
			}

			// Switch turns.
			TurnColor = previousState.TurnColor == FourzyPieceColor.Blue ? FourzyPieceColor.Red : FourzyPieceColor.Blue;

			bool isCheck = false;
			bool isCheckMate = false;
			if (lookForCheck)
			{
				new BoardState(this, isCheck, out isCheckMate);
				PreviousMove = new ChessMove(PreviousMove.From, PreviousMove.To, PreviousMove.PieceType, PreviousMove.IsCapture, PreviousMove.IsPromotionToQueen, PreviousMove.DrawOfferExtended, isCheck, isCheckMate, PreviousMove.KingsideCastle, PreviousMove.QueensideCastle);
			}
			// Finally, determine the list of legal moves.
			InitializeAllPossibleMoves();
		}

		// Hypothetical board state where the turn color has not changed
		private BoardState(BoardState previousState, bool isCheck, out bool isCheckMate)
		{
			BoardGrid = new FourzyPiece[8, 8];
			// Copy elements.
			for (int row = 0; row < 8; row++)
			{
				for (int column = 0; column < 8; column++)
				{
					Coordinate coordinate = new Coordinate(row, column);
					SetPieceAtCoordinate(previousState.GetPieceAtCoordinate(coordinate), coordinate);
				}
			}

			// Copy other board state values.
			BlackKingsideCastling = previousState.BlackKingsideCastling;
			BlackKingsideCastling = previousState.BlackQueensideCastling;
			WhiteKingsideCastling = previousState.WhiteKingsideCastling;
			WhiteQueensideCastling = previousState.WhiteQueensideCastling;
			TurnColor = previousState.TurnColor == FourzyPieceColor.Blue ? FourzyPieceColor.Red : FourzyPieceColor.Blue; ;
			FullMove = previousState.FullMove;
			HalfMove = previousState.HalfMove;
			EnPassantTarget = new Coordinate(-1, -1);
			ParseLongAlgebraicNotation("");

			InitializeAllPossibleMoves();

			isCheckMate = false;

				this.TurnColor = TurnColor == FourzyPieceColor.Blue ? FourzyPieceColor.Red : FourzyPieceColor.Blue;

				InitializeAllPossibleMoves();

				isCheckMate = true;
				for (int row = 0; row < 8 && isCheckMate; row++)
				{
					for (int column = 0; column < 8 && isCheckMate; column++)
					{
						var moves = PossibleMovesGrid[row, column];
						if (moves != null)
						{
							foreach (var move in moves)
							{

									break;

							}
						}
					}
				}

		}

		private void InitializeAllPossibleMoves()
		{
			PossibleMovesGrid = new HashSet<ChessMove>[8, 8];
			for (int row = 0; row < 8; row++)
			{
				for (int column = 0; column < 8; column++)
				{
					PossibleMovesGrid[row, column] = InitializePossibleMoves(new Coordinate(row, column));
				}
			}
		}

		public HashSet<ChessMove> GetPossibleMoves(Coordinate coordinate)
		{
			return new HashSet<ChessMove>(PossibleMovesGrid[coordinate.Row, coordinate.Column]);
		}

		private void ParseFEN(string fen)
		{

		}
			
		private HashSet<ChessMove> InitializePossibleMoves(Coordinate fromCoordinate)
		{
			var legalMoves = new HashSet<ChessMove>();


			return legalMoves;
		}

		// Determine what piece, if any, is on each square of the board by parsing the first
		// section of the FEN, for example: rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR
		private void ParsePosistions(string ranks, string fen)
		{
			// Row is rank - 1, for ease of array indexing (rank is a chess term -
			// the white queen and king are rank 1 in the initial game setup,
			// whereas the black queen and king are rank 8.
			int row = 7;
			// Column corresponds to file, but as integers starting at 0, for ease
			// of array indexing (file is a chess term - queens have file 'd' and
			// kings have file 'e' in the initial game setup.
			int column = 0;
			foreach (char c in ranks)
			{
				// Case that the character represents a chess piece.
				if (CharToChessPieceType.ContainsKey(Char.ToUpper(c)))
				{
					if (column > 7)
					{
						throw new BoardStateException(string.Format("Invalid Forsyth-Edwards Notation '{0}'. More than 8 files in a rank.", fen));
					}

					FourzyPieceColor color = Char.IsUpper(c) ? FourzyPieceColor.Blue : FourzyPieceColor.Red;
					ChessPieceType type = CharToChessPieceType[Char.ToUpper(c)];
					BoardGrid[row, column] = new FourzyPiece(color, type);

					column++;
				}
				// Case that the character represents some number of empty squares.
				else if (Char.IsNumber(c) && c != '0')
				{
					column += Convert.ToInt32(Char.GetNumericValue(c));
				}
				// Case that the character represents moving to the next rank
				else if (c == '/')
				{
					if (column != 8)
					{
						throw new BoardStateException(string.Format("Invalid Forsyth-Edwards Notation '{0}'. Less than 8 files in a rank.", fen));
					}
					row--;
					column = 0;
					if (row < 0)
					{
						throw new BoardStateException(string.Format("Invalid Forsyth-Edwards Notation '{0}'. More than 8 ranks.", fen));
					}
				}
				else
				{
					throw new BoardStateException(string.Format("Invalid Forsyth-Edwards Notation '{0}'. Invalid character '{1}' in piece-positions secion.", fen, c));
				}
			}
			if (row != 0 || column != 8)
			{
				throw new BoardStateException(string.Format("Invalid Forsyth-Edwards Notation '{0}'. Does not describe every square on board.", fen));
			}
		}

		// Determine the castling capability of each player using the castling section of the FEN, for example: KQkq.
		private void ParseCastling(string castling, string fen)
		{
			if (castling.Equals("-"))
			{
				WhiteKingsideCastling = false;
				WhiteQueensideCastling = false;
				BlackKingsideCastling = false;
				BlackQueensideCastling = false;
			}
			else
			{
				WhiteKingsideCastling = castling.Contains("K");
				WhiteQueensideCastling = castling.Contains("Q");
				BlackKingsideCastling = castling.Contains("k");
				BlackQueensideCastling = castling.Contains("q");

				// Not 100% necessary check but could be useful for catching bugs. If there are no castling opportunities, it should be marked with a "-".
				if (!(WhiteKingsideCastling || WhiteQueensideCastling || BlackKingsideCastling || BlackQueensideCastling))
				{
					throw new BoardStateException(string.Format("Invalid Forsyth-Edwards Notation '{0}'. Castling section is invalid.", fen));
				}
			}
		}

		// Determine the potential target of an "en passant" move using the en passant section of the FEN, for example: c3.
		private void ParseEnPassantTarget(string enPassantTargetString, string fen)
		{
			if (enPassantTargetString.Equals("-"))
			{
				EnPassantTarget = new Coordinate(-1, -1);
			}
			else if (enPassantTargetString.Length != 2)
			{
				throw new BoardStateException(string.Format("Invalid Forsyth-Edwards Notation '{0}'. En Passant Target is not 2 characters.", fen));
			}
			else if (FileToColumn.ContainsKey(enPassantTargetString[0]) && Char.IsNumber(enPassantTargetString[1]) && enPassantTargetString[1] != '0' && enPassantTargetString[1] != '9')
			{
				EnPassantTarget = new Coordinate(FileToColumn[enPassantTargetString[0]], Convert.ToInt32(Char.GetNumericValue(enPassantTargetString[1])) - 1);
			}
			else
			{
				throw new BoardStateException(string.Format("Invalid Forsyth-Edwards Notation '{0}'. Invalid En Passant Target.", fen));
			}
		}

		// Determine what the previous move was using Long Algebraic Notation, for example: Rd3xd7Q.
		private void ParseLongAlgebraicNotation(string moveString)
		{
			
		}

		public string ToForsythEdwardsNotation()
		{
			var fenBuilder = new StringBuilder();
			// Number of empty squares in a row. E.g. if a row has a white pawn, 6 empty squares, then a black queen, it will be "P6q".
			int repeatedEmptySquares = 0;
			// Fill in piece position section of fen
			for (int row = 7; row >= 0; row--)
			{
				for (int column = 0; column < 8; column++)
				{
					if (BoardGrid[row, column].Type == ChessPieceType.None)
					{
						repeatedEmptySquares++;
					}
					else
					{
						// Flush repeated empty squares
						if (repeatedEmptySquares > 0)
						{
							fenBuilder.Append(repeatedEmptySquares.ToString());
							repeatedEmptySquares = 0;
						}
						char c = ChessPieceToCapitalChar[(int)(BoardGrid[row, column].Type)];
						// Use lower case for black pieces
						fenBuilder.Append((BoardGrid[row, column].Color == FourzyPieceColor.Red) ? Char.ToLower(c) : c);
					}
				}
				// Flush repeated empty squares
				if (repeatedEmptySquares > 0)
				{
					fenBuilder.Append(repeatedEmptySquares.ToString());
					repeatedEmptySquares = 0;
				}
				// Last rank does not end with a slash, so instead add a space to move to the next setcion.
				fenBuilder.Append(row == 0 ? ' ' : '/');
			}

			// Who moves next, plus space to move to next section.
			fenBuilder.Append(TurnColor == FourzyPieceColor.Blue ? "w " : "b ");

			// Castling section
			if (!(WhiteKingsideCastling || WhiteQueensideCastling || BlackKingsideCastling || BlackQueensideCastling))
			{
				fenBuilder.Append('-');
			}
			else
			{
				if (WhiteKingsideCastling)
				{
					fenBuilder.Append('K');
				}
				if (WhiteQueensideCastling)
				{
					fenBuilder.Append('Q');
				}
				if (BlackKingsideCastling)
				{
					fenBuilder.Append('k');
				}
				if (BlackQueensideCastling)
				{
					fenBuilder.Append('q');
				}
			}

			// En Passant Target, Half Move, and Full Move sections.
			fenBuilder.Append(' ');
			fenBuilder.Append(EnPassantTarget.ToString());
			fenBuilder.Append(' ');
			fenBuilder.Append(HalfMove.ToString());
			fenBuilder.Append(' ');
			fenBuilder.Append(FullMove.ToString());

			return fenBuilder.ToString();
		}

		public struct FourzyPiece
		{
			private FourzyPieceColor color;
			private ChessPieceType type;
			public FourzyPieceColor Color
			{
				get { return color; }
			}
			public ChessPieceType Type
			{
				get { return type; }
			}
			public FourzyPiece(FourzyPieceColor color, ChessPieceType type)
			{
				this.color = color;
				this.type = type;
			}
		}

		public struct Coordinate
		{
			private int row;
			private int column;
			public int Row { get { return row; } }
			public int Column { get { return column; } }
			public Coordinate(int row, int column)
			{
				this.row = row;
				this.column = column;
			}
			// Make a new coordinate based on a translation of another coordinate
			public Coordinate(Coordinate originalCoordinate, int rowTranslation, int columnTranslation)
			{
				this.row = originalCoordinate.Row + rowTranslation;
				this.column = originalCoordinate.Column + columnTranslation;
			}
			public override string ToString()
			{
				return IsInBoardBounds() ? ColumnToFile[column] + (row + 1).ToString() : "-";
			}

			// Check if the coordinate is within the bounds of the board (i.e. not beyond the 8th row/column now before the 1st).
			public bool IsInBoardBounds()
			{
				return column >= 0 && column <= 7 && row >= 0 && row <= 7;
			}

			public override bool Equals(object obj)
			{
				return (obj is Coordinate) && ((Coordinate)obj).Row == row && ((Coordinate)obj).Column == column;
			}
		}

		public struct ChessMove
		{
			private Coordinate from;
			private Coordinate to;
			private ChessPieceType pieceType;
			private bool isCapture;
			private bool isPromotionToQueen;
			private bool drawOfferExtended;
			private bool isCheck;
			private bool isCheckMate;
			private bool kingsideCastle;
			private bool queensideCastle;
			public Coordinate From { get { return from; } }
			public Coordinate To { get { return to; } }
			public ChessPieceType PieceType { get { return pieceType; } }
			public bool IsCapture { get { return isCapture; } }
			public bool IsPromotionToQueen { get { return isPromotionToQueen; } }
			public bool DrawOfferExtended { get { return drawOfferExtended; } }
			public bool IsCheck { get { return isCheck; } }
			public bool IsCheckMate { get { return isCheckMate; } }
			public bool KingsideCastle { get { return kingsideCastle; } }
			public bool QueensideCastle { get { return queensideCastle; } }

			public ChessMove(Coordinate from, Coordinate to, ChessPieceType pieceType, bool isCapture, bool isPromotionToQueen, bool drawOfferExtended, bool isCheck, bool isCheckMate, bool kingsideCastle, bool queensideCastle)
			{
				this.from = from;
				this.to = to;
				this.pieceType = pieceType;
				this.isCapture = isCapture;
				this.isPromotionToQueen = isPromotionToQueen;
				this.drawOfferExtended = drawOfferExtended;
				this.isCheck = isCheck;
				this.isCheckMate = isCheckMate;
				this.kingsideCastle = kingsideCastle;
				this.queensideCastle = queensideCastle;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is ChessMove))
				{
					return false;
				}
				var m = (ChessMove)obj;
				return m.From.Equals(from) &&
					m.To.Equals(to) &&
					m.PieceType == pieceType &&
					m.IsCapture == isCapture &&
					m.IsPromotionToQueen == isPromotionToQueen &&
					m.DrawOfferExtended == drawOfferExtended &&
					m.IsCheck == isCheck &&
					m.IsCheckMate == isCheckMate &&
					m.KingsideCastle == kingsideCastle &&
					m.QueensideCastle == queensideCastle;
			}

			public string ToLongAlgebraicNotation()
			{
				string notation = "";
				return notation;
			}
		}

		// Any Exceptions involving the Board State
		public class BoardStateException : System.Exception
		{
			public BoardStateException() : base() { }
			public BoardStateException(string message) : base(message) { }
			public BoardStateException(string message, System.Exception inner) : base(message, inner) { }
		}

		public FourzyPiece GetPieceAtCoordinate(Coordinate coordinate)
		{
			return BoardGrid[coordinate.Row, coordinate.Column];
		}

		private void SetPieceAtCoordinate(FourzyPiece piece, Coordinate coordinate)
		{
			BoardGrid[coordinate.Row, coordinate.Column] = piece;
		}
	}
}