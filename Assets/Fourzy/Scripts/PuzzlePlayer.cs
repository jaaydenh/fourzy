using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

namespace Fourzy
{
    public class PuzzlePlayer : MonoBehaviour {

        public IEnumerator MakeMove(Game game)
        {
			bool foundMove = false;

			// Check if the current board state matches a board state in the list of boards states for the puzzle challenge
			foreach (var puzzleMove in game.puzzleChallengeInfo.Moves)
			{
				string gameBoardString = string.Join("", game.gameState.GetGameBoardArray().Select(item => item.ToString()).ToArray());
				string puzzleMoveBoardString = string.Join("", puzzleMove.BoardState.Select(item => item.ToString()).ToArray());

				puzzleMoveBoardString = Regex.Replace(puzzleMoveBoardString, "9", "[0-2]{1}");
				puzzleMoveBoardString = Regex.Replace(puzzleMoveBoardString, "3", "[1-2]{1}");

				if (Regex.IsMatch(gameBoardString, puzzleMoveBoardString)) {
					Debug.Log("FOUND MOVE");	
					Move newMove = new Move(puzzleMove.Location, (Direction)puzzleMove.Direction, PlayerEnum.TWO);
					//Debug.Log("move direction: " + puzzleMove.Direction);
					//Debug.Log("move location: " + puzzleMove.Location);
					//StartCoroutine(GamePlayManager.Instance.MovePiece(newMove, false, true));
                    GameManager.instance.CallMovePiece(newMove, false, true);
					foundMove = true;
					break;
				}
			}
			if (!foundMove) {
				Debug.Log("!foundMove");
				// Make Random Move
				float random = UnityEngine.Random.Range(0.0f, 1.0f);
				//Debug.Log("random number: " + random);
				float cumulative = 0.0f;
				foreach (var randomMove in game.puzzleChallengeInfo.RandomMoves)
				{
					Debug.Log("game.puzzleChallengeInfo.RandomMoves: " + game.puzzleChallengeInfo.RandomMoves.Count);
					cumulative += randomMove.Weight;
					Debug.Log("randomMove.weight: " + randomMove.Weight);
					Debug.Log("cumulative: " + cumulative);
					if (random < cumulative)
					{
						Debug.Log("make puzzle random move");
						Move newMove = new Move(randomMove.Location, (Direction)randomMove.Direction, PlayerEnum.TWO);

						if (game.gameState.CanMove(newMove.GetNextPosition(), game.gameState.TokenBoard.tokens)) {
							//StartCoroutine(GamePlayManager.Instance.MovePiece(newMove, false, true));
							Debug.Log("make puzzle random move part 2");
                            GameManager.instance.CallMovePiece(newMove, false, true);
							break;
						}
					}
				}
			}

            yield return null;
        }
    }
}
