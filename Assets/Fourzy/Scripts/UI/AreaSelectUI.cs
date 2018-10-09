using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Fourzy
{
    public class AreaSelectUI : MonoBehaviour
    {
        public string AreaName { get; private set; }

        public void OpenAreaOnClick(int index)
        {
            GameContentManager.Instance.CurrentTheme = index;
            ViewController.instance.ChangeView(ViewController.instance.view3);
            ViewController.instance.ShowTabView();
        }
    }
}
