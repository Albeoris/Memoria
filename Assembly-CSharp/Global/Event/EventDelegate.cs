using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

[Serializable]
public class EventDelegate
{
    public EventDelegate()
    {
    }

    public EventDelegate(EventDelegate.Callback call)
    {
        this.Set(call);
    }

    public EventDelegate(MonoBehaviour target, String methodName)
    {
        this.Set(target, methodName);
    }

    public MonoBehaviour target
    {
        get
        {
            return this.mTarget;
        }
        set
        {
            this.mTarget = value;
            this.mCachedCallback = (EventDelegate.Callback)null;
            this.mRawDelegate = false;
            this.mCached = false;
            this.mMethod = (MethodInfo)null;
            this.mParameterInfos = null;
            this.mParameters = null;
        }
    }

    public String methodName
    {
        get
        {
            return this.mMethodName;
        }
        set
        {
            this.mMethodName = value;
            this.mCachedCallback = (EventDelegate.Callback)null;
            this.mRawDelegate = false;
            this.mCached = false;
            this.mMethod = (MethodInfo)null;
            this.mParameterInfos = null;
            this.mParameters = null;
        }
    }

    public EventDelegate.Parameter[] parameters
    {
        get
        {
            if (!this.mCached)
            {
                this.Cache();
            }
            return this.mParameters;
        }
    }

    public Boolean isValid
    {
        get
        {
            if (!this.mCached)
            {
                this.Cache();
            }
            return (this.mRawDelegate && this.mCachedCallback != null) || (this.mTarget != (UnityEngine.Object)null && !String.IsNullOrEmpty(this.mMethodName));
        }
    }

    public Boolean isEnabled
    {
        get
        {
            if (!this.mCached)
            {
                this.Cache();
            }
            if (this.mRawDelegate && this.mCachedCallback != null)
            {
                return true;
            }
            if (this.mTarget == (UnityEngine.Object)null)
            {
                return false;
            }
            MonoBehaviour monoBehaviour = this.mTarget;
            return monoBehaviour == (UnityEngine.Object)null || monoBehaviour.enabled;
        }
    }

    private static String GetMethodName(EventDelegate.Callback callback)
    {
        return callback.Method.Name;
    }

    private static Boolean IsValid(EventDelegate.Callback callback)
    {
        return callback != null && callback.Method != (MethodInfo)null;
    }

    public override Boolean Equals(Object obj)
    {
        if (obj == null)
        {
            return !this.isValid;
        }
        if (obj is EventDelegate.Callback)
        {
            EventDelegate.Callback callback = obj as EventDelegate.Callback;
            if (callback.Equals(this.mCachedCallback))
            {
                return true;
            }
            MonoBehaviour y = callback.Target as MonoBehaviour;
            return this.mTarget == y && String.Equals(this.mMethodName, EventDelegate.GetMethodName(callback));
        }
        else
        {
            if (obj is EventDelegate)
            {
                EventDelegate eventDelegate = obj as EventDelegate;
                return this.mTarget == eventDelegate.mTarget && String.Equals(this.mMethodName, eventDelegate.mMethodName);
            }
            return false;
        }
    }

    public override Int32 GetHashCode()
    {
        return EventDelegate.s_Hash;
    }

    private void Set(EventDelegate.Callback call)
    {
        this.Clear();
        if (call != null && EventDelegate.IsValid(call))
        {
            this.mTarget = (call.Target as MonoBehaviour);
            if (this.mTarget == (UnityEngine.Object)null)
            {
                this.mRawDelegate = true;
                this.mCachedCallback = call;
                this.mMethodName = (String)null;
            }
            else
            {
                this.mMethodName = EventDelegate.GetMethodName(call);
                this.mRawDelegate = false;
            }
        }
    }

    public void Set(MonoBehaviour target, String methodName)
    {
        this.Clear();
        this.mTarget = target;
        this.mMethodName = methodName;
    }

