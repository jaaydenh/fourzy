using System.Collections.Generic;
using System.Linq;
using GameSparks.Api.Responses;
using GameSparks.Api.Messages;
using GameSparks.Core;
using System;
using UnityEngine;

namespace Fourzy
{
    public class GameSparksChallenge {

        public string challengeId;
        public int currentPlayerMove;
        public bool isPlayerOneTurn;
        public bool isGameOver;
        public int[] previousGameboardData;
        public GameType gameType;
        public TokenBoard tokenBoard;
        public List<GSData> moveList;
        public string nextPlayer;
        public string challengerGamePieceId;
        public string challengedGamePieceId;
        public int challengedRatingDelta;
        public int challengerRatingDelta;

        public GameSparksChallenge(ChallengeTurnTakenMessage._Challenge challenge) {
            challengeId = challenge.ChallengeId;
            currentPlayerMove = challenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
            isPlayerOneTurn = currentPlayerMove == (int)Piece.BLUE ? true : false;
            nextPlayer = challenge.NextPlayer;
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
            List<int> tokenData = challenge.ScriptData.GetIntList("lastTokenBoard");
            int[] tokenBoardData;
            if (tokenData != null)
            {
                tokenBoardData = challenge.ScriptData.GetIntList("lastTokenBoard").ToArray();
                tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
            }
            else
            {
                tokenData = challenge.ScriptData.GetIntList("tokenBoard");
                if (tokenData != null)
                {
                    tokenBoardData = challenge.ScriptData.GetIntList("tokenBoard").ToArray();
                    tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
                }
                else
                {
                    // should never get here as the corners would be missing blocker tokens
                    tokenBoardData = Enumerable.Repeat(0, 64).ToArray();
                    tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
                }
            }

            moveList = challenge.ScriptData.GetGSDataList("moveList");

            string gameTypeString = challenge.ScriptData.GetString("gameType");
            if (gameTypeString != null) {
                gameType = (GameType)Enum.Parse(typeof(GameType), challenge.ScriptData.GetString("gameType"));
            } else {
                gameType = GameType.NONE;
            }

            challengerGamePieceId = challenge.ScriptData.GetString("challengerGamePieceId");
            challengedGamePieceId = challenge.ScriptData.GetString("challengedGamePieceId");
            challengerRatingDelta = challenge.ScriptData.GetInt("challengerRatingDelta").GetValueOrDefault();
            challengedRatingDelta = challenge.ScriptData.GetInt("challengedRatingDelta").GetValueOrDefault();
        }

        public GameSparksChallenge(ListChallengeResponse._Challenge challenge) {
            //Debug.Log("GameSparksChallenge(ListChallengeResponse._Challenge");
            challengeId = challenge.ChallengeId;
            currentPlayerMove = challenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
            isPlayerOneTurn = currentPlayerMove == (int)Piece.BLUE ? true : false;
            nextPlayer = challenge.NextPlayer;
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
            List<int> tokenData = challenge.ScriptData.GetIntList("lastTokenBoard");
            int[] tokenBoardData;
            if (tokenData != null) {
                tokenBoardData = challenge.ScriptData.GetIntList("lastTokenBoard").ToArray();
                tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
            } else {
                tokenData = challenge.ScriptData.GetIntList("tokenBoard");
                if (tokenData != null) {
                    tokenBoardData = challenge.ScriptData.GetIntList("tokenBoard").ToArray();
                    tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
                } else {
                    // should never get here as the corners would be missing blocker tokens
                    tokenBoardData = Enumerable.Repeat(0, 64).ToArray();
                    tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
                }
            }

            moveList = challenge.ScriptData.GetGSDataList("moveList");

            string gameTypeString = challenge.ScriptData.GetString("gameType");
            if (gameTypeString != null)
            {
                gameType = (GameType)Enum.Parse(typeof(GameType), challenge.ScriptData.GetString("gameType"));
            }
            else
            {
                gameType = GameType.NONE;
            }

            challengerGamePieceId = challenge.ScriptData.GetString("challengerGamePieceId");
            challengedGamePieceId = challenge.ScriptData.GetString("challengedGamePieceId");
            challengerRatingDelta = challenge.ScriptData.GetInt("challengerRatingDelta").GetValueOrDefault();
            challengedRatingDelta = challenge.ScriptData.GetInt("challengedRatingDelta").GetValueOrDefault();
        }

