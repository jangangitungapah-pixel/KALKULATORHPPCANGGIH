using Hpp.Application.Interfaces;
using Hpp.Domain.Entities;
using Hpp.Domain.ValueObjects;

namespace Hpp.Application.Services;

/// <summary>
/// Weighted average costing strategy.
/// </summary>
public sealed class WeightedAverageCostingEngine : ICostingEngine
{
    public string StrategyName => "WeightedAverage";

    public Money CalculateCost(Item item, IReadOnlyList<InventoryLot> lots, decimal quantity)
    {
        if (lots.Count == 0)
        {
            return Money.Zero(item.StandardCost.Currency);
        }

        var totalQuantity = lots.Sum(l => l.QuantityOnHand.Value);
        if (totalQuantity <= 0)
        {
            return Money.Zero(item.StandardCost.Currency);
        }

        var totalCost = lots.Sum(l => l.QuantityOnHand.Value * l.UnitCost.Amount);
        var avgCost = totalCost / totalQuantity;
        return new Money(avgCost * quantity, item.StandardCost.Currency);
    }
}
