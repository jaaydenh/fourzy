using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;

namespace ByteSheep.Events
{
    public enum QuickSupportedTypes { Void = 0, String, Int, Float, Bool, Color, Vector2, Vector3, Object, GameObject, Transform }

    [Serializable]
    public class GenericMenuData
    {
        [FormerlySerializedAs("m_selectedComponent")] public int selectedComponent;
        [FormerlySerializedAs("m_selectedMember")] public int selectedMember;
        [FormerlySerializedAs("m_isDynamic")] public bool isDynamic;
    }

    [Serializable]
    public class QuickArgumentCache
    {
        [FormerlySerializedAs("m_supportedType")] public QuickSupportedTypes supportedType = QuickSupportedTypes.Void;
        [FormerlySerializedAs("m_stringArgument")] public string stringArgument;
        [FormerlySerializedAs("m_intArgument")] public int intArgument;
        [FormerlySerializedAs("m_floatArgument")] public float floatArgument;
        [FormerlySerializedAs("m_boolArgument")] public bool boolArgument;
        [FormerlySerializedAs("m_colorArgument")] public Color colorArgument;
        [FormerlySerializedAs("m_vector2Argument")] public Vector2 vector2Argument;
        [FormerlySerializedAs("m_vector3Argument")] public Vector3 vector3Argument;
        [FormerlySerializedAs("m_objectArgument")] public Object objectArgument;
        [FormerlySerializedAs("m_gameObjectArgument")] public GameObject gameObjectArgument;
        [FormerlySerializedAs("m_transformArgument")] public Transform transformArgument;

        public object GetArgumentValue()
        {
            switch (supportedType)
            {
                case QuickSupportedTypes.String:
                    return stringArgument;
                case QuickSupportedTypes.Int:
                    return intArgument;
                case QuickSupportedTypes.Float:
                    return floatArgument;
                case QuickSupportedTypes.Bool:
                    return boolArgument;
                case QuickSupportedTypes.Color:
                    return colorArgument;
                case QuickSupportedTypes.Vector2:
                    return vector2Argument;
                case QuickSupportedTypes.Vector3:
                    return vector3Argument;
                case QuickSupportedTypes.Object:
                    return objectArgument;
                case QuickSupportedTypes.GameObject:
                    return gameObjectArgument;
                case QuickSupportedTypes.Transform:
                    return transformArgument;
                default:
                    return null;
            }
        }

        public Type GetArgumentType()
        {
            switch (supportedType)
            {
                case QuickSupportedTypes.String:
                    return typeof(string);
                case QuickSupportedTypes.Int:
                    return typeof(int);
                case QuickSupportedTypes.Float:
                    return typeof(float);
                case QuickSupportedTypes.Bool:
                    return typeof(bool);
                case QuickSupportedTypes.Color:
                    return typeof(Color);
                case QuickSupportedTypes.Vector2:
                    return typeof(Vector2);
                case QuickSupportedTypes.Vector3:
                    return typeof(Vector3);
                case QuickSupportedTypes.Object:
                    return typeof(Object);
                case QuickSupportedTypes.GameObject:
                    return typeof(GameObject);
                case QuickSupportedTypes.Transform:
                    return typeof(Transform);
                default:
                    return null;
            }
        }
    }

    [Serializable]
    public class QuickActionGroup
    {
        public QuickAction QuickDelegate;
        public QuickAction<string> QuickStringDelegate;
        public QuickAction<int> QuickIntDelegate;
        public QuickAction<float> QuickFloatDelegate;
        public QuickAction<bool> QuickBoolDelegate;
        public QuickAction<Color> QuickColorDelegate;
        public QuickAction<Vector2> QuickVector2Delegate;
        public QuickAction<Vector3> QuickVector3Delegate;
        public QuickAction<Object> QuickObjectDelegate;
        public QuickAction<GameObject> QuickGameObjectDelegate;
        public QuickAction<Transform> QuickTransformDelegate;

