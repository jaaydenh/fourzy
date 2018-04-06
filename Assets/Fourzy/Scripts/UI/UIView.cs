using UnityEngine;

namespace Fourzy
{
    public enum AnimationDirection
    {
        left,
        right
    }

    [RequireComponent(typeof(Camera))]

    public class UIView : MonoBehaviour
    {
        private Camera cam;
        private Canvas[] canvases;
        public float animSpeed = 0.5f;
        public UITabButton tabButton;

        public virtual void Awake()
        {
            cam = this.GetComponent<Camera>();
            canvases = this.GetComponentsInChildren<Canvas>();

            Application.targetFrameRate = 30;
        }

        public virtual void Show()
        {
            ViewWillAppear();
            GameObject mainCanvas = this.transform.Find("UICanvas").gameObject;
            GameObject viewContent = mainCanvas.transform.Find("ViewContent").gameObject;
            viewContent.transform.localPosition = Vector3.zero;

            cam.enabled = true;
            foreach (Canvas canvas in canvases)
                canvas.enabled = true;

            Canvas.ForceUpdateCanvases();
        }

        public virtual void Hide()
        {
            cam.enabled = false;
            foreach (Canvas canvas in canvases)
                canvas.enabled = false;

            Canvas.ForceUpdateCanvases();
        }

        public virtual void ShowAnimated(AnimationDirection sourceDirection)
        {
            //Debug.Log("UIView ShowAnimated");
            cam.enabled = true;
            foreach (Canvas canvas in canvases)
                canvas.enabled = true;

            GameObject mainCanvas = this.transform.Find("UICanvas").gameObject;
            GameObject viewContent = mainCanvas.transform.Find("ViewContent").gameObject;
            RectTransform mainCanvasRectTransform = (RectTransform)mainCanvas.transform;

            if (sourceDirection == AnimationDirection.right)
            {
                viewContent.transform.localPosition = new Vector3(mainCanvasRectTransform.rect.width,
                    mainCanvasRectTransform.localPosition.y,
                    mainCanvasRectTransform.localPosition.z);
            }
            else
            {
                viewContent.transform.localPosition = new Vector3(-mainCanvasRectTransform.rect.width,
                    mainCanvasRectTransform.localPosition.y,
                    mainCanvasRectTransform.localPosition.z);
            }

            ViewWillAppear();
            iTween.MoveTo(viewContent.gameObject, iTween.Hash("x", 0, "time", animSpeed, "islocal", true));
            //yield return new WaitForSecondsRealtime(animSpeed);
            Invoke("Show", animSpeed);
            //Show();
            if (tabButton != null)
                tabButton.TabButtonTapped();
        }

        public virtual void HideAnimated(AnimationDirection getAwayDirection)
        {
            cam.enabled = true;
            foreach (Canvas canvas in canvases)
                canvas.enabled = true;

            Canvas.ForceUpdateCanvases();

            GameObject mainCanvas = this.transform.Find("UICanvas").gameObject;
            GameObject viewContent = mainCanvas.transform.Find("ViewContent").gameObject;
            RectTransform mainCanvasRectTransform = (RectTransform)mainCanvas.transform;

            mainCanvas.transform.localPosition = new Vector3(0,
                mainCanvasRectTransform.localPosition.y,
                mainCanvasRectTransform.localPosition.z);

            if (getAwayDirection == AnimationDirection.right)
            {
                iTween.MoveTo(viewContent.gameObject, iTween.Hash("x", -mainCanvasRectTransform.rect.width, "time", animSpeed, "islocal", true));
            }
            else
            {
                iTween.MoveTo(viewContent.gameObject, iTween.Hash("x", mainCanvasRectTransform.rect.width, "time", animSpeed, "islocal", true));
            }

            ViewWillDisappear();
            Invoke("Hide", animSpeed);
        }

        public virtual void SwipeAnimated()
        {
            cam.enabled = true;
            foreach (Canvas canvas in canvases)
                canvas.enabled = true;

            Canvas.ForceUpdateCanvases();

            GameObject mainCanvas = this.transform.Find("UICanvas").gameObject;
            GameObject viewContent = mainCanvas.transform.Find("ViewContent").gameObject;
            RectTransform mainCanvasRectTransform = (RectTransform)mainCanvas.transform;

            mainCanvas.transform.localPosition = new Vector3(0,
                mainCanvasRectTransform.localPosition.y,
                mainCanvasRectTransform.localPosition.z);

            iTween.MoveTo(viewContent.gameObject, iTween.Hash("x", -mainCanvasRectTransform.rect.width, "time", animSpeed, "islocal", true));

            ViewWillDisappear();
            Invoke("Hide", animSpeed);
        }

        //void AnimateComplete()
        //{
        //    Debug.Log("AnimateComplete");
        //    ViewTabs.instance.isAnimating = false;
        //}

        public virtual void ViewWillAppear()
        {

        }

        public virtual void ViewWillDisappear()
        {

        }
    }
}

