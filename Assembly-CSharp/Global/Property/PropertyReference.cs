using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

[Serializable]
public class PropertyReference
{
	public PropertyReference()
	{
	}

	public PropertyReference(Component target, String fieldName)
	{
		this.mTarget = target;
		this.mName = fieldName;
	}

	public Component target
	{
		get
		{
			return this.mTarget;
		}
		set
		{
			this.mTarget = value;
			this.mProperty = (PropertyInfo)null;
			this.mField = (FieldInfo)null;
		}
	}

	public String name
	{
		get
		{
			return this.mName;
		}
		set
		{
			this.mName = value;
			this.mProperty = (PropertyInfo)null;
			this.mField = (FieldInfo)null;
		}
	}

	public Boolean isValid
	{
		get
		{
			return this.mTarget != (UnityEngine.Object)null && !String.IsNullOrEmpty(this.mName);
		}
	}

	public Boolean isEnabled
	{
		get
		{
			if (this.mTarget == (UnityEngine.Object)null)
			{
				return false;
			}
			MonoBehaviour monoBehaviour = this.mTarget as MonoBehaviour;
			return monoBehaviour == (UnityEngine.Object)null || monoBehaviour.enabled;
		}
	}

	public Type GetPropertyType()
	{
		if (this.mProperty == null && this.mField == null && this.isValid)
		{
			this.Cache();
		}
		if (this.mProperty != null)
		{
			return this.mProperty.PropertyType;
		}
		if (this.mField != null)
		{
			return this.mField.FieldType;
		}
		return typeof(void);
	}

	public override Boolean Equals(Object obj)
	{
		if (obj == null)
		{
			return !this.isValid;
		}
		if (obj is PropertyReference)
		{
			PropertyReference propertyReference = obj as PropertyReference;
			return this.mTarget == propertyReference.mTarget && String.Equals(this.mName, propertyReference.mName);
		}
		return false;
	}

	public override Int32 GetHashCode()
	{
		return PropertyReference.s_Hash;
	}

	public void Set(Component target, String methodName)
	{
		this.mTarget = target;
		this.mName = methodName;
	}

	public void Clear()
	{
		this.mTarget = (Component)null;
		this.mName = (String)null;
	}

	public void Reset()
	{
		this.mField = (FieldInfo)null;
		this.mProperty = (PropertyInfo)null;
	}

	public override String ToString()
	{
		return PropertyReference.ToString(this.mTarget, this.name);
	}

	public static String ToString(Component comp, String property)
	{
		if (!(comp != (UnityEngine.Object)null))
		{
			return (String)null;
		}
		String text = comp.GetType().ToString();
		Int32 num = text.LastIndexOf('.');
		if (num > 0)
		{
			text = text.Substring(num + 1);
		}
		if (!String.IsNullOrEmpty(property))
		{
			return text + "." + property;
		}
		return text + ".[property]";
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public Object Get()
	{
		if (this.mProperty == null && this.mField == null && this.isValid)
		{
			this.Cache();
		}
		if (this.mProperty != null)
		{
			if (this.mProperty.CanRead)
			{
				return this.mProperty.GetValue(this.mTarget, null);
			}
		}
		else if (this.mField != null)
		{
			return this.mField.GetValue(this.mTarget);
		}
		return null;
	}

	[DebuggerStepThrough]
	[DebuggerHidden]
	public Boolean Set(Object value)
	{
		if (this.mProperty == null && this.mField == null && this.isValid)
		{
			this.Cache();
		}
		if (this.mProperty == null && this.mField == null)
		{
			return false;
		}
		if (value == null)
		{
			try
			{
				if (this.mProperty == null)
				{
					this.mField.SetValue(this.mTarget, null);
					return true;
				}
				if (this.mProperty.CanWrite)
				{
					this.mProperty.SetValue(this.mTarget, null, null);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}
		if (!this.Convert(ref value))
		{
			if (Application.isPlaying)
			{
				global::Debug.LogError(String.Concat(new Object[]
				{
					"Unable to convert ",
					value.GetType(),
					" to ",
					this.GetPropertyType()
				}));
			}
		}
		else
		{
			if (this.mField != null)
			{
				this.mField.SetValue(this.mTarget, value);
				return true;
			}
			if (this.mProperty.CanWrite)
			{
				this.mProperty.SetValue(this.mTarget, value, null);
				return true;
			}
		}
		return false;
	}

	[DebuggerStepThrough]
	[DebuggerHidden]
	private Boolean Cache()
	{
		if (this.mTarget != (UnityEngine.Object)null && !String.IsNullOrEmpty(this.mName))
		{
			Type type = this.mTarget.GetType();
			this.mField = type.GetField(this.mName);
			this.mProperty = type.GetProperty(this.mName);
		}
		else
		{
			this.mField = (FieldInfo)null;
			this.mProperty = (PropertyInfo)null;
		}
		return this.mField != null || this.mProperty != (PropertyInfo)null;
	}

	private Boolean Convert(ref Object value)
	{
		if (this.mTarget == (UnityEngine.Object)null)
		{
			return false;
		}
		Type propertyType = this.GetPropertyType();
		Type from;
		if (value == null)
		{
			if (!propertyType.IsClass)
			{
				return false;
			}
			from = propertyType;
		}
		else
		{
			from = value.GetType();
		}
		return PropertyReference.Convert(ref value, from, propertyType);
	}

	public static Boolean Convert(Type from, Type to)
	{
		Object obj = null;
		return PropertyReference.Convert(ref obj, from, to);
	}

	public static Boolean Convert(Object value, Type to)
	{
		if (value == null)
		{
			value = null;
			return PropertyReference.Convert(ref value, to, to);
		}
		return PropertyReference.Convert(ref value, value.GetType(), to);
	}

	public static Boolean Convert(ref Object value, Type from, Type to)
	{
		if (to.IsAssignableFrom(from))
		{
			return true;
		}
		if (to == typeof(String))
		{
			value = ((value == null) ? "null" : value.ToString());
			return true;
		}
		if (value == null)
		{
			return false;
		}
		Single num2;
		if (to == typeof(Int32))
		{
			if (from == typeof(String))
			{
				Int32 num;
				if (Int32.TryParse((String)value, out num))
				{
					value = num;
					return true;
				}
			}
			else if (from == typeof(Single))
			{
				value = Mathf.RoundToInt((Single)value);
				return true;
			}
		}
		else if (to == typeof(Single) && from == typeof(String) && Single.TryParse((String)value, out num2))
		{
			value = num2;
			return true;
		}
		return false;
	}

	[SerializeField]
	private Component mTarget;

	[SerializeField]
	private String mName;

	private FieldInfo mField;

	private PropertyInfo mProperty;

	private static Int32 s_Hash = "PropertyBinding".GetHashCode();
}
