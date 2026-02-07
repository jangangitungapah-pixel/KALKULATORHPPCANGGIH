using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hpp.Application.DTOs;
using Hpp.Application.Interfaces;
using Hpp.Shared.Primitives;

namespace Hpp.WinUI.ViewModels;

public sealed partial class ScenarioSimulatorViewModel : ObservableObject
{
    private readonly IScenarioSimulator _simulator;
    private readonly List<CostDriverRow> _driverCache;

    public ScenarioSimulatorViewModel(IScenarioSimulator simulator)
    {
        _simulator = simulator;
        _driverCache = new List<CostDriverRow>();

        CostDrivers = new ObservableCollection<CostDriverRow>();
        RunHistory = new ObservableCollection<SimulationRunRow>();
        SensitivityResults = new ObservableCollection<SensitivityRow>();
        ForecastPoints = new ObservableCollection<ForecastPointRow>();
        AssumptionNotes = new ObservableCollection<string>();
        ScenarioOwner = "Finance Team";
        ScenarioName = "Baseline Scenario";
        ScenarioDescription = "Simulate price and volume changes across core SKUs.";
        Currency = AppConstants.DefaultCurrency;
        AdjustmentPercent = 5;
        VolumeChangePercent = 0;
        OverheadFactor = 1.05;
        WastePercent = 1.5;
        ConfidenceTarget = 0.85;
        SelectedHorizon = "6 Months";
        SelectedRiskProfile = "Balanced";
        SelectedStrategy = "Weighted Average";
        IsAdvancedMode = true;
        IncludeOvertime = true;
        IncludeFreight = true;
        IncludeOverhead = true;
        IncludeTax = false;
        RoundValues = true;
        StatusMessage = "Ready to simulate.";
        LastRunAt = "Never";

        ForecastHorizons = new ObservableCollection<string>
        {
            "3 Months",
            "6 Months",
            "12 Months"
        };

        RiskProfiles = new ObservableCollection<string>
        {
            "Conservative",
            "Balanced",
            "Aggressive"
        };

        StrategyOptions = new ObservableCollection<string>
        {
            "FIFO",
            "LIFO",
            "Weighted Average"
        };

        InitializeDefaults();
    }

    public ObservableCollection<CostDriverRow> CostDrivers { get; }

    public ObservableCollection<SimulationRunRow> RunHistory { get; }

    public ObservableCollection<SensitivityRow> SensitivityResults { get; }

    public ObservableCollection<ForecastPointRow> ForecastPoints { get; }

    public ObservableCollection<string> AssumptionNotes { get; }

    public ObservableCollection<string> ForecastHorizons { get; }

    public ObservableCollection<string> RiskProfiles { get; }

    public ObservableCollection<string> StrategyOptions { get; }

    [ObservableProperty]
    private string _scenarioName;

    [ObservableProperty]
    private string _scenarioOwner;

    [ObservableProperty]
    private string _scenarioDescription;

    [ObservableProperty]
    private string _currency;

    [ObservableProperty]
    private double _adjustmentPercent;

    [ObservableProperty]
    private double _volumeChangePercent;

    [ObservableProperty]
    private double _overheadFactor;

    [ObservableProperty]
    private double _wastePercent;

    [ObservableProperty]
    private bool _includeOvertime;

    [ObservableProperty]
    private bool _includeFreight;

    [ObservableProperty]
    private bool _includeOverhead;

    [ObservableProperty]
    private bool _includeTax;

    [ObservableProperty]
    private bool _roundValues;

    [ObservableProperty]
    private bool _isAdvancedMode;

    [ObservableProperty]
    private double _confidenceTarget;

    [ObservableProperty]
    private string _selectedHorizon;

    [ObservableProperty]
    private string _selectedRiskProfile;

    [ObservableProperty]
    private string _selectedStrategy;

    [ObservableProperty]
    private CostDriverRow? _selectedDriver;

    [ObservableProperty]
    private string _statusMessage;

    [ObservableProperty]
    private string _lastRunAt;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private decimal _baselineCost;

