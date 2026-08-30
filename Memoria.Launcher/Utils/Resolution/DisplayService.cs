using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Memoria.Launcher.Utils
{
    public sealed class DisplayService
    {
        private static readonly Lazy<DisplayService> CurrentInstance = new(Create, LazyThreadSafetyMode.ExecutionAndPublication);

        public static DisplayService Current => CurrentInstance.Value;

        public IReadOnlyList<DisplayMonitor> Monitors { get; }
        public IReadOnlyList<DisplayResolution> SupportedResolutions { get; }
        public DisplayResolution PrimaryResolution { get; }

        private DisplayService(
            IList<DisplayMonitor> monitors,
            IList<DisplayResolution> supportedResolutions,
            DisplayResolution primaryResolution)
        {
            Monitors = new ReadOnlyCollection<DisplayMonitor>(monitors);
            SupportedResolutions = new ReadOnlyCollection<DisplayResolution>(supportedResolutions);
            PrimaryResolution = primaryResolution;
        }

        public Boolean TryGetMonitor(Int32 index, out DisplayMonitor monitor)
        {
            if (index >= 0 && index < Monitors.Count)
            {
                monitor = Monitors[index];
                return true;
            }

            monitor = null;
            return false;
        }

        private static DisplayService Create()
        {
            IReadOnlyDictionary<String, String> friendlyNames = NativeDisplayMethods.GetFriendlyNamesByDeviceName();
            List<NativeDisplayDevice> devices = NativeDisplayMethods
                .EnumerateActiveDisplayDevices()
                // Unity reserves display index zero for the primary display. Keep the launcher's
                // monitor indices aligned with that convention while preserving native order otherwise.
                .OrderByDescending(device => device.IsPrimary)
                .ToList();

            List<DisplayMonitor> monitors = new(devices.Count);
            HashSet<DisplayResolution> allResolutions = new();

            for (Int32 index = 0; index < devices.Count; index++)
            {
                NativeDisplayDevice device = devices[index];
                DisplayResolution currentResolution = DisplayResolution.Empty;
                DisplayBounds physicalBounds = default;

                if (NativeDisplayMethods.TryGetDisplayMode(device.DeviceName, NativeDisplayMethods.EnumCurrentSettings, out NativeDisplayMode currentMode))
                {
                    currentResolution = new DisplayResolution(currentMode.Width, currentMode.Height);
                    physicalBounds = new DisplayBounds(
                        currentMode.PositionX,
                        currentMode.PositionY,
                        currentMode.Width,
                        currentMode.Height);
                }

                List<DisplayResolution> supportedResolutions = GetSupportedResolutions(device.DeviceName);
                if (currentResolution.IsValid && !supportedResolutions.Contains(currentResolution))
                    supportedResolutions.Add(currentResolution);
                supportedResolutions.Sort();

                foreach (DisplayResolution resolution in supportedResolutions)
                    allResolutions.Add(resolution);

                String displayName = friendlyNames.TryGetValue(device.DeviceName, out String friendlyName)
                    ? friendlyName
                    : device.Name;
                if (String.IsNullOrWhiteSpace(displayName))
                    displayName = device.DeviceName;

                monitors.Add(new DisplayMonitor(
                    index,
                    device.DeviceName,
                    device.DeviceId,
                    displayName,
                    device.IsPrimary,
                    currentResolution,
                    physicalBounds,
                    supportedResolutions));
            }

            DisplayResolution primaryResolution = monitors.FirstOrDefault()?.CurrentResolution ?? DisplayResolution.Empty;
            if (!primaryResolution.IsValid && NativeDisplayMethods.TryGetDisplayMode(null, NativeDisplayMethods.EnumCurrentSettings, out NativeDisplayMode primaryMode))
            {
                primaryResolution = new DisplayResolution(primaryMode.Width, primaryMode.Height);
                allResolutions.Add(primaryResolution);
            }

            List<DisplayResolution> sortedResolutions = allResolutions.ToList();
            sortedResolutions.Sort();
            return new DisplayService(monitors, sortedResolutions, primaryResolution);
        }

        private static List<DisplayResolution> GetSupportedResolutions(String deviceName)
        {
            HashSet<DisplayResolution> resolutions = [];
            for (Int32 modeIndex = 0; NativeDisplayMethods.TryGetDisplayMode(deviceName, modeIndex, out NativeDisplayMode mode); modeIndex++)
            {
                if (mode.Width > 0 && mode.Height > 0)
                    resolutions.Add(new DisplayResolution(mode.Width, mode.Height));
            }

            return resolutions.ToList();
        }
    }
}
