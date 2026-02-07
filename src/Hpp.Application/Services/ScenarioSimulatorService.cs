using Hpp.Application.DTOs;
using Hpp.Application.Interfaces;

namespace Hpp.Application.Services;

/// <summary>
/// Executes what-if simulations in memory.
/// </summary>
public sealed class ScenarioSimulatorService : IScenarioSimulator
{
    private static readonly decimal[] SensitivitySteps = [-15m, -10m, -5m, 0m, 5m, 10m, 15m];
    private readonly IInventoryRepository _repository;

    public ScenarioSimulatorService(IInventoryRepository repository)
    {
        _repository = repository;
    }

    public Task<ScenarioResultDto> SimulateAsync(string scenarioName, decimal priceAdjustmentPercent, CancellationToken cancellationToken)
    {
        var request = ScenarioSimulationRequestDto.Default(scenarioName, priceAdjustmentPercent, "IDR");
        return SimulateAsync(request, cancellationToken);
    }

    public async Task<ScenarioResultDto> SimulateAsync(ScenarioSimulationRequestDto request, CancellationToken cancellationToken)
    {
        var normalized = NormalizeRequest(request);
        var items = await _repository.GetItemsAsync(cancellationToken);
        var currency = ResolveCurrency(normalized, items.Select(x => x.StandardCost.Currency).ToArray());

        var baselineCost = ComputeBaselineCost(items, normalized.Currency);
        var baselineCogs = ApplyOperationalOverhead(baselineCost, normalized);

        var driverImpact = ComputeDriverImpact(normalized);
        var simulatedCost = ApplyScenarioAdjustment(baselineCost + driverImpact, normalized);
        var simulatedCogs = ApplyOperationalOverhead(simulatedCost, normalized);

        var deltaCost = simulatedCost - baselineCost;
        var deltaPercent = baselineCost == 0m
            ? 0m
            : decimal.Round(deltaCost / baselineCost * 100m, 2, MidpointRounding.AwayFromZero);
        var margin = simulatedCogs == 0m
            ? 0m
            : decimal.Round((simulatedCogs - simulatedCost) / simulatedCogs * 100m, 2, MidpointRounding.AwayFromZero);

        var forecast = BuildForecast(simulatedCost, normalized);
        var sensitivity = BuildSensitivity(baselineCost, normalized);
        var notes = BuildNotes(normalized, items.Count, driverImpact, baselineCost, deltaPercent);

        return new ScenarioResultDto(
            ScenarioName: normalized.ScenarioName,
            SimulatedCost: decimal.Round(simulatedCost, 2, MidpointRounding.AwayFromZero),
            Currency: currency,
            Notes: notes,
            BaselineCost: decimal.Round(baselineCost, 2, MidpointRounding.AwayFromZero),
            BaselineCogs: decimal.Round(baselineCogs, 2, MidpointRounding.AwayFromZero),
            SimulatedCogs: decimal.Round(simulatedCogs, 2, MidpointRounding.AwayFromZero),
            DeltaCost: decimal.Round(deltaCost, 2, MidpointRounding.AwayFromZero),
            DeltaPercent: deltaPercent,
            SimulatedMargin: margin,
            RiskProfile: normalized.RiskProfile,
            HorizonMonths: normalized.HorizonMonths,
            Forecast: forecast,
            Sensitivity: sensitivity);
    }

    private static ScenarioSimulationRequestDto NormalizeRequest(ScenarioSimulationRequestDto request)
    {
        var scenarioName = string.IsNullOrWhiteSpace(request.ScenarioName) ? "Scenario" : request.ScenarioName.Trim();
        var strategy = string.IsNullOrWhiteSpace(request.Strategy) ? "Weighted Average" : request.Strategy.Trim();
        var currency = string.IsNullOrWhiteSpace(request.Currency) ? "IDR" : request.Currency.Trim().ToUpperInvariant();
        var overheadFactor = request.OverheadFactor <= 0m ? 1m : request.OverheadFactor;
        var confidence = Math.Clamp(request.ConfidenceTarget, 0.4m, 0.99m);
        var waste = Math.Clamp(request.WastePercent, 0m, 100m);
        var horizon = request.HorizonMonths switch
        {
            <= 3 => 3,
            <= 6 => 6,
            _ => 12
        };
        var risk = request.RiskProfile switch
        {
            "Conservative" => "Conservative",
            "Aggressive" => "Aggressive",
            _ => "Balanced"
        };
        var drivers = request.Drivers
            .Where(x => x.Quantity > 0m && x.UnitCost >= 0m && x.ImpactWeight > 0m)
            .Select(x => new ScenarioCostDriverDto(
                Name: string.IsNullOrWhiteSpace(x.Name) ? "Driver" : x.Name.Trim(),
                Category: string.IsNullOrWhiteSpace(x.Category) ? "General" : x.Category.Trim(),
                UnitCost: x.UnitCost,
                Quantity: x.Quantity,
                ImpactWeight: x.ImpactWeight))
            .ToArray();

        return request with
        {
            ScenarioName = scenarioName,
            Strategy = strategy,
            Currency = currency,
            OverheadFactor = overheadFactor,
            ConfidenceTarget = confidence,
            WastePercent = waste,
            HorizonMonths = horizon,
            RiskProfile = risk,
            Drivers = drivers
        };
    }

    private static decimal ComputeBaselineCost(IReadOnlyList<Domain.Entities.Item> items, string fallbackCurrency)
    {
        if (items.Count == 0)
        {
            return 0m;
        }

        var perItemCosts = new List<decimal>(items.Count);
        foreach (var item in items)
        {
            var lots = item.Lots.Where(lot => lot.QuantityOnHand.Value > 0m).ToList();
            if (lots.Count == 0)
            {
                perItemCosts.Add(item.StandardCost.Amount);
                continue;
            }

            var weightedQuantity = lots.Sum(lot => lot.QuantityOnHand.Value);
            if (weightedQuantity <= 0m)
            {
                perItemCosts.Add(item.StandardCost.Amount);
                continue;
            }

            var weightedCost = lots.Sum(lot => lot.QuantityOnHand.Value * lot.UnitCost.Amount) / weightedQuantity;
            perItemCosts.Add(weightedCost);
        }

        _ = fallbackCurrency;
        return perItemCosts.Average();
    }

    private static decimal ApplyScenarioAdjustment(decimal amount, ScenarioSimulationRequestDto request)
    {
        var riskMultiplier = request.RiskProfile switch
        {
            "Conservative" => 0.985m,
            "Aggressive" => 1.045m,
            _ => 1.01m
        };

        var confidenceMultiplier = request.ConfidenceTarget < 0.7m
            ? 1.01m
            : request.ConfidenceTarget > 0.9m
                ? 0.995m
                : 1m;

        var adjusted = amount;
        adjusted *= 1m + request.PriceAdjustmentPercent / 100m;
        adjusted *= 1m + request.VolumeChangePercent / 100m;
        adjusted *= riskMultiplier;
        adjusted *= confidenceMultiplier;
        return adjusted;
    }

    private static decimal ApplyOperationalOverhead(decimal amount, ScenarioSimulationRequestDto request)
    {
        var adjusted = amount * request.OverheadFactor;

        if (request.IncludeOverhead)
        {
            adjusted *= 1.02m;
        }

        if (request.IncludeFreight)
        {
            adjusted *= 1.01m;
        }

        if (request.IncludeOvertime)
        {
            adjusted *= 1.015m;
        }

        if (request.IncludeTax)
        {
            adjusted *= 1.1m;
        }

        adjusted *= 1m + request.WastePercent / 100m;
        return adjusted;
    }

    private static decimal ComputeDriverImpact(ScenarioSimulationRequestDto request)
    {
        if (request.Drivers.Count == 0)
        {
            return 0m;
        }

        return request.Drivers.Sum(driver => driver.UnitCost * driver.Quantity * driver.ImpactWeight);
    }

    private static IReadOnlyList<string> BuildNotes(
        ScenarioSimulationRequestDto request,
        int itemCount,
        decimal driverImpact,
        decimal baselineCost,
        decimal deltaPercent)
    {
        var notes = new List<string>
        {
            $"Simulation executed in-memory with {itemCount} catalog items.",
            $"Strategy: {request.Strategy}; Risk profile: {request.RiskProfile}.",
            $"Confidence target set to {request.ConfidenceTarget:P0} for a {request.HorizonMonths}-month horizon."
        };

        if (request.Drivers.Count > 0)
        {
            notes.Add($"Applied {request.Drivers.Count} cost drivers adding {driverImpact:0.##} {request.Currency}.");
        }

        if (baselineCost == 0m)
        {
            notes.Add("Baseline inventory cost is zero; verify seeded inventory data.");
        }
        else if (deltaPercent >= 10m)
        {
            notes.Add("High increase detected; procurement buffers should be reviewed.");
        }
        else if (deltaPercent <= -10m)
        {
            notes.Add("Strong downward shift detected; validate quality and supply continuity assumptions.");
        }

        return notes;
    }

    private static IReadOnlyList<ScenarioSeriesPointDto> BuildSensitivity(decimal baselineCost, ScenarioSimulationRequestDto request)
    {
        var points = new List<ScenarioSeriesPointDto>(SensitivitySteps.Length);
        foreach (var step in SensitivitySteps)
        {
            var value = ApplyScenarioAdjustment(baselineCost * (1m + step / 100m), request);
            points.Add(new ScenarioSeriesPointDto($"{step:+#;-#;0}%", decimal.Round(value, 2, MidpointRounding.AwayFromZero), step));
        }

        return points;
    }

    private static IReadOnlyList<ScenarioSeriesPointDto> BuildForecast(decimal startCost, ScenarioSimulationRequestDto request)
    {
        var monthlyDrift = request.RiskProfile switch
        {
            "Conservative" => 0.0025m,
            "Aggressive" => 0.011m,
            _ => 0.006m
        };

        var points = new List<ScenarioSeriesPointDto>(request.HorizonMonths);
        for (var month = 1; month <= request.HorizonMonths; month++)
        {
            var value = startCost * (1m + monthlyDrift * month);
            points.Add(new ScenarioSeriesPointDto($"M{month}", decimal.Round(value, 2, MidpointRounding.AwayFromZero), monthlyDrift));
        }

        return points;
    }

    private static string ResolveCurrency(ScenarioSimulationRequestDto request, IReadOnlyList<string> currencies)
    {
        if (currencies.Count == 0)
        {
            return request.Currency;
        }

        var selected = currencies
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToUpperInvariant())
            .GroupBy(x => x)
            .OrderByDescending(group => group.Count())
            .Select(group => group.Key)
            .FirstOrDefault();

        return string.IsNullOrWhiteSpace(selected) ? request.Currency : selected;
    }
}
