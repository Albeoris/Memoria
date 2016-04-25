# Memoria
Final Fantasy IX tools

**Build:**
0) Use Visual Studio 2015
1) Install Unity: https://unity3d.com/
2) Set a correct path to the Unity library folder in the Memoria.csproj (Notepad)
<FrameworkPathOverride>C:\Program Files\Unity\Editor\Data\Mono\lib\mono\unity\</FrameworkPathOverride>
3) Create the "\References" folder in the root solution directory.
4) Copy "Assembly-CSharp.dll" and "UnityEngine.dll" from "\FINAL FANTASY IX\x64\FF9_Data\Managed" to "\References"
5) Download "Mono.Cecil" package manualy or enable automaticaly package downloads.

**Patch:**
Memoria.Patcher.exe - will patch game files using path from the windows registry
Memoria.Patcher.exe <gameDirectory> - will patch game files using a provided path

**First run:**
1) Run game.
2) If there is no error you will see "Memoria.ini" file in the game directory.
3) If something went wrong you will see error in the "Memoria.log"
4) If you not see "Memoria.log" try to run game with administrator rights
5) If the error persists see "\FINAL FANTASY IX\x64(or x86)\FF9_Data\output_log.txt"

**Configuration:**
Now you can change font and export text resources.
Change "Enabled" value from "0" to "1", specify other params and try run the game.