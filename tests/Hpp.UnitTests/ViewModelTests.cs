using Hpp.Application.DTOs;
using Hpp.Application.Interfaces;
using Hpp.WinUI.ViewModels;
using Xunit;

namespace Hpp.UnitTests;

public class ViewModelTests
{
    [Fact]
    public async Task ScenarioSimulatorViewModel_Should_Run_Command()
    {
        var simulator = new FakeScenarioSimulator();
        var viewModel = new ScenarioSimulatorViewModel(simulator)
        {
            ScenarioName = "Test",
            AdjustmentPercent = "10"
        };

        await viewModel.RunSimulationCommand.ExecuteAsync(null);

        Assert.Contains("Test", viewModel.ResultText);
    }

    private sealed class FakeScenarioSimulator : IScenarioSimulator
    {
        public Task<ScenarioResultDto> SimulateAsync(string scenarioName, decimal priceAdjustmentPercent, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ScenarioResultDto(scenarioName, 123m, "IDR", new List<string>()));
        }
    }
}
