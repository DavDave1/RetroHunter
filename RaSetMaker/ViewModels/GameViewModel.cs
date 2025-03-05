using RaSetMaker.Models;
using System.Linq;
using System.Xml.Serialization;

namespace RaSetMaker.ViewModels
{
    public partial class GameViewModel : TreeViewItemModel
    {
        public override string Title => Game.GameTypes.Count == 0 ? Game.Name : $"{Game.Name} {GameTypesToString()}";

        public Game Game { get; private set; }

        public int ValidRomsCount { get; private set; }

        public override string IconSrc => ValidRomsCount == 0 ? "avares://RaSetMaker/Assets/error.png" : "avares://RaSetMaker/Assets/check.png";

        public GameViewModel(MainViewModel mainVm, Game game, UserConfig userConfig) : base(mainVm)
        {
            Game = game;
            var romViewModels = Game.Roms.Select(r => new RomViewModel(mainVm, r, userConfig)).OrderBy(r => r.Title).ToList();
            ValidRomsCount = romViewModels.Sum(rvm => rvm.IsRomValid() ? 1 : 0);
            Children = [.. romViewModels.Select(r => (TreeViewItemModel)r)];
        }


        private string GameTypesToString() => string.Join("", Game.GameTypes.Select(t => $"[{t}]").Order());
    }
}