        public void SetDelegate(Delegate listener, QuickSupportedTypes type)
        {
            switch (type)
            {
                case QuickSupportedTypes.String:
                    QuickStringDelegate = (QuickAction<string>)listener;
                    break;
                case QuickSupportedTypes.Int:
                    QuickIntDelegate = (QuickAction<int>)listener;
                    break;
                case QuickSupportedTypes.Float:
                    QuickFloatDelegate = (QuickAction<float>)listener;
                    break;
                case QuickSupportedTypes.Bool:
                    QuickBoolDelegate = (QuickAction<bool>)listener;
                    break;
                case QuickSupportedTypes.Color:
                    QuickColorDelegate = (QuickAction<Color>)listener;
                    break;
                case QuickSupportedTypes.Vector2:
                    QuickVector2Delegate = (QuickAction<Vector2>)listener;
                    break;
                case QuickSupportedTypes.Vector3:
                    QuickVector3Delegate = (QuickAction<Vector3>)listener;
                    break;
                case QuickSupportedTypes.Object:
                    QuickObjectDelegate = (QuickAction<Object>)listener;
                    break;
                case QuickSupportedTypes.GameObject:
                    QuickGameObjectDelegate = (QuickAction<GameObject>)listener;
                    break;
                case QuickSupportedTypes.Transform:
                    QuickTransformDelegate = (QuickAction<Transform>)listener;
                    break;
                default:
                    QuickDelegate = (QuickAction)listener;
                    break;
            }
        }

        public void Invoke(object argument, QuickSupportedTypes type)
        {
            switch (type)
            {
                case QuickSupportedTypes.String:
                    if (QuickStringDelegate != null)
                        QuickStringDelegate(argument as string);
                    break;
                case QuickSupportedTypes.Int:
                    if (QuickIntDelegate != null)
                        QuickIntDelegate((int)argument);
                    break;
                case QuickSupportedTypes.Float:
                    if (QuickFloatDelegate != null)
                        QuickFloatDelegate((float)argument);
                    break;
                case QuickSupportedTypes.Bool:
                    if (QuickBoolDelegate != null)
                        QuickBoolDelegate((bool)argument);
                    break;
                case QuickSupportedTypes.Color:
                    if (QuickColorDelegate != null)
                        QuickColorDelegate((Color)argument);
                    break;
                case QuickSupportedTypes.Vector2:
                    if (QuickVector2Delegate != null)
                        QuickVector2Delegate((Vector2)argument);
                    break;
                case QuickSupportedTypes.Vector3:
                    if (QuickVector3Delegate != null)
                        QuickVector3Delegate((Vector3)argument);
                    break;
                case QuickSupportedTypes.Object:
                    if (QuickObjectDelegate != null)
                        QuickObjectDelegate(argument as Object);
                    break;
                case QuickSupportedTypes.GameObject:
                    if (QuickGameObjectDelegate != null)
                        QuickGameObjectDelegate(argument as GameObject);
                    break;
                case QuickSupportedTypes.Transform:
                    if (QuickTransformDelegate != null)
                        QuickTransformDelegate(argument as Transform);
                    break;
                default:
                    if (QuickDelegate != null)
                        QuickDelegate();
                    break;
            }
        }
    }

    [Serializable]
    public class QuickPersistentCallGroup
    {
        [FormerlySerializedAs("m_calls")] public List<QuickPersistentCall> calls = new List<QuickPersistentCall>();
    }

    [Serializable]
    public class QuickPersistentCall
    {
        [FormerlySerializedAs("m_genericMenuData")] public GenericMenuData genericMenuData;
        [FormerlySerializedAs("m_target")] public Object target;
        [FormerlySerializedAs("m_memberName")] public string memberName;
        [FormerlySerializedAs("m_memberType")] public MemberTypes memberType;
        [FormerlySerializedAs("m_fieldInfo")] public FieldInfo fieldInfo;
        [FormerlySerializedAs("m_argument")] public QuickArgumentCache argument;
        [FormerlySerializedAs("m_argumentValue")] public object argumentValue;
        [FormerlySerializedAs("m_isDynamic")] public bool isDynamic;
        [FormerlySerializedAs("m_isCallEnabled")] public bool isCallEnabled;
        [FormerlySerializedAs("m_actionGroup"), HideInInspector] public QuickActionGroup actionGroup;

