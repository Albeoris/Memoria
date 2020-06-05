namespace FF8.JSM
{
    public static partial class Jsm
    {
        public enum Opcode
        {
            /// <summary>
            /// NOTHING
            /// Do nothing.
            /// </summary>
            NOP = 0x00,

            /// <summary>
            /// JMP
            /// Skip some operations.
            /// WARNING: unsafe to use.
            /// AT_JUMP Jump (2 bytes)
            /// </summary>
            JMP = 0x01,

            /// <summary>
            /// JMP_IFN
            /// Skip some operations if the stack value is not 0.
            /// WARNING: unsafe to use.
            /// AT_JUMP Jump (2 bytes)
            /// </summary>
            JMP_IFN = 0x02,

            /// <summary>
            /// JMP_IF
            /// Skip some operations if the stack value is 0.
            /// WARNING: unsafe to use.
            /// AT_JUMP Jump (2 bytes)
            /// </summary>
            JMP_IF = 0x03,

            /// <summary>
            /// return
            /// End the function.
            /// </summary>
            Return = 0x04,

            /// <summary>
            /// set
            /// Perform variable operations and store the result in the stack.
            /// AT_NONE Variable Code (0 bytes)
            /// </summary>
            EXPR = 0x05,

            /// <summary>
            /// JMP_SWITCHEX
            /// Skip some operations depending on the stack value.
            /// WARNING: unsafe to use.
            /// Negative args: -1
            /// </summary>
            JMP_SWITCHEX = 0x06,

            /// <summary>
            /// InitCode
            /// Init a normal code (independant functions).
            /// 
            /// 1st argument: code entry to init.
            /// 2nd argument: Unique ID (defaulted to entry's ID if 0).
            /// AT_ENTRY Entry (1 bytes)
            /// AT_USPIN UID (1 bytes)
            /// </summary>
            NEW = 0x07,

            /// <summary>
            /// InitRegion
            /// Init a region code (associated with a region).
            /// 
            /// 1st argument: code entry to init.
            /// 2nd argument: Unique ID (defaulted to entry's ID if 0).
            /// AT_ENTRY Entry (1 bytes)
            /// AT_USPIN UID (1 bytes)
            /// </summary>
            NEW2 = 0x08,

            /// <summary>
            /// InitObject
            /// Init an object code (associated with a 3D model). Also load the related model into the RAM.
            /// 
            /// 1st argument: code entry to init.
            /// 2nd argument: Unique ID (defaulted to entry's ID if 0).
            /// AT_ENTRY Entry (1 bytes)
            /// AT_USPIN UID (1 bytes)
            /// </summary>
            NEW3 = 0x09,

            /// <summary>
            /// 0xA
            /// Unused Opcode.
            /// </summary>
            pad0a = 0x0A,

            /// <summary>
            /// JMP_SWITCH
            /// Skip some operations depending on the stack value.
            /// WARNING: unsafe to use.
            /// Negative args: -1
            /// </summary>
            rsv0b = 0x0B,

            /// <summary>
            /// 0xC
            /// Unknown Opcode.
            /// </summary>
            rsv0c = 0x0C,

            /// <summary>
            /// 0xD
            /// Unknown Opcode.
            /// </summary>
            rsv0d = 0x0D,

            /// <summary>
            /// 0xE
            /// Unused Opcode.
            /// </summary>
            pad0e = 0x0E,

            /// <summary>
            /// 0xF
            /// Unused Opcode.
            /// </summary>
            pad0f = 0x0F,

            /// <summary>
            /// RunScriptAsync
            /// Run script function and continue executing the current one.
            /// 
            /// Entry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not run. Otherwise, the entry's script level is set to the specified script level until the function returns.
            /// 
            /// 1st argument: script level.
            /// 2nd argument: entry of the function.
            /// 3rd argument: function.
            /// AT_SCRIPTLVL Script Leve (1 bytes)
            /// AT_ENTRY Entry (1 bytes)
            /// AT_FUNCTION Function (1 bytes)
            /// </summary>
            REQ = 0x10,

            /// <summary>
            /// 0x11
            /// Unused Opcode.
            /// </summary>
            pad11 = 0x11,

            /// <summary>
            /// RunScript
            /// Wait until the entry's script level gets higher than the specified script level then run the script function and continue executing the current one.
            /// 
            /// Entry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not run. Otherwise, the entry's script level is set to the specified script level until the function returns.
            /// 
            /// 1st argument: script level.
            /// 2nd argument: entry of the function.
            /// 3rd argument: function.
            /// AT_SCRIPTLVL Script Leve (1 bytes)
            /// AT_ENTRY Entry (1 bytes)
            /// AT_FUNCTION Function (1 bytes)
            /// </summary>
            REQSW = 0x12,

            /// <summary>
            /// 0x13
            /// Unused Opcode.
            /// </summary>
            pad13 = 0x13,

            /// <summary>
            /// RunScriptSync
            /// Wait until the entry's script level gets higher than the specified script level then run the script function and wait until it returns.
            /// 
            /// Entry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not run. Otherwise, the entry's script level is set to the specified script level until the function returns.
            /// 
            /// 1st argument: script level.
            /// 2nd argument: entry of the function.
            /// 3rd argument: function.
            /// AT_SCRIPTLVL Script Leve (1 bytes)
            /// AT_ENTRY Entry (1 bytes)
            /// AT_FUNCTION Function (1 bytes)
            /// </summary>
            REQEW = 0x14,

            /// <summary>
            /// 0x15
            /// Unused Opcode.
            /// </summary>
            pad15 = 0x15,

            /// <summary>
            /// RunScriptObjectAsync
            /// Run script function and continue executing the current one. Must only be used in response to a function call ; the argument's entry is the one that called this function.
            /// 
            /// Entry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not run. Otherwise, the entry's script level is set to the specified script level until the function returns.
            /// 
            /// 1st argument: script level.
            /// 2nd argument: function.
            /// AT_SCRIPTLVL Script Leve (1 bytes)
            /// AT_FUNCTION Function (1 bytes)
            /// </summary>
            REPLY = 0x16,

            /// <summary>
            /// 0x17
            /// Unused Opcode.
            /// </summary>
            pad17 = 0x17,

            /// <summary>
            /// RunScriptObject
            /// Wait until the entry's script level gets higher than the specified script level then run the script function and continue executing the current one. Must only be used in response to a function call ; the argument's entry is the one that called this function.
            /// 
            /// Entry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not run. Otherwise, the entry's script level is set to the specified script level until the function returns.
            /// 
            /// 1st argument: script level.
            /// 2nd argument: function.
            /// AT_SCRIPTLVL Script Leve (1 bytes)
            /// AT_FUNCTION Function (1 bytes)
            /// </summary>
            REPLYSW = 0x18,

            /// <summary>
            /// 0x19
            /// Unused Opcode.
            /// </summary>
            pad19 = 0x19,

            /// <summary>
            /// RunScriptObjectSync
            /// Wait until the entry's script level gets higher than the specified script level then run the script function and wait until it returns. Must only be used in response to a function call ; the argument's entry is the one that called this function.
            /// 
            /// Entry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not run. Otherwise, the entry's script level is set to the specified script level until the function returns.
            /// 
            /// 1st argument: script level.
            /// 2nd argument: function.
            /// AT_SCRIPTLVL Script Leve (1 bytes)
            /// AT_FUNCTION Function (1 bytes)
            /// </summary>
            REPLYEW = 0x1A,

            /// <summary>
            /// ContinueBattleMusic
            /// Continue the music after the battle end.
            /// 
            /// 1st argument: flag continue/don't continue.
            /// AT_BOOL Continue (1 bytes)
            /// </summary>
            SONGFLAG = 0x1B,

            /// <summary>
            /// TerminateEntry
            /// Stop the execution of an entry's code.
            /// 
            /// 1st argument: entry to terminate.
            /// AT_ENTRY Object (1 bytes)
            /// </summary>
            DELETE = 0x1C,

            /// <summary>
            /// CreateObject
            /// Place (or replace) the 3D model on the field.
            /// 
            /// 1st and 2nd arguments: position in (X, Y) format.
            /// AT_POSITION_X PositionX (2 bytes)
            /// AT_POSITION_Y PositionY (2 bytes)
            /// </summary>
            POS = 0x1D,

            /// <summary>
            /// SetCameraBounds
            /// Redefine the field camera boundaries (default value is part of the background's data).
            /// 
            /// 1st argument: camera ID.
            /// 2nd to 5th arguments: Boundaries in (MinX, MaxX, MinY, MaxY) format.
            /// AT_USPIN Camera (1 bytes)
            /// AT_SPIN Min X (2 bytes)
            /// AT_SPIN Max X (2 bytes)
            /// AT_SPIN Min Y (2 bytes)
            /// AT_SPIN Max Y (2 bytes)
            /// </summary>
            BGVPORT = 0x1E,

            /// <summary>
            /// WindowSync
            /// Display a window with text inside and wait until it closes.
            /// 
            /// 1st argument: window ID.
            /// 2nd argument: UI flag list.
            ///  3: disable bubble tail
            ///  4: mognet format
            ///  5: hide window
            ///  7: ATE window
            ///  8: dialog window
            /// 3rd argument: text to display.
            /// AT_USPIN Window ID (1 bytes)
            /// AT_BOOLLIST UI (1 bytes)
            /// AT_TEXT Text (2 bytes)
            /// </summary>
            MES = 0x1F,

            /// <summary>
            /// WindowAsync
            /// Display a window with text inside and continue the execution of the script without waiting.
            /// 
            /// 1st argument: window ID.
            /// 2nd argument: UI flag list.
            ///  3: disable bubble tail
            ///  4: mognet format
            ///  5: hide window
            ///  7: ATE window
            ///  8: dialog window
            /// 3rd argument: text to display.
            /// AT_USPIN Window ID (1 bytes)
            /// AT_BOOLLIST UI (1 bytes)
            /// AT_TEXT Text (2 bytes)
            /// </summary>
            MESN = 0x20,

            /// <summary>
            /// CloseWindow
            /// Close a window.
            /// 
            /// 1st argument: window ID determined at its creation.
            /// AT_USPIN Window ID (1 bytes)
            /// </summary>
            CLOSE = 0x21,

            /// <summary>
            /// Wait
            /// Wait some time.
            /// 
            /// 1st argument: amount of frames to wait.
            ///  For PAL, 1 frame is 0.04 seconds.
            ///  For other versions, 1 frame is about 0.033 seconds.
            /// AT_USPIN Frame Amount (1 bytes)
            /// </summary>
            WAIT = 0x22,

            /// <summary>
            /// Walk
            /// Make the character walk to destination. Make it synchronous if InitWalk is called before.
            /// 
            /// 1st and 2nd arguments: destination in (X, Y) format.
            /// AT_POSITION_X DestinationX (2 bytes)
            /// AT_POSITION_Y DestinationY (2 bytes)
            /// </summary>
            MOVE = 0x23,

            /// <summary>
            /// WalkTowardObject
            /// Make the character walk and follow an object.
            /// 
            /// 1st argument: object to walk toward.
            /// AT_ENTRY Object (1 bytes)
            /// </summary>
            MOVA = 0x24,

            /// <summary>
            /// InitWalk
            /// Make a further Walk call (or variations of Walk) synchronous.
            /// </summary>
            CLRDIST = 0x25,

            /// <summary>
            /// SetWalkSpeed
            /// Change the walk speed.
            /// 
            /// 1st argument: speed (surely in unit/frame).
            /// AT_USPIN Speed (1 bytes)
            /// </summary>
            MSPEED = 0x26,

            /// <summary>
            /// SetTriangleFlagMask
            /// Set a bitmask for some of the walkmesh triangle flags.
            /// 
            /// 1st argument: flag mask.
            ///  7: disable restricted triangles
            ///  8: disable player-restricted triangles
            /// AT_BOOLLIST Flags (1 bytes)
            /// </summary>
            BGIMASK = 0x27,

            /// <summary>
            /// Cinematic
            /// Run or setup a cinematic.
            /// 
            /// 1st argument: unknown.
            /// 2nd argument: cinematic ID (may depends on 1st argument's value).
            /// 3rd argument: unknown.
            /// 4th argument: unknown.
            /// AT_SPIN Unknown (1 bytes)
            /// AT_FMV Cinematic ID (1 bytes)
            /// AT_SPIN Unknown (1 bytes)
            /// AT_SPIN Unknown (1 bytes)
            /// </summary>
            FMV = 0x28,

            /// <summary>
            /// SetRegion
            /// Define the polygonal region linked with the entry script.
            /// 
            /// Arguments are in the format (Vertice X, Vertice Y) and can be of any amount.
            /// Use arg: True
            /// Negative args: -1
            /// </summary>
            QUAD = 0x29,

            /// <summary>
            /// Battle
            /// Start a battle (using a random enemy group).
            /// 
            /// 1st argument: rush type (unknown).
            /// 2nd argument: gathered battle and Steiner's state (highest bit) informations.
            /// AT_SPIN Rush Type (1 bytes)
            /// AT_BATTLE Battle (2 bytes)
            /// </summary>
            ENCOUNT = 0x2A,

            /// <summary>
            /// Field
            /// Change the field scene.
            /// 
            /// 1st argument: field scene destination.
            /// AT_FIELD Field (2 bytes)
            /// </summary>
            MAPJUMP = 0x2B,

            /// <summary>
            /// DefinePlayerCharacter
            /// Apply the player's control over the entry's object.
            /// </summary>
            CC = 0x2C,

            /// <summary>
            /// DisableMove
            /// Disable the player's movement control.
            /// </summary>
            UCOFF = 0x2D,

            /// <summary>
            /// EnableMove
            /// Enable the player's movement control.
            /// </summary>
            UCON = 0x2E,

            /// <summary>
            /// SetMode
            /// Set the model of the object and its head's height (used to set the dialog box's height).
            /// 
            /// 1st argument: model.
            /// 2nd argument: head's height.
            /// AT_MODEL Mode (2 bytes)
            /// AT_USPIN Height (1 bytes)
            /// </summary>
            MODEL = 0x2F,

            /// <summary>
            /// 0x30
            /// Unknown Opcode; ignored in the non-PSX versions.
            /// </summary>
            PRINT1 = 0x30,

            /// <summary>
            /// 0x31
            /// Unknown Opcode; ignored in the non-PSX versions.
            /// </summary>
            PRINTF = 0x31,

            /// <summary>
            /// 0x32
            /// Unused Opcode; bugs if used in the non-PSX versions.
            /// AT_SPIN Unknown (1 bytes)
            /// AT_SPIN Unknown (1 bytes)
            /// </summary>
            LOCATE = 0x32,

            /// <summary>
            /// SetStandAnimation
            /// Change the standing animation.
            /// 
            /// 1st argument: animation ID.
            /// AT_ANIMATION Animation (2 bytes)
            /// </summary>
            AIDLE = 0x33,

            /// <summary>
            /// SetWalkAnimation
            /// Change the walking animation.
            /// 
            /// 1st argument: animation ID.
            /// AT_ANIMATION Animation (2 bytes)
            /// </summary>
            AWALK = 0x34,

            /// <summary>
            /// SetRunAnimation
            /// Change the running animation.
            /// 
            /// 1st argument: animation ID.
            /// AT_ANIMATION Animation (2 bytes)
            /// </summary>
            ARUN = 0x35,

            /// <summary>
            /// TurnInstant
            /// Make the character face an angle.
            /// 
            /// 1st argument: angle.
            /// 0 faces south, 64 faces west, 128 faces north and 192 faces east.
            /// AT_USPIN Angle (1 bytes)
            /// </summary>
            DIRE = 0x36,

            /// <summary>
            /// SetPitchAngle
            /// Turn the model in the up/down direction.
            /// 
            /// 1st argument: angle (pitch axis).
            /// 2nd argument: angle (XZ axis).
            /// AT_USPIN Pitch (1 bytes)
            /// AT_USPIN XZ Angle (1 bytes)
            /// </summary>
            ROTXZ = 0x37,

            /// <summary>
            /// Attack
            /// Make the enemy attack. The target(s) are to be set using the SV_Target variable.
            /// Inside an ATB function, the attack is added to the queue.
            /// Inside a counter function, the attack occurs directly.
            /// 
            /// 1st argument: attack to perform.
            /// AT_ATTACK Attack (1 bytes)
            /// </summary>
            BTLCMD = 0x38,

            /// <summary>
            /// ShowObject
            /// Show an object.
            /// 
            /// 1st argument: object.
            /// 2nd argument: unknown.
            /// AT_ENTRY Object (1 bytes)
            /// AT_USPIN Unknown (1 bytes)
            /// </summary>
            MESHSHOW = 0x39,

            /// <summary>
            /// HideObject
            /// Hide an object.
            /// 
            /// 1st argument: object.
            /// 2nd argument: unknown.
            /// AT_ENTRY Object (1 bytes)
            /// AT_USPIN Unknown (1 bytes)
            /// </summary>
            MESHHIDE = 0x3A,

            /// <summary>
            /// SetObjectIndex
            /// Redefine the current object's index.
            /// 
            /// 1st argument: new index.
            /// AT_USPIN Index (1 bytes)
            /// </summary>
            OBJINDEX = 0x3B,

            /// <summary>
            /// SetRandomBattles
            /// Define random battles.
            /// 
            /// 1st argument: pattern, deciding the encounter chances and the topography (World Map only).
            ///  0: {0.375, 0.28, 0.22, 0.125}
            ///  1: {0.25, 0.25, 0.25, 0.25}
            ///  2: {0.35, 0.3, 0.3, 0.05}
            ///  3: {0.45, 0.4, 0.1, 0.05}
            /// 2nd to 5th arguments: possible random battles.
            /// AT_USPIN Pattern (1 bytes)
            /// AT_BATTLE Battle 1 (2 bytes)
            /// AT_BATTLE Battle 2 (2 bytes)
            /// AT_BATTLE Battle 3 (2 bytes)
            /// AT_BATTLE Battle 4 (2 bytes)
            /// </summary>
            ENCSCENE = 0x3C,

            /// <summary>
            /// 0x3D
            /// Some animation flags.
            /// 
            /// 1st argument: unknown.
            /// 2nd argument: unknown.
            /// AT_USPIN In-Frame (1 bytes)
            /// AT_USPIN Out-Frame (1 bytes)
            /// </summary>
            AFRAME = 0x3D,

            /// <summary>
            /// SetAnimationSpeed
            /// Set the current object's animation speed.
            /// 
            /// 1st argument: speed.
            /// AT_USPIN Speed (1 bytes)
            /// </summary>
            ASPEED = 0x3E,

            /// <summary>
            /// SetAnimationFlags
            /// Set the current object's next animation looping flags.
            /// 
            /// 1st argument: looping flag list.
            ///  1: freeze at end
            ///  2: loop
            ///  3: loop back and forth
            /// 2nd arguments: times to repeat.
            /// AT_ANIMFLAG Flag (1 bytes)
            /// AT_USPIN Repeat (1 bytes)
            /// </summary>
            AMODE = 0x3F,

            /// <summary>
            /// RunAnimation
            /// Make the character play an animation.
            /// 
            /// 1st argument: animation ID.
            /// AT_ANIMATION Animation (2 bytes)
            /// </summary>
            ANIM = 0x40,

            /// <summary>
            /// WaitAnimation
            /// Wait until the current object's animation has ended.
            /// </summary>
            WAITANIM = 0x41,

            /// <summary>
            /// StopAnimation
            /// Stop the character's animation.
            /// </summary>
            ENDANIM = 0x42,

            /// <summary>
            /// RunSharedScript
            /// Run script passing the current object to it and continue executing the current function. If another shared script is already running for this object, it will be terminated.
            /// 
            /// 1st argument: entry (should be a one-function entry).
            /// AT_ENTRY Entry (1 bytes)
            /// </summary>
            STARTSEQ = 0x43,

            /// <summary>
            /// WaitSharedScript
            /// Wait until the ran shared script has ended.
            /// </summary>
            WAITSEQ = 0x44,

            /// <summary>
            /// StopSharedScript
            /// Terminate the execution of the ran shared script.
            /// </summary>
            ENDSEQ = 0x45,

            /// <summary>
            /// 0x46
            /// No use.
            /// </summary>
            DEBUGCC = 0x46,

            /// <summary>
            /// EnableHeadFocus
            /// Enable or disable the character turning his head toward an active object.
            /// 
            /// 1st argument: flags.
            ///  1: unknown
            ///  2: unknown
            ///  3: turn toward talkers
            /// AT_BOOLLIST Flags (1 bytes)
            /// </summary>
            NECKFLAG = 0x47,

            /// <summary>
            /// AddItem
            /// Add item to the player's inventory. Only one copy of key items can be in the player's inventory.
            /// 
            /// 1st argument: item to add.
            /// 2nd argument: amount to add.
            /// AT_ITEM Item (2 bytes)
            /// AT_USPIN Amount (1 bytes)
            /// </summary>
            ITEMADD = 0x48,

            /// <summary>
            /// RemoveItem
            /// Remove item from the player's inventory.
            /// 
            /// 1st argument: item to remove.
            /// 2nd argument: amount to remove.
            /// AT_ITEM Item (2 bytes)
            /// AT_USPIN Amount (1 bytes)
            /// </summary>
            ITEMDELETE = 0x49,

            /// <summary>
            /// RunBattleCode
            /// Run a battle code.
            /// 
            /// 1st argument: battle code.
            /// 2nd argument: depends on the battle code.
            ///  End Battle: 0 for a defeat (deprecated), 1 for a victory, 2 for a victory without victory pose, 3 for a defeat, 4 for an escape, 5 for an interrupted battle, 6 for a game over, 7 for an enemy escape.
            ///  Run Camera: Camera ID.
            ///  Change Field: ID of the destination field after the battle.
            ///  Add Gil: amount to add.
            /// AT_BATTLECODE Code (1 bytes)
            /// AT_SPIN Argument (2 bytes)
            /// </summary>
            BTLSET = 0x4A,

            /// <summary>
            /// SetObjectLogicalSize
            /// Set different size informations of the object.
            /// 
            /// 1st argument: size for pathing.
            /// 2nd argument: collision radius.
            /// 3rd argument: talk distance.
            /// AT_SPIN Walk Radius (1 bytes)
            /// AT_USPIN Collision Radius (1 bytes)
            /// AT_USPIN Talk Distance (1 bytes)
            /// </summary>
            RADIUS = 0x4B,

            /// <summary>
            /// AttachObject
            /// Attach an object to another one.
            /// 
            /// 1st argument: carried object.
            /// 2nd argument: carrying object.
            /// 3rd argument: attachment point (unknown format).
            /// AT_ENTRY Object (1 bytes)
            /// AT_ENTRY Carrier (1 bytes)
            /// AT_USPIN Attachement Point (1 bytes)
            /// </summary>
            ATTACH = 0x4C,

            /// <summary>
            /// DetachObject
            /// Stop attaching an object to another one.
            /// 
            /// 1st argument: carried object.
            /// AT_ENTRY Object (1 bytes)
            /// </summary>
            DETACH = 0x4D,

            /// <summary>
            /// 0x4E
            /// Unknown Opcode.
            /// </summary>
            WATCH = 0x4E,

            /// <summary>
            /// 0x4F
            /// Unknown Opcode.
            /// AT_SPIN Unknown (1 bytes)
            /// </summary>
            STOP = 0x4F,

            /// <summary>
            /// WaitTurn
            /// Wait until the character has turned.
            /// </summary>
            WAITTURN = 0x50,

            /// <summary>
            /// TurnTowardObject
            /// Turn the character toward an entry object (animated).
            /// 
            /// 1st argument: object.
            /// 2nd argument: turn speed (1 is slowest).
            /// AT_ENTRY Object (1 bytes)
            /// AT_USPIN Speed (1 bytes)
            /// </summary>
            TURNA = 0x51,

            /// <summary>
            /// SetInactiveAnimation
            /// Change the animation played when inactive for a long time. The inaction time required is:
            /// First Time = 200 + 4 * Random[0, 255]
            /// Subsequent Times = 200 + 2 * Random[0, 255]
            /// 
            /// 1st argument: animation ID.
            /// AT_ANIMATION Animation (2 bytes)
            /// </summary>
            ASLEEP = 0x52,

            /// <summary>
            /// 0x53
            /// Seems to prevent new windows to close older ones.
            /// </summary>
            NOINITMES = 0x53,

            /// <summary>
            /// WaitWindow
            /// Wait until the window is closed.
            /// 
            /// 1st argument: window ID determined at its creation.
            /// AT_USPIN Window ID (1 bytes)
            /// </summary>
            WAITMES = 0x54,

            /// <summary>
            /// SetWalkTurnSpeed
            /// Change the turn speed of the object when it walks or runs (default is 16).
            /// 
            /// 1st argument: turn speed (with 0, the object doesn't turn while moving).
            /// 
            /// Special treatments:
            /// Vivi's in Iifa Tree/Eidolon Moun (field 1656) is initialized to 48.
            /// Choco's in Chocobo's Paradise (field 2954) is initialized to 96.
            /// AT_USPIN Turn Speed (1 bytes)
            /// </summary>
            MROT = 0x55,

            /// <summary>
            /// TimedTurn
            /// Make the character face an angle (animated).
            /// 
            /// 1st argument: angle.
            /// 0 faces south, 64 faces west, 128 faces north and 192 faces east.
            /// 2nd argument: turn speed (1 is slowest).
            /// AT_USPIN Angle (1 bytes)
            /// AT_USPIN Speed (1 bytes)
            /// </summary>
            TURN = 0x56,

            /// <summary>
            /// SetRandomBattleFrequency
            /// Set the frequency of random battles.
            /// 
            /// 1st argument: frequency.
            ///  255 is the maximum frequency, corresponding to ~12 walking steps or ~7 running steps.
            ///  0 is the minimal frequency and disables random battles.
            /// AT_USPIN Frequency (1 bytes)
            /// </summary>
            ENCRATE = 0x57,

            /// <summary>
            /// SlideXZY
            /// Make the character slide to destination (walk without using the walk animation and without changing the facing angle).
            /// 
            /// 1st to 3rd arguments: destination in (X, Z, Y) format.
            /// AT_POSITION_X DestinationX (2 bytes)
            /// AT_POSITION_Z DestinationZ (2 bytes)
            /// AT_POSITION_Y DestinationY (2 bytes)
            /// </summary>
            BGSMOVE = 0x58,

            /// <summary>
            /// SetTileColor
            /// Change the color of a field tile block.
            /// 
            /// 1st argument: background tile block.
            /// 2nd to 4th arguments: color in (Cyan, Magenta, Yellow) format.
            /// AT_TILE Tile Block (1 bytes)
            /// AT_COLOR_CYAN ColorC (1 bytes)
            /// AT_COLOR_MAGENTA ColorM (1 bytes)
            /// AT_COLOR_YELLOW ColorY (1 bytes)
            /// </summary>
            BGLCOLOR = 0x59,

            /// <summary>
            /// SetTilePositionEx
            /// Move a field tile block.
            /// 
            /// 1st argument: background tile block.
            /// 2nd and 3rd argument: position in (X, Y) format.
            /// 4th argument: closeness, defining whether 3D models are over or under that background tile.
            /// AT_TILE Tile Block (1 bytes)
            /// AT_SPIN Position X (2 bytes)
            /// AT_SPIN Position Y (2 bytes)
            /// AT_SPIN Position Closeness (2 bytes)
            /// </summary>
            BGLMOVE = 0x5A,

            /// <summary>
            /// ShowTile
            /// Show or hide a field tile block.
            /// 
            /// 1st argument: background tile block.
            /// 2nd argument: boolean show/hide.
            /// AT_TILE Tile Block (1 bytes)
            /// AT_BOOL Show (1 bytes)
            /// </summary>
            BGLACTIVE = 0x5B,

            /// <summary>
            /// MoveTileLoop
            /// Make the image of a field tile loop over space.
            /// 
            /// 1st argument: background tile block.
            /// 2nd argument: boolean on/off.
            /// 3rd and 4th arguments: speed in the X and Y directions.
            /// AT_TILE Tile Block (1 bytes)
            /// AT_BOOL Activate (1 bytes)
            /// AT_SPIN X Loop (2 bytes)
            /// AT_SPIN Y Loop (2 bytes)
            /// </summary>
            BGLLOOP = 0x5C,

            /// <summary>
            /// MoveTile
            /// Make the field moves depending on the camera position.
            /// 
            /// 1st argument: background tile block.
            /// 2nd argument: boolean on/off.
            /// 3rd and 4th arguments: parallax movement in (X, Y) format.
            /// AT_TILE Tile Block (1 bytes)
            /// AT_BOOL Activate (1 bytes)
            /// AT_SPIN Movement X (2 bytes)
            /// AT_SPIN Movement Y (2 bytes)
            /// </summary>
            BGLPARALLAX = 0x5D,

            /// <summary>
            /// SetTilePosition
            /// Move a field tile block.
            /// 
            /// 1st argument: background tile block.
            /// 2nd and 3rd argument: position in (X, Y) format.
            /// AT_TILE Tile Block (1 bytes)
            /// AT_SPIN Position X (2 bytes)
            /// AT_SPIN Position Y (2 bytes)
            /// </summary>
            BGLORIGIN = 0x5E,

            /// <summary>
            /// RunTileAnimation
            /// Run a field tile animation.
            /// 
            /// 1st argument: background animation.
            /// 2nd argument: starting frame.
            /// AT_TILEANIM Field Animation (1 bytes)
            /// AT_USPIN Frame (1 bytes)
            /// </summary>
            BGAANIME = 0x5F,

            /// <summary>
            /// ActivateTileAnimation
            /// Make a field tile animation active.
            /// 
            /// 1st argument: background animation.
            /// 2nd argument: boolean on/off.
            /// AT_TILEANIM Tile Animation (1 bytes)
            /// AT_BOOL Activate (1 bytes)
            /// </summary>
            BGAACTIVE = 0x60,

            /// <summary>
            /// SetTileAnimationSpeed
            /// Change the speed of a field tile animation.
            /// 
            /// 1st argument: background animation.
            /// 2nd argument: speed (256 = 1 tile/frame).
            /// AT_TILEANIM Tile Animation (1 bytes)
            /// AT_SPIN Speed (2 bytes)
            /// </summary>
            BGARATE = 0x61,

            /// <summary>
            /// SetRow
            /// Change the battle row of a party member.
            /// 
            /// 1st argument: party member.
            /// 2nd argument: boolean front/back.
            /// AT_LCHARACTER Character (1 bytes)
            /// AT_BOOL Row (1 bytes)
            /// </summary>
            SETROW = 0x62,

            /// <summary>
            /// SetTileAnimationPause
            /// Make a field tile animation pause at some frame in addition to its normal animation speed.
            /// 
            /// 1st argument: background animation.
            /// 2nd argument: animation frame.
            /// 3rd argument: wait time.
            /// AT_TILEANIM Tile Animation (1 bytes)
            /// AT_USPIN Frame ID (1 bytes)
            /// AT_USPIN Time (1 bytes)
            /// </summary>
            BGAWAIT = 0x63,

            /// <summary>
            /// SetTileAnimationFlags
            /// Add flags of a field tile animation.
            /// 
            /// 1st argument: background animation.
            /// 2nd argument: flags (only the flags 5 and 6 can be added).
            ///  5: unknown
            ///  6: loop back and forth
            /// AT_TILEANIM Tile Animation (1 bytes)
            /// AT_BOOLLIST Flags (1 bytes)
            /// </summary>
            BGAFLAG = 0x64,

            /// <summary>
            /// RunTileAnimationEx
            /// Run a field tile animation and choose its frame range.
            /// 
            /// 1st argument: background animation.
            /// 2nd argument: starting frame.
            /// 3rd argument: ending frame.
            /// AT_TILEANIM Tile Animation (1 bytes)
            /// AT_USPIN Start (1 bytes)
            /// AT_USPIN End (1 bytes)
            /// </summary>
            BGARANGE = 0x65,

            /// <summary>
            /// SetTextVariable
            /// Set the value of a text number or item variable.
            /// 
            /// 1st argument: text variable's "Script ID".
            /// 2nd argument: depends on which text opcode is related to the text variable.
            ///  For [VAR_NUM]: integral value.
            ///  For [VAR_ITEM]: item ID.
            ///  For [VAR_TOKEN]: token number.
            /// AT_USPIN Variable ID (1 bytes)
            /// AT_ITEM Value (2 bytes)
            /// </summary>
            MESVALUE = 0x66,

            /// <summary>
            /// SetControlDirection
            /// Set the angles for the player's movement control.
            /// 
            /// 1st argument: angle used for arrow movements.
            /// 2nd argument: angle used for analogic stick movements.
            /// AT_SPIN Arrow Angle (1 bytes)
            /// AT_SPIN Analogic Angle (1 bytes)
            /// </summary>
            TWIST = 0x67,

            /// <summary>
            /// Bubble
            /// Display a speech bubble with a symbol inside over the head of player's character.
            /// 
            /// 1st argument: bubble symbol.
            /// AT_BUBBLESYMBOL Symbo (1 bytes)
            /// </summary>
            FICON = 0x68,

            /// <summary>
            /// ChangeTimerTime
            /// Change the remaining time of the timer window.
            /// 
            /// 1st argument: time in seconds.
            /// AT_USPIN Time (2 bytes)
            /// </summary>
            TIMERSET = 0x69,

            /// <summary>
            /// DisableRun
            /// Make the player's character always walk.
            /// </summary>
            DASHOFF = 0x6A,

            /// <summary>
            /// SetBackgroundColor
            /// Change the default color, seen behind the field's tiles.
            /// 
            /// 1st to 3rd arguments: color in (Red, Green, Blue) format.
            /// AT_COLOR_RED ColorR (1 bytes)
            /// AT_COLOR_GREEN ColorG (1 bytes)
            /// AT_COLOR_BLUE ColorB (1 bytes)
            /// </summary>
            CLEARCOLOR = 0x6B,

            /// <summary>
            /// 0x6C
            /// Unknown Opcode.
            /// </summary>
            PPRINT = 0x6C,

            /// <summary>
            /// 0x6D
            /// Unknown Opcode.
            /// </summary>
            PPRINTF = 0x6D,

            /// <summary>
            /// 0x6E
            /// Unknown Opcode.
            /// </summary>
            MAPID = 0x6E,

            /// <summary>
            /// MoveCamera
            /// Move camera over time.
            /// 
            /// 1st and 2nd arguments: destination in (X, Y) format.
            /// 3nd argument: movement duration.
            /// 4th argument: scrolling type (8 for sinusoidal, other values for linear interpolation).
            /// AT_POSITION_X DestinationX (2 bytes)
            /// AT_POSITION_Y DestinationY (2 bytes)
            /// AT_USPIN Time (1 bytes)
            /// AT_USPIN Smoothness (1 bytes)
            /// </summary>
            BGSSCROLL = 0x6F,

            /// <summary>
            /// ReleaseCamera
            /// Release camera movement, getting back to its normal behaviour.
            /// 
            /// 1st arguments: duration of the repositioning.
            /// 2nd argument: scrolling type (8 for sinusoidal, other values for linear interpolation).
            /// AT_SPIN Time (1 bytes)
            /// AT_USPIN Smoothness (1 bytes)
            /// </summary>
            BGSRELEASE = 0x70,

            /// <summary>
            /// EnableCameraServices
            /// Enable or disable camera services. When disabling, the 2nd and 3rd arguments are ignored.
            /// 
            /// 1st arguments: boolean activate/deactivate.
            /// 2nd argument: duration of the repositioning when activating (defaulted to 30 if -1 is given).
            /// 3rd argument: scrolling type of the repositioning when activating (8 for sinusoidal, other values for linear interpolation).
            /// AT_BOOL Enable (1 bytes)
            /// AT_SPIN Time (1 bytes)
            /// AT_USPIN Smoothness (1 bytes)
            /// </summary>
            BGCACTIVE = 0x71,

            /// <summary>
            /// SetCameraFollowHeight
            /// Define the standard height gap between the player's character position and the camera view.
            /// 
            /// 1st argument: height.
            /// AT_SPIN Height (2 bytes)
            /// </summary>
            BGCHEIGHT = 0x72,

            /// <summary>
            /// EnableCameraFollow
            /// Make the camera follow the player's character.
            /// </summary>
            BGCLOCK = 0x73,

            /// <summary>
            /// DisableCameraFollow
            /// Stop making the camera follow the player's character.
            /// </summary>
            BGCUNLOCK = 0x74,

            /// <summary>
            /// Menu
            /// Open a menu.
            /// 
            /// 1st argument: menu type.
            /// 2nd argument: depends on the menu type.
            ///  Naming Menu: character to name.
            ///  Shop Menu: shop ID.
            /// AT_MENUTYPE Menu Type (1 bytes)
            /// AT_MENU Menu (1 bytes)
            /// </summary>
            MENU = 0x75,

            /// <summary>
            /// 0x76
            /// Unknown Opcode.
            /// AT_SPIN Unknown (2 bytes)
            /// AT_SPIN Unknown (2 bytes)
            /// </summary>
            TRACKSTART = 0x76,

            /// <summary>
            /// 0x77
            /// Unknown Opcode.
            /// AT_SPIN Unknown (2 bytes)
            /// AT_SPIN Unknown (2 bytes)
            /// </summary>
            TRACK = 0x77,

            /// <summary>
            /// 0x78
            /// Unknown Opcode.
            /// </summary>
            TRACKADD = 0x78,

            /// <summary>
            /// 0x79
            /// Unknown Opcode.
            /// </summary>
            PRINTQUAD = 0x79,

            /// <summary>
            /// SetLeftAnimation
            /// Change the left turning animation.
            /// 
            /// 1st argument: animation ID.
            /// AT_ANIMATION Animation (2 bytes)
            /// </summary>
            ATURNL = 0x7A,

            /// <summary>
            /// SetRightAnimation
            /// Change the right turning animation.
            /// 
            /// 1st argument: animation ID.
            /// AT_ANIMATION Animation (2 bytes)
            /// </summary>
            ATURNR = 0x7B,

            /// <summary>
            /// EnableDialogChoices
            /// Define choices availability in dialogs using the [INIT_MULTICHOICE] text opcode.
            /// 
            /// 1st argument: boolean list for the different choices.
            /// 2nd argument: default choice selected.
            /// AT_BOOLLIST Choice List (2 bytes)
            /// AT_USPIN Default Choice (1 bytes)
            /// </summary>
            CHOOSEPARAM = 0x7C,

            /// <summary>
            /// RunTimer
            /// Run or pause the timer window.
            /// 
            /// 1st argument: boolean run/pause.
            /// AT_BOOL Run (1 bytes)
            /// </summary>
            TIMERCONTROL = 0x7D,

            /// <summary>
            /// SetFieldCamera
            /// Change the field's background camera.
            /// 
            /// 1st argument: camera ID.
            /// AT_USPIN Camera ID (1 bytes)
            /// </summary>
            SETCAM = 0x7E,

            /// <summary>
            /// EnableShadow
            /// Enable the shadow for the entry's object.
            /// </summary>
            SHADOWON = 0x7F,

            /// <summary>
            /// DisableShadow
            /// Disable the shadow for the entry's object.
            /// </summary>
            SHADOWOFF = 0x80,

            /// <summary>
            /// SetShadowSize
            /// Set the entry's object shadow size.
            /// 
            /// 1st argument: size X.
            /// 2nd argument: size Y.
            /// AT_SPIN Size X (1 bytes)
            /// AT_SPIN Size Y (1 bytes)
            /// </summary>
            SHADOWSCALE = 0x81,

            /// <summary>
            /// SetShadowOffset
            /// Change the offset between the entry's object and its shadow.
            /// 
            /// 1st argument: offset X.
            /// 2nd argument: offset Y.
            /// AT_SPIN Offset X (2 bytes)
            /// AT_SPIN Offset Y (2 bytes)
            /// </summary>
            SHADOWOFFSET = 0x82,

            /// <summary>
            /// LockShadowRotation
            /// Stop updating the shadow rotation by the object's rotation.
            /// 
            /// 1st argument: locked rotation.
            /// AT_SPIN Locked Rotation (1 bytes)
            /// </summary>
            SHADOWLOCK = 0x83,

            /// <summary>
            /// UnlockShadowRotation
            /// Make the shadow rotate accordingly with its object.
            /// </summary>
            SHADOWUNLOCK = 0x84,

            /// <summary>
            /// SetShadowAmplifier
            /// Amplify or reduce the shadow transparancy.
            /// 
            /// 1st argument: amplification factor.
            /// AT_USPIN Amplification Factor (1 bytes)
            /// </summary>
            SHADOWAMP = 0x85,

            /// <summary>
            /// SetAnimationStandSpeed
            /// Change the standing animation speed.
            /// 
            /// 1st argument: unknown.
            /// 2nd argument: unknown.
            /// 3rd argument: unknown.
            /// 4th argument: unknown.
            /// AT_USPIN Unknown (1 bytes)
            /// AT_USPIN Unknown (1 bytes)
            /// AT_USPIN Unknown (1 bytes)
            /// AT_USPIN Unknown (1 bytes)
            /// </summary>
            IDLESPEED = 0x86,

            /// <summary>
            /// 0x87
            /// Unknown Opcode.
            /// AT_ENTRY Entry (1 bytes)
            /// AT_SPIN Unknown (1 bytes)
            /// </summary>
            DDIR = 0x87,

            /// <summary>
            /// RunModelCode
            /// Run a model code.
            /// 
            /// 1st argument: model code.
            /// 2nd to 4th arguments: depends on the model code.
            ///  Texture Blend (blend mode) with blend mode being 1 for screen, 2 for multiply, 4 for Soft and 255 for a solid texture
            ///  Slice (boolean slice/unslice, value)
            ///  Enable Mirror (boolean enable/disable)
            ///  Mirror Position (X, Z, Y)
            ///  Mirror Normal (X, Z, Y)
            ///  Mirror Color (Red, Green, Blue)
            ///  Sound codes (Animation, Frame, Value)
            ///   For Add (Secondary) Sound, the 3rd argument is the sound ID
            ///   For Sound Random Pitch, the 3rd argument is a boolean random/not random
            ///   For Remove Sound, the 3rd argument is unused
            /// AT_MODELCODE Model Code (1 bytes)
            /// AT_SPIN Argument 1 (2 bytes)
            /// AT_SPIN Argument 2 (2 bytes)
            /// AT_SPIN Argument 3 (2 bytes)
            /// </summary>
            CHRFX = 0x88,

            /// <summary>
            /// SetSoundPosition
            /// Set the position of a 3D sound.
            /// 
            /// 1st to 3rd arguments: sound position.
            /// 4th argument: sound volume.
            /// AT_POSITION_X PositionX (2 bytes)
            /// AT_POSITION_Z PositionZ (2 bytes)
            /// AT_POSITION_Y PositionY (2 bytes)
            /// AT_SPIN Volume (1 bytes)
            /// </summary>
            SEPV = 0x89,

            /// <summary>
            /// SetSoundObjectPosition
            /// Set the position of a 3D sound to the object's position.
            /// 
            /// 1st argument: object.
            /// 2nd argument: sound volume.
            /// AT_ENTRY Object (1 bytes)
            /// AT_SPIN Volume (1 bytes)
            /// </summary>
            SEPVA = 0x8A,

            /// <summary>
            /// SetHeadAngle
            /// Maybe define the maximum angle and distance for the character to turn his head toward an active object.
            /// 
            /// 1st argument: unknown.
            /// 2nd argument: unknown.
            /// AT_USPIN Unknown (1 bytes)
            /// AT_USPIN Unknown (1 bytes)
            /// </summary>
            NECKID = 0x8B,

            /// <summary>
            /// BattleEx
            /// Start a battle and choose its battle group.
            /// 
            /// 1st argument: rush type (unknown).
            /// 2nd argument: group.
            /// 3rd argument: gathered battle and Steiner's state (highest bit) informations.
            /// AT_SPIN Rush Type (1 bytes)
            /// AT_SPIN Battle Group (1 bytes)
            /// AT_BATTLE Battle (2 bytes)
            /// </summary>
            ENCOUNT2 = 0x8C,

            /// <summary>
            /// ShowTimer
            /// Activate the timer window.
            /// 
            /// 1st argument: boolean show/hide.
            /// AT_SPIN Enable (1 bytes)
            /// </summary>
            TIMERDISPLAY = 0x8D,

            /// <summary>
            /// RaiseWindows
            /// Make all the dialogs and windows display over the filters.
            /// </summary>
            RAISE = 0x8E,

            /// <summary>
            /// SetModelColor
            /// Change a 3D model's color.
            /// 
            /// 1st argument: entry associated with the model.
            /// 2nd to 4th arguments: color in (Red, Green, Blue) format.
            /// AT_ENTRY Object (1 bytes)
            /// AT_COLOR_RED ColorR (1 bytes)
            /// AT_COLOR_GREEN ColorG (1 bytes)
            /// AT_COLOR_BLUE ColorB (1 bytes)
            /// </summary>
            CHRCOLOR = 0x8F,

            /// <summary>
            /// DisableInactiveAnimation
            /// Prevent player's character to play its inactive animation.
            /// </summary>
            SLEEPINH = 0x90,

            /// <summary>
            /// 0x91
            /// Unknown Opcode.
            /// AT_BOOL Unknown (1 bytes)
            /// </summary>
            AUTOTURN = 0x91,

            /// <summary>
            /// AttachTile
            /// Make a part of the field background follow the player's movements. Also apply a color filter out of that tile block's range.
            /// 
            /// 1st argument: tile block.
            /// 2nd and 3rd arguments: offset position in (X, Y) format.
            /// 4th argument: filter mode ; use -1 for no filter effect.
            /// 5th to 7th arguments: filter color in (Red, Green, Blue) format.
            /// AT_TILE Tile Block (1 bytes)
            /// AT_SPIN Position X (2 bytes)
            /// AT_SPIN Position Y (1 bytes)
            /// AT_SPIN Filter Mode (1 bytes)
            /// AT_COLOR_RED Filter ColorR (1 bytes)
            /// AT_COLOR_GREEN Filter ColorG (1 bytes)
            /// AT_COLOR_BLUE Filter ColorB (1 bytes)
            /// </summary>
            BGLATTACH = 0x92,

            /// <summary>
            /// SetObjectFlags
            /// Change flags of the current entry's object.
            /// 
            /// 1st argument: object flags.
            ///  1: show model
            ///  2: unknown
            ///  4: unknown
            ///  8: unknown
            ///  16: unknown
            ///  32: unknown
            /// AT_BOOLLIST Flags (1 bytes)
            /// </summary>
            CFLAG = 0x93,

            /// <summary>
            /// SetJumpAnimation
            /// Change the jumping animation.
            /// 
            /// 1st argument: animation ID.
            /// 2nd argument: unknown.
            /// 3rd argument: unknown.
            /// AT_ANIMATION Animation (2 bytes)
            /// AT_SPIN Unknown (1 bytes)
            /// AT_SPIN Unknown (1 bytes)
            /// </summary>
            AJUMP = 0x94,

            /// <summary>
            /// WindowSyncEx
            /// Display a window with text inside and wait until it closes.
            /// 
            /// 1st argument: talker's entry.
            /// 2nd argument: window ID.
            /// 3rd argument: UI flag list.
            ///  3: disable bubble tail
            ///  4: mognet format
            ///  5: hide window
            ///  7: ATE window
            ///  8: dialog window
            /// 4th argument: text to display.
            /// AT_ENTRY Talker (1 bytes)
            /// AT_USPIN Window ID (1 bytes)
            /// AT_BOOLLIST UI (1 bytes)
            /// AT_TEXT Text (2 bytes)
            /// </summary>
            MESA = 0x95,

            /// <summary>
            /// WindowAsyncEx
            /// Display a window with text inside and continue the execution of the script without waiting.
            /// 
            /// 1st argument: talker's entry.
            /// 2nd argument: window ID.
            /// 3rd argument: UI flag list.
            ///  3: disable bubble tail
            ///  4: mognet format
            ///  5: hide window
            ///  7: ATE window
            ///  8: dialog window
            /// 4th argument: text to display.
            /// AT_ENTRY Talker (1 bytes)
            /// AT_USPIN Window ID (1 bytes)
            /// AT_BOOLLIST UI (1 bytes)
            /// AT_TEXT Text (2 bytes)
            /// </summary>
            MESAN = 0x96,

            /// <summary>
            /// ReturnEntryFunctions
            /// Make all the currently executed functions return for a given entry.
            /// 
            /// 1st argument: entry for which functions are returned.
            /// AT_ENTRY Entry (1 bytes)
            /// </summary>
            DRET = 0x97,

            /// <summary>
            /// MakeAnimationLoop
            /// Make current object's currently playing animation loop.
            /// 
            /// 1st argument: loop amount.
            /// AT_USPIN Amount (1 bytes)
            /// </summary>
            MOVT = 0x98,

            /// <summary>
            /// SetTurnSpeed
            /// Change the entry's object turn speed.
            /// 
            /// 1st argument: turn speed (1 is slowest).
            /// AT_USPIN Speed (1 bytes)
            /// </summary>
            TSPEED = 0x99,

            /// <summary>
            /// EnablePathTriangle
            /// Enable or disable a triangle of field pathing.
            /// 
            /// 1st argument: triangle ID.
            /// 2nd argument: boolean enable/disable.
            /// AT_WALKTRIANGLE Triangle ID (2 bytes)
            /// AT_BOOL Enable (1 bytes)
            /// </summary>
            BGIACTIVET = 0x9A,

            /// <summary>
            /// TurnTowardPosition
            /// Turn the character toward a position (animated). The object's turn speed is used (default to 16).
            /// 
            /// 1st and 2nd arguments: coordinates in (X, Y) format.
            /// AT_POSITION_X CoordinateX (2 bytes)
            /// AT_POSITION_Y CoordinateY (2 bytes)
            /// </summary>
            TURNTO = 0x9B,

            /// <summary>
            /// RunJumpAnimation
            /// Make the character play its jumping animation.
            /// </summary>
            PREJUMP = 0x9C,

            /// <summary>
            /// RunLandAnimation
            /// Make the character play its landing animation (inverted jumping animation).
            /// </summary>
            POSTJUMP = 0x9D,

            /// <summary>
            /// ExitField
            /// Make the player's character walk to the field exit and prepare to flush the field datas.
            /// </summary>
            MOVQ = 0x9E,

            /// <summary>
            /// SetObjectSize
            /// Set the size of a 3D model.
            /// 
            /// 1st argument: entry of the 3D model.
            /// 2nd to 4th arguments: size ratio in (Ratio X, Ratio Z, Ratio Y) format. A ratio of 64 is the default size.
            /// AT_ENTRY Object (1 bytes)
            /// AT_SPIN Size X (1 bytes)
            /// AT_SPIN Size Z (1 bytes)
            /// AT_SPIN Size Y (1 bytes)
            /// </summary>
            CHRSCALE = 0x9F,

            /// <summary>
            /// WalkToExit
            /// Make the entry's object walk to the field exit.
            /// </summary>
            MOVJ = 0xA0,

            /// <summary>
            /// MoveInstantXZY
            /// Instantatly move the object.
            /// 
            /// 1st to 3rd arguments: destination in (X, Z, Y) format.
            /// AT_POSITION_X DestinationX (2 bytes)
            /// AT_POSITION_Z DestinationZ (2 bytes)
            /// AT_POSITION_Y DestinationY (2 bytes)
            /// </summary>
            POS3 = 0xA1,

            /// <summary>
            /// WalkXZY
            /// Make the character walk to destination. Make it synchronous if InitWalk is called before.
            /// 
            /// 1st argument: destination in (X, Z, Y) format.
            /// AT_POSITION_X DestinationX (2 bytes)
            /// AT_POSITION_Z DestinationZ (2 bytes)
            /// AT_POSITION_Y DestinationY (2 bytes)
            /// </summary>
            MOVE3 = 0xA2,

            /// <summary>
            /// 0xA3
            /// Unknown Opcode.
            /// AT_SPIN Unknown (1 bytes)
            /// AT_SPIN Unknown (1 bytes)
            /// AT_SPIN Unknown (1 bytes)
            /// AT_SPIN Unknown (1 bytes)
            /// </summary>
            DRADIUS = 0xA3,

            /// <summary>
            /// CalculateExitPosition
            /// Calculate the field exit position based on the region's polygon.
            /// </summary>
            MJPOS = 0xA4,

            /// <summary>
            /// Slide
            /// Make the character slide to destination (walk without using the walk animation and without changing the facing angle).
            /// 
            /// 1st and 2nd arguments: destination in (X, Y) format.
            /// AT_POSITION_X DestinationX (2 bytes)
            /// AT_POSITION_Y DestinationY (2 bytes)
            /// </summary>
            MOVH = 0xA5,

            /// <summary>
            /// SetRunSpeedLimit
            /// Change the speed at which the character uses his run animation instead of his walk animation (default is 31).
            /// 
            /// 1st argument: speed limit.
            /// AT_USPIN Speed Limit (1 bytes)
            /// </summary>
            SPEEDTH = 0xA6,

            /// <summary>
            /// Turn
            /// Make the character face an angle (animated). Speed is defaulted to 16.
            /// 
            /// 1st argument: angle.
            /// 0 faces south, 64 faces west, 128 faces north and 192 faces east.
            /// AT_USPIN Angle (1 bytes)
            /// </summary>
            TURNDS = 0xA7,

            /// <summary>
            /// SetPathing
            /// Change the pathing of the character.
            /// 
            /// 1st argument: boolean pathing on/off.
            /// AT_BOOL Pathing (1 bytes)
            /// </summary>
            BGI = 0xA8,

            /// <summary>
            /// 0xA9
            /// Unknown Opcode.
            /// AT_ENTRY Unknown (1 bytes)
            /// </summary>
            GETSCREEN = 0xA9,

            /// <summary>
            /// EnableMenu
            /// Enable menu access by the player.
            /// </summary>
            MENUON = 0xAA,

            /// <summary>
            /// DisableMenu
            /// Disable menu access by the player.
            /// </summary>
            MENUOFF = 0xAB,

            /// <summary>
            /// ChangeDisc
            /// Allow to save the game and change disc.
            /// 
            /// 1st argument: gathered field destination and disc destination.
            /// AT_DISC_FIELD Disc (2 bytes)
            /// </summary>
            DISCCHANGE = 0xAC,

            /// <summary>
            /// MoveInstantXZYEx
            /// Instantatly move an object.
            /// 
            /// 1st argument: object's entry.
            /// 2nd to 4th arguments: destination in (X, Z, Y) format.
            /// AT_ENTRY Object (1 bytes)
            /// AT_POSITION_X DestinationX (2 bytes)
            /// AT_POSITION_Z DestinationY (2 bytes)
            /// AT_POSITION_Y DestinationZ (2 bytes)
            /// </summary>
            DPOS3 = 0xAD,

            /// <summary>
            /// TetraMaster
            /// Begin a card game.
            /// 
            /// 1st argument: card deck of the opponent.
            /// AT_DECK Card Deck (2 bytes)
            /// </summary>
            MINIGAME = 0xAE,

            /// <summary>
            /// 0xAF
            /// Unknown Opcode.
            /// </summary>
            DELETEALLCARD = 0xAF,

            /// <summary>
            /// SetFieldName
            /// Change the name of the field.
            /// 
            /// 1st argument: new name (unknown format).
            /// AT_SPIN Name (2 bytes)
            /// </summary>
            SETMAPNAME = 0xB0,

            /// <summary>
            /// ResetFieldName
            /// Reset the name of the field.
            /// </summary>
            RESETMAPNAME = 0xB1,

            /// <summary>
            /// Party
            /// Allow the player to change the members of its party.
            /// 
            /// 1st argument: party size (if characters occupy slots beyond it, they are locked).
            /// 2nd argument: list of locked characters.
            /// AT_USPIN Party Size (1 bytes)
            /// AT_CHARACTER Locked Characters (2 bytes)
            /// </summary>
            PARTYMENU = 0xB2,

            /// <summary>
            /// RunSPSCode
            /// Run Sps code, which seems to be special model effects on the field.
            /// 
            /// 1st argument: sps ID.
            /// 2nd argument: sps code.
            /// 3rd to 5th arguments: depend on the sps code.
            ///  Load Sps (sps type)
            ///  Enable Attribute (attribute list, boolean enable/disable)
            ///  Set Position (X, -Z, Y)
            ///  Set Rotation (angle X, angle Z, angle Y)
            ///  Set Scale (scale factor)
            ///  Attach (object's entry to attach, bone number)
            ///  Set Fade (fade)
            ///  Set Animation Rate (rate)
            ///  Set Frame Rate (rate)
            ///  Set Frame (value) where the value is factored by 16 to get the frame
            ///  Set Position Offset (X, -Z, Y)
            ///  Set Depth Offset (depth)
            /// AT_USPIN Sps (1 bytes)
            /// AT_SPSCODE Code (1 bytes)
            /// AT_SPIN Parameter 1 (2 bytes)
            /// AT_SPIN Parameter 2 (2 bytes)
            /// AT_SPIN Parameter 3 (2 bytes)
            /// </summary>
            SPS = 0xB3,

            /// <summary>
            /// SetPartyReserve
            /// Define the party member availability for a future Party call.
            /// 
            /// 1st argument: list of available characters.
            /// AT_CHARACTER Characters available (2 bytes)
            /// </summary>
            FULLMEMBER = 0xB4,

            /// <summary>
            /// 0xB5
            /// Seem to somehow make the object appropriate itself another entry's function list.
            /// 
            /// 1st argument: entry to get functions from.
            /// AT_ENTRY Entry (1 bytes)
            /// </summary>
            PRETEND = 0xB5,

            /// <summary>
            /// WorldMap
            /// Change the scene to a world map.
            /// 
            /// 1st argument: world map destination.
            /// AT_WORLDMAP World Map (2 bytes)
            /// </summary>
            WMAPJUMP = 0xB6,

            /// <summary>
            /// 0xB7
            /// Unknown Opcode.
            /// </summary>
            EYE = 0xB7,

            /// <summary>
            /// 0xB8
            /// Unknown Opcode.
            /// </summary>
            AIM = 0xB8,

            /// <summary>
            /// AddControllerMask
            /// Prevent the input to be processed by the game.
            /// 
            /// 1st argument: pad number (should only be 0 or 1).
            /// 2nd argument: button list.
            /// 1: Select
            /// 4: Start
            /// 5: Up
            /// 6: Right
            /// 7: Down
            /// 8: Left
            /// 9: L2
            /// 10: R2
            /// 11: L1
            /// 12: R1
            /// 13: Triangle
            /// 14: Circle
            /// 15: Cross
            /// 16: Square
            /// 17: Cancel
            /// 18: Confirm
            /// 20: Moogle
            /// 21: L1 Ex
            /// 22: R1 Ex
            /// 23: L2 Ex
            /// 24: R2 Ex
            /// 25: Menu
            /// 26: Select Ex
            /// AT_USPIN Pad (1 bytes)
            /// AT_BUTTONLIST Buttons (2 bytes)
            /// </summary>
            SETKEYMASK = 0xB9,

            /// <summary>
            /// RemoveControllerMask
            /// Enable back the controller's inputs.
            /// 
            /// 1st argument: pad number (should only be 0 or 1).
            /// 2nd argument: button list.
            /// 1: Select
            /// 4: Start
            /// 5: Up
            /// 6: Right
            /// 7: Down
            /// 8: Left
            /// 9: L2
            /// 10: R2
            /// 11: L1
            /// 12: R1
            /// 13: Triangle
            /// 14: Circle
            /// 15: Cross
            /// 16: Square
            /// 17: Cancel
            /// 18: Confirm
            /// 20: Moogle
            /// 21: L1 Ex
            /// 22: R1 Ex
            /// 23: L2 Ex
            /// 24: R2 Ex
            /// 25: Menu
            /// 26: Select Ex
            /// AT_USPIN Pad (1 bytes)
            /// AT_BUTTONLIST Buttons (2 bytes)
            /// </summary>
            CLEARKEYMASK = 0xBA,

            /// <summary>
            /// TimedTurnEx
            /// Make an object face an angle (animated).
            /// 
            /// 1st argument: object's entry.
            /// 2nd argument: angle.
            /// 0 faces south, 64 faces west, 128 faces north and 192 faces east.
            /// 3rd argument: turn speed (1 is slowest).
            /// AT_ENTRY Object (1 bytes)
            /// AT_USPIN Angle (1 bytes)
            /// AT_USPIN Speed (1 bytes)
            /// </summary>
            DTURN = 0xBB,

            /// <summary>
            /// WaitTurnEx
            /// Wait until an object facing movement has ended.
            /// 
            /// 1st argument: object's entry.
            /// AT_ENTRY Object (1 bytes)
            /// </summary>
            DWAITTURN = 0xBC,

            /// <summary>
            /// RunAnimationEx
            /// Play an object's animation.
            /// 
            /// 1st argument: object's entry.
            /// 2nd argument: animation ID.
            /// AT_ENTRY Object (1 bytes)
            /// AT_ANIMATION Animation ID (2 bytes)
            /// </summary>
            DANIM = 0xBD,

            /// <summary>
            /// WaitAnimationEx
            /// Wait until the object's animation has ended.
            /// 
            /// 1st argument: object's entry.
            /// AT_ENTRY Object (1 bytes)
            /// </summary>
            DWAITANIM = 0xBE,

            /// <summary>
            /// MoveInstantEx
            /// Instantatly move an object.
            /// 
            /// 1st argument: object's entry.
            /// 2nd and 3rd arguments: destination in (X, Y) format.
            /// AT_ENTRY Object (1 bytes)
            /// AT_POSITION_X DestinationX (2 bytes)
            /// AT_POSITION_Y DestinationY (2 bytes)
            /// </summary>
            DPOS = 0xBF,

            /// <summary>
            /// EnableTextureAnimation
            /// Run a model texture animation and make it loop.
            /// 
            /// 1st argument: model's entry.
            /// 2nd argument: texture animation ID.
            /// AT_ENTRY Object (1 bytes)
            /// AT_USPIN Texture Animation (1 bytes)
            /// </summary>
            TEXPLAY = 0xC0,

            /// <summary>
            /// RunTextureAnimation
            /// Run once a model texture animation.
            /// 
            /// 1st argument: model's entry.
            /// 2nd argument: texture animation ID.
            /// AT_ENTRY Object (1 bytes)
            /// AT_USPIN Texture Animation (1 bytes)
            /// </summary>
            TEXPLAY1 = 0xC1,

            /// <summary>
            /// StopTextureAnimation
            /// Stop playing the model texture animation.
            /// 
            /// 1st argument: model's entry.
            /// 2nd argument: texture animation ID.
            /// AT_ENTRY Object (1 bytes)
            /// AT_USPIN Texture Animation (1 bytes)
            /// </summary>
            TEXSTOP = 0xC2,

            /// <summary>
            /// SetTileCamera
            /// Link a tile block to a specific field camera (useful for looping movement bounds).
            /// 
            /// 1st argument: background tile block.
            /// 2nd argument: camera ID.
            /// AT_TILE Tile Block (1 bytes)
            /// AT_USPIN Camera ID (1 bytes)
            /// </summary>
            BGVSET = 0xC3,

            /// <summary>
            /// RunWorldCode
            /// Run one of the World Map codes, which effects have a large range. May modify the weather, the music, call the chocobo or enable the auto-pilot.
            /// 
            /// 1st argument: world code.
            /// 2nd argument: depends on the code.
            /// AT_WORLDCODE Code (1 bytes)
            /// AT_SPIN Argument (2 bytes)
            /// </summary>
            WPRM = 0xC4,

            /// <summary>
            /// RunSoundCode
            /// Same as RunSoundCode3( code, music, 0, 0, 0 ).
            /// AT_SOUNDCODE Code (2 bytes)
            /// AT_SOUND Sound (2 bytes)
            /// </summary>
            FLDSND0 = 0xC5,

            /// <summary>
            /// RunSoundCode1
            /// Same as RunSoundCode3( code, music, arg1, 0, 0 ).
            /// AT_SOUNDCODE Code (2 bytes)
            /// AT_SOUND Sound (2 bytes)
            /// AT_SPIN Argument (3 bytes)
            /// </summary>
            FLDSND1 = 0xC6,

            /// <summary>
            /// RunSoundCode2
            /// Same as RunSoundCode3( code, music, arg1, arg2, 0 ).
            /// AT_SOUNDCODE Code (2 bytes)
            /// AT_SOUND Sound (2 bytes)
            /// AT_SPIN Argument (3 bytes)
            /// AT_SPIN Argument (1 bytes)
            /// </summary>
            FLDSND2 = 0xC7,

            /// <summary>
            /// RunSoundCode3
            /// Run a sound code.
            /// 
            /// 1st argument: sound code.
            /// 2nd argument: music or sound to process.
            /// 3rd to 5th arguments: depend on the sound code.
            /// AT_SOUNDCODE Code (2 bytes)
            /// AT_SOUND Sound (2 bytes)
            /// AT_SPIN Argument (3 bytes)
            /// AT_SPIN Argument (1 bytes)
            /// AT_SPIN Argument (1 bytes)
            /// </summary>
            FLDSND3 = 0xC8,

            /// <summary>
            /// 0xC9
            /// Unknown Opcode.
            /// AT_USPIN Unknown (1 bytes)
            /// AT_USPIN Unknown (2 bytes)
            /// AT_USPIN Unknown (2 bytes)
            /// AT_USPIN Unknown (2 bytes)
            /// AT_USPIN Unknown (2 bytes)
            /// </summary>
            BGVDEFINE = 0xC9,

            /// <summary>
            /// 0xCA
            /// Unknown Opcode.
            /// AT_SPIN Unknown (1 bytes)
            /// AT_BOOL Visible (1 bytes)
            /// </summary>
            BGAVISIBLE = 0xCA,

            /// <summary>
            /// EnablePath
            /// Enable a field path.
            /// 
            /// 1st argument: field path ID.
            /// 2nd argument: boolean enable/disable.
            /// AT_WALKPATH Path (1 bytes)
            /// AT_BOOL Enable (1 bytes)
            /// </summary>
            BGIACTIVEF = 0xCB,

            /// <summary>
            /// 0xCC
            /// Maybe make the character transparent.
            /// AT_BOOLLIST Flag List (2 bytes)
            /// </summary>
            CHRSET = 0xCC,

            /// <summary>
            /// 0xCD
            /// Unknown Opcode.
            /// AT_BOOLLIST Flag List (2 bytes)
            /// </summary>
            CHRCLEAR = 0xCD,

            /// <summary>
            /// AddGi
            /// Give gil to the player.
            /// 
            /// 1st argument: gil amount.
            /// AT_SPIN Amount (3 bytes)
            /// </summary>
            GILADD = 0xCE,

            /// <summary>
            /// RemoveGi
            /// Remove gil from the player.
            /// 
            /// 1st argument: gil amount.
            /// AT_SPIN Amount (3 bytes)
            /// </summary>
            GILDELETE = 0xCF,

            /// <summary>
            /// BattleDialog
            /// Display text in battle for 60 frames.
            /// 
            /// 1st argument: text to display.
            /// AT_TEXT Text (2 bytes)
            /// </summary>
            MESB = 0xD0,

            /// <summary>
            /// 0xD1
            /// Unknown Opcode; ignored in the non-PSX versions.
            /// </summary>
            GLOBALCLEAR = 0xD1,

            /// <summary>
            /// 0xD2
            /// Unknown Opcode; ignored in the non-PSX versions.
            /// </summary>
            DEBUGSAVE = 0xD2,

            /// <summary>
            /// 0xD3
            /// Unknown Opcode; ignored in the non-PSX versions.
            /// </summary>
            DEBUGLOAD = 0xD3,

            /// <summary>
            /// 0xD4
            /// Unknown Opcode.
            /// AT_SPIN Unknown (2 bytes)
            /// AT_SPIN Unknown (2 bytes)
            /// AT_SPIN Unknown (2 bytes)
            /// </summary>
            ATTACHOFFSET = 0xD4,

            /// <summary>
            /// 0xD5
            /// Unknown Opcode.
            /// </summary>
            PUSHHIDE = 0xD5,

            /// <summary>
            /// 0xD6
            /// Unknown Opcode.
            /// </summary>
            POPSHOW = 0xD6,

            /// <summary>
            /// ATE
            /// Enable or disable ATE.
            /// 
            /// 1st argument: maybe flags (unknown format).
            /// AT_SPIN Unknown (1 bytes)
            /// </summary>
            AICON = 0xD7,

            /// <summary>
            /// SetWeather
            /// Add a foreground effect.
            /// 
            /// 1st argument: unknown.
            /// 2nd argument: unknown.
            /// AT_SPIN Unknown (1 bytes)
            /// AT_SPIN Unknown (1 bytes)
            /// </summary>
            RAIN = 0xD8,

            /// <summary>
            /// CureStatus
            /// Cure the status ailments of a party member.
            /// 
            /// 1st argument: character.
            /// 2nd argument: status list.
            ///  1: Petrified
            ///  2: Venom
            ///  3: Virus
            ///  4: Silence
            ///  5: Darkness
            ///  6: Trouble
            ///  7: Zombie
            /// AT_LCHARACTER Character (1 bytes)
            /// AT_BOOLLIST Statuses (1 bytes)
            /// </summary>
            CLEARSTATUS = 0xD9,

            /// <summary>
            /// RunSPSCodeSimple
            /// Run Sps code, which seems to be special model effects on the field.
            /// 
            /// 1st argument: sps ID.
            /// 2nd argument: sps code.
            /// 3rd to 5th arguments: depend on the sps code.
            ///  Load Sps (sps type)
            ///  Enable Attribute (attribute list, boolean enable/disable)
            ///  Set Position (X, -Z, Y)
            ///  Set Rotation (angle X, angle Z, angle Y)
            ///  Set Scale (scale factor)
            ///  Attach (object's entry to attach, bone number)
            ///  Set Fade (fade)
            ///  Set Animation Rate (rate)
            ///  Set Frame Rate (rate)
            ///  Set Frame (value) where the value is factored by 16 to get the frame
            ///  Set Position Offset (X, -Z, Y)
            ///  Set Depth Offset (depth)
            /// AT_USPIN Sps (1 bytes)
            /// AT_SPSCODE Code (1 bytes)
            /// AT_SPIN Parameter 1 (1 bytes)
            /// AT_SPIN Parameter 2 (2 bytes)
            /// AT_SPIN Parameter 3 (2 bytes)
            /// </summary>
            SPS2 = 0xDA,

            /// <summary>
            /// EnableVictoryPose
            /// Enable or disable the victory pose at the end of battles for a specific character.
            /// 
            /// 1st argument: which character.
            /// 2nd argument: boolean activate/deactivate.
            /// AT_LCHARACTER Character (1 bytes)
            /// AT_BOOL Enable (1 bytes)
            /// </summary>
            WINPOSE = 0xDB,

            /// <summary>
            /// Jump
            /// Perform a jumping animation. Must be used after a SetupJump call.
            /// </summary>
            JUMP3 = 0xDC,

            /// <summary>
            /// RemoveParty
            /// Remove a character from the player's team.
            /// 
            /// 1st argument: character to remove.
            /// AT_LCHARACTER Character (1 bytes)
            /// </summary>
            PARTYDELETE = 0xDD,

            /// <summary>
            /// SetName
            /// Change the name of a party member. Clear the text opcodes from the chosen text.
            /// 
            /// 1st argument: character to rename.
            /// 2nd argument: new name.
            /// AT_LCHARACTER Character (1 bytes)
            /// AT_TEXT Text (2 bytes)
            /// </summary>
            PLAYERNAME = 0xDE,

            /// <summary>
            /// 0xDF
            /// Unknown Opcode.
            /// AT_USPIN Unknown (1 bytes)
            /// </summary>
            OVAL = 0xDF,

            /// <summary>
            /// AddFrog
            /// Add one frog to the frog counter.
            /// </summary>
            INCFROG = 0xE0,

            /// <summary>
            /// TerminateBattle
            /// Return to the field (or world map) when the rewards are disabled.
            /// </summary>
            BEND = 0xE1,

            /// <summary>
            /// SetupJump
            /// Setup datas for a Jump call.
            /// 
            /// 1st to 3rd arguments: destination in (X, Z, Y) format.
            /// 4th argument: number of steps for the jump.
            /// AT_POSITION_X DestinationX (2 bytes)
            /// AT_POSITION_Z DestinationZ (2 bytes)
            /// AT_POSITION_Y DestinationY (2 bytes)
            /// AT_USPIN Steps (1 bytes)
            /// </summary>
            SETVY3 = 0xE2,

            /// <summary>
            /// SetDialogProgression
            /// Change the dialog progression value.
            /// 
            /// 1st argument: new value.
            /// AT_SPIN Progression (1 bytes)
            /// </summary>
            SETSIGNAL = 0xE3,

            /// <summary>
            /// 0xE4
            /// Seem to move field tile while applying a loop effect to it.
            /// 
            /// 1st argument: background tile block.
            /// 2nd argument: boolean activate/deactivate.
            /// 3rd and 4th arguments: seems to be the movement in (X/256, Y/256) format.
            /// 5th argument: unknown boolean.
            /// AT_TILE Tile Block (1 bytes)
            /// AT_BOOL Enable (1 bytes)
            /// AT_SPIN Delta (2 bytes)
            /// AT_SPIN Offset (2 bytes)
            /// AT_BOOL Is X Offset (1 bytes)
            /// </summary>
            BGLSCROLLOFFSET = 0xE4,

            /// <summary>
            /// AttackSpecia
            /// Make the enemy instantatly use a special move. It doesn't use nor modify the battle state so it should be used when the battle is paused. The target(s) are to be set using the SV_Target variable.
            /// 
            /// 1st argument: attack to perform.
            /// AT_ATTACK Attack (1 bytes)
            /// </summary>
            BTLSEQ = 0xE5,

            /// <summary>
            /// SetTileLoopType
            /// Let tile be screen anchored or not.
            /// 
            /// 1st argument: background tile block.
            /// 2nd argument: boolean on/off.
            /// AT_TILE Tile Block (1 bytes)
            /// AT_BOOL Screen Anchored (1 bytes)
            /// </summary>
            BGLLOOPTYPE = 0xE6,

            /// <summary>
            /// SetTileAnimationFrame
            /// Change the frame of a field tile animation (can be used to hide them all if the given frame is out of range, eg. 255).
            /// 
            /// 1st argument: background animation.
            /// 2nd argument: animation frame to display.
            /// AT_TILEANIM Animation (1 bytes)
            /// AT_USPIN Frame ID (1 bytes)
            /// </summary>
            BGAFRAME = 0xE7,

            /// <summary>
            /// SideWalkXZY
            /// Make the character walk to destination without changing his facing angle. Make it synchronous if InitWalk is called before.
            /// 
            /// 1st to 3rd arguments: destination in (X, Z, Y) format.
            /// AT_POSITION_X DestinationX (2 bytes)
            /// AT_POSITION_Z DestinationY (2 bytes)
            /// AT_POSITION_Y DestinationZ (2 bytes)
            /// </summary>
            MOVE3H = 0xE8,

            /// <summary>
            /// UpdatePartyUI
            /// Update the party's menu icons and such.
            /// </summary>
            SYNCPARTY = 0xE9,

            /// <summary>
            /// 0xEA
            /// Unknown Opcode.
            /// </summary>
            VRP = 0xEA,

            /// <summary>
            /// CloseAllWindows
            /// Close all the dialogs and UI windows.
            /// </summary>
            CLOSEALL = 0xEB,

            /// <summary>
            /// FadeFilter
            /// Apply a fade filter on the screen.
            /// 
            /// 1st argument: filter mode (0 for ADD, 2 for SUBTRACT).
            /// 2nd argument: fading time.
            /// 3rd argument: unknown.
            /// 4th to 6th arguments: color of the filter in (Cyan, Magenta, Yellow) format.
            /// AT_USPIN Fade In/Out (1 bytes)
            /// AT_USPIN Fading Time (1 bytes)
            /// AT_SPIN Unknown (1 bytes)
            /// AT_COLOR_CYAN ColorC (1 bytes)
            /// AT_COLOR_MAGENTA ColorM (1 bytes)
            /// AT_COLOR_YELLOW ColorY (1 bytes)
            /// </summary>
            WIPERGB = 0xEC,

            /// <summary>
            /// 0xED
            /// Unknown opcode about tile looping movements.
            /// 
            /// 1st argument: camera ID.
            /// 2nd and 3rd arguments: unknown factors (X, Y).
            /// AT_USPIN Camera ID (1 bytes)
            /// AT_SPIN Unknown X (2 bytes)
            /// AT_SPIN Unknown Y (2 bytes)
            /// </summary>
            BGVALPHA = 0xED,

            /// <summary>
            /// EnableInactiveAnimation
            /// Allow the player's character to play its inactive animation. The inaction time required is:
            /// First Time = 200 + 4 * Random[0, 255]
            /// Following Times = 200 + 2 * Random[0, 255]
            /// </summary>
            SLEEPON = 0xEE,

            /// <summary>
            /// ShowHereIcon
            /// Show the Here icon over player's chatacter.
            /// 
            /// 1st argument: display type (0 to hide, 3 to show unconditionally)
            /// AT_SPIN Show (1 bytes)
            /// </summary>
            HEREON = 0xEF,

            /// <summary>
            /// EnableRun
            /// Allow the player's character to run.
            /// </summary>
            DASHON = 0xF0,

            /// <summary>
            /// SetHP
            /// Change the HP of a party's member.
            /// 
            /// 1st argument: character.
            /// 2nd argument: new HP value.
            /// AT_LCHARACTER Character (1 bytes)
            /// AT_USPIN HP (2 bytes)
            /// </summary>
            SETHP = 0xF1,

            /// <summary>
            /// SetMP
            /// Change the MP of a party's member.
            /// 
            /// 1st argument: character.
            /// 2nd argument: new MP value.
            /// AT_LCHARACTER Character (1 bytes)
            /// AT_USPIN MP (2 bytes)
            /// </summary>
            SETMP = 0xF2,

            /// <summary>
            /// UnlearnAbility
            /// Set an ability's AP back to 0.
            /// 
            /// 1st argument: character.
            /// 2nd argument: ability to reset.
            /// AT_LCHARACTER Character (1 bytes)
            /// AT_ABILITY Ability (1 bytes)
            /// </summary>
            CLEARAP = 0xF3,

            /// <summary>
            /// LearnAbility
            /// Make character learn an ability.
            /// 
            /// 1st argument: character.
            /// 2nd argument: ability to learn.
            /// AT_LCHARACTER Character (1 bytes)
            /// AT_ABILITY Ability (1 bytes)
            /// </summary>
            MAXAP = 0xF4,

            /// <summary>
            /// GameOver
            /// Terminate the game with a Game Over screen.
            /// </summary>
            GAMEOVER = 0xF5,

            /// <summary>
            /// VibrateController
            /// Start the vibration lifespan.
            /// 
            /// 1st argument: frame to begin with.
            /// AT_USPIN Frame (1 bytes)
            /// </summary>
            VIBSTART = 0xF6,

            /// <summary>
            /// ActivateVibration
            /// Make the controller's vibration active. If the current controller's frame is out of the vibration time range, the vibration lifespan is reinit.
            /// 
            /// 1st argument: boolean activate/deactivate.
            /// AT_BOOL Activate (1 bytes)
            /// </summary>
            VIBACTIVE = 0xF7,

            /// <summary>
            /// RunVibrationTrack
            /// Run a vibration track.
            /// 
            /// 1st argument: track ID.
            /// 2nd argument: sample (0 or 1).
            /// 3rd argument: boolean activate/deactivate.
            /// AT_USPIN Track (1 bytes)
            /// AT_USPIN Sample (1 bytes)
            /// AT_BOOL Activate (1 bytes)
            /// </summary>
            VIBTRACK1 = 0xF8,

            /// <summary>
            /// ActivateVibrationTrack
            /// Activate a vibration track.
            /// 
            /// 1st argument: track ID.
            /// 2nd argument: sample (0 or 1).
            /// 3rd argument: boolean activate/deactivate.
            /// AT_USPIN Track (1 bytes)
            /// AT_USPIN Sample (1 bytes)
            /// AT_BOOL Activate (1 bytes)
            /// </summary>
            VIBTRACK = 0xF9,

            /// <summary>
            /// SetVibrationSpeed
            /// Set the vibration frame rate.
            /// 
            /// 1st argument: frame rate.
            /// AT_USPIN Frame Rate (2 bytes)
            /// </summary>
            VIBRATE = 0xFA,

            /// <summary>
            /// SetVibrationFlags
            /// Change the vibration flags.
            /// 
            /// 1st argument: flags.
            ///  8: Loop
            ///  16: Wrap
            /// AT_BOOLLIST Flags (1 bytes)
            /// </summary>
            VIBFLAG = 0xFB,

            /// <summary>
            /// SetVibrationRange
            /// Set the time range of vibration.
            /// 
            /// 1st and 2nd arguments: vibration range in (Start, End) format.
            /// AT_USPIN Start (1 bytes)
            /// AT_USPIN End (1 bytes)
            /// </summary>
            VIBRANGE = 0xFC,

            /// <summary>
            /// PreloadField
            /// Surely preload a field; ignored in the non-PSX versions.
            /// 
            /// 1st argument: unknown.
            /// 2nd argument: field to preload.
            /// AT_SPIN Unknown (1 bytes)
            /// AT_FIELD Field (2 bytes)
            /// </summary>
            HINT = 0xFD,

            /// <summary>
            /// SetCharacterData
            /// Init a party's member battle and menu datas.
            /// 
            /// 1st argument: character.
            /// 2nd argument: boolean update level/don't update level.
            /// 3rd argument: equipement set to use.
            /// 4th argument: character categories ; doesn't change if all are enabled.
            ///  1: male
            ///  2: female
            ///  3: gaian
            ///  4: terran
            ///  5: temporary character
            /// 5th argument: ability and command set to use.
            /// AT_LCHARACTER Character (1 bytes)
            /// AT_BOOL Update Leve (1 bytes)
            /// AT_EQUIPSET Equipement Set (1 bytes)
            /// AT_BOOLLIST Category (1 bytes)
            /// AT_ABILITYSET Ability Set (1 bytes)
            /// </summary>
            JOIN = 0xFE,

            /// <summary>
            /// EXTENDED_CODE
            /// Not an opcode.
            /// </summary>
            EXT = 0xFF,
        }
    }
}