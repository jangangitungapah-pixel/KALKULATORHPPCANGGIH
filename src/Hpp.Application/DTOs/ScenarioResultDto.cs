namespace Hpp.Application.DTOs;

/// <summary>
/// Scenario simulation output.
/// </summary>
public sealed record ScenarioResultDto(
    string ScenarioName,
    decimal SimulatedCost,
    string Currency,
    IReadOnlyList<string> Notes,
    decimal BaselineCost = 0m,
    decimal BaselineCogs = 0m,
    decimal SimulatedCogs = 0m,
    decimal DeltaCost = 0m,
    decimal DeltaPercent = 0m,
    decimal SimulatedMargin = 0m,
    string RiskProfile = "Balanced",
    int HorizonMonths = 6,
    IReadOnlyList<ScenarioSeriesPointDto>? Forecast = null,
    IReadOnlyList<ScenarioSeriesPointDto>? Sensitivity = null);

public sealed record ScenarioSeriesPointDto(string Label, decimal Value, decimal SecondaryValue = 0m);
