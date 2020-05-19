using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Memoria.EventEngine.EV;

namespace Memoria.Test
{
    /// <summary>
    /// 
    /// </summary>
    internal class Program
    {
	    public static void Main(string[] args)
	    {
		    // Generate();

		    // foreach (var file in Directory.GetFiles(@"W:\Steam\steamapps\common\FINAL FANTASY IX\EventEngine", "*.eb.bytes"))
		    // {
		    //     using var input = File.OpenRead(file);
		    //     EVFileReader reader = new EVFileReader(input);
		    //     reader.Read();
		    // }

		    using (var input = File.OpenRead(@"W:\Steam\steamapps\common\FINAL FANTASY IX\EventEngine\EVT_ALEX1_TS_CARGO_0.eb.bytes"))
		    {
			    EVFileReader reader = new EVFileReader(input);
			    EVObject[] objects = reader.Read();
		    }
	    }

	    private static void Generate()
        {
	        var result = Enumerate().ToArray();

	        StringBuilder sb = new StringBuilder();

	        foreach (var item in result)
	        {
		        sb.AppendLine("/// <summary>");
		        sb.AppendLine($"/// {item.label}");
		        foreach (var line in item.help.Split('\n'))
			        sb.AppendLine($"/// {line}");
		        if (item.arg_amount > 0)
		        {
			        if (item.arg_length.Length != item.arg_amount || item.arg_amount != item.arg_type.Length || item.arg_amount != item.arg_help.Length)
				        throw new NotSupportedException();

			        for (int i = 0; i < item.arg_amount; i++)
			        {
				        sb.AppendLine($"/// {item.arg_type[i]} {item.arg_help[i]} ({item.arg_length[i]} bytes)");
			        }
		        }
		        else
		        {
			        if (item.use_vararg)
				        sb.AppendLine($"/// Use arg: {item.use_vararg}");
			        if (item.arg_amount < 0)
				        sb.AppendLine($"/// Negative args: {item.arg_amount}");
		        }

		        sb.AppendLine("/// </summary>");
		        sb.AppendLine($"{item.ecb} = 0x{((Int32) item.ecb):X3},");
		        sb.AppendLine();
	        }
        }