        public GameSparksChallenge(ChallengeIssuedMessage._Challenge challenge)
        {
            challengeId = challenge.ChallengeId;
            currentPlayerMove = challenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
            isPlayerOneTurn = currentPlayerMove == (int)Piece.BLUE ? true : false;
            nextPlayer = challenge.NextPlayer;
            if (challenge.State == "COMPLETE")
            {
                isGameOver = true;
            }
            else
            {
                isGameOver = false;
            }

            List<int> lastBoardData = challenge.ScriptData.GetIntList("lastGameBoard");
            if (lastBoardData != null)
            {
                previousGameboardData = challenge.ScriptData.GetIntList("lastGameBoard").ToArray();
            }
            else
            {
                previousGameboardData = new int[64];
            }

            string tokenBoardId = challenge.ScriptData.GetString("tokenBoardId");
            string tokenBoardName = challenge.ScriptData.GetString("tokenBoardName");
            List<int> tokenData = challenge.ScriptData.GetIntList("lastTokenBoard");
            int[] tokenBoardData;
            if (tokenData != null)
            {
                tokenBoardData = challenge.ScriptData.GetIntList("lastTokenBoard").ToArray();
                tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
            }
            else
            {
                tokenData = challenge.ScriptData.GetIntList("tokenBoard");
                if (tokenData != null)
                {
                    tokenBoardData = challenge.ScriptData.GetIntList("tokenBoard").ToArray();
                    tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
                }
                else
                {
                    // should never get here as the corners would be missing blocker tokens
                    tokenBoardData = Enumerable.Repeat(0, 64).ToArray();
                    tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
                }
            }

            moveList = challenge.ScriptData.GetGSDataList("moveList");

            string gameTypeString = challenge.ScriptData.GetString("gameType");
            if (gameTypeString != null)
            {
                gameType = (GameType)Enum.Parse(typeof(GameType), challenge.ScriptData.GetString("gameType"));
            }
            else
            {
                gameType = GameType.NONE;
            }

            challengerGamePieceId = challenge.ScriptData.GetString("challengerGamePieceId");
            challengedGamePieceId = challenge.ScriptData.GetString("challengedGamePieceId");
            challengerRatingDelta = challenge.ScriptData.GetInt("challengerRatingDelta").GetValueOrDefault();
            challengedRatingDelta = challenge.ScriptData.GetInt("challengedRatingDelta").GetValueOrDefault();
        }

        public GameSparksChallenge(ChallengeLostMessage._Challenge challenge)
        {
            challengeId = challenge.ChallengeId;
            currentPlayerMove = challenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
            isPlayerOneTurn = currentPlayerMove == (int)Piece.BLUE ? true : false;
            nextPlayer = challenge.NextPlayer;
            if (challenge.State == "COMPLETE")
            {
                isGameOver = true;
            }
            else
            {
                isGameOver = false;
            }

            List<int> lastBoardData = challenge.ScriptData.GetIntList("lastGameBoard");
            if (lastBoardData != null)
            {
                previousGameboardData = challenge.ScriptData.GetIntList("lastGameBoard").ToArray();
            }
            else
            {
                previousGameboardData = new int[64];
            }

            string tokenBoardId = challenge.ScriptData.GetString("tokenBoardId");
            string tokenBoardName = challenge.ScriptData.GetString("tokenBoardName");
            List<int> tokenData = challenge.ScriptData.GetIntList("lastTokenBoard");
            int[] tokenBoardData;
            if (tokenData != null)
            {
                tokenBoardData = challenge.ScriptData.GetIntList("lastTokenBoard").ToArray();
                tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
            }
            else
            {
                tokenData = challenge.ScriptData.GetIntList("tokenBoard");
                if (tokenData != null)
                {
                    tokenBoardData = challenge.ScriptData.GetIntList("tokenBoard").ToArray();
                    tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
                }
                else
                {
                    // should never get here as the corners would be missing blocker tokens
                    tokenBoardData = Enumerable.Repeat(0, 64).ToArray();
                    tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
                }
            }

            moveList = challenge.ScriptData.GetGSDataList("moveList");

            string gameTypeString = challenge.ScriptData.GetString("gameType");
            if (gameTypeString != null)
            {
                gameType = (GameType)Enum.Parse(typeof(GameType), challenge.ScriptData.GetString("gameType"));
            }
            else
            {
                gameType = GameType.NONE;
            }

            challengerGamePieceId = challenge.ScriptData.GetString("challengerGamePieceId");
            challengedGamePieceId = challenge.ScriptData.GetString("challengedGamePieceId");
            challengerRatingDelta = challenge.ScriptData.GetInt("challengerRatingDelta").GetValueOrDefault();
            challengedRatingDelta = challenge.ScriptData.GetInt("challengedRatingDelta").GetValueOrDefault();
        }

