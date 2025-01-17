using RaSetMaker.Models;

namespace RaSetMaker.Tests.MatchersTests
{
    public class MatcherTestBase
    {
        protected Ra2DatModel _model = new();

        protected MatcherTestBase()
        {
            _model.InitGameSystems();
        }

        protected GameSystem GetGameSystemByName(string name)
        {
            return _model.Systems.First(s => s.Name == name);
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
    }
}
