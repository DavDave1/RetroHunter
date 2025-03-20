using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using RaSetMaker.Models;
using RaSetMaker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RaSetMaker.ViewModels
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

        public GameViewModel(MainViewModel mainVm, Game game, UserConfig userConfig) : base()
        {
            Game = game;
            _mainVm = mainVm;
            _userConfig = userConfig;
            var romViewModels = Game.Roms.Select(r => new RomViewModel(mainVm, r, userConfig)).OrderBy(r => r.Title).ToList();
            ValidRomsCount = romViewModels.Sum(rvm => rvm.IsRomValid ? 1 : 0);
            Roms = romViewModels;
            GameTypes = GameTypesToString();
            var iconSrc = ValidRomsCount == 0 ? "avares://RaSetMaker/Assets/error.png" : "avares://RaSetMaker/Assets/check.png";
            StatusIcon = ImageHelper.LoadFromResource(new Uri(iconSrc));
        }

        public async Task LoadDetails(CancellationTokenSource ct)
        {
            GameIcon = await ImageHelper.LoadFromWeb(new Uri(Game.IconUrl));

            if (ct.IsCancellationRequested)
            {
                return;
            }

            await _mainVm.UpdateGameData(Game);

            if (ct.IsCancellationRequested)
            {
                return;
            }

            Roms = [.. Game.Roms.Select(r => new RomViewModel(_mainVm, r, _userConfig))];
        }

        private string GameTypesToString() => string.Join("", Game.GameTypes.Select(t => $"[{t}]").Order());

        private MainViewModel _mainVm;

        private UserConfig _userConfig;
    }
}
