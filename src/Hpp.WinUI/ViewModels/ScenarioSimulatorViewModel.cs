using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hpp.Application.Interfaces;

namespace Hpp.WinUI.ViewModels;

public sealed partial class ScenarioSimulatorViewModel : ObservableObject
{
    private readonly IScenarioSimulator _simulator;

    public ScenarioSimulatorViewModel(IScenarioSimulator simulator)
    {
        _simulator = simulator;
    }

    [ObservableProperty]
    private string _scenarioName = "Default";

    [ObservableProperty]
    private string _adjustmentPercent = "5";

    [ObservableProperty]
    private string _resultText = "Ready.";

    [RelayCommand]
    private async Task RunSimulationAsync(CancellationToken cancellationToken)
    {
        if (!decimal.TryParse(AdjustmentPercent, out var percent))
        {
            ResultText = "Invalid adjustment percent.";
            return;
        }

        var result = await _simulator.SimulateAsync(ScenarioName, percent, cancellationToken);
        ResultText = $"{result.ScenarioName}: {result.SimulatedCost} {result.Currency}.";
    }
}
