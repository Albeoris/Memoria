#nullable disable
using System;
using System.Runtime.Serialization;

namespace Memoria.Launcher.Utils.ModValidation
{
    [DataContract]
    internal sealed class ModFixChange
    {
        public ModFixChange()
        {
        }

        public ModFixChange(Int32 fileLine, String before, String after)
        {
            FileLine = fileLine;
            Before = before ?? throw new ArgumentNullException(nameof(before));
            After = after ?? throw new ArgumentNullException(nameof(after));
        }

        [DataMember(Order = 1)] public Int32 FileLine { get; set; }
        [DataMember(Order = 2)] public String Before { get; set; }
        [DataMember(Order = 3)] public String After { get; set; }
    }
}
