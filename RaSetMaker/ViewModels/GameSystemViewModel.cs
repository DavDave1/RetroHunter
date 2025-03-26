
using CommunityToolkit.Mvvm.ComponentModel;
using RaSetMaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RaSetMaker.ViewModels
{
    public partial class GameSystemViewModel : TreeViewItemModel
    {

        public override string IconSrc => GameSystem.Games.Count == 0 ? "avares://RaSetMaker/Assets/system-warn.png" : "avares://RaSetMaker/Assets/system.png";

        public GameSystem GameSystem => _gameSystem;

        public readonly List<GameViewModel> Games;

        public GameSystemViewModel(MainViewModel mainVm, GameSystem gameSystem) : base(mainVm)
        {
            _gameSystem = gameSystem;
            IsChecked = gameSystem.IsChecked;

            Games = [.. _gameSystem
                .GetGamesMatchingFilter()
                .OrderBy(g => g.Name)
                .Select(g => new GameViewModel(mainVm, g))];

            Title = GameSystem.Games.Count == 0 ? GameSystem.Name : $"{GameSystem.Name} ({GetValidGamesCount()} / {Games.Count})";
        }

        protected override void OnItemChecked(bool value)
        {
            GameSystem.IsChecked = value;
        }

        private int GetValidGamesCount() => Games.Where(g => g.Game.HasValidRom()).Count();

        private readonly GameSystem _gameSystem;
    }
}
