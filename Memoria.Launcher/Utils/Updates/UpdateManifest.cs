using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Memoria.Launcher.Utils.Updates
{
    [DataContract]
    internal sealed class UpdateManifest
    {
        public const Int32 CurrentSchemaVersion = 1;

        [DataMember(Order = 1)]
        public Int32 SchemaVersion { get; set; }
        [DataMember(Order = 2)]
        public String BuildTimeUtc { get; set; } = String.Empty;
        [DataMember(Order = 3)]
        public Int64 PatcherSize { get; set; }

        public static UpdateManifest Parse(String json)
        {
            if (String.IsNullOrWhiteSpace(json))
                throw new FormatException("The update manifest is empty.");

            UpdateManifest manifest;
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                if (!(new DataContractJsonSerializer(typeof(UpdateManifest)).ReadObject(stream) is UpdateManifest parsedManifest))
                    throw new FormatException("The update manifest could not be deserialized.");
                manifest = parsedManifest;
            }
            if (manifest.SchemaVersion != CurrentSchemaVersion)
                throw new FormatException($"Unsupported update manifest schema version: {manifest.SchemaVersion}.");
            if (manifest.PatcherSize <= 0)
                throw new FormatException($"Invalid patcher size in the update manifest: {manifest.PatcherSize}.");
            if (!DateTimeOffset.TryParse(manifest.BuildTimeUtc, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out DateTimeOffset buildTime))
                throw new FormatException($"Invalid build time in the update manifest: '{manifest.BuildTimeUtc}'.");

            manifest.BuildTime = buildTime.UtcDateTime;
            return manifest;
        }

        public DateTime BuildTime { get; private set; }
    }
}