    private void Cache()
    {
        this.mCached = true;
        if (this.mRawDelegate)
        {
            return;
        }
        if ((this.mCachedCallback == null || this.mCachedCallback.Target as MonoBehaviour != this.mTarget || EventDelegate.GetMethodName(this.mCachedCallback) != this.mMethodName) && this.mTarget != (UnityEngine.Object)null && !String.IsNullOrEmpty(this.mMethodName))
        {
            Type type = this.mTarget.GetType();
            this.mMethod = (MethodInfo)null;
            while (type != null)
            {
                try
                {
                    this.mMethod = type.GetMethod(this.mMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (this.mMethod != null)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                }
                type = type.BaseType;
            }
            if (this.mMethod == null)
            {
                global::Debug.LogError(String.Concat(new Object[]
                {
                    "Could not find method '",
                    this.mMethodName,
                    "' on ",
                    this.mTarget.GetType()
                }), this.mTarget);
                return;
            }
            if (this.mMethod.ReturnType != typeof(void))
            {
                global::Debug.LogError(String.Concat(new Object[]
                {
                    this.mTarget.GetType(),
                    ".",
                    this.mMethodName,
                    " must have a 'void' return type."
                }), this.mTarget);
                return;
            }
            this.mParameterInfos = this.mMethod.GetParameters();
            if (this.mParameterInfos.Length == 0)
            {
                this.mCachedCallback = (EventDelegate.Callback)Delegate.CreateDelegate(typeof(EventDelegate.Callback), this.mTarget, this.mMethodName);
                this.mArgs = null;
                this.mParameters = null;
                return;
            }
            this.mCachedCallback = (EventDelegate.Callback)null;
            if (this.mParameters == null || (Int32)this.mParameters.Length != (Int32)this.mParameterInfos.Length)
            {
                this.mParameters = new EventDelegate.Parameter[(Int32)this.mParameterInfos.Length];
                Int32 i = 0;
                Int32 num = (Int32)this.mParameters.Length;
                while (i < num)
                {
                    this.mParameters[i] = new EventDelegate.Parameter();
                    i++;
                }
            }
            Int32 j = 0;
            Int32 num2 = (Int32)this.mParameters.Length;
            while (j < num2)
            {
                this.mParameters[j].expectedType = this.mParameterInfos[j].ParameterType;
                j++;
            }
        }
    }

    public Boolean Execute()
    {
        if (!this.mCached)
        {
            this.Cache();
        }
        if (this.mCachedCallback != null)
        {
            this.mCachedCallback();
            return true;
        }
        if (this.mMethod != null)
        {
            if (this.mParameters == null || this.mParameters.Length == 0)
            {
                this.mMethod.Invoke(this.mTarget, null);
            }
            else
            {
                if (this.mArgs == null || (Int32)this.mArgs.Length != (Int32)this.mParameters.Length)
                {
                    this.mArgs = new Object[(Int32)this.mParameters.Length];
                }
                Int32 i = 0;
                Int32 num = (Int32)this.mParameters.Length;
                while (i < num)
                {
                    this.mArgs[i] = this.mParameters[i].value;
                    i++;
                }
                try
                {
                    this.mMethod.Invoke(this.mTarget, this.mArgs);
                }
                catch (ArgumentException ex)
                {
                    String text = "Error calling ";
                    if (this.mTarget == (UnityEngine.Object)null)
                    {
                        text += this.mMethod.Name;
                    }
                    else
                    {
                        String text2 = text;
                        text = String.Concat(new Object[]
                        {
                            text2,
                            this.mTarget.GetType(),
                            ".",
                            this.mMethod.Name
                        });
                    }
                    text = text + ": " + ex.Message;
                    text += "\n  Expected: ";
                    if (this.mParameterInfos.Length == 0)
                    {
                        text += "no arguments";
                    }
                    else
                    {
                        text += this.mParameterInfos[0];
                        for (Int32 j = 1; j < (Int32)this.mParameterInfos.Length; j++)
                        {
                            text = text + ", " + this.mParameterInfos[j].ParameterType;
                        }
                    }
                    text += "\n  Received: ";
                    if (this.mParameters.Length == 0)
                    {
                        text += "no arguments";
                    }
                    else
                    {
                        text += this.mParameters[0].type;
                        for (Int32 k = 1; k < (Int32)this.mParameters.Length; k++)
                        {
                            text = text + ", " + this.mParameters[k].type;
                        }
                    }
                    text += "\n";
                    global::Debug.LogError(text);
                }
                Int32 l = 0;
                Int32 num2 = (Int32)this.mArgs.Length;
                while (l < num2)
                {
                    if (this.mParameterInfos[l].IsIn || this.mParameterInfos[l].IsOut)
                    {
                        this.mParameters[l].value = this.mArgs[l];
                    }
                    this.mArgs[l] = null;
                    l++;
                }
            }
            return true;
        }
        return false;
    }

    public void Clear()
    {
        this.mTarget = (MonoBehaviour)null;
        this.mMethodName = (String)null;
        this.mRawDelegate = false;
        this.mCachedCallback = (EventDelegate.Callback)null;
        this.mParameters = null;
        this.mCached = false;
        this.mMethod = (MethodInfo)null;
        this.mParameterInfos = null;
        this.mArgs = null;
    }

    public override String ToString()
    {
        if (!(this.mTarget != (UnityEngine.Object)null))
        {
            return (!this.mRawDelegate) ? null : "[delegate]";
        }
        String text = this.mTarget.GetType().ToString();
        Int32 num = text.LastIndexOf('.');
        if (num > 0)
        {
            text = text.Substring(num + 1);
        }
        if (!String.IsNullOrEmpty(this.methodName))
        {
            return text + "/" + this.methodName;
        }
        return text + "/[delegate]";
    }

    public static void Execute(List<EventDelegate> list)
    {
        if (list != null)
        {
            for (Int32 i = 0; i < list.Count; i++)
            {
                EventDelegate eventDelegate = list[i];
                if (eventDelegate != null)
                {
                    try
                    {
                        eventDelegate.Execute();
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            global::Debug.LogError(ex.InnerException.Message);
                        }
                        else
                        {
                            global::Debug.LogError(ex.Message);
                        }
                    }
                    if (i >= list.Count)
                    {
                        break;
                    }
                    if (list[i] != eventDelegate)
                    {
                        continue;
                    }
                    if (eventDelegate.oneShot)
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                }
            }
        }
    }

