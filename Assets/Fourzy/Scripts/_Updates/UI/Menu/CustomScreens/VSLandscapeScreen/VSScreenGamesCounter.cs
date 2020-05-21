//@vadym udod

using Fourzy._Updates.UI.Helpers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class VSScreenGamesCounter : WidgetBase
    {
        public Action<int> onOptionSelected;

        public Sprite selectedSprite;
        public Sprite defaultSprite;
        public List<ButtonExtended> buttons = new List<ButtonExtended>();

        private ButtonExtended current;

        public VSScreenGamesCounter SetOptions(List<string> options)
        {
            //no need to clear since create only once
            int index = 0;
            foreach (string option in options)
            {
                buttons[index].SetLabel(option);
                buttons[index].onTap += data => OnOptionSelected(index);

                index++;
            }

            return this;
        }

        public void OnOptionSelected(int index)
        {
            if (current != null) current.image.sprite = defaultSprite;

            current = buttons[index];
            current.image.sprite = selectedSprite;

            onOptionSelected.Invoke(index);
        }
    }
}