using RetroHunter.Models;
using RetroHunter.Utils.Matchers;
using System;
using static RetroHunter.Models.GameSystemData;

namespace RetroHunter.Services
{
    public class MatcherFactory
    {
      public  MatcherBase CreateMatcher(GameSystem system, RetroHunterSettings settings) {
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
                => new Md5Matcher(system),
                GameSystemType.Nes => new NesMatcher(system),
                GameSystemType.SuperNintendo => new SnesMatcher(system),
                GameSystemType.Nintendo64 => new N64Matcher(system),
                GameSystemType.GameCube => new GameCubeMatcher(system, settings.DolphinToolExePath),
                GameSystemType.NintendoDS or
                GameSystemType.NintendoDSi => new NdsMatcher(system),
                GameSystemType.PlayStation => new Ps1Matcher(system),
                GameSystemType.PlayStation2 => new Ps2Matcher(system),
                GameSystemType.Psp => new PspMatcher(system),
                GameSystemType.Atari7800 => new Atari7800Matcher(system),
                GameSystemType.AtariJaguarCD => new JaguarCDMatcher(system),
                GameSystemType.AtariLynx => new LynxMatcher(system),
                GameSystemType.Saturn or
                GameSystemType.SegaCD => new SegaSaturnAndCDMatcher(system),
                GameSystemType.Dreamcast => new DreamcastMatcher(system),
                GameSystemType.PcEngine => new PcEngineMatcher(system),
                GameSystemType.PcEngineCD => new PcEngineCDMatcher(system),
                GameSystemType.PcFx => new PcFxMatcher(system),
                GameSystemType.NeoGeoCD => new NeoGeoCDMatcher(system),
                GameSystemType.ThreeDo => new ThreeDOMatcher(system),
                GameSystemType.Arcade => new FileNameHashMatcher(system),
                GameSystemType.Arduboy => new ArduboyMatcher(system),
                _ => throw new Exception($"Unknown matcher for Game System {system.GameSystemType}"),
            };
        }
    }
}
