using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SA.Foundation.Editor
{
    public abstract class SA_GUILayoutElement : ScriptableObject, SA_iGUILayoutElement
    {
        protected Rect m_position = new Rect();

        public abstract void OnGUI();
        public virtual void OnLayoutEnable() { }
        public virtual void OnAwake() {}


        public void SetPosition(Rect position) {
            m_position = position;
        }
    }
}