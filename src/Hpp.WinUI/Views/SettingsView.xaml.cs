using Microsoft.UI.Xaml.Controls;

namespace Hpp.WinUI.Views;

public sealed partial class SettingsView : Page
{
    public SettingsView(ViewModels.SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
