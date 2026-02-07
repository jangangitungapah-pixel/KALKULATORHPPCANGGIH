using Hpp.WinUI.Helpers;
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
    private readonly IReadOnlyDictionary<string, Type> _navigationMap;

    public MainWindow()
    {
        InitializeComponent();
        RootGrid.DataContext = App.Services.GetRequiredService<ViewModels.MainViewModel>();

        _navigationMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            ["Dashboard"] = typeof(DashboardView),
            ["Inventory"] = typeof(InventoryListView),
            ["ItemDetail"] = typeof(ItemDetailView),
            ["Scenario"] = typeof(ScenarioSimulatorView),
            ["BatchTrace"] = typeof(BatchTraceView),
            ["Settings"] = typeof(SettingsView)
        };

        ContentFrame.Navigate(typeof(DashboardView));
        ConfigurePalette();
        TryResizeWindow();
    }

    private void ConfigurePalette()
    {
        var commands = new[]
        {
            new PaletteCommandItem("dashboard", "Open Dashboard", "View KPI and top SKU overview"),
            new PaletteCommandItem("inventory", "Open Inventory", "Manage item catalog and exports"),
            new PaletteCommandItem("item", "Open Item Detail", "Inspect one SKU and lot history"),
            new PaletteCommandItem("scenario", "Open Scenario Simulator", "Run what-if costing simulation"),
            new PaletteCommandItem("batch", "Open Batch Trace", "Review lot aging and traceability"),
            new PaletteCommandItem("settings", "Open Settings", "Adjust app-level options")
        };

        CommandPalette.SetCommands(commands);
        CommandPalette.CommandInvoked += OnPaletteCommandInvoked;
    }

    private void TryResizeWindow()
    {
        try
        {
            AppWindow.Resize(new SizeInt32(1280, 820));
        }
        catch
        {
            // Keep default size when host windowing does not permit direct resize.
        }
    }

    private void OnNavigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem item && item.Tag is string tag)
        {
            NavigateToTag(tag);
        }
    }

    private void OnCommandPaletteInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        CommandPalette.Toggle();
        args.Handled = true;
    }

    private void OnPaletteCommandInvoked(object? sender, PaletteCommandItem command)
    {
        var tag = command.Key switch
        {
            "dashboard" => "Dashboard",
            "inventory" => "Inventory",
            "item" => "ItemDetail",
            "scenario" => "Scenario",
            "batch" => "BatchTrace",
            "settings" => "Settings",
            _ => "Dashboard"
        };

        NavigateToTag(tag);
        SelectNavigationTag(tag);
    }

    private void NavigateToTag(string tag)
    {
        if (!_navigationMap.TryGetValue(tag, out var viewType))
        {
            viewType = typeof(DashboardView);
        }

        if (ContentFrame.CurrentSourcePageType != viewType)
        {
            ContentFrame.Navigate(viewType);
        }
    }

    private void SelectNavigationTag(string tag)
    {
        foreach (var menuItem in ShellNav.MenuItems.OfType<NavigationViewItem>())
        {
            if (menuItem.Tag is string itemTag && string.Equals(itemTag, tag, StringComparison.OrdinalIgnoreCase))
            {
                ShellNav.SelectedItem = menuItem;
                return;
            }
        }
    }
}
