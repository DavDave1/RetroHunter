using RetroHunter.Models;
using RetroHunter.Utils.Matchers;
using System;
using System.Collections.Generic;
using static RetroHunter.Models.GameSystemData;

namespace RetroHunter.Services
{
    public class MatcherFactory
    {
      public  MatcherBase CreateMatcher(Dictionary<string, Rom> romsDictionary, GameSystem system, RetroHunterSettings settings) {
            return system.GameSystemType switch
            {
                GameSystemType.GameBoy or
                GameSystemType.GameBoyColor or
                GameSystemType.GameBoyAdvance or
                GameSystemType.PokemonMini or
                GameSystemType.VirtualBoy or
                GameSystemType.Atari2600 or
                GameSystemType.AtariJaguar or
                GameSystemType.Sg1000 or
                GameSystemType.MasterSystem or
                GameSystemType.GameGear or
                GameSystemType.Genesis or
                GameSystemType.Sega32x or
                GameSystemType.Pc8000 or
                GameSystemType.AmstradCPC or
                GameSystemType.AppleII or
                GameSystemType.Arcadia2001 or
                GameSystemType.ColecoVision or
                GameSystemType.ElektorTvGamesComputer or
                GameSystemType.FairchildChannelF or
                GameSystemType.Intellivision or
                GameSystemType.IntertonVC4000 or
                GameSystemType.MagnavoxOdyssey2 or
                GameSystemType.MegaDuck or
                GameSystemType.Msx or
                GameSystemType.Uzebox or
                GameSystemType.Vectrex or
                GameSystemType.Wasm4 or
                GameSystemType.Supervision or
                GameSystemType.WonderSwan or
                GameSystemType.NeoGeoPocket 
                => new Md5Matcher(system, romsDictionary),
                GameSystemType.Nes => new NesMatcher(system, romsDictionary),
                GameSystemType.SuperNintendo => new SnesMatcher(system, romsDictionary),
                GameSystemType.Nintendo64 => new N64Matcher(system, romsDictionary),
                GameSystemType.GameCube => new GameCubeMatcher(system, romsDictionary, settings.DolphinToolExePath),
                GameSystemType.NintendoDS or
                GameSystemType.NintendoDSi => new NdsMatcher(system, romsDictionary),
                GameSystemType.PlayStation => new Ps1Matcher(system, romsDictionary),
                GameSystemType.PlayStation2 => new Ps2Matcher(system, romsDictionary),
                GameSystemType.Psp => new PspMatcher(system, romsDictionary),
                GameSystemType.Atari7800 => new Atari7800Matcher(system, romsDictionary),
                GameSystemType.AtariJaguarCD => new JaguarCDMatcher(system, romsDictionary),
                GameSystemType.AtariLynx => new LynxMatcher(system, romsDictionary),
                GameSystemType.Saturn or
                GameSystemType.SegaCD => new SegaSaturnAndCDMatcher(system, romsDictionary),
                GameSystemType.Dreamcast => new DreamcastMatcher(system, romsDictionary),
                GameSystemType.PcEngine => new PcEngineMatcher(system, romsDictionary),
                GameSystemType.PcEngineCD => new PcEngineCDMatcher(system, romsDictionary),
                GameSystemType.PcFx => new PcFxMatcher(system, romsDictionary),
                GameSystemType.NeoGeoCD => new NeoGeoCDMatcher(system, romsDictionary),
                GameSystemType.ThreeDo => new ThreeDOMatcher(system, romsDictionary),
                GameSystemType.Arcade => new FileNameHashMatcher(system, romsDictionary),
                GameSystemType.Arduboy => new ArduboyMatcher(system, romsDictionary),
                _ => throw new Exception($"Unknown matcher for Game System {system.GameSystemType}"),
            };
        }
    }
}
