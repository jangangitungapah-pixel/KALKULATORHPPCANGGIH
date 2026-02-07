namespace Hpp.Application.DTOs;

/// <summary>
/// Scenario simulation output.
/// </summary>
public sealed record ScenarioResultDto(string ScenarioName, decimal SimulatedCost, string Currency, IReadOnlyList<string> Notes);
