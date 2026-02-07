using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hpp.Application.DTOs;
using Hpp.Application.Interfaces;

namespace Hpp.Application.ViewModels;

/// <summary>
/// Lightweight view model for scenario preview.
/// </summary>
public sealed partial class ScenarioPreviewViewModel : ObservableObject
{
    private readonly IScenarioSimulator _simulator;

    public ScenarioPreviewViewModel(IScenarioSimulator simulator)
    {
        _simulator = simulator;
    }

    [ObservableProperty]
    private string _scenarioName = "Preview";

    [ObservableProperty]
    private decimal _adjustmentPercent = 5m;

    [ObservableProperty]
    private string _riskProfile = "Balanced";

    [ObservableProperty]
    private int _horizonMonths = 6;

    [ObservableProperty]
    private ScenarioResultDto? _lastResult;

    [RelayCommand]
    private async Task RunSimulationAsync(CancellationToken cancellationToken)
    {
        var request = ScenarioSimulationRequestDto.Default(ScenarioName, AdjustmentPercent, "IDR") with
        {
            RiskProfile = RiskProfile,
            HorizonMonths = HorizonMonths
        };

        LastResult = await _simulator.SimulateAsync(request, cancellationToken);
    }
}
