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
        _strategies = strategies
            .GroupBy(strategy => NormalizeStrategyKey(strategy.StrategyName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyList<string> AvailableStrategies => _strategies.Values
        .Select(strategy => strategy.StrategyName)
        .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
        .ToArray();

    public Money Calculate(Item item, IReadOnlyList<InventoryLot> lots, decimal quantity, string strategy)
    {
        if (quantity <= 0m)
        {
            throw new InvalidOperationException("Quantity must be greater than zero.");
        }

        var normalizedStrategy = NormalizeStrategyKey(strategy);
        if (!_strategies.TryGetValue(normalizedStrategy, out var engine))
        {
            throw new InvalidOperationException(
                $"Unknown costing strategy: {strategy}. Available: {string.Join(", ", AvailableStrategies)}");
        }

        return engine.CalculateCost(item, lots, quantity);
    }

    private static string NormalizeStrategyKey(string? strategy)
    {
        if (string.IsNullOrWhiteSpace(strategy))
        {
            return "weightedaverage";
        }

        return new string(strategy
            .Trim()
            .Where(char.IsLetterOrDigit)
            .ToArray())
            .ToLowerInvariant();
    }
}
