using Microsoft.UI.Xaml.Controls;

using Microsoft.Extensions.DependencyInjection;

namespace Hpp.WinUI.Views;

public sealed partial class SettingsView : Page
{
    public SettingsView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ViewModels.SettingsViewModel>();
    }
}
