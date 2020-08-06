#if NETSTANDARD

namespace System.Runtime.Serialization
{
    using System.Diagnostics;

    [Conditional("DO_NOT_COMPILE")]
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class OnSerializingAttribute : Attribute
    {
    }
}

#endif
