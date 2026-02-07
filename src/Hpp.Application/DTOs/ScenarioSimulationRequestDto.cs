namespace Hpp.Application.DTOs;

/// <summary>
/// Request payload for scenario simulation.
/// </summary>
public sealed record ScenarioSimulationRequestDto(
    string ScenarioName,
    string Strategy,
    string Currency,
    decimal PriceAdjustmentPercent,
    decimal VolumeChangePercent,
    decimal OverheadFactor,
    decimal WastePercent,
    bool IncludeOvertime,
    bool IncludeFreight,
    bool IncludeOverhead,
    bool IncludeTax,
    decimal ConfidenceTarget,
    string RiskProfile,
    int HorizonMonths,
    IReadOnlyList<ScenarioCostDriverDto> Drivers)
{
    public static ScenarioSimulationRequestDto Default(string scenarioName, decimal adjustmentPercent, string currency)
        => new(
            ScenarioName: string.IsNullOrWhiteSpace(scenarioName) ? "Scenario" : scenarioName.Trim(),
            Strategy: "Weighted Average",
            Currency: string.IsNullOrWhiteSpace(currency) ? "IDR" : currency.Trim().ToUpperInvariant(),
            PriceAdjustmentPercent: adjustmentPercent,
            VolumeChangePercent: 0m,
            OverheadFactor: 1.05m,
            WastePercent: 1.5m,
            IncludeOvertime: true,
            IncludeFreight: true,
            IncludeOverhead: true,
            IncludeTax: false,
            ConfidenceTarget: 0.85m,
            RiskProfile: "Balanced",
            HorizonMonths: 6,
            Drivers: Array.Empty<ScenarioCostDriverDto>());
}

public sealed record ScenarioCostDriverDto(
    string Name,
    string Category,
    decimal UnitCost,
    decimal Quantity,
    decimal ImpactWeight);
