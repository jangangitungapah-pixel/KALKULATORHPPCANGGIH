using Microsoft.UI.Xaml.Controls;

namespace Hpp.WinUI.Views;

public sealed partial class ScenarioSimulatorView : Page
{
    public ScenarioSimulatorView(ViewModels.ScenarioSimulatorViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
