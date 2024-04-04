# Memoria 
    Final Fantasy IX Engine


## Features

- New launcher with easy options (for not included options: Memoria.ini)
- Mod Manager, with integrated community catalog and 1 click install
- Optional features:
    - Battle UI layouts (includes original PSX layout)
    - Configurable framerate (15, 30, 60fps...)
    - Font change (includes original PSX font)
    - Volume control
    - Anti-aliasing
    - Full analog movement
- Support for:
    - Larger backgrounds definition (e.g. Moguri)
    - Widescreen (now for all resolutions)
    - Unlocks FMV framerate change (e.g. Moguri)
    - Voice acting (e.g. WIP project ECHO-S)
    - Many limitations removed for mods
    - Some bugfixes
- Faster battles:
    - Speed change
    - Swirl duration
    - Waiting skip
    - Turn-based mode
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
- In Visual Studio, install ".NET Framework 3.5 Development Tools" & "Visual Studio Tools for Unity" (in Tools > Get Tools and Functionalities > Individual Components <>)
- Install [.NET Framework 3.5 SP1 and 4.6.2](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks?cid=getdotnetsdk).
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
