# 2024-04-?? update

## Features
* Camera moves, scrolling backgrounds and parallax now unrestricted from their grid (smooth movement)
* Player camera stabilization (configurable)
* New audio backend (soLoud): better audio quality and control
* Native support for PS4, PS5 and Switch controllers
* PSX font (by Teaito/zePilot) embedded
* Widescreen support for 16:10 (e.g. Steamdeck)
* Support for more 19+ widescreen backgrounds, including parallax
* New cheat: No time limit for Excalibur II

## Launcher / settings
* Complete re-design of the Launcher and Mod Manager https://github.com/Albeoris/Memoria/pull/283
* Ensured consistency of Memoria.ini
* Explanation on settings when hovering
* New Launcher options: MovieVolume, Anti-Aliasing, PSX font, fast battle intro, FPS for each part, Moguri soundtrack/FMV, Camera smoothness, battle speed, menu skip
* Better list of resolutions
* Launcher Brazilian-pr translation (Thanks to Felipefpl)
* Version date on window title
* New In-game sliders: ATB, volume
* New ini Option: menu rows https://github.com/Albeoris/Memoria/commit/cfc7ccebf17d06aaba47e3d504ca4822c8b6d156#comments

## Bugfixes (gameplay)
* Fixed Moguri crash at endgame "Airship FMV"
* Launcher: Fixed not handling missing Memoria.ini
* Launcher: Fixed crash with debuggable mode
* Battles: Fixed Accuracy+ with Distract
* Worldmap: fixed beach heals soft-lock
* Fixed Cinna/Marcus/Blank scripted heal
* Fixed achievement "The Ultimate Racket" being tied to the wrong item
* Fixed PSXEncounterMethod option creating soft-locks https://github.com/Albeoris/Memoria/issues/345
* Fixed some menus bugs: https://github.com/Albeoris/Memoria/commit/cfc7ccebf17d06aaba47e3d504ca4822c8b6d156#comments
* Fixed Ambient sound problems
* Fixed volume ignored for some sounds
* Fixed leveling-up sound stacking
* Default Battle UI position improved for non-widescreen
* Turn-based mode improvements https://github.com/Albeoris/Memoria/commit/3c0000fa7a4e1877fcd13b8787cdeb01ca06537b

## Bugfixes (visual)
* Widescreen: Fixed ~22 unsupported scenes (e.g. pee scene)
* Fixed quit screen not filling the window
* Fixed pause screen bugged with widescreen
* Fixed Clayra Trunk english name invisible
* Fixed cropped layer/depth problems on ~8 backgrounds (PSX bugs)
* Fixed some parallax problems
* Fixed some screens 1px too wide (vertical line)
* 60fps: Fixed some animations jittering up and down
* 60fps: Fixed some scrolling background trembling with player movement
* 60fps: Fixed 1 frame offcenter when bumping borders
* Ending: Fixed elevator text on background instead of black
* Ending: Fixed some overlayed text
* Restored light layer on Leviathan statue (PSX bug)
* Widescreen: Fixed disappearing people on Alexandria Plaza
* Widescreen: Fixed hunting festival timer overlap
* Widescreen: Fixed some misplacement of dialog bubbles https://github.com/Albeoris/Memoria/commit/acf64fdd76e4864e870efc48c504729f2c2c76d4#comments
* Adjustments in some scenes: Blank jumping in hole, Iifa smoke effect, Floating glass in pub
* Fixed glitch with battle results

## Backend / mods
* (For devs) Project easier to compile, code improvements, debug messages
* title_bg, title_logo and gameover now moddable
* Voice Acting system improvements / fixes https://github.com/Albeoris/Memoria/commit/da5826c4fc4de8ce972cbfd5b791d14f47297376 https://github.com/Albeoris/Memoria/commit/2c7071c045661c686eb6d05a84ff221941fa5950#commitcomment-141268272
* More scripting support: https://github.com/Albeoris/Memoria/commit/06f1ccf3275dc08f58e9bf2c52de22adf79857e5 https://github.com/Albeoris/Memoria/commit/301db7ee520c85782424f15c73972cb81641a3c6 https://github.com/Albeoris/Memoria/commit/82a0a4392bfcde9754a74c64aa911bab773005f4 https://github.com/Albeoris/Memoria/commit/dffc95aea70152c7987383553dd6b7dd3fc8018c https://github.com/Albeoris/Memoria/commit/935e207bdf14484c87b52a1d4a790ce2027770bb#comments https://github.com/Albeoris/Memoria/commit/91e94a66deb83d232a07220d14966cfbb052719f#comments
* Command-line options: https://github.com/Albeoris/Memoria/commit/068c85b6a9b8ff4e2e945cd6b99c7aff3a6f0bc0
* Fixed issues with [`[code=Patch]` ](https://github.com/Albeoris/Memoria/wiki/Active-ability-features) (e.g. MP cost confuse base and replacement ability)
* More information on window title