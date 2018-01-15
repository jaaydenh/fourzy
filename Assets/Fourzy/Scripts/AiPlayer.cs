using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Fourzy {
    
    public class AiPlayer {

        AIPlayerSkill skill; 

        public AiPlayer(AIPlayerSkill skill) {
            this.skill = skill;
        }

        public IEnumerator MakeMove(Move playerMove)
        {
            if (skill == AIPlayerSkill.LEVEL1) {
                MakeMoveLevel1(playerMove);
            } else if (skill == AIPlayerSkill.LEVEL2) {
                MakeMoveLevel2(playerMove);
            }

            yield return null;
        }

        public void MakeMoveLevel1(Move playermove)
        {
            GameState gameState = GameManager.instance.gameState;

            List<Move> possibleMoves = gameState.GetPossibleMoves();

            Debug.Log(string.Format("There are {0} Possible Moves", possibleMoves.Count));

            Dictionary<Move, GameState> moveInfo = new Dictionary<Move, GameState>();
            Debug.Log(string.Format("testing before loop"));

            bool found_move = false;
            foreach (Move AiMove in possibleMoves)
            {
                Debug.Log("test1");
                Debug.Log(string.Format("Looking at a move {0}:{1}", AiMove.position.column, AiMove.position.row));

                bool AddMoveToList = true;
                GameState testGameState = gameState.Clone();
                testGameState.MovePiece(AiMove, false);
                Debug.Log(string.Format("WINNING MOVE?{0}:{1}", testGameState.isGameOver, testGameState.winner));
                //Can I make a Move that Wins?

                if (testGameState.isGameOver && testGameState.winner == PlayerEnum.TWO)
                {
                    Debug.Log(string.Format("MAKING THIS MOVE!!{0}:{1}", testGameState.isGameOver, testGameState.winner));
                    found_move = true;
                    GameManager.instance.CallMovePiece(AiMove, false, true);
                    //StartCoroutine(MovePiece(AiMove, false, updatePlayer));
                    break;
                }
                else
                {
                    List<Move> PlayerPossibleMoves = testGameState.GetPossibleMoves();
                    // Can My Opponent Make a Move that Wins?
                    foreach (Move pm1 in PlayerPossibleMoves)
                    {
                        GameState pm1TestGameState = testGameState.Clone();
                        pm1TestGameState.MovePiece(pm1, false);

                        if (pm1TestGameState.isGameOver && pm1TestGameState.winner == PlayerEnum.ONE)
                        {

                            AddMoveToList = false;
                            break;
                        }

                    }
                    if (AddMoveToList == true)
                    {
                        moveInfo.Add(AiMove, testGameState);
                    }
                }
            }
            if (!found_move)
            {
                // Make a Random move among the moves left.
                if (moveInfo.Count > 0)
                {
                    Debug.Log(string.Format("make a random move."));

                    Dictionary<Move, int> WeightedMove = new Dictionary<Move, int>();
                    int total_weight = 0;
                    foreach (Move x in moveInfo.Keys)
                    {

                        switch (x.direction)
                        {
                            case Direction.DOWN:
                            case Direction.UP:
                                if (new[] { 1, 6 }.Contains(x.position.column))
                                {
                                    WeightedMove.Add(x, 2);
                                    total_weight += 2;
                                }
                                else if (new[] { 2, 5 }.Contains(x.position.column))
                                {
                                    WeightedMove.Add(x, 16);
                                    total_weight += 16;
                                }
                                else if (new[] { 3, 4 }.Contains(x.position.column))
                                {
                                    WeightedMove.Add(x, 30);
                                    total_weight += 30;
                                }
                                break;
                            case Direction.LEFT:
                            case Direction.RIGHT:
                                if (new[] { 1, 6 }.Contains(x.position.row))
                                {
                                    WeightedMove.Add(x, 2);
                                    total_weight += 2;
                                }
                                else if (new[] { 2, 5 }.Contains(x.position.row))
                                {
                                    WeightedMove.Add(x, 16);
                                    total_weight += 16;
                                }
                                else if (new[] { 3, 4 }.Contains(x.position.row))
                                {
                                    WeightedMove.Add(x, 30);
                                    total_weight += 30;
                                }
                                break;
                        }
                    }

                    System.Random rnd = new System.Random();
                    int random_move_weight = rnd.Next(0, total_weight);

                    Debug.Log(string.Format("making random move: {0}:{1}", total_weight, random_move_weight));
                    foreach (Move nm in WeightedMove.Keys)
                    {
                        random_move_weight -= WeightedMove[nm];
                        if (random_move_weight <= 0)
                        {
                            GameManager.instance.CallMovePiece(nm, false, true);
                            //StartCoroutine(MovePiece(nm, false, updatePlayer));
                            break;
                        }
                    }

                    //Simple Random Move
                    //System.Random rnd = new System.Random();
                    //int move_id = rnd.Next(0, moveInfo.Count);
                    //Move TheAIMove = moveInfo.ElementAt(move_id).Key;

                    //StartCoroutine(MovePiece(TheAIMove, false, updatePlayer));
                }

                else
                {
                    Debug.Log(string.Format("no good moves. make any move."));

                    //No good moves.
                    //Make a move from any of the possible moves.
                    System.Random rnd = new System.Random();
                    int move_id = rnd.Next(0, moveInfo.Count);
                    Move TheAIMove = possibleMoves.ElementAt(move_id);
                    GameManager.instance.CallMovePiece(TheAIMove, false, true);
                }
            }
        }

        public void MakeMoveLevel2(Move playerMove)
        {
            GameState gameState = GameManager.instance.gameState;

            List<Move> possibleMoves = gameState.GetPossibleMoves();
            Debug.Log(string.Format("There are {0} Possible Moves", possibleMoves.Count));

            Dictionary<Move, int> WeightedMove = new Dictionary<Move, int>();

            Move AiMove = null;
            foreach (Move x in possibleMoves)
            {
                Debug.Log(string.Format("PossibleMove:{0}:{1},{2}", x.direction, x.position.row, x.position.column));
                //Can I make a Move that Wins?
                GameState testGameState = gameState.Clone();
                testGameState.MovePiece(x, false);
                if (testGameState.isGameOver && testGameState.winner == PlayerEnum.TWO)
                {
                    AiMove = x;
                    Debug.Log(string.Format("MAKING THIS MOVE!!{0}:{1}", testGameState.isGameOver, testGameState.winner));
                    GameManager.instance.CallMovePiece(AiMove, false, true);
                    //StartCoroutine(MovePiece(AiMove, false, updatePlayer));
                    break;
                }

                int match_bonus = 0;
                if (playerMove.direction == x.direction && playerMove.position == x.position) { match_bonus += 10; }

                switch (x.direction)
                {
                    case Direction.DOWN:
                    case Direction.UP:
                        if (new[] { 1, 6 }.Contains(x.position.column))
                        {
                            WeightedMove.Add(x, 2 + match_bonus);
                        }
                        else if (new[] { 2, 5 }.Contains(x.position.column))
                        {
                            WeightedMove.Add(x, 16 + match_bonus);
                        }
                        else if (new[] { 3, 4 }.Contains(x.position.column))
                        {
                            WeightedMove.Add(x, 30 + match_bonus);
                        }
                        break;
                    case Direction.LEFT:
                    case Direction.RIGHT:
                        if (new[] { 1, 6 }.Contains(x.position.row))
                        {
                            WeightedMove.Add(x, 2 + match_bonus);
                        }
                        else if (new[] { 2, 5 }.Contains(x.position.row))
                        {
                            WeightedMove.Add(x, 16 + match_bonus);
                        }
                        else if (new[] { 3, 4 }.Contains(x.position.row))
                        {
                            WeightedMove.Add(x, 30 + match_bonus);

                        }
                        break;
                }
            }

            Move BestMove = new Move(new Position(0, 0), Direction.NONE, 0);
            int BestMoveCount = 0;
            //LOOP THROUGH A WEIGHTED LIST OF MOVES.
            //  CHECK EACH MOVE AND IF OK, MAKE IT.
            if (AiMove == null)
            {
                while (WeightedMove.Count > 0)
                {
                    //get a random move.
                    int total_weight = 0;
                    foreach (Move wm in WeightedMove.Keys)
                    {
                        total_weight += WeightedMove[wm];
                    }

                    System.Random rnd = new System.Random();
                    int random_move_weight = rnd.Next(0, total_weight);

                    foreach (Move nm in WeightedMove.Keys)
                    {
                        random_move_weight -= WeightedMove[nm];
                        if (random_move_weight <= 0)
                        {
                            AiMove = nm;
                            WeightedMove.Remove(nm);
                            break;
                        }
                    }

                    Debug.Log(string.Format("Looking at a move {0}:{1}", AiMove.position.column, AiMove.position.row));

                    bool MakeThisMove = false;
                    GameState testGameState = gameState.Clone();
                    testGameState.MovePiece(AiMove, false);

                    List<Move> PlayerPossibleMoves = testGameState.GetPossibleMoves();
                    // Can My Opponent Make a Move that Wins?

                    int player_win_count = 0;
                    int move_ok_count = 0;
                    foreach (Move pm1 in PlayerPossibleMoves)
                    {
                        GameState pm1TestGameState = testGameState.Clone();
                        pm1TestGameState.MovePiece(pm1, false);

                        //If I make this move, the player can win. 
                        // Don't make this move.
                        if (pm1TestGameState.isGameOver && pm1TestGameState.winner == PlayerEnum.ONE)
                        {
                            Debug.Log("Don't Make Move.  Opponent Can Win In One Move!");
                            break;
                        }

                        //if this case is true, opponent can force me to win on his turn.
                        //   this is only good if all moves like this.
                        if (pm1TestGameState.isGameOver && pm1TestGameState.winner == PlayerEnum.TWO)
                        {
                            player_win_count++;
                            move_ok_count++;

                            if (player_win_count == PlayerPossibleMoves.Count)
                            {
                                Debug.Log(string.Format("MAKING THIS MOVE!!{0}:{1}", testGameState.isGameOver, testGameState.winner));
                                GameManager.instance.CallMovePiece(AiMove, false, true);
                                //StartCoroutine(MovePiece(AiMove, false, updatePlayer));
                                break;
                            }
                            continue;
                        }

                        //For each player move can I make a move that wins?
                        //  or is at least ok?
                        bool found_ok_move = false;
                        List<Move> PlayerMoves = pm1TestGameState.GetPossibleMoves();
                        foreach (Move aim2 in PlayerMoves)
                        {
                            GameState Aim2TestGameState = pm1TestGameState.Clone();
                            Aim2TestGameState.MovePiece(aim2, false);

                            if (!Aim2TestGameState.isGameOver)
                            {

                                //check if I make this move if a player can respond and win in the second move.
                                bool player_can_win = false;
                                foreach (Move Pm2 in Aim2TestGameState.GetPossibleMoves())
                                {
                                    GameState Pm2TestGameState = Aim2TestGameState.Clone();
                                    Pm2TestGameState.MovePiece(Pm2, false);

                                    if (Pm2TestGameState.isGameOver && Pm2TestGameState.winner == PlayerEnum.ONE)
                                    {
                                        player_can_win = true;
                                        break;
                                    }
                                }

                                if (!player_can_win)
                                {
                                    found_ok_move = true;
                                }
                            }
                            else
                            {
                                if (Aim2TestGameState.winner == PlayerEnum.TWO)
                                {
                                    found_ok_move = true;
                                }
                            }
                        }
                        Debug.Log(string.Format("ok?:{0}:{1}/{2}", found_ok_move, move_ok_count, PlayerMoves.Count));
                        if (found_ok_move)
                        {
                            move_ok_count++;
                            //ok. if we've looked through all moves and it's still ok, make this move.

                            if (move_ok_count > BestMoveCount && AiMove != null)
                            {
                                Debug.Log(string.Format("New Best Move:{0}:{1},{2}", AiMove.direction, AiMove.position.row, AiMove.position.column));

                                BestMove = new Move(AiMove.position, AiMove.direction, AiMove.player);
                                BestMoveCount = move_ok_count;
                            }

                            if (move_ok_count >= PlayerMoves.Count)
                            {
                                MakeThisMove = true;
                                break;
                            }
                        }
                    }

                    if (MakeThisMove)
                    {
                        Debug.Log(string.Format("Checked the Move and I'm making it."));
                        GameManager.instance.CallMovePiece(AiMove, false, true);
                        //StartCoroutine(MovePiece(AiMove, false, updatePlayer));
                        break;
                    }
                    else
                    {
                        AiMove = null;
                    }
                }

                // If we get to this block without making a move.  We have a problem...

                if (AiMove == null)
                {
                    Debug.Log(string.Format("No Good Moves Just doing anything for now."));
                    if (BestMoveCount > 0)
                    {
                        GameManager.instance.CallMovePiece(BestMove, false, true);
                        //StartCoroutine(MovePiece(BestMove, false, updatePlayer));
                    }
                    else
                    {
                        GameManager.instance.CallMovePiece(gameState.GetPossibleMoves().First(), false, true);
                        //StartCoroutine(MovePiece(gameState.GetPossibleMoves().First(), false, updatePlayer));
                    }
                }
            }
        }
    }
}  