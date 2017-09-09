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
        public Move GetMove(GameState gameState, int player)  {
            //Debug.Log("GET MOVE");
            Move move;
            Dictionary<Move, AIGameState> goodMoves = new Dictionary<Move, AIGameState>();

            AIGameState start = new AIGameState(gameState, player);
             
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
        public GameState gameState;
        public TokenBoard tokenBoard;

        public AIGameState(string defaultval)
        {
            depth = 0;
            isWinForPlayer = 0;
            player = 0;
            isEvaluated = false;
            tokenBoard = TokenBoardLoader.instance.GetTokenBoard();
            gameBoard = new GameBoard (Constants.numRows, Constants.numColumns, Constants.numPiecesToWin);
            gameState = new GameState(Constants.numRows, Constants.numColumns, true, true, tokenBoard, null, false);
        }

        //TO DO: Need to think about pointers for pieces and token arrays.
        public AIGameState(GameState gameState, int player)
        {
            depth = 0;
            isWinForPlayer = 0;
            //player = 0;
            isEvaluated = false;
            this.tokenBoard = gameState.tokenBoard;
            this.player = player;
            this.gameBoard = gameState.gameBoard;
            this.gameState = gameState;
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

            gs.gameBoard = gameBoard.Clone();

			//for (int i = 0; i < gameBoard.board.Length; i++) {
			//	gs.gameBoard.board [i] = gameBoard.board [i];
			//}

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

                if (gameState.CanMove(new Move(Utility.GetNextPosition(m), m.direction, m.player), tokenBoard.tokens)) {
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
            MovingGamePiece activeMovingPiece = new MovingGamePiece(move);
            gameBoard.activeMovingPieces.Add(activeMovingPiece);

            Position movePosition = activeMovingPiece.GetNextPosition();

            gameBoard.SetCell(movePosition.column, movePosition.row, (Player)player);

            tokenBoard.tokens[movePosition.column, movePosition.row].UpdateBoard(gameBoard, false);    

            while (gameBoard.activeMovingPieces.Count > 0) {
                Position endPosition = gameBoard.activeMovingPieces[0].GetNextPosition();

                if (gameState.CanMove(new Move(endPosition, move.direction, move.player), tokenBoard.tokens)) {
                    tokenBoard.tokens[endPosition.column, endPosition.row].UpdateBoard(gameBoard, true);
                } else {
                    gameBoard.DisableNextMovingPiece();
                }
            }

            gameState.UpdateMoveablePieces();
            gameBoard.completedMovingPieces.Clear();
            //isWinForPlayer = IsEndGameState();

            return this;
        }
    }
}  