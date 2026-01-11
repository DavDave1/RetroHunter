using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RetroHunter.Models;
using System;
using System.Collections.ObjectModel;

namespace RetroHunter.ViewModels.Design;

public partial class DesignMainViewModel : MainViewModel
{
    public DesignMainViewModel()
    {
        var collection = new ServiceCollection();
        collection.AddLogging(builder => builder.AddConsole());
        _serviceProvier  = collection.BuildServiceProvider();

        _dbContext = new Persistence.DbContext();
        _settingsManager = new Services.SettingsManager(null);
        _raClient = new Services.RaClient(_settingsManager);

        var companyList = new ObservableCollection<GameSystemCompanyViewModel>();
        foreach (var company in Enum.GetValues<GameSystemCompany>())
        {
            companyList.Add(new(this, company, []));
        }
        CompanyList = companyList;
    }
}
