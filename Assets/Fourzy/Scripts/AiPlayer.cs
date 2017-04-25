using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Fourzy {
    public class AiPlayer {

        string profile = "default";

        public AiPlayer(string profile) {
            this.profile = profile;
        }

        public static List<Move> PotentialMoves()
        {
            List<Move> moves = new List<Move>();
            for (int r = 0; r < Constants.numRows; r++)
            {
                Move m1 = new Move (r, Direction.LEFT);
                Move m2 = new Move (r, Direction.RIGHT);
                moves.Add (m1);
                moves.Add (m2);
            }

            for (int c = 0; c < Constants.numColumns; c++)
            {
                Move m1 = new Move (c, Direction.UP);
                Move m2 = new Move (c, Direction.DOWN);
                moves.Add (m1);
                moves.Add (m2);
            }

            return moves;
        }

        public Move GetRandomMove(List<Move> availableMoves)
        {
            //Move move = new Move(1, Direction.DOWN);

            int move = Random.Range(0, availableMoves.Count-1);

            return availableMoves[move];

            //            int movePosition;
            //            Move move;
            //            Direction direction;
            //            bool canMove = true;
            //            do
            //            {
            //                movePosition = Random.Range(0, 7);
            //                //int dir = Random.Range(0,3);
            //                move = new Move(movePosition, Direction.DOWN);
            //                MovingGamePiece piece = new MovingGamePiece(move.position, Direction.DOWN);
            //                if (GameManager.instance.CanMoveInPosition(new Position(0,0), piece.GetNextPosition(), Direction.DOWN)) {
            //                    canMove = true;
            //                }
            //            } while (!canMove);
        }

        public int GetOpponent(int player) {
            if (player == 1) {
                return 2;
            }
            return 1;
        }

        // gameboard, tokenboard, player 1-2 turn, num rows, num cols
        public Move GetMove(GameBoard gameBoard, TokenBoard tokenBoard, int player)  {
            //Debug.Log("GET MOVE");
            Move move;
            Dictionary<Move, AIGameState> goodMoves = new Dictionary<Move, AIGameState>();

            AIGameState start = new AIGameState(gameBoard, tokenBoard, player);
             
            //We will eventuall have a potential library here.
            //  When we have it, we will look to see if the current state is in the library
            //  and return the matching move

            //List<AIGameState> lookAhead = new List<AIGameState> ();
            Dictionary<Move, AIGameState> lookAhead = new Dictionary<Move, AIGameState>();

            //for each move in start position
            // add ai states to lookahead
            // for each state in lookahead
            //    for each move in state
            //       make move and add to lookAhead
            // this will give us a combination of all possible states from current player
            //  and opponent

            //RULE: IF you can make a winning move, make it!
            Debug.Log("GetAvailableMoves: " + start.GetAvailableMoves().Count);
            foreach (Move m in start.GetAvailableMoves()) {
				//start.gameBoard.PrintBoard ();

				AIGameState state = start.Copy();
			    state.MakeMove(m);
				state.gameBoard.PrintBoard("BEFORE CHECKING FOR WIN");

                if (state.isWinForPlayer == player) {
			        Debug.Log("MAKE FIRST WINNING MOVE for PLAYER=" + player);
					//state.gameBoard.PrintBoard ();

                    return m;
                }
                lookAhead.Add(m, state);
            }

            //RULE: If the opponent can make a winning move, don't make a move that sets that up
            foreach (var x in lookAhead)
            {
                //Debug.Log("LOOKAHEAD NUM: " + x);
                bool willLose = false;

                AIGameState state = x.Value;
                Move parentMove = x.Key;

                foreach (var m in lookAhead.Keys)
                {
                    AIGameState newState = state.Copy();
                    newState.MakeMove(m);

                    if (newState.isWinForPlayer == GetOpponent(player)) {
                        Debug.Log("WILL LOSE is true");
                        willLose = true;
                        break;
                    }
                }

                if (!willLose) {
                    goodMoves.Add(parentMove, state);
                }
            }

            //IF there are NO good moves, then return ANY move
            //IF there is ONLY ONE godd move, then return it.
            Debug.Log("GOOD MOVES COUNT: " + goodMoves.Count);
            if (goodMoves.Count == 0) {
                Debug.Log("GetRandomMove");
                return GetRandomMove(start.GetAvailableMoves());
            } else if (goodMoves.Count == 1) {
                Debug.Log("First good move");
                return goodMoves.Keys.First();
            }

            //TO DO:
            //
            //  For each good move, look one more move deeper
            //
 
            //TO DO:
            //
            //  Prioritize moves with three in a row
            //

            //TO DO:
            //  Prioritize moves next to your own pieces.
           


            //TEMPORARY RULE
            //  Prioritize the available moves if they are on the center
            //    Make the top move in the priority list

            List<Move> priorityMoves = new List<Move>();
            priorityMoves.Add(new Move(3, Direction.DOWN));
            priorityMoves.Add(new Move(4, Direction.DOWN));
            priorityMoves.Add(new Move(3, Direction.UP));
            priorityMoves.Add(new Move(4, Direction.UP));
            priorityMoves.Add(new Move(3, Direction.LEFT));
            priorityMoves.Add(new Move(4, Direction.LEFT));
            priorityMoves.Add(new Move(3, Direction.RIGHT));
            priorityMoves.Add(new Move(4, Direction.RIGHT));

            priorityMoves.Add(new Move(2, Direction.DOWN));
            priorityMoves.Add(new Move(5, Direction.DOWN));
            priorityMoves.Add(new Move(2, Direction.UP));
            priorityMoves.Add(new Move(5, Direction.UP));
            priorityMoves.Add(new Move(2, Direction.LEFT));
            priorityMoves.Add(new Move(5, Direction.LEFT));
            priorityMoves.Add(new Move(2, Direction.RIGHT));
            priorityMoves.Add(new Move(5, Direction.RIGHT));

            priorityMoves.Add(new Move(1, Direction.DOWN));
            priorityMoves.Add(new Move(6, Direction.DOWN));
            priorityMoves.Add(new Move(1, Direction.UP));
            priorityMoves.Add(new Move(6, Direction.UP));
            priorityMoves.Add(new Move(1, Direction.LEFT));
            priorityMoves.Add(new Move(6, Direction.LEFT));
            priorityMoves.Add(new Move(1, Direction.RIGHT));
            priorityMoves.Add(new Move(6, Direction.RIGHT));

            foreach (var mo in priorityMoves)
            {
                if (goodMoves.ContainsKey(mo)) {
                    Debug.Log("MORE THAN 1 GOOD MOVE, RETURN THE BEST: col: " + mo.position.column + " row: " + mo.position.row);
                    return mo;
                }
            }

            move = GetRandomMove(start.GetAvailableMoves());

            Debug.Log("return default move: col: " + move.position.column + " row: " + move.position.row);
            return move;
        }
    }

    public struct AIGameState
    {
        public int player;
        public int depth;
        public int isWinForPlayer;
        public bool isEvaluated;
        public GameBoard gameBoard;
        public TokenBoard tokenBoard;

		public AIGameState(string defaultval)
		{
			depth = 0;
			isWinForPlayer = 0;
			player = 0;
			isEvaluated = false;
			tokenBoard = new TokenBoard ("EMPTY");
			gameBoard = new GameBoard (true);

			//Debug.Log ("DEFAULT CONSTRUCTOR!!");
			//Debug.Log ("size=" + gameBoard.board.Length);
			//gameBoard.PrintBoard ();


		}

        //TO DO: Need to think about pointers for pieces and token arrays.
        public AIGameState(GameBoard gameBoard, TokenBoard tokenBoard, int player)
        {
            depth = 0;
            isWinForPlayer = 0;
            //player = 0;
            isEvaluated = false;
            this.tokenBoard = tokenBoard;
            this.player = player;
            this.gameBoard = gameBoard;
        }



//		public string GameStateId { get { 
//				string id =  gameBoard.PrintBoard () + ":" + tokenBoard.ToString () + ":"+ player;
//			} }


		public AIGameState Copy()
		{
			//AIGameState gs = new AIGameState (this.gameBoard, this.tokenBoard, this.player);
		
			AIGameState gs = new AIGameState ("TEST");

			//Debug.Log ("COPY NEW OBJECT");
			//gs.gameBoard.PrintBoard ();
			//Debug.Log ("COPY EXISTING");
			//this.gameBoard.PrintBoard ();

			gs.player = player;

			gs.gameBoard.InitGameBoard ();
			for (int i = 0; i < gameBoard.board.Length; i++) {
				gs.gameBoard.board [i] = gameBoard.board [i];
			}

			for(int col = 0; col < Constants.numColumns; col++)
			{
				for(int row = 0; row < Constants.numRows; row++)
				{
					gs.tokenBoard.tokens [col, row] = tokenBoard.tokens [col, row];
				}
			}


			return gs;
		}

        //not sure which of these to use: List or Dictionary.
        // don't want to recalculate all of these.

        public List<Move> GetAvailableMoves()
        {
            List<Move> moves = new List<Move>();
            //gameBoard.PrintBoard();
            foreach (Move m in AiPlayer.PotentialMoves ()) {

                if (tokenBoard.CanMove(gameBoard, new Move(gameBoard.GetNextPosition(m), m.direction))) {
                    moves.Add(m);
                }
            }

            return moves;
        }

        //        public Dictionary<Move, Position> GetAllMoves()
        //        {
        //            Dictionary<Move, Position> moves = new Dictionary<Move, Position>();
        //
        //            foreach (Move m in AiPlayer.PotentialMoves ()) {
        //                Position pos = GetEndPositionOfMove (m);
        //                if (GetEndPositionOfMove != null) {
        //                    moves.Add (m,pos);
        //                }
        //            }
        //
        //            return moves;
        //        }

        public AIGameState MakeMove(Move move) {
            //gameBoard.PrintBoard("BEGIN MAKE MOVE: COLUMNS: " + move.position.column + " ROW: " + move.position.row);
            MovingGamePiece activeMovingPiece = new MovingGamePiece(move);
            gameBoard.activeMovingPieces.Add(activeMovingPiece);

            Position movePosition = activeMovingPiece.GetNextPosition();
            //Debug.Log("MOVE POSITION!!!!!!: COL: " + movePosition.column + ", ROW:" + movePosition.row);
            //Debug.Log("movePosition.column: " + movePosition.column);
            //Debug.Log("movePosition.row: " + movePosition.row);


            gameBoard.board[movePosition.column * Constants.numColumns + movePosition.row] = player;

            tokenBoard.tokens[movePosition.column, movePosition.row].UpdateBoard(gameBoard, false);    

            while (gameBoard.activeMovingPieces.Count > 0) {
                Position endPosition = gameBoard.activeMovingPieces[0].GetNextPosition();

                if (tokenBoard.CanMove(gameBoard, new Move(endPosition, move.direction))) {
                    tokenBoard.tokens[endPosition.column, endPosition.row].UpdateBoard(gameBoard, true);
                } else {
                    gameBoard.DisableNextMovingPiece();
                }
            }
            //gameBoard.PrintBoard();
            tokenBoard.UpdateMoveablePieces(gameBoard);

            gameBoard.completedMovingPieces.Clear();
            //gameBoard.PrintBoard("END MAKE MOVE: COLUMNS: " + move.position.column + " ROW: " + move.position.row);
            isWinForPlayer = IsEndGameState();
//            if (isWinForPlayer != 0) {
//                Debug.Log("isWinForPlayer: " + isWinForPlayer);    
//            }

            return this;
        }

        public int IsEndGameState()
        {
            //this function will not work with walls.
            // returns 0 if there is no win
            // returns 1 if win for player 1
            // returns 2 if win for player 2
            // returns 3 if game is a draw

            int count = 0;
            int currentPiece = 0;

            //wins[0] isn't used. Starting at 1 to match player id.
            bool[] wins = { false, false, false };

            // check for horizontal win
            for (int r = 0; r < Constants.numRows; r++) {
				count = 0;
                for (int c = 0; c < Constants.numColumns; c++)
                {
                    int evalPiece = gameBoard.board[c * Constants.numColumns + r];
					if (evalPiece == 0) {
						count = 0;
						currentPiece = 0;
						continue;
					}
                    if (currentPiece == 0)
                    {
                        currentPiece = evalPiece;
                        count = 1;
                    } else {
                        if (currentPiece == evalPiece)
                        {
                            count++;
							if (count > 2) {
                                Debug.Log("count horizontal: " + count + " for " + currentPiece + " at row: " + r + " col: " + c);
							}

							if (count >= Constants.numPiecesToWin)
                            {
                                Debug.Log("HORIZONTAL WIN FOUND FOR PIECE: " + currentPiece);
                                wins[currentPiece] = true;
                            }
                        } else {
                            currentPiece = evalPiece;
                            count = 1;
                        }
                    }
                }
                currentPiece = 0;
            }

            count = 0;
            //gameBoard.PrintBoard();
            // check for vertical win
            for (int c = 0; c < Constants.numColumns; c++) {
				count = 0;
                for (int r = 0; r < Constants.numRows; r++)
                {
                    int evalPiece = gameBoard.board[c * Constants.numColumns + r];

					if (evalPiece == 0) {
						count = 0;
						currentPiece = 0;
						continue;
					}

                    if (currentPiece == 0)
                    {
                        currentPiece = evalPiece;
                        count = 1;
                    } else {
                        if (currentPiece == evalPiece)
                        {
                            count++;
                            if (count > 2) {
                                Debug.Log("count vertical: " + count + " for " + currentPiece + " at row: " + r + " col: " + c);
                            }

                            if (count >= Constants.numPiecesToWin)
                            {
                                Debug.Log("VERTICAL WIN FOUND FOR PIECE: " + currentPiece);
                                wins[currentPiece] = true;
                            }
                        } else {
                            currentPiece = evalPiece;
                            count = 1;
                        }
                    }
                }
                currentPiece = 0;
            }

            // NEED TO IMPLEMENT DIAGONAL DETECTION!!
            // Go left high and then right high



            //draw state if win found for both players
            if (wins[1] && wins[2]) {
                Debug.Log("FOUND DRAW");
                return 3;
            }
                

            if (wins[1]) {
                Debug.Log("PLAYER 1 WINS");
                return 1;
            }
                

            if (wins[2]) {
                Debug.Log("PLAYER 2 WINS");
                return 2;                
            }


            return 0;
        }

//        public Position GetEndPositionOfMove(Move m)
//        {
//            MovingGamePiece piece = new MovingGamePiece(m.position, m.direction);
//            do {
//
//                if (CanMoveInPosition (new Position (0, 0), piece.GetNextPosition (),piece.currentDirection)) {
//                    canMove = true;
//                }
//
//            } while (!CanMove);
//            return piece.position;
//        }

        public bool CanMoveInPosition(GameObject[,] pieces, IToken[,] tokens, Position endPosition, Direction direction)
        {
            // if the next end position is outside of the board then return false;
            if (endPosition.column >= Constants.numColumns || endPosition.column < 0 || endPosition.row >= Constants.numRows || endPosition.row < 0) {
                //Debug.Log("OUTSIDE OF BOARD");
                return false;
            }

            // check for piece at end position if there is a piece and the piece is not moveable then return false
            if (pieces[endPosition.column, endPosition.row]) {
                GameObject pieceObject = pieces[endPosition.column, endPosition.row];
                GamePiece gamePiece = pieceObject.GetComponent<GamePiece>();

                switch (direction)
                {
                    case Direction.UP:
                        if (!gamePiece.isMoveableUp) {
                            return false;
                        }
                        break;
                    case Direction.DOWN:
                        if (!gamePiece.isMoveableDown) {
                            return false;
                        }
                        break;
                    case Direction.LEFT:
                        if (!gamePiece.isMoveableLeft) {
                            return false;
                        }
                        break;
                    case Direction.RIGHT:
                        if (!gamePiece.isMoveableRight) {
                            return false;
                        }
                        break;
                    default:
                        break;
                }
                //Debug.Log("endPosition:col: " + endPosition.column + " row: " + endPosition.row);
                //Debug.Log("next Position:col: " + gamePiece.GetNextPosition(direction).column + " row: " + gamePiece.GetNextPosition(direction).row);
                return CanMoveInPosition(pieces, tokens, gamePiece.GetNextPosition(direction), direction);
            }

            // if there is a token at the end position and canPassThrough is true then true
            if (!tokens[endPosition.column, endPosition.row].canPassThrough) {
                Debug.Log("CANT PASS THROUGH");
                return false;
            }

            return true;
        }

    }
}  