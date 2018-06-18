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

        public static Position GetPositonFromTransform(Vector3 pos) {
            int column = Mathf.RoundToInt(pos.x);
            int row = Mathf.CeilToInt((pos.y * -1 - .3f));

            return new Position(column, row);
        }

        public static bool inTopRowBounds(float x, float y)
        {
            return x > 0.5 && x < Constants.numColumns - 1.5 && y > -0.46 && y < 1.2;
        }

        public static bool inBottomRowBounds(float x, float y)
        {
            return x > 0.5 && x < Constants.numColumns - 1.5 && y > -Constants.numColumns && y < -Constants.numColumns + 2;
        }

        public static bool inLeftRowBounds(float x, float y)
        {
            return x > -0.8 && x < 0.66 && y > -Constants.numColumns + 1.3 && y < -0.5;
        }

        public static bool inRightRowBounds(float x, float y)
        {
            return x > Constants.numColumns - 1.7 && x < Constants.numColumns - 0.5 && y > -Constants.numColumns + 1.5 && y < -0.5;
        }
    }
}

