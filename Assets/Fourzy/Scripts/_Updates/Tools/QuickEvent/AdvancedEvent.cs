using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;

namespace ByteSheep.Events
{
    public enum AdvancedSupportedTypes { Void = 0, Object, String, Int, Float, Bool, Color, Vector2, Vector3, Enum }

    [Serializable]
    public class AdvancedArgumentCache
    {
        public string typeName;
        [FormerlySerializedAs("m_supportedType")] public AdvancedSupportedTypes supportedType = AdvancedSupportedTypes.Void;
        [FormerlySerializedAs("m_objectArgument")] public Object objectArgument;
        [FormerlySerializedAs("m_stringArgument")] public string stringArgument;
        [FormerlySerializedAs("m_intArgument")] public int intArgument;
        [FormerlySerializedAs("m_floatArgument")] public float floatArgument;
        [FormerlySerializedAs("m_boolArgument")] public bool boolArgument;
        [FormerlySerializedAs("m_colorArgument")] public Color colorArgument;
        [FormerlySerializedAs("m_vector2Argument")] public Vector2 vector2Argument;
        [FormerlySerializedAs("m_vector3Argument")] public Vector3 vector3Argument;
        public string enumArgument;

        public object GetArgumentValue()
        {
            switch (supportedType)
            {
                case AdvancedSupportedTypes.Object:
                    return objectArgument;
                case AdvancedSupportedTypes.String:
                    return stringArgument;
                case AdvancedSupportedTypes.Int:
                    return intArgument;
                case AdvancedSupportedTypes.Float:
                    return floatArgument;
                case AdvancedSupportedTypes.Bool:
                    return boolArgument;
                case AdvancedSupportedTypes.Color:
                    return colorArgument;
                case AdvancedSupportedTypes.Vector2:
                    return vector2Argument;
                case AdvancedSupportedTypes.Vector3:
                    return vector3Argument;
                case AdvancedSupportedTypes.Enum:
                    Type enumType = Type.GetType(typeName);
                    string[] names = Enum.GetNames(enumType);
                    for (int i = 0; i < names.Length; i++)
                        if (enumArgument == names[i])
                            return Enum.GetValues(enumType).GetValue(i);
                    return 0;
                default:
                    return null;
            }
        }

        public Type GetArgumentType()
        {
            switch (supportedType)
            {
                case AdvancedSupportedTypes.Object:
                    return typeof(Object);
                case AdvancedSupportedTypes.String:
                case AdvancedSupportedTypes.Enum:
                    return typeof(string);
                case AdvancedSupportedTypes.Int:
                    return typeof(int);
                case AdvancedSupportedTypes.Float:
                    return typeof(float);
                case AdvancedSupportedTypes.Bool:
                    return typeof(bool);
                case AdvancedSupportedTypes.Color:
                    return typeof(Color);
                case AdvancedSupportedTypes.Vector2:
                    return typeof(Vector2);
                case AdvancedSupportedTypes.Vector3:
                    return typeof(Vector3);
                default:
                    return null;
            }
        }

        public static object[] CombineArguments(AdvancedArgumentCache[] arguments)
        {
            object[] argumentValues = new object[arguments.Length];
            for (int i = 0; i < argumentValues.Length; i++)
                argumentValues[i] = arguments[i].GetArgumentValue();
            return argumentValues;
        }

        public static Type[] CombineArgumentTypes(AdvancedArgumentCache[] arguments)
        {
            Type[] argumentTypes = new Type[arguments.Length];
            for (int i = 0; i < argumentTypes.Length; i++)
            {
                //if (arguments[i].GetArgumentValue() != null)
                    argumentTypes[i] = arguments[i].GetArgumentValue().GetType();
            }
            return argumentTypes;
        }
    }

    public class DynamicArguments
    {
        private object[] oneArgument = new object[1];
        private object[] twoArguments = new object[2];
        private object[] threeArguments = new object[3];
        private object[] fourArguments = new object[4];
        private int argumentCount = 1;

        // Separated parameters avoid the cost of creating a new object array each time the event is invoked
        public object[] UpdateDynamicArguments(object arg0, object arg1 = null, object arg2 = null, object arg3 = null)
        {
            argumentCount = 1;
            if (arg1 != null) argumentCount++;
            if (arg2 != null) argumentCount++;
            if (arg3 != null) argumentCount++;

            switch (argumentCount)
            {
                case 1:
                    oneArgument[0] = arg0;
                    return oneArgument;
                case 2:
                    twoArguments[0] = arg0;
                    twoArguments[1] = arg1;
                    return twoArguments;
                case 3:
                    threeArguments[0] = arg0;
                    threeArguments[1] = arg1;
                    threeArguments[2] = arg1;
                    return threeArguments;
                case 4:
                    fourArguments[0] = arg0;
                    fourArguments[1] = arg1;
                    fourArguments[2] = arg2;
                    fourArguments[3] = arg3;
                    return fourArguments;
                default:
                    return null;
            }
        }
    }

    [Serializable]
    public class AdvancedPersistentCallGroup
    {
        [FormerlySerializedAs("m_calls")] public List<AdvancedPersistentCall> calls = new List<AdvancedPersistentCall>();
    }

    [Serializable]
    public class AdvancedPersistentCall
    {
        [FormerlySerializedAs("m_genericMenuData")] public GenericMenuData genericMenuData;
        [FormerlySerializedAs("m_target")] public Object target;
        [FormerlySerializedAs("m_memberName")] public string memberName;
        [FormerlySerializedAs("m_memberType")] public MemberTypes memberType;
        [FormerlySerializedAs("m_fieldInfo")] public FieldInfo fieldInfo;
        [FormerlySerializedAs("m_methodInfo")] public MethodInfo methodInfo;
        [FormerlySerializedAs("m_arguments")] public AdvancedArgumentCache[] arguments;
        [FormerlySerializedAs("m_argumentValues")] public object[] argumentValues;
        [FormerlySerializedAs("m_argumentTypes")] public Type[] argumentTypes;
        [FormerlySerializedAs("m_isDynamic")] public bool isDynamic;
        [FormerlySerializedAs("m_isCallEnabled")] public bool isCallEnabled;
        public QuickAction ZeroParamMethod = delegate { };

        public void Invoke()
        {
            if (!isCallEnabled || target == null) return;

            if (memberType == MemberTypes.Field && argumentValues.Length > 0 && fieldInfo != null)
                fieldInfo.SetValue(target, argumentValues[0]);
            else
            {
                if (argumentValues.Length == 0)
                    ZeroParamMethod();
                else if (methodInfo != null)
                    methodInfo.Invoke(target, argumentValues);
            }
        }

        public void SetDynamicArguments(object[] dynamicArguments)
        {
            if (isCallEnabled && isDynamic)
                argumentValues = dynamicArguments;
        }
    }

    [Serializable]
    public abstract class AdvancedEventBase : ISerializationCallbackReceiver
    {
#pragma warning disable 0414
        [SerializeField]
        [FormerlySerializedAs("m_inspectorListHeight")]
        private float inspectorListHeight = 40f;
#pragma warning restore 0414
        [FormerlySerializedAs("m_persistentCalls")] public AdvancedPersistentCallGroup persistentCalls = new AdvancedPersistentCallGroup();
        [FormerlySerializedAs("m_dynamicArguments")] private DynamicArguments dynamicArguments = new DynamicArguments();

        protected void InvokePersistent()
        {
            for (int i = 0; i < persistentCalls.calls.Count; i++)
                persistentCalls.calls[i].Invoke();
        }

        // Update the argument value of each dynamic call, with the argument passed to AdvancedEvent<T>.Invoke (T arg0);
        protected void InvokePersistent(object arg0, object arg1 = null, object arg2 = null, object arg3 = null)
        {
            for (int i = 0; i < persistentCalls.calls.Count; i++)
            {
                persistentCalls.calls[i].SetDynamicArguments(dynamicArguments.UpdateDynamicArguments(arg0, arg1, arg2, arg3));
                persistentCalls.calls[i].Invoke();
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
                AdvancedPersistentCall persistentCall = persistentCalls.calls[i];

                if (!persistentCall.isCallEnabled || (object)persistentCall.target == null || persistentCall.memberName == "")
                    continue;

                persistentCall.argumentValues = AdvancedArgumentCache.CombineArguments(persistentCall.arguments);
                persistentCall.argumentTypes = AdvancedArgumentCache.CombineArgumentTypes(persistentCall.arguments);

                if (persistentCall.memberType == MemberTypes.Method)
                {
                    persistentCall.methodInfo = persistentCall.target.GetType().GetMethod(persistentCall.memberName, persistentCall.argumentTypes);
                    if (persistentCall.methodInfo != null && persistentCall.argumentTypes.Length == 0)
                        persistentCall.ZeroParamMethod = (QuickAction)Delegate.CreateDelegate(typeof(QuickAction), persistentCall.target, persistentCall.methodInfo, true);
                }
                else if (persistentCall.memberType == MemberTypes.Property)
                {
                    PropertyInfo propertyInfo = persistentCall.target.GetType().GetProperty(persistentCall.memberName);
                    if (propertyInfo != null)
                        persistentCall.methodInfo = propertyInfo.GetSetMethod();
                }
                else if (persistentCall.memberType == MemberTypes.Field)
                {
                    persistentCall.fieldInfo = persistentCall.target.GetType().GetField(persistentCall.memberName);
                }
            }
        }
    }

