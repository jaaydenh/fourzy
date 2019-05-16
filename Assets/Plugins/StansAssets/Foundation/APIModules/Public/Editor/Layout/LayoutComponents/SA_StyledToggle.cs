using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace SA.Foundation.Editor
{
    public static class SA_StyledToggle
    {

        public enum YesNoBool {
            Yes,
            No
        }

        public enum EnabledDisabledBool {
            Enabled,
            Disabled
        }

        public enum ToggleType {
            YesNo,
            EnabledDisabled
        }


        public static bool ToggleFiled(GUIContent content, bool value, ToggleType type) {
            switch(type) {
                case ToggleType.EnabledDisabled:
                    return EnabledDisabledToggleFiled(content, value);
                case ToggleType.YesNo:
                    return YesNoToggleFiled(content, value);
            }

            return true;
        }



        private static bool EnabledDisabledToggleFiled(GUIContent title, bool value) {

            EnabledDisabledBool initialValue = EnabledDisabledBool.Enabled;
            if (!value) {
                initialValue = EnabledDisabledBool.Disabled;
            }

            if(string.IsNullOrEmpty(title.text)) {
                initialValue = (EnabledDisabledBool)EditorGUILayout.EnumPopup(initialValue);
                if (initialValue == EnabledDisabledBool.Enabled) {
                    value = true;
                } else {
                    value = false;
                }
            } else {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(title);
                initialValue = (EnabledDisabledBool)EditorGUILayout.EnumPopup(initialValue);
                if (initialValue == EnabledDisabledBool.Enabled) {
                    value = true;
                } else {
                    value = false;
                }
                EditorGUILayout.EndHorizontal();
            }

          

            return value;
        }



        private static bool YesNoToggleFiled(GUIContent title, bool value) {

            YesNoBool initialValue = YesNoBool.Yes;
            if (!value) {
                initialValue = YesNoBool.No;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title);

            initialValue = (YesNoBool)EditorGUILayout.EnumPopup(initialValue);
            if (initialValue == YesNoBool.Yes) {
                value = true;
            } else {
                value = false;
            }
            EditorGUILayout.EndHorizontal();

            return value;
        }


    }
}