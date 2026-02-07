using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Hpp.WinUI.Views;

public sealed partial class ItemDetailView : Page
{
    public ItemDetailView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ViewModels.ItemDetailViewModel>();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.ItemDetailViewModel viewModel)
        {
            await viewModel.LoadDefaultCommand.ExecuteAsync(null);
        }
    }
}