        public GameSparksChallenge(ChallengeWonMessage._Challenge challenge)
        {
            challengeId = challenge.ChallengeId;
            currentPlayerMove = challenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
            isPlayerOneTurn = currentPlayerMove == (int)Piece.BLUE ? true : false;
            nextPlayer = challenge.NextPlayer;
            if (challenge.State == "COMPLETE")
            {
                isGameOver = true;
            }
            else
            {
                isGameOver = false;
            }

            List<int> lastBoardData = challenge.ScriptData.GetIntList("lastGameBoard");
            if (lastBoardData != null)
            {
                previousGameboardData = challenge.ScriptData.GetIntList("lastGameBoard").ToArray();
            }
            else
            {
                previousGameboardData = new int[64];
            }

            string tokenBoardId = challenge.ScriptData.GetString("tokenBoardId");
            string tokenBoardName = challenge.ScriptData.GetString("tokenBoardName");
            List<int> tokenData = challenge.ScriptData.GetIntList("lastTokenBoard");
            int[] tokenBoardData;
            if (tokenData != null)
            {
                tokenBoardData = challenge.ScriptData.GetIntList("lastTokenBoard").ToArray();
                tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
            }
            else
            {
                tokenData = challenge.ScriptData.GetIntList("tokenBoard");
                if (tokenData != null)
                {
                    tokenBoardData = challenge.ScriptData.GetIntList("tokenBoard").ToArray();
                    tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
                }
                else
                {
                    // should never get here as the corners would be missing blocker tokens
                    tokenBoardData = Enumerable.Repeat(0, 64).ToArray();
                    tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
                }
            }

            moveList = challenge.ScriptData.GetGSDataList("moveList");

            string gameTypeString = challenge.ScriptData.GetString("gameType");
            if (gameTypeString != null)
            {
                gameType = (GameType)Enum.Parse(typeof(GameType), challenge.ScriptData.GetString("gameType"));
            }
            else
            {
                gameType = GameType.NONE;
            }

            challengerGamePieceId = challenge.ScriptData.GetString("challengerGamePieceId");
            challengedGamePieceId = challenge.ScriptData.GetString("challengedGamePieceId");
            challengerRatingDelta = challenge.ScriptData.GetInt("challengerRatingDelta").GetValueOrDefault();
            challengedRatingDelta = challenge.ScriptData.GetInt("challengedRatingDelta").GetValueOrDefault();
        }

        public GameSparksChallenge(ChallengeDrawnMessage._Challenge challenge)
        {
            challengeId = challenge.ChallengeId;
            currentPlayerMove = challenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
            isPlayerOneTurn = currentPlayerMove == (int)Piece.BLUE ? true : false;
            nextPlayer = challenge.NextPlayer;
            if (challenge.State == "COMPLETE")
            {
                isGameOver = true;
            }
            else
            {
                isGameOver = false;
            }

            List<int> lastBoardData = challenge.ScriptData.GetIntList("lastGameBoard");
            if (lastBoardData != null)
            {
                previousGameboardData = challenge.ScriptData.GetIntList("lastGameBoard").ToArray();
            }
            else
            {
                previousGameboardData = new int[64];
            }

            string tokenBoardId = challenge.ScriptData.GetString("tokenBoardId");
            string tokenBoardName = challenge.ScriptData.GetString("tokenBoardName");
            List<int> tokenData = challenge.ScriptData.GetIntList("lastTokenBoard");
            int[] tokenBoardData;
            if (tokenData != null)
            {
                tokenBoardData = challenge.ScriptData.GetIntList("lastTokenBoard").ToArray();
                tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
            }
            else
            {
                tokenData = challenge.ScriptData.GetIntList("tokenBoard");
                if (tokenData != null)
                {
                    tokenBoardData = challenge.ScriptData.GetIntList("tokenBoard").ToArray();
                    tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
                }
                else
                {
                    // should never get here as the corners would be missing blocker tokens
                    tokenBoardData = Enumerable.Repeat(0, 64).ToArray();
                    tokenBoard = new TokenBoard(tokenBoardData, tokenBoardId, tokenBoardName, null, null, true);
                }
            }

            moveList = challenge.ScriptData.GetGSDataList("moveList");

            string gameTypeString = challenge.ScriptData.GetString("gameType");
            if (gameTypeString != null)
            {
                gameType = (GameType)Enum.Parse(typeof(GameType), challenge.ScriptData.GetString("gameType"));
            }
            else
            {
                gameType = GameType.NONE;
            }

            challengerGamePieceId = challenge.ScriptData.GetString("challengerGamePieceId");
            challengedGamePieceId = challenge.ScriptData.GetString("challengedGamePieceId");
            challengerRatingDelta = challenge.ScriptData.GetInt("challengerRatingDelta").GetValueOrDefault();
            challengedRatingDelta = challenge.ScriptData.GetInt("challengedRatingDelta").GetValueOrDefault();
        }
    }
}