    [ObservableProperty]
    private decimal _baselineCogs;

    [ObservableProperty]
    private decimal _baselineMargin;

    [ObservableProperty]
    private decimal _simulatedCost;

    [ObservableProperty]
    private decimal _simulatedCogs;

    [ObservableProperty]
    private decimal _simulatedMargin;

    [ObservableProperty]
    private decimal _deltaCost;

    [ObservableProperty]
    private decimal _deltaPercent;

    [ObservableProperty]
    private string _resultNarrative = "";

    partial void OnSelectedDriverChanged(CostDriverRow? value)
    {
        if (value is null)
        {
            StatusMessage = "Select a driver to view details.";
            return;
        }

        StatusMessage = $"Selected driver: {value.Name}.";
    }

    partial void OnSelectedHorizonChanged(string value) => GenerateForecast();
    partial void OnSelectedRiskProfileChanged(string value) => GenerateForecast();
    partial void OnAdjustmentPercentChanged(double value) => _ = RecomputeSummary();
    partial void OnVolumeChangePercentChanged(double value) => _ = RecomputeSummary();
    partial void OnOverheadFactorChanged(double value) => _ = RecomputeSummary();
    partial void OnWastePercentChanged(double value) => _ = RecomputeSummary();
    partial void OnIncludeOvertimeChanged(bool value) => _ = RecomputeSummary();
    partial void OnIncludeFreightChanged(bool value) => _ = RecomputeSummary();
    partial void OnIncludeOverheadChanged(bool value) => _ = RecomputeSummary();
    partial void OnIncludeTaxChanged(bool value) => _ = RecomputeSummary();
    partial void OnConfidenceTargetChanged(double value) => _ = RecomputeSummary();

    [RelayCommand]
    private void AddDriver()
    {
        var driver = new CostDriverRow
        {
            Name = "New Driver",
            Category = "Materials",
            UnitCost = 1000,
            Quantity = 1,
            ImpactWeight = 1,
            Notes = ""
        };

        AttachDriver(driver);
        CostDrivers.Add(driver);
        SelectedDriver = driver;
        StatusMessage = "Driver added.";
    }

    [RelayCommand]
    private void RemoveSelectedDriver()
    {
        if (SelectedDriver is null)
        {
            StatusMessage = "Select a driver to remove.";
            return;
        }

        DetachDriver(SelectedDriver);
        CostDrivers.Remove(SelectedDriver);
        SelectedDriver = CostDrivers.LastOrDefault();
        _ = RecomputeSummary();
        StatusMessage = "Driver removed.";
    }

    [RelayCommand]
    private void ResetScenario()
    {
        InitializeDefaults();
        StatusMessage = "Scenario reset to defaults.";
    }

    [RelayCommand]
    private void AddAssumptionNote()
    {
        AssumptionNotes.Add($"{DateTimeOffset.Now:g}: Add assumption note.");
        StatusMessage = "Assumption note added.";
    }

    [RelayCommand]
    private void RemoveSelectedNote(string note)
    {
        if (AssumptionNotes.Remove(note))
        {
            StatusMessage = "Assumption note removed.";
        }
    }

