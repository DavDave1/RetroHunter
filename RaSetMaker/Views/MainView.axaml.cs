using Avalonia.Controls;
using Avalonia.Interactivity;
using RaSetMaker.ViewModels;

namespace RaSetMaker.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    public void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.LoadModel();
        }
    }
}
