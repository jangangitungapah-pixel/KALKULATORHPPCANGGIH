using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Hpp.WinUI.Views;

public sealed partial class ScenarioSimulatorView : Page
{
    public ScenarioSimulatorView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ViewModels.ScenarioSimulatorViewModel>();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.ScenarioSimulatorViewModel viewModel)
        {
            await viewModel.RunSimulationCommand.ExecuteAsync(null);
        }
    }

    private void OnSelectionChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.ScenarioSimulatorViewModel viewModel)
        {
            viewModel.RecomputeSummaryCommand.Execute(null);
            viewModel.GenerateForecastCommand.Execute(null);
        }
    }

    private void OnToggleChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.ScenarioSimulatorViewModel viewModel)
        {
            viewModel.RecomputeSummaryCommand.Execute(null);
        }
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is ViewModels.ScenarioSimulatorViewModel viewModel)
        {
            viewModel.RecomputeSummaryCommand.Execute(null);
        }
    }

    private void OnNumberChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (DataContext is ViewModels.ScenarioSimulatorViewModel viewModel)
        {
            viewModel.RecomputeSummaryCommand.Execute(null);
        }
    }

    private void OnDriverChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is ViewModels.ScenarioSimulatorViewModel viewModel)
        {
            viewModel.RecomputeSummaryCommand.Execute(null);
        }
    }

    private void OnDriverNumberChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (DataContext is ViewModels.ScenarioSimulatorViewModel viewModel)
        {
            viewModel.RecomputeSummaryCommand.Execute(null);
        }
    }

    private void OnDriverSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is ViewModels.ScenarioSimulatorViewModel viewModel)
        {
            viewModel.RecomputeSummaryCommand.Execute(null);
        }
    }

    private void OnAssumptionSelected(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is ViewModels.ScenarioSimulatorViewModel viewModel)
        {
            viewModel.StatusMessage = "Assumption selected.";
        }
    }
}
