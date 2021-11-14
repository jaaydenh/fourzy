//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class VSScreenDragableGamepiece : MonoBehaviour
    {
        [SerializeField]
        private GameObject randomIcon;

        private Action<VSScreenDragableGamepiece> onRemoved;
        private Action<VSScreenDragableGamepiece> onDropped;

        private GamePieceView currentPiece;
        private int pointerId = -1;

        private void Update()
        {
            if (pointerId < 0)
            {
                if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                {
                    transform.position = Input.mousePosition;
                }
                else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                {
                    Release(true);
                }
            }
            else
            {
                if (!Input.touches.Any(touch => touch.fingerId == pointerId))
                {
                    Release(true);
                }
                else
                {
                    transform.position = Input.touches.First(touch => touch.fingerId == pointerId).position;
                }
            }
        }

        public VSScreenDragableGamepiece AttachToPointer(PointerEventData pointerEvent)
        {
            if (pointerEvent.pointerId < 0)
            {
                transform.position = Input.mousePosition;
            }
            else
            {
                pointerId = pointerEvent.pointerId;
                transform.position = pointerEvent.position;
            }

            return this;
        }

        public VSScreenDragableGamepiece SetOnRemoved(Action<VSScreenDragableGamepiece> action)
        {
            onRemoved = action;

            return this;
        }

        public VSScreenDragableGamepiece SetOnDropped(Action<VSScreenDragableGamepiece> action)
        {
            onDropped = action;

            return this;
        }

        public void Release(bool invokeDropped)
        {
            Destroy(gameObject);

            onRemoved?.Invoke(this);
            if (invokeDropped)
            {
                onDropped?.Invoke(this);
            }
        }

        public VSScreenDragableGamepiece SetGamepiece(GamePieceData pieceData)
        {
            if (pieceData != null)
            {
                currentPiece = Instantiate(pointerId > 0 ? pieceData.player2Prefab : pieceData.player1Prefab, transform);
                currentPiece.transform.localPosition = Vector3.zero;
                currentPiece.StartBlinking();

                randomIcon.SetActive(false);
            }

            return this;
        }
    }
}