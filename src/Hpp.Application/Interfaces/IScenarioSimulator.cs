using Hpp.Application.DTOs;

namespace Hpp.Application.Interfaces;

/// <summary>
/// Runs what-if simulations without persistence.
/// </summary>
public interface IScenarioSimulator
{
    Task<ScenarioResultDto> SimulateAsync(ScenarioSimulationRequestDto request, CancellationToken cancellationToken);

    Task<ScenarioResultDto> SimulateAsync(string scenarioName, decimal priceAdjustmentPercent, CancellationToken cancellationToken);
}
