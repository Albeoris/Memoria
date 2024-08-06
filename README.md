# Memoria 
    Final Fantasy IX Engine

## Overview

Memoria Engine is an open-source community rewrite of Final Fantasy IX's engine that aims to add features, fix bugs and expand modding possibilities. It includes built-in improvements (QoL, camera, framerate, audio, controller, UI, options, cheats, shaders...), bugfixes, a mod manager/downloader and a model viewer. [More info here](https://github.com/Albeoris/Memoria/wiki/Project-Overview)

## Features


- Main features:
    - **New launcher with easy options (for not included options: Memoria.ini)**
    - **Mod Manager/downloader**, individual mod folders
    - **Widescreen (for all resolutions)**
    - **Higher framerate (60fps+)**
    - **Camera stabilizer (configurable)**
    - **Font change (includes original PSX font)**
    - **Smooth camera movement & texture scrolling**
    - **Anti-aliasing, texture filtering, Improved layer rendering, experimental shaders**
    - **Triple Triad** / Tetra Triad option
    - Better **sound engine**, volume control
    - **Many, many bugfixes**
- UI
    - **Battle speed change & Swirl skip**
    - **Turn-based mode / Simultaneous mode**
    - **Controller support with full analog movement**
    - **Battle UI layouts (includes original PSX layout)**
    - More items displayed at once
    - PSX disc change screens
    - Model Viewer
- Support for:
    - UI, backgrounds, shaders, texture, FMV modding/upscale (e.g. Moguri)
    - Voice acting (e.g. WIP project ECHO-S)
    - Translations
    - Expanded features for mods
    - Moddable game data (StreamingAssets\Data\) and abilities (StreamingAssets\Scripts\)
    - More character playable mods (e.g. Playable character pack, Tantalus...)
    - Import/export text/audio/textures
- Optional Cheats:
    - **Stealing 100% success**
    - Enable/Disable vanilla cheats
    - Easy minigames (rope, frogs, racing...)
    - Excalibur II time limit removal
    - Cards: lower randomness, card limit raised, auto discard cards
    - Manual Trance


## Install

- Download and run [Memoria.Patcher.exe](https://github.com/Albeoris/Memoria/releases/)

    > Automatically finds the game path from Windows registry (if you've launched the game once and haven't moved the install folder)
    > If not, launch the patcher from the game folder, or provide a custom path in command line: 'Memoria.Patcher.exe "gameDirectory"'


## Configure

- Most crucial options and cheats are embedded in the game launcher
- more in-depth configuration is available in the file Memoria.ini (in the game folder)


## Update

- Update the game to the **latest** version.
- Patch with the latest [Memoria.Patcher.exe](https://github.com/Albeoris/Memoria/releases/)


## Debug (for users)

- After first running the game, you should see "Memoria.ini" in the game directory.
- If something went wrong, there should be errors in "Memoria.log".
- If you can't see "Memoria.log", try running the game with administrator rights.
- "Sharing violation on path" error: close applications holding the file.
- "at Memoria.CsvReader.Read" error: delete files in (game)\StreamingAssets\Data and patch again.
- "at Memoria.ScriptsLoader.Initialize" error: delete files in (game)\StreamingAssets\Scripts and patch again.
- If an error persists, check "\FINAL FANTASY IX\x64(or x86)\FF9_Data\output_log.txt".

## DEVELOPERS

[INFO for developers](https://github.com/Albeoris/Memoria/wiki/Developer-instructions)


## Knowledge base

Please [visit our knowledge base](../../wiki#knowledge-base) before using this software.
