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

        public static Func<T1, T2, TResult> MakeStaticFunction<T1, T2, TResult>(MethodInfo method)
        {
            DynamicMethod dm = new DynamicMethod(method.Name + "_publicAccessor", typeof(TResult), new[] {typeof(T1), typeof(T2)}, typeof(Expressions), true);
            ILGenerator cg = dm.GetILGenerator();

            cg.Emit(OpCodes.Ldarg_0);
            cg.Emit(OpCodes.Ldarg_1);
            cg.Emit(OpCodes.Call, method);
            cg.Emit(OpCodes.Ret);

            return (Func<T1, T2, TResult>)dm.CreateDelegate(typeof(Func<T1, T2, TResult>));
        }
    }
}