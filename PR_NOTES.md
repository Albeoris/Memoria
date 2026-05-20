# World Map Sound Effects for Modders

## Summary

Adds support for custom looping sound effects on the world map, tied to rain and world effects (SandStorm, Memoria, etc.). Sounds are proximity-based, fading in/out as the player moves closer/further from the effect. Sounds also carry over into random battles automatically.

## How It Works

### Rain Sound

Add a `[RainSound=<path>]` entry to your `Environment.txt` to attach a sound to the default Burmecia rain without changing its position or behavior:

```
Rain [RainSound=SE/rain_loop]
```

Or attach it to a custom rain zone:

```
Rain Add [Position=(247888,-185944)] [RadiusLarge=9184] [RadiusSmall=992] [RainStrength=64] [RainSpeed=64] [RainSound=SE/rain_loop]
```

The volume scales with rain strength — louder near the center, fading at the edges.

### Effect Sounds

Add a `[Sound=<path>]` parameter to any `Effect` entry in `Environment.txt`:

```
Effect SandStorm [Sound=SE/sandstorm_wind]
Effect Memoria [Sound=SE/memoria_ambience]
Effect WaterShrine [Sound=SE/shrine_rumble]
Effect FireShrine [Sound=SE/fire_crackle]
Effect AlexandriaWaterfall [Sound=SE/waterfall_roar]
Effect Windmill [Sound=SE/windmill_creak]
```

Effect sounds are proximity-based — volume is 1.0 at the effect position and fades to 0 at the audible radius edge (~500 world units). Sounds only play when the effect is active (controlled by `[Condition=]` or the default game logic).

You can combine `[Condition=]` and `[Sound=]` on the same line:

```
Effect SandStorm [Condition=GetGlobal_b(190) < 5] [Sound=SE/sandstorm_wind]
```

### Available Effect Names

| Effect Name | Location | Notes |
|---|---|---|
| `SandStorm` | Cleyra | Active on Disc 1 before Cleyra is destroyed |
| `Memoria` | Memoria entrance | Active on Disc 4 |
| `FireShrine` | Fire Shrine | Always active |
| `WaterShrine` | Earth/Water Shrine | Active when shrine is opened |
| `AlexandriaWaterfall` | Alexandria | Always active |
| `Windmill` | Dali area | Active before/after certain story points |

### Battle Carry-Over

When a random encounter triggers while the player is within range of an active sound (rain or effect), that sound automatically plays during the battle at the same volume it was at on the world map. It stops and unloads when the battle ends.

No scripting or battle init changes are needed — it's fully automatic.

## File Placement

Sound files go in your mod folder at:

```
<ModFolder>/FF9_Data/Sounds/<path>.ogg
```

For example, `[RainSound=SE/rain_loop]` resolves to:

```
<ModFolder>/FF9_Data/Sounds/SE/rain_loop.ogg
```

The `.ogg` extension is appended automatically by the loader.

### Looping

For seamless looping, add vorbis comments to your OGG file:

```
LoopStart=0
LoopEnd=<total sample count - 1>
```

If no loop points are set, the sound will play once and restart (which may cause a brief gap).

## Files Changed

- `Assembly-CSharp/Global/ff9/ff9.cs` — Rain sound integration, effect sound servicing in `w_cellService`, `w_effectServiceSound` method
- `Assembly-CSharp/Global/battle/BattleRainRenderer.cs` — Battle rain and effect sound playback
- `Assembly-CSharp/Memoria/World/WorldConfiguration.cs` — `[Sound=]` and `[RainSound=]` parsing, `GetEffectSoundPath`, `BattleRainSoundPath`, `BattleEffectSounds`
- `Assembly-CSharp/Memoria/World/WorldSoundPlayer.cs` — `WorldSoundPlayer` (loads/plays/stops a single sound) and `WorldSoundService` (manages multiple keyed instances)
- `Assembly-CSharp/Assembly-CSharp.csproj` — Added `WorldSoundPlayer.cs` to compile list