    [Serializable]
    public class AdvancedEvent : AdvancedEventBase
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
    public class AdvancedEvent<T> : AdvancedEventBase
    {
        protected QuickAction<T> DynamicMethodCalls;

        /// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
        public void Invoke(T arg0)
        {
            if (DynamicMethodCalls != null)
                DynamicMethodCalls(arg0);

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
    }

    [Serializable]
    public class AdvancedEvent<T, U> : AdvancedEventBase
    {
        protected QuickAction<T, U> DynamicMethodCalls;

        /// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
        public void Invoke(T arg0, U arg1)
        {
            if (DynamicMethodCalls != null)
                DynamicMethodCalls(arg0, arg1);

            base.InvokePersistent(arg0, arg1);
        }

        protected Type GetActionType() { return typeof(QuickAction<T, U>); }
        /// <summary>Add a non persistent listener to the event.</summary>
        /// <param name="listener">Callback function.</param>
        public void AddListener(QuickAction<T, U> listener) { DynamicMethodCalls += listener; }
        /// <summary>Remove a non persistent listener from the event.</summary>
        /// <param name="listener">Callback function.</param>
        public void RemoveListener(QuickAction<T, U> listener) { DynamicMethodCalls -= listener; }
        /// <summary>Remove all non persistent listeners from the event.</summary>
        public void RemoveAllListeners() { DynamicMethodCalls = null; }
    }

    [Serializable]
    public class AdvancedEvent<T, U, V> : AdvancedEventBase
    {
        protected QuickAction<T, U, V> DynamicMethodCalls;

        /// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
        public void Invoke(T arg0, U arg1, V arg2)
        {
            if (DynamicMethodCalls != null)
                DynamicMethodCalls(arg0, arg1, arg2);

            base.InvokePersistent(arg0, arg1, arg2);
        }

        protected Type GetActionType() { return typeof(QuickAction<T, U, V>); }
        /// <summary>Add a non persistent listener to the event.</summary>
        /// <param name="listener">Callback function.</param>
        public void AddListener(QuickAction<T, U, V> listener) { DynamicMethodCalls += listener; }
        /// <summary>Remove a non persistent listener from the event.</summary>
        /// <param name="listener">Callback function.</param>
        public void RemoveListener(QuickAction<T, U, V> listener) { DynamicMethodCalls -= listener; }
        /// <summary>Remove all non persistent listeners from the event.</summary>
        public void RemoveAllListeners() { DynamicMethodCalls = null; }
    }

    [Serializable]
    public class AdvancedEvent<T, U, V, W> : AdvancedEventBase
    {
        protected QuickAction<T, U, V, W> DynamicMethodCalls;

        /// <summary>Invoke all registered callbacks (runtime and persistent).</summary>
        public void Invoke(T arg0, U arg1, V arg2, W arg3)
        {
            if (DynamicMethodCalls != null)
                DynamicMethodCalls(arg0, arg1, arg2, arg3);

            base.InvokePersistent(arg0, arg1, arg2, arg3);
        }

        protected Type GetActionType() { return typeof(QuickAction<T, U, V, W>); }
        /// <summary>Add a non persistent listener to the event.</summary>
        /// <param name="listener">Callback function.</param>
        public void AddListener(QuickAction<T, U, V, W> listener) { DynamicMethodCalls += listener; }
        /// <summary>Remove a non persistent listener from the event.</summary>
        /// <param name="listener">Callback function.</param>
        public void RemoveListener(QuickAction<T, U, V, W> listener) { DynamicMethodCalls -= listener; }
        /// <summary>Remove all non persistent listeners from the event.</summary>
        public void RemoveAllListeners() { DynamicMethodCalls = null; }
    }

    [Serializable] public class AdvancedObjectEvent : AdvancedEvent<Object> { }
    [Serializable] public class AdvancedStringEvent : AdvancedEvent<string> { }
    [Serializable] public class AdvancedIntEvent : AdvancedEvent<int> { }
    [Serializable] public class AdvancedFloatEvent : AdvancedEvent<float> { }
    [Serializable] public class AdvancedBoolEvent : AdvancedEvent<bool> { }
    [Serializable] public class AdvancedColorEvent : AdvancedEvent<Color> { }
    [Serializable] public class AdvancedVector2Event : AdvancedEvent<Vector2> { }
    [Serializable] public class AdvancedVector3Event : AdvancedEvent<Vector3> { }
}