using Hpp.Domain.Entities;
using Hpp.Domain.ValueObjects;

namespace Hpp.Infrastructure.Data;

/// <summary>
/// Seed data for demo usage.
/// </summary>
public static class SeedData
{
    public static IReadOnlyList<Item> BuildItems()
    {
        return new List<Item>
        {
            new()
            {
                Sku = "SKU-001",
                Name = "Kopi Arabica",
                Category = "Bahan Baku",
                StandardCost = new Money(12000m, "IDR"),
                Lots = new List<InventoryLot>
                {
                    new() { QuantityOnHand = new Quantity(50), UnitCost = new Money(12000m, "IDR"), ReceivedAt = DateTimeOffset.UtcNow.AddDays(-10) },
                    new() { QuantityOnHand = new Quantity(40), UnitCost = new Money(13000m, "IDR"), ReceivedAt = DateTimeOffset.UtcNow.AddDays(-5) }
                }
            },
            new()
            {
                Sku = "SKU-002",
                Name = "Gula Aren",
                Category = "Bahan Baku",
                StandardCost = new Money(8000m, "IDR"),
                Lots = new List<InventoryLot>
                {
                    new() { QuantityOnHand = new Quantity(30), UnitCost = new Money(8000m, "IDR"), ReceivedAt = DateTimeOffset.UtcNow.AddDays(-7) }
                }
            }
        };
    }
}
