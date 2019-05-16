//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
using System;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    [RequireComponent(typeof(ColorTween))]
    [RequireComponent(typeof(AlphaTween))]
    public class HintBlock : MonoBehaviour
    {
        public bool active = true;

        public AdvancedEvent onShow;
        public AdvancedEvent onHide;
        public AdvancedEvent onSelected;
        public AdvancedEvent onDeselected;

        private ColorTween colorTween;
        private AlphaTween alphaTween;
        private SpriteRenderer spriteRenderer;

        public bool shown { get; private set; }
        public bool selected { get; private set; }
        public Direction direction { get; private set; }
        public int location { get; private set; }
        public BoardLocation boardLocation { get; private set; }
        public GameboardView board { get; private set; }

        public float radius => Mathf.Max(spriteRenderer.bounds.size.x * transform.localScale.x, spriteRenderer.bounds.size.y * transform.localScale.y) * .5f;

        protected void Awake()
        {
            colorTween = GetComponent<ColorTween>();
            alphaTween = GetComponent<AlphaTween>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            shown = false;
            selected = false;
        }

        protected void Start()
        {
            alphaTween.AtProgress(0f);
        }

        public void SetData(BoardLocation _boardLocation, GameboardView board)
        {
            this.board = board;
            boardLocation = _boardLocation;

            if (_boardLocation.Row == 0)
            {
                direction = Direction.DOWN;
                location = _boardLocation.Column;
            }
            else if (_boardLocation.Row == board.model.Rows - 1)
            {
                direction = Direction.UP;
                location = _boardLocation.Column;
            }
            else if (_boardLocation.Column == 0)
            {
                direction = Direction.RIGHT;
                location = _boardLocation.Row;
            }
            else if (_boardLocation.Column == board.model.Columns - 1)
            {
                direction = Direction.LEFT;
                location = _boardLocation.Row;
            }
        }

        public SimpleMove GetMove(Piece piece)
        {
            return new SimpleMove(piece, direction, location);
        }

        public void Show()
        {
            alphaTween.PlayForward(true);
            shown = true;

            onShow.Invoke();
        }

        public void Hide()
        {
            if (!shown)
                return;

            if (selected)
                Deselect();

            shown = false;
            alphaTween.PlayBackward(true);

            onHide.Invoke();
        }

        public void Select()
        {
            if (selected || !active)
                return;

            selected = true;
            colorTween.PlayForward(true);

            onSelected.Invoke();
        }

        public void Deselect()
        {
            if (!selected)
                return;

            selected = false;
            colorTween.PlayBackward(true);

            onDeselected.Invoke();
        }
    }
}