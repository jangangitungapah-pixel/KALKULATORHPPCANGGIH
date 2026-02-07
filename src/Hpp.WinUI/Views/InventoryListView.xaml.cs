using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Hpp.WinUI.Views;

public sealed partial class InventoryListView : Page
{
    public InventoryListView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ViewModels.InventoryListViewModel>();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.InventoryListViewModel viewModel)
        {
            await viewModel.LoadCommand.ExecuteAsync(null);
        }
    }

    private void OnFilterChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.InventoryListViewModel viewModel)
        {
            viewModel.ApplyFiltersCommand.Execute(null);
        }
    }

    private void OnFilterToggled(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.InventoryListViewModel viewModel)
        {
            viewModel.ApplyFiltersCommand.Execute(null);
        }
    }

    private void OnNumberFilterChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (DataContext is ViewModels.InventoryListViewModel viewModel)
        {
            viewModel.ApplyFiltersCommand.Execute(null);
        }
    }
}
