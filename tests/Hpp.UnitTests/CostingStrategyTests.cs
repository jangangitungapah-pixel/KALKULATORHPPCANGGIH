using Hpp.Application.Services;
using Hpp.Domain.Entities;
using Hpp.Domain.ValueObjects;
using Xunit;

namespace Hpp.UnitTests;

public class CostingStrategyTests
{
    [Fact]
    public void Fifo_Should_Use_Oldest_Lots_First()
    {
        var item = new Item { StandardCost = new Money(0m, "IDR") };
        var lots = new List<InventoryLot>
        {
            new() { QuantityOnHand = new Quantity(10), UnitCost = new Money(100m, "IDR"), ReceivedAt = DateTimeOffset.UtcNow.AddDays(-2) },
            new() { QuantityOnHand = new Quantity(10), UnitCost = new Money(200m, "IDR"), ReceivedAt = DateTimeOffset.UtcNow.AddDays(-1) }
        };

        var engine = new FifoCostingEngine();
        var cost = engine.CalculateCost(item, lots, 5);

        Assert.Equal(500m, cost.Amount);
    }

    [Fact]
    public void Lifo_Should_Use_Latest_Lots_First()
    {
        var item = new Item { StandardCost = new Money(0m, "IDR") };
        var lots = new List<InventoryLot>
        {
            new() { QuantityOnHand = new Quantity(10), UnitCost = new Money(100m, "IDR"), ReceivedAt = DateTimeOffset.UtcNow.AddDays(-2) },
            new() { QuantityOnHand = new Quantity(10), UnitCost = new Money(200m, "IDR"), ReceivedAt = DateTimeOffset.UtcNow.AddDays(-1) }
        };

        var engine = new LifoCostingEngine();
        var cost = engine.CalculateCost(item, lots, 5);

        Assert.Equal(1000m, cost.Amount);
    }

    [Fact]
    public void WeightedAverage_Should_Calculate_Average()
    {
        var item = new Item { StandardCost = new Money(0m, "IDR") };
        var lots = new List<InventoryLot>
        {
            new() { QuantityOnHand = new Quantity(10), UnitCost = new Money(100m, "IDR"), ReceivedAt = DateTimeOffset.UtcNow.AddDays(-2) },
            new() { QuantityOnHand = new Quantity(10), UnitCost = new Money(200m, "IDR"), ReceivedAt = DateTimeOffset.UtcNow.AddDays(-1) }
        };

        var engine = new WeightedAverageCostingEngine();
        var cost = engine.CalculateCost(item, lots, 5);

        Assert.Equal(750m, cost.Amount);
    }
}
