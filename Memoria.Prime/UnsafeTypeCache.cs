using System;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Memoria.Prime
{
    public static class UnsafeTypeCache<T>
    {
        public static readonly Int32 UnsafeSize = GetSize();
        public static readonly UIntPtr ArrayTypePointer = GetArrayTypePointer();

        private static Int32 GetSize()
        {
            DynamicMethod dynamicMethod = new DynamicMethod("SizeOf", typeof(Int32), Type.EmptyTypes);
            ILGenerator generator = dynamicMethod.GetILGenerator();

            generator.Emit(OpCodes.Sizeof, TypeCache<T>.Type);
            generator.Emit(OpCodes.Ret);

            return ((Func<int>)dynamicMethod.CreateDelegate(typeof(Func<Int32>)))();
        }

        private static unsafe UIntPtr GetArrayTypePointer()
        {
            T[] result = new T[1];
            using (SafeGCHandle handle = new SafeGCHandle(result, GCHandleType.Pinned))
                return *(((UIntPtr*)handle.AddrOfPinnedObject().ToPointer()) - 2);
        }

        public static IDisposable ChangeArrayTypes(Array array, Int32 oldElementSize)
        {
            unsafe
            {
                void* pinned;
                return ChangeArrayType(array, oldElementSize, out pinned);
            }
        }

        public static unsafe IDisposable ChangeArrayType(Array array, Int32 oldElementSize, out void* pointer)
        {
            if (array.Length < 1)
                throw new NotSupportedException();

            SafeGCHandle handle = new SafeGCHandle(array, GCHandleType.Pinned);
            try
            {
                pointer = handle.AddrOfPinnedObject().ToPointer();
                UIntPtr* arrayPointer = (UIntPtr*)pointer;
                UIntPtr arrayLength = *(arrayPointer - 1);
                UIntPtr arrayType = *(arrayPointer - 2);
                UInt64 arraySize = ((UInt64)arrayLength * (UInt64)oldElementSize);

                if (arraySize % (UInt64)UnsafeSize != 0)
                    throw new InvalidCastException();

                try
                {
                    *(arrayPointer - 1) = new UIntPtr(arraySize / (UInt64)UnsafeSize);
                    *(arrayPointer - 2) = ArrayTypePointer;

                    return new DisposableAction(() =>
                    {
                        *(arrayPointer - 1) = arrayLength;
                        *(arrayPointer - 2) = arrayType;
                        handle.Dispose();
                    });
                }
                catch
                {
                    *(arrayPointer - 1) = arrayLength;
                    *(arrayPointer - 2) = arrayType;
                    throw;
                }
            }
            catch
            {
                handle.SafeDispose();
                throw;
            }
        }

        public sealed class SafeGCHandle : SafeHandle
        {
            public SafeGCHandle(object target, GCHandleType type)
                : base(IntPtr.Zero, true)
            {
                SetHandle(GCHandle.ToIntPtr(GCHandle.Alloc(target, type)));
            }

            public GCHandle Handle
            {
                get { return GCHandle.FromIntPtr(handle); }
            }

            public override bool IsInvalid
            {
                get { return handle == IntPtr.Zero; }
            }

            protected override bool ReleaseHandle()
            {
                try
                {
                    GCHandle.FromIntPtr(handle).Free();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public IntPtr AddrOfPinnedObject()
            {
                return Handle.AddrOfPinnedObject();
            }
        }
    }
}
