using Hpp.Infrastructure;
using Hpp.WinUI.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

namespace Hpp.WinUI;

public partial class App : Microsoft.UI.Xaml.Application
{
    private readonly IHost _host;

    public static IServiceProvider Services => ((App)Current)._host.Services;

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
        window.Activate();
    }
}
