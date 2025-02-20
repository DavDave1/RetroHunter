﻿using RaSetMaker.Models;
using RaSetMaker.Utils;
using static RaSetMaker.Models.GameSystemData;

namespace RaSetMaker.Tests.MatchersTests
{
    public class MatchersTest
    {
        protected Ra2DatModel _model = new();

        public MatchersTest()
        {
            _model.InitGameSystems();
        }

        protected GameSystem GetGameSystemByType(GameSystemType type)
        {
            return _model.Systems.First(s => s.GameSystemType == type);
        }

        protected void AddRomWithHash(GameSystem system, string hash)
        {
            var game = new Game(system);
            game.Roms.Add(new Rom(game)
            {
                Hash = hash
            });

            system.Games.Add(game);
        }

        [Theory]
        [InlineData("../../../TestRoms/atari2600.zip", "0db4f4150fecf77e4ce72ca4d04c052f")]
        public void Md5HashMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.Atari2600);
            AddRomWithHash(sys, expectedHash);
            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);

        }

        [Theory]
        [InlineData("../../../TestRoms/arduboy.zip", "cee7f24fab74cef92ff1e03cd76d38cb")]
        public void ArduboyHashMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.Arduboy);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var foundRom = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom.Item1);
            Assert.Equal(expectedHash, foundRom.Item1.Hash);
        }

        [Theory]
        [InlineData("../../../TestRoms/progolf.zip", "d0f2f686b61f08f07cd2925bb3ae8b41")]
        public void FileNameHashMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var arcade = GetGameSystemByType(GameSystemType.Arcade);
            AddRomWithHash(arcade, expectedHash);

            var matcher = RomMatcherFactory.Create(arcade);
            Assert.NotNull(matcher);
            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }

        [Theory]
        [InlineData("../../../TestRoms/Animal Crossing (USA).iso", "4f69c7886162509baa0882062bb2e1c8")]
        public void GameCubeHashMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.GameCube);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var foundRom = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom.Item1);
            Assert.Equal(expectedHash, foundRom.Item1.Hash);

        }

        [Theory]
        [InlineData("../../../TestRoms/nintendo64_bigendian.zip", "755df7f57edf87706d4c80ff15883312")]
        public void N64BigEndianHashMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var n64 = GetGameSystemByType(GameSystemType.Nintendo64);
            AddRomWithHash(n64, expectedHash);

            var matcher = RomMatcherFactory.Create(n64);
            Assert.NotNull(matcher);
            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }

        [Theory]
        [InlineData("../../../TestRoms/nintendo64_byteswapped.zip", "755df7f57edf87706d4c80ff15883312")]
        public void N64ByteSwappedHashMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var n64 = GetGameSystemByType(GameSystemType.Nintendo64);
            AddRomWithHash(n64, expectedHash);

            var matcher = RomMatcherFactory.Create(n64);
            Assert.NotNull(matcher);
            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }

        [Theory]
        [InlineData("../../../TestRoms/nds.zip", "e6d6d2daad4cc49483793ba298067065")]
        public void NdsHashMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.NintendoDS);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }

        [Theory]
        [InlineData("../../../TestRoms/nes_headered.zip", "29e5e1a5f8b400773ef9d959044456b2")]
        public void NesHeaderedHashMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.Nes);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);

        }
        [Theory]
        [InlineData("../../../TestRoms/ps1/007 Racing (USA).cue", "3620a316e7ce463e604d91540840df62")]
        public void Ps1BinCueMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.PlayStation);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);

        }

        [Theory]
        [InlineData("../../../TestRoms/ps1/007 Racing (USA).chd", "3620a316e7ce463e604d91540840df62")]
        public void Ps1ChdMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.PlayStation);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);

        }

        [Theory]
        [InlineData("../../../TestRoms/psp.iso", "3cd66bf66e631a8d90c97a2f7628bd2a")]
        public void PspIsoMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.Psp);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);

        }

        [Theory]
        [InlineData("../../../TestRoms/snes.zip", "c638c1175840c6640d897951daa73637")]
        public void SnesHeaderedMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.SuperNintendo);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);

        }

        [Theory]
        [InlineData("../../../TestRoms/3do/Alone in the Dark (USA).cue", "4d7f2e1b2e8b9d9d14f083fae44d9760")]
        public void TheeeDOBinCueMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.ThreeDo);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }

        [Theory]
        [InlineData("../../../TestRoms/dreamcast/18 Wheeler - American Pro Trucker (USA).cue", "aa571861964e9f58437f609239cbfb7c")]
        public void DreamcastBinCueMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.Dreamcast);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);

        }

        [Theory]
        [InlineData("../../../TestRoms/pcenginecd/Advanced V.G. (Japan).cue", "17f2a62623bf1091f286c92eda4ddf43")]
        public void PcEngineCDBinCueMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.PcEngineCD);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }

        [Theory]
        [InlineData("../../../TestRoms/pcfx/Battle Heat (Japan).cue", "7511f808929b912962e468daba4fe844")]
        public void PcFxBinCueMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.PcFx);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }

        [Theory]
        [InlineData("../../../TestRoms/neogeocd/ADK World (Japan).cue", "0fd821fdb242210f87e33f8ff921ef6d")]
        public void NeoGeoCDBinCueMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.NeoGeoCD);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }

        [Theory]
        [InlineData("../../../TestRoms/saturn/Alien Trilogy (USA)/Alien Trilogy (USA).cue", "3ea899f2b89ad6887071cfe178c0e559")]
        public void SegaSaturnBinCueMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.Saturn);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }

        [Theory]
        [InlineData("../../../TestRoms/saturn/Alien Trilogy (USA).chd", "3ea899f2b89ad6887071cfe178c0e559")]
        public void SegaSaturnChdMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.Saturn);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }

        [Theory]
        [InlineData("../../../TestRoms/segacd/Adventures of Willy Beamish, The (USA).cue", "88e929c3c9db840c2f784d16398651d8")]
        public void SegaCDBinCueMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.SegaCD);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }

        [Theory]
        [InlineData("../../../TestRoms/segacd/Adventures of Willy Beamish, The (USA).chd", "88e929c3c9db840c2f784d16398651d8")]
        public void SegaCDChdMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.SegaCD);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }

        [Theory]
        [InlineData("../../../TestRoms/ps2/007 - Agent Under Fire (USA).iso", "4c1df2b611348bc16ace85c4cbc626db")]
        public void Ps2IsoMatch(string filePath, string expectedHash)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            var sys = GetGameSystemByType(GameSystemType.PlayStation2);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }
    }
}
