
using CommunityToolkit.Mvvm.ComponentModel;
using RaSetMaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RaSetMaker.ViewModels
{
    public partial class GameSystemViewModel : TreeViewItemModel
    {
        public GameSystem GameSystem => _gameSystem;

        [ObservableProperty]
        private List<GameViewModel> _games = [];

        public GameSystemViewModel(MainViewModel mainVm, GameSystem gameSystem) : base(mainVm)
        {
            _mainVm = mainVm;
            _gameSystem = gameSystem;
            RefreshModel(gameSystem);
        }

        protected override void OnItemChecked(bool value)
        {
            GameSystem.IsChecked = value;
        }

        public void RefreshModel(GameSystem system)
        {
            _gameSystem = system;
            IsChecked = _gameSystem.IsChecked;

            Games = [.. _gameSystem
                .GetGamesMatchingFilter()
                .OrderBy(g => g.Name)
                .Select(g => new GameViewModel(_mainVm, g))];

            Title = GameSystem.Games.Count == 0 ? GameSystem.Name : $"{GameSystem.Name} ({GetValidGamesCount()} / {Games.Count})";
        }

        private int GetValidGamesCount() => Games.Where(g => g.Game.HasValidRom()).Count();

        private GameSystem _gameSystem;

        private readonly MainViewModel _mainVm;
    }
}
