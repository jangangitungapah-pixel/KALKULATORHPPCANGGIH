using Hpp.Application.Interfaces;
using Hpp.Domain.Entities;
using Hpp.Domain.ValueObjects;

namespace Hpp.Application.Services;

/// <summary>
/// Weighted average costing strategy.
/// </summary>
public sealed class WeightedAverageCostingEngine : ICostingEngine
{
    public string StrategyName => "Weighted Average";

    public Money CalculateCost(Item item, IReadOnlyList<InventoryLot> lots, decimal quantity)
    {
        if (quantity <= 0m)
        {
            return Money.Zero(item.StandardCost.Currency);
        }

        if (lots.Count == 0)
        {
            return item.StandardCost.Multiply(quantity).Round(2);
        }

        var usableLots = lots.Where(lot => lot.QuantityOnHand.Value > 0m).ToList();
        var totalQuantity = usableLots.Sum(l => l.QuantityOnHand.Value);
        if (totalQuantity <= 0)
        {
            return item.StandardCost.Multiply(quantity).Round(2);
        }

        var totalCost = usableLots.Sum(l => l.QuantityOnHand.Value * l.UnitCost.Amount);
        var avgCost = totalCost / totalQuantity;
        return new Money(avgCost * quantity, item.StandardCost.Currency).Round(2);
    }
}
