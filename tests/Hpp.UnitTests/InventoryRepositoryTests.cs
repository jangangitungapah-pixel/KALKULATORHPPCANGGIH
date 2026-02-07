using Hpp.Domain.Entities;
using Hpp.Domain.ValueObjects;
using Hpp.Infrastructure;
using Hpp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Hpp.UnitTests;

public class InventoryRepositoryTests
{
    [Fact]
    public async Task AddItemAsync_Persists_Item()
    {
        await using var dbContext = CreateDbContext();
        var repository = new EfInventoryRepository(dbContext);

        var item = new Item { Sku = "SKU-TEST", Name = "Test", StandardCost = new Money(100m, "IDR") };
        await repository.AddItemAsync(item, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        var loaded = await repository.GetItemAsync(item.Id, CancellationToken.None);
        Assert.NotNull(loaded);
        Assert.Equal("SKU-TEST", loaded!.Sku);
    }

    [Fact]
    public async Task SkuExistsAsync_Respects_Exclusion()
    {
        await using var dbContext = CreateDbContext();
        var repository = new EfInventoryRepository(dbContext);
        var item = new Item { Sku = "SKU-01", Name = "A", StandardCost = new Money(100m, "IDR") };
        await repository.AddItemAsync(item, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        var existsForCreate = await repository.SkuExistsAsync("SKU-01", null, CancellationToken.None);
        var existsForUpdate = await repository.SkuExistsAsync("SKU-01", item.Id, CancellationToken.None);

        Assert.True(existsForCreate);
        Assert.False(existsForUpdate);
    }

    [Fact]
    public async Task GetRecentPurchasesAsync_Returns_Ordered_Results()
    {
        await using var dbContext = CreateDbContext();
        var repository = new EfInventoryRepository(dbContext);
        var item = new Item { Sku = "SKU-02", Name = "Item", StandardCost = new Money(50m, "IDR") };
        await repository.AddItemAsync(item, CancellationToken.None);

        await repository.AddPurchaseAsync(new Purchase
        {
            ItemId = item.Id,
            PurchasedAt = DateTimeOffset.UtcNow.AddDays(-2),
            Quantity = new Quantity(1),
            UnitCost = new Money(50m, "IDR")
        }, CancellationToken.None);
        await repository.AddPurchaseAsync(new Purchase
        {
            ItemId = item.Id,
            PurchasedAt = DateTimeOffset.UtcNow.AddDays(-1),
            Quantity = new Quantity(2),
            UnitCost = new Money(55m, "IDR")
        }, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        var rows = await repository.GetRecentPurchasesAsync(10, CancellationToken.None);
        Assert.Equal(2, rows.Count);
        Assert.True(rows[0].PurchasedAt >= rows[1].PurchasedAt);
    }

    private static HppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<HppDbContext>()
            .UseInMemoryDatabase($"hpp-test-db-{Guid.NewGuid()}")
            .Options;
        return new HppDbContext(options);
    }
}
