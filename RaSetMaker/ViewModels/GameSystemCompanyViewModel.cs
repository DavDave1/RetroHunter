
using RaSetMaker.Models;
using System.Collections.Generic;
using System.Linq;

namespace RaSetMaker.ViewModels
{
    public partial class GameSystemCompanyViewModel : TreeViewItemModel
    {
        public override string Title => _company.ToString();

        public GameSystemCompanyViewModel(MainViewModel mainVm, GameSystemCompany company, List<GameSystem> systems, UserConfig config) : base(mainVm)
        {
            _company = company;
            Children = [.. systems.Select(s => new GameSystemViewModel(mainVm, s, config))];
            IsChecked = Children.Any(c => c.IsChecked);
        }

        private readonly GameSystemCompany _company;
    }
}
