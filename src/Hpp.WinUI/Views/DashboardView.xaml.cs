using Microsoft.UI.Xaml.Controls;

namespace Hpp.WinUI.Views;

public sealed partial class DashboardView : Page
{
    public DashboardView(ViewModels.DashboardViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
