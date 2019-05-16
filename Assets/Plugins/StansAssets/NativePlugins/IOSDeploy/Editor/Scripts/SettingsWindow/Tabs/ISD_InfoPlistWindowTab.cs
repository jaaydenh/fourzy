using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;


namespace SA.iOS.XCode
{
    public class ISD_InfoPlistWindowTab : SA_GUILayoutElement
    {

    
        private static string NewPlistValueName = string.Empty;
        private static string NewValueName = string.Empty;



        public override void OnGUI() {

            SA_EditorGUILayout.Header("PLIST VALUES");

           
            foreach (ISD_PlistKey plistKey in ISD_Settings.Instance.PlistVariables) {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawPlistVariable(plistKey, (object)plistKey, ISD_Settings.Instance.PlistVariables);
                EditorGUILayout.EndVertical();

                if (!ISD_Settings.Instance.PlistVariables.Contains(plistKey)) {
                    return;
                }
            }
            EditorGUILayout.Space();
           

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("New Variable Name");
            NewPlistValueName = EditorGUILayout.TextField(NewPlistValueName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Add", GUILayout.Width(100))) {
                if (NewPlistValueName.Length > 0) {
                    ISD_PlistKey v = new ISD_PlistKey();
                    v.Name = NewPlistValueName;
                    ISD_API.SetInfoPlistKey(v);
                }
                NewPlistValueName = string.Empty;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }




        public static void DrawPlistVariable(ISD_PlistKey plistKey, object valuePointer, IList valueOrigin) {
            EditorGUILayout.BeginHorizontal();

            if(plistKey.Name.Length > 0) {
                plistKey.IsOpen = EditorGUILayout.Foldout(plistKey.IsOpen, plistKey.Name + "   (" + plistKey.Type.ToString() +  ")");
            } else {
                plistKey.IsOpen = EditorGUILayout.Foldout(plistKey.IsOpen, plistKey.Type.ToString());
            }



            bool ItemWasRemoved = SrotingButtons (valuePointer, valueOrigin);
            if(ItemWasRemoved) {
                ISD_Settings.Instance.RemoveVariable (plistKey, valueOrigin);
                return;
            }
            EditorGUILayout.EndHorizontal();

            if(plistKey.IsOpen) {                        
                EditorGUI.indentLevel++; {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Type");
                    if (plistKey.ChildrensIds.Count > 0) {
                        GUI.enabled = false;
                        plistKey.Type = (ISD_PlistKeyType)EditorGUILayout.EnumPopup (plistKey.Type);
                    } else {
                        plistKey.Type = (ISD_PlistKeyType)EditorGUILayout.EnumPopup (plistKey.Type);
                    }
                    EditorGUILayout.EndHorizontal();


                    if (plistKey.Type == ISD_PlistKeyType.Array) {
                        DrawArrayValues (plistKey);
                    } else if (plistKey.Type == ISD_PlistKeyType.Dictionary) {
                        DrawDictionaryValues (plistKey);
                    } else if (plistKey.Type == ISD_PlistKeyType.Boolean) {
                        plistKey.BooleanValue = SA_EditorGUILayout.ToggleFiled("Value", plistKey.BooleanValue, SA_StyledToggle.ToggleType.YesNo);


                    } else {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Value");                                
                        switch(plistKey.Type) {                          
                        case ISD_PlistKeyType.Integer:
                            plistKey.IntegerValue = EditorGUILayout.IntField (plistKey.IntegerValue);
                            break;                                  
                        case ISD_PlistKeyType.String:
                            plistKey.StringValue = EditorGUILayout.TextField (plistKey.StringValue);
                            break;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                } EditorGUI.indentLevel--;
            }

        }


        public static void DrawArrayValues (ISD_PlistKey plistKey) {


            plistKey.IsListOpen = EditorGUILayout.Foldout (plistKey.IsListOpen, "Array Values (" + plistKey.ChildrensIds.Count + ")");

            if (plistKey.IsListOpen) {       

                EditorGUI.indentLevel++; {

                    foreach (string uniqueKey in plistKey.ChildrensIds) {
                        ISD_PlistKey v = ISD_Settings.Instance.getVariableById(uniqueKey);
                        DrawPlistVariable (v, uniqueKey, plistKey.ChildrensIds);

                        if(!plistKey.ChildrensIds.Contains(uniqueKey)) {
                            return;
                        }
                    }


                    EditorGUILayout.BeginHorizontal ();
                    EditorGUILayout.Space ();
                    if (GUILayout.Button ("Add Value", GUILayout.Width (100))) {
                        ISD_PlistKey newVar = new ISD_PlistKey();

                        plistKey.AddChild (newVar);
                    }
                    EditorGUILayout.EndHorizontal ();
                    EditorGUILayout.Space ();

                } EditorGUI.indentLevel--;
            } 
        }

        public static void DrawDictionaryValues (ISD_PlistKey plistKey) {
            plistKey.IsListOpen = EditorGUILayout.Foldout (plistKey.IsListOpen, "Dictionary Values");

            if (plistKey.IsListOpen) {

                EditorGUI.indentLevel++; {

                    foreach (string uniqueKey in plistKey.ChildrensIds) {
                        ISD_PlistKey v = ISD_Settings.Instance.getVariableById(uniqueKey);
                        DrawPlistVariable (v, uniqueKey, plistKey.ChildrensIds);

                        if(!plistKey.ChildrensIds.Contains(uniqueKey)) {
                            return;
                        }
                    }


                    EditorGUILayout.BeginHorizontal ();
                    EditorGUILayout.PrefixLabel ("New Key");
                    NewValueName = EditorGUILayout.TextField (NewValueName);

                    if (GUILayout.Button ("Add", GUILayout.Width (50))) {
                        if (NewValueName.Length > 0) {
                            ISD_PlistKey v = new ISD_PlistKey ();
                            v.Name = NewValueName;
                            plistKey.AddChild (v);                                   
                        }
                    }

                    EditorGUILayout.EndHorizontal ();
                } EditorGUI.indentLevel--;
            } 

        }


        public static bool SrotingButtons(object currentObject, IList ObjectsList) {

            int ObjectIndex = ObjectsList.IndexOf(currentObject);
            if (ObjectIndex == 0) {
                GUI.enabled = false;
            }

            bool up = GUILayout.Button("↑", EditorStyles.miniButtonLeft, GUILayout.Width(20));
            if (up) {
                object c = currentObject;
                ObjectsList[ObjectIndex] = ObjectsList[ObjectIndex - 1];
                ObjectsList[ObjectIndex - 1] = c;
            }


            if (ObjectIndex >= ObjectsList.Count - 1) {
                GUI.enabled = false;
            } else {
                GUI.enabled = true;
            }

            bool down = GUILayout.Button("↓", EditorStyles.miniButtonMid, GUILayout.Width(20));
            if (down) {
                object c = currentObject;
                ObjectsList[ObjectIndex] = ObjectsList[ObjectIndex + 1];
                ObjectsList[ObjectIndex + 1] = c;
            }


            GUI.enabled = true;
            bool r = GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20));
            if (r) {
                ObjectsList.Remove(currentObject);
            }

            return r;
        }


       
    }
}