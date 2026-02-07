using Hpp.Application.DTOs;
using Hpp.Application.Interfaces;
using Hpp.Application.ViewModels;
using Xunit;

namespace Hpp.UnitTests;

public class ViewModelTests
{
    [Fact]
    public async Task ScenarioPreviewViewModel_Should_Run_Command()
    {
        var simulator = new FakeScenarioSimulator();
        var viewModel = new ScenarioPreviewViewModel(simulator)
        {
            ScenarioName = "Test",
            AdjustmentPercent = 10m
        };

        await viewModel.RunSimulationCommand.ExecuteAsync(null);

        Assert.NotNull(viewModel.LastResult);
        Assert.Equal("Test", viewModel.LastResult?.ScenarioName);
    }

    private sealed class FakeScenarioSimulator : IScenarioSimulator
    {
        public Task<ScenarioResultDto> SimulateAsync(string scenarioName, decimal priceAdjustmentPercent, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ScenarioResultDto(scenarioName, 123m, "IDR", new List<string>()));
        }
    }
}
