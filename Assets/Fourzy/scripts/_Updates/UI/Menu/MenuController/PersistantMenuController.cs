//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.UI.Menu
{
    public class PersistantMenuController : MenuController
    {
        public static MenuController instance;

        protected override void Awake()
        {
            if (instance)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            instance = this;
            base.Awake();
        }

        protected override void Update()
        {
            //back button functionality
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (screensStack.Count > 0)
                {
                    MenuScreen menuScreen = screensStack.Peek();

                    if (!menuScreen.interactable)
                        return;

                    menuScreen.OnBack();
                }
            }
        }
    }
}
