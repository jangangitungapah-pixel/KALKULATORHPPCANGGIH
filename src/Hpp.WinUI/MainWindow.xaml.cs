using Hpp.WinUI.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Hpp.WinUI;

public sealed partial class MainWindow : Window
{
    public MainWindow(ViewModels.MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        ContentFrame.Navigate(typeof(DashboardView));
    }

    private void OnNavigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem item && item.Tag is string tag)
        {
            var viewType = tag switch
            {
                "Dashboard" => typeof(DashboardView),
                "Inventory" => typeof(InventoryListView),
                "Scenario" => typeof(ScenarioSimulatorView),
                "Settings" => typeof(SettingsView),
                _ => typeof(DashboardView)
            };

            ContentFrame.Navigate(viewType);
        }
    }

    private void OnCommandPaletteInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        CommandPalette.Toggle();
        args.Handled = true;
    }
}
