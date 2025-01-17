
using CommunityToolkit.Mvvm.ComponentModel;
using RaSetMaker.Models;
using System.Collections.Generic;
using System.Linq;

namespace RaSetMaker.ViewModels
{
    public partial class GameSystemViewModel : TreeViewItemModel
    {
        public override string Title => GameSystem.Games.Count == 0 ? GameSystem.Name : $"{GameSystem.Name} ({GetValidGamesCount()} / {Games.Count})";

        public override string ForegroundColor => GameSystem.RaId == 0 ? "DarkRed" : (GameSystem.Games.Count == 0 ? "Orange" : "Black");

        public GameSystem GameSystem => _gameSystem;

        public readonly List<GameViewModel> Games;

        public GameSystemViewModel(GameSystem gameSystem, UserConfig config)
        {
            _gameSystem = gameSystem;
            _config = config;
            IsChecked = gameSystem.IsChecked;
            Games = _gameSystem.GetGamesMatchingFilter(_config.GameTypesFilter).OrderBy(g => g.Name).Select(g => new GameViewModel(g)).ToList();
        }

        protected override void OnItemChecked(bool value)
        {
            GameSystem.IsChecked = value;
        }

        private int GetValidGamesCount() => Games.Where(g => g.Game.HasValidRom).Count();

        private readonly GameSystem _gameSystem;
        private readonly UserConfig _config;
    }
}
