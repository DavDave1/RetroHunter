using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using RetroHunter.Models;
using RetroHunter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RetroHunter.ViewModels
{
    public partial class GameViewModel : ViewModelBase
    {
        [ObservableProperty]
        private Bitmap? _gameIcon;

        [ObservableProperty]
        private Bitmap? _statusIcon;

        [ObservableProperty]
        private string _gameTypes;

        [ObservableProperty]
        private List<RomViewModel> _roms;

        public Game Game { get; private set; }

        public int ValidRomsCount { get; private set; }

        public GameViewModel(MainViewModel mainVm, Game game) : base()
        {
            Game = game;
            _mainVm = mainVm;
            Roms = Game.Roms.Select(r => new RomViewModel(mainVm, this, r)).OrderBy(r => r.Title).ToList();
            GameTypes = GameTypesToString();
            UpdateStatus();
        }

        public async Task LoadDetails(CancellationTokenSource? ct)
        {
            GameIcon = await ImageHelper.LoadFromWeb(new Uri(Game.IconUrl));

            if (ct?.IsCancellationRequested ?? false)
            {
                return;
            }

            await _mainVm.UpdateGameData(Game);

            if (ct?.IsCancellationRequested ?? false)
            {
                return;
            }

            Roms = [.. Game.Roms.Select(r => new RomViewModel(_mainVm, this, r))];
        }

        public void UpdateStatus()
        {
            ValidRomsCount = Roms.Sum(rvm => rvm.IsRomValid ? 1 : 0);
            var iconSrc = ValidRomsCount == 0 ? "avares://RetroHunter/Assets/error.png" : "avares://RetroHunter/Assets/check.png";
            StatusIcon = ImageHelper.LoadFromResource(new Uri(iconSrc));
        }

        private string GameTypesToString() => string.Join("", Game.GameTypes.Select(t => $"[{t}]").Order());

        private MainViewModel _mainVm;
    }
}
