using Hpp.WinUI.Helpers;
using Hpp.WinUI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

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

        ConfigureWindowChrome();
        ConfigurePalette();
        ContentFrame.Navigate(typeof(DashboardView));
    }

    private void ConfigureWindowChrome()
    {
        try
        {
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(TitleBarDragRegion);
        }
        catch
        {
            // Keep default behavior if host environment does not support specific chrome settings.
        }
    }

    private void ConfigurePalette()
    {
        var commands = new[]
        {
            new PaletteCommandItem("dashboard", "Open Dashboard", "View KPI and top SKU overview"),
            new PaletteCommandItem("inventory", "Open Inventory", "Manage item catalog and exports"),
            new PaletteCommandItem("item", "Open Item Detail", "Inspect SKU lots and purchase traces"),
            new PaletteCommandItem("scenario", "Open Scenario Studio", "Run simulation and forecast scenarios"),
            new PaletteCommandItem("batch", "Open Batch Trace", "Review lot aging and stale inventory"),
            new PaletteCommandItem("settings", "Open Settings", "Tune themes and operational options")
        };

        CommandPalette.SetCommands(commands);
        CommandPalette.CommandInvoked += OnPaletteCommandInvoked;
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

    private void OnOpenPaletteClick(object sender, RoutedEventArgs e)
    {
        CommandPalette.Toggle();
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
