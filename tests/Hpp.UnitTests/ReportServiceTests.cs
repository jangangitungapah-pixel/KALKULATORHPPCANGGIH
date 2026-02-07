using Hpp.Domain.Entities;
using Hpp.Domain.ValueObjects;
using Hpp.Infrastructure;
using Hpp.Infrastructure.Repositories;
using Hpp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Hpp.UnitTests;

public class ReportServiceTests
{
    [Fact]
    public async Task ExportInventoryToCsvAsync_EscapesSpecialCharacters()
    {
        await using var dbContext = CreateDbContext();
        var repository = new EfInventoryRepository(dbContext);
        await repository.AddItemAsync(new Item
        {
            Sku = "SKU-CSV",
            Name = "Sugar, \"Premium\"",
            Category = "Raw",
            StandardCost = new Money(120m, "IDR")
        }, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        var service = new ReportService(repository);
        var csv = await service.ExportInventoryToCsvAsync(CancellationToken.None);

        Assert.Contains("\"Sugar, \"\"Premium\"\"\"", csv);
        Assert.Contains("SKU-CSV", csv);
    }

    [Fact]
    public async Task ExportInventoryToExcelAsync_ReturnsWorkbookXml()
    {
        await using var dbContext = CreateDbContext();
        var repository = new EfInventoryRepository(dbContext);
        await repository.AddItemAsync(new Item
        {
            Sku = "SKU-XML",
            Name = "XML Item",
            Category = "Raw",
            StandardCost = new Money(200m, "IDR")
        }, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        var service = new ReportService(repository);
        var xml = await service.ExportInventoryToExcelAsync(CancellationToken.None);

        Assert.Contains("<Workbook", xml);
        Assert.Contains("SKU-XML", xml);
    }

    private static HppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<HppDbContext>()
            .UseInMemoryDatabase($"hpp-report-test-db-{Guid.NewGuid()}")
            .Options;
        return new HppDbContext(options);
    }
}
