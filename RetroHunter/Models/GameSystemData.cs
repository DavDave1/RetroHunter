
using System;
using System.Collections.Generic;
using DiscUtils.Streams;
using RetroHunter.Utils.Matchers;

namespace RetroHunter.Models;

public static class GameSystemData
{
    public enum GameSystemType
    {
        Genesis = 1,
        Nintendo64 = 2,
        SuperNintendo = 3,
        GameBoy = 4,
        GameBoyAdvance = 5,
        GameBoyColor = 6,
        Nes = 7,
        PcEngine = 8,
        SegaCD = 9,
        Sega32x = 10,
        MasterSystem = 11,
        PlayStation = 12,
        AtariLynx = 13,
        NeoGeoPocket = 14,
        GameGear = 15,
        GameCube = 16,
        AtariJaguar = 17,
        NintendoDS = 18,
        PlayStation2 = 21,
        MagnavoxOdyssey2 = 23,
        PokemonMini = 24,
        Atari2600 = 25,
        Arcade = 27,
        VirtualBoy = 28,
        Msx = 29,
        Sg1000 = 33,
        AmstradCPC = 37,
        AppleII = 38,
        Saturn = 39,
        Dreamcast = 40,
        Psp = 41,
        ThreeDo = 43,
        ColecoVision = 44,
        Intellivision = 45,
        Vectrex = 46,
        Pc8000 = 47,
        PcFx = 49,
        Atari7800 = 51,
        WonderSwan = 53,
        NeoGeoCD = 56,
        FairchildChannelF = 57,
        Supervision = 63,
        MegaDuck = 69,
        Arduboy = 71,
        Wasm4 = 72,
        Arcadia2001 = 73,
        IntertonVC4000 = 74,
        ElektorTvGamesComputer = 75,
        PcEngineCD = 76,
        AtariJaguarCD = 77,
        NintendoDSi = 78,
        Uzebox = 80,
    }

    public static string Name(this GameSystemType type)
    {
        return type switch
        {
            GameSystemType.GameBoy => "Game Boy",
            GameSystemType.GameBoyColor => "Game Boy Color",
            GameSystemType.GameBoyAdvance => "Game Boy Advance",
            GameSystemType.Nes => "NES/Famicom",
            GameSystemType.SuperNintendo => "SNES/Super Famicom",
            GameSystemType.Nintendo64 => "Nintendo 64",
            GameSystemType.GameCube => "Game Cube",
            GameSystemType.NintendoDS => "Nintendo DS",
            GameSystemType.NintendoDSi => "Nintendo DSi",
            GameSystemType.PokemonMini => "Pokemon Mini",
            GameSystemType.VirtualBoy => "Virtual Boy",
            GameSystemType.PlayStation => "PlayStation",
            GameSystemType.PlayStation2 => "PlayStation 2",
            GameSystemType.Psp => "PlayStation Portable",
            GameSystemType.Atari2600 => "Atari 2600",
            GameSystemType.Atari7800 => "Atari 7800",
            GameSystemType.AtariJaguar => "Atari Jaguar",
            GameSystemType.AtariJaguarCD => "Atari Jaguar CD",
            GameSystemType.AtariLynx => "Atari Lynx",
            GameSystemType.Sg1000 => "SG-1000",
            GameSystemType.MasterSystem => "Master System",
            GameSystemType.GameGear => "Game Gear",
            GameSystemType.Genesis => "Genesis/Mega Drive",
            GameSystemType.SegaCD => "Sega CD",
            GameSystemType.Sega32x => "32X",
            GameSystemType.Saturn => "Saturn",
            GameSystemType.Dreamcast => "Dreamcast",
            GameSystemType.PcEngine => "PC Engine/Turbografx-16",
            GameSystemType.PcEngineCD => "PC Engine CD/Turbografx-CD",
            GameSystemType.Pc8000 => "PC-8000/8800",
            GameSystemType.PcFx => "PC-FX",
            GameSystemType.NeoGeoCD => "Neo Geo CD",
            GameSystemType.NeoGeoPocket => "Neo Geo Pocket",
            GameSystemType.ThreeDo => "3DO Interactive Multiplayer",
            GameSystemType.AmstradCPC => "Amstrad CPC",
            GameSystemType.AppleII => "Apple II",
            GameSystemType.Arcade => "Arcade",
            GameSystemType.Arcadia2001 => "Arcadia 2001",
            GameSystemType.Arduboy => "Arduboy",
            GameSystemType.ColecoVision => "ColecoVision",
            GameSystemType.ElektorTvGamesComputer => "Elektor TV Games Computer",
            GameSystemType.FairchildChannelF => "Fairchild Channel F",
            GameSystemType.Intellivision => "Intellivision",
            GameSystemType.IntertonVC4000 => "Interton VC 4000",
            GameSystemType.MagnavoxOdyssey2 => "Magnavox Odissey 2",
            GameSystemType.MegaDuck => "Mega Duck",
            GameSystemType.Msx => "MSX",
            GameSystemType.Uzebox => "Uzebox",
            GameSystemType.Vectrex => "Vectrex",
            GameSystemType.Wasm4 => "WASM-4",
            GameSystemType.Supervision => "Watara Supervision",
            GameSystemType.WonderSwan => "WonderSwan",
            _ => throw new Exception($"Unknown name for Game System {type}"),
        };
    }

    public static string IconUrl(this GameSystemType type)
    {
        return type switch
        {
            GameSystemType.GameBoy => "",
            GameSystemType.GameBoyColor => "",
            GameSystemType.GameBoyAdvance => "avares://RetroHunter/Assets/GameSystem/Gameboy Advance.png",
            GameSystemType.Nes => "avares://RetroHunter/Assets/GameSystem/Nintendo NES.png",
            GameSystemType.SuperNintendo => "avares://RetroHunter/Assets/GameSystem/Nintendo SNES.png",
            GameSystemType.Nintendo64 => "avares://RetroHunter/Assets/GameSystem/Nintendo 64.png",
            GameSystemType.GameCube => "avares://RetroHunter/Assets/GameSystem/Nintendo Gamecube.png",
            GameSystemType.NintendoDS => "avares://RetroHunter/Assets/GameSystem/Nintendo DS.png",
            GameSystemType.NintendoDSi => "avares://RetroHunter/Assets/GameSystem/Nintendo DS.png",
            GameSystemType.PokemonMini => "",
            GameSystemType.VirtualBoy => "",
            GameSystemType.PlayStation => "",
            GameSystemType.PlayStation2 => "avares://RetroHunter/Assets/GameSystem/Sony Playstation 2.png",
            GameSystemType.Psp => "avares://RetroHunter/Assets/GameSystem/Sony Playstation Portable.png",
            GameSystemType.Atari2600 => "",
            GameSystemType.Atari7800 => "",
            GameSystemType.AtariJaguar => "",
            GameSystemType.AtariJaguarCD => "",
            GameSystemType.AtariLynx => "",
            GameSystemType.Sg1000 => "",
            GameSystemType.MasterSystem => "",
            GameSystemType.GameGear => "",
            GameSystemType.Genesis => "avares://RetroHunter/Assets/GameSystem/Sega Genesis.png",
            GameSystemType.SegaCD => "",
            GameSystemType.Sega32x => "",
            GameSystemType.Saturn => "avares://RetroHunter/Assets/GameSystem/Sega Saturn.png",
            GameSystemType.Dreamcast => "avares://RetroHunter/Assets/GameSystem/Sega Dreamcast.png",
            GameSystemType.PcEngine => "",
            GameSystemType.PcEngineCD => "",
            GameSystemType.Pc8000 => "",
            GameSystemType.PcFx => "",
            GameSystemType.NeoGeoCD => "avares://RetroHunter/Assets/GameSystem/SNK Neo Geo.png",
            GameSystemType.NeoGeoPocket => "",
            GameSystemType.ThreeDo => "",
            GameSystemType.AmstradCPC => "",
            GameSystemType.AppleII => "",
            GameSystemType.Arcade => "avares://RetroHunter/Assets/GameSystem/MAME.png",
            GameSystemType.Arcadia2001 => "",
            GameSystemType.Arduboy => "",
            GameSystemType.ColecoVision => "",
            GameSystemType.ElektorTvGamesComputer => "",
            GameSystemType.FairchildChannelF => "",
            GameSystemType.Intellivision => "",
            GameSystemType.IntertonVC4000 => "",
            GameSystemType.MagnavoxOdyssey2 => "",
            GameSystemType.MegaDuck => "",
            GameSystemType.Msx => "",
            GameSystemType.Uzebox => "",
            GameSystemType.Vectrex => "",
            GameSystemType.Wasm4 => "",
            GameSystemType.Supervision => "",
            GameSystemType.WonderSwan => "",
            _ => throw new Exception($"Unknown name for Game System {type}"),
        };
    }

    public static string IconUrl(this GameSystemCompany company)
    {
        return company switch
        {
            GameSystemCompany.Atari => "",
            GameSystemCompany.Nintendo => "",
            GameSystemCompany.Sega => "avares://RetroHunter/Assets/sega.png",
            GameSystemCompany.Sony => "",
            GameSystemCompany.NEC => "",
            GameSystemCompany.SNK => "",
            GameSystemCompany.Other => "",
            _ => throw new NotImplementedException($"Unknown icon url for Game System Company {company}"),
        };

    }

    public static GameSystemCompany Company(this GameSystemType type)
    {
        return type switch
        {
            GameSystemType.GameBoy or
            GameSystemType.GameBoyColor or
            GameSystemType.GameBoyAdvance or
            GameSystemType.Nes or
            GameSystemType.SuperNintendo or
            GameSystemType.Nintendo64 or
            GameSystemType.GameCube or
            GameSystemType.NintendoDS or
            GameSystemType.NintendoDSi or
            GameSystemType.PokemonMini or
            GameSystemType.VirtualBoy
             => GameSystemCompany.Nintendo,
            GameSystemType.PlayStation or
            GameSystemType.PlayStation2 or
            GameSystemType.Psp
            => GameSystemCompany.Sony,
            GameSystemType.Atari2600 or
            GameSystemType.Atari7800 or
            GameSystemType.AtariJaguar or
            GameSystemType.AtariJaguarCD or
            GameSystemType.AtariLynx
            => GameSystemCompany.Atari,
            GameSystemType.Sg1000 or
            GameSystemType.MasterSystem or
            GameSystemType.GameGear or
            GameSystemType.Genesis or
            GameSystemType.SegaCD or
            GameSystemType.Sega32x or
            GameSystemType.Saturn or
            GameSystemType.Dreamcast
            => GameSystemCompany.Sega,
            GameSystemType.PcEngine or
            GameSystemType.PcEngineCD or
            GameSystemType.Pc8000 or
            GameSystemType.PcFx
            => GameSystemCompany.NEC,
            GameSystemType.NeoGeoCD or
            GameSystemType.NeoGeoPocket
            => GameSystemCompany.SNK,
            _ => GameSystemCompany.Other,
        };
    }

    public static string FolderName(this GameSystemType type, DirStructureStyle style)
    {
        return style switch
        {
            DirStructureStyle.Retroachievements => type switch
            {
                GameSystemType.SuperNintendo => "SNES",
                GameSystemType.Nes => "NES",
                GameSystemType.PcEngine => "PC Engine",
                GameSystemType.PcEngineCD => "PC Engine CD",
                GameSystemType.Pc8000 => "PC-8000",
                _ => type.Name(),
            },
            DirStructureStyle.EmuDeck => type switch
            {
                GameSystemType.GameBoy => "gb",
                GameSystemType.GameBoyColor => "gbc",
                GameSystemType.GameBoyAdvance => "gba",
                GameSystemType.Nes => "nes",
                GameSystemType.SuperNintendo => "snes",
                GameSystemType.Nintendo64 => "n64",
                GameSystemType.GameCube => "gc",
                GameSystemType.NintendoDS => "nds",
                GameSystemType.NintendoDSi => "nds",
                GameSystemType.PokemonMini => "pokemini",
                GameSystemType.VirtualBoy => "virtualboy",
                GameSystemType.PlayStation => "psx",
                GameSystemType.PlayStation2 => "ps2",
                GameSystemType.Psp => "psp",
                GameSystemType.Atari2600 => "atari2600",
                GameSystemType.Atari7800 => "atari7800",
                GameSystemType.AtariJaguar => "atarijaguar",
                GameSystemType.AtariJaguarCD => "atarijaguarcd",
                GameSystemType.AtariLynx => "lynx",
                GameSystemType.Sg1000 => "sg-1000",
                GameSystemType.MasterSystem => "masyersystem",
                GameSystemType.GameGear => "gamegear",
                GameSystemType.Genesis => "genesis",
                GameSystemType.SegaCD => "Segacd",
                GameSystemType.Sega32x => "sega32x",
                GameSystemType.Saturn => "saturn",
                GameSystemType.Dreamcast => "dreamcast",
                GameSystemType.PcEngine => "pcengine",
                GameSystemType.PcEngineCD => "pcenginecd",
                GameSystemType.Pc8000 => "pc88",
                GameSystemType.PcFx => "pcfx",
                GameSystemType.NeoGeoCD => "neogeocd",
                GameSystemType.NeoGeoPocket => "ngp",
                GameSystemType.ThreeDo => "3do",
                GameSystemType.AmstradCPC => "amstradcpc",
                GameSystemType.AppleII => "apple2",
                GameSystemType.Arcade => "fbneo",
                GameSystemType.Arcadia2001 => "arcadia",
                GameSystemType.Arduboy => "arduboy",
                GameSystemType.ColecoVision => "colecovision",
                GameSystemType.ElektorTvGamesComputer => "tvgc",
                GameSystemType.FairchildChannelF => "channelf",
                GameSystemType.Intellivision => "intellivision",
                GameSystemType.IntertonVC4000 => "vc4000",
                GameSystemType.MagnavoxOdyssey2 => "odyssey2",
                GameSystemType.MegaDuck => "megaduck",
                GameSystemType.Msx => "msx",
                GameSystemType.Uzebox => "uzebox",
                GameSystemType.Vectrex => "vectrex",
                GameSystemType.Wasm4 => "wasm4j",
                GameSystemType.Supervision => "supervision",
                GameSystemType.WonderSwan => "wonderswan",
                _ => throw new Exception("Unknown emudeck folder name for {type}"),
            },
            _ => throw new Exception($"Unknown dir structure style")
        };
    }

    public static List<string> Extensions(this GameSystemType type)
    {
        return type switch
        {

            GameSystemType.GameBoy => [".gb"],
            GameSystemType.GameBoyColor => [".gbc"],
            GameSystemType.GameBoyAdvance => [".gba"],
            GameSystemType.Nes => [".nes", ".fds"],
            GameSystemType.SuperNintendo => [".sfc", ".smc"],
            GameSystemType.Nintendo64 => [".d64", ".ndd", ".n64", ".v64", ".z64"],
            GameSystemType.GameCube => [".iso", ".gcz", ".rvz"],
            GameSystemType.NintendoDS or
            GameSystemType.NintendoDSi => [".nds"],
            GameSystemType.PokemonMini => [".min"],
            GameSystemType.VirtualBoy => [".vb", ".vboy"],
            GameSystemType.PlayStation => [".cue", ".iso", ".chd"],
            GameSystemType.PlayStation2 => [".iso", ".chd"],
            GameSystemType.Psp => [".iso", ".chd"],
            GameSystemType.Atari2600 => [".a26"],
            GameSystemType.Atari7800 => [".a78"],
            GameSystemType.AtariJaguar => [".j64", ".rom"],
            GameSystemType.AtariJaguarCD => [".cue"],
            GameSystemType.AtariLynx => [".lyx"],
            GameSystemType.Sg1000 => [".sg"],
            GameSystemType.MasterSystem => [".sms"],
            GameSystemType.GameGear => [".gg"],
            GameSystemType.Genesis => [".md"],
            GameSystemType.SegaCD => [".cue", ".chd"],
            GameSystemType.Sega32x => [".32x"],
            GameSystemType.Saturn => [".cue", ".chd"],
            GameSystemType.Dreamcast => [".cue", ".chd"],
            GameSystemType.PcEngine => [".pce"],
            GameSystemType.PcEngineCD => [".cue"],
            GameSystemType.Pc8000 => [".88d", ".cmt", ".d88", ".t88", ".u88"],
            GameSystemType.PcFx => [".cue"],
            GameSystemType.NeoGeoCD => [".cue"],
            GameSystemType.NeoGeoPocket => [".ngp", ".ngc"],
            GameSystemType.ThreeDo => [".cue"],
            GameSystemType.AmstradCPC => [],
            GameSystemType.AppleII => [".ar2", ".woz"],
            GameSystemType.Arcade => [".zip"],
            GameSystemType.Arcadia2001 => [".bin"],
            GameSystemType.Arduboy => [".hex"],
            GameSystemType.ColecoVision => [".col"],
            GameSystemType.ElektorTvGamesComputer => [],
            GameSystemType.FairchildChannelF => [".bin"],
            GameSystemType.Intellivision => [".int"],
            GameSystemType.IntertonVC4000 => [".bin"],
            GameSystemType.MagnavoxOdyssey2 => [".bin"],
            GameSystemType.MegaDuck => [".bin"],
            GameSystemType.Msx => [".rom"],
            GameSystemType.Uzebox => [],
            GameSystemType.Vectrex => [".vec"],
            GameSystemType.Wasm4 => [],
            GameSystemType.Supervision => [".sv"],
            GameSystemType.WonderSwan => [".ws", ".wsc"],
            _ => throw new Exception($"Unknown extensions for Game System {type}")
        };
    }

    public static long RomSizeLimit(this GameSystemType type)
    {
        return type switch
        {
            GameSystemType.MagnavoxOdyssey2 => 8192,
            GameSystemType.FairchildChannelF or
            GameSystemType.IntertonVC4000 or
            GameSystemType.Arcadia2001 or
            GameSystemType.MegaDuck => 65536,
            _ => 0,
        };
    }

}
