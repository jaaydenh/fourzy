//@vadym udod

using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class TabsScreen : MenuScreen
    {
        private List<MenuScreen> tabs;

        protected override void Awake()
        {
            base.Awake();
            
            //get tabs
            tabs = new List<MenuScreen>(GetComponentsInChildren<MenuScreen>());
            tabs.Remove(this);
        }

        protected override void Start()
        {
            base.Start();
            
        }
    }
}
