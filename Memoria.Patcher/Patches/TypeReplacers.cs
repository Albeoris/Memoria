using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Memoria.Patcher
{
    public static class TypeReplacers
    {
        private static readonly Dictionary<String, TypeHash> TypeRedirection = InitializeRedirection();

        private static Dictionary<String, TypeHash> InitializeRedirection()
        {
            return new Dictionary<String, TypeHash>
            {
                {"ScrollItemKeyNavigation", TypeHash.FromBase64(@"lU1N4QEAAADXMKF7CwAAADoveBBwOgHXKcb4ImNzNUYGW5rs26Pe0q4B9ug8JuWYorehrlcb1DISHQLJDAAAAL2yhLtTxoBiV4wmm8DAw4fuSsp55OwpzSQgVg5hpHrXAstMTJ7j8zi5xMvNTQOrCP////8=")},
                {"UIKeyTrigger", TypeHash.FromBase64(@"ah+ACQQAAACH727jE6zN16Y341KQ8vmDCAAAACQZPmdSswIp06FPbqUIODCf5m2ljE+yr8wNkCzhuhFRHgAAAI5gzqbsF6y6KgHgnnGDvTNLYBKaimPoL2+9VkgeKTh7ve4M7dxkZfBC/n8o7tcedv/LRQMwV72vsIAf7FOK96C3ykRFEpNyzxYSRnJQMQmW7mZ/TIGlwWU/5wXbG28/oOOX7jmKMPO8cjXB5eE0Co7wD+Cupx7pvf////8=")},
                {"SettingsState", TypeHash.FromBase64(@"ac2azAwAAABV4C9chTFClnW4G2z9Fihinfz+ImYXmas81ME73picwzZ3sUOeyrkOL3RyOKMUbvUJAAAAYSv7FGmH+pAbgquz2zScuOkWhojv5Y/gWF22dSsyl3PzSefWIAAAAOpoiCinLgXbQRjQiUdXy8vex7DnDV87IVtT5uflaP1hxLBrzn3DF3YqUcy3roy7JbgcZt3mECpXlFKcO+AB6FOuOZ9/RZ+3bY3WXeoBxpA+ssG2yL4g3DRhN0u6saVTAuleiGngLQBcvU4/NK/kYUru4+qjTy5xSfdBEZvvSHUfAwAAAMh1i2f/////AgAAAM+u0WvX7NlyAgAAAL9799ZqXrcl/////8PmSvb/////AwAAAFlhPKHPrtFr1+zZcgIAAAC/e/fWI6JkQv////9fCf1C/////wMAAADmcy+Vz67Ra9fs2XICAAAAv3v31nelVmv/////")},
                {"ConfigField", TypeHash.FromBase64(@"XQ9N6P////8GAAAAGbTDgJqPWOgW09K2kn5SeJoMBLLoASejAQAAAI0JNeT/////")},
                {"ControllerType", TypeHash.FromBase64(@"Pl37R/////8DAAAAsGfd/GX2yh5QAs51//////////8=")},
                {"SnapDragScrollView", TypeHash.FromBase64(@"zKrlFQEAAAAwDFfRCwAAAK4B9ujc6ljXjGJlmHqB5AKZihVA+YVZhLj+oZoZEpM0G+QHq1K084bKq+rpCQAAAPHbGO1cga6b659mRttDfyZWRO9DGcVt1u2+SFtNLZULIzyOQf////8=")},
                {"ConfigUI", TypeHash.FromBase64(@"Z0FRrf////80AAAAhW0tRAKL7qKUuRlOqVou2d9sn9HBe/hdXL3RZ8CIE63PbkXbqhJPi0dkIqeAXgbe/2HaRjJTDktQ8nYvxtGD5TIMStGKW+9oG3x6P1wKxdV5bAx//sMLQR5rolCgO89pvAX2NauMB8tvW5l3Kiof26cWeEgBNzuGyX1ge4dK42bdNH/bTzMeR40IT4MS4x4BCjGahbHQNfQgiEqxCTjO+8Iy6xpkG8fc5iLzCplgSj+fWQmaWokQqNgJWBhj3NRH+Nx8e195envu3Wd7jeZlezkAAAByk9VBe0HKI9aS3TVSH1NnLxWAJQ+gJuMDvrKNdyVoOmfhNRcOruVhRSiPBI+CGiFdUdGQ/V7nrejgqXLO3xBtZYtTEBPahfRccRdF/rqt/VJGv29KAIrWuK/rQpGIDiJw+eKwObqCeyDlMjgDGGPS/RaNO57I+0EUtVHaUI2oEWRSqHO+BByLrjnP9tw61H2SWXa+VLdDzVDTMmuvznEYazJ5P8jkp7/4jpTN+we3ipLTqAH1mExib9OhADKp0Yvs9ROgDKGCG8HmWkV6F+C54E9ySLcQLZlYaBk8L9rUOIJpL7gGAAAASMPkIv////8SAAAAsGfd/OcBNBYopi72IrhSnvwixAhnjJrZ345KFTxf5JefimtV6WICf+Ff0zoNET2fFQM/3RzFDIZ/IA8stPrx86yR3ANrY8lF//////////8i3L45AgAAAHz/Ran0cIpPAwAAAJlf/d96yzXRoR8Z7gYAAAC/e/fWJ88QVL+nRfq/CozFRByNoEeATTj/////mT1YFf////8BAAAA+xyChgIAAAC/e/fWZTSrEv////8P1k1K/////wEAAAD7HIKGAgAAAL9799bp9gMP/////ygSdYj/////AQAAAPscgoYCAAAAv3v31ijwCLH/////8uAg2v////8BAAAAbpOFbwIAAAC/e/fWKvaSQv////8=")},
                {"QuitUI", TypeHash.FromBase64(@"VOr/2/////8NAAAA6IQHy/gOmuJcCsXVN9QVKUsOtF5W6MzmnLYIf3SyThk6rDRDDJHYGSOG9sI2RnlHm3A84AsAAAB2lXJ2/uYIMSEg3G80OvZd7J4BeldC9fR0gNy1b2OHUUmDirmDgTzl3f/kMf////8=")},
                {"UIAtlas", TypeHash.FromBase64(@"bqmo4wYAAACWOeIs7ZryjvSK8FqbuunhPCdjazUYXwgJAAAAejba86Hj2KUiCODSTeXSwTScJc6mZg1bAT/p1vCZROevw5JfFgAAAPqkraizM0Fj3NBScnlEHELODuvJbXKcuXH4vUxBG1G0vusAlDCKqM0xDiFcYQSQ7poGUH1/iuobVKxG8knnMtMw+kivDrH0x+fmx1tz3+1Dx86IvcGqgIICAAAA6sUWwgEAAAB4orfUCAAAAPNlxwXEPffK91Xq4zw/j3AF6ma93tM+cQWWg7Qrk8GRAgAAAI53JQNqdgbb/////0r8+kL/////AwAAALBn3fxQ0QYxHBg+rv//////////")},
                {"UISprite", TypeHash.FromBase64(@"EJ3mFwsAAADvNNrziBnfmEJ2Lq6zSBgIfUAWbV408dA8J2Nr24OJ/LSg5A+J9mcB7ZryjgUAAAD6k5yIvFfoPvd+jrfVBxB5KtNYwRUAAADW+F6KeEbscHpZizRjdOjamEGmjlknm51BHrSLi5fjNXj19rQ6NVdl2y71kWpIds9gMaaaSEBMs+DFE4cwHFN6nIKRqnozvVIgNssLMUTWPlek3aP/////")},
                {"BGSCENE_DEF", TypeHash.FromBase64(@"EuCY6/////8mAAAA7JPbC1QzHpOMam5tKxesH3d0Qe0eR0SYKNgzwUvnyDgPj/ZW5owlZsfb7H8fsQbH0X81q2oCgdX15aGbmExScSPqXfwVS0lb9lIOUriz/TKzApvMRmlPovNlxwUMvFshOfa5EyNYOWVZqG/SkbpzgfKGbFctK4iqI/jw5ieG/m9JchGZKGAsnAs4ATRCc3Y0QuNLbN/c76oQAAAABByFPROj80GcahIZKbh4OXhH6pKXAOfWMkce1tmFW4Bi1u/b5zIMdaHaa+Qc7YABCuNCHgGRPPzbrMdOztOrT/////8=")},
                {"ff9play", TypeHash.FromBase64(@"1MrW0f////8KAAAA2mqqyj8FnBrIzQ+X5ZsnWheSuGlwNr7RNZjqRQeYDlhyS9VpnqAiQRsAAAC/e/fW1BPhiIhQbLY/YO2lOOyA3OcbkK29LLNr3F2GnQ1pH+BAsqsXvDcslXL/fzmpv3Zk3B3URKeF6W4Wdx24CNlToLjpgGjRYQBxXuqQJ3kRx/HBnnv52WEWjWqkzJL2PCksGkE7HoOAkYsBAAAAh3abj/////8EAAAAR8wxF/EW7IAO2CtlCkR0jQEAAAD99JQL/////w==")},
                {"PartySettingUI", TypeHash.FromBase64(@"FQRurgEAAAAqbOfEEwAAABepBVH97b5uhmQ0D8nPNiyKW+9oL0ZNr6Oa9ySb7GsfG3x6P8gIlkgY3+2Gt8CwbMez+4reN89B3Q59rJ+/lN8ia5SvYcaRrxBsn68WAAAAmjpbiwy7VC0+5na5st2lxQxClBhVblYKgZlsYjp6ehsxRvomiUka7WhrSbr+TYoXrPOrLi1pP5Y6QWx4sqUsiZodinllkCkUEVR18WVAV28TXbDt6i/PAwQAAACUfKt4/////wQAAACwZ938n7FWSc4TBaBAPypW///////////2bEUj/////wIAAAB3ErLlno3CNQEAAAC0QNhv/////8wtFSX/////BAAAAKaJd8OZ1SAvtqGlfp6NwjUBAAAAciDAbP////+LPgIX/////wIAAAC3VHJ43RlG6AEAAADb3JuZ/////w==")},
                // Battle
                {"Assets.Sources.Scripts.Common.DebugGuiSkin", TypeHash.FromBase64(@"1fEIt/////8EAAAARaZ1x7nbCvvwIBmlUbvG+wgAAADwj4WVcES09eOCsVSWdxxTmUVtbUzPwxfF3WI/pdiJdf////8=")},
                {"HonoluluBattleMain", TypeHash.FromBase64(@"MopSwgIAAABjb9QrCB5r+iAAAACZpaPBlzquDSx7H7APCPogztPv7EWF8H2vX3U5/iFt3T30ZiizA7Br+j7m90/auJiZt4CBfahNOoY7vUMsuVyAyAWSGKdY947dwdfy0ytkqO+UScfqPgz06RtHMWvW84wwOJOlVUhtzHTA/bYM81jXgXbMlc6DN6BolB1jxpbCThkAAACW/6S6yYKzxhM0HmNwdGIRRYhfz/AueeDW1IwvVFh24FkC/Y9PCoSakd8qOBzav5buqNASSa1Kr3EHWJd9ABCIMHVWT8Nc8R83fbyntIc8KuLS4WajNw1dS2XBnrAuS2Roi3lhAQAAAHOkWpj/////AwAAALBn3fzfFppq3NmMYP//////////")},
                {"AssetManager", TypeHash.FromBase64(@"ksjRp/////8EAAAATy9Ki4DsBKUocsjdnAFPdQcAAACZrathj7Wk655tyuJcZ8XWatYDjI5j1zGtmCtxAQAAAIMQQ4T/////AwAAAP9/O+K5jNx1R6tSxwEAAACjYDIL/////w==")}
            };
        }

        public static void RedirectTypes(AssemblyDefinition mod, AssemblyDefinition victim)
        {
            Boolean errorOccured = false;

            ModuleDefinition victimModule = victim.MainModule;
            ModuleDefinition modModule = mod.MainModule;
            AssemblyNameReference modReference = victimModule.AssemblyReferences.First(r => r.FullName == mod.MainModule.Assembly.Name.FullName);

            Dictionary<String, TypeDefinition> oldTypes = GetRedirectionTypes(victim);

            foreach (KeyValuePair<String, TypeDefinition> pair in oldTypes)
            {
                String typeName = pair.Key;
                TypeDefinition oldType = pair.Value;

                TypeHash currentHash = new TypeHash(oldType);
                TypeHash knownHash = TypeRedirection[typeName];

                String error = currentHash.Compare(knownHash);
                if (!String.IsNullOrEmpty(error))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Victim type was changed. Update the Game and Memoria to the last version.");
                    sb.Append("Fullname: ").AppendLine(typeName);
                    sb.Append("Current hash: ").AppendLine(currentHash.ToBase64());
                    sb.Append("Error: ").AppendLine(error);
                    String message = sb.ToString();

                    Log.Warning(message);
                    Console.Error.WriteLine(message);
                    Console.Error.WriteLine();
                    errorOccured = true;
                }
            }

            if (errorOccured)
                throw new Exception("Failed to export types. See Memoria.log for details.");

            Dictionary<String, TypeDefinition> newTypes = GetRedirectionTypes(mod);

            foreach (KeyValuePair<String, TypeDefinition> pair in oldTypes)
            {
                String typeName = pair.Key;
                TypeDefinition oldType = pair.Value;
                TypeDefinition newType;
                if (!newTypes.TryGetValue(typeName, out newType))
                {
                    String message = $"Redirected type was not found: {typeName}";
                    Console.Error.WriteLine(message);
                    Log.Warning(message);
                    continue;
                }

                ReplaceType(victimModule, newType, oldType, modModule, modReference);
                ExportedType exportedType = new ExportedType(oldType.Namespace, oldType.Name, modModule, modReference);
                victimModule.Types.Remove(oldType);
                victimModule.ExportedTypes.Add(exportedType);
            }
        }

        private static void ReplaceType(ModuleDefinition victimModule, TypeDefinition newType, TypeDefinition oldType, ModuleDefinition modModule, AssemblyNameReference modReference)
        {
            TypeReference newReference = victimModule.Import(newType);

            if (oldType.HasNestedTypes)
            {
                foreach (TypeDefinition oldNested in oldType.NestedTypes)
                {
                    if (oldNested.IsNestedPrivate)
                        continue;

                    TypeDefinition newNested = newType.NestedTypes.First(t => t.Name == oldNested.Name);
                    ReplaceType(victimModule, newNested, oldNested, modModule, modReference);
                }
            }

            new TypeReplacer(oldType.FullName, newReference).Replace(victimModule);
        }

        private static Dictionary<String, TypeDefinition> GetRedirectionTypes(AssemblyDefinition assembly)
        {
            Dictionary<String, TypeDefinition> oldTypes = new Dictionary<String, TypeDefinition>(TypeRedirection.Count);
            foreach (TypeDefinition type in assembly.MainModule.Types)
            {
                if (TypeRedirection.ContainsKey(type.FullName))
                    oldTypes.Add(type.FullName, type);
            }
            return oldTypes;
        }
    }
}