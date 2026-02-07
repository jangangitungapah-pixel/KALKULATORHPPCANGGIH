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
        var options = new DbContextOptionsBuilder<HppDbContext>()
            .UseInMemoryDatabase("hpp-test-db")
            .Options;

        await using var dbContext = new HppDbContext(options);
        var repository = new EfInventoryRepository(dbContext);

        var item = new Item { Sku = "SKU-TEST", Name = "Test", StandardCost = new Money(100m, "IDR") };
        await repository.AddItemAsync(item, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        var loaded = await repository.GetItemAsync(item.Id, CancellationToken.None);
        Assert.NotNull(loaded);
    }
}
