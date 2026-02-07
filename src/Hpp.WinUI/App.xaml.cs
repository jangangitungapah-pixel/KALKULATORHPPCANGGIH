using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

namespace Hpp.WinUI;

public partial class App : Application
{
    private readonly IHost _host;

    public App(IHost host)
    {
        _host = host;
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var window = _host.Services.GetService(typeof(MainWindow)) as MainWindow;
        window?.Activate();
    }
}
