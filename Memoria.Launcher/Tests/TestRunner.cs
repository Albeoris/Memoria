using System;

namespace Memoria.Launcher.Tests
{
    /// <summary>
    /// Simple test runner that can be called to verify the parseXmlIntoChunks functionality
    /// Run this to ensure the function correctly ignores mods inside XML comments
    /// </summary>
    public static class TestRunner
    {
        public static void RunParseXmlIntoChunksTest()
        {
            try
            {
                Console.WriteLine("Running parseXmlIntoChunks test...");
                ParseXmlIntoChunksTest.TestModInCommentIsIgnored();
                Console.WriteLine("✅ All tests passed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Demonstrates the parseXmlIntoChunks functionality with sample XML
        /// </summary>
        public static void DemonstrateParseXmlIntoChunks()
        {
            Console.WriteLine("Demonstrating parseXmlIntoChunks functionality...");
            
            String sampleXml = @"<?xml version=""1.0""?>
<ModCatalog>
    <!-- Commented out mod (should be ignored):
         <mod>
             <Name>DisabledMod</Name>
             <InstallationPath>disabled</InstallationPath>
         </mod>
    -->
    <mod>
        <Name>Active Mod 1</Name>
        <InstallationPath>active1</InstallationPath>
        <Description>This mod should be found</Description>
    </mod>
    <!-- <mod><Name>InlineCommentedMod</Name></mod> -->
    <mod>
        <Name>Active Mod 2</Name>
        <InstallationPath>active2</InstallationPath>
    </mod>
    <!-- End of catalog -->
</ModCatalog>";

            var result = Mod.ParseXmlIntoChunks(sampleXml);
            
            Console.WriteLine($"📊 Parsing results:");
            Console.WriteLine($"   - Found {result.ModChunks.Count} mod chunks");
            Console.WriteLine($"   - Found {result.Chunks.Count} text chunks");
            
            Console.WriteLine("\n📦 Mod chunks found:");
            for (int i = 0; i < result.ModChunks.Count; i++)
            {
                var modChunk = result.ModChunks[i];
                // Extract mod name for better display
                var nameMatch = System.Text.RegularExpressions.Regex.Match(modChunk.Content, @"<Name>(.*?)</Name>");
                String modName = nameMatch.Success ? nameMatch.Groups[1].Value : "Unknown";
                Console.WriteLine($"   {i + 1}. {modName} (chars {modChunk.StartIndex}-{modChunk.EndIndex})");
            }
            
            Console.WriteLine("\n📄 Text chunks (structure without mod content):");
            for (int i = 0; i < result.Chunks.Count; i++)
            {
                String preview = result.Chunks[i].Trim();
                if (preview.Length > 60)
                    preview = preview.Substring(0, 60) + "...";
                Console.WriteLine($"   Chunk {i + 1}: {preview}");
            }
            
            Console.WriteLine("\n✅ Demonstration complete. Note that commented mods were ignored.");
        }
    }
}