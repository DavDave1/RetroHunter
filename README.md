# RetroHunter


RetroHunter is a Desktop application that builds emulators rom sets by collecting games that have [Retroachievements](http://retroachievements.org/).

RetroHunter is built using the C# [AvaloniaUI](https://avaloniaui.net/) framework with a Win 98 inspired theme thanks to the [Classic.Avalonia](https://github.com/BAndysc/Classic.Avalonia) package.

<div style="text-align: center;">
<img src="docs/images/retro_hunter_main_screen.png" alt="application screenshot" width="500" height="auto">
</div>

## How it works

RetroHunter pulls games data and hashes from [Retroachievements](http://retroachievements.org/) and identifies Roms by following the Retroachievments [Games identification methods](https://docs.retroachievements.org/developer-docs/game-identification.html).

RetroHunter parses roms from an input folder and builds a roms set into a user defined output folder. The output folder structure can be configured to follow the [EmuDeck](https://www.emudeck.com/) roms folder structure, for easy transfer of games to the Steam Deck


## Supported Platforms

- Windows
- Linux


## Supported systems

RetroHunter aims to support all game systems for which achievements are available on [Retroachievements](http://retroachievements.org/).

<details>
<summary> 
<b>Supported roms formats</b> 
</summary>

| System                    | Rom format                         |
| ------------------------- | ---------------------------------- |
| Game Boy                  | .gb, .zip                          |
| Game Boy Color            | .gbc, .zip                         |
| Nes                       | .nes, .fds, .zip                   |
| Snes                      | .sfc .smc .zip                     |
| Nintendo 64               | .d64, .ndd, .n64, .v64, .z64, .zip |
| GameCube                  | .iso                               |
| NindendoDS                | .nds, .zip                         |
| Pokemon Mini              | .min, .zip                         |
| Virtual Boy               | .vbs, .zip                         |
| PlayStation               | .cue, .iso, .chd                   |
| PlayStation 2             | .iso, .chd                         |
| Psp                       | .iso, .chd                         |
| Atari2600                 | .a26, .zip                         |
| Atari7800                 | .a78, .zip                         |
| Atari Jaguar              | .j64, .rom, .zip                   |
| Atari Jaguar CD           | .cue, .chd                         |
| Atary Lynx                | .lyx, .zip                         |
| SG-1000                   | .sg                                |
| Master System             | .sms, .zip                         |
| Genesis                   | .md, .zip                          |
| Sega CD                   | .cue, .chd                         |
| Sega 32X                  | .32x, .zip                         |
| Saturn                    | .cue, .chd                         |
| Dreamcast                 | .iso, .chd                         |
| PC Engine                 | .pce, .zip                         |
| PC Engine CD              | .cue, .iso                         |
| PC-8000/8800              | .88d, .cmt, .d88, .t88, .u88, .zip |
| PC-FX                     | .cue, .chd                         |
| NeoGeo CD                 | .cue, .chd                         |
| NeoGeo Pocket             | .ngp, .ngc, .zip                   |
| 3DO                       | .cue, .chd                         |
| Amstrad CPC               |                                    |
| Apple II                  | .ar2, .woz, .zip                   |
| Arcade                    | .zip                               |
| Arcadia 2001              | .bin, .zip                         |
| Arduboy                   | .hex, .bin                         |
| ColecoVision              | .col                               |
| Elektor TV Games Computer |                                    |
| Fairchild Channel F       | .bin, .zip                         |
| Intellivision             | .int, .zip                         |
| Interton VC 4000          | .bin, .zip                         |
| Magnavox Odyssey2         | .bin, .zip                         |
| Mega Duck                 | .bin, .zip                         |
| MSX                       | .rom, .zip                         |
| Uzebox                    |                                    |
| Vectrex                   | .vec, .zip                         |
| Supervision               | .sv, .zip                          |
| WonderSwan                | .ws, .wsc, zip                     |

</details>

## How to build

RetroHunter requires .NET 9.0 and can be build on Windows and Linux (Verify Mac OS support).

RetroHunter includes the [libchdr](https://github.com/rtissera/libchdr) C library as submodule dependency for reading compressed disk images in CHD format. Before building the RetroHunter C# solution, you need to compile the libchdr libary by running the `build_libchdr.sh` script.


## Status

[![Build Main](https://github.com/DavDave1/RetroHunter/actions/workflows/build-main.yml/badge.svg)](https://github.com/DavDave1/RetroHunter/actions/workflows/build-main.yml)


## TODOs

### Systems
- [x] GameCube matcher
- [x] PlayStation 2 matcher
- [x] Dreamcast matcher
- [ ] Support reading RZV format

### UI
- [ ] Configuration Wizard when opening for 1st time


### Extra features
- [ ] Support downloading and applying patches
- [ ] Display RA user profile summary
- [ ] Implement github actions to build, test and release
- [ ] Implement compression to chd format