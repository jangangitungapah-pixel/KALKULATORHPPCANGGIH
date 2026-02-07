using Hpp.Application.Interfaces;
using Hpp.Domain.Entities;
using Hpp.Domain.ValueObjects;

namespace Hpp.Application.Services;

/// <summary>
/// Coordinates costing strategy selection.
/// </summary>
public sealed class CostingService
{
    private readonly IReadOnlyDictionary<string, ICostingEngine> _strategies;

    public CostingService(IEnumerable<ICostingEngine> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.StrategyName, StringComparer.OrdinalIgnoreCase);
    }

    public Money Calculate(Item item, IReadOnlyList<InventoryLot> lots, decimal quantity, string strategy)
    {
        if (!_strategies.TryGetValue(strategy, out var engine))
        {
            throw new InvalidOperationException($"Unknown costing strategy: {strategy}");
        }

        return engine.CalculateCost(item, lots, quantity);
    }
}
