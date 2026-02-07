using Hpp.Application.DTOs;
using Hpp.Application.Services;
using Hpp.Domain.Entities;
using Hpp.Domain.ValueObjects;
using Hpp.Infrastructure;
using Hpp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Hpp.UnitTests;

public class ScenarioSimulatorServiceTests
{
    [Fact]
    public async Task SimulateAsync_WithRichRequest_ReturnsForecastAndSensitivity()
    {
        await using var dbContext = CreateDbContext();
        var repository = new EfInventoryRepository(dbContext);

        var item = new Item
        {
            Sku = "SKU-100",
            Name = "Test Item",
            StandardCost = new Money(100m, "IDR"),
            Lots = new List<InventoryLot>
            {
                new() { QuantityOnHand = new Quantity(10), UnitCost = new Money(90m, "IDR"), ReceivedAt = DateTimeOffset.UtcNow.AddDays(-5) },
                new() { QuantityOnHand = new Quantity(12), UnitCost = new Money(110m, "IDR"), ReceivedAt = DateTimeOffset.UtcNow.AddDays(-2) }
            }
        };

        await repository.AddItemAsync(item, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        var service = new ScenarioSimulatorService(repository);
        var request = ScenarioSimulationRequestDto.Default("Stress", 7m, "IDR") with
        {
            HorizonMonths = 12,
            RiskProfile = "Aggressive",
            Drivers = new[]
            {
                new ScenarioCostDriverDto("Fuel", "Logistics", 30m, 20m, 1.05m)
            }
        };

        var result = await service.SimulateAsync(request, CancellationToken.None);

        Assert.Equal("Stress", result.ScenarioName);
        Assert.Equal(12, result.HorizonMonths);
        Assert.NotEmpty(result.Notes);
        Assert.NotNull(result.Forecast);
        Assert.NotNull(result.Sensitivity);
        Assert.Equal(12, result.Forecast!.Count);
        Assert.Equal(7, result.Sensitivity!.Count);
    }

    [Fact]
    public async Task SimulateAsync_LegacyOverload_UsesDefaults()
    {
        await using var dbContext = CreateDbContext();
        var repository = new EfInventoryRepository(dbContext);
        var service = new ScenarioSimulatorService(repository);

        var result = await service.SimulateAsync("Legacy", 5m, CancellationToken.None);

        Assert.Equal("Legacy", result.ScenarioName);
        Assert.Equal("Balanced", result.RiskProfile);
    }

    private static HppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<HppDbContext>()
            .UseInMemoryDatabase($"hpp-sim-test-db-{Guid.NewGuid()}")
            .Options;
        return new HppDbContext(options);
    }
}
