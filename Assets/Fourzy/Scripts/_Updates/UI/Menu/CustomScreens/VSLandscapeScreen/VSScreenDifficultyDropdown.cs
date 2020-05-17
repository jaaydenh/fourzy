//@vadym udod

using Fourzy._Updates.Tween;
using System;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu
{
    public class VSScreenDifficultyDropdown : MonoBehaviour
    {
        public Action<int> onOption;
        public RectTransform dropdownBody;

        private CanvasGroup canvasGroup;
        private AlphaTween alphaTween;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            alphaTween = GetComponent<AlphaTween>();
        }

        public VSScreenDifficultyDropdown Open(bool filpX)
        {
            alphaTween.PlayForward(true);
            canvasGroup.interactable = canvasGroup.blocksRaycasts = true;

            dropdownBody.transform.localScale = new Vector3(filpX ? -1f : 1f, 1f, 1f);

            return this;
        }

        public void Close()
        {
            alphaTween.PlayBackward(true);
            canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
        }

        public VSScreenDifficultyDropdown SetPosition(Transform _transform, Vector2 offset)
        {
            dropdownBody.transform.position = _transform.position + (Vector3)offset;

            return this;
        }

        public VSScreenDifficultyDropdown SetOnClick(Action<int> action)
        {
            onOption = action;

            return this;
        }

        public void OnOptionClick(int option)
        {
            onOption?.Invoke(option);
            Close();
        }
    }
}