using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Memoria.Launcher.Utils
{
    public sealed class DisplayMonitor
    {
        public Int32 Index { get; }
        public String DeviceName { get; }
        public String DeviceId { get; }
        public String Name { get; }
        public Boolean IsPrimary { get; }
        public DisplayResolution CurrentResolution { get; }
        public DisplayBounds PhysicalBounds { get; }
        public IReadOnlyList<DisplayResolution> SupportedResolutions { get; }

        internal DisplayMonitor(
            Int32 index,
            String deviceName,
            String deviceId,
            String name,
            Boolean isPrimary,
            DisplayResolution currentResolution,
            DisplayBounds physicalBounds,
            IList<DisplayResolution> supportedResolutions)
        {
            Index = index;
            DeviceName = deviceName ?? String.Empty;
            DeviceId = deviceId ?? String.Empty;
            Name = name ?? String.Empty;
            IsPrimary = isPrimary;
            CurrentResolution = currentResolution;
            PhysicalBounds = physicalBounds;
            SupportedResolutions = new ReadOnlyCollection<DisplayResolution>(new List<DisplayResolution>(supportedResolutions));
        }
    }
}
