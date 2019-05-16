using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Foundation.Editor
{
    public interface SA_iGUILayoutElement
    {
        /// <summary>
        /// Draw a Layout element instance.
        /// </summary>
        void OnGUI();

        void OnLayoutEnable();
    }



}
