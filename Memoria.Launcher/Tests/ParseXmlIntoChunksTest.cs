using System;
using System.Linq;

namespace Memoria.Launcher.Tests
{
    /// <summary>
    /// Unit test for the parseXmlIntoChunks function to verify it correctly
    /// ignores &lt;mod&gt; tags inside XML comments.
    /// This test ensures mods inside comments are not included in the result.
    /// </summary>
    public static class ParseXmlIntoChunksTest
    {
        /// <summary>
        /// Test case where &lt;mod&gt; appears in a comment to ensure it's ignored
        /// </summary>
        public static void TestModInCommentIsIgnored()
        {
            // Arrange: XML with mod inside comment and real mods outside
            String xmlWithCommentedMod = @"<?xml version=""1.0""?>
<ModCatalog>
    <!-- This is a commented mod that should be ignored: 
         <mod>
             <Name>CommentedMod</Name>
             <InstallationPath>this/should/be/ignored</InstallationPath>
         </mod>
    -->
    <mod>
        <Name>RealMod1</Name>
        <InstallationPath>real/mod/path1</InstallationPath>
    </mod>
    <!-- Another comment: <mod><Name>AnotherCommentedMod</Name></mod> -->
    <mod>
        <Name>RealMod2</Name>
        <InstallationPath>real/mod/path2</InstallationPath>
    </mod>
</ModCatalog>";

            // Act: Parse the XML into chunks
            var result = Mod.ParseXmlIntoChunks(xmlWithCommentedMod);

            // Assert: Verify only real mods are found, commented mods are ignored
            if (result.ModChunks.Count != 2)
            {
                throw new Exception($"Expected 2 mod chunks, but found {result.ModChunks.Count}");
            }

            // Verify the real mods are found
            Boolean foundRealMod1 = result.ModChunks.Any(chunk => chunk.Content.Contains("RealMod1"));
            Boolean foundRealMod2 = result.ModChunks.Any(chunk => chunk.Content.Contains("RealMod2"));

            if (!foundRealMod1)
            {
                throw new Exception("RealMod1 was not found in the parsed chunks");
            }

            if (!foundRealMod2)
            {
                throw new Exception("RealMod2 was not found in the parsed chunks");
            }

            // Verify commented mods are NOT found
            Boolean foundCommentedMod = result.ModChunks.Any(chunk => chunk.Content.Contains("CommentedMod"));
            Boolean foundAnotherCommentedMod = result.ModChunks.Any(chunk => chunk.Content.Contains("AnotherCommentedMod"));

            if (foundCommentedMod)
            {
                throw new Exception("CommentedMod was incorrectly included in the parsed chunks");
            }

            if (foundAnotherCommentedMod)
            {
                throw new Exception("AnotherCommentedMod was incorrectly included in the parsed chunks");
            }

            Console.WriteLine("✅ ParseXmlIntoChunks test passed: Mods inside comments are correctly ignored");
        }
    }
}