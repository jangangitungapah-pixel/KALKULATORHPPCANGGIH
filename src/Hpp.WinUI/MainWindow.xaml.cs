using Hpp.WinUI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Graphics;

namespace Hpp.WinUI;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        RootGrid.DataContext = App.Services.GetRequiredService<ViewModels.MainViewModel>();
        ContentFrame.Navigate(typeof(DashboardView));
        TryResizeWindow();
    }

    private void TryResizeWindow()
    {
        try
        {
            AppWindow.Resize(new SizeInt32(1200, 800));
        }
        catch
        {
            // TODO: Handle window sizing for different windowing environments if needed.
        }
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
