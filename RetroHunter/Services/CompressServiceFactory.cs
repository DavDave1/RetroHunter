using Microsoft.Extensions.Logging;
using RetroHunter.Models;
using System;

namespace RetroHunter.Services
{
    public class CompressServiceFactory(ILoggerFactory loggerFactory)
    {
        public ICompressService CreateCompressService(RetroHunterSettings settings, Rom rom)
        {
            var game = rom.Game ?? throw new Exception($"Game is null for rom ${rom.RaName}");
            var gameSystem = game.GameSystem ?? throw new Exception($"GameSystem is null for rom ${game.Name}");

            return gameSystem.GameSystemType switch
            {
                GameSystemData.GameSystemType.GameCube 
                => new DolphinTool(loggerFactory.CreateLogger<DolphinTool>(), settings.DolphinToolExePath),
                GameSystemData.GameSystemType.PlayStation or
                GameSystemData.GameSystemType.PlayStation2 or
                GameSystemData.GameSystemType.Psp or
                GameSystemData.GameSystemType.AtariJaguarCD or
                GameSystemData.GameSystemType.SegaCD or
                GameSystemData.GameSystemType.Saturn or
                GameSystemData.GameSystemType.Dreamcast or
                GameSystemData.GameSystemType.PcFx or
                GameSystemData.GameSystemType.NeoGeoCD or
                GameSystemData.GameSystemType.ThreeDo
                => new Chdman(loggerFactory.CreateLogger<Chdman>(), settings.ChdmanExePath),
                GameSystemData.GameSystemType.GameBoy or
                GameSystemData.GameSystemType.GameBoyColor or
                GameSystemData.GameSystemType.GameBoyAdvance or
                GameSystemData.GameSystemType.Nes or
                GameSystemData.GameSystemType.SuperNintendo or
                GameSystemData.GameSystemType.Nintendo64 or
                GameSystemData.GameSystemType.NintendoDS or
                GameSystemData.GameSystemType.NintendoDSi or
                GameSystemData.GameSystemType.PokemonMini or
                GameSystemData.GameSystemType.VirtualBoy or
                GameSystemData.GameSystemType.Atari2600 or
                GameSystemData.GameSystemType.Atari7800 or
                GameSystemData.GameSystemType.AtariJaguar or
                GameSystemData.GameSystemType.AtariLynx or
                GameSystemData.GameSystemType.MasterSystem or
                GameSystemData.GameSystemType.GameGear or
                GameSystemData.GameSystemType.Genesis or
                GameSystemData.GameSystemType.Sega32x or
                GameSystemData.GameSystemType.PcEngine or
                GameSystemData.GameSystemType.Pc8000 or
                GameSystemData.GameSystemType.NeoGeoPocket or
                GameSystemData.GameSystemType.AppleII or
                GameSystemData.GameSystemType.Arcade or
                GameSystemData.GameSystemType.Arcadia2001 or
                GameSystemData.GameSystemType.FairchildChannelF or
                GameSystemData.GameSystemType.Intellivision or
                GameSystemData.GameSystemType.IntertonVC4000 or
                GameSystemData.GameSystemType.MagnavoxOdyssey2 or
                GameSystemData.GameSystemType.MegaDuck or
                GameSystemData.GameSystemType.Msx or
                GameSystemData.GameSystemType.Vectrex or
                GameSystemData.GameSystemType.Supervision or
                GameSystemData.GameSystemType.WonderSwan
                => throw new Exception($"zip compression service not yet implemented for {gameSystem.GameSystemType.Name()}"),
                GameSystemData.GameSystemType.Sg1000 or
                GameSystemData.GameSystemType.PcEngineCD or
                GameSystemData.GameSystemType.AmstradCPC or
                GameSystemData.GameSystemType.ColecoVision or
                GameSystemData.GameSystemType.Arduboy or
                GameSystemData.GameSystemType.ElektorTvGamesComputer or
                GameSystemData.GameSystemType.Uzebox or
                GameSystemData.GameSystemType.Wasm4
                => throw new Exception($"compression service not available for {gameSystem.GameSystemType.Name()}"),
                _ => throw new Exception($"Compression service not available for unknown game system {gameSystem.GameSystemType.Name()}"),
            };
        }
    }
}
