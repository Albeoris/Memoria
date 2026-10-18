using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace Memoria.Launcher.Utils
{
    public sealed class SystemFontService
    {
        private const Int32 NormalWeight = 400;
        private const Int32 BoldWeight = 700;

        private static readonly Logger Log = AppLogger.GetLogger();
        private static readonly Lazy<SystemFontService> CurrentInstance = new(Create, LazyThreadSafetyMode.ExecutionAndPublication);
        private static readonly CultureInfo EnglishCulture = CultureInfo.GetCultureInfo("en-US");

        public static SystemFontService Current => CurrentInstance.Value;

        public IReadOnlyList<String> InstalledFontNames { get; }

        private SystemFontService(IList<String> installedFontNames)
        {
            InstalledFontNames = new ReadOnlyCollection<String>(installedFontNames);
        }

        private static SystemFontService Create()
        {
            try
            {
                List<SystemFontFace> faces = EnumerateSystemFontFaces();
                HashSet<String> names = BuildUnityFontNames(faces);
                List<String> sortedNames = names.ToList();
                sortedNames.Sort(StringComparer.CurrentCultureIgnoreCase);
                return new SystemFontService(sortedNames);
            }
            catch (Exception exception)
            {
                // Font selection is optional. Keep the launcher usable when a platform's
                // font subsystem cannot enumerate installed font faces.
                Log.Warn(exception, "Failed to enumerate installed system fonts.");
                return new SystemFontService(Array.Empty<String>());
            }
        }

        private static List<SystemFontFace> EnumerateSystemFontFaces()
        {
            String fontDirectory = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.Fonts))
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            List<SystemFontFace> faces = [];

            foreach (System.Windows.Media.FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                foreach (Typeface typeface in fontFamily.GetTypefaces())
                {
                    // This Unity version only exposes fonts installed for every Windows user.
                    // Per-user and packaged fonts would appear in WPF but fail in the game.
                    if (!typeface.TryGetGlyphTypeface(out GlyphTypeface glyphTypeface)
                        || !glyphTypeface.FontUri.IsFile
                        || !Path.GetFullPath(glyphTypeface.FontUri.LocalPath).StartsWith(fontDirectory, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    String familyName = GetEnglishName(glyphTypeface.FamilyNames, fontFamily.Source);
                    String faceName = GetEnglishName(glyphTypeface.Win32FaceNames, "Regular");
                    if (String.IsNullOrWhiteSpace(familyName) || String.IsNullOrWhiteSpace(faceName))
                        continue;

                    faces.Add(new SystemFontFace(
                        glyphTypeface.FontUri.AbsoluteUri,
                        familyName,
                        faceName,
                        typeface.Weight.ToOpenTypeWeight(),
                        typeface.Style,
                        typeface.Stretch));
                }
            }

            return faces;
        }

        private static HashSet<String> BuildUnityFontNames(IEnumerable<SystemFontFace> faces)
        {
            HashSet<String> result = new(StringComparer.OrdinalIgnoreCase);

            foreach (IGrouping<String, SystemFontFace> family in faces.GroupBy(face => face.FamilyName, StringComparer.OrdinalIgnoreCase))
            {
                Boolean hasStandardWeight = family.Any(face => face.Weight is NormalWeight or BoldWeight);
                IEnumerable<SystemFontFace> supportedFaces = hasStandardWeight
                    ? family.Where(face => face.Weight is NormalWeight or BoldWeight)
                    : family;

                foreach (IGrouping<String, SystemFontFace> fontFile in supportedFaces.GroupBy(face => face.FontUri, StringComparer.OrdinalIgnoreCase))
                {
                    // WPF exposes synthetic weights, styles, and stretches for a single font file.
                    // Unity exposes one installed name per physical face, so select its canonical form.
                    SystemFontFace face = fontFile
                        .OrderBy(GetWeightPriority)
                        .ThenBy(GetStylePriority)
                        .ThenBy(GetStretchPriority)
                        .First();

                    String name = face.FaceName is "Regular" or "Normal"
                        ? face.FamilyName
                        : $"{face.FamilyName} {face.FaceName}";
                    result.Add(name);
                }
            }

            return result;
        }

        private static String GetEnglishName(IDictionary<CultureInfo, String> names, String fallback)
        {
            if (names.TryGetValue(EnglishCulture, out String englishName))
                return englishName;
            if (names.TryGetValue(CultureInfo.CurrentCulture, out String currentName))
                return currentName;

            return names.Values.FirstOrDefault() ?? fallback;
        }

        private static Int32 GetWeightPriority(SystemFontFace face)
        {
            return face.Weight switch
            {
                NormalWeight => 0,
                BoldWeight => 1,
                _ => 2
            };
        }

        private static Int32 GetStylePriority(SystemFontFace face)
        {
            if (face.Style == FontStyles.Normal)
                return 0;
            if (face.Style == FontStyles.Italic)
                return 1;

            return 2;
        }

        private static Int32 GetStretchPriority(SystemFontFace face)
        {
            return face.Stretch == FontStretches.Normal ? 0 : 1;
        }

        private sealed class SystemFontFace
        {
            public String FontUri { get; }
            public String FamilyName { get; }
            public String FaceName { get; }
            public Int32 Weight { get; }
            public FontStyle Style { get; }
            public FontStretch Stretch { get; }

            public SystemFontFace(String fontUri, String familyName, String faceName, Int32 weight, FontStyle style, FontStretch stretch)
            {
                FontUri = fontUri;
                FamilyName = familyName;
                FaceName = faceName;
                Weight = weight;
                Style = style;
                Stretch = stretch;
            }
        }
    }
}
