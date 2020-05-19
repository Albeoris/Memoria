using System;
using System.Collections.Generic;

namespace FF8.JSM
{
    public static partial class Jsm
    {
        public interface IJsmControl
        {
            IEnumerable<Segment> EnumerateSegments();
        }
    }
}