# Memoria
Final Fantasy IX Engine

# Updates
1. Update the game to the **latest** version.
2. Update Memoria to the **latest** version.
3. **Delete** "Memoria.ini" from the game directory (otherwise you will not see new settings).
4. Patch again


# Build
1. Use Visual Studio 2019 or JetBrains Rider.
2. Copy "*.dll" from "\FINAL FANTASY IX\x64\FF9_Data\Managed" to the "\References" folder in the solution directory.
3. Resture NuGet packages manualy or enable automaticaly package downloads.


# Patch:
1. Memoria.Patcher.exe - will patch game files using current directory or path from the windows registry
2. Memoria.Patcher.exe gameDirectory - will patch game files using a provided path


# First run:
1. Run game.
2. If there is no error you will see "Memoria.ini" file in the game directory.
3. If something went wrong you will see error in the "Memoria.log"
4. If you not see "Memoria.log" try to run game with administrator rights
5. If you see "Sharing violation on path" then close applications that hold this file
6. If you see "at Memoria.CsvReader.Read" then fix files in the StreamingAssets\Data directory or delete them and patch again.
7. If you see "at Memoria.ScriptsLoader.Initialize" then fix files in the StreamingAssets\Scripts directory and recompile scripts or delete them and patch again.
8. If the error persists see "\FINAL FANTASY IX\x64(or x86)\FF9_Data\output_log.txt"


# Features:
1. Widescreen support (just select any 16:9 resolution)
2. Disable/Enable cheats
3. Fast battles (Change FPS, a Swirl duration, skip waiting or turn-based)
4. Save/Load anywhere (Alt+F5, Alt+F9) 
5. Edit game data (look at the "StreamingAssets\Data" folder)
6. Change ability mechanics (look at the "StreamingAssets\Scripts" folder)
7. All of the characters available (Alt+F2 to show a party window) [be careful](https://github.com/Albeoris/Memoria/issues/3)!
8. Easy rope jumping, frog catching, hippaul racing
9. Auto discard cards
10. Change the game font
11. [Change audio volume](https://github.com/Albeoris/Memoria/issues/36) (Ctrl+Alt+Shift+M to show configuration menu)
12. Export/import text resources
13. Export/import audio resources (Ctrl+Alt+Shift+S to debug)
14. [Bypass the Launcher](https://github.com/Albeoris/Memoria/issues/70) 


# Configuration:
1. Open Memoria.ini
2. Change "Enabled" value from "0" to "1" for what you need
3. Specify other params.
4. Save your edits and try run the game.


# Scripting (for modmakers)
1. Make a copy of an existing item from the "StreamingAssets\Scripts" folder.
2. Change a namespace to your own.
3. Make some changes.
4. Run Memoria.Compiler.exe from the "Compiler".
5. Run the game, test all of you can and see Memoria.log and output_log.txt for errors.

Now you can change mechanics of battle actions. In the future I will add more scriptable entries.
Also you can use a Visual Studio project from the "Project" folder. It will load every .cs file from the "Sources\Battle" folder.
Be careful - future updates could remove your changes. Please make your own copies if it possible.
https://www.youtube.com/watch?v=cU4T3GSIjxs


# Restrictions (for developers)
1. **Please** don't change any data that can be sent to the game server! We don't want any trouble.
2. Don't change a serializable data that can be deserialized by the Unity Engine. The game will crash or corrupt.


# Debugging (for developers)
1. Check the "Debuggable" checkbox in the Launcher.
2. Attach to the game process (Debug -> Attach Unity Debugger in the Visual Studio 2015/2017 Tools for Unity)


# Knowledge base
Please [visit our knowledge base](../../wiki#knowledge-base) before using this software.
