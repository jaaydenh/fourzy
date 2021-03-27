//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Helpers;
using System;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu
{
    public class FourzyGameMenuController : MenuController
    {
        public ButtonExtended backButton;

        protected override void OnInvokeMenuEvents(MenuEvents events)
        {

        }

        internal void OnGameLoaded(IClientFourzy game)
        {
            switch (GameManager.Instance.ExpectedGameType)
            {
                case GameTypeLocal.LOCAL_GAME:
                    backButton.SetActive(true);

                    break;

                case GameTypeLocal.REALTIME_BOT_GAME:
                case GameTypeLocal.REALTIME_LOBBY_GAME:
                case GameTypeLocal.REALTIME_QUICKMATCH:
                    backButton.SetActive(false);

                    break;
            }
        }
    }
}