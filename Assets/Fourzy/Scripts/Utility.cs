using UnityEngine;

namespace Fourzy
{
    public class Utility {

        public static float GetEloRatingDelta(int myRating, int opponentRating, float gameResult) {

            float myChanceToWin = 1 / (1 + Mathf.Pow(10, (opponentRating - myRating) / 400));

            return Mathf.Floor(60 * (gameResult - myChanceToWin));
        }

        public static void SetSpriteAlpha(GameObject go, float alpha) {
            SpriteRenderer sprite = go.GetComponent<SpriteRenderer>();
            Color c = sprite.color;
            c.a = 0.0f;
            sprite.color = c;
        }

        public static Position GetNextPosition(Move move) {
            Position nextPosition = new Position(0,0);

            switch (move.direction)
            {
                case Direction.UP:
                    nextPosition.column = move.position.column;
                    nextPosition.row = move.position.row - 1;
                    break;
                case Direction.DOWN:
                    nextPosition.column = move.position.column;
                    nextPosition.row = move.position.row + 1;
                    break;
                case Direction.LEFT:
                    nextPosition.column = move.position.column - 1;
                    nextPosition.row = move.position.row;
                    break;
                case Direction.RIGHT:
                    nextPosition.column = move.position.column + 1;
                    nextPosition.row = move.position.row;
                    break;
                default:
                    break;
            }

            return nextPosition;
        }

        public static int GetMoveLocation(Move move) {
            int movePosition = -1;

            switch (move.direction)
            {
                case Direction.UP:
                    movePosition = move.position.column;
                    break;
                case Direction.DOWN:
                    movePosition = move.position.column;
                    break;
                case Direction.LEFT:
                    movePosition = move.position.row;
                    break;
                case Direction.RIGHT:
                    movePosition = move.position.row;
                    break;
                default:
                    break;
            }

            return movePosition;
        }
    }
}

