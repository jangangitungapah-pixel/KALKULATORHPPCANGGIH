using Hpp.Application.DTOs;
using Hpp.Application.Interfaces;

namespace Hpp.Application.Services;

/// <summary>
/// Executes what-if simulations in memory.
/// </summary>
public sealed class ScenarioSimulatorService : IScenarioSimulator
{
    private readonly IInventoryRepository _repository;

    public ScenarioSimulatorService(IInventoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ScenarioResultDto> SimulateAsync(string scenarioName, decimal priceAdjustmentPercent, CancellationToken cancellationToken)
    {
        var items = await _repository.GetItemsAsync(cancellationToken);
        var averageCost = items.Count == 0
            ? 0m
            : items.Average(i => i.StandardCost.Amount);

        var adjusted = averageCost * (1 + priceAdjustmentPercent / 100m);
        return new ScenarioResultDto(scenarioName, adjusted, items.FirstOrDefault()?.StandardCost.Currency ?? "IDR",
            new List<string>
            {
                "Simulation executed in-memory.",
                "No persisted data was changed."
            });
    }
}
