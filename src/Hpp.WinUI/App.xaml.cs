using Hpp.Infrastructure;
using Hpp.WinUI.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Hpp.WinUI;

public partial class App : Microsoft.UI.Xaml.Application
{
    private readonly IHost _host;

    public static IServiceProvider Services => ((App)Current)._host.Services;
    public static MainWindow? MainAppWindow { get; private set; }
    public static bool IsDarkThemeEnabled { get; private set; } = true;
    public static string CurrentAccent { get; private set; } = "Blue";

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddHppServices(context.Configuration);
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<DashboardViewModel>();
                services.AddSingleton<InventoryListViewModel>();
                services.AddSingleton<ItemDetailViewModel>();
                services.AddSingleton<BatchTraceViewModel>();
                services.AddSingleton<ScenarioSimulatorViewModel>();
                services.AddSingleton<SettingsViewModel>();
            })
            .Build();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var window = new MainWindow();
        MainAppWindow = window;
        ApplyAppearance(IsDarkThemeEnabled, CurrentAccent);
        window.Activate();
    }

    public static void ApplyAppearance(bool isDarkTheme, string accentName)
    {
        IsDarkThemeEnabled = isDarkTheme;
        CurrentAccent = string.IsNullOrWhiteSpace(accentName) ? "Blue" : accentName;

        var app = (App)Current;
        app.ApplyThemeToWindow();
        app.ApplyPalette();
        app.ApplyTitleBarPalette();
    }

    private void ApplyThemeToWindow()
    {
        if (MainAppWindow?.Content is FrameworkElement root)
        {
            root.RequestedTheme = IsDarkThemeEnabled ? ElementTheme.Dark : ElementTheme.Light;
        }
    }

    private void ApplyPalette()
    {
        if (IsDarkThemeEnabled)
        {
            var accent = CurrentAccent switch
            {
                "Green" => Hex("#4ADE80"),
                "Orange" => Hex("#FBBF24"),
                "Red" => Hex("#F87171"),
                _ => Hex("#38BDF8")
            };

            SetBrushColor("BrandHighlightBrush", accent);
            SetBrushColor("BrandAccentBrush", accent);
            SetBrushColor("BrandWarningBrush", Hex("#F59E0B"));
            SetBrushColor("BrandPanelBrush", Hex("#151F34"));
            SetBrushColor("BrandPanelOverlayBrush", Hex("#1F2D49"));
            SetBrushColor("BrandTitleBarBrush", Hex("#111C30"));
            SetBrushColor("BrandCardBackgroundBrush", Hex("#2A1F2D49"));
            SetBrushColor("BrandCardBorderBrush", Hex("#334A6D9B"));
            SetBrushColor("BrandHeroSurfaceBrush", Hex("#2A0F243E"));
            SetBrushColor("BrandRowSurfaceBrush", Hex("#2A243B5E"));
            SetBrushColor("BrandPillBackgroundBrush", Hex("#334A6D9B"));
            SetBrushColor("BrandPillBorderBrush", Hex("#556EA8D8"));
            SetBrushColor("AppTextPrimaryBrush", Hex("#F8FAFC"));
            SetBrushColor("AppTextSecondaryBrush", Hex("#CBD5E1"));
            SetBrushColor("AppInputBackgroundBrush", Hex("#1E293B"));
            SetBrushColor("AppInputForegroundBrush", Hex("#F8FAFC"));
            SetBrushColor("BrandButtonTextBrush", Hex("#06121F"));
            SetBrushColor("CommandPaletteBackgroundBrush", Hex("#EE0E172A"));
            SetBrushColor("CommandPaletteBorderBrush", Hex("#446EA8D8"));
            SetGradient("BrandCanvasBrush", Hex("#0B1220"), Hex("#101A2E"), Hex("#1D3456"));
        }
        else
        {
            var accent = CurrentAccent switch
            {
                "Green" => Hex("#166534"),
                "Orange" => Hex("#B45309"),
                "Red" => Hex("#B91C1C"),
                _ => Hex("#0369A1")
            };

            SetBrushColor("BrandHighlightBrush", accent);
            SetBrushColor("BrandAccentBrush", accent);
            SetBrushColor("BrandWarningBrush", Hex("#B45309"));
            SetBrushColor("BrandPanelBrush", Hex("#FFFFFF"));
            SetBrushColor("BrandPanelOverlayBrush", Hex("#F4F7FC"));
            SetBrushColor("BrandTitleBarBrush", Hex("#EAF1FB"));
            SetBrushColor("BrandCardBackgroundBrush", Hex("#FFFFFF"));
            SetBrushColor("BrandCardBorderBrush", Hex("#C5D4E7"));
            SetBrushColor("BrandHeroSurfaceBrush", Hex("#E7EEF9"));
            SetBrushColor("BrandRowSurfaceBrush", Hex("#EEF3FB"));
            SetBrushColor("BrandPillBackgroundBrush", Hex("#DFE9F9"));
            SetBrushColor("BrandPillBorderBrush", Hex("#AEC3E2"));
            SetBrushColor("AppTextPrimaryBrush", Hex("#0F172A"));
            SetBrushColor("AppTextSecondaryBrush", Hex("#334155"));
            SetBrushColor("AppInputBackgroundBrush", Hex("#FFFFFF"));
            SetBrushColor("AppInputForegroundBrush", Hex("#0F172A"));
            SetBrushColor("BrandButtonTextBrush", Colors.White);
            SetBrushColor("CommandPaletteBackgroundBrush", Hex("#F7FBFF"));
            SetBrushColor("CommandPaletteBorderBrush", Hex("#B8CCE8"));
            SetGradient("BrandCanvasBrush", Hex("#F8FAFF"), Hex("#EEF4FE"), Hex("#E3EDFC"));
        }
    }

    private void ApplyTitleBarPalette()
    {
        var titleBar = MainAppWindow?.AppWindow?.TitleBar;
        if (titleBar is null)
        {
            return;
        }

        if (IsDarkThemeEnabled)
        {
            titleBar.BackgroundColor = Hex("#111C30");
            titleBar.ForegroundColor = Hex("#F8FAFC");
            titleBar.InactiveBackgroundColor = Hex("#111C30");
            titleBar.InactiveForegroundColor = Hex("#94A3B8");
            titleBar.ButtonBackgroundColor = Hex("#111C30");
            titleBar.ButtonForegroundColor = Hex("#F8FAFC");
            titleBar.ButtonHoverBackgroundColor = Hex("#1E2A42");
            titleBar.ButtonHoverForegroundColor = Hex("#FFFFFF");
            titleBar.ButtonPressedBackgroundColor = Hex("#273855");
            titleBar.ButtonPressedForegroundColor = Hex("#FFFFFF");
            titleBar.ButtonInactiveBackgroundColor = Hex("#111C30");
            titleBar.ButtonInactiveForegroundColor = Hex("#94A3B8");
            return;
        }

        titleBar.BackgroundColor = Hex("#EAF1FB");
        titleBar.ForegroundColor = Hex("#0F172A");
        titleBar.InactiveBackgroundColor = Hex("#EAF1FB");
        titleBar.InactiveForegroundColor = Hex("#475569");
        titleBar.ButtonBackgroundColor = Hex("#EAF1FB");
        titleBar.ButtonForegroundColor = Hex("#0F172A");
        titleBar.ButtonHoverBackgroundColor = Hex("#D7E3F6");
        titleBar.ButtonHoverForegroundColor = Hex("#020617");
        titleBar.ButtonPressedBackgroundColor = Hex("#C9DBF2");
        titleBar.ButtonPressedForegroundColor = Hex("#020617");
        titleBar.ButtonInactiveBackgroundColor = Hex("#EAF1FB");
        titleBar.ButtonInactiveForegroundColor = Hex("#64748B");
    }

    private static void SetBrushColor(string key, Color color)
    {
        if (Current.Resources.ContainsKey(key) && Current.Resources[key] is SolidColorBrush brush)
        {
            brush.Color = color;
        }
    }

    private static void SetGradient(string key, Color start, Color middle, Color end)
    {
        if (Current.Resources.ContainsKey(key) && Current.Resources[key] is LinearGradientBrush gradient)
        {
            if (gradient.GradientStops.Count >= 3)
            {
                gradient.GradientStops[0].Color = start;
                gradient.GradientStops[1].Color = middle;
                gradient.GradientStops[2].Color = end;
            }
        }
    }

    private static Color Hex(string value)
    {
        var hex = value.TrimStart('#');
        if (hex.Length == 6)
        {
            return ColorHelper.FromArgb(255,
                Convert.ToByte(hex.Substring(0, 2), 16),
                Convert.ToByte(hex.Substring(2, 2), 16),
                Convert.ToByte(hex.Substring(4, 2), 16));
        }

        if (hex.Length == 8)
        {
            return ColorHelper.FromArgb(
                Convert.ToByte(hex.Substring(0, 2), 16),
                Convert.ToByte(hex.Substring(2, 2), 16),
                Convert.ToByte(hex.Substring(4, 2), 16),
                Convert.ToByte(hex.Substring(6, 2), 16));
        }

        return Colors.White;
    }
}