    public static Boolean IsValid(List<EventDelegate> list)
    {
        if (list != null)
        {
            Int32 i = 0;
            Int32 count = list.Count;
            while (i < count)
            {
                EventDelegate eventDelegate = list[i];
                if (eventDelegate != null && eventDelegate.isValid)
                {
                    return true;
                }
                i++;
            }
        }
        return false;
    }

    public static EventDelegate Set(List<EventDelegate> list, EventDelegate.Callback callback)
    {
        if (list != null)
        {
            EventDelegate eventDelegate = new EventDelegate(callback);
            list.Clear();
            list.Add(eventDelegate);
            return eventDelegate;
        }
        return (EventDelegate)null;
    }

    public static void Set(List<EventDelegate> list, EventDelegate del)
    {
        if (list != null)
        {
            list.Clear();
            list.Add(del);
        }
    }

    public static EventDelegate Add(List<EventDelegate> list, EventDelegate.Callback callback)
    {
        return EventDelegate.Add(list, callback, false);
    }

    public static EventDelegate Add(List<EventDelegate> list, EventDelegate.Callback callback, Boolean oneShot)
    {
        if (list != null)
        {
            Int32 i = 0;
            Int32 count = list.Count;
            while (i < count)
            {
                EventDelegate eventDelegate = list[i];
                if (eventDelegate != null && eventDelegate.Equals(callback))
                {
                    return eventDelegate;
                }
                i++;
            }
            EventDelegate eventDelegate2 = new EventDelegate(callback);
            eventDelegate2.oneShot = oneShot;
            list.Add(eventDelegate2);
            return eventDelegate2;
        }
        global::Debug.LogWarning("Attempting to add a callback to a list that's null");
        return (EventDelegate)null;
    }

    public static void Add(List<EventDelegate> list, EventDelegate ev)
    {
        EventDelegate.Add(list, ev, ev.oneShot);
    }

