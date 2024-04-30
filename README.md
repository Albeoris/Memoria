# Memoria 
    Final Fantasy IX Engine

## Project Overview

This project, dubbed "Final Fantasy IX Engine", is a complete rewrite of the game's engine in C#. It maintains binary compatibility with the native parts of the game and the serialized data within the game archives, allowing the compiled version to be used in place of the original DLLs. 

The primary goal of this project is to provide mod makers with a convenient framework for developing gameplay mods, replacing game assets, expand original content, localizing the game into native languages, and fixing bugs of the original.

Memoria addresses numerous bugs in the existing game port across various aspects like rendering, audio, and abilities. It includes built-in mods (turn-based battles, gameplay mechanics changes, a new card game system, cheat disablers, etc.) and provides a framework for integrating third-party mods, supporting import/export and modification of game resources.

The engine integrates seamlessly with Unity, allowing the use of features ranging from loading external AssetBundles to manual manipulation of GameObjects and Components. However, many game elements like the event interpreter or animation pipelines emulate the PSX assembler, meaning significant modifications such as importing custom models, shaders, sounds, etc., require creating workarounds to either transform external data into the game's native format or completely alter the approach by transforming game resources into compatible formats and implementing custom rendering mechanisms.

The engine introduces a new audio API, extending the native engine with the dynamic library `soloud.dll`, enhancing audio capabilities.

Memoria includes multiple components:
- **Assembly-CSharp**, **Memoria.Prime**, and **Memoria.Scripts**: These handle the core game code and battle script assembly.
- **Memoria.Compiler**: Allows users to modify **Memoria.Scripts** without an IDE.
- **Memoria.XInputDotNetPure** and **UnityEngine.UI**: Extend original assemblies by providing new APIs.
- **Memoria.MSBuild**: Aids in build process automation, including patcher packaging and dependency deployment.
- **Memoria.Debugger** and **Memoria.Injection**: Enable debugging by injecting into the game process and establishing a debugging server.
- **Memoria.Client**: Facilitates game object manipulation via a GUI similar to UnityExplorer (Alt+Ctrl+Shift+F12 to start in-game server).
- **Memoria.Patcher**: A custom self-extracting archive used for Memoria installation.
- **Memoria.Launcher** and **Memoria.SteamFix**: Components of an alternative launcher that allows use it without disabling the Steam Overlay.

## Game Ownership Requirement

To use this engine, owning a licensed copy of the game is necessary as it requires several native DLLs and a set of original assets to function properly. This ensures compatibility with all APIs and serialized data, essential for correct operation with the game's scenes and other assets.

## Features

- New launcher with easy options (for not included options: Memoria.ini)
- Mod Manager, with integrated community catalog and 1 click install
- Smooth camera movement
- Optional features:
    - Battle UI layouts (includes original PSX layout)
    - Configurable framerate (15, 30, 60fps...)
    - Configurable camera stabilizer
    - Font change (includes original PSX font)
    - Volume control
    - Anti-aliasing
    - Full analog movement
- Support for:
    - Larger backgrounds definition (e.g. Moguri)
    - Widescreen (Supports 16:9 and 16:10)
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


## Debug
- After first running the game, you should see "Memoria.ini" in the game directory.
- If something went wrong, there will be errors in "Memoria.log".
- If you can't see "Memoria.log", try running the game with administrator rights.
- "Sharing violation on path" error: close applications holding the file.
- "at Memoria.CsvReader.Read" error: delete files in (game)\StreamingAssets\Data and patch again.
- "at Memoria.ScriptsLoader.Initialize" error: delete files in (game)\StreamingAssets\Scripts and patch again.
- If an error persists, check "\FINAL FANTASY IX\x64(or x86)\FF9_Data\output_log.txt".


-----


# DEVELOPERS


## Build & Contribute
- Use Visual Studio Community (2019 or above). Install .NET dev tools when prompted.
- Install [.NET Framework 4.7.2 SDK](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks?cid=getdotnetsdk).
  - .NET Framework 3.5 SP1 SDK is available as a component of [Visual Studio](https://visualstudio.microsoft.com/downloads). See next step.
- In Visual Studio, install ".NET Framework 3.5 Development Tools" & "Visual Studio Tools for Unity" (in Tools > Get Tools and Functionalities > Individual Components <>)
- Make a fork of the project and download it locally.
- Open Powershell as administrator and execute `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope LocalMachine`
- Change directory to your Memoria Fork download location and run powershell script `.\SetupProjectEnvironment.ps1`
- Once you've commited your changes to your fork, make it a Pull Request to the main repository.
- I recommend Github Desktop for easy push to repository.


## Scripting
1. Make a copy of an existing item from the "StreamingAssets\Scripts" folder.
2. Change a namespace to your own.
3. Make some changes.
4. Run Memoria.Compiler.exe from the "Compiler".
5. Run the game, test what you need and see Memoria.log and output_log.txt for errors.

Now you can change mechanics of battle actions. In the future, I will add more scriptable entries.
Also, you can use a Visual Studio project from the "Project" folder. It will load every .cs file from the "Sources\Battle" folder.
Be careful - future updates could remove your changes. Please make your own copies if it possible.
https://www.youtube.com/watch?v=cU4T3GSIjxs


## Restrictions
1. **Please** don't change any data that can be sent to the game server! We don't want any trouble.
2. Don't change a serializable data that can be deserialized by the Unity Engine. The game will crash or corrupt.


## Debug
1. Check the "Debuggable" checkbox in the Launcher.
2. Attach to the game process (Debug -> Attach Unity Debugger in the Visual Studio 2015/2017 Tools for Unity)


## Knowledge base
Please [visit our knowledge base](../../wiki#knowledge-base) before using this software.
