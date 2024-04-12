using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Memoria.Scenes
{
	internal abstract class GOBase
	{
		public readonly GameObject GameObject;
		public Transform Transform => GameObject.transform;

		protected GOBase(GameObject obj)
		{
			GameObject = obj;
		}

		public Boolean IsActive
		{
			get { return GameObject.activeSelf; }
			set { GameObject.SetActive(value); }
		}

		public static implicit operator GameObject(GOBase self)
		{
			return self.GameObject;
		}

		public static T Create<T>(GameObject obj) where T : GOBase
		{
			return ConstructorCache<T>.Constructor(obj);
		}

		private static class ConstructorCache<T> where T : GOBase
		{
			public static readonly Func<GameObject, T> Constructor = CreateConstructor();

			private static Func<GameObject, T> CreateConstructor()
			{
				ParameterExpression param = Expression.Parameter(TypeCache<GameObject>.Type, "obj");
				ConstructorInfo ctor = TypeCache<T>.Type.GetConstructor(new[] { TypeCache<GameObject>.Type });
				Expression<Func<GameObject, T>> lambda = Expression.Lambda<Func<GameObject, T>>(Expression.New(ctor, param), param);
				return lambda.Compile();
			}
		}

		public virtual IEnumerable<IEnumerable<String>> EnumerateLines()
		{
			yield return ComponentLogger.EnumerateLines(nameof(Transform), Transform.EnumerateSetters());
		}
	}

	internal static class ComponentLogger
	{
		public static IEnumerable<String> EnumerateLines(String memberName, IEnumerable<String> setters)
		{
			StringBuilder sb = new StringBuilder(memberName);
			Int32 offset = sb.Length;

			foreach (String setter in setters)
			{
				sb.AppendLine(setter);
				yield return sb.ToString();
				sb.Length = offset;
			}
		}

		public static IEnumerable<String> EnumerateSetters(this Transform c)
		{
			yield return Set(nameof(c.hideFlags), F(c.hideFlags));
			yield return Set(nameof(c.localEulerAngles), F(c.localEulerAngles));
			yield return Set(nameof(c.localPosition), F(c.localPosition));
			yield return Set(nameof(c.localRotation), F(c.localRotation));
			yield return Set(nameof(c.localScale), F(c.localScale));
			yield return Set(nameof(c.localToWorldMatrix), F(c.localToWorldMatrix));
			yield return Set(nameof(c.name), F(c.name));
			yield return Set(nameof(c.position), F(c.position));
			yield return Set(nameof(c.rotation), F(c.rotation));
			yield return Set(nameof(c.tag), F(c.tag));
			yield return Set(nameof(c.worldToLocalMatrix), F(c.worldToLocalMatrix));
		}

		private static String Set(String memberName, String formattedValue)
		{
			return '.' + Init(memberName, formattedValue) + ';';
		}

		private static String Init(String memberName, String formattedValue)
		{
			return memberName + " = " + formattedValue;
		}

		private static String F(String value)
		{
			return "@\"" + value.Replace("\"", "\"\"") + '"';
		}

		private static String F(Int32 value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		private static String F(Single value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		private static String F(Vector3 value)
		{
			return $"new Vector3({F(value.x)}, {F(value.y)}, {F(value.z)})";
		}

		private static String F(Quaternion value)
		{
			return $"new Quaternion({F(value.x)}, {F(value.y)}, {F(value.z)}, {F(value.w)})";
		}

		private static String F(Matrix4x4 value)
		{
			StringBuilder sb = new StringBuilder(256);
			sb.Append("new Matrix4x4 {");
			for (Int32 c = 0; c < 4; c++)
			{
				for (Int32 r = 0; r < 4; r++)
				{
					sb.Append(Init('m' + F((Single)r) + F((Single)c), F(value[r, c])));
					sb.Append(", ");
				}
			}
			sb.Length -= 2;
			sb.Append('}');
			return sb.ToString();
		}

		private static String F(HideFlags value)
		{
			return "(HideFlags)" + F((Int32)value);
		}
	}
}