    public static void Add(List<EventDelegate> list, EventDelegate ev, Boolean oneShot)
    {
        if (ev.mRawDelegate || ev.target == (UnityEngine.Object)null || String.IsNullOrEmpty(ev.methodName))
        {
            EventDelegate.Add(list, ev.mCachedCallback, oneShot);
        }
        else if (list != null)
        {
            Int32 i = 0;
            Int32 count = list.Count;
            while (i < count)
            {
                EventDelegate eventDelegate = list[i];
                if (eventDelegate != null && eventDelegate.Equals(ev))
                {
                    return;
                }
                i++;
            }
            EventDelegate eventDelegate2 = new EventDelegate(ev.target, ev.methodName);
            eventDelegate2.oneShot = oneShot;
            if (ev.mParameters != null && (Int32)ev.mParameters.Length > 0)
            {
                eventDelegate2.mParameters = new EventDelegate.Parameter[(Int32)ev.mParameters.Length];
                for (Int32 j = 0; j < (Int32)ev.mParameters.Length; j++)
                {
                    eventDelegate2.mParameters[j] = ev.mParameters[j];
                }
            }
            list.Add(eventDelegate2);
        }
        else
        {
            global::Debug.LogWarning("Attempting to add a callback to a list that's null");
        }
    }

    public static Boolean Remove(List<EventDelegate> list, EventDelegate.Callback callback)
    {
        if (list != null)
        {
            Int32 i = 0;
            Int32 count = list.Count;
            while (i < count)
            {
                EventDelegate eventDelegate = list[i];
                if (eventDelegate != null && eventDelegate.Equals(callback))
                {
                    list.RemoveAt(i);
                    return true;
                }
                i++;
            }
        }
        return false;
    }

    public static Boolean Remove(List<EventDelegate> list, EventDelegate ev)
    {
        if (list != null)
        {
            Int32 i = 0;
            Int32 count = list.Count;
            while (i < count)
            {
                EventDelegate eventDelegate = list[i];
                if (eventDelegate != null && eventDelegate.Equals(ev))
                {
                    list.RemoveAt(i);
                    return true;
                }
                i++;
            }
        }
        return false;
    }

    [SerializeField]
    private MonoBehaviour mTarget;

    [SerializeField]
    private String mMethodName;

    [SerializeField]
    private EventDelegate.Parameter[] mParameters;

    public Boolean oneShot;

    [NonSerialized]
    private EventDelegate.Callback mCachedCallback;

    [NonSerialized]
    private Boolean mRawDelegate;

    [NonSerialized]
    private Boolean mCached;

    [NonSerialized]
    private MethodInfo mMethod;

    [NonSerialized]
    private ParameterInfo[] mParameterInfos;

    [NonSerialized]
    private Object[] mArgs;

    private static Int32 s_Hash = "EventDelegate".GetHashCode();

    [Serializable]
    public class Parameter
    {
        public Parameter()
        {
        }

        public Parameter(UnityEngine.Object obj, String field)
        {
            this.obj = obj;
            this.field = field;
        }

        public Parameter(Object val)
        {
            this.mValue = val;
        }

        public Object value
        {
            get
            {
                if (this.mValue != null)
                {
                    return this.mValue;
                }
                if (!this.cached)
                {
                    this.cached = true;
                    this.fieldInfo = (FieldInfo)null;
                    this.propInfo = (PropertyInfo)null;
                    if (this.obj != (UnityEngine.Object)null && !String.IsNullOrEmpty(this.field))
                    {
                        Type type = this.obj.GetType();
                        this.propInfo = type.GetProperty(this.field);
                        if (this.propInfo == null)
                        {
                            this.fieldInfo = type.GetField(this.field);
                        }
                    }
                }
                if (this.propInfo != null)
                {
                    return this.propInfo.GetValue(this.obj, null);
                }
                if (this.fieldInfo != null)
                {
                    return this.fieldInfo.GetValue(this.obj);
                }
                if (this.obj != (UnityEngine.Object)null)
                {
                    return this.obj;
                }
                if (this.expectedType != null && this.expectedType.IsValueType)
                {
                    return null;
                }
                return Convert.ChangeType(null, this.expectedType);
            }
            set
            {
                this.mValue = value;
            }
        }

        public Type type
        {
            get
            {
                if (this.mValue != null)
                {
                    return this.mValue.GetType();
                }
                if (this.obj == (UnityEngine.Object)null)
                {
                    return typeof(void);
                }
                return this.obj.GetType();
            }
        }

        public UnityEngine.Object obj;

        public String field;

        [NonSerialized]
        private Object mValue;

        [NonSerialized]
        public Type expectedType = typeof(void);

        [NonSerialized]
        public Boolean cached;

        [NonSerialized]
        public PropertyInfo propInfo;

        [NonSerialized]
        public FieldInfo fieldInfo;
    }

    public delegate void Callback();
}
