using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Memoria.Launcher.Utils
{
    internal static class NativeDisplayMethods
    {
        internal const Int32 EnumCurrentSettings = -1;
        private const UInt32 QueryOnlyActivePaths = 0x00000002;
        private const Int32 ErrorSuccess = 0;
        private const Int32 ErrorInsufficientBuffer = 122;
        private const Int32 MaxDisplayConfigAttempts = 3;

        [Flags]
        internal enum DisplayDeviceStateFlags : UInt32
        {
            None = 0,
            AttachedToDesktop = 0x00000001,
            PrimaryDevice = 0x00000004,
            MirroringDriver = 0x00000008
        }

        internal static IReadOnlyDictionary<String, String> GetFriendlyNamesByDeviceName()
        {
            try
            {
                return QueryFriendlyNamesByDeviceName();
            }
            catch (DllNotFoundException)
            {
                return new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
            }
            catch (EntryPointNotFoundException)
            {
                return new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
            }
        }

        internal static IEnumerable<NativeDisplayDevice> EnumerateActiveDisplayDevices()
        {
            for (UInt32 deviceIndex = 0; ; deviceIndex++)
            {
                DisplayDevice adapter = DisplayDevice.Create();
                if (!EnumDisplayDevices(null, deviceIndex, ref adapter, 0))
                    yield break;

                Boolean isAttached = (adapter.StateFlags & DisplayDeviceStateFlags.AttachedToDesktop) != 0;
                Boolean isMirroringDriver = (adapter.StateFlags & DisplayDeviceStateFlags.MirroringDriver) != 0;
                if (!isAttached || isMirroringDriver)
                    continue;

                DisplayDevice monitor = DisplayDevice.Create();
                Boolean hasMonitorDetails = EnumDisplayDevices(adapter.DeviceName, 0, ref monitor, 0);

                yield return new NativeDisplayDevice(
                    adapter.DeviceName,
                    hasMonitorDetails ? monitor.DeviceId : adapter.DeviceId,
                    hasMonitorDetails ? monitor.DeviceString : adapter.DeviceString,
                    (adapter.StateFlags & DisplayDeviceStateFlags.PrimaryDevice) != 0);
            }
        }

        internal static Boolean TryGetDisplayMode(String deviceName, Int32 modeIndex, out NativeDisplayMode mode)
        {
            // Do not derive pixel dimensions from GetMonitorInfo/EnumDisplayMonitors rectangles.
            // Those rectangles belong to the desktop coordinate space and can be DPI-virtualized.
            // This is particularly visible under Wine: a DPI-unaware process on a 4K display at
            // 200% scaling can receive a 1920x1080 rectangle. EnumDisplaySettings is explicitly
            // defined in physical pixels and works for both native Windows and Wine display drivers.
            DevMode nativeMode = DevMode.Create();
            if (!EnumDisplaySettings(deviceName, modeIndex, ref nativeMode))
            {
                mode = default;
                return false;
            }

            mode = new NativeDisplayMode(
                nativeMode.PositionX,
                nativeMode.PositionY,
                checked((Int32)nativeMode.PelsWidth),
                checked((Int32)nativeMode.PelsHeight));
            return true;
        }

        private static IReadOnlyDictionary<String, String> QueryFriendlyNamesByDeviceName()
        {
            Dictionary<String, String> result = new(StringComparer.OrdinalIgnoreCase);

            for (Int32 attempt = 0; attempt < MaxDisplayConfigAttempts; attempt++)
            {
                Int32 error = GetDisplayConfigBufferSizes(QueryOnlyActivePaths, out UInt32 pathCount, out UInt32 modeCount);
                if (error != ErrorSuccess)
                    return result;

                DisplayConfigPathInfo[] paths = new DisplayConfigPathInfo[pathCount];
                DisplayConfigModeInfo[] modes = new DisplayConfigModeInfo[modeCount];
                error = QueryDisplayConfig(
                    QueryOnlyActivePaths,
                    ref pathCount,
                    paths,
                    ref modeCount,
                    modes,
                    IntPtr.Zero);

                // Display topology may change between the size query and the data query.
                // Retrying is the documented way to handle that race.
                if (error == ErrorInsufficientBuffer)
                    continue;
                if (error != ErrorSuccess)
                    return result;

                for (Int32 pathIndex = 0; pathIndex < pathCount; pathIndex++)
                {
                    DisplayConfigSourceDeviceName sourceName = DisplayConfigSourceDeviceName.Create(paths[pathIndex].SourceInfo);
                    DisplayConfigTargetDeviceName targetName = DisplayConfigTargetDeviceName.Create(paths[pathIndex].TargetInfo);
                    if (DisplayConfigGetDeviceInfo(ref sourceName) != ErrorSuccess
                        || DisplayConfigGetDeviceInfo(ref targetName) != ErrorSuccess
                        || String.IsNullOrWhiteSpace(sourceName.GdiDeviceName)
                        || String.IsNullOrWhiteSpace(targetName.MonitorFriendlyDeviceName))
                    {
                        continue;
                    }

                    result[sourceName.GdiDeviceName] = targetName.MonitorFriendlyDeviceName;
                }

                return result;
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "EnumDisplayDevicesW", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean EnumDisplayDevices(
            String deviceName,
            UInt32 deviceIndex,
            ref DisplayDevice displayDevice,
            UInt32 flags);

        [DllImport("user32.dll", EntryPoint = "EnumDisplaySettingsW", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean EnumDisplaySettings(
            String deviceName,
            Int32 modeIndex,
            ref DevMode deviceMode);

        [DllImport("user32.dll")]
        private static extern Int32 GetDisplayConfigBufferSizes(
            UInt32 flags,
            out UInt32 pathCount,
            out UInt32 modeCount);

        [DllImport("user32.dll")]
        private static extern Int32 QueryDisplayConfig(
            UInt32 flags,
            ref UInt32 pathCount,
            [Out] DisplayConfigPathInfo[] paths,
            ref UInt32 modeCount,
            [Out] DisplayConfigModeInfo[] modes,
            IntPtr currentTopologyId);

        [DllImport("user32.dll", EntryPoint = "DisplayConfigGetDeviceInfo")]
        private static extern Int32 DisplayConfigGetDeviceInfo(ref DisplayConfigSourceDeviceName request);

        [DllImport("user32.dll", EntryPoint = "DisplayConfigGetDeviceInfo")]
        private static extern Int32 DisplayConfigGetDeviceInfo(ref DisplayConfigTargetDeviceName request);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DisplayDevice
        {
            public Int32 Size;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public String DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public String DeviceString;
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public String DeviceId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public String DeviceKey;

            public static DisplayDevice Create()
            {
                return new DisplayDevice { Size = Marshal.SizeOf<DisplayDevice>() };
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DevMode
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public String DeviceName;
            public UInt16 SpecVersion;
            public UInt16 DriverVersion;
            public UInt16 Size;
            public UInt16 DriverExtra;
            public UInt32 Fields;
            public Int32 PositionX;
            public Int32 PositionY;
            public UInt32 DisplayOrientation;
            public UInt32 DisplayFixedOutput;
            public Int16 Color;
            public Int16 Duplex;
            public Int16 YResolution;
            public Int16 TTOption;
            public Int16 Collate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public String FormName;
            public UInt16 LogPixels;
            public UInt32 BitsPerPixel;
            public UInt32 PelsWidth;
            public UInt32 PelsHeight;
            public UInt32 DisplayFlags;
            public UInt32 DisplayFrequency;
            public UInt32 IcmMethod;
            public UInt32 IcmIntent;
            public UInt32 MediaType;
            public UInt32 DitherType;
            public UInt32 Reserved1;
            public UInt32 Reserved2;
            public UInt32 PanningWidth;
            public UInt32 PanningHeight;

            public static DevMode Create()
            {
                return new DevMode { Size = checked((UInt16)Marshal.SizeOf<DevMode>()) };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigLuid
        {
            public UInt32 LowPart;
            public Int32 HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigPathSourceInfo
        {
            public DisplayConfigLuid AdapterId;
            public UInt32 Id;
            public UInt32 ModeInfoIndex;
            public UInt32 StatusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigPathTargetInfo
        {
            public DisplayConfigLuid AdapterId;
            public UInt32 Id;
            public UInt32 ModeInfoIndex;
            public UInt32 OutputTechnology;
            public UInt32 Rotation;
            public UInt32 Scaling;
            public DisplayConfigRational RefreshRate;
            public UInt32 ScanLineOrdering;
            [MarshalAs(UnmanagedType.Bool)] public Boolean TargetAvailable;
            public UInt32 StatusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigRational
        {
            public UInt32 Numerator;
            public UInt32 Denominator;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigPathInfo
        {
            public DisplayConfigPathSourceInfo SourceInfo;
            public DisplayConfigPathTargetInfo TargetInfo;
            public UInt32 Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigRegion
        {
            public UInt32 Width;
            public UInt32 Height;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigVideoSignalInfo
        {
            public UInt64 PixelRate;
            public DisplayConfigRational HorizontalSyncFrequency;
            public DisplayConfigRational VerticalSyncFrequency;
            public DisplayConfigRegion ActiveSize;
            public DisplayConfigRegion TotalSize;
            public UInt32 VideoStandard;
            public UInt32 ScanLineOrdering;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigTargetMode
        {
            public DisplayConfigVideoSignalInfo TargetVideoSignalInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigPoint
        {
            public Int32 X;
            public Int32 Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigSourceMode
        {
            public UInt32 Width;
            public UInt32 Height;
            public UInt32 PixelFormat;
            public DisplayConfigPoint Position;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct DisplayConfigModeInfoUnion
        {
            [FieldOffset(0)] public DisplayConfigTargetMode TargetMode;
            [FieldOffset(0)] public DisplayConfigSourceMode SourceMode;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigModeInfo
        {
            public UInt32 InfoType;
            public UInt32 Id;
            public DisplayConfigLuid AdapterId;
            public DisplayConfigModeInfoUnion ModeInfo;
        }

        private enum DisplayConfigDeviceInfoType : UInt32
        {
            GetSourceName = 1,
            GetTargetName = 2
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DisplayConfigDeviceInfoHeader
        {
            public DisplayConfigDeviceInfoType Type;
            public UInt32 Size;
            public DisplayConfigLuid AdapterId;
            public UInt32 Id;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DisplayConfigSourceDeviceName
        {
            public DisplayConfigDeviceInfoHeader Header;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public String GdiDeviceName;

            public static DisplayConfigSourceDeviceName Create(DisplayConfigPathSourceInfo source)
            {
                DisplayConfigSourceDeviceName result = default;
                result.Header.Type = DisplayConfigDeviceInfoType.GetSourceName;
                result.Header.Size = checked((UInt32)Marshal.SizeOf<DisplayConfigSourceDeviceName>());
                result.Header.AdapterId = source.AdapterId;
                result.Header.Id = source.Id;
                return result;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DisplayConfigTargetDeviceName
        {
            public DisplayConfigDeviceInfoHeader Header;
            public UInt32 Flags;
            public UInt32 OutputTechnology;
            public UInt16 EdidManufactureId;
            public UInt16 EdidProductCodeId;
            public UInt32 ConnectorInstance;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public String MonitorFriendlyDeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public String MonitorDevicePath;

            public static DisplayConfigTargetDeviceName Create(DisplayConfigPathTargetInfo target)
            {
                DisplayConfigTargetDeviceName result = default;
                result.Header.Type = DisplayConfigDeviceInfoType.GetTargetName;
                result.Header.Size = checked((UInt32)Marshal.SizeOf<DisplayConfigTargetDeviceName>());
                result.Header.AdapterId = target.AdapterId;
                result.Header.Id = target.Id;
                return result;
            }
        }
    }

    internal sealed class NativeDisplayDevice
    {
        public String DeviceName { get; }
        public String DeviceId { get; }
        public String Name { get; }
        public Boolean IsPrimary { get; }

        public NativeDisplayDevice(String deviceName, String deviceId, String name, Boolean isPrimary)
        {
            DeviceName = deviceName ?? String.Empty;
            DeviceId = deviceId ?? String.Empty;
            Name = name ?? String.Empty;
            IsPrimary = isPrimary;
        }
    }

    internal readonly struct NativeDisplayMode
    {
        public Int32 PositionX { get; }
        public Int32 PositionY { get; }
        public Int32 Width { get; }
        public Int32 Height { get; }

        public NativeDisplayMode(Int32 positionX, Int32 positionY, Int32 width, Int32 height)
        {
            PositionX = positionX;
            PositionY = positionY;
            Width = width;
            Height = height;
        }
    }
}
