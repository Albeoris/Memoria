# Memoria 
    Final Fantasy IX Engine

## Overview

Memoria Engine is a community rewrite of Final Fantasy IX's game engine that aims to add features, fix bugs and expand mod making capabilities (adding content, localizing...). Memoria includes a built-in Mod Manager, bugfixes and optional improvements (camera, framerate, audio, controller, UI, options, cheats...) while supporting third-party mods. [More info here](https://github.com/Albeoris/Memoria/wiki/Project-Overview)

## Features


- Main features:
    - **New launcher with easy options (for not included options: Memoria.ini)**
    - **Mod Manager/downloader**
    - **Widescreen (for all resolutions)**
    - **Highter framerate (60fps+)**
    - **Camera stabilizer (configurable)**
    - **Font change (includes original PSX font)**
    - **Smooth camera movement & texture scrolling**
    - **Anti-aliasing & Improved layer rendering**
    - **Many, many bugfixes**
- UI
    - **Battle speed change & Swirl skip**
    - **Turn-based mode / Simultaneous mode**
    - **Controller support with full analog movement**
    - **Battle UI layouts (includes original PSX layout)**
    - More items displayed at once
    - Volume control
- Support for:
    - Larger backgrounds definition (e.g. Moguri)
    - Unlocks FMV framerate change (e.g. Moguri)
    - Voice acting (e.g. WIP project ECHO-S)
    - Translations
    - Special effects modding (soon)
    - UI, shader, texture modding
    - Expanded features for mods
- Optional Cheats:
    - **Stealing 100% success**
    - Enable/Disable vanilla cheats
    - Easy minigames (rope, frogs, racing)
    - Excalibur II time limit removal
- Tetra Master (Card game):
    - **(Option) replace TM with a custom Triple Triad (FF8) or TM/TT hybrid**
    - Raise card limit
    - Auto discard cards
    - Randomness tweaking
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


## Configure

Most crucial options are embedded in the game launcher, more in-depth configuration is available in the file Memoria.ini generated in the game directory


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
