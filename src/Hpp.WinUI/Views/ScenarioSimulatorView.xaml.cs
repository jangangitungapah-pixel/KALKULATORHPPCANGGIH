using Microsoft.UI.Xaml.Controls;

using Microsoft.Extensions.DependencyInjection;

namespace Hpp.WinUI.Views;

public sealed partial class ScenarioSimulatorView : Page
{
    public ScenarioSimulatorView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ViewModels.ScenarioSimulatorViewModel>();
    }
}
