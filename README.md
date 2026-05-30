![memoria-logo.jpg](https://github.com/user-attachments/assets/625396cc-7553-4607-9626-9f98917d6073)

### The Memoria engine is an open-source community rewrite of Final Fantasy IX's engine that aims to add features, fix bugs and expand modding possibilities. It includes built-in improvements (QoL, camera, framerate, audio, controller, UI, options, cheats, shaders...), bugfixes, a mod manager/downloader and a model viewer. [More info here](https://github.com/Albeoris/Memoria/wiki/Project-Overview)

Note: This is **not** affiliated with the Memoria Project, a 2023 Proof-of-concept tribute render in Unreal Engine

[![Download Latest Release](https://img.shields.io/badge/Download%20Latest%20Release-006400?logo=github)](https://github.com/Albeoris/Memoria/releases/latest)

## Features

- Main features:
    - Easy to use, everything optional, main settings in the launcher, the rest in Memoria.ini
    - **Mod Manager/downloader**, using individual mod folders
    - **Widescreen (for all resolutions)**
    - **Higher framerate (60fps+)**
    - **Smooth and stabilized camera movement**
    - **Improved rendering**: Anti-aliasing, texture filtering, layer edges, shaders
    - **Triple Triad** / Tetra Triad option
    - Better **sound engine**, volume control
    - **Many, many bugfixes**
- UI
    - **Font change (includes the original PSX font)**
    - **Battle speed change & Swirl skip**
    - **Turn-based mode / Simultaneous mode**
    - **Controller support with full analog movement**
    - **Battle UI layouts (includes original PSX layout)**
    - More items displayed at once
    - PSX disc change screens
    - Model Viewer
- Modding support for:
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

![screenshots.jpg](https://github.com/user-attachments/assets/2bacaa4c-c380-44a8-bc67-9814594154d0)

## Install

- [Latest stable release](https://github.com/Albeoris/Memoria/releases/latest)
- [Latest canary release](https://github.com/Albeoris/Memoria/releases/tag/canary)

### Windows

- Download `Memoria.Patcher.exe` from which ever release channel you prefer
- Copy the file inside your game directory (next to `FF9_Launcher.exe` )
- Double click `Memoria.Patcher.exe`
- Launch the game and enjoy

### Linux

> **PLEASE NOTE:**
>
> If you prefer to use an automated approach, this gist is also available:
>
> ```
> bash -c "$(curl -sL https://gist.githubusercontent.com/dotaxis/1ad1c64baa7ad9c1dabcb255ea6257ae/raw/memoria.sh)"
> ```

- Download `Memoria.Patcher-linux-x64` from which ever release channel you prefer
- Copy the file inside your game directory (next to `FF9_Launcher.exe` )
- Open a terminal session in your game path ( or `cd` to its path )
- `chmod +x Memoria.Patcher-linux-x64 && ./Memoria.Patcher-linux-x64`
- Launch the game and enjoy

### MacOS

> **PLEASE NOTE:**
>
> If you prefer to use an automated approach, SummonKit provides an easy way to install and run Memoria. See https://github.com/julianxhokaxhiu/SummonKit

- Download `Memoria.Patcher-osx-x64` from which ever release channel you prefer
- Copy the file inside your game directory (next to `FF9_Launcher.exe` )
- Open a terminal session in your game path ( or `cd` to its path )
- `chmod +x Memoria.Patcher-osx-x64 && ./Memoria.Patcher-osx-x64`
- Launch the game and enjoy

## Update

- Memoria: Use the same steps described in [Install](#install).
- Mods: The Mod Manager will show a yellow dot if some of your mods are not up-to-date

## Documentation

- [Info for developers](../../wiki#Developers)
- [Knowledge base](../../wiki#knowledge-base)
- [Memoria.ini](../../wiki/Memoria.ini-sections)
- Most crucial options and cheats are embedded in the game launcher
- More in-depth configuration is available in the file `Memoria.ini` (in the game folder)

## 🚀 Build

### Visual Studio

0. Download the the latest [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/) installer
1. Run the installer and import this [.vsconfig](.vsconfig) file in the installer to pick the required components to build this project
2. Install the game ( [Steam](https://store.steampowered.com/app/377840/FINAL_FANTASY_IX/) or [GOG](https://www.gog.com/en/game/final_fantasy_ix), whichever you prefer )
3. Open the file [`Memoria.sln`](Memoria.sln) in Visual Studio and click the build button
4. Click on `Start` to launch the `Memoria.Launcher` in the game directory

