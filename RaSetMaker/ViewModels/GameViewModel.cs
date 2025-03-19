using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RaSetMaker.Models;
using RaSetMaker.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RaSetMaker.ViewModels
{
    public partial class GameViewModel : TreeViewItemModel
    {
        [ObservableProperty]
        private Bitmap? _gameIcon;

        public Game Game { get; private set; }

        public int ValidRomsCount { get; private set; }

        public override string IconSrc => ValidRomsCount == 0 ? "avares://RaSetMaker/Assets/error.png" : "avares://RaSetMaker/Assets/check.png";

        public List<RomViewModel> Roms => Children.Select(c => (RomViewModel)c).ToList();

        public GameViewModel(MainViewModel mainVm, Game game, UserConfig userConfig) : base(mainVm)
        {
            Game = game;
            _mainVm = mainVm;
            _userConfig = userConfig;
            var romViewModels = Game.Roms.Select(r => new RomViewModel(mainVm, r, userConfig)).OrderBy(r => r.Title).ToList();
            ValidRomsCount = romViewModels.Sum(rvm => rvm.IsRomValid ? 1 : 0);
            Children = [.. romViewModels.Select(r => (TreeViewItemModel)r)];
            Title = Game.GameTypes.Count == 0 ? Game.Name : $"{Game.Name} {GameTypesToString()}";
        }

        public async Task LoadDetails()
        {
            GameIcon = await ImageHelper.LoadFromWeb(new Uri(Game.IconUrl));
            await _mainVm.UpdateGameData(Game);
            Children = [.. Game.Roms.Select(r => new RomViewModel(_mainVm, r, _userConfig))];
        }

        private string GameTypesToString() => string.Join("", Game.GameTypes.Select(t => $"[{t}]").Order());

        private MainViewModel _mainVm;

        private UserConfig _userConfig;
    }
}