        public void Invoke()
        {
            if (!isCallEnabled || target == null) return;

            if (memberType == MemberTypes.Field)
                fieldInfo.SetValue(target, argumentValue);
            else if (!isDynamic)
                actionGroup.Invoke(argumentValue, argument.supportedType);
        }

        public void SetDynamicArgument(object dynamicArgument)
        {
            if (isCallEnabled && isDynamic)
                argumentValue = dynamicArgument;
        }
    }

    public delegate void QuickAction();
    public delegate void QuickAction<T>(T arg0);
    public delegate void QuickAction<T, U>(T arg0, U arg1);
    public delegate void QuickAction<T, U, V>(T arg0, U arg1, V arg2);
    public delegate void QuickAction<T, U, V, W>(T arg0, U arg1, V arg2, W arg3);

    [Serializable]
    public abstract class QuickEventBase : ISerializationCallbackReceiver
    {
        #pragma warning disable 0414
        [SerializeField]
        [FormerlySerializedAs("m_inspectorListHeight")] private float inspectorListHeight = 40f;
        #pragma warning restore 0414
        [FormerlySerializedAs("m_persistentCalls")] public QuickPersistentCallGroup persistentCalls;

        protected void InvokePersistent()
        {
            for (int i = 0; i < persistentCalls.calls.Count; i++)
                persistentCalls.calls[i].Invoke();
        }

        // Update the argument value of each dynamic call, with the argument passed to QuickEvent<T>.Invoke (T arg0);
        protected void InvokePersistent(object dynamicArgument)
        {
            for (int i = 0; i < persistentCalls.calls.Count; i++)
            {
                persistentCalls.calls[i].SetDynamicArgument(dynamicArgument);
                persistentCalls.calls[i].Invoke();
            }
        }

        protected Type GetActionType(QuickSupportedTypes type)
        {
            switch (type)
            {
                case QuickSupportedTypes.String:
                    return typeof(QuickAction<string>);
                case QuickSupportedTypes.Int:
                    return typeof(QuickAction<int>);
                case QuickSupportedTypes.Float:
                    return typeof(QuickAction<float>);
                case QuickSupportedTypes.Bool:
                    return typeof(QuickAction<bool>);
                case QuickSupportedTypes.Color:
                    return typeof(QuickAction<Color>);
                case QuickSupportedTypes.Vector2:
                    return typeof(QuickAction<Vector2>);
                case QuickSupportedTypes.Vector3:
                    return typeof(QuickAction<Vector3>);
                case QuickSupportedTypes.Object:
                    return typeof(QuickAction<Object>);
                case QuickSupportedTypes.GameObject:
                    return typeof(QuickAction<GameObject>);
                case QuickSupportedTypes.Transform:
                    return typeof(QuickAction<Transform>);
                default:
                    return typeof(QuickAction);
            }
        }

        /// <summary>Get the number of registered persistent listeners.</summary>
        public int GetPersistentEventCount()
        {
            return persistentCalls.calls.Count;
        }

        /// <summary>Get the target member name of the listener at index.</summary>
        /// <param name="index">Index of the listener to query.</param>
        public string GetPersistentMemberName(int index)
        {
            return (GetPersistentEventCount() > 0) ? persistentCalls.calls[Mathf.Clamp(index, 0, Mathf.Max(0, GetPersistentEventCount() - 1))].memberName : null;
        }

        /// <summary>Get the target component of the listener at index.</summary>
        /// <param name="index">Index of the listener to query.</param>
        public Object GetPersistentTarget(int index)
        {
            return (GetPersistentEventCount() > 0) ? persistentCalls.calls[Mathf.Clamp(index, 0, Mathf.Max(0, GetPersistentEventCount() - 1))].target : null;
        }

