using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hpp.WinUI;

/// <summary>
/// WinUI 3 program entry point.
/// </summary>
public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        var app = new App(host);
        app.InitializeComponent();
        app.Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddHppServices(context.Configuration);
                services.AddSingleton<MainWindow>();
                services.AddSingleton<ViewModels.MainViewModel>();
                services.AddSingleton<ViewModels.DashboardViewModel>();
                services.AddSingleton<ViewModels.InventoryListViewModel>();
                services.AddSingleton<ViewModels.ItemDetailViewModel>();
                services.AddSingleton<ViewModels.ScenarioSimulatorViewModel>();
                services.AddSingleton<ViewModels.SettingsViewModel>();
            });
    }
}
