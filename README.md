# Memoria 
    Final Fantasy IX Engine
    
## Install
- Download and run [Memoria.Patcher.exe](https://github.com/Albeoris/Memoria/releases/)

    > Automatically find the game path from Windows registry or current directory, you can provide a custom path as argument:
    > Memoria.Patcher.exe gameDirectory

## Update

- Update the game to the **latest** version.
- Patch with the latest [Memoria.Patcher.exe](https://github.com/Albeoris/Memoria/releases/)

## Features

- New launcher with easy options (for not included options: Memoria.ini)
- Mod Manager, with integrated community catalog and 1 click install
- Support for:
    - Larger backgrounds resolution (e.g. Moguri)
    - Widescreen (select any 16:9 resolution)
    - Configurable FPS 
    - Higher FMV framerate (e.g. Moguri)
    - Voice acting (e.g. WIP project ECHO-S)
    - Font change (includes original PSX font)
- Cheats:
    - Enable/Disable vanilla cheats
    - Stealing 100% rate
    - Easy minigames (rope, frogs, racing)
- Faster battles:
    - Speed change
    - Swirl duration
    - Waiting skip
    - Turn-based mode
- QoL:
    - Save/load anywhere (Alt+F5, Alt+F9)
    - Full analog movement
    - Auto discard cards
    - [LauncherBypass](https://github.com/Albeoris/Memoria/issues/70#issuecomment-626077188)
    - [Change audio volume](https://github.com/Albeoris/Memoria/issues/36#issuecomment-626098739) (Ctrl+Alt+Shift+M to show configuration menu)
- Edit game data (look at the "StreamingAssets\Data" folder)
- Change ability mechanics (look at the "StreamingAssets\Scripts" folder)
- Make every character available (Alt+F2 to show a party window)
- Export/import text/audio resources (Ctrl+Alt+Shift+S to debug)

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

# Build
1. Use Visual Studio 2019 or JetBrains Rider.
2. Copy "*.dll" from "\FINAL FANTASY IX\x64\FF9_Data\Managed" to the "\References" folder in the solution directory.
3. Restore NuGet packages manually or enable automatically package downloads.


# Scripting
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
