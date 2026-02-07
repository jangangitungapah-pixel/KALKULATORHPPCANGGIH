using Hpp.Application.Interfaces;
using Hpp.Domain.Entities;
using Hpp.Domain.ValueObjects;

namespace Hpp.Application.Services;

/// <summary>
/// FIFO costing strategy.
/// </summary>
public sealed class FifoCostingEngine : ICostingEngine
{
    public string StrategyName => "FIFO";

    public Money CalculateCost(Item item, IReadOnlyList<InventoryLot> lots, decimal quantity)
    {
        if (quantity <= 0m)
        {
            return Money.Zero(item.StandardCost.Currency);
        }

        var remaining = quantity;
        var total = Money.Zero(item.StandardCost.Currency);

        foreach (var lot in lots
                     .Where(lot => lot.QuantityOnHand.Value > 0m)
                     .OrderBy(l => l.ReceivedAt))
        {
            if (remaining <= 0)
            {
                break;
            }

            var take = Math.Min(remaining, lot.QuantityOnHand.Value);
            total = total.Add(new Money(take * lot.UnitCost.Amount, lot.UnitCost.Currency));
            remaining -= take;
        }

        // If lots are insufficient, fall back to standard cost for missing quantity.
        if (remaining > 0m)
        {
            total = total.Add(item.StandardCost.Multiply(remaining));
        }

        return total.Round(2);
    }
}
