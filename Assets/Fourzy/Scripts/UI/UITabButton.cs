using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class UITabButton : MonoBehaviour
    {
        private LayoutElement layoutElement;
        private Image bgImage;
        public float expandOffset;
        public float tabExpandSpeed = 0.5f;
        float tabWidth = 0;//  set volume to expand tab
        private bool isAnimating;  //want allow multiple animation at single time

        void Awake()
        {
            layoutElement = this.GetComponent<LayoutElement>();
            bgImage = this.GetComponent<Image>();

            if (layoutElement != null)
            {
                tabWidth = layoutElement.flexibleWidth;
            }
        }

        public void TabButtonTapped()
        {
            if (isAnimating)
                return;
            isAnimating = true;
            UITabManager.instance.ResetTabs(this);
            if (layoutElement != null)
            {
                Hashtable hashTable = iTween.Hash("from", tabWidth, "to", expandOffset, "time", tabExpandSpeed, "onupdate", "Animate", "oncomplete", "AnimateComplete");
                iTween.ValueTo(this.gameObject, hashTable);
            }
            else
            {
                Debug.Log("Please attach layout element");
            }
            if (bgImage != null) {
                bgImage.color = new Color(0f, 120f/255f, 195f/255f);
            }
        }

        /// <summary>
        /// resets the size when other tab tapped
        /// </summary>
        public void ResetTab()
        {
            Hashtable hashTable = iTween.Hash("from", tabWidth, "to", 1, "time", tabExpandSpeed, "onupdate", "Animate", "oncomplete", "AnimateComplete");
            iTween.ValueTo(this.gameObject, hashTable);
            bgImage.color = new Color(0f, 51f/255f, 83f/255f);
        }

        void Animate(float value)
        {
            layoutElement.flexibleWidth = value;
        }

        void AnimateComplete()
        {
            //Debug.Log("AnimateComplete uitabbutton");
            isAnimating = false;
            tabWidth = layoutElement.flexibleWidth;
        }
    }
}
