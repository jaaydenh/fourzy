using System.Collections.Generic;
using System.Linq;
using GameSparks.Api.Responses;
using GameSparks.Api.Messages;
using GameSparks.Core;

namespace Fourzy
{
    public class GameSparksChallenge {

        public int currentPlayerMove;
        public bool isPlayerOneTurn;
        public bool isGameOver;
        public int[] previousGameboardData;
        public TokenBoard tokenBoard;
        public List<GSData> moveList;

        public GameSparksChallenge(ChallengeTurnTakenMessage._Challenge challenge) {
            currentPlayerMove = challenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
            isPlayerOneTurn = currentPlayerMove == (int)Piece.BLUE ? true : false;
            if (challenge.State == "COMPLETE") {
                isGameOver = true;
            } else {
                isGameOver = false;
            }

            List<int> lastBoardData = challenge.ScriptData.GetIntList("lastGameBoard");
            if (lastBoardData != null) {
                previousGameboardData = challenge.ScriptData.GetIntList("lastGameBoard").ToArray();
            } else {
                previousGameboardData = new int[64];
            }

            string tokenBoardId = challenge.ScriptData.GetString("tokenBoardId");
            string tokenBoardName = challenge.ScriptData.GetString("tokenBoardName");
            List<int> tokenData = challenge.ScriptData.GetIntList("tokenBoard");
            if (tokenData != null) {
                int[] tokenBoardData = challenge.ScriptData.GetIntList("tokenBoard").ToArray();
                tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, true);
            } else {
                int[] tokenBoardData = Enumerable.Repeat(0, 64).ToArray();
                tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, true);
            }

            moveList = challenge.ScriptData.GetGSDataList("moveList");
        }

        public GameSparksChallenge(ListChallengeResponse._Challenge challenge) {
            currentPlayerMove = challenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
            isPlayerOneTurn = currentPlayerMove == (int)Piece.BLUE ? true : false;
            if (challenge.State == "COMPLETE") {
                isGameOver = true;
            } else {
                isGameOver = false;
            }

            List<int> lastBoardData = challenge.ScriptData.GetIntList("lastGameBoard");
            if (lastBoardData != null) {
                previousGameboardData = challenge.ScriptData.GetIntList("lastGameBoard").ToArray();
            } else {
                previousGameboardData = new int[64];
            }

            string tokenBoardId = challenge.ScriptData.GetString("tokenBoardId");
            string tokenBoardName = challenge.ScriptData.GetString("tokenBoardName");
            List<int> tokenData = challenge.ScriptData.GetIntList("tokenBoard");
            if (tokenData != null) {
                int[] tokenBoardData = challenge.ScriptData.GetIntList("tokenBoard").ToArray();
                tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, true);
            } else {
                int[] tokenBoardData = Enumerable.Repeat(0, 64).ToArray();
                tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, true);
            }

            moveList = challenge.ScriptData.GetGSDataList("moveList");
        }
    }
}