        private static IEnumerable<SortedChoiceItemScriptOpcode> Enumerate()
        {
	yield return new SortedChoiceItemScriptOpcode(0x00, "NOTHING", "Do nothing.", false, 0, null, null, null, 0 );
	yield return new SortedChoiceItemScriptOpcode(0x01, "JMP", "Skip some operations.\nWARNING: unsafe to use.", false, 1, new byte[1]{ 2}, new string[1]{ "Jump"}, new string[1]{ "AT_JUMP"}, 3);
	yield return new SortedChoiceItemScriptOpcode(0x02, "JMP_IFN", "Skip some operations if the stack value is not 0.\nWARNING: unsafe to use.", false, 1, new byte[1]{ 2}, new string[1]{ "Jump"}, new string[1]{ "AT_JUMP"}, 3);
	yield return new SortedChoiceItemScriptOpcode(0x03, "JMP_IF", "Skip some operations if the stack value is 0.\nWARNING: unsafe to use.", false, 1, new byte[1]{ 2}, new string[1]{ "Jump"}, new string[1]{ "AT_JUMP"}, 3);
	yield return new SortedChoiceItemScriptOpcode(0x04, "return", "End the function.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x05, "set", "Perform variable operations and store the result in the stack.", false, 1, new byte[1]{ 0}, new string[1]{ "Variable Code"}, new string[1]{ "AT_NONE"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x06, "JMP_SWITCHEX", "Skip some operations depending on the stack value.\nWARNING: unsafe to use.", false, -1, new byte[3]{ 2, 2, 2}, new string[3]{ "Default Jump", "Case", "Jump"}, new string[3]{ "AT_JUMP", "AT_SPIN", "AT_JUMP"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x07, "InitCode", "Init a normal code (independant functions).\n\n1st argument: code entry to init.\n2nd argument: Unique ID (defaulted to entry's ID if 0).", false, 2, new byte[2]{ 1, 1}, new string[2]{ "Entry", "UID"}, new string[2]{ "AT_ENTRY", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x08, "InitRegion", "Init a region code (associated with a region).\n\n1st argument: code entry to init.\n2nd argument: Unique ID (defaulted to entry's ID if 0).", false, 2, new byte[2]{ 1, 1}, new string[2]{ "Entry", "UID"}, new string[2]{ "AT_ENTRY", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x09, "InitObject", "Init an object code (associated with a 3D model). Also load the related model into the RAM.\n\n1st argument: code entry to init.\n2nd argument: Unique ID (defaulted to entry's ID if 0).", false, 2, new byte[2]{ 1, 1}, new string[2]{ "Entry", "UID"}, new string[2]{ "AT_ENTRY", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x0A, "0x0A", "Unused Opcode.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x0B, "JMP_SWITCH", "Skip some operations depending on the stack value.\nWARNING: unsafe to use.", false, -1, new byte[3]{ 2, 2, 2}, new string[3]{ "Starting Value", "Default Jump", "Jump"}, new string[3]{ "AT_USPIN", "AT_JUMP", "AT_JUMP"}, 1);
	yield return new SortedChoiceItemScriptOpcode(0x0C, "0x0C", "Unknown Opcode.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x0D, "0x0D", "Unknown Opcode.", false, 0, null, null, null, 0); // Steam seems to handle it like a JMP_SWITCH with a short instead of a char
	yield return new SortedChoiceItemScriptOpcode(0x0E, "0x0E", "Unused Opcode.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x0F, "0x0F", "Unused Opcode.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x10, "RunScriptAsync", "Run script function and continue executing the current one.\n\nEntry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not run. Otherwise, the entry's script level is set to the specified script level until the function returns.\n\n1st argument: script level.\n2nd argument: entry of the function.\n3rd argument: function.", true, 3, new byte[3]{ 1, 1, 1}, new string[3]{ "Script Leve", "Entry", "Function"}, new string[3]{ "AT_SCRIPTLVL", "AT_ENTRY", "AT_FUNCTION"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x11, "0x11", "Unused Opcode.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x12, "RunScript", "Wait until the entry's script level gets higher than the specified script level then run the script function and continue executing the current one.\n\nEntry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not run. Otherwise, the entry's script level is set to the specified script level until the function returns.\n\n1st argument: script level.\n2nd argument: entry of the function.\n3rd argument: function.", true, 3, new byte[3]{ 1, 1, 1}, new string[3]{ "Script Leve", "Entry", "Function"}, new string[3]{ "AT_SCRIPTLVL", "AT_ENTRY", "AT_FUNCTION"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x13, "0x13", "Unused Opcode.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x14, "RunScriptSync", "Wait until the entry's script level gets higher than the specified script level then run the script function and wait until it returns.\n\nEntry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not run. Otherwise, the entry's script level is set to the specified script level until the function returns.\n\n1st argument: script level.\n2nd argument: entry of the function.\n3rd argument: function.", true, 3, new byte[3]{ 1, 1, 1}, new string[3]{ "Script Leve", "Entry", "Function"}, new string[3]{ "AT_SCRIPTLVL", "AT_ENTRY", "AT_FUNCTION"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x15, "0x15", "Unused Opcode.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x16, "RunScriptObjectAsync", "Run script function and continue executing the current one. Must only be used in response to a function call ; the argument's entry is the one that called this function.\n\nEntry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not run. Otherwise, the entry's script level is set to the specified script level until the function returns.\n\n1st argument: script level.\n2nd argument: function.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Script Leve", "Function"}, new string[2]{ "AT_SCRIPTLVL", "AT_FUNCTION"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x17, "0x17", "Unused Opcode.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x18, "RunScriptObject", "Wait until the entry's script level gets higher than the specified script level then run the script function and continue executing the current one. Must only be used in response to a function call ; the argument's entry is the one that called this function.\n\nEntry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not run. Otherwise, the entry's script level is set to the specified script level until the function returns.\n\n1st argument: script level.\n2nd argument: function.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Script Leve", "Function"}, new string[2]{ "AT_SCRIPTLVL", "AT_FUNCTION"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x19, "0x19", "Unused Opcode.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x1A, "RunScriptObjectSync", "Wait until the entry's script level gets higher than the specified script level then run the script function and wait until it returns. Must only be used in response to a function call ; the argument's entry is the one that called this function.\n\nEntry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not run. Otherwise, the entry's script level is set to the specified script level until the function returns.\n\n1st argument: script level.\n2nd argument: function.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Script Leve", "Function"}, new string[2]{ "AT_SCRIPTLVL", "AT_FUNCTION"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x1B, "ContinueBattleMusic", "Continue the music after the battle end.\n\n1st argument: flag continue/don't continue.", true, 1, new byte[1]{ 1}, new string[1]{ "Continue"}, new string[1]{ "AT_BOOL"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x1C, "TerminateEntry", "Stop the execution of an entry's code.\n\n1st argument: entry to terminate.", true, 1, new byte[1]{ 1}, new string[1]{ "Object"}, new string[1]{ "AT_ENTRY"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x1D, "CreateObject", "Place (or replace) the 3D model on the field.\n\n1st and 2nd arguments: position in (X, Y) format.", true, 2, new byte[2]{ 2, 2}, new string[2]{ "PositionX", "PositionY"}, new string[2]{ "AT_POSITION_X", "AT_POSITION_Y"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x1E, "SetCameraBounds", "Redefine the field camera boundaries (default value is part of the background's data).\n\n1st argument: camera ID.\n2nd to 5th arguments: Boundaries in (MinX, MaxX, MinY, MaxY) format.", true, 5, new byte[5]{ 1, 2, 2, 2, 2}, new string[5]{ "Camera", "Min X", "Max X", "Min Y", "Max Y"}, new string[5]{ "AT_USPIN", "AT_SPIN", "AT_SPIN", "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x1F, "WindowSync", "Display a window with text inside and wait until it closes.\n\n1st argument: window ID.\n2nd argument: UI flag list.\n 3: disable bubble tail\n 4: mognet format\n 5: hide window\n 7: ATE window\n 8: dialog window\n3rd argument: text to display.", true, 3, new byte[3]{ 1, 1, 2}, new string[3]{ "Window ID", "UI", "Text"}, new string[3]{ "AT_USPIN", "AT_BOOLLIST", "AT_TEXT"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x20, "WindowAsync", "Display a window with text inside and continue the execution of the script without waiting.\n\n1st argument: window ID.\n2nd argument: UI flag list.\n 3: disable bubble tail\n 4: mognet format\n 5: hide window\n 7: ATE window\n 8: dialog window\n3rd argument: text to display.", true, 3, new byte[3]{ 1, 1, 2}, new string[3]{ "Window ID", "UI", "Text"}, new string[3]{ "AT_USPIN", "AT_BOOLLIST", "AT_TEXT"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x21, "CloseWindow", "Close a window.\n\n1st argument: window ID determined at its creation.", true, 1, new byte[1]{ 1}, new string[1]{ "Window ID"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x22, "Wait", "Wait some time.\n\n1st argument: amount of frames to wait.\n For PAL, 1 frame is 0.04 seconds.\n For other versions, 1 frame is about 0.033 seconds.", true, 1, new byte[1]{ 1}, new string[1]{ "Frame Amount"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x23, "Walk", "Make the character walk to destination. Make it synchronous if InitWalk is called before.\n\n1st and 2nd arguments: destination in (X, Y) format.", true, 2, new byte[2]{ 2, 2}, new string[2]{ "DestinationX", "DestinationY"}, new string[2]{ "AT_POSITION_X", "AT_POSITION_Y"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x24, "WalkTowardObject", "Make the character walk and follow an object.\n\n1st argument: object to walk toward.", true, 1, new byte[1]{ 1}, new string[1]{ "Object"}, new string[1]{ "AT_ENTRY"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x25, "InitWalk", "Make a further Walk call (or variations of Walk) synchronous.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x26, "SetWalkSpeed", "Change the walk speed.\n\n1st argument: speed (surely in unit/frame).", true, 1, new byte[1]{ 1}, new string[1]{ "Speed"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x27, "SetTriangleFlagMask", "Set a bitmask for some of the walkmesh triangle flags.\n\n1st argument: flag mask.\n 7: disable restricted triangles\n 8: disable player-restricted triangles", true, 1, new byte[1]{ 1}, new string[1]{ "Flags"}, new string[1]{ "AT_BOOLLIST"}, 0); // BGIMASK
	yield return new SortedChoiceItemScriptOpcode(0x28, "Cinematic", "Run or setup a cinematic.\n\n1st argument: unknown.\n2nd argument: cinematic ID (may depends on 1st argument's value).\n3rd argument: unknown.\n4th argument: unknown.", true, 4, new byte[4]{ 1, 1, 1, 1}, new string[4]{ "Unknown", "Cinematic ID", "Unknown", "Unknown"}, new string[4]{ "AT_SPIN", "AT_FMV", "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x29, "SetRegion", "Define the polygonal region linked with the entry script.\n\nArguments are in the format (Vertice X, Vertice Y) and can be of any amount.", true, -1, new byte[1]{ 4}, new string[1]{ "Polygon"}, new string[1]{ "AT_NONE"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x2A, "Battle", "Start a battle (using a random enemy group).\n\n1st argument: rush type (unknown).\n2nd argument: gathered battle and Steiner's state (highest bit) informations.", true, 2, new byte[2]{ 1, 2}, new string[2]{ "Rush Type", "Battle"}, new string[2]{ "AT_SPIN", "AT_BATTLE"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x2B, "Field", "Change the field scene.\n\n1st argument: field scene destination.", true, 1, new byte[1]{ 2}, new string[1]{ "Field"}, new string[1]{ "AT_FIELD"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x2C, "DefinePlayerCharacter", "Apply the player's control over the entry's object.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x2D, "DisableMove", "Disable the player's movement control.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x2E, "EnableMove", "Enable the player's movement control.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x2F, "SetMode", "Set the model of the object and its head's height (used to set the dialog box's height).\n\n1st argument: model.\n2nd argument: head's height.", true, 2, new byte[2]{ 2, 1}, new string[2]{ "Mode", "Height"}, new string[2]{ "AT_MODEL", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x30, "0x30", "Unknown Opcode; ignored in the non-PSX versions.", false, 0, null, null, null, 0); // "PRINT1" Return 0 on Steam
	yield return new SortedChoiceItemScriptOpcode(0x31, "0x31", "Unknown Opcode; ignored in the non-PSX versions.", false, 0, null, null, null, 0); // "PRINTF" Return 0 on Steam
	yield return new SortedChoiceItemScriptOpcode(0x32, "0x32", "Unused Opcode; bugs if used in the non-PSX versions.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_SPIN", "AT_SPIN"}, 0); // "LOCATE" Ignored on Steam and not even handled properly (no argument read)
	yield return new SortedChoiceItemScriptOpcode(0x33, "SetStandAnimation", "Change the standing animation.\n\n1st argument: animation ID.", true, 1, new byte[1]{ 2}, new string[1]{ "Animation"}, new string[1]{ "AT_ANIMATION"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x34, "SetWalkAnimation", "Change the walking animation.\n\n1st argument: animation ID.", true, 1, new byte[1]{ 2}, new string[1]{ "Animation"}, new string[1]{ "AT_ANIMATION"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x35, "SetRunAnimation", "Change the running animation.\n\n1st argument: animation ID.", true, 1, new byte[1]{ 2}, new string[1]{ "Animation"}, new string[1]{ "AT_ANIMATION"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x36, "TurnInstant", "Make the character face an angle.\n\n1st argument: angle.\n0 faces south, 64 faces west, 128 faces north and 192 faces east.", true, 1, new byte[1]{ 1}, new string[1]{ "Angle"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x37, "SetPitchAngle", "Turn the model in the up/down direction.\n\n1st argument: angle (pitch axis).\n2nd argument: angle (XZ axis).", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Pitch", "XZ Angle"}, new string[2]{ "AT_USPIN", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x38, "Attack", "Make the enemy attack. The target(s) are to be set using the SV_Target variable.\nInside an ATB function, the attack is added to the queue.\nInside a counter function, the attack occurs directly.\n\n1st argument: attack to perform.", true, 1, new byte[1]{ 1}, new string[1]{ "Attack"}, new string[1]{ "AT_ATTACK"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x39, "ShowObject", "Show an object.\n\n1st argument: object.\n2nd argument: unknown.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Object", "Unknown"}, new string[2]{ "AT_ENTRY", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x3A, "HideObject", "Hide an object.\n\n1st argument: object.\n2nd argument: unknown.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Object", "Unknown"}, new string[2]{ "AT_ENTRY", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x3B, "SetObjectIndex", "Redefine the current object's index.\n\n1st argument: new index.", true, 1, new byte[1]{ 1}, new string[1]{ "Index"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x3C, "SetRandomBattles", "Define random battles.\n\n1st argument: pattern, deciding the encounter chances and the topography (World Map only).\n 0: {0.375, 0.28, 0.22, 0.125}\n 1: {0.25, 0.25, 0.25, 0.25}\n 2: {0.35, 0.3, 0.3, 0.05}\n 3: {0.45, 0.4, 0.1, 0.05}\n2nd to 5th arguments: possible random battles.", true, 5, new byte[5]{ 1, 2, 2, 2, 2}, new string[5]{ "Pattern", "Battle 1", "Battle 2", "Battle 3", "Battle 4"}, new string[5]{ "AT_USPIN", "AT_BATTLE", "AT_BATTLE", "AT_BATTLE", "AT_BATTLE"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x3D, "0x3D", "Some animation flags.\n\n1st argument: unknown.\n2nd argument: unknown.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "In-Frame",  "Out-Frame"}, new string[2]{ "AT_USPIN", "AT_USPIN"}, 0); // AFRAME
	yield return new SortedChoiceItemScriptOpcode(0x3E, "SetAnimationSpeed", "Set the current object's animation speed.\n\n1st argument: speed.", true, 1, new byte[1]{ 1}, new string[1]{ "Speed"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x3F, "SetAnimationFlags", "Set the current object's next animation looping flags.\n\n1st argument: looping flag list.\n 1: freeze at end\n 2: loop\n 3: loop back and forth\n2nd arguments: times to repeat.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Flag", "Repeat"}, new string[2]{ "AT_ANIMFLAG", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x40, "RunAnimation", "Make the character play an animation.\n\n1st argument: animation ID.", true, 1, new byte[1]{ 2}, new string[1]{ "Animation"}, new string[1]{ "AT_ANIMATION"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x41, "WaitAnimation", "Wait until the current object's animation has ended.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x42, "StopAnimation", "Stop the character's animation.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x43, "RunSharedScript", "Run script passing the current object to it and continue executing the current function. If another shared script is already running for this object, it will be terminated.\n\n1st argument: entry (should be a one-function entry).", true, 1, new byte[1]{ 1}, new string[1]{ "Entry"}, new string[1]{ "AT_ENTRY"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x44, "WaitSharedScript", "Wait until the ran shared script has ended.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x45, "StopSharedScript", "Terminate the execution of the ran shared script.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x46, "0x46", "No use.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x47, "EnableHeadFocus", "Enable or disable the character turning his head toward an active object.\n\n1st argument: flags.\n 1: unknown\n 2: unknown\n 3: turn toward talkers", true, 1, new byte[1]{ 1}, new string[1]{ "Flags"}, new string[1]{ "AT_BOOLLIST"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x48, "AddItem", "Add item to the player's inventory. Only one copy of key items can be in the player's inventory.\n\n1st argument: item to add.\n2nd argument: amount to add.", true, 2, new byte[2]{ 2, 1}, new string[2]{ "Item", "Amount"}, new string[2]{ "AT_ITEM", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x49, "RemoveItem", "Remove item from the player's inventory.\n\n1st argument: item to remove.\n2nd argument: amount to remove.", true, 2, new byte[2]{ 2, 1}, new string[2]{ "Item", "Amount"}, new string[2]{ "AT_ITEM", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x4A, "RunBattleCode", "Run a battle code.\n\n1st argument: battle code.\n2nd argument: depends on the battle code.\n End Battle: 0 for a defeat (deprecated), 1 for a victory, 2 for a victory without victory pose, 3 for a defeat, 4 for an escape, 5 for an interrupted battle, 6 for a game over, 7 for an enemy escape.\n Run Camera: Camera ID.\n Change Field: ID of the destination field after the battle.\n Add Gil: amount to add.", true, 2, new byte[2]{ 1, 2}, new string[2]{ "Code", "Argument"}, new string[2]{ "AT_BATTLECODE", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x4B, "SetObjectLogicalSize", "Set different size informations of the object.\n\n1st argument: size for pathing.\n2nd argument: collision radius.\n3rd argument: talk distance.", true, 3, new byte[3]{ 1, 1, 1}, new string[3]{ "Walk Radius", "Collision Radius", "Talk Distance"}, new string[3]{ "AT_SPIN", "AT_USPIN", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x4C, "AttachObject", "Attach an object to another one.\n\n1st argument: carried object.\n2nd argument: carrying object.\n3rd argument: attachment point (unknown format).", true, 3, new byte[3]{ 1, 1, 1}, new string[3]{ "Object", "Carrier", "Attachement Point"}, new string[3]{ "AT_ENTRY", "AT_ENTRY", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x4D, "DetachObject", "Stop attaching an object to another one.\n\n1st argument: carried object.", true, 1, new byte[1]{ 1}, new string[1]{ "Object"}, new string[1]{ "AT_ENTRY"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x4E, "0x4E", "Unknown Opcode.", false, 0, null, null, null, 0); // WATCH & STOP
	yield return new SortedChoiceItemScriptOpcode(0x4F, "0x4F", "Unknown Opcode.", true, 1, new byte[1]{ 1}, new string[1]{ "Unknown"}, new string[1]{ "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x50, "WaitTurn", "Wait until the character has turned.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x51, "TurnTowardObject", "Turn the character toward an entry object (animated).\n\n1st argument: object.\n2nd argument: turn speed (1 is slowest).", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Object", "Speed"}, new string[2]{ "AT_ENTRY", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x52, "SetInactiveAnimation", "Change the animation played when inactive for a long time. The inaction time required is:\nFirst Time = 200 + 4 * Random[0, 255]\nSubsequent Times = 200 + 2 * Random[0, 255]\n\n1st argument: animation ID.", true, 1, new byte[1]{ 2}, new string[1]{ "Animation"}, new string[1]{ "AT_ANIMATION"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x53, "0x53", "Seems to prevent new windows to close older ones.", false, 0, null, null, null, 0); // NOINITMES
	yield return new SortedChoiceItemScriptOpcode(0x54, "WaitWindow", "Wait until the window is closed.\n\n1st argument: window ID determined at its creation.", true, 1, new byte[1]{ 1}, new string[1]{ "Window ID"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x55, "SetWalkTurnSpeed", "Change the turn speed of the object when it walks or runs (default is 16).\n\n1st argument: turn speed (with 0, the object doesn't turn while moving).\n\nSpecial treatments:\nVivi's in Iifa Tree/Eidolon Moun (field 1656) is initialized to 48.\nChoco's in Chocobo's Paradise (field 2954) is initialized to 96.", true, 1, new byte[1]{ 1}, new string[1]{ "Turn Speed"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x56, "TimedTurn", "Make the character face an angle (animated).\n\n1st argument: angle.\n0 faces south, 64 faces west, 128 faces north and 192 faces east.\n2nd argument: turn speed (1 is slowest).", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Angle", "Speed"}, new string[2]{ "AT_USPIN", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x57, "SetRandomBattleFrequency", "Set the frequency of random battles.\n\n1st argument: frequency.\n 255 is the maximum frequency, corresponding to ~12 walking steps or ~7 running steps.\n 0 is the minimal frequency and disables random battles.", true, 1, new byte[1]{ 1}, new string[1]{ "Frequency"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x58, "SlideXZY", "Make the character slide to destination (walk without using the walk animation and without changing the facing angle).\n\n1st to 3rd arguments: destination in (X, Z, Y) format.", true, 3, new byte[3]{ 2, 2, 2}, new string[3]{ "DestinationX", "DestinationZ", "DestinationY"}, new string[3]{ "AT_POSITION_X", "AT_POSITION_Z", "AT_POSITION_Y"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x59, "SetTileColor", "Change the color of a field tile block.\n\n1st argument: background tile block.\n2nd to 4th arguments: color in (Cyan, Magenta, Yellow) format.", true, 4, new byte[4]{ 1, 1, 1, 1}, new string[4]{ "Tile Block", "ColorC", "ColorM", "ColorY"}, new string[4]{ "AT_TILE", "AT_COLOR_CYAN", "AT_COLOR_MAGENTA", "AT_COLOR_YELLOW"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x5A, "SetTilePositionEx", "Move a field tile block.\n\n1st argument: background tile block.\n2nd and 3rd argument: position in (X, Y) format.\n4th argument: closeness, defining whether 3D models are over or under that background tile.", true, 4, new byte[4]{ 1, 2, 2, 2}, new string[4]{ "Tile Block", "Position X", "Position Y", "Position Closeness"}, new string[4]{ "AT_TILE", "AT_SPIN", "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x5B, "ShowTile", "Show or hide a field tile block.\n\n1st argument: background tile block.\n2nd argument: boolean show/hide.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Tile Block", "Show"}, new string[2]{ "AT_TILE", "AT_BOOL"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x5C, "MoveTileLoop", "Make the image of a field tile loop over space.\n\n1st argument: background tile block.\n2nd argument: boolean on/off.\n3rd and 4th arguments: speed in the X and Y directions.", true, 4, new byte[4]{ 1, 1, 2, 2}, new string[4]{ "Tile Block", "Activate", "X Loop", "Y Loop"}, new string[4]{ "AT_TILE", "AT_BOOL", "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x5D, "MoveTile", "Make the field moves depending on the camera position.\n\n1st argument: background tile block.\n2nd argument: boolean on/off.\n3rd and 4th arguments: parallax movement in (X, Y) format.", true, 4, new byte[4]{ 1, 1, 2, 2}, new string[4]{ "Tile Block", "Activate", "Movement X", "Movement Y"}, new string[4]{ "AT_TILE", "AT_BOOL", "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x5E, "SetTilePosition", "Move a field tile block.\n\n1st argument: background tile block.\n2nd and 3rd argument: position in (X, Y) format.", true, 3, new byte[3]{ 1, 2, 2}, new string[3]{ "Tile Block", "Position X", "Position Y"}, new string[3]{ "AT_TILE", "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x5F, "RunTileAnimation", "Run a field tile animation.\n\n1st argument: background animation.\n2nd argument: starting frame.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Field Animation", "Frame"}, new string[2]{ "AT_TILEANIM", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x60, "ActivateTileAnimation", "Make a field tile animation active.\n\n1st argument: background animation.\n2nd argument: boolean on/off.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Tile Animation", "Activate"}, new string[2]{ "AT_TILEANIM", "AT_BOOL"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x61, "SetTileAnimationSpeed", "Change the speed of a field tile animation.\n\n1st argument: background animation.\n2nd argument: speed (256 = 1 tile/frame).", true, 2, new byte[2]{ 1, 2}, new string[2]{ "Tile Animation", "Speed"}, new string[2]{ "AT_TILEANIM", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x62, "SetRow", "Change the battle row of a party member.\n\n1st argument: party member.\n2nd argument: boolean front/back.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Character", "Row"}, new string[2]{ "AT_LCHARACTER", "AT_BOOL"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x63, "SetTileAnimationPause", "Make a field tile animation pause at some frame in addition to its normal animation speed.\n\n1st argument: background animation.\n2nd argument: animation frame.\n3rd argument: wait time.", true, 3, new byte[3]{ 1, 1, 1}, new string[3]{ "Tile Animation", "Frame ID", "Time"}, new string[3]{ "AT_TILEANIM", "AT_USPIN", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x64, "SetTileAnimationFlags", "Add flags of a field tile animation.\n\n1st argument: background animation.\n2nd argument: flags (only the flags 5 and 6 can be added).\n 5: unknown\n 6: loop back and forth", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Tile Animation", "Flags"}, new string[2]{ "AT_TILEANIM", "AT_BOOLLIST"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x65, "RunTileAnimationEx", "Run a field tile animation and choose its frame range.\n\n1st argument: background animation.\n2nd argument: starting frame.\n3rd argument: ending frame.", true, 3, new byte[3]{ 1, 1, 1}, new string[3]{ "Tile Animation", "Start", "End"}, new string[3]{ "AT_TILEANIM", "AT_USPIN", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x66, "SetTextVariable", "Set the value of a text number or item variable.\n\n1st argument: text variable's \"Script ID\".\n2nd argument: depends on which text opcode is related to the text variable.\n For [VAR_NUM]: integral value.\n For [VAR_ITEM]: item ID.\n For [VAR_TOKEN]: token number.", true, 2, new byte[2]{ 1, 2}, new string[2]{ "Variable ID", "Value"}, new string[2]{ "AT_USPIN", "AT_ITEM"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x67, "SetControlDirection", "Set the angles for the player's movement control.\n\n1st argument: angle used for arrow movements.\n2nd argument: angle used for analogic stick movements.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Arrow Angle", "Analogic Angle"}, new string[2]{ "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x68, "Bubble", "Display a speech bubble with a symbol inside over the head of player's character.\n\n1st argument: bubble symbol.", true, 1, new byte[1]{ 1}, new string[1]{ "Symbo"}, new string[1]{ "AT_BUBBLESYMBOL"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x69, "ChangeTimerTime", "Change the remaining time of the timer window.\n\n1st argument: time in seconds.", true, 1, new byte[1]{ 2}, new string[1]{ "Time"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x6A, "DisableRun", "Make the player's character always walk.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x6B, "SetBackgroundColor", "Change the default color, seen behind the field's tiles.\n\n1st to 3rd arguments: color in (Red, Green, Blue) format.", true, 3, new byte[3]{ 1, 1, 1}, new string[3]{ "ColorR", "ColorG", "ColorB"}, new string[3]{ "AT_COLOR_RED", "AT_COLOR_GREEN", "AT_COLOR_BLUE"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x6C, "0x6C", "Unknown Opcode.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x6D, "0x6D", "Unknown Opcode.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x6E, "0x6E", "Unknown Opcode.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x6F, "MoveCamera", "Move camera over time.\n\n1st and 2nd arguments: destination in (X, Y) format.\n3nd argument: movement duration.\n4th argument: scrolling type (8 for sinusoidal, other values for linear interpolation).", true, 4, new byte[4]{ 2, 2, 1, 1}, new string[4]{ "DestinationX", "DestinationY", "Time", "Smoothness"}, new string[4]{ "AT_POSITION_X", "AT_POSITION_Y", "AT_USPIN", "AT_USPIN"}, 0); // screen size = 320?
	yield return new SortedChoiceItemScriptOpcode(0x70, "ReleaseCamera", "Release camera movement, getting back to its normal behaviour.\n\n1st arguments: duration of the repositioning.\n2nd argument: scrolling type (8 for sinusoidal, other values for linear interpolation).", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Time", "Smoothness"}, new string[2]{ "AT_SPIN", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x71, "EnableCameraServices", "Enable or disable camera services. When disabling, the 2nd and 3rd arguments are ignored.\n\n1st arguments: boolean activate/deactivate.\n2nd argument: duration of the repositioning when activating (defaulted to 30 if -1 is given).\n3rd argument: scrolling type of the repositioning when activating (8 for sinusoidal, other values for linear interpolation).", true, 3, new byte[3]{ 1, 1, 1}, new string[3]{ "Enable", "Time", "Smoothness"}, new string[3]{ "AT_BOOL", "AT_SPIN", "AT_USPIN"}, 0); // BGCACTIVE
	yield return new SortedChoiceItemScriptOpcode(0x72, "SetCameraFollowHeight", "Define the standard height gap between the player's character position and the camera view.\n\n1st argument: height.", true, 1, new byte[1]{ 2}, new string[1]{ "Height"}, new string[1]{ "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x73, "EnableCameraFollow", "Make the camera follow the player's character.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x74, "DisableCameraFollow", "Stop making the camera follow the player's character.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x75, "Menu", "Open a menu.\n\n1st argument: menu type.\n2nd argument: depends on the menu type.\n Naming Menu: character to name.\n Shop Menu: shop ID.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Menu Type", "Menu"}, new string[2]{ "AT_MENUTYPE", "AT_MENU"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x76, "0x76", "Unknown Opcode.", true, 2, new byte[2]{ 2, 2}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x77, "0x77", "Unknown Opcode.", true, 2, new byte[2]{ 2, 2}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x78, "0x78", "Unknown Opcode.", false, 0, null, null, null, 0); // TRACKSTART, TRACK, TRACKADD and PRINTQUAD
	yield return new SortedChoiceItemScriptOpcode(0x79, "0x79", "Unknown Opcode.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x7A, "SetLeftAnimation", "Change the left turning animation.\n\n1st argument: animation ID.", true, 1, new byte[1]{ 2}, new string[1]{ "Animation"}, new string[1]{ "AT_ANIMATION"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x7B, "SetRightAnimation", "Change the right turning animation.\n\n1st argument: animation ID.", true, 1, new byte[1]{ 2}, new string[1]{ "Animation"}, new string[1]{ "AT_ANIMATION"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x7C, "EnableDialogChoices", "Define choices availability in dialogs using the [INIT_MULTICHOICE] text opcode.\n\n1st argument: boolean list for the different choices.\n2nd argument: default choice selected.", true, 2, new byte[2]{ 2, 1}, new string[2]{ "Choice List", "Default Choice"}, new string[2]{ "AT_BOOLLIST", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x7D, "RunTimer", "Run or pause the timer window.\n\n1st argument: boolean run/pause.", true, 1, new byte[1]{ 1}, new string[1]{ "Run"}, new string[1]{ "AT_BOOL"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x7E, "SetFieldCamera", "Change the field's background camera.\n\n1st argument: camera ID.", true, 1, new byte[1]{ 1}, new string[1]{ "Camera ID"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x7F, "EnableShadow", "Enable the shadow for the entry's object.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x80, "DisableShadow", "Disable the shadow for the entry's object.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x81, "SetShadowSize", "Set the entry's object shadow size.\n\n1st argument: size X.\n2nd argument: size Y.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Size X", "Size Y"}, new string[2]{ "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x82, "SetShadowOffset", "Change the offset between the entry's object and its shadow.\n\n1st argument: offset X.\n2nd argument: offset Y.", true, 2, new byte[2]{ 2, 2}, new string[2]{ "Offset X", "Offset Y"}, new string[2]{ "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x83, "LockShadowRotation", "Stop updating the shadow rotation by the object's rotation.\n\n1st argument: locked rotation.", true, 1, new byte[1]{ 1}, new string[1]{ "Locked Rotation"}, new string[1]{ "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x84, "UnlockShadowRotation", "Make the shadow rotate accordingly with its object.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x85, "SetShadowAmplifier", "Amplify or reduce the shadow transparancy.\n\n1st argument: amplification factor.", true, 1, new byte[1]{ 1}, new string[1]{ "Amplification Factor"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x86, "SetAnimationStandSpeed", "Change the standing animation speed.\n\n1st argument: unknown.\n2nd argument: unknown.\n3rd argument: unknown.\n4th argument: unknown.", true, 4, new byte[4]{ 1, 1, 1, 1}, new string[4]{ "Unknown", "Unknown", "Unknown", "Unknown"}, new string[4]{ "AT_USPIN", "AT_USPIN", "AT_USPIN", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x87, "0x87", "Unknown Opcode.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Entry", "Unknown"}, new string[2]{ "AT_ENTRY", "AT_SPIN"}, 0); // DDIR
	yield return new SortedChoiceItemScriptOpcode(0x88, "RunModelCode", "Run a model code.\n\n1st argument: model code.\n2nd to 4th arguments: depends on the model code.\n Texture Blend (blend mode) with blend mode being 1 for screen, 2 for multiply, 4 for Soft and 255 for a solid texture\n Slice (boolean slice/unslice, value)\n Enable Mirror (boolean enable/disable)\n Mirror Position (X, Z, Y)\n Mirror Normal (X, Z, Y)\n Mirror Color (Red, Green, Blue)\n Sound codes (Animation, Frame, Value)\n  For Add (Secondary) Sound, the 3rd argument is the sound ID\n  For Sound Random Pitch, the 3rd argument is a boolean random/not random\n  For Remove Sound, the 3rd argument is unused", true, 4, new byte[4]{ 1, 2, 2, 2}, new string[4]{ "Model Code", "Argument 1", "Argument 2", "Argument 3"}, new string[4]{ "AT_MODELCODE", "AT_SPIN", "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x89, "SetSoundPosition", "Set the position of a 3D sound.\n\n1st to 3rd arguments: sound position.\n4th argument: sound volume.", true, 4, new byte[4]{ 2, 2, 2, 1}, new string[4]{ "PositionX", "PositionZ", "PositionY", "Volume"}, new string[4]{ "AT_POSITION_X", "AT_POSITION_Z", "AT_POSITION_Y", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x8A, "SetSoundObjectPosition", "Set the position of a 3D sound to the object's position.\n\n1st argument: object.\n2nd argument: sound volume.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Object", "Volume"}, new string[2]{ "AT_ENTRY", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x8B, "SetHeadAngle", "Maybe define the maximum angle and distance for the character to turn his head toward an active object.\n\n1st argument: unknown.\n2nd argument: unknown.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_USPIN", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x8C, "BattleEx", "Start a battle and choose its battle group.\n\n1st argument: rush type (unknown).\n2nd argument: group.\n3rd argument: gathered battle and Steiner's state (highest bit) informations.", true, 3, new byte[3]{ 1, 1, 2}, new string[3]{ "Rush Type", "Battle Group", "Battle"}, new string[3]{ "AT_SPIN", "AT_SPIN", "AT_BATTLE"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x8D, "ShowTimer", "Activate the timer window.\n\n1st argument: boolean show/hide.", true, 1, new byte[1]{ 1}, new string[1]{ "Enable"}, new string[1]{ "AT_SPIN"}, 0); // Surely "AT_BOOL"
	yield return new SortedChoiceItemScriptOpcode(0x8E, "RaiseWindows", "Make all the dialogs and windows display over the filters.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x8F, "SetModelColor", "Change a 3D model's color.\n\n1st argument: entry associated with the model.\n2nd to 4th arguments: color in (Red, Green, Blue) format.", true, 4, new byte[4]{ 1, 1, 1, 1}, new string[4]{ "Object", "ColorR", "ColorG", "ColorB"}, new string[4]{ "AT_ENTRY", "AT_COLOR_RED", "AT_COLOR_GREEN", "AT_COLOR_BLUE"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x90, "DisableInactiveAnimation", "Prevent player's character to play its inactive animation.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x91, "0x91", "Unknown Opcode.", true, 1, new byte[1]{ 1}, new string[1]{ "Unknown"}, new string[1]{ "AT_BOOL"}, 0); // AUTOTURN: actor.turninst0 = (arg ? 167: 4)
	yield return new SortedChoiceItemScriptOpcode(0x92, "AttachTile", "Make a part of the field background follow the player's movements. Also apply a color filter out of that tile block's range.\n\n1st argument: tile block.\n2nd and 3rd arguments: offset position in (X, Y) format.\n4th argument: filter mode ; use -1 for no filter effect.\n5th to 7th arguments: filter color in (Red, Green, Blue) format.", true, 7, new byte[7]{ 1, 2, 1, 1, 1, 1, 1}, new string[7]{ "Tile Block", "Position X", "Position Y", "Filter Mode", "Filter ColorR", "Filter ColorG", "Filter ColorB"}, new string[7]{ "AT_TILE", "AT_SPIN", "AT_SPIN", "AT_SPIN", "AT_COLOR_RED", "AT_COLOR_GREEN", "AT_COLOR_BLUE"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x93, "SetObjectFlags", "Change flags of the current entry's object.\n\n1st argument: object flags.\n 1: show model\n 2: unknown\n 4: unknown\n 8: unknown\n 16: unknown\n 32: unknown", true, 1, new byte[1]{ 1}, new string[1]{ "Flags"}, new string[1]{ "AT_BOOLLIST"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x94, "SetJumpAnimation", "Change the jumping animation.\n\n1st argument: animation ID.\n2nd argument: unknown.\n3rd argument: unknown.", true, 3, new byte[3]{ 2, 1, 1}, new string[3]{ "Animation", "Unknown", "Unknown"}, new string[3]{ "AT_ANIMATION", "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x95, "WindowSyncEx", "Display a window with text inside and wait until it closes.\n\n1st argument: talker's entry.\n2nd argument: window ID.\n3rd argument: UI flag list.\n 3: disable bubble tail\n 4: mognet format\n 5: hide window\n 7: ATE window\n 8: dialog window\n4th argument: text to display.", true, 4, new byte[4]{ 1, 1, 1, 2}, new string[4]{ "Talker", "Window ID", "UI", "Text"}, new string[4]{ "AT_ENTRY", "AT_USPIN", "AT_BOOLLIST", "AT_TEXT"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x96, "WindowAsyncEx", "Display a window with text inside and continue the execution of the script without waiting.\n\n1st argument: talker's entry.\n2nd argument: window ID.\n3rd argument: UI flag list.\n 3: disable bubble tail\n 4: mognet format\n 5: hide window\n 7: ATE window\n 8: dialog window\n4th argument: text to display.", true, 4, new byte[4]{ 1, 1, 1, 2}, new string[4]{ "Talker", "Window ID", "UI", "Text"}, new string[4]{ "AT_ENTRY", "AT_USPIN", "AT_BOOLLIST", "AT_TEXT"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x97, "ReturnEntryFunctions", "Make all the currently executed functions return for a given entry.\n\n1st argument: entry for which functions are returned.", true, 1, new byte[1]{ 1}, new string[1]{ "Entry"}, new string[1]{ "AT_ENTRY"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x98, "MakeAnimationLoop", "Make current object's currently playing animation loop.\n\n1st argument: loop amount.", true, 1, new byte[1]{ 1}, new string[1]{ "Amount"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x99, "SetTurnSpeed", "Change the entry's object turn speed.\n\n1st argument: turn speed (1 is slowest).", true, 1, new byte[1]{ 1}, new string[1]{ "Speed"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x9A, "EnablePathTriangle", "Enable or disable a triangle of field pathing.\n\n1st argument: triangle ID.\n2nd argument: boolean enable/disable.", true, 2, new byte[2]{ 2, 1}, new string[2]{ "Triangle ID", "Enable"}, new string[2]{ "AT_WALKTRIANGLE", "AT_BOOL"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x9B, "TurnTowardPosition", "Turn the character toward a position (animated). The object's turn speed is used (default to 16).\n\n1st and 2nd arguments: coordinates in (X, Y) format.", true, 2, new byte[2]{ 2, 2}, new string[2]{ "CoordinateX", "CoordinateY"}, new string[2]{ "AT_POSITION_X", "AT_POSITION_Y"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x9C, "RunJumpAnimation", "Make the character play its jumping animation.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x9D, "RunLandAnimation", "Make the character play its landing animation (inverted jumping animation).", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x9E, "ExitField", "Make the player's character walk to the field exit and prepare to flush the field datas.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x9F, "SetObjectSize", "Set the size of a 3D model.\n\n1st argument: entry of the 3D model.\n2nd to 4th arguments: size ratio in (Ratio X, Ratio Z, Ratio Y) format. A ratio of 64 is the default size.", true, 4, new byte[4]{ 1, 1, 1, 1}, new string[4]{ "Object", "Size X", "Size Z", "Size Y"}, new string[4]{ "AT_ENTRY", "AT_SPIN", "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xA0, "WalkToExit", "Make the entry's object walk to the field exit.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0xA1, "MoveInstantXZY", "Instantatly move the object.\n\n1st to 3rd arguments: destination in (X, Z, Y) format.", true, 3, new byte[3]{ 2, 2, 2}, new string[3]{ "DestinationX", "DestinationZ", "DestinationY"}, new string[3]{ "AT_POSITION_X", "AT_POSITION_Z", "AT_POSITION_Y"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xA2, "WalkXZY", "Make the character walk to destination. Make it synchronous if InitWalk is called before.\n\n1st argument: destination in (X, Z, Y) format.", true, 3, new byte[3]{ 2, 2, 2}, new string[3]{ "DestinationX", "DestinationZ", "DestinationY"}, new string[3]{ "AT_POSITION_X", "AT_POSITION_Z", "AT_POSITION_Y"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xA3, "0xA3", "Unknown Opcode.", true, 4, new byte[4]{ 1, 1, 1, 1}, new string[4]{ "Unknown", "Unknown", "Unknown", "Unknown"}, new string[4]{ "AT_SPIN", "AT_SPIN", "AT_SPIN", "AT_SPIN"}, 0); // DRADIUS ; Unused in Steam
	yield return new SortedChoiceItemScriptOpcode(0xA4, "CalculateExitPosition", "Calculate the field exit position based on the region's polygon.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0xA5, "Slide", "Make the character slide to destination (walk without using the walk animation and without changing the facing angle).\n\n1st and 2nd arguments: destination in (X, Y) format.", true, 2, new byte[2]{ 2, 2}, new string[2]{ "DestinationX", "DestinationY"}, new string[2]{ "AT_POSITION_X", "AT_POSITION_Y"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xA6, "SetRunSpeedLimit", "Change the speed at which the character uses his run animation instead of his walk animation (default is 31).\n\n1st argument: speed limit.", true, 1, new byte[1]{ 1}, new string[1]{ "Speed Limit"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xA7, "Turn", "Make the character face an angle (animated). Speed is defaulted to 16.\n\n1st argument: angle.\n0 faces south, 64 faces west, 128 faces north and 192 faces east.", true, 1, new byte[1]{ 1}, new string[1]{ "Angle"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xA8, "SetPathing", "Change the pathing of the character.\n\n1st argument: boolean pathing on/off.", true, 1, new byte[1]{ 1}, new string[1]{ "Pathing"}, new string[1]{ "AT_BOOL"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xA9, "0xA9", "Unknown Opcode.", true, 1, new byte[1]{ 1}, new string[1]{ "Unknown"}, new string[1]{ "AT_ENTRY"}, 0); // GETSCREEN ; change the sSys position
	yield return new SortedChoiceItemScriptOpcode(0xAA, "EnableMenu", "Enable menu access by the player.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0xAB, "DisableMenu", "Disable menu access by the player.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0xAC, "ChangeDisc", "Allow to save the game and change disc.\n\n1st argument: gathered field destination and disc destination.", true, 1, new byte[1]{ 2}, new string[1]{ "Disc"}, new string[1]{ "AT_DISC_FIELD"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xAD, "MoveInstantXZYEx", "Instantatly move an object.\n\n1st argument: object's entry.\n2nd to 4th arguments: destination in (X, Z, Y) format.", true, 4, new byte[4]{ 1, 2, 2, 2}, new string[4]{ "Object", "DestinationX","DestinationY", "DestinationZ"}, new string[4]{ "AT_ENTRY", "AT_POSITION_X", "AT_POSITION_Z", "AT_POSITION_Y"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xAE, "TetraMaster", "Begin a card game.\n\n1st argument: card deck of the opponent.", true, 1, new byte[1]{ 2}, new string[1]{ "Card Deck"}, new string[1]{ "AT_DECK"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xAF, "0xAF", "Unknown Opcode.", false, 0, null, null, null, 0); // DELETEALLCARD ; doesn't seem to work
	yield return new SortedChoiceItemScriptOpcode(0xB0, "SetFieldName", "Change the name of the field.\n\n1st argument: new name (unknown format).", true, 1, new byte[1]{ 2}, new string[1]{ "Name"}, new string[1]{ "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xB1, "ResetFieldName", "Reset the name of the field.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0xB2, "Party", "Allow the player to change the members of its party.\n\n1st argument: party size (if characters occupy slots beyond it, they are locked).\n2nd argument: list of locked characters.", true, 2, new byte[2]{ 1, 2}, new string[2]{ "Party Size", "Locked Characters"}, new string[2]{ "AT_USPIN", "AT_CHARACTER"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xB3, "RunSPSCode", "Run Sps code, which seems to be special model effects on the field.\n\n1st argument: sps ID.\n2nd argument: sps code.\n3rd to 5th arguments: depend on the sps code.\n Load Sps (sps type)\n Enable Attribute (attribute list, boolean enable/disable)\n Set Position (X, -Z, Y)\n Set Rotation (angle X, angle Z, angle Y)\n Set Scale (scale factor)\n Attach (object's entry to attach, bone number)\n Set Fade (fade)\n Set Animation Rate (rate)\n Set Frame Rate (rate)\n Set Frame (value) where the value is factored by 16 to get the frame\n Set Position Offset (X, -Z, Y)\n Set Depth Offset (depth)", true, 5, new byte[5]{ 1, 1, 2, 2, 2}, new string[5]{ "Sps", "Code", "Parameter 1", "Parameter 2", "Parameter 3"}, new string[5]{ "AT_USPIN", "AT_SPSCODE", "AT_SPIN", "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xB4, "SetPartyReserve", "Define the party member availability for a future Party call.\n\n1st argument: list of available characters.", true, 1, new byte[1]{ 2}, new string[1]{ "Characters available"}, new string[1]{ "AT_CHARACTER"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xB5, "0xB5", "Seem to somehow make the object appropriate itself another entry's function list.\n\n1st argument: entry to get functions from.", true, 1, new byte[1]{ 1}, new string[1]{ "Entry"}, new string[1]{ "AT_ENTRY"}, 0); // PRETEND
	yield return new SortedChoiceItemScriptOpcode(0xB6, "WorldMap", "Change the scene to a world map.\n\n1st argument: world map destination.", true, 1, new byte[1]{ 2}, new string[1]{ "World Map"}, new string[1]{ "AT_WORLDMAP"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xB7, "0xB7", "Unknown Opcode.", false, 0, null, null, null, 0); // EYE
	yield return new SortedChoiceItemScriptOpcode(0xB8, "0xB8", "Unknown Opcode.", false, 0, null, null, null, 0); // AIM
	yield return new SortedChoiceItemScriptOpcode(0xB9, "AddControllerMask", "Prevent the input to be processed by the game.\n\n1st argument: pad number (should only be 0 or 1).\n2nd argument: button list.\n1: Select\n4: Start\n5: Up\n6: Right\n7: Down\n8: Left\n9: L2\n10: R2\n11: L1\n12: R1\n13: Triangle\n14: Circle\n15: Cross\n16: Square\n17: Cancel\n18: Confirm\n20: Moogle\n21: L1 Ex\n22: R1 Ex\n23: L2 Ex\n24: R2 Ex\n25: Menu\n26: Select Ex", true, 2, new byte[2]{ 1, 2}, new string[2]{ "Pad", "Buttons"}, new string[2]{ "AT_USPIN", "AT_BUTTONLIST"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xBA, "RemoveControllerMask", "Enable back the controller's inputs.\n\n1st argument: pad number (should only be 0 or 1).\n2nd argument: button list.\n1: Select\n4: Start\n5: Up\n6: Right\n7: Down\n8: Left\n9: L2\n10: R2\n11: L1\n12: R1\n13: Triangle\n14: Circle\n15: Cross\n16: Square\n17: Cancel\n18: Confirm\n20: Moogle\n21: L1 Ex\n22: R1 Ex\n23: L2 Ex\n24: R2 Ex\n25: Menu\n26: Select Ex", true, 2, new byte[2]{ 1, 2}, new string[2]{ "Pad", "Buttons"}, new string[2]{ "AT_USPIN", "AT_BUTTONLIST"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xBB, "TimedTurnEx", "Make an object face an angle (animated).\n\n1st argument: object's entry.\n2nd argument: angle.\n0 faces south, 64 faces west, 128 faces north and 192 faces east.\n3rd argument: turn speed (1 is slowest).", true, 3, new byte[3]{ 1, 1, 1}, new string[3]{ "Object", "Angle", "Speed"}, new string[3]{ "AT_ENTRY", "AT_USPIN", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xBC, "WaitTurnEx", "Wait until an object facing movement has ended.\n\n1st argument: object's entry.", true, 1, new byte[1]{ 1}, new string[1]{ "Object"}, new string[1]{ "AT_ENTRY"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xBD, "RunAnimationEx", "Play an object's animation.\n\n1st argument: object's entry.\n2nd argument: animation ID.", true, 2, new byte[2]{ 1, 2}, new string[2]{ "Object", "Animation ID"}, new string[2]{ "AT_ENTRY", "AT_ANIMATION"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xBE, "WaitAnimationEx", "Wait until the object's animation has ended.\n\n1st argument: object's entry.", true, 1, new byte[1]{ 1}, new string[1]{ "Object"}, new string[1]{ "AT_ENTRY"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xBF, "MoveInstantEx", "Instantatly move an object.\n\n1st argument: object's entry.\n2nd and 3rd arguments: destination in (X, Y) format.", true, 3, new byte[3]{ 1, 2, 2}, new string[3]{ "Object", "DestinationX", "DestinationY"}, new string[3]{ "AT_ENTRY", "AT_POSITION_X", "AT_POSITION_Y"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xC0, "EnableTextureAnimation", "Run a model texture animation and make it loop.\n\n1st argument: model's entry.\n2nd argument: texture animation ID.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Object", "Texture Animation"}, new string[2]{ "AT_ENTRY", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xC1, "RunTextureAnimation", "Run once a model texture animation.\n\n1st argument: model's entry.\n2nd argument: texture animation ID.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Object", "Texture Animation"}, new string[2]{ "AT_ENTRY", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xC2, "StopTextureAnimation", "Stop playing the model texture animation.\n\n1st argument: model's entry.\n2nd argument: texture animation ID.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Object", "Texture Animation"}, new string[2]{ "AT_ENTRY", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xC3, "SetTileCamera", "Link a tile block to a specific field camera (useful for looping movement bounds).\n\n1st argument: background tile block.\n2nd argument: camera ID.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Tile Block", "Camera ID"}, new string[2]{ "AT_TILE", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xC4, "RunWorldCode", "Run one of the World Map codes, which effects have a large range. May modify the weather, the music, call the chocobo or enable the auto-pilot.\n\n1st argument: world code.\n2nd argument: depends on the code.", true, 2, new byte[2]{ 1, 2}, new string[2]{ "Code", "Argument"}, new string[2]{ "AT_WORLDCODE", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xC5, "RunSoundCode", "Same as RunSoundCode3( code, music, 0, 0, 0 ).", true, 2, new byte[2]{ 2, 2}, new string[2]{ "Code", "Sound"}, new string[2]{ "AT_SOUNDCODE", "AT_SOUND"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xC6, "RunSoundCode1", "Same as RunSoundCode3( code, music, arg1, 0, 0 ).", true, 3, new byte[3]{ 2, 2, 3}, new string[3]{ "Code", "Sound", "Argument"}, new string[3]{ "AT_SOUNDCODE", "AT_SOUND", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xC7, "RunSoundCode2", "Same as RunSoundCode3( code, music, arg1, arg2, 0 ).", true, 4, new byte[4]{ 2, 2, 3, 1}, new string[4]{ "Code", "Sound", "Argument", "Argument"}, new string[4]{ "AT_SOUNDCODE", "AT_SOUND", "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xC8, "RunSoundCode3", "Run a sound code.\n\n1st argument: sound code.\n2nd argument: music or sound to process.\n3rd to 5th arguments: depend on the sound code.", true, 5, new byte[5]{ 2, 2, 3, 1, 1}, new string[5]{ "Code", "Sound", "Argument", "Argument", "Argument"}, new string[5]{ "AT_SOUNDCODE", "AT_SOUND", "AT_SPIN", "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xC9, "0xC9", "Unknown Opcode.", true, 5, new byte[5]{ 1, 2, 2, 2, 2}, new string[5]{ "Unknown", "Unknown", "Unknown", "Unknown", "Unknown"}, new string[5]{ "AT_USPIN", "AT_USPIN", "AT_USPIN", "AT_USPIN", "AT_USPIN"}, 0); // EBG_overlayDefineViewport
	yield return new SortedChoiceItemScriptOpcode(0xCA, "0xCA", "Unknown Opcode.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Visible"}, new string[2]{ "AT_SPIN", "AT_BOOL"}, 0); // EBG_animSetVisible
	yield return new SortedChoiceItemScriptOpcode(0xCB, "EnablePath", "Enable a field path.\n\n1st argument: field path ID.\n2nd argument: boolean enable/disable.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Path", "Enable"}, new string[2]{ "AT_WALKPATH", "AT_BOOL"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xCC, "0xCC", "Maybe make the character transparent.", true, 1, new byte[1]{ 2}, new string[1]{ "Flag List"}, new string[1]{ "AT_BOOLLIST"}, 0); // CHRSET
	yield return new SortedChoiceItemScriptOpcode(0xCD, "0xCD", "Unknown Opcode.", true, 1, new byte[1]{ 2}, new string[1]{ "Flag List"}, new string[1]{ "AT_BOOLLIST"}, 0); // CHRCLEAR
	yield return new SortedChoiceItemScriptOpcode(0xCE, "AddGi", "Give gil to the player.\n\n1st argument: gil amount.", true, 1, new byte[1]{ 3}, new string[1]{ "Amount"}, new string[1]{ "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xCF, "RemoveGi", "Remove gil from the player.\n\n1st argument: gil amount.", true, 1, new byte[1]{ 3}, new string[1]{ "Amount"}, new string[1]{ "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xD0, "BattleDialog", "Display text in battle for 60 frames.\n\n1st argument: text to display.", true, 1, new byte[1]{ 2}, new string[1]{ "Text"}, new string[1]{ "AT_TEXT"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xD1, "0xD1", "Unknown Opcode; ignored in the non-PSX versions.", false, 0, null, null, null, 0); // "GLOBALCLEAR" Return 0 on Steam
	yield return new SortedChoiceItemScriptOpcode(0xD2, "0xD2", "Unknown Opcode; ignored in the non-PSX versions.", false, 0, null, null, null, 0); // "DEBUGSAVE" Return 0 on Steam
	yield return new SortedChoiceItemScriptOpcode(0xD3, "0xD3", "Unknown Opcode; ignored in the non-PSX versions.", false, 0, null, null, null, 0); // "DEBUGLOAD" Return 0 on Steam
	yield return new SortedChoiceItemScriptOpcode(0xD4, "0xD4", "Unknown Opcode.", true, 3, new byte[3]{ 2, 2, 2}, new string[3]{ "Unknown", "Unknown", "Unknown"}, new string[3]{ "AT_SPIN", "AT_SPIN", "AT_SPIN"}, 0); // geoAttachOffset
	yield return new SortedChoiceItemScriptOpcode(0xD5, "0xD5", "Unknown Opcode.", false, 0, null, null, null, 0); // PUSHHIDE
	yield return new SortedChoiceItemScriptOpcode(0xD6, "0xD6", "Unknown Opcode.", false, 0, null, null, null, 0); // POPSHOW
	yield return new SortedChoiceItemScriptOpcode(0xD7, "ATE", "Enable or disable ATE.\n\n1st argument: maybe flags (unknown format).", true, 1, new byte[1]{ 1}, new string[1]{ "Unknown"}, new string[1]{ "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xD8, "SetWeather", "Add a foreground effect.\n\n1st argument: unknown.\n2nd argument: unknown.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xD9, "CureStatus", "Cure the status ailments of a party member.\n\n1st argument: character.\n2nd argument: status list.\n 1: Petrified\n 2: Venom\n 3: Virus\n 4: Silence\n 5: Darkness\n 6: Trouble\n 7: Zombie", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Character", "Statuses"}, new string[2]{ "AT_LCHARACTER", "AT_BOOLLIST"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xDA, "RunSPSCodeSimple", "Run Sps code, which seems to be special model effects on the field.\n\n1st argument: sps ID.\n2nd argument: sps code.\n3rd to 5th arguments: depend on the sps code.\n Load Sps (sps type)\n Enable Attribute (attribute list, boolean enable/disable)\n Set Position (X, -Z, Y)\n Set Rotation (angle X, angle Z, angle Y)\n Set Scale (scale factor)\n Attach (object's entry to attach, bone number)\n Set Fade (fade)\n Set Animation Rate (rate)\n Set Frame Rate (rate)\n Set Frame (value) where the value is factored by 16 to get the frame\n Set Position Offset (X, -Z, Y)\n Set Depth Offset (depth)", true, 5, new byte[5]{ 1, 1, 1, 2, 2}, new string[5]{ "Sps", "Code", "Parameter 1", "Parameter 2", "Parameter 3"}, new string[5]{ "AT_USPIN", "AT_SPSCODE", "AT_SPIN", "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xDB, "EnableVictoryPose", "Enable or disable the victory pose at the end of battles for a specific character.\n\n1st argument: which character.\n2nd argument: boolean activate/deactivate.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Character", "Enable"}, new string[2]{ "AT_LCHARACTER", "AT_BOOL"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xDC, "Jump", "Perform a jumping animation. Must be used after a SetupJump call.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0xDD, "RemoveParty", "Remove a character from the player's team.\n\n1st argument: character to remove.", true, 1, new byte[1]{ 1}, new string[1]{ "Character"}, new string[1]{ "AT_LCHARACTER"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xDE, "SetName", "Change the name of a party member. Clear the text opcodes from the chosen text.\n\n1st argument: character to rename.\n2nd argument: new name.", true, 2, new byte[2]{ 1, 2}, new string[2]{ "Character", "Text"}, new string[2]{ "AT_LCHARACTER", "AT_TEXT"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xDF, "0xDF", "Unknown Opcode.", true, 1, new byte[1]{ 1}, new string[1]{ "Unknown"}, new string[1]{ "AT_USPIN"}, 0); // OVAL: posObj.ovalRatio
	yield return new SortedChoiceItemScriptOpcode(0xE0, "AddFrog", "Add one frog to the frog counter.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0xE1, "TerminateBattle", "Return to the field (or world map) when the rewards are disabled.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0xE2, "SetupJump", "Setup datas for a Jump call.\n\n1st to 3rd arguments: destination in (X, Z, Y) format.\n4th argument: number of steps for the jump.", true, 4, new byte[4]{ 2, 2, 2, 1}, new string[4]{ "DestinationX", "DestinationZ", "DestinationY", "Steps"}, new string[4]{ "AT_POSITION_X", "AT_POSITION_Z", "AT_POSITION_Y", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xE3, "SetDialogProgression", "Change the dialog progression value.\n\n1st argument: new value.", true, 1, new byte[1]{ 1}, new string[1]{ "Progression"}, new string[1]{ "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xE4, "0xE4", "Seem to move field tile while applying a loop effect to it.\n\n1st argument: background tile block.\n2nd argument: boolean activate/deactivate.\n3rd and 4th arguments: seems to be the movement in (X/256, Y/256) format.\n5th argument: unknown boolean.", true, 5, new byte[5]{ 1, 1, 2, 2, 1}, new string[5]{ "Tile Block", "Enable", "Delta", "Offset", "Is X Offset"}, new string[5]{ "AT_TILE", "AT_BOOL", "AT_SPIN", "AT_SPIN", "AT_BOOL"}, 0); // BGLSCROLLOFFSET: EBG_overlaySetScrollWithOffset
	yield return new SortedChoiceItemScriptOpcode(0xE5, "AttackSpecia", "Make the enemy instantatly use a special move. It doesn't use nor modify the battle state so it should be used when the battle is paused. The target(s) are to be set using the SV_Target variable.\n\n1st argument: attack to perform.", true, 1, new byte[1]{ 1}, new string[1]{ "Attack"}, new string[1]{ "AT_ATTACK"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xE6, "SetTileLoopType", "Let tile be screen anchored or not.\n\n1st argument: background tile block.\n2nd argument: boolean on/off.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Tile Block", "Screen Anchored"}, new string[2]{ "AT_TILE", "AT_BOOL"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xE7, "SetTileAnimationFrame", "Change the frame of a field tile animation (can be used to hide them all if the given frame is out of range, eg. 255).\n\n1st argument: background animation.\n2nd argument: animation frame to display.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Animation", "Frame ID"}, new string[2]{ "AT_TILEANIM", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xE8, "SideWalkXZY", "Make the character walk to destination without changing his facing angle. Make it synchronous if InitWalk is called before.\n\n1st to 3rd arguments: destination in (X, Z, Y) format.", true, 3, new byte[3]{ 2, 2, 2}, new string[3]{ "DestinationX","DestinationY","DestinationZ"}, new string[3]{ "AT_POSITION_X", "AT_POSITION_Z", "AT_POSITION_Y"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xE9, "UpdatePartyUI", "Update the party's menu icons and such.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0xEA, "0xEA", "Unknown Opcode.", false, 0, null, null, null, 0); // VRP: EBG_sceneGetVRP ; modify sSys Position
	yield return new SortedChoiceItemScriptOpcode(0xEB, "CloseAllWindows", "Close all the dialogs and UI windows.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0xEC, "FadeFilter", "Apply a fade filter on the screen.\n\n1st argument: filter mode (0 for ADD, 2 for SUBTRACT).\n2nd argument: fading time.\n3rd argument: unknown.\n4th to 6th arguments: color of the filter in (Cyan, Magenta, Yellow) format.", true, 6, new byte[6]{ 1, 1, 1, 1, 1, 1}, new string[6]{ "Fade In/Out", "Fading Time", "Unknown", "ColorC", "ColorM", "ColorY"}, new string[6]{ "AT_USPIN", "AT_USPIN", "AT_SPIN", "AT_COLOR_CYAN", "AT_COLOR_MAGENTA", "AT_COLOR_YELLOW"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xED, "0xED", "Unknown opcode about tile looping movements.\n\n1st argument: camera ID.\n2nd and 3rd arguments: unknown factors (X, Y).", true, 3, new byte[3]{ 1, 2, 2}, new string[3]{ "Camera ID", "Unknown X", "Unknown Y"}, new string[3]{ "AT_USPIN", "AT_SPIN", "AT_SPIN"}, 0); // BGVALPHA: EBG_overlayDefineViewportAlpha
	yield return new SortedChoiceItemScriptOpcode(0xEE, "EnableInactiveAnimation", "Allow the player's character to play its inactive animation. The inaction time required is:\nFirst Time = 200 + 4 * Random[0, 255]\nFollowing Times = 200 + 2 * Random[0, 255]", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0xEF, "ShowHereIcon", "Show the Here icon over player's chatacter.\n\n1st argument: display type (0 to hide, 3 to show unconditionally)", true, 1, new byte[1]{ 1}, new string[1]{ "Show"}, new string[1]{ "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xF0, "EnableRun", "Allow the player's character to run.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0xF1, "SetHP", "Change the HP of a party's member.\n\n1st argument: character.\n2nd argument: new HP value.", true, 2, new byte[2]{ 1, 2}, new string[2]{ "Character", "HP"}, new string[2]{ "AT_LCHARACTER", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xF2, "SetMP", "Change the MP of a party's member.\n\n1st argument: character.\n2nd argument: new MP value.", true, 2, new byte[2]{ 1, 2}, new string[2]{ "Character", "MP"}, new string[2]{ "AT_LCHARACTER", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xF3, "UnlearnAbility", "Set an ability's AP back to 0.\n\n1st argument: character.\n2nd argument: ability to reset.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Character", "Ability"}, new string[2]{ "AT_LCHARACTER", "AT_ABILITY"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xF4, "LearnAbility", "Make character learn an ability.\n\n1st argument: character.\n2nd argument: ability to learn.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Character", "Ability"}, new string[2]{ "AT_LCHARACTER", "AT_ABILITY"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xF5, "GameOver", "Terminate the game with a Game Over screen.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0xF6, "VibrateController", "Start the vibration lifespan.\n\n1st argument: frame to begin with.", true, 1, new byte[1]{ 1}, new string[1]{ "Frame"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xF7, "ActivateVibration", "Make the controller's vibration active. If the current controller's frame is out of the vibration time range, the vibration lifespan is reinit.\n\n1st argument: boolean activate/deactivate.", true, 1, new byte[1]{ 1}, new string[1]{ "Activate"}, new string[1]{ "AT_BOOL"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xF8, "RunVibrationTrack", "Run a vibration track.\n\n1st argument: track ID.\n2nd argument: sample (0 or 1).\n3rd argument: boolean activate/deactivate.", true, 3, new byte[3]{ 1, 1, 1}, new string[3]{ "Track", "Sample", "Activate"}, new string[3]{ "AT_USPIN", "AT_USPIN", "AT_BOOL"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xF9, "ActivateVibrationTrack", "Activate a vibration track.\n\n1st argument: track ID.\n2nd argument: sample (0 or 1).\n3rd argument: boolean activate/deactivate.", true, 3, new byte[3]{ 1, 1, 1}, new string[3]{ "Track", "Sample", "Activate"}, new string[3]{ "AT_USPIN", "AT_USPIN", "AT_BOOL"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xFA, "SetVibrationSpeed", "Set the vibration frame rate.\n\n1st argument: frame rate.", true, 1, new byte[1]{ 2}, new string[1]{ "Frame Rate"}, new string[1]{ "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xFB, "SetVibrationFlags", "Change the vibration flags.\n\n1st argument: flags.\n 8: Loop\n 16: Wrap", true, 1, new byte[1]{ 1}, new string[1]{ "Flags"}, new string[1]{ "AT_BOOLLIST"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xFC, "SetVibrationRange", "Set the time range of vibration.\n\n1st and 2nd arguments: vibration range in (Start, End) format.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Start", "End"}, new string[2]{ "AT_USPIN", "AT_USPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xFD, "PreloadField", "Surely preload a field; ignored in the non-PSX versions.\n\n1st argument: unknown.\n2nd argument: field to preload.", true, 2, new byte[2]{ 1, 2}, new string[2]{ "Unknown", "Field"}, new string[2]{ "AT_SPIN", "AT_FIELD"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xFE, "SetCharacterData", "Init a party's member battle and menu datas.\n\n1st argument: character.\n2nd argument: boolean update level/don't update level.\n3rd argument: equipement set to use.\n4th argument: character categories ; doesn't change if all are enabled.\n 1: male\n 2: female\n 3: gaian\n 4: terran\n 5: temporary character\n5th argument: ability and command set to use.", true, 5, new byte[5]{ 1, 1, 1, 1, 1}, new string[5]{ "Character", "Update Leve", "Equipement Set", "Category", "Ability Set"}, new string[5]{ "AT_LCHARACTER", "AT_BOOL", "AT_EQUIPSET", "AT_BOOLLIST", "AT_ABILITYSET"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0xFF, "EXTENDED_CODE", "Not an opcode.", false, 0, null, null, null, 0);
	yield return new SortedChoiceItemScriptOpcode(0x100, "0x100", "Unknown Opcode.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x101, "0x101", "Unknown Opcode.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x102, "0x102", "Unknown Opcode.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_USPIN", "AT_USPIN"}, 0); // BSACTIVE: walkMesh.BGI_simSetActive
	yield return new SortedChoiceItemScriptOpcode(0x103, "0x103", "Unknown Opcode.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_USPIN", "AT_USPIN"}, 0); // BSFLAG: walkMesh.BGI_simSetFlags
	yield return new SortedChoiceItemScriptOpcode(0x104, "0x104", "Unknown Opcode.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_USPIN", "AT_USPIN"}, 0); // BSFLOOR: walkMesh.BGI_simSetFloor
	yield return new SortedChoiceItemScriptOpcode(0x105, "0x105", "Unknown Opcode.", true, 2, new byte[2]{ 1, 2}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_USPIN", "AT_USPIN"}, 0); // BSRATE: walkMesh.BGI_simSetFrameRate
	yield return new SortedChoiceItemScriptOpcode(0x106, "0x106", "Unknown Opcode.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_USPIN", "AT_USPIN"}, 0); // BSALGO: walkMesh.BGI_simSetAlgorithm
	yield return new SortedChoiceItemScriptOpcode(0x107, "0x107", "Unknown Opcode.", true, 3, new byte[3]{ 1, 2, 2}, new string[3]{ "Unknown", "Unknown" , "Unknown"}, new string[3]{ "AT_USPIN", "AT_USPIN", "AT_USPIN"}, 0); // BSDELTA: walkMesh.BGI_simSetDelta
	yield return new SortedChoiceItemScriptOpcode(0x108, "0x108", "Unknown Opcode.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_USPIN", "AT_USPIN"}, 0); // BSAXIS: walkMesh.BGI_simSetAxis
	yield return new SortedChoiceItemScriptOpcode(0x109, "0x109", "Unknown Opcode.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x10A, "0x10A", "Unknown Opcode.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_USPIN", "AT_USPIN"}, 0); // BAFRAME: walkMesh.BGI_animShowFrame
	yield return new SortedChoiceItemScriptOpcode(0x10B, "0x10B", "Unknown Opcode.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x10C, "0x10C", "Unknown Opcode.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x10D, "0x10D", "Unknown Opcode.", true, 2, new byte[2]{ 1, 2}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x10E, "0x10E", "Unknown Opcode.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x10F, "0x10F", "Unknown Opcode.", true, 3, new byte[3]{ 1, 1, 1}, new string[3]{ "Unknown", "Unknown", "Unknown"}, new string[3]{ "AT_SPIN", "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x110, "0x110", "Unknown Opcode.", true, 3, new byte[3]{ 1, 1, 1}, new string[3]{ "Unknown", "Unknown", "Unknown"}, new string[3]{ "AT_SPIN", "AT_SPIN", "AT_SPIN"}, 0);
	yield return new SortedChoiceItemScriptOpcode(0x111, "0x111", "Unknown Opcode.", true, 2, new byte[2]{ 1, 1}, new string[2]{ "Unknown", "Unknown"}, new string[2]{ "AT_SPIN", "AT_SPIN"}, 0 );
        }
        
        private  sealed class SortedChoiceItemScriptOpcode {
                              	public uint id;
                              	public string label;
                              	public string help;
                              	public bool use_vararg;
                              	public int arg_amount;
                              	public byte[] arg_length;
                              	public string[] arg_help;
                              	public string[] arg_type;
                              	public uint jump_pos;

								public event_code_binary ecb => (event_code_binary)this.id;

                                public SortedChoiceItemScriptOpcode(uint id,
                                string label,
                                string help,
                                bool use_vararg,
                                int arg_amount,
                                byte[] arg_length,
                                string[] arg_help,
                                string[] arg_type,
                                uint jump_pos)
                                {
	                                this.id = id;
				this.label = label;
				this.help = help;
				this.use_vararg = use_vararg;
				this.arg_amount = arg_amount;
				this.arg_length = arg_length;
				this.arg_help = arg_help;
				this.arg_type = arg_type;
				this.jump_pos = jump_pos;
			}
        }
        public enum event_code_binary
    {
        NOP,
        rsv01,
        rsv02,
        rsv03,
        rsv04,
        EXPR,
        rsv06,
        NEW,
        NEW2,
        NEW3,
        pad0a,
        rsv0b,
        rsv0c,
        rsv0d,
        pad0e,
        pad0f,
        REQ,
        pad11,
        REQSW,
        pad13,
        REQEW,
        pad15,
        REPLY,
        pad17,
        REPLYSW,
        pad19,
        REPLYEW,
        SONGFLAG,
        DELETE,
        POS,
        BGVPORT,
        MES,
        MESN,
        CLOSE,
        WAIT,
        MOVE,
        MOVA,
        CLRDIST,
        MSPEED,
        BGIMASK,
        FMV,
        QUAD,
        ENCOUNT,
        MAPJUMP,
        CC,
        UCOFF,
        UCON,
        MODEL,
        PRINT1,
        PRINTF,
        LOCATE,
        AIDLE,
        AWALK,
        ARUN,
        DIRE,
        ROTXZ,
        BTLCMD,
        MESHSHOW,
        MESHHIDE,
        OBJINDEX,
        ENCSCENE,
        AFRAME,
        ASPEED,
        AMODE,
        ANIM,
        WAITANIM,
        ENDANIM,
        STARTSEQ,
        WAITSEQ,
        ENDSEQ,
        DEBUGCC,
        NECKFLAG,
        ITEMADD,
        ITEMDELETE,
        BTLSET,
        RADIUS,
        ATTACH,
        DETACH,
        WATCH,
        STOP,
        WAITTURN,
        TURNA,
        ASLEEP,
        NOINITMES,
        WAITMES,
        MROT,
        TURN,
        ENCRATE,
        BGSMOVE,
        BGLCOLOR,
        BGLMOVE,
        BGLACTIVE,
        BGLLOOP,
        BGLPARALLAX,
        BGLORIGIN,
        BGAANIME,
        BGAACTIVE,
        BGARATE,
        SETROW,
        BGAWAIT,
        BGAFLAG,
        BGARANGE,
        MESVALUE,
        TWIST,
        FICON,
        TIMERSET,
        DASHOFF,
        CLEARCOLOR,
        PPRINT,
        PPRINTF,
        MAPID,
        BGSSCROLL,
        BGSRELEASE,
        BGCACTIVE,
        BGCHEIGHT,
        BGCLOCK,
        BGCUNLOCK,
        MENU,
        TRACKSTART,
        TRACK,
        TRACKADD,
        PRINTQUAD,
        ATURNL,
        ATURNR,
        CHOOSEPARAM,
        TIMERCONTROL,
        SETCAM,
        SHADOWON,
        SHADOWOFF,
        SHADOWSCALE,
        SHADOWOFFSET,
        SHADOWLOCK,
        SHADOWUNLOCK,
        SHADOWAMP,
        IDLESPEED,
        DDIR,
        CHRFX,
        SEPV,
        SEPVA,
        NECKID,
        ENCOUNT2,
        TIMERDISPLAY,
        RAISE,
        CHRCOLOR,
        SLEEPINH,
        AUTOTURN,
        BGLATTACH,
        CFLAG,
        AJUMP,
        MESA,
        MESAN,
        DRET,
        MOVT,
        TSPEED,
        BGIACTIVET,
        TURNTO,
        PREJUMP,
        POSTJUMP,
        MOVQ,
        CHRSCALE,
        MOVJ,
        POS3,
        MOVE3,
        DRADIUS,
        MJPOS,
        MOVH,
        SPEEDTH,
        TURNDS,
        BGI,
        GETSCREEN,
        MENUON,
        MENUOFF,
        DISCCHANGE,
        DPOS3,
        MINIGAME,
        DELETEALLCARD,
        SETMAPNAME,
        RESETMAPNAME,
        PARTYMENU,
        SPS,
        FULLMEMBER,
        PRETEND,
        WMAPJUMP,
        EYE,
        AIM,
        SETKEYMASK,
        CLEARKEYMASK,
        DTURN,
        DWAITTURN,
        DANIM,
        DWAITANIM,
        DPOS,
        TEXPLAY,
        TEXPLAY1,
        TEXSTOP,
        BGVSET,
        WPRM,
        FLDSND0,
        FLDSND1,
        FLDSND2,
        FLDSND3,
        BGVDEFINE,
        BGAVISIBLE,
        BGIACTIVEF,
        CHRSET,
        CHRCLEAR,
        GILADD,
        GILDELETE,
        MESB,
        GLOBALCLEAR,
        DEBUGSAVE,
        DEBUGLOAD,
        ATTACHOFFSET,
        PUSHHIDE,
        POPSHOW,
        AICON,
        RAIN,
        CLEARSTATUS,
        SPS2,
        WINPOSE,
        JUMP3,
        PARTYDELETE,
        PLAYERNAME,
        OVAL,
        INCFROG,
        BEND,
        SETVY3,
        SETSIGNAL,
        BGLSCROLLOFFSET,
        BTLSEQ,
        BGLLOOPTYPE,
        BGAFRAME,
        MOVE3H,
        SYNCPARTY,
        VRP,
        CLOSEALL,
        WIPERGB,
        BGVALPHA,
        SLEEPON,
        HEREON,
        DASHON,
        SETHP,
        SETMP,
        CLEARAP,
        MAXAP,
        GAMEOVER,
        VIBSTART,
        VIBACTIVE,
        VIBTRACK1,
        VIBTRACK,
        VIBRATE,
        VIBFLAG,
        VIBRANGE,
        HINT,
        JOIN,
        EXT
    }
    };
}