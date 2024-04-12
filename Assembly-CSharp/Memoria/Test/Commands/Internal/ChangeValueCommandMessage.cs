using System;
using System.Linq;
using System.Reflection;
using Object = UnityEngine.Object;

namespace Memoria.Test
{
	internal sealed partial class ChangeValueCommandMessage : CommandMessage
	{
		public override void Execute()
		{
			const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			Object[] objects = Object.FindObjectsOfType<Object>();
			Object obj = objects.FirstOrDefault(o => o.GetInstanceID() == InstanceId);
			if (obj == null)
				return;

			Type type = obj.GetType();
			MemberInfo[] members = type.GetMember(MemberName, bindingFlags);
			MemberInfo member = members.Single();

			PropertyInfo property = member as PropertyInfo;
			if (property != null)
			{
				property.SetValue(obj, Value.Object, null);
				return;
			}

			FieldInfo field = member as FieldInfo;
			if (field != null)
			{
				field.SetValue(obj, Value.Object);
				return;
			}

			throw new NotSupportedException(member.GetType().FullName);
		}
	}
}
