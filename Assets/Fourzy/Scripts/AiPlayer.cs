using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Fourzy
{

    public enum AiMoveType
    {
        Winning,
        Losing,
        Unknown
    }

    public class AiMove
    {
        public const int AI_SCORE_WINNING_MOVE = 10000;


        public Move move = null;
        public int score = 0;
        public AiMoveType movetype = AiMoveType.Unknown;

        // count the number of potential wins?
        //        int four_count = 0;

        public AiMove(Move Move, int Score, AiMoveType MoveType)
        {
            this.move = Move;
            this.score = Score;
            this.movetype = MoveType;
        }

    }

    public class AiPlayer
    {

        public const int AI_LOOKAHEAD_DEPTH = 3;
        public static int move_review_count = 0;

        AIPlayerSkill skill;
        PlayerEnum AiPlayerId = PlayerEnum.TWO;
        public PlayerEnum PlayerId
        {
            get
            {
                if (AiPlayerId == PlayerEnum.ONE)
                    return PlayerEnum.TWO;
                else
                    return PlayerEnum.ONE;
            }
        }

        public AiPlayer(AIPlayerSkill skill)
        {
            this.skill = skill;
        }

        public IEnumerator MakeMove(Move playerMove)
        {
            if (skill == AIPlayerSkill.LEVEL1)
            {
                MakeMoveLevel1(playerMove);
            }
            else if (skill == AIPlayerSkill.LEVEL2)
            {
                MakeMoveLevel2(playerMove);
            }
            else if (skill == AIPlayerSkill.LEVEL3)
            {
                MakeMoveLevel3(playerMove);
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
                //Debug.Log("test1");
                //Debug.Log(string.Format("Looking at a move {0}:{1}", AiMove.position.column, AiMove.position.row));

                bool AddMoveToList = true;
                GameState testGameState = gameState.Clone();
                testGameState.MovePiece(AiMove, false);
                //Debug.Log(string.Format("WINNING MOVE?{0}:{1}", testGameState.isGameOver, testGameState.winner));
                //Can I make a Move that Wins?

                if (testGameState.isGameOver && testGameState.winner == this.AiPlayerId)
                {
                    //Debug.Log(string.Format("MAKING THIS MOVE!!{0}:{1}", testGameState.isGameOver, testGameState.winner));
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
                if (testGameState.isGameOver && testGameState.winner == this.AiPlayerId)
                {
                    AiMove = x;
                    Debug.Log(string.Format("MAKING THIS MOVE!!{0}:{1}", testGameState.isGameOver, testGameState.winner));
                    GameManager.instance.CallMovePiece(AiMove, false, true);
                    //StartCoroutine(MovePiece(AiMove, false, updatePlayer));
                    break;
                }

                int match_bonus = 0;
                if (playerMove.direction == x.direction && playerMove.position == x.position) { match_bonus += 20; }
                //TODO:  Add a bonus if the move matches the previous player move.

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
                        if (pm1TestGameState.isGameOver && pm1TestGameState.winner == this.AiPlayerId)
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
                                if (Aim2TestGameState.winner == this.AiPlayerId)
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

        public void MakeMoveLevel3(Move playerMove)
        {

            GameState gameState = GameManager.instance.gameState;
            int player = gameState.isPlayerOneTurn ? 1 : 2;
            int opponent = gameState.isPlayerOneTurn ? 2 : 1;


            List<Move> possibleMoves = gameState.GetPossibleMoves();
            Debug.Log(string.Format("There are {0} Possible Moves", possibleMoves.Count));

            if (gameState.moveList.Count < 3)
            {
                List<Move> PossibleMoves = gameState.GetPossibleMoves();

                List<Move> AiMoves = new List<Move>();
                foreach (Move m in PossibleMoves)
                {
                    if (playerMove.direction == m.direction && playerMove.position.Equals(m.position))
                    {
                        AiMoves.Add(m);
                        continue;
                    }

                    //there will be certain boards where doing what the player did won't work...
                    switch (m.direction)
                    {
                        case Direction.DOWN:
                        case Direction.UP:
                            if (new[] { 3, 4 }.Contains(m.position.column))
                            {
                                AiMoves.Add(m);
                            }
                            break;
                        case Direction.LEFT:
                        case Direction.RIGHT:
                            if (new[] { 3, 4 }.Contains(m.position.column))
                            {
                                AiMoves.Add(m);
                            }
                            break;
                    }
                }
                System.Random rnd = new System.Random();
                int random_move_id = rnd.Next(0, AiMoves.Count - 1);

                GameManager.instance.CallMovePiece(AiMoves[random_move_id], false, true);
                return;
            }

            Dictionary<Move, int> WeightedMove = new Dictionary<Move, int>();

            Move AiMove = null;
            foreach (Move x in possibleMoves)
            {
                Debug.Log(string.Format("PossibleMove:{0}:{1},{2}", x.direction, x.position.row, x.position.column));
                //Can I make a Move that Wins?
                GameState testGameState = gameState.Clone();
                testGameState.MovePiece(x, false);
                if (testGameState.isGameOver && testGameState.winner == this.AiPlayerId)
                {
                    AiMove = x;
                    Debug.Log(string.Format("MAKING THIS MOVE!!{0}:{1}", testGameState.isGameOver, testGameState.winner));
                    GameManager.instance.CallMovePiece(AiMove, false, true);
                    break;
                }

                int score = testGameState.EstimateScore(player);
                score = score * 1000;
                if (playerMove.direction == x.direction && playerMove.position == x.position) { score += 200; }
                WeightedMove.Add(x, score);

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
                        if (pm1TestGameState.isGameOver && pm1TestGameState.winner == this.AiPlayerId)
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
                                if (Aim2TestGameState.winner == this.AiPlayerId)
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

        public void MakeMoveLevel3Sorted(Move playerMove)
        {

            GameState gameState = GameManager.instance.gameState;
            int player = gameState.isPlayerOneTurn ? 1 : 2;
            int opponent = gameState.isPlayerOneTurn ? 2 : 1;


            List<Move> possibleMoves = gameState.GetPossibleMoves();
            Debug.Log(string.Format("There are {0} Possible Moves", possibleMoves.Count));

            if (gameState.moveList.Count < 3)
            {
                List<Move> PossibleMoves = gameState.GetPossibleMoves();

                List<Move> AiMoves = new List<Move>();
                foreach (Move m in PossibleMoves)
                {
                    if (playerMove.direction == m.direction && playerMove.position.Equals(m.position))
                    {
                        AiMoves.Add(m);
                        continue;
                    }

                    //there will be certain boards where doing what the player did won't work...
                    switch (m.direction)
                    {
                        case Direction.DOWN:
                        case Direction.UP:
                            if (new[] { 3, 4 }.Contains(m.position.column))
                            {
                                AiMoves.Add(m);
                            }
                            break;
                        case Direction.LEFT:
                        case Direction.RIGHT:
                            if (new[] { 3, 4 }.Contains(m.position.column))
                            {
                                AiMoves.Add(m);
                            }
                            break;
                    }
                }
                System.Random rnd = new System.Random();
                int random_move_id = rnd.Next(0, AiMoves.Count - 1);

                GameManager.instance.CallMovePiece(AiMoves[random_move_id], false, true);
                return;
            }

            Dictionary<Move, int> WeightedMove = new Dictionary<Move, int>();

            Move AiMove = null;
            foreach (Move x in possibleMoves)
            {
                Debug.Log(string.Format("PossibleMove:{0}:{1},{2}", x.direction, x.position.row, x.position.column));
                //Can I make a Move that Wins?
                GameState testGameState = gameState.Clone();
                testGameState.MovePiece(x, false);
                if (testGameState.isGameOver && testGameState.winner == this.AiPlayerId)
                {
                    AiMove = x;
                    Debug.Log(string.Format("MAKING THIS MOVE!!{0}:{1}", testGameState.isGameOver, testGameState.winner));
                    GameManager.instance.CallMovePiece(AiMove, false, true);
                    break;
                }

                int score = testGameState.CountFours(opponent);
                WeightedMove.Add(x, score);

            }

            Move BestMove = new Move(new Position(0, 0), Direction.NONE, 0);
            int BestMoveCount = 0;
            //LOOP THROUGH A WEIGHTED LIST OF MOVES.
            //  CHECK EACH MOVE AND IF OK, MAKE IT.
            if (AiMove == null)
            {
                while (WeightedMove.Count > 0)
                {

                    AiMove = WeightedMove.FirstOrDefault(x => x.Value == WeightedMove.Values.Min()).Key;
                    WeightedMove.Remove(AiMove);

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
                        if (pm1TestGameState.isGameOver && pm1TestGameState.winner == this.AiPlayerId)
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
                                if (Aim2TestGameState.winner == this.AiPlayerId)
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

        public void MakeMoveLevel3Deeper(Move playerMove)
        {

            GameState gameState = GameManager.instance.gameState;
            int player = gameState.isPlayerOneTurn ? 1 : 2;
            int opponent = gameState.isPlayerOneTurn ? 2 : 1;


            List<Move> possibleMoves = gameState.GetPossibleMoves();
            Debug.Log(string.Format("There are {0} Possible Moves", possibleMoves.Count));

            if (gameState.moveList.Count < 3)
            {
                List<Move> PossibleMoves = gameState.GetPossibleMoves();

                List<Move> AiMoves = new List<Move>();
                foreach (Move m in PossibleMoves)
                {
                    if (playerMove.direction == m.direction && playerMove.position.Equals(m.position))
                    {
                        AiMoves.Add(m);
                        continue;
                    }

                    //there will be certain boards where doing what the player did won't work...
                    switch (m.direction)
                    {
                        case Direction.DOWN:
                        case Direction.UP:
                            if (new[] { 3, 4 }.Contains(m.position.column))
                            {
                                AiMoves.Add(m);
                            }
                            break;
                        case Direction.LEFT:
                        case Direction.RIGHT:
                            if (new[] { 3, 4 }.Contains(m.position.column))
                            {
                                AiMoves.Add(m);
                            }
                            break;
                    }
                }
                System.Random rnd = new System.Random();
                int random_move_id = rnd.Next(0, AiMoves.Count - 1);

                GameManager.instance.CallMovePiece(AiMoves[random_move_id], false, true);
                return;
            }

            Dictionary<Move, int> WeightedMove = new Dictionary<Move, int>();

            Move AiMove = null;
            foreach (Move x in possibleMoves)
            {
                Debug.Log(string.Format("PossibleMove:{0}:{1},{2}", x.direction, x.position.row, x.position.column));
                //Can I make a Move that Wins?
                GameState testGameState = gameState.Clone();
                testGameState.MovePiece(x, false);
                if (testGameState.isGameOver && testGameState.winner == this.AiPlayerId)
                {
                    AiMove = x;
                    Debug.Log(string.Format("MAKING THIS MOVE!!{0}:{1}", testGameState.isGameOver, testGameState.winner));
                    GameManager.instance.CallMovePiece(AiMove, false, true);
                    break;
                }

                int score = testGameState.CountFours(opponent);
                WeightedMove.Add(x, score);

            }

            Move BestMove = new Move(new Position(0, 0), Direction.NONE, 0);
            int BestMoveCount = 0;
            //LOOP THROUGH A WEIGHTED LIST OF MOVES.
            //  CHECK EACH MOVE AND IF OK, MAKE IT.
            if (AiMove == null)
            {
                while (WeightedMove.Count > 0)
                {

                    AiMove = WeightedMove.FirstOrDefault(x => x.Value == WeightedMove.Values.Min()).Key;
                    WeightedMove.Remove(AiMove);

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
                        if (pm1TestGameState.isGameOver && (int)pm1TestGameState.winner == player)
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

                                    if (Pm2TestGameState.isGameOver && (int)Pm2TestGameState.winner == opponent)
                                    {
                                        player_can_win = true;
                                        break;
                                    }


                                    List<Move> AILevel3PossibleMoves = Pm2TestGameState.GetPossibleMoves();
                                    bool is_good_level3_path = false;
                                    foreach (Move aim3 in AILevel3PossibleMoves)
                                    {
                                        GameState aim3TestGameState = Pm2TestGameState.Clone();
                                        aim3TestGameState.MovePiece(aim3, false);

                                        //I found a good move. This is a good path.
                                        if (aim3TestGameState.isGameOver && (int)aim3TestGameState.winner == player)
                                        {
                                            is_good_level3_path = true;
                                            break;
                                        }

                                        //I'm not sure.  Is there a way for opponent to win if I make this move?
                                        int count_good_level3_moves = 0;
                                        List<Move> PlayerLevel3PossibleMoves = aim3TestGameState.GetPossibleMoves();
                                        foreach (Move Pm3 in PlayerLevel3PossibleMoves)
                                        {
                                            GameState Pm3TestGameState = aim3TestGameState.Clone();
                                            Pm3TestGameState.MovePiece(Pm3, false);

                                            if (Pm3TestGameState.isGameOver && (int)Pm3TestGameState.winner == opponent)
                                            {
                                                break;
                                            }
                                            count_good_level3_moves++;
                                        }

                                        //Did we find a winning player move??
                                        if (count_good_level3_moves == PlayerLevel3PossibleMoves.Count())
                                        {
                                            is_good_level3_path = true;
                                            break;
                                        }

                                    }
                                    //if all of my third moves are broken, then player can win.
                                    if (!is_good_level3_path)
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
                                if ((int)Aim2TestGameState.winner == player)
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

        public void MakeMoveLevel3Broken(Move playerMove)
        {
            GameState gameState = GameManager.instance.gameState;
            Debug.Log("AIMODE = LEVEL 3");
            //we should be making better some library moves...
            // if it's the first computer move going second, or the first two moves if going first
            //   either make the opposite move if available.
            //   make the same move the player made
            //   or use a center space if available.
            if (gameState.moveList.Count < 3)
            {
                List<Move> PossibleMoves = gameState.GetPossibleMoves();

                List<Move> AiMoves = new List<Move>();
                foreach (Move m in PossibleMoves)
                {
                    if (playerMove.direction == m.direction && playerMove.position.Equals(m.position))
                    {
                        AiMoves.Add(m);
                        continue;
                    }

                    //there will be certain boards where doing what the player did won't work...
                    switch (m.direction)
                    {
                        case Direction.DOWN:
                        case Direction.UP:
                            if (new[] { 3, 4 }.Contains(m.position.column))
                            {
                                AiMoves.Add(m);
                            }
                            break;
                        case Direction.LEFT:
                        case Direction.RIGHT:
                            if (new[] { 3, 4 }.Contains(m.position.column))
                            {
                                AiMoves.Add(m);
                            }
                            break;
                    }
                }
                System.Random rnd = new System.Random();
                int random_move_id = rnd.Next(0, AiMoves.Count - 1);

                GameManager.instance.CallMovePiece(AiMoves[random_move_id], false, true);
            }
            //Do a lookahead to check possibilities.
            else
            {
                move_review_count = 0;
                AiMove BestAiMove = GetBestMove(gameState);
                GameManager.instance.CallMovePiece(BestAiMove.move, false, true);
            }


        }

        private AiMove GetBestMove(GameState testGameState, int CurrentDepth = 1)
        {
            move_review_count++;
            int player = testGameState.isPlayerOneTurn ? 1 : 2;
            int opponent = testGameState.isPlayerOneTurn ? 2 : 1;

            AiMove BestMove = null;
            Debug.Log(string.Format("GetBestMove Depth={0}: playerone?={1}", CurrentDepth, testGameState.isPlayerOneTurn));

            if (testGameState == null) { testGameState = GameManager.instance.gameState; }

            List<Move> possibleMoves = testGameState.GetPossibleMoves();
            Debug.Log(string.Format("There are {0} Possible Moves", possibleMoves.Count));

            Dictionary<Move, GameState> EvalMoves = new Dictionary<Move, GameState>();

            //Step 1: Check for a Winning AI Move.
            //  If I find it, return it with the highest score.


            Debug.Log(string.Format("Check if I can win in this state?"));

            foreach (Move aim in possibleMoves)
            {
                //Can I make a Move that Wins?
                GameState AiMoveGameState = testGameState.Clone();
                AiMoveGameState.MovePiece(aim, false);
                if (AiMoveGameState.isGameOver && (int)AiMoveGameState.winner == player)
                {
                    Debug.Log(string.Format("MAKING THIS MOVE!!{0}:{1}", testGameState.isGameOver, testGameState.winner));
                    BestMove = new AiMove(aim, AiMove.AI_SCORE_WINNING_MOVE, AiMoveType.Winning);
                    return BestMove;
                }

                // potential remove some moves from eval list using heuristics??
                // add other moves to a dictionary to prevent having to reeval gameStates.
                EvalMoves.Add(aim, AiMoveGameState);
            }

            //STEP 2: Check to see if the opponent can win.
            //   If so, then return a bad score.

            Debug.Log(string.Format("Eval Every AI move"));

            foreach (Move aim in EvalMoves.Keys)
            {
                Debug.Log(string.Format("Depth={0}, PossibleMove:{1}:{2},{3}", CurrentDepth, aim.direction, aim.position.row, aim.position.column));

                //SOME HEURISTIC INFO HERE
                //int match_bonus = 0;
                //if (playerMove.direction == x.direction && playerMove.position == x.position) { match_bonus += 20; }
                //TODO:  Add a bonus if the move matches the previous player move.

                //WHAT CAN MY OPPONENT DO FROM THIS GAME STATE?
                //CHECK EACH OF HIS MOVES.

                int force_ai_win_count = 0;
                int player_win_count = 0;
                int move_ok_count = 0;

                List<Move> PlayerPossibleMoves = EvalMoves[aim].GetPossibleMoves();

                Dictionary<Move, GameState> EvalPlayerMoves = new Dictionary<Move, GameState>();

                foreach (Move pm1 in PlayerPossibleMoves)
                {
                    GameState pm1TestGameState = testGameState.Clone();
                    pm1TestGameState.MovePiece(pm1, false);

                    //If I make this move, the player can win. 
                    // Don't make this move.
                    if (pm1TestGameState.isGameOver && (int)pm1TestGameState.winner == opponent)
                    {
                        player_win_count++;
                        Debug.Log("Don't Make Move.  Opponent Can Win In One Move!");
                        break;
                    }

                    //if this case is true, opponent can force me to win on his turn.
                    //   this is only good if all moves like this.
                    if (pm1TestGameState.isGameOver && (int)pm1TestGameState.winner == player)
                    {
                        force_ai_win_count++;
                        move_ok_count++;

                        if (force_ai_win_count == PlayerPossibleMoves.Count)
                        {
                            Debug.Log(string.Format("MAKING THIS MOVE!!{0}:{1}", testGameState.isGameOver, testGameState.winner));
                            break;
                        }
                        continue;
                    }

                    move_ok_count++;
                    EvalPlayerMoves.Add(pm1, pm1TestGameState);
                }

                // if we get to this stage, if we make the move, the player can make a winning move
                //  we'll only want to make this move if it's the best one...
                if (move_ok_count < PlayerPossibleMoves.Count)
                {
                    int eval_score = EvalMoves[aim].EstimateScore((int)AiPlayerId);
                    if (BestMove == null)
                    {
                        Debug.Log(string.Format("New Best Score={0} (losing)", move_ok_count));
                        BestMove = new AiMove(aim, eval_score, AiMoveType.Losing);
                    }
                    else
                    {
                        if (move_ok_count > BestMove.score)
                        {
                            Debug.Log(string.Format("New Best Score={0} (losing)", move_ok_count));
                            BestMove = new AiMove(aim, eval_score, AiMoveType.Losing);
                        }
                    }
                    continue;
                }

                //if all the moves are 'ok', meaning no direct victory, we are in an unknown state.  Do we go deeper?
                //we might want to go another level deeper?
                if (move_ok_count == PlayerPossibleMoves.Count)
                {
                    int eval_score = move_ok_count;
                    //go deeper
                    if (CurrentDepth < AI_LOOKAHEAD_DEPTH)
                    {
                        //look at all player moves and our score is the WORST SCORE.
                        AiMove WorstPosition = null;
                        foreach (Move pm in EvalPlayerMoves.Keys)
                        {
                            AiMove DeeperMove = GetBestMove(EvalPlayerMoves[pm].Clone(), CurrentDepth + 1);
                            if (WorstPosition == null)
                            {
                                WorstPosition = DeeperMove;
                                if (WorstPosition.movetype == AiMoveType.Losing) break;
                            }
                            else
                            {
                                if (DeeperMove.score < WorstPosition.score || (WorstPosition.movetype == AiMoveType.Winning && DeeperMove.movetype != AiMoveType.Winning))
                                {
                                    WorstPosition = DeeperMove;
                                }
                                if (WorstPosition.movetype == AiMoveType.Losing) break;
                            }

                        }
                        if (BestMove == null)
                        {
                            BestMove = new AiMove(aim, WorstPosition.score, WorstPosition.movetype);
                        }
                        else
                        {
                            if (WorstPosition.score > BestMove.score || (BestMove.movetype == AiMoveType.Losing && WorstPosition.movetype != AiMoveType.Losing))
                            {
                                BestMove = new AiMove(aim, WorstPosition.score, WorstPosition.movetype);
                            }
                        }
                        continue;
                    }
                    else
                    {
                        eval_score = EvalMoves[aim].EstimateScore((int)AiPlayerId);
                    }

                    if (BestMove == null)
                    {
                        Debug.Log(string.Format("New Best Score={0}; Move={1},{2}", eval_score, aim.position.column, aim.position.row));

                        BestMove = new AiMove(aim, eval_score, AiMoveType.Unknown);
                    }
                    else
                    {
                        Debug.Log(string.Format("Best Score={0}/{1}; Move={2},{3}", eval_score, BestMove.score, aim.position.column, aim.position.row));

                        if (eval_score > BestMove.score || BestMove.movetype == AiMoveType.Losing) BestMove = new AiMove(aim, eval_score, AiMoveType.Unknown);
                    }
                }
            }

            return BestMove;
        }
    }
}