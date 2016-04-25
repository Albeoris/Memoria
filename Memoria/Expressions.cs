using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Memoria
{
    public static class Expressions
    {
        public static Action<TValue> MakeStaticSetter<TValue>(FieldInfo field)
        {
            DynamicMethod method = new DynamicMethod(field.Name + "_publicSetter", typeof(void), new[] {typeof(TValue)}, typeof(Expressions), true);
            ILGenerator cg = method.GetILGenerator();

            cg.Emit(OpCodes.Ldarg_0);
            cg.Emit(OpCodes.Stsfld, field);
            cg.Emit(OpCodes.Ret);

            return (Action<TValue>)method.CreateDelegate(typeof(Action<TValue>));
        }

        public static Func<TValue> MakeStaticGetter<TValue>(FieldInfo field)
        {
            DynamicMethod method = new DynamicMethod(field.Name + "_publicGetter", typeof(TValue), null, typeof(Expressions), true);
            ILGenerator cg = method.GetILGenerator();

            cg.Emit(OpCodes.Ldsfld, field);
            cg.Emit(OpCodes.Ret);

            return (Func<TValue>)method.CreateDelegate(typeof(Func<TValue>));
        }
    }
}