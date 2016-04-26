# Memoria
Final Fantasy IX tools

# Updates
1. Update the game to the last version.
2. Update Memoria to the last version.
3. Delete "Memoria.ini" from the game directory.
4. **[IMPORTANT]** If "\FINAL FANTASY IX\x64(or x86)\FF9_Data\Managed\Assembly-CSharp.dll" was changed by update...
4.1 Delete "Assembly-CSharp.bak"
4.2 Patch again


# Build
1. Use Visual Studio 2015
2. Install Unity: https://unity3d.com/
3. Set a correct path to the Unity library folder in the Memoria.csproj (Notepad)
<FrameworkPathOverride>C:\Program Files\Unity\Editor\Data\Mono\lib\mono\unity\</FrameworkPathOverride>
4. Create the "\References" folder in the root solution directory.
5. Copy "Assembly-CSharp.dll" and "UnityEngine.dll" from "\FINAL FANTASY IX\x64\FF9_Data\Managed" to "\References"
6. Download "Mono.Cecil" package manualy or enable automaticaly package downloads.


# Patch:
1. Memoria.Patcher.exe - will patch game files using current directory or path from the windows registry
2. Memoria.Patcher.exe gameDirectory - will patch game files using a provided path


# First run:
1. Run game.
2. If there is no error you will see "Memoria.ini" file in the game directory.
3. If something went wrong you will see error in the "Memoria.log"
4. If you not see "Memoria.log" try to run game with administrator rights
5. If the error persists see "\FINAL FANTASY IX\x64(or x86)\FF9_Data\output_log.txt"


# Configuration:
Now you can:
1. Disable cheats
2. Change font
3. Export text resources.
Change "Enabled" value from "0" to "1", specify other params and try run the game.


# Exported types (for developers)
1. Write you own type. Keep all members and its signature (private fields too), name **and namespace**.
2. Add **FullName** of type to the TypeReplacers without hash
3. Run patcher
4. Copy hash of type from console or Memoria.log and past to the TypeReplacers
5. Run the game, test all of you can and see output_log.txt for errors.