        /// <summary>Modify the execution state of a listener.</summary>
        /// <param name="index">Index of the listener to query.</param>
        /// <param name="enabled">State to set.</param>
        public void SetPersistentListenerState(int index, bool enabled)
        {
            if (GetPersistentEventCount() > 0)
            {
                persistentCalls.calls[Mathf.Clamp(index, 0, Mathf.Max(0, GetPersistentEventCount() - 1))].isCallEnabled = enabled;
                if (enabled)
                    this.OnAfterDeserialize();
            }
        }

        public virtual void OnBeforeSerialize() { }
        public virtual void OnAfterDeserialize()
        {
            for (int i = 0; i < persistentCalls.calls.Count; i++)
            {
                QuickPersistentCall persistentCall = persistentCalls.calls[i];

                if (!persistentCall.isCallEnabled || (object) persistentCall.target == null || persistentCall.memberName == "" || persistentCall.argument == null || persistentCall.isDynamic)
                    continue;

                Type argumentType = persistentCall.argument.GetArgumentType();
                persistentCall.argumentValue = persistentCall.argument.GetArgumentValue();

                if (persistentCall.memberType == MemberTypes.Method)
                {
                    MethodInfo methodInfo = persistentCall.target.GetType().GetMethod(persistentCall.memberName, (argumentType == null) ? new Type[] { } : new Type[] { argumentType });
                    if (methodInfo != null)
                        persistentCall.actionGroup.SetDelegate(Delegate.CreateDelegate(GetActionType(persistentCall.argument.supportedType), persistentCall.target, methodInfo, true), persistentCall.argument.supportedType);
                }
                else if (persistentCall.memberType == MemberTypes.Property)
                {
                    PropertyInfo propertyInfo = persistentCall.target.GetType().GetProperty(persistentCall.memberName);
                    if (propertyInfo != null)
                    {
                        MethodInfo setMethodInfo = propertyInfo.GetSetMethod();
                        if (setMethodInfo != null)
                            persistentCall.actionGroup.SetDelegate(Delegate.CreateDelegate(GetActionType(persistentCall.argument.supportedType), persistentCall.target, setMethodInfo, true), persistentCall.argument.supportedType);
                    }
                }
                else if (persistentCall.memberType == MemberTypes.Field)
                {
                    persistentCall.fieldInfo = persistentCall.target.GetType().GetField(persistentCall.memberName);
                }
            }
        }
    }

    [Serializable]
    public class QuickEvent : QuickEventBase
    {
        protected QuickAction DynamicMethodCalls;

        /// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
        public void Invoke()
        {
            if (DynamicMethodCalls != null)
                DynamicMethodCalls();

            base.InvokePersistent();
        }

        /// <summary>Add a non persistent listener to the event.</summary>
        /// <param name="listener">Callback function.</param>
        public void AddListener(QuickAction listener) { DynamicMethodCalls += listener; }
        /// <summary>Remove a non persistent listener from the event.</summary>
        /// <param name="listener">Callback function.</param>
        public void RemoveListener(QuickAction listener) { DynamicMethodCalls -= listener; }
        /// <summary>Remove all non persistent listeners from the event.</summary>
        public void RemoveAllListeners() { DynamicMethodCalls = null; }
    }

    [Serializable]
    public class QuickEvent<T> : QuickEventBase
    {
        protected QuickAction<T> DynamicMethodCalls;
        protected QuickAction<T> PersistentDynamicMethodCalls;

        /// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
        public void Invoke(T arg0)
        {
            if (DynamicMethodCalls != null)
                DynamicMethodCalls(arg0);

            if (PersistentDynamicMethodCalls != null)
                PersistentDynamicMethodCalls(arg0);

            base.InvokePersistent(arg0);
        }

