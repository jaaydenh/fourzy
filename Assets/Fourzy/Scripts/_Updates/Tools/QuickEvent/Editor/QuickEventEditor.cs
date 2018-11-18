using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.CodeDom;
using System.Linq;
using Microsoft.CSharp;
using ByteSheep.Events;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(QuickEventBase), true)]
public class QuickEventEditor : PropertyDrawer
{
    private const float IndentValue = 15f;
    private const float ToggleWidth = 16f;
    private const float ControlHeight = 16f;
    private SerializedProperty serializedProperty;
    private InspectorState inspectorState;
    private Dictionary<string, InspectorState> inspectorStateList = new Dictionary<string, InspectorState>();

    // START Inspector data classes

    private class InspectorState
    {
        public ReorderableList reorderableList;
        public InspectorData inspectorData;
        public bool inspectorDataIsNotNull;

        public InspectorState(ReorderableList list, InspectorData data, bool dataIsNotNull = false)
        {
            reorderableList = list;
            inspectorData = data;
            inspectorDataIsNotNull = dataIsNotNull;
        }
    }

    [Serializable]
    private class InspectorData
    {
        public Type eventType;
        public SerializedProperty property;
        public SerializedProperty listProperty;
        public Rect position;
        public int indentLevel;
        public GUIContent label;
        public string eventHeaderText;
        public GenericMenu.MenuFunction2 menuSelectionHandler;
        public GenericMenu.MenuFunction2 noFunctionHandler;
        public PersistentCallData[] persistentCallData;
        public EventFilterAttribute filterAttribute;

        public InspectorData(SerializedProperty property, GUIContent label, Rect position, int indentLevel, GenericMenu.MenuFunction2 menuSelectionHandler, GenericMenu.MenuFunction2 noFunctionHandler, FieldInfo fieldInfo)
        {
            this.property = property;
            this.listProperty = property.FindPropertyRelative("persistentCalls").FindPropertyRelative("calls");

            this.position = position;
            this.indentLevel = indentLevel;
            this.label = label;
            this.menuSelectionHandler = menuSelectionHandler;
            this.noFunctionHandler = noFunctionHandler;

            object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(EventFilterAttribute), false);
            if (customAttributes.Length == 1)
                filterAttribute = customAttributes[0] as EventFilterAttribute;

            Type[] genericArguments = fieldInfo.FieldType.BaseType.GetGenericArguments();
            if (genericArguments.Length == 1)
                eventType = genericArguments[0];

            eventHeaderText = label.text + " (";
            for (int i = 0; i < genericArguments.Length; i++)
            {
                eventHeaderText += GetCleanTypeName(genericArguments[i]);
                if (i < genericArguments.Length - 1)
                    eventHeaderText += ", ";
            }
            eventHeaderText += ")";

            persistentCallData = new PersistentCallData[listProperty.arraySize];
            for (int i = 0; i < listProperty.arraySize; i++)
                persistentCallData[i] = new PersistentCallData(this, i, indentLevel, position, property, listProperty, menuSelectionHandler, noFunctionHandler);
        }

