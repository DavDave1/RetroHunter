using RaSetMaker.Models;
using RaSetMaker.Utils.Matchers;
using System;
using System.Collections.Generic;

namespace RaSetMaker.Utils
{
    public class RomMatcherFactory
    {
        static RomMatcherFactory() {
            _gameIdentificators = new()
            {
                { RomMatcherType.Null, typeof(NullMatcher) },
                { RomMatcherType.Md5, typeof(Md5Matcher) },
                { RomMatcherType.FileNameHash, typeof(FileNameHashMatcher) },
                { RomMatcherType.ThreeDO, typeof(NullMatcher) },
                { RomMatcherType.Arduboy, typeof(ArduboyMatcher) },
                { RomMatcherType.Atari7800, typeof(Atari7800Matcher) },
                { RomMatcherType.AtariJaguarCD, typeof(NullMatcher) },
                { RomMatcherType.AtariLynx, typeof(LynxMatcher) },
                { RomMatcherType.PcEngine, typeof(PcEngineMatcher) },
                { RomMatcherType.PcEngineCD, typeof(NullMatcher) },
                { RomMatcherType.PcFx, typeof(NullMatcher) },
                { RomMatcherType.GameCube, typeof(NullMatcher) },
                { RomMatcherType.Nintendo64, typeof(N64Matcher) },
                { RomMatcherType.NintendoDS, typeof(NdsMatcher) },
                { RomMatcherType.Nes, typeof(NesMatcher) },
                { RomMatcherType.Snes, typeof(SnesMatcher) },
                { RomMatcherType.NeoGeoCD, typeof(NullMatcher) },
                { RomMatcherType.Dreamcast, typeof(NullMatcher) },
                { RomMatcherType.Saturn, typeof(NullMatcher) },
                { RomMatcherType.SegaCD, typeof(NullMatcher) },
                { RomMatcherType.Ps1, typeof(NullMatcher) },
                { RomMatcherType.Ps2, typeof(NullMatcher) },
                { RomMatcherType.Psp, typeof(NullMatcher) },
            };
        }

        public static MatcherBase Create(GameSystem system)
        {
            return (MatcherBase)Activator.CreateInstance(_gameIdentificators[system.MatcherType], system)!;
        }


        private static readonly Dictionary<RomMatcherType, Type> _gameIdentificators;
    }
}