    [RelayCommand]
    private async Task RunSimulationAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        try
        {
            var result = await _simulator.SimulateAsync(BuildSimulationRequest(), cancellationToken);
            ApplyResult(result);
            AppendRunHistory();
            LastRunAt = DateTimeOffset.Now.ToString("g");
            StatusMessage = "Simulation complete.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Simulation failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RecomputeSummary()
    {
        try
        {
            var result = await _simulator.SimulateAsync(BuildSimulationRequest(), CancellationToken.None);
            ApplyResult(result);
        }
        catch
        {
            // Keep UI responsive; status updates happen on explicit run command.
        }
    }

    [RelayCommand]
    private void GenerateForecast()
    {
        if (ForecastPoints.Count == 0)
        {
            _ = RunSimulationCommand.ExecuteAsync(null);
            return;
        }

        var months = SelectedHorizon switch
        {
            "3 Months" => 3,
            "6 Months" => 6,
            _ => 12
        };

        while (ForecastPoints.Count > months)
        {
            ForecastPoints.RemoveAt(ForecastPoints.Count - 1);
        }

        while (ForecastPoints.Count < months && ForecastPoints.Count > 0)
        {
            var last = ForecastPoints.Last();
            ForecastPoints.Add(last with
            {
                Period = $"M{ForecastPoints.Count + 1}",
                ProjectedCost = decimal.Round(last.ProjectedCost * 1.006m, 2)
            });
        }

        StatusMessage = "Forecast refreshed.";
    }

    private void InitializeDefaults()
    {
        foreach (var driver in CostDrivers.ToList())
        {
            DetachDriver(driver);
        }

        CostDrivers.Clear();
        AssumptionNotes.Clear();

        var drivers = new[]
        {
            new CostDriverRow { Name = "Steel", Category = "Materials", UnitCost = 850, Quantity = 120, ImpactWeight = 1, Notes = "Imported" },
            new CostDriverRow { Name = "Labor", Category = "Labor", UnitCost = 110, Quantity = 720, ImpactWeight = 1.1, Notes = "Shift A" },
            new CostDriverRow { Name = "Freight", Category = "Logistics", UnitCost = 60, Quantity = 180, ImpactWeight = 0.9, Notes = "Domestic" },
            new CostDriverRow { Name = "Packaging", Category = "Materials", UnitCost = 25, Quantity = 400, ImpactWeight = 1, Notes = "Eco" }
        };

        foreach (var driver in drivers)
        {
            AttachDriver(driver);
            CostDrivers.Add(driver);
        }

        AssumptionNotes.Add("Review supplier contracts for Q3.");
        AssumptionNotes.Add("Negotiate freight discounts in Q2.");
        AssumptionNotes.Add("Optimize overtime allocation across plants.");

        SelectedDriver = CostDrivers.FirstOrDefault();
        _ = RunSimulationCommand.ExecuteAsync(null);
    }

    private ScenarioSimulationRequestDto BuildSimulationRequest()
    {
        return new ScenarioSimulationRequestDto(
            ScenarioName: string.IsNullOrWhiteSpace(ScenarioName) ? "Scenario" : ScenarioName.Trim(),
            Strategy: string.IsNullOrWhiteSpace(SelectedStrategy) ? "Weighted Average" : SelectedStrategy,
            Currency: string.IsNullOrWhiteSpace(Currency) ? AppConstants.DefaultCurrency : Currency.Trim().ToUpperInvariant(),
            PriceAdjustmentPercent: (decimal)AdjustmentPercent,
            VolumeChangePercent: (decimal)VolumeChangePercent,
            OverheadFactor: (decimal)OverheadFactor,
            WastePercent: (decimal)WastePercent,
            IncludeOvertime: IncludeOvertime,
            IncludeFreight: IncludeFreight,
            IncludeOverhead: IncludeOverhead,
            IncludeTax: IncludeTax,
            ConfidenceTarget: (decimal)ConfidenceTarget,
            RiskProfile: SelectedRiskProfile,
            HorizonMonths: SelectedHorizon switch
            {
                "3 Months" => 3,
                "6 Months" => 6,
                _ => 12
            },
            Drivers: CostDrivers
                .Where(driver => driver.Quantity > 0d && driver.UnitCost >= 0d && driver.ImpactWeight > 0d)
                .Select(driver => new ScenarioCostDriverDto(
                    Name: driver.Name,
                    Category: driver.Category,
                    UnitCost: (decimal)driver.UnitCost,
                    Quantity: (decimal)driver.Quantity,
                    ImpactWeight: (decimal)driver.ImpactWeight))
                .ToArray());
    }

    private void ApplyResult(ScenarioResultDto result)
    {
        BaselineCost = MaybeRound(result.BaselineCost);
        BaselineCogs = MaybeRound(result.BaselineCogs);
        SimulatedCost = MaybeRound(result.SimulatedCost);
        SimulatedCogs = MaybeRound(result.SimulatedCogs);
        DeltaCost = MaybeRound(result.DeltaCost);
        DeltaPercent = MaybeRound(result.DeltaPercent);
        BaselineMargin = ComputeMargin(BaselineCost, BaselineCogs);
        SimulatedMargin = MaybeRound(result.SimulatedMargin);

        ResultNarrative = BuildNarrative(result);
        BuildSensitivityTable(result.Sensitivity ?? Array.Empty<ScenarioSeriesPointDto>());
        BuildForecast(result.Forecast ?? Array.Empty<ScenarioSeriesPointDto>());
    }

    private string BuildNarrative(ScenarioResultDto result)
    {
        var topNotes = string.Join(" ", result.Notes.Take(2));
        return $"{result.ScenarioName}: {result.SimulatedCost:0.##} {result.Currency}. "
               + $"Delta {result.DeltaPercent:0.##}% using {SelectedStrategy}, horizon {SelectedHorizon}. "
               + topNotes;
    }

    private decimal MaybeRound(decimal value)
        => RoundValues ? Math.Round(value, 2, MidpointRounding.AwayFromZero) : value;

    private static decimal ComputeMargin(decimal cost, decimal cogs)
    {
        if (cogs == 0m)
        {
            return 0m;
        }

        return Math.Round((cogs - cost) / cogs * 100m, 2, MidpointRounding.AwayFromZero);
    }

    private void AppendRunHistory()
    {
        RunHistory.Insert(0, new SimulationRunRow(
            ScenarioName,
            DateTimeOffset.Now,
            SelectedStrategy,
            SelectedRiskProfile,
            SelectedHorizon,
            BaselineCost,
            SimulatedCost,
            DeltaPercent,
            Currency));

        while (RunHistory.Count > 24)
        {
            RunHistory.RemoveAt(RunHistory.Count - 1);
        }
    }

    private void BuildSensitivityTable(IReadOnlyList<ScenarioSeriesPointDto> points)
    {
        SensitivityResults.Clear();
        foreach (var point in points)
        {
            SensitivityResults.Add(new SensitivityRow(
                point.Label,
                MaybeRound(point.Value),
                MaybeRound(point.SecondaryValue)));
        }
    }

    private void BuildForecast(IReadOnlyList<ScenarioSeriesPointDto> points)
    {
        ForecastPoints.Clear();
        foreach (var point in points)
        {
            ForecastPoints.Add(new ForecastPointRow(
                point.Label,
                MaybeRound(point.Value),
                MaybeRound(point.SecondaryValue)));
        }
    }

    private void AttachDriver(CostDriverRow driver)
    {
        driver.PropertyChanged += OnDriverChanged;
        _driverCache.Add(driver);
    }

    private void DetachDriver(CostDriverRow driver)
    {
        driver.PropertyChanged -= OnDriverChanged;
        _driverCache.Remove(driver);
    }

    private void OnDriverChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(CostDriverRow.Name) or nameof(CostDriverRow.Category) or nameof(CostDriverRow.Notes))
        {
            return;
        }

        _ = RecomputeSummary();
    }
}

public sealed partial class CostDriverRow : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _category = string.Empty;

    [ObservableProperty]
    private double _unitCost;

    [ObservableProperty]
    private double _quantity;

    [ObservableProperty]
    private double _impactWeight = 1;

    [ObservableProperty]
    private string _notes = string.Empty;

    [ObservableProperty]
    private bool _isLocked;

    public decimal TotalCost
        => (decimal)(UnitCost * Quantity * ImpactWeight);
}

public sealed record SimulationRunRow(
    string Name,
    DateTimeOffset RunAt,
    string Strategy,
    string RiskProfile,
    string Horizon,
    decimal BaselineCost,
    decimal SimulatedCost,
    decimal DeltaPercent,
    string Currency);

public sealed record SensitivityRow(string Step, decimal AdjustedCost, decimal VariancePercent);

public sealed record ForecastPointRow(string Period, decimal ProjectedCost, decimal RiskFactor);
