using RaSetMaker.Models;
using System.Linq;

namespace RaSetMaker.ViewModels
{
    public partial class GameViewModel : TreeViewItemModel
    {
        public override string Title => Game.GameTypes.Count == 0 ? Game.Name : $"{Game.Name} {GameTypesToString()}";

        public override string ForegroundColor => ValidRomsCount == 0 ? "DarkRed" : "Black"; 

        public Game Game { get; private set; }

        public int ValidRomsCount { get; private set; }

        public GameViewModel(Game game)
        {
            Game = game;
            var romViewModels = Game.Roms.Select(r => new RomViewModel(r)).ToList();
            ValidRomsCount = romViewModels.Sum(rvm => rvm.IsRomValid() ? 1 : 0);
            Children = romViewModels.Select(r => (TreeViewItemModel)r).ToList();
        }

        private string GameTypesToString() => string.Join("", Game.GameTypes.Select(t => $"[{t}]").Order());
    }
}
