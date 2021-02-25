//@vadym udod

using FourzyGameModel.Model;
using System.Collections.Generic;
using UnityEngine;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.ClientModel;

namespace Fourzy._Updates.Mechanics.Board
{
    public class MoveArrowsController : MonoBehaviour
    {
        private static float BASE_PROGRESS = .15f;
        private static float CONTINUE_PROGRESS = 1f - BASE_PROGRESS * 2f;

        public MoveArrow arrowPrefab;
        public BoardEdgeXMark xmarkPrefab;

        private GameboardView board;
        private Dictionary<Direction, MoveArrow> arrows = new Dictionary<Direction, MoveArrow>();
        private Dictionary<Direction, BoardEdgeXMark> xmarks = new Dictionary<Direction, BoardEdgeXMark>();
        private Dictionary<Direction, GamePieceView> gamepieces = new Dictionary<Direction, GamePieceView>();

        public Direction pickedDirection { get; private set; } = Direction.NONE;

        protected void Awake()
        {
            board = GetComponentInParent<GameboardView>();
        }

        public void GetObjects(List<BoardLocation> possibleLocations, List<BoardLocation> blockedLocations)
        {
            Direction direction;

            IClientFourzy game = GameManager.Instance.activeGame;

            foreach (BoardLocation location in possibleLocations)
            {
                direction = location.GetDirection();

                MoveArrow arrowInstance = Instantiate(arrowPrefab, transform);
                arrowInstance.Rotate(direction);
                arrowInstance._Reset();
                arrowInstance.Position(location);
                arrowInstance.SetProgress(BASE_PROGRESS);

                arrows.Add(direction, arrowInstance);

                //add gamepieces
                GamePieceView gamepiece = Instantiate(game.activePlayerGamePiece, transform);
                gamepiece.SetAlpha(BASE_PROGRESS * 6f);
                BoardLocation newLocation = location.Neighbor(direction, -1);
                gamepiece.transform.localPosition = board.BoardLocationToVec2(newLocation);
                gamepiece.SetPositionFromTo(gamepiece.transform.localPosition, board.BoardLocationToVec2(location));

                gamepieces.Add(direction, gamepiece);
            }

            foreach(BoardLocation location in blockedLocations)
            {
                direction = location.GetDirection();

                BoardEdgeXMark markInstance = Instantiate(xmarkPrefab, transform);
                markInstance.Position(location);
                markInstance.SetProgress(BASE_PROGRESS * 6f);

                xmarks.Add(direction, markInstance);
            }
        }

        public void ContinueProgress(float value)
        {
            switch (pickedDirection)
            {
                case Direction.RIGHT:
                    SetObjectsAlpha(Direction.RIGHT, BASE_PROGRESS * 2f + (value * CONTINUE_PROGRESS));

                    break;

                case Direction.LEFT:
                    SetObjectsAlpha(Direction.LEFT, BASE_PROGRESS * 2f + (value * CONTINUE_PROGRESS));

                    break;

                case Direction.UP:
                    SetObjectsAlpha(Direction.UP, BASE_PROGRESS * 2f + (value * CONTINUE_PROGRESS));

                    break;

                case Direction.DOWN:
                    SetObjectsAlpha(Direction.DOWN, BASE_PROGRESS * 2f + (value * CONTINUE_PROGRESS));

                    break;
            }
        }

        public void SetInitialProgress(Vector2 value)
        {
            if (Mathf.Abs(value.x) > Mathf.Abs(value.y))
            {
                float inversedX = 1f - Mathf.Abs(value.x);

                if (value.x > 0f)
                {
                    pickedDirection = Direction.RIGHT;
                    SetObjectsAlpha(Direction.RIGHT, BASE_PROGRESS + BASE_PROGRESS * value.x);
                    SetObjectsAlpha(Direction.LEFT, BASE_PROGRESS * inversedX);
                }
                else
                {
                    pickedDirection = Direction.LEFT;
                    SetObjectsAlpha(Direction.RIGHT, BASE_PROGRESS * inversedX);
                    SetObjectsAlpha(Direction.LEFT, BASE_PROGRESS + BASE_PROGRESS * -value.x);
                }

                SetObjectsAlpha(Direction.UP, BASE_PROGRESS * inversedX);
                SetObjectsAlpha(Direction.DOWN, BASE_PROGRESS * inversedX);
            }
            else
            {
                float inversedY = 1f - Mathf.Abs(value.y);

                if (value.y > 0f)
                {
                    pickedDirection = Direction.UP;
                    SetObjectsAlpha(Direction.UP, BASE_PROGRESS + BASE_PROGRESS * value.y);
                    SetObjectsAlpha(Direction.DOWN, BASE_PROGRESS * inversedY);
                }
                else
                {
                    pickedDirection = Direction.DOWN;
                    SetObjectsAlpha(Direction.UP, BASE_PROGRESS * inversedY);
                    SetObjectsAlpha(Direction.DOWN, BASE_PROGRESS + BASE_PROGRESS * -value.y);
                }

                SetObjectsAlpha(Direction.RIGHT, BASE_PROGRESS * inversedY);
                SetObjectsAlpha(Direction.LEFT, BASE_PROGRESS * inversedY);
            }
        }

        public void ExplodeCurrent()
        {
            if (pickedDirection == Direction.NONE) return;

            if (arrows.ContainsKey(pickedDirection)) arrows[pickedDirection].ParticleExplode();
        }

        public void Clear()
        {
            pickedDirection = Direction.NONE;

            foreach (MoveArrow arrow in arrows.Values) Destroy(arrow.gameObject);
            arrows.Clear();

            foreach (BoardEdgeXMark mark in xmarks.Values) Destroy(mark.gameObject);
            xmarks.Clear();

            foreach (GamePieceView gamepiece in gamepieces.Values) Destroy(gamepiece.gameObject);
            gamepieces.Clear();
        }

        public void Hide()
        {
            pickedDirection = Direction.NONE;

            foreach (MoveArrow arrow in arrows.Values) arrow.Hide();
            foreach (BoardEdgeXMark mark in xmarks.Values) mark.Hide(.3f);
        }

        private void SetObjectsAlpha(Direction direciton, float value)
        {
            if (arrows.ContainsKey(direciton)) arrows[direciton].SetProgress(value);
            if (xmarks.ContainsKey(direciton)) xmarks[direciton].SetProgress(value * 6f);
            if (gamepieces.ContainsKey(direciton))
            {
                gamepieces[direciton].SetAlpha(value * 6f);
                gamepieces[direciton].positionTween.AtProgress(value);
            }
        }
    }
}