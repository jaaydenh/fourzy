//@vadym udod

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.UI.Helpers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenPointer : WidgetBase
    {
        public static float POINTER_MOVE_SPEED = 2.5f;

        public Image hand;
        public Badge messagaBox;
        public VerticalLayoutGroup container;

        private RectTransform root;

        private GameboardView board;

        public override WidgetBase SetAnchors(Vector2 anchor)
        {
            if (anchor.x > .6f) hand.transform.localScale = Vector3.one;
            else hand.transform.localScale = new Vector3(-1f, 1f, 1f);

            return base.SetAnchors(anchor);
        }

        public override void Hide(float time)
        {
            messagaBox.transform.SetParent(transform);
            base.Hide(time);
        }

        public OnboardingScreenPointer AnimatePointer(params Vector2[] points)
        {
            //update board reference
            board = GamePlayManager.instance.board;

            StartRoutine("pointerAnimation", PointerAnimationRoutine(points));

            return this;
        }

        public OnboardingScreenPointer HidePointer(bool force = false)
        {
            if (force || alphaTween._value > 0f)
            {
                StopRoutine("pointerAnimation", false);
                Hide(.3f);
            }

            return this;
        }

        public OnboardingScreenPointer SetMessage(string message, MessageData messagePositionData = null)
        {
            messagaBox.SetValue(message);

            if (messagePositionData != null)
            {
                messagaBox.transform.SetParent(root);
                messagaBox.SetAnchors(messagePositionData.pivot);
                messagaBox.SetPosition(messagePositionData.positionOffset);
            }
            else
            {
                messagaBox.transform.SetParent(transform);

                Vector2 pivot = new Vector2(0f, 1f);
                Vector2 position = new Vector2(0f, -120f);

                if (rectTransform.anchorMin.x > .6f)
                {
                    pivot.x = 1f;
                    container.childAlignment = TextAnchor.MiddleRight;
                }
                else if (rectTransform.anchorMin.x < .4f)
                    container.childAlignment = TextAnchor.MiddleLeft;
                else
                {
                    pivot.x = .5f;
                    container.childAlignment = TextAnchor.MiddleCenter;
                }

                if (rectTransform.anchorMin.y < .3f)
                {
                    pivot.y = 0f;
                    position.y = 0f;
                }

                messagaBox.ResetAnchors();
                messagaBox.SetPivot(pivot);
                messagaBox.SetPosition(position);
            }

            return this;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            SetInteractable(false);
            BlockRaycast(false);

            root = transform.parent as RectTransform;
        }

        private IEnumerator PointerAnimationRoutine(Vector2[] points)
        {
            if (points.Length == 0) yield break;
            else if (points.Length == 1)
            {
                if (!visible) Show(.2f);
                SetAnchors(Camera.main.WorldToViewportPoint((Vector2)board.transform.position + board.BoardLocationToVec2(points[0].y, points[0].x)));
            }
            else
            {
                while (true)
                {
                    SetAnchors(Camera.main.WorldToViewportPoint((Vector2)board.transform.position + board.BoardLocationToVec2(points[0].y, points[0].x)));

                    if (!visible) Show(.3f);
                    yield return new WaitForSeconds(.7f);

                    for (int pointIndex = 0; pointIndex < points.Length - 1; pointIndex++)
                    {
                        Vector2 start = points[pointIndex];
                        Vector2 end = points[pointIndex + 1];

                        float distance = Vector2.Distance(start, end);

                        for (float t = 0; t < 1; t += Time.deltaTime * POINTER_MOVE_SPEED / distance)
                        {
                            Vector2 point = Vector2.Lerp(start, end, t);

                            SetAnchors(Camera.main.WorldToViewportPoint((Vector2)board.transform.position + board.BoardLocationToVec2(point.y, point.x)));

                            yield return null;
                        }
                    }

                    Hide(.3f);
                    yield return new WaitForSeconds(.8f);
                }
            }
        }
    }
}