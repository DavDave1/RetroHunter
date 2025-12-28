
using CommunityToolkit.Mvvm.ComponentModel;
using RetroHunter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RetroHunter.ViewModels
{
    public partial class GameSystemViewModel : TreeViewItemModel
    {
        public GameSystem GameSystem { get; private set; }

        [ObservableProperty]
        private List<GameViewModel> _games = [];

        public override string IconSrc { get => GameSystem.GameSystemType.IconUrl(); }

        public GameSystemViewModel(MainViewModel mainVm, GameSystem gameSystem) : base()
        {
            _mainVm = mainVm;
            GameSystem = gameSystem;
            RefreshModel(gameSystem);
        }

        protected override void OnItemChecked(bool value)
        {
            GameSystem.IsChecked = value;
        }

        public void RefreshModel(GameSystem system)
        {
            GameSystem = system;
            IsChecked = GameSystem.IsChecked;

            Games = [.. GameSystem
                .GetGamesMatchingFilter()
                .OrderBy(g => g.Name)
                .Select(g => new GameViewModel(_mainVm, g))];

            Title = GameSystem.Games.Count == 0 ? GameSystem.Name : $"{GameSystem.Name} ({GetValidGamesCount()} / {Games.Count})";
        }

        private int GetValidGamesCount() => Games.Where(g => g.Game.HasValidRom()).Count();

        private readonly MainViewModel _mainVm;
    }
}
