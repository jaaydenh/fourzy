using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    public class MovingGamePiece {

        public Direction currentDirection;
        public List<Position> positions;
        public Position position;
        public Position endPosition 
        { 
            get
            {
                return positions[positions.Count - 1];
            }
        }
        public PlayerEnum player = PlayerEnum.NONE;
        public GamePiece gamePiece;
        public bool playHitAnimation;
        public bool isDestroyed;
        public PieceAnimState animationState;
        public float friction;
        public int momentum;

        public MovingGamePiece(Move move, int momentum = 0) 
        {
            positions = new List<Position>();
            this.position = move.position;
            positions.Add(position);
            currentDirection = move.direction;
            this.player = move.player;
            this.isDestroyed = false;
            this.animationState = PieceAnimState.NONE;
            this.friction = 0.0f;

            if (momentum == 0) 
            {
                // if (game.gameState.GameType == GameType.RANDOM || game.gameState.GameType == GameType.FRIEND || game.gameState.GameType == GameType.LEADERBOARD) {
                //int numMoves = GameManager.Instance.gameState.moveList.Count;
                //long tempMomentum = 20 + System.Int64.Parse(GameManager.Instance.challengeInstanceId.Substring(0, 10), System.Globalization.NumberStyles.HexNumber) % 20;
                //this.momentum = (int)tempMomentum;
                //   this.momentum = Random.Range(20, 32);
                if (GameManager.Instance.activeGame == null)
                {
                    this.momentum = Constants.numColumns + Constants.numRows;
                }
                else
                {
                    this.momentum = GameManager.Instance.activeGame.gameState.GetRandomNumber(Constants.numColumns + Constants.numRows, Constants.numColumns + Constants.numRows + 10);
                }
                //Debug.Log("MovingGamePiece: Momentum: " + this.momentum);
                // } else {
                // this.momentum = Random.Range(20, 40);
                // }
            } 
            else 
            {
                this.momentum = momentum;
            }
        }

        public Position GetCurrentPosition() {
            return positions[positions.Count - 1];
        }

        // position is a row and column
        public Position GetNextPosition() {
            return GetNextPositionWithDirection(currentDirection);
        }

        // position is a row and column
        public Position GetNextPositionWithDirection(Direction direction) {
            Position nextPosition = new Position(0,0);
            Position currentPosition = positions[positions.Count - 1];

            switch (direction)
            {
                case Direction.UP:
                    nextPosition.column = currentPosition.column;
                    nextPosition.row = currentPosition.row - 1;
                    break;
                case Direction.DOWN:
                    nextPosition.column = currentPosition.column;
                    nextPosition.row = currentPosition.row + 1;
                    break;
                case Direction.LEFT:
                    nextPosition.column = currentPosition.column - 1;
                    nextPosition.row = currentPosition.row;
                    break;
                case Direction.RIGHT:
                    nextPosition.column = currentPosition.column + 1;
                    nextPosition.row = currentPosition.row;
                    break;
                default:
                    break;
            }

            return nextPosition;
        }
    }
}