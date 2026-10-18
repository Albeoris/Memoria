#nullable disable
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Memoria.Launcher.Utils.ModValidation
{
    [DataContract]
    internal sealed class ModFixChangelogEntry
    {
        [DataMember(Order = 1)] public String Timestamp { get; set; }
        [DataMember(Order = 2)] public String RelativePath { get; set; }
        [DataMember(Order = 3)] public String FileSha256 { get; set; }
        [DataMember(Order = 4)] public String FixerName { get; set; }
        [DataMember(Order = 5)] public List<ModFixChange> Changes { get; set; } = new();
    }
}
