using Hpp.Application.Interfaces;
using Hpp.Domain.Entities;
using Hpp.Domain.ValueObjects;

namespace Hpp.Application.Services;

/// <summary>
/// LIFO costing strategy.
/// </summary>
public sealed class LifoCostingEngine : ICostingEngine
{
    public string StrategyName => "LIFO";

    public Money CalculateCost(Item item, IReadOnlyList<InventoryLot> lots, decimal quantity)
    {
        var remaining = quantity;
        var total = Money.Zero(item.StandardCost.Currency);

        foreach (var lot in lots.OrderByDescending(l => l.ReceivedAt))
        {
            if (remaining <= 0)
            {
                break;
            }

            var take = Math.Min(remaining, lot.QuantityOnHand.Value);
            total = total.Add(new Money(take * lot.UnitCost.Amount, lot.UnitCost.Currency));
            remaining -= take;
        }

        return total;
    }
}