        protected Type GetActionType() { return typeof(QuickAction<T>); }
        /// <summary>Add a non persistent listener to the event.</summary>
        /// <param name="listener">Callback function.</param>
        public void AddListener(QuickAction<T> listener) { DynamicMethodCalls += listener; }
        /// <summary>Remove a non persistent listener from the event.</summary>
        /// <param name="listener">Callback function.</param>
        public void RemoveListener(QuickAction<T> listener) { DynamicMethodCalls -= listener; }
        /// <summary>Remove all non persistent listeners from the event.</summary>
        public void RemoveAllListeners() { DynamicMethodCalls = null; }

        protected void AddPersistentListener(QuickAction<T> listener) { PersistentDynamicMethodCalls += listener; }
        protected void RemovePersistentListener(QuickAction<T> listener) { PersistentDynamicMethodCalls -= listener; }
        public void RemoveAllPersistentListeners() { PersistentDynamicMethodCalls = null; }

        public override void OnAfterDeserialize()
        {
            RemoveAllPersistentListeners();
            base.OnAfterDeserialize();

            for (int i = 0; i < persistentCalls.calls.Count; i++)
            {
                QuickPersistentCall persistentCall = persistentCalls.calls[i];

                if (!persistentCall.isCallEnabled || (object) persistentCall.target == null || persistentCall.memberName == "" || !persistentCall.isDynamic)
                    continue;

                if (persistentCall.memberType == MemberTypes.Method)
                {
                    MethodInfo methodInfo = persistentCall.target.GetType().GetMethod(persistentCall.memberName, new Type[] { typeof(T) });
                    if (methodInfo != null)
                        AddPersistentListener((QuickAction<T>)Delegate.CreateDelegate(GetActionType(), persistentCall.target, methodInfo, false));
                }
                else if (persistentCall.memberType == MemberTypes.Property)
                {
                    PropertyInfo propertyInfo = persistentCall.target.GetType().GetProperty(persistentCall.memberName);
                    if (propertyInfo != null)
                    {
                        MethodInfo setMethodInfo = propertyInfo.GetSetMethod();
                        if (setMethodInfo != null)
                            AddPersistentListener((QuickAction<T>)Delegate.CreateDelegate(GetActionType(), persistentCall.target, setMethodInfo, false));
                    }
                }
                else if (persistentCall.memberType == MemberTypes.Field)
                {
                    persistentCall.fieldInfo = persistentCall.target.GetType().GetField(persistentCall.memberName);
                }
            }
        }
    }

    [Serializable]
    public class QuickEvent<T, U> : QuickEventBase
    {
        protected QuickAction<T, U> DynamicMethodCalls;
        protected QuickAction<T, U> PersistentDynamicMethodCalls;

        /// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
        public void Invoke(T arg0, U arg1)
        {
            if (DynamicMethodCalls != null)
                DynamicMethodCalls(arg0, arg1);

            if (PersistentDynamicMethodCalls != null)
                PersistentDynamicMethodCalls(arg0, arg1);

            base.InvokePersistent();
        }

        /// <summary>Add a non persistent listener to the event.</summary>
        /// <param name="listener">Callback function.</param>
        public void AddListener(QuickAction<T, U> listener) { DynamicMethodCalls += listener; }
        /// <summary>Remove a non persistent listener from the event.</summary>
        /// <param name="listener">Callback function.</param>
        public void RemoveListener(QuickAction<T, U> listener) { DynamicMethodCalls -= listener; }
        /// <summary>Remove all non persistent listeners from the event.</summary>
        public void RemoveAllListeners() { DynamicMethodCalls = null; }

        protected void AddPersistentListener(QuickAction<T, U> listener) { PersistentDynamicMethodCalls += listener; }
        protected void RemovePersistentListener(QuickAction<T, U> listener) { PersistentDynamicMethodCalls -= listener; }
        public void RemoveAllPersistentListeners() { PersistentDynamicMethodCalls = null; }

        public override void OnAfterDeserialize()
        {
            RemoveAllPersistentListeners();
            base.OnAfterDeserialize();
        }
    }

    [Serializable]
    public class QuickEvent<T, U, V> : QuickEventBase
    {
        protected QuickAction<T, U, V> DynamicMethodCalls;
        protected QuickAction<T, U, V> PersistentDynamicMethodCalls;

