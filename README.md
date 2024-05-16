# Memoria 
    Final Fantasy IX Engine

## Overview

Memoria Engine (for Final Fantasy IX's Steam version) is a C# rewrite of the game's engine, maintaining compatibility with original game components and data. Its main aim is to aid mod makers in developing mods, expanding content, localizing the game, and fixing bugs. Memoria fixes existing game bugs and includes built-in improvements (camera, framerate, audio, controller, UI, options, cheats...) while supporting third-party mods. You need to own the game on Steam to run it. [More info here](https://github.com/Albeoris/Memoria/wiki/Project-Overview)

## Features

- New launcher with easy options (for not included options: Memoria.ini)
- Mod Manager, with integrated community catalog and 1 click install
- Smooth camera movement, texture scrolling...
- Optional features:
    - Battle UI layouts (includes original PSX layout)
    - Highter framerate (60fps+)
    - Camera stabilizer (configurable)
    - Font change (includes original PSX font)
    - Volume control
    - Anti-aliasing
    - Full analog movement and controller support
- Support for:
    - Larger backgrounds definition (e.g. Moguri)
    - Widescreen (16:9 or 16:10)
    - Unlocks FMV framerate change (e.g. Moguri)
    - Voice acting (e.g. WIP project ECHO-S)
    - Many limitations removed for mods
    - Some bugfixes
- Faster battles:
    - Speed change
    - Swirl duration / skip
    - Turn-based mode / Waiting skip mode / Simultaneous attacks mode
- Optional Cheats:
    - Stealing 100% success
    - Enable/Disable vanilla cheats
    - Easy minigames (rope, frogs, racing)
    - Excalibur II time limit removal
- Tetra Master (Card game):
    - (Option) replace with a custom version of Triple Triad (FFVIII) or Tetra Master/Triple Triad hybrid
    - Raise card limit
    - Auto discard cards
    - Ways to tweak the randomness
- Include individual mod assets in folders
- Edit game data (look at the "StreamingAssets\Data" folder)
- Change ability mechanics (look at the "StreamingAssets\Scripts" folder)
- Make every character available (Alt+F2)
- Export/import text/audio resources (Ctrl+Alt+Shift+S to debug)


## Install

- Download and run [Memoria.Patcher.exe](https://github.com/Albeoris/Memoria/releases/)

    > Automatically finds the game path from Windows registry or current directory, you can provide a custom path as argument:
    > Memoria.Patcher.exe gameDirectory
- Note: if you want Moguri Mod, use Memoria patcher after.


## Update

- Update the game to the **latest** version.
- Patch with the latest [Memoria.Patcher.exe](https://github.com/Albeoris/Memoria/releases/)


## Debug (for users)

- After first running the game, you should see "Memoria.ini" in the game directory.
- If something went wrong, there will be errors in "Memoria.log".
- If you can't see "Memoria.log", try running the game with administrator rights.
- "Sharing violation on path" error: close applications holding the file.
- "at Memoria.CsvReader.Read" error: delete files in (game)\StreamingAssets\Data and patch again.
- "at Memoria.ScriptsLoader.Initialize" error: delete files in (game)\StreamingAssets\Scripts and patch again.
- If an error persists, check "\FINAL FANTASY IX\x64(or x86)\FF9_Data\output_log.txt".

## DEVELOPERS

[INFO for developers](https://github.com/Albeoris/Memoria/wiki/Developer-instructions)


## Knowledge base

Please [visit our knowledge base](../../wiki#knowledge-base) before using this software.
