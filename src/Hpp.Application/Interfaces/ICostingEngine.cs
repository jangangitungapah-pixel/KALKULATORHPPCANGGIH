using Hpp.Domain.Entities;
using Hpp.Domain.ValueObjects;

namespace Hpp.Application.Interfaces;

/// <summary>
/// Runs HPP costing calculations.
/// </summary>
public interface ICostingEngine
{
    string StrategyName { get; }
    Money CalculateCost(Item item, IReadOnlyList<InventoryLot> lots, decimal quantity);
}