        /// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
        public void Invoke(T arg0, U arg1, V arg2)
        {
            if (DynamicMethodCalls != null)
                DynamicMethodCalls(arg0, arg1, arg2);

            if (PersistentDynamicMethodCalls != null)
                PersistentDynamicMethodCalls(arg0, arg1, arg2);

            base.InvokePersistent();
        }

        /// <summary>Add a non persistent listener to the event.</summary>
        /// <param name="listener">Callback function.</param>
        public void AddListener(QuickAction<T, U, V> listener) { DynamicMethodCalls += listener; }
        /// <summary>Remove a non persistent listener from the event.</summary>
        /// <param name="listener">Callback function.</param>
        public void RemoveListener(QuickAction<T, U, V> listener) { DynamicMethodCalls -= listener; }
        /// <summary>Remove all non persistent listeners from the event.</summary>
        public void RemoveAllListeners() { DynamicMethodCalls = null; }

        protected void AddPersistentListener(QuickAction<T, U, V> listener) { PersistentDynamicMethodCalls += listener; }
        protected void RemovePersistentListener(QuickAction<T, U, V> listener) { PersistentDynamicMethodCalls -= listener; }
        public void RemoveAllPersistentListeners() { PersistentDynamicMethodCalls = null; }

        public override void OnAfterDeserialize()
        {
            RemoveAllPersistentListeners();
            base.OnAfterDeserialize();
        }
    }

    [Serializable]
    public class QuickEvent<T, U, V, W> : QuickEventBase
    {
        protected QuickAction<T, U, V, W> DynamicMethodCalls;
        protected QuickAction<T, U, V, W> PersistentDynamicMethodCalls;

        /// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
        public void Invoke(T arg0, U arg1, V arg2, W arg3)
        {
            if (DynamicMethodCalls != null)
                DynamicMethodCalls(arg0, arg1, arg2, arg3);

            if (PersistentDynamicMethodCalls != null)
                PersistentDynamicMethodCalls(arg0, arg1, arg2, arg3);

            base.InvokePersistent();
        }

        /// <summary>Add a non persistent listener to the event.</summary>
        /// <param name="listener">Callback function.</param>
        public void AddListener(QuickAction<T, U, V, W> listener) { DynamicMethodCalls += listener; }
        /// <summary>Remove a non persistent listener from the event.</summary>
        /// <param name="listener">Callback function.</param>
        public void RemoveListener(QuickAction<T, U, V, W> listener) { DynamicMethodCalls -= listener; }
        /// <summary>Remove all non persistent listeners from the event.</summary>
        public void RemoveAllListeners() { DynamicMethodCalls = null; }

        protected void AddPersistentListener(QuickAction<T, U, V, W> listener) { PersistentDynamicMethodCalls += listener; }
        protected void RemovePersistentListener(QuickAction<T, U, V, W> listener) { PersistentDynamicMethodCalls -= listener; }
        public void RemoveAllPersistentListeners() { PersistentDynamicMethodCalls = null; }

        public override void OnAfterDeserialize()
        {
            RemoveAllPersistentListeners();
            base.OnAfterDeserialize();
        }
    }

    [Serializable] public class QuickStringEvent : QuickEvent<string> { }
    [Serializable] public class QuickIntEvent : QuickEvent<int> { }
    [Serializable] public class QuickFloatEvent : QuickEvent<float> { }
    [Serializable] public class QuickBoolEvent : QuickEvent<bool> { }
    [Serializable] public class QuickColorEvent : QuickEvent<Color> { }
    [Serializable] public class QuickVector2Event : QuickEvent<Vector2> { }
    [Serializable] public class QuickVector3Event : QuickEvent<Vector3> { }
    [Serializable] public class QuickObjectEvent : QuickEvent<Object> { }
    [Serializable] public class QuickGameObjectEvent : QuickEvent<GameObject> { }
    [Serializable] public class QuickTransformEvent : QuickEvent<Transform> { }
}