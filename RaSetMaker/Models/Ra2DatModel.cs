using System.Collections.Generic;

namespace RaSetMaker.Models
{
    public class Ra2DatModel : ModelBase
    {
        public UserConfig UserConfig { get; set; } = new();

        public List<GameSystem> Systems { get; set; } = [];

        public Ra2DatModel() : base()
        {
        }

        public void InitGameSystems()
        {
            Systems.Add(new(
                GameSystemCompany.Nintendo,
                "Nintendo 64",
                RomMatcherType.Nintendo64,
                new() { { DirStructureStyle.Retroachievements, "Nintendo 64" }, { DirStructureStyle.EmuDeck, "n64" } },
                [".bin ", ".d64", ".ndd", ".u1", ".n64", ".z64", ".v64"]));

            Systems.Add(new(
                GameSystemCompany.Nintendo, 
                "SNES/Super Famicom", 
                RomMatcherType.Snes, 
                new() { { DirStructureStyle.Retroachievements, "SNES" }, { DirStructureStyle.EmuDeck, "snes" } },
                [".bin", ".bml", ".bs", ".bsx", ".dx2", ".fig", ".gd3", ".gd7", ".mgd", ".sfc", ".smc", ".st", ".swc"]));

            Systems.Add(new(
                GameSystemCompany.Nintendo, 
                "Game Boy", 
                RomMatcherType.Md5,
                new() { { DirStructureStyle.Retroachievements, "Game Boy" }, { DirStructureStyle.EmuDeck, "gb" } },
                [".gb"]));

            Systems.Add(new(
                GameSystemCompany.Nintendo, 
                "Game Boy Advance", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Game Boy Advance" }, { DirStructureStyle.EmuDeck, "gba" } },
                [".gba"]));

            Systems.Add(new(
                GameSystemCompany.Nintendo, 
                "Game Boy Color", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Game Boy Color" }, { DirStructureStyle.EmuDeck, "gbc" } },
                [".gbc"]));

            Systems.Add(new(
                GameSystemCompany.Nintendo, 
                "NES/Famicom", 
                RomMatcherType.Nes, 
                new() { { DirStructureStyle.Retroachievements, "NES" }, { DirStructureStyle.EmuDeck, "nes" } },
                [".3dsen", ".fds", ".qd", ".nes", ".unf", ".unif"]));

            Systems.Add(new(
                GameSystemCompany.Nintendo, 
                "GameCube", 
                RomMatcherType.GameCube, 
                new() { { DirStructureStyle.Retroachievements, "GameCube" }, { DirStructureStyle.EmuDeck, "gc" } },
                [".ciso", ".dff", ".dol", ".elf", ".gcm", ".gcz", ".iso", ".json", ".m3u", ".rvz", ".tgc", ".wad", ".wbfs", ".wia"]));

            Systems.Add(new(
                GameSystemCompany.Nintendo, 
                "Nintendo DS", 
                RomMatcherType.NintendoDS, 
                new() { { DirStructureStyle.Retroachievements, "Nintendo DS" }, { DirStructureStyle.EmuDeck, "nds" } },
                [".app", ".bin", ".nds"]));

            Systems.Add(new(
                GameSystemCompany.Nintendo, 
                "Nintendo DSi", 
                RomMatcherType.NintendoDS, 
                new() { { DirStructureStyle.Retroachievements, "Nintendo DSi" }, { DirStructureStyle.EmuDeck, "nds" } },
                [".app", ".bin", ".nds"]));

            Systems.Add(new(
                GameSystemCompany.Nintendo, 
                "Pokemon Mini", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Pokemon Mini" }, { DirStructureStyle.EmuDeck, "pokemini" } },
                [".min"]));

            Systems.Add(new(
                GameSystemCompany.Nintendo, 
                "Virtual Boy", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Virtual Boy" }, { DirStructureStyle.EmuDeck, "virtualboy" } },
                [".bin", ".vb", ".vboy"]));

            Systems.Add(new(
                GameSystemCompany.NEC, 
                "PC Engine/TurboGrafx-16", 
                RomMatcherType.PcEngine, 
                new() { { DirStructureStyle.Retroachievements, "PC Engine" }, { DirStructureStyle.EmuDeck, "pcengine" } },
                [".pce"]));

            Systems.Add(new(
                GameSystemCompany.NEC,
                "PC Engine CD/TurboGrafx-CD", 
                RomMatcherType.PcEngineCD, 
                new() { { DirStructureStyle.Retroachievements, "PC Engine CD" }, { DirStructureStyle.EmuDeck, "pcenginecd" } },
                [".ccd", ".chd", ".cue", ".img", ".iso", ".m3u", ".pce", ".sgx", ".toc"]));

            Systems.Add(new(GameSystemCompany.NEC, 
                "PC-8000/8800", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "PC-8000" }, { DirStructureStyle.EmuDeck, "pc88" } },
                [".88d", ".cmt", ".d88", ".m3u", ".t88", ".u88"]));

            Systems.Add(new(
                GameSystemCompany.NEC, 
                "PC-FX", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "PC-FX" }, { DirStructureStyle.EmuDeck, "pcfx" } },
                [".ccd", ".chd", ".cue", ".m3u", ".toc"]));

            Systems.Add(new(
                GameSystemCompany.Sega, 
                "Genesis/Mega Drive", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Genesis" }, { DirStructureStyle.EmuDeck, "genesis" } },
                [".md"]));

            Systems.Add(new(
                GameSystemCompany.Sega, 
                "Sega CD", 
                RomMatcherType.SegaCD, 
                new() { { DirStructureStyle.Retroachievements, "Sega CD" }, { DirStructureStyle.EmuDeck, "segacd" } },
                []));

            Systems.Add(new(
                GameSystemCompany.Sega, 
                "32X", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "32X" }, { DirStructureStyle.EmuDeck, "sega32x" } },
                [".32x"]));

            Systems.Add(new(
                GameSystemCompany.Sega, 
                "Game Gear", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Game Gear" }, { DirStructureStyle.EmuDeck, "gamegear" } },
                [".gg"]));

            Systems.Add(new(
                GameSystemCompany.Sega, 
                "Master System", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Master System" }, { DirStructureStyle.EmuDeck, "mastersystem" } }, 
                [".sms"]));

            Systems.Add(new(
                GameSystemCompany.Sega,
                "SG-1000", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "SG-1000" }, { DirStructureStyle.EmuDeck, "sg-1000" } },
                [".sg"]));

            Systems.Add(new(
                GameSystemCompany.Sega, 
                "Saturn", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Saturn" }, { DirStructureStyle.EmuDeck, "saturn" } }, 
                []));

            Systems.Add(new(
                GameSystemCompany.Sega, 
                "Dreamcast", 
                RomMatcherType.Dreamcast, 
                new() { { DirStructureStyle.Retroachievements, "Dreamcast" }, { DirStructureStyle.EmuDeck, "dreamcast" } }, 
                [".cue", ".chd", ".iso"]));

            Systems.Add(new(
                GameSystemCompany.Sony, 
                "PlayStation", 
                RomMatcherType.Ps1, 
                new() { { DirStructureStyle.Retroachievements, "PlayStation" }, { DirStructureStyle.EmuDeck, "psx" } },
                [".cue", ".chd", ".iso"]));

            Systems.Add(new(
                GameSystemCompany.Sony, 
                "PlayStation 2", 
                RomMatcherType.Ps2, 
                new() { { DirStructureStyle.Retroachievements, "PlayStation 2" }, { DirStructureStyle.EmuDeck, "ps2" } },
                [".chd", ".iso"]));

            Systems.Add(new(
                GameSystemCompany.Sony, 
                "PlayStation Portable", 
                RomMatcherType.Psp, 
                new() { { DirStructureStyle.Retroachievements, "PlayStation Portable" }, { DirStructureStyle.EmuDeck, "psp" } },
                [".chd", ".iso"]));

            Systems.Add(new(
                GameSystemCompany.Atari, 
                "Atari Lynx", 
                RomMatcherType.AtariLynx, 
                new() { { DirStructureStyle.Retroachievements, "Atari LynxD" }, { DirStructureStyle.EmuDeck, "lynx" } },
                [".lyx"]));

            Systems.Add(new(
                GameSystemCompany.Atari, 
                "Atari Jaguar", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Atari Jaguar" }, { DirStructureStyle.EmuDeck, "atarijaguar" } },
                [".j64", ".rom"]));

            Systems.Add(new(
                GameSystemCompany.Atari, 
                "Atari Jaguar CD", 
                RomMatcherType.AtariJaguarCD, 
                new() { { DirStructureStyle.Retroachievements, "Atari Jaguar CD" }, { DirStructureStyle.EmuDeck, "atarijaguarcd" } },
                []));

            Systems.Add(new(
                GameSystemCompany.Atari, 
                "Atari 2600", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Atari 2600" }, { DirStructureStyle.EmuDeck, "atari2600" } },
                [".a26"]));

            Systems.Add(new(
                GameSystemCompany.Atari, 
                "Atari 7800", 
                RomMatcherType.Atari7800, 
                new() { { DirStructureStyle.Retroachievements, "Atari 7800" }, { DirStructureStyle.EmuDeck, "atari7800" } },
                [".a78"]));

            Systems.Add(new(
                GameSystemCompany.SNK, 
                "Neo Geo Pocket", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Neo Geo Pocket" }, { DirStructureStyle.EmuDeck, "ngp" } },
                [".ngp", ".ngc"]));

            Systems.Add(new(
                GameSystemCompany.SNK, 
                "Neo Geo CD", 
                RomMatcherType.NeoGeoCD, 
                new() { { DirStructureStyle.Retroachievements, "Neo Geo CD" }, { DirStructureStyle.EmuDeck, "neogeocd" } }, 
                []));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "Magnavox Odyssey 2", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Magnavox Odyssey 2" }, { DirStructureStyle.EmuDeck, "odyssey2" } }, 
                [".bin"]));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "Arcade", 
                RomMatcherType.FileNameHash, 
                new() { { DirStructureStyle.Retroachievements, "Arcade" }, { DirStructureStyle.EmuDeck, "arcade" } },
                [".zip", ".7zip"]));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "MSX", 
                RomMatcherType.Md5, new() { { DirStructureStyle.Retroachievements, "MSX" }, { DirStructureStyle.EmuDeck, "msx" } },
                [".rom"]));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "Amstrad CPC", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Amstrad CPC" }, { DirStructureStyle.EmuDeck, "amstradcpc" } },
                []));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "Apple II", 
                RomMatcherType.Md5,
                new() { { DirStructureStyle.Retroachievements, "Apple II" }, { DirStructureStyle.EmuDeck, "apple2" } },
                [".a2r", ".wav", ".woz"]));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "3DO Interactive Multiplayer", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "3DO Interactive Multiplayer" }, { DirStructureStyle.EmuDeck, "3do" } },
                []));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "ColecoVision", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "ColecoVision" }, { DirStructureStyle.EmuDeck, "colecovision" } }, 
                [".col"]));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "Intellivision", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Intellivision" }, { DirStructureStyle.EmuDeck, "intellivision" } },
                [".int"]));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "Vectrex", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Vectrex" }, { DirStructureStyle.EmuDeck, "vectrex" } },
                [".vec"]));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "WonderSwan", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "WonderSwan" }, { DirStructureStyle.EmuDeck, "wonderswan" } },
                [".ws", ".wsc"]));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "Fairchild Channel F", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Fairchild Channel F" }, { DirStructureStyle.EmuDeck, "channelf" } }, 
                [".bin"]));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "Watara Supervision", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Watara Supervision" }, { DirStructureStyle.EmuDeck, "supervision" } },
                [".sv"]));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "Mega Duck", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Mega Duck" }, { DirStructureStyle.EmuDeck, "megaduck" } },
                [".bin"]));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "Arduboy", 
                RomMatcherType.Arduboy, 
                new() { { DirStructureStyle.Retroachievements, "Arduboy" }, { DirStructureStyle.EmuDeck, "arduboy" } }, 
                [".hex"]));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "WASM-4", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "WASM-4" }, { DirStructureStyle.EmuDeck, "wasm4" } }, 
                []));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "Arcadia 2001", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Arcadia 2001" }, { DirStructureStyle.EmuDeck, "arcadia" } }, 
                [".bin"]));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "Interton VC 4000", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Interton VC 4000" }, { DirStructureStyle.EmuDeck, "unsupported_vc4000" } }, 
                [".bin"]));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "Elektor TV Games Computer", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Elektor TV Games Computer" }, { DirStructureStyle.EmuDeck, "unsupported_tvgamescomputer" } }, 
                []));

            Systems.Add(new(
                GameSystemCompany.Other, 
                "Uzebox", 
                RomMatcherType.Md5, 
                new() { { DirStructureStyle.Retroachievements, "Uzebox" }, { DirStructureStyle.EmuDeck, "uzebox" } }, 
                []));
        }
    }
}
