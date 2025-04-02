
using RaSetMaker.Models;
using System.Collections.Generic;
using System.Linq;

namespace RaSetMaker.ViewModels;

public partial class GameSystemCompanyViewModel : TreeViewItemModel
{
    public GameSystemCompanyViewModel(MainViewModel mainVm, GameSystemCompany company, List<GameSystem> systems) : base(mainVm)
    {
        _company = company;
        Children = [.. systems.Select(s => new GameSystemViewModel(mainVm, s))];
        IsChecked = Children.Any(c => c.IsChecked);
        Title = company.ToString();
    }

    public void RefreshModel(List<GameSystem> systems)
    {
        for (int i = 0; i < Children.Count; i++)
        {
            ((GameSystemViewModel)Children[i]).RefreshModel(systems[i]);
        }
    }

    public GameSystemCompany Company => _company;

    private readonly GameSystemCompany _company;
}
