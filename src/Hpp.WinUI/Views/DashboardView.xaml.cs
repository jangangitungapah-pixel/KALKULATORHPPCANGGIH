using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Hpp.WinUI.Views;

public sealed partial class DashboardView : Page
{
    public DashboardView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ViewModels.DashboardViewModel>();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.DashboardViewModel viewModel)
        {
            await viewModel.LoadAsync(CancellationToken.None);
        }
    }
}
