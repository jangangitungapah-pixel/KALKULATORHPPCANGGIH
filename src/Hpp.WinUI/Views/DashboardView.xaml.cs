using Microsoft.UI.Xaml.Controls;

using Microsoft.Extensions.DependencyInjection;

namespace Hpp.WinUI.Views;

public sealed partial class DashboardView : Page
{
    public DashboardView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ViewModels.DashboardViewModel>();
    }
}
