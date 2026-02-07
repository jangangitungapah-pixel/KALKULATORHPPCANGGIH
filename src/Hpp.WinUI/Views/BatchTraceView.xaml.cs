using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Hpp.WinUI.Views;

public sealed partial class BatchTraceView : Page
{
    public BatchTraceView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ViewModels.BatchTraceViewModel>();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.BatchTraceViewModel viewModel)
        {
            await viewModel.LoadCommand.ExecuteAsync(null);
        }
    }
}