        public void UpdateDataValues(int index)
        {
            // Update PersistentCallData values
            persistentCallData[index] = new PersistentCallData(this, index, indentLevel, position, property, listProperty, menuSelectionHandler, noFunctionHandler);
        }
    }

    [Serializable]
    private class PersistentCallData
    {
        public InspectorData inspectorData;
        public SerializedProperty persistentCallProperty;
        public SerializedProperty genericMenuDataProperty;
        public SerializedProperty componentIndexProperty;
        public SerializedProperty memberIndexProperty;
        public SerializedProperty isDynamicDataProperty;

        public SerializedProperty targetProperty;
        public SerializedProperty memberNameProperty;
        public SerializedProperty memberTypeProperty;
        public SerializedProperty isDynamicProperty;
        public SerializedProperty isCallEnabledProperty;
        public SerializedProperty argumentTypeProperty;

        public GameObject targetGameObject;
        public int index;
        public GenericMenu genericMenu;
        public MemberMenuData genericMenuData;
        public string selectedOptionText;
        public string shortSelectedOptionText;

        public PersistentCallData(InspectorData inspectorData, int index, int indentLevel, Rect position, SerializedProperty property, SerializedProperty listProperty, GenericMenu.MenuFunction2 menuSelectionHandler, GenericMenu.MenuFunction2 noFunctionHandler)
        {
            this.inspectorData = inspectorData;
            this.index = index;
            this.persistentCallProperty = listProperty.GetArrayElementAtIndex(index);
            this.genericMenuDataProperty = persistentCallProperty.FindPropertyRelative("genericMenuData");
            this.componentIndexProperty = genericMenuDataProperty.FindPropertyRelative("selectedComponent");
            this.memberIndexProperty = genericMenuDataProperty.FindPropertyRelative("selectedMember");
            this.isDynamicDataProperty = genericMenuDataProperty.FindPropertyRelative("isDynamic");

            this.targetProperty = persistentCallProperty.FindPropertyRelative("target");
            this.memberNameProperty = persistentCallProperty.FindPropertyRelative("memberName");
            this.memberTypeProperty = persistentCallProperty.FindPropertyRelative("memberType");
            this.isDynamicProperty = persistentCallProperty.FindPropertyRelative("isDynamic");
            this.isCallEnabledProperty = persistentCallProperty.FindPropertyRelative("isCallEnabled");
            this.argumentTypeProperty = persistentCallProperty.FindPropertyRelative("argument").FindPropertyRelative("supportedType");

            // Try to cast assigned target to GameObject type
            targetGameObject = targetProperty.objectReferenceValue as GameObject;
            Component targetComponent = targetProperty.objectReferenceValue as Component;

            if (targetGameObject == null)
            {
                // If that doesn't work, perhaps a component was assigned
                if (targetComponent != null)
                    targetGameObject = targetComponent.gameObject;
                // If it wasn't a component or not an instance then set the field to null
                if (targetGameObject == null)
                    targetProperty.objectReferenceValue = null;
            }

            if (targetGameObject == null) return;

            // Add all components and the game object reference to the selectable list
            Component[] components = targetGameObject.GetComponents(typeof(Component));
            Object[] gameObjectAndComponents = new Object[components.Length + 1];
            gameObjectAndComponents[0] = targetGameObject;
            components.CopyTo(gameObjectAndComponents, 1);
            genericMenuData = new MemberMenuData(inspectorData, gameObjectAndComponents);

            // Check if components have been added or moved up/down in the inspector
            if (componentIndexProperty.intValue != -1)
                for (int i = 0; i < components.Length; i++)
                    if (components[i] == targetComponent)
                        componentIndexProperty.intValue = i + 1;

            // Check if members have been renamed or removed
            if (componentIndexProperty.intValue >= 0 && memberIndexProperty.intValue >= 0)
            {
                MemberMenuComponent selectedComponent = genericMenuData.components[Mathf.Clamp(componentIndexProperty.intValue, 0, genericMenuData.components.Length - 1)];
                MemberMenuItem selectedMemberItem = selectedComponent.GetMemberItem(memberIndexProperty.intValue, isDynamicDataProperty.boolValue);
                if (selectedMemberItem != null)
                {
                    // Check if string value at memberIndexProperty.intValue equals the expected member name
                    string selectedMemberName = selectedMemberItem.memberInfo.Name;
                    if (selectedMemberName != memberNameProperty.stringValue)
                    {
                        // Get the relevant members array
                        MemberMenuItem[] members = isDynamicDataProperty.boolValue ? selectedComponent.dynamicMembers : selectedComponent.staticMembers;

                        // Try to find the correct index
                        for (int i = 0; i < members.Length; i++)
                            if (members[i].memberInfo.Name == memberNameProperty.stringValue)
                                memberIndexProperty.intValue = i;

                        // Ensure that if the member has been renamed we keep our current index and update the memberNameProperty value
                        memberNameProperty.stringValue = members[Mathf.Clamp(memberIndexProperty.intValue, 0, members.Length - 1)].memberInfo.Name;
                    }
                }
            }

            genericMenu = new GenericMenu();
            bool isNoFunctionSelected = componentIndexProperty.intValue == -1;
            genericMenu.AddItem(new GUIContent("No Function"), isNoFunctionSelected, noFunctionHandler, new MemberMenuIndex(property, index, 0, 0, false));
            genericMenu.AddSeparator("");

            for (int i = 0; i < genericMenuData.components.Length; i++)
            {
                MemberMenuComponent component = genericMenuData.components[i];

                for (int j = 0; j < component.dynamicMembers.Length; j++)
                {
                    string prefix = component.displayName + "/";
                    if (j == 0)
                        genericMenu.AddDisabledItem(new GUIContent(prefix + "Dynamic " + GetCleanTypeName(inspectorData.eventType)));

                    bool isSelected = (isDynamicProperty.boolValue && i == componentIndexProperty.intValue && component.GetMemberItem(j, true).index == memberIndexProperty.intValue);
                    genericMenu.AddItem(new GUIContent(prefix + component.dynamicMembers[j].memberInfo.Name), isSelected, menuSelectionHandler, new MemberMenuIndex(property, index, i, j, true));
                }

                if (component.dynamicMembers.Length > 0 && component.staticMembers.Length > 0)
                    genericMenu.AddDisabledItem(new GUIContent(component.displayName + "/ "));

                for (int j = 0; j < component.staticMembers.Length; j++)
                {
                    string prefix = component.displayName + "/";
                    if (j == 0)
                        genericMenu.AddDisabledItem(new GUIContent(prefix + "Static Parameters"));

                    bool isSelected = (!isDynamicProperty.boolValue && i == componentIndexProperty.intValue && component.GetMemberItem(j, false).index == memberIndexProperty.intValue);
                    genericMenu.AddItem(new GUIContent(prefix + component.staticMembers[j].menuDisplayName), isSelected, menuSelectionHandler, new MemberMenuIndex(property, index, i, j, false));
                }
            }

            selectedOptionText = "No Function";
            shortSelectedOptionText = selectedOptionText;
			
            if (componentIndexProperty.intValue >= 0)
            {
                MemberMenuComponent menuComponent = genericMenuData.components[Mathf.Clamp(componentIndexProperty.intValue, 0, genericMenuData.components.Length - 1)];
                MemberMenuItem menuItem = menuComponent.GetMemberItem(memberIndexProperty.intValue, isDynamicProperty.boolValue);
                if (menuItem != null)
                {
                    shortSelectedOptionText = menuItem.shortDisplayName;
                    selectedOptionText = menuComponent.component.GetType().Name + "." + shortSelectedOptionText;
                }
            }

            property.serializedObject.ApplyModifiedProperties();
        }
    }

    private class MemberMenuIndex
    {
        public SerializedProperty property;
        public int persistentCallIndex;
        public int componentIndex = -1;
        public int memberIndex;
        public bool isDynamic;

        public MemberMenuIndex(SerializedProperty property, int persistentCallIndex, int componentIndex, int memberIndex, bool isDynamic)
        {
            this.property = property;
            this.persistentCallIndex = persistentCallIndex;
            this.componentIndex = componentIndex;
            this.memberIndex = memberIndex;
            this.isDynamic = isDynamic;
        }
    }

    private class MemberMenuData
    {
        public InspectorData inspectorData;
        public MemberMenuComponent[] components;
        private Dictionary<Type, int> componentOccurrences = new Dictionary<Type, int>();

        public MemberMenuData(InspectorData inspectorData, Object[] components)
        {
            this.inspectorData = inspectorData;
            this.components = new MemberMenuComponent[components.Length];
            for (int i = 0; i < components.Length; i++)
            {
                int occurences = 1;
                if (componentOccurrences.ContainsKey(components[i].GetType()))
                    occurences = ++componentOccurrences[components[i].GetType()];
                else
                    componentOccurrences.Add(components[i].GetType(), occurences);

                TargetFilter menuTargetFilters = TargetFilter.Static | TargetFilter.Dynamic;
                TargetTypeFilter menuTypeFilters = TargetTypeFilter.Any;
                if (inspectorData.filterAttribute != null)
                {
                    menuTargetFilters = inspectorData.filterAttribute.targetFilters;
                    menuTypeFilters = inspectorData.filterAttribute.typeFilters;
                }

                this.components[i] = new MemberMenuComponent(i, components[i], components[i].GetType().Name + (occurences > 1 ? (" " + (occurences - 1)) : ""), inspectorData.eventType, menuTargetFilters, menuTypeFilters);
            }
        }

        public MemberMenuComponent GetComponent(int index)
        {
            return components[Mathf.Clamp(index, 0, Mathf.Max(components.Length - 1))];
        }

        public MemberMenuItem GetMemberItem(int componentIndex, int index, bool isDynamic)
        {
            return components[Mathf.Clamp(componentIndex, 0, Mathf.Max(0, components.Length - 1))].GetMemberItem(index, isDynamic);
        }
    }

    private class MemberMenuComponent
    {
        public int index;
        public Object component;
        public string displayName;
        public Type eventType;
        public TargetFilter menuTargetFilters;
        public TargetTypeFilter menuTypeFilters;
        public MemberMenuItem[] staticMembers;
        public MemberMenuItem[] dynamicMembers;

        public MemberMenuComponent(int index, Object component, string displayName, Type eventType, TargetFilter menuTargetFilters, TargetTypeFilter menuTypeFilters)
        {
            this.index = index;
            this.component = component;
            this.displayName = displayName;
            this.eventType = eventType;
            this.menuTargetFilters = menuTargetFilters;
            this.menuTypeFilters = menuTypeFilters;
            // Automatically create members list when new instance of this class is created
            CreateStaticMemberItems(GetValidStaticMembers());
            CreateDynamicMemberItems(GetValidDynamicMembers());
        }

        public MemberInfo[] GetValidDynamicMembers()
        {
            // Sort out all the valid members
            MemberInfo[] members = component.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public);
            List<MemberInfo> validDynamicMembers = new List<MemberInfo>();
            List<MemberInfo> validDynamicMethods = new List<MemberInfo>();

            if (!FilterContains(TargetFilter.Dynamic)) return new MemberInfo[0];

            Type eventType = this.eventType;
            if (eventType == null) return new MemberInfo[0];

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].MemberType == MemberTypes.Property & FilterContains(TargetFilter.DynamicProperty))
                {
                    PropertyInfo propertyInfo = members[i] as PropertyInfo;
                    if (propertyInfo.PropertyType == eventType &&
                        propertyInfo.GetSetMethod(false) != null &&
                        propertyInfo.CanWrite &&
                        !propertyInfo.IsDefined(typeof(ObsoleteAttribute), true))
                    {
                        validDynamicMembers.Add(propertyInfo);
                    }
                }
                else if (members[i].MemberType == MemberTypes.Field & FilterContains(TargetFilter.DynamicField))
                {
                    FieldInfo fieldInfo = members[i] as FieldInfo;
                    if (fieldInfo.FieldType == eventType && !fieldInfo.IsInitOnly && !fieldInfo.IsDefined(typeof(ObsoleteAttribute), true))
                    {
                        validDynamicMembers.Add(fieldInfo);
                    }
                }
            }
            // Next list all methods
            if (FilterContains(TargetFilter.DynamicMethod))
            {
                for (int i = 0; i < members.Length; i++)
                {
                    if (members[i].MemberType == MemberTypes.Method)
                    {
                        MethodInfo methodInfo = members[i] as MethodInfo;
                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        if (!methodInfo.IsSpecialName &&
                            methodInfo.ReturnType == typeof(void) &&
                            parameters.Length == 1 &&
                            parameters[0].ParameterType == eventType &&
                            !methodInfo.IsDefined(typeof(ObsoleteAttribute), true))
                        {
                            validDynamicMethods.Add(methodInfo);
                        }
                    }
                }
            }

            // Sort members alphabetically
            validDynamicMembers = validDynamicMembers.OrderBy(x => x.Name).ToList();
            validDynamicMethods = validDynamicMethods.OrderBy(x => x.Name).ToList();

            return validDynamicMembers.Concat(validDynamicMethods).ToArray();
        }

        public bool ArrayContains<T>(T[] array, T value) where T : class
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == value)
                    return true;
            return false;
        }

        public bool FilterContains(TargetFilter filterValue)
        {
            return ((this.menuTargetFilters & filterValue) != 0);
        }

        public bool FilterContains(TargetTypeFilter filterValue)
        {
            return ((this.menuTypeFilters & filterValue) != 0);
        }

        public static bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
        {
            return potentialDescendant.IsSubclassOf(potentialBase) || potentialDescendant == potentialBase;
        }

        public MemberInfo[] GetValidStaticMembers()
        {
            // Sort out all the valid members
            MemberInfo[] members = component.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public);
            List<MemberInfo> validMembers = new List<MemberInfo>();
            List<MemberInfo> validMethods = new List<MemberInfo>();
            Type[] supportedTypes = GetFilteredStaticTypes();

            if (!FilterContains(TargetFilter.Static)) return new MemberInfo[0];

            // List all properties and fields first
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].MemberType == MemberTypes.Property & FilterContains(TargetFilter.StaticProperty))
                {
                    PropertyInfo propertyInfo = members[i] as PropertyInfo;
                    if (!propertyInfo.IsDefined(typeof(ObsoleteAttribute), true) && propertyInfo.CanWrite && ArrayContains(supportedTypes, propertyInfo.PropertyType))
                        validMembers.Add(propertyInfo);
                }
                else if (members[i].MemberType == MemberTypes.Field & FilterContains(TargetFilter.StaticField))
                {
                    FieldInfo fieldInfo = members[i] as FieldInfo;
                    if (!fieldInfo.IsDefined(typeof(ObsoleteAttribute), true) && ArrayContains(supportedTypes, fieldInfo.FieldType))
                        validMembers.Add(fieldInfo);
                }
            }
            // Next list all methods
            if (FilterContains(TargetFilter.StaticMethod))
            {
                for (int i = 0; i < members.Length; i++)
                {
                    if (members[i].MemberType == MemberTypes.Method)
                    {
                        MethodInfo methodInfo = members[i] as MethodInfo;
                        if (methodInfo.ReturnType == typeof(void) && !methodInfo.IsSpecialName && !methodInfo.IsDefined(typeof(ObsoleteAttribute), true))
                        {
                            ParameterInfo[] parameters = methodInfo.GetParameters();
                            if ((parameters.Length == 0 & FilterContains(TargetTypeFilter.Void)) || (parameters.Length == 1 && ArrayContains(supportedTypes, parameters[0].ParameterType)))
                                validMethods.Add(methodInfo);
                        }
                    }
                }
            }

            // Sort members alphabetically
            validMembers = validMembers.OrderBy(x => x.Name).ToList();
            validMethods = validMethods.OrderBy(x => x.Name).ToList();

            return validMembers.Concat(validMethods).ToArray();
        }

        public void CreateDynamicMemberItems(MemberInfo[] membersInfo)
        {
            dynamicMembers = new MemberMenuItem[membersInfo.Length];
            for (int i = 0; i < membersInfo.Length; i++)
                dynamicMembers[i] = new MemberMenuItem(i, true, membersInfo[i]);
        }

        public void CreateStaticMemberItems(MemberInfo[] membersInfo)
        {
            staticMembers = new MemberMenuItem[membersInfo.Length];
            for (int i = 0; i < membersInfo.Length; i++)
                staticMembers[i] = new MemberMenuItem(i, false, membersInfo[i]);
        }

        public MemberMenuItem GetMemberItem(int index, bool isDynamic)
        {
            if (isDynamic && dynamicMembers.Length > 0)
                return dynamicMembers[Mathf.Clamp(index, 0, dynamicMembers.Length - 1)];
            else if (!isDynamic && staticMembers.Length > 0)
                return staticMembers[Mathf.Clamp(index, 0, staticMembers.Length - 1)];
            else
                return null;
        }

        public Type[] GetFilteredStaticTypes()
        {
            List<Type> filteredTypes = new List<Type>();

            if (FilterContains(TargetTypeFilter.String)) filteredTypes.Add(typeof(string));
            if (FilterContains(TargetTypeFilter.Int)) filteredTypes.Add(typeof(int));
            if (FilterContains(TargetTypeFilter.Float)) filteredTypes.Add(typeof(float));
            if (FilterContains(TargetTypeFilter.Bool)) filteredTypes.Add(typeof(bool));
            if (FilterContains(TargetTypeFilter.Color)) filteredTypes.Add(typeof(Color));
            if (FilterContains(TargetTypeFilter.Vector2)) filteredTypes.Add(typeof(Vector2));
            if (FilterContains(TargetTypeFilter.Vector3)) filteredTypes.Add(typeof(Vector3));
            if (FilterContains(TargetTypeFilter.Object)) filteredTypes.Add(typeof(Object));
            if (FilterContains(TargetTypeFilter.GameObject)) filteredTypes.Add(typeof(GameObject));
            if (FilterContains(TargetTypeFilter.Transform)) filteredTypes.Add(typeof(Transform));

            return filteredTypes.ToArray();
        }
    }

    private class MemberMenuItem
    {
        public int index;
        public bool isDynamic;
        public string menuDisplayName;
        public string shortDisplayName;
        public MemberInfo memberInfo;

        public MemberMenuItem(int index, bool isDynamic, MemberInfo memberInfo, string menuDisplayName = "", string shortDisplayName = "")
        {
            this.index = index;
            this.isDynamic = isDynamic;
            this.memberInfo = memberInfo;

            if (menuDisplayName == String.Empty)
                this.menuDisplayName = GetDisplayName(memberInfo, true);
            else
                this.menuDisplayName = menuDisplayName;

            if (shortDisplayName == String.Empty)
                this.shortDisplayName = GetDisplayName(memberInfo, false);
            else
                this.shortDisplayName = shortDisplayName;
        }

        public static string GetDisplayName(MemberInfo memberInfo, bool prependMemberType)
        {
            if (memberInfo.MemberType == MemberTypes.Method)
            {
                ParameterInfo[] parameters = (memberInfo as MethodInfo).GetParameters();
                return memberInfo.Name + " (" + ((parameters.Length > 0) ? QuickEventEditor.GetCleanTypeName(parameters[0].ParameterType) : "") + ")";
            }
            else if (memberInfo.MemberType == MemberTypes.Property)
            {
                PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                return (prependMemberType ? QuickEventEditor.GetCleanTypeName(propertyInfo.PropertyType) + " " : "") + propertyInfo.Name;
            }
            else
            {
                FieldInfo fieldInfo = memberInfo as FieldInfo;
                return (prependMemberType ? QuickEventEditor.GetCleanTypeName(fieldInfo.FieldType) + " " : "") + fieldInfo.Name;
            }
        }
    }

    // END Inspector data classes

    private InspectorState UpdateInspectorState(SerializedProperty property, ReorderableList list, InspectorData data, bool dataIsNotNull)
    {
        return UpdateInspectorState(property, new InspectorState(list, data, dataIsNotNull));
    }

    private InspectorState UpdateInspectorState(SerializedProperty property, InspectorState state)
    {
        string propertyPath = property.propertyPath;
        if (inspectorStateList.ContainsKey(propertyPath))
            inspectorStateList[propertyPath] = state;
        else
            inspectorStateList.Add(propertyPath, state);
        inspectorState = state;
        return inspectorState;
    }

    private InspectorState GetInspectorState(SerializedProperty property)
    {
        string propertyPath = property.propertyPath;
        inspectorStateList.TryGetValue(propertyPath, out inspectorState);
        return inspectorState;
    }

    private SerializedProperty GetList(SerializedProperty property = null)
    {
        if (inspectorState.inspectorData.listProperty == null)
        {
            if (property == null && inspectorState.inspectorData.property == null)
                return null;

            return inspectorState.inspectorData.listProperty = (property ?? inspectorState.inspectorData.property).FindPropertyRelative("persistentCalls").FindPropertyRelative("calls");
        }
        else
            return inspectorState.inspectorData.listProperty;
    }

    private void OnAddButton(ReorderableList list)
    {
        ReorderableList.defaultBehaviours.DoAddButton(list);
        // Calculate and save new list height
        serializedProperty.FindPropertyRelative("inspectorListHeight").floatValue = Mathf.Max(1, list.count) * list.elementHeight + 38f;
        var callsProperty = serializedProperty.FindPropertyRelative("persistentCalls").FindPropertyRelative("calls");
        // if this isn't a duplicated list element with preassigned values then set defaults
        if (callsProperty.GetArrayElementAtIndex(callsProperty.arraySize - 1).FindPropertyRelative("target").objectReferenceValue == null)
        {
            callsProperty.GetArrayElementAtIndex(callsProperty.arraySize - 1).FindPropertyRelative("genericMenuData").FindPropertyRelative("selectedComponent").intValue = -1;
            callsProperty.GetArrayElementAtIndex(callsProperty.arraySize - 1).FindPropertyRelative("isCallEnabled").boolValue = true;
        }
        // Signal that inspector data needs updating
        inspectorState.inspectorDataIsNotNull = false;
    }

    private void OnRemoveButton(ReorderableList list)
    {
        ReorderableList.defaultBehaviours.DoRemoveButton(list);
        // Calculate and save new list height
        serializedProperty.FindPropertyRelative("inspectorListHeight").floatValue = Mathf.Max(1, list.count) * list.elementHeight + 38f;
        // Signal that inspector data needs updating
        inspectorState.inspectorDataIsNotNull = false;
    }

    private InspectorState CreateReorderableList(SerializedProperty property, GUIContent label)
    {
        if (inspectorState != null && inspectorState.reorderableList == null)
        {
            var listProperty = property.FindPropertyRelative("persistentCalls").FindPropertyRelative("calls");
            UpdateInspectorState(property, new ReorderableList(listProperty.serializedObject, listProperty, true, true, true, true), inspectorState.inspectorData, inspectorState.inspectorDataIsNotNull);

            inspectorState.reorderableList.drawElementCallback = DrawEventListener;
            inspectorState.reorderableList.onRemoveCallback = OnRemoveButton;
            inspectorState.reorderableList.onAddCallback = OnAddButton;

            inspectorState.reorderableList.onChangedCallback = (ReorderableList list) =>
            {
                // Signal that inspector data needs updating
                inspectorState.inspectorDataIsNotNull = false;
            };

            // TODO: Only generate header text once
            inspectorState.reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                string genericArgumentsText = "";
                Type[] genericArguments = fieldInfo.FieldType.BaseType.GetGenericArguments();
                for (int i = 0; i < genericArguments.Length; i++)
                {
                    genericArgumentsText += GetCleanTypeName(genericArguments[i]);
                    if (i < genericArguments.Length - 1)
                        genericArgumentsText += ", ";
                }
                // Draw event header
                EditorGUI.LabelField(rect, label.text + " (" + genericArgumentsText + ")");
            };
        }
        return inspectorState;
    }

    private void GenericMenuSelectionHandler(object menuIndexInfo)
    {
        MemberMenuIndex menuIndex = menuIndexInfo as MemberMenuIndex;
        GetInspectorState(menuIndex.property);
        PersistentCallData persistentCall = inspectorState.inspectorData.persistentCallData[menuIndex.persistentCallIndex];

        if (menuIndex == null || persistentCall.targetProperty.objectReferenceValue == null) return;

        persistentCall.componentIndexProperty.intValue = menuIndex.componentIndex;
        persistentCall.memberIndexProperty.intValue = menuIndex.memberIndex;
        persistentCall.isDynamicDataProperty.boolValue = menuIndex.isDynamic;
        persistentCall.isDynamicProperty.boolValue = menuIndex.isDynamic;

        // Get the info of the option we selected in the generic menu
        MemberMenuItem menuItem = persistentCall.genericMenuData.GetComponent(menuIndex.componentIndex).GetMemberItem(menuIndex.memberIndex, menuIndex.isDynamic);
        // Set target object to reference of component selected in generic dropdown menu
        persistentCall.targetProperty.objectReferenceValue = (menuIndex.componentIndex < 0) ? persistentCall.targetGameObject : persistentCall.genericMenuData.GetComponent(menuIndex.componentIndex).component;

        if (menuItem == null) return;

        // Set target object to reference of component selected in generic dropdown menu
        persistentCall.targetProperty.objectReferenceValue = persistentCall.genericMenuData.GetComponent(menuIndex.componentIndex).component;
        // Set memberName to string at selectedIndex
        persistentCall.memberNameProperty.stringValue = menuItem.memberInfo.Name;
        // Set memberType enum value
        persistentCall.memberTypeProperty.enumValueIndex = GetMemberTypeIndex(menuItem.memberInfo.MemberType);

        // Set the function argument type
        Type argumentType = typeof(void);
        if (menuItem.memberInfo.MemberType == MemberTypes.Method)
        {
            ParameterInfo[] parameterInfo = (menuItem.memberInfo as MethodInfo).GetParameters();
            argumentType = parameterInfo.Length == 0 ? typeof(void) : parameterInfo[0].ParameterType;
        }
        else if (menuItem.memberInfo.MemberType == MemberTypes.Property)
            argumentType = (menuItem.memberInfo as PropertyInfo).PropertyType;
        else if (menuItem.memberInfo.MemberType == MemberTypes.Field)
            argumentType = (menuItem.memberInfo as FieldInfo).FieldType;

        persistentCall.argumentTypeProperty.enumValueIndex = (int)GetArgumentType(argumentType);
        inspectorState.inspectorData.UpdateDataValues(menuIndex.persistentCallIndex);
        inspectorState.inspectorData.property.serializedObject.ApplyModifiedProperties(); //TODO?

        GUIUtility.hotControl = 0;
    }

    private void NoFunctionHandler(object menuIndexInfo)
    {
        MemberMenuIndex menuIndex = menuIndexInfo as MemberMenuIndex;
        if (menuIndex == null) return;

        GetInspectorState(menuIndex.property);

        // Clear serialized member name and type info
        PersistentCallData persistentCall = inspectorState.inspectorData.persistentCallData[menuIndex.persistentCallIndex];
        persistentCall.componentIndexProperty.intValue = -1;
        persistentCall.memberNameProperty.stringValue = "";
        persistentCall.memberTypeProperty.enumValueIndex = GetMemberTypeIndex(MemberTypes.Method);
        persistentCall.argumentTypeProperty.enumValueIndex = (int)QuickSupportedTypes.Void;

        inspectorState.inspectorData.UpdateDataValues(menuIndex.persistentCallIndex);
        inspectorState.inspectorData.property.serializedObject.ApplyModifiedProperties();

        GUIUtility.hotControl = 0;
    }

    private void DrawEventListener(Rect position, int index, bool isActive, bool isFocused)
    {
        GetInspectorState(serializedProperty);
        if (inspectorState == null || !inspectorState.inspectorDataIsNotNull || inspectorState.inspectorData == null || inspectorState.inspectorData.persistentCallData == null)
            return;

        // Calculate control positions
        int indentLevel = EditorGUI.indentLevel;
        int divisions = 3;
        float top = position.y + 4f;
        float left = position.x + ToggleWidth - indentLevel * IndentValue;
        float fullWidth = position.width - ToggleWidth;

        Rect dynamicSize = new Rect(position.x - indentLevel * IndentValue, top, fullWidth, ControlHeight);
        float itemWidth = dynamicSize.width / divisions;

        Rect controlRect0 = new Rect(left - ToggleWidth, top, ToggleWidth + indentLevel * IndentValue - 1f, ControlHeight);
        Rect controlRect1 = new Rect(left + 0f * itemWidth + 1f, top, itemWidth + indentLevel * IndentValue - 1f, ControlHeight);
        Rect controlRect2 = new Rect(left + 1f * itemWidth + 1f, top, itemWidth + indentLevel * IndentValue - 1f, ControlHeight);
        Rect controlRect3 = new Rect(left + 2f * itemWidth + 2f, top, itemWidth + indentLevel * IndentValue - 1f, ControlHeight);

        PersistentCallData persistentCall = inspectorState.inspectorData.persistentCallData[index];
        // Draw listener state toggle (enabled / disabled)
        EditorGUI.PropertyField(controlRect0, persistentCall.isCallEnabledProperty, GUIContent.none);
        // Draw target object field
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(controlRect1, persistentCall.targetProperty, GUIContent.none);
        if (EditorGUI.EndChangeCheck())
        {
            // If the target object field has changed, update the inspector data and the dropdown selection
            inspectorState.inspectorData = new InspectorData(serializedProperty, inspectorState.inspectorData.label, position, inspectorState.inspectorData.indentLevel, GenericMenuSelectionHandler, NoFunctionHandler, fieldInfo);
            UpdateInspectorState(serializedProperty, inspectorState.reorderableList, inspectorState.inspectorData, true);
            persistentCall = inspectorState.inspectorData.persistentCallData[index];
            GenericMenuSelectionHandler(new MemberMenuIndex(serializedProperty, index, persistentCall.componentIndexProperty.intValue, persistentCall.memberIndexProperty.intValue, persistentCall.isDynamicDataProperty.boolValue));
        }

        if (persistentCall.targetProperty.objectReferenceValue == null)
            GUI.enabled = false;
        else if (Event.current != null && Event.current.type == EventType.MouseDown && controlRect2.Contains(Event.current.mousePosition))
            persistentCall.genericMenu.DropDown(new Rect(controlRect2.x, controlRect2.y + 9f, 0f, 0f));

        GUIStyle popupStyle = new GUIStyle(EditorStyles.popup);
        popupStyle.fixedHeight = ControlHeight;
        EditorGUI.LabelField(controlRect2, new GUIContent(persistentCall.shortSelectedOptionText, persistentCall.selectedOptionText), popupStyle);
        GUI.enabled = true;

        if (persistentCall.targetProperty.objectReferenceValue != null && persistentCall.componentIndexProperty.intValue != -1 && !persistentCall.isDynamicProperty.boolValue)
            DrawArgumentField(controlRect3, index);
    }

    private void DrawArgumentField(Rect position, int index)
    {
        // Based on the method/property/field selected in the dropdown, show the corresponding argument field.
        SerializedProperty persistentCallProperty = GetList(inspectorState.inspectorData.property).GetArrayElementAtIndex(index);
        SerializedProperty argumentProperty = persistentCallProperty.FindPropertyRelative("argument");
        SerializedProperty argumentTypeProperty = argumentProperty.FindPropertyRelative("supportedType");

        SerializedProperty stringArgumentProperty = argumentProperty.FindPropertyRelative("stringArgument");
        SerializedProperty intArgumentProperty = argumentProperty.FindPropertyRelative("intArgument");
        SerializedProperty floatArgumentProperty = argumentProperty.FindPropertyRelative("floatArgument");
        SerializedProperty boolArgumentProperty = argumentProperty.FindPropertyRelative("boolArgument");
        SerializedProperty colorArgumentProperty = argumentProperty.FindPropertyRelative("colorArgument");
        SerializedProperty vector2ArgumentProperty = argumentProperty.FindPropertyRelative("vector2Argument");
        SerializedProperty vector3ArgumentProperty = argumentProperty.FindPropertyRelative("vector3Argument");
        SerializedProperty objectArgumentProperty = argumentProperty.FindPropertyRelative("objectArgument");
        SerializedProperty gameObjectArgumentProperty = argumentProperty.FindPropertyRelative("gameObjectArgument");
        SerializedProperty transformArgumentProperty = argumentProperty.FindPropertyRelative("transformArgument");

        SerializedProperty property = objectArgumentProperty;

        if (argumentTypeProperty.enumValueIndex == (int)QuickSupportedTypes.String)
            property = stringArgumentProperty;
        else if (argumentTypeProperty.enumValueIndex == (int)QuickSupportedTypes.Int)
            property = intArgumentProperty;
        else if (argumentTypeProperty.enumValueIndex == (int)QuickSupportedTypes.Float)
            property = floatArgumentProperty;
        else if (argumentTypeProperty.enumValueIndex == (int)QuickSupportedTypes.Bool)
            property = boolArgumentProperty;
        else if (argumentTypeProperty.enumValueIndex == (int)QuickSupportedTypes.Color)
            property = colorArgumentProperty;
        else if (argumentTypeProperty.enumValueIndex == (int)QuickSupportedTypes.Vector2)
            property = vector2ArgumentProperty;
        else if (argumentTypeProperty.enumValueIndex == (int)QuickSupportedTypes.Vector3)
            property = vector3ArgumentProperty;
        else if (argumentTypeProperty.enumValueIndex == (int)QuickSupportedTypes.Object)
            property = objectArgumentProperty;
        else if (argumentTypeProperty.enumValueIndex == (int)QuickSupportedTypes.GameObject)
            property = gameObjectArgumentProperty;
        else if (argumentTypeProperty.enumValueIndex == (int)QuickSupportedTypes.Transform)
            property = transformArgumentProperty;

        if (argumentTypeProperty.enumValueIndex != (int)QuickSupportedTypes.Void)
            EditorGUI.PropertyField(position, property, GUIContent.none);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int indent = EditorGUI.indentLevel;
        position = new Rect(position.x + indent * IndentValue, position.y, position.width - indent * IndentValue, position.height);

        serializedProperty = property;

        GetInspectorState(property);
        if (inspectorState == null || !inspectorState.inspectorDataIsNotNull)
            UpdateInspectorState(property, inspectorState == null ? null : inspectorState.reorderableList, new InspectorData(property, label, position, indent, GenericMenuSelectionHandler, NoFunctionHandler, fieldInfo), true);

        CreateReorderableList(property, label);

        inspectorState.reorderableList.elementHeight = 25f;
        property.FindPropertyRelative("inspectorListHeight").floatValue = Mathf.Max(1, inspectorState.reorderableList.count) * inspectorState.reorderableList.elementHeight + 38f;
        inspectorState.reorderableList.DoList(position);

        EditorGUI.indentLevel = indent;
    }

    public static string GetCleanTypeName(Type type)
    {
        string typeName;
        using (var provider = new CSharpCodeProvider())
        {
            var typeRef = new CodeTypeReference(type);
            typeName = provider.GetTypeOutput(typeRef);
            string[] nameSpaceName = typeName.Split('.');
            typeName = nameSpaceName[nameSpaceName.Length - 1];
        }
        return typeName;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return property.FindPropertyRelative("inspectorListHeight").floatValue;
    }

    private QuickSupportedTypes GetArgumentType(Type type)
    {
        if (type == typeof(string))
            return QuickSupportedTypes.String;
        else if (type == typeof(int))
            return QuickSupportedTypes.Int;
        else if (type == typeof(float))
            return QuickSupportedTypes.Float;
        else if (type == typeof(bool))
            return QuickSupportedTypes.Bool;
        else if (type == typeof(Color))
            return QuickSupportedTypes.Color;
        else if (type == typeof(Vector2))
            return QuickSupportedTypes.Vector2;
        else if (type == typeof(Vector3))
            return QuickSupportedTypes.Vector3;
        else if (type == typeof(Object))
            return QuickSupportedTypes.Object;
        else if (type == typeof(GameObject))
            return QuickSupportedTypes.GameObject;
        else if (type == typeof(Transform))
            return QuickSupportedTypes.Transform;
        else
            return QuickSupportedTypes.Void;
    }

    // For some reason unity's dropdown maps the indices to different enum values
    private int GetMemberTypeIndex(MemberTypes type)
    {
        switch (type)
        {
            case MemberTypes.Property:
                return 4;
            case MemberTypes.Field:
                return 2;
            default:
                return 3;
        }
    }
}