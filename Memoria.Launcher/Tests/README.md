# parseXmlIntoChunks Function

## Overview
The `parseXmlIntoChunks` function in the `Mod` class correctly parses XML content while ignoring `<mod>` tags that are inside XML comments (`<!-- ... -->`).

## Problem Solved
Previously, XML parsing would incorrectly treat `<mod>` tags inside comments as real mod entries. This function ensures that only actual mod tags outside of comments are processed.

## Usage

```csharp
using Memoria.Launcher;

// Parse XML content
string xmlContent = "..."; // Your XML content
var result = Mod.ParseXmlIntoChunks(xmlContent);

// Access the results
foreach (var modChunk in result.ModChunks)
{
    Console.WriteLine($"Found mod: {modChunk.Content}");
    Console.WriteLine($"Position: {modChunk.StartIndex}-{modChunk.EndIndex}");
}

// Access text chunks (XML structure without mod content)
foreach (var textChunk in result.Chunks)
{
    Console.WriteLine($"Text chunk: {textChunk}");
}
```

## Return Types

### `ParseResult`
- `List<string> Chunks`: Text chunks from the XML (everything except mod content)
- `List<ModChunk> ModChunks`: Mod chunks found outside of comments

### `ModChunk`
- `string Content`: The full `<mod>...</mod>` XML content
- `int StartIndex`: Starting position in the original XML
- `int EndIndex`: Ending position in the original XML

## Example

**Input XML:**
```xml
<?xml version="1.0"?>
<ModCatalog>
    <!-- This mod is commented out: <mod><Name>DisabledMod</Name></mod> -->
    <mod>
        <Name>ActiveMod</Name>
        <InstallationPath>active</InstallationPath>
    </mod>
</ModCatalog>
```

**Result:**
- `ModChunks.Count`: 1 (only "ActiveMod" is found)
- `Chunks.Count`: 2 (before and after the active mod)
- The commented "DisabledMod" is ignored

## Test Case
A comprehensive test is available in `Tests/ParseXmlIntoChunksTest.cs` that verifies:
- Mods inside comments are ignored
- Real mods outside comments are found
- Multiline comments work correctly
- Nested structures in comments are handled properly

## Implementation Details
1. **Comment Detection**: Uses regex `<!--.*?-->` with `RegexOptions.Singleline`
2. **Mod Detection**: Uses regex `<mod\b[^>]*>.*?</mod>` with case-insensitive matching
3. **Overlap Check**: Verifies each mod tag is not within any comment range
4. **Chunk Creation**: Splits text around found mod tags while preserving order