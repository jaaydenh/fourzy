//@vadym udod

using System;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class WaitingPromptScreen : PromptScreen
    {
        public void _Prompt(string title, string noButtonText = null)
        {


            Prompt(title, "", null, noButtonText, null, null);
        }
    }
}