using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    partial void OnAdjustmentPercentChanged(double value)
    {
        RecomputeSummary();
    }

    partial void OnVolumeChangePercentChanged(double value)
    {
        RecomputeSummary();
    }

    partial void OnOverheadFactorChanged(double value)
    {
        RecomputeSummary();
    }

    partial void OnWastePercentChanged(double value)
    {
        RecomputeSummary();
    }

    partial void OnIncludeOvertimeChanged(bool value)
    {
        RecomputeSummary();
    }

    partial void OnIncludeFreightChanged(bool value)
    {
        RecomputeSummary();
    }

    partial void OnIncludeOverheadChanged(bool value)
    {
        RecomputeSummary();
    }

    partial void OnIncludeTaxChanged(bool value)
    {
        RecomputeSummary();
    }

    partial void OnRoundValuesChanged(bool value)
    {
        RecomputeSummary();
    }

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
        RecomputeSummary();
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
            var result = await _simulator.SimulateAsync(ScenarioName, (decimal)AdjustmentPercent, cancellationToken);
            BaselineCost = ComputeBaselineCost();
            BaselineCogs = ComputeBaselineCogs();
            BaselineMargin = ComputeMargin(BaselineCost, BaselineCogs);

            SimulatedCost = ApplyAdjustments(BaselineCost);
            SimulatedCogs = ApplyAdjustments(BaselineCogs);
            SimulatedMargin = ComputeMargin(SimulatedCost, SimulatedCogs);

            DeltaCost = SimulatedCost - BaselineCost;
            DeltaPercent = BaselineCost == 0 ? 0m : DeltaCost / BaselineCost * 100m;

            ResultNarrative = $"{result.ScenarioName}: {result.SimulatedCost} {result.Currency}."
                + $" Strategy {SelectedStrategy}, horizon {SelectedHorizon}.";

            AppendRunHistory();
            BuildSensitivityTable();
            BuildForecast();

            LastRunAt = DateTimeOffset.Now.ToString("g");
            StatusMessage = "Simulation complete.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void RecomputeSummary()
    {
        BaselineCost = ComputeBaselineCost();
        BaselineCogs = ComputeBaselineCogs();
        BaselineMargin = ComputeMargin(BaselineCost, BaselineCogs);
        SimulatedCost = ApplyAdjustments(BaselineCost);
        SimulatedCogs = ApplyAdjustments(BaselineCogs);
        SimulatedMargin = ComputeMargin(SimulatedCost, SimulatedCogs);
        DeltaCost = SimulatedCost - BaselineCost;
        DeltaPercent = BaselineCost == 0 ? 0m : DeltaCost / BaselineCost * 100m;
    }

    [RelayCommand]
    private void GenerateForecast()
    {
        BuildForecast();
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
        BuildForecast();
        RecomputeSummary();
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
        RecomputeSummary();
    }

    private decimal ComputeBaselineCost()
    {
        var total = CostDrivers.Sum(driver => driver.TotalCost);
        return RoundValues ? Math.Round(total, 2) : total;
    }

    private decimal ComputeBaselineCogs()
    {
        var adjusted = (decimal)OverheadFactor * ComputeBaselineCost();
        if (IncludeOverhead)
        {
            adjusted *= 1.02m;
        }

        if (IncludeFreight)
        {
            adjusted *= 1.01m;
        }

        if (IncludeOvertime)
        {
            adjusted *= 1.015m;
        }

        if (IncludeTax)
        {
            adjusted *= 1.1m;
        }

        adjusted *= 1 + (decimal)(WastePercent / 100d);
        return RoundValues ? Math.Round(adjusted, 2) : adjusted;
    }

    private decimal ApplyAdjustments(decimal baseline)
    {
        var adjusted = baseline * (1 + (decimal)(AdjustmentPercent / 100d));
        adjusted *= 1 + (decimal)(VolumeChangePercent / 100d);
        return RoundValues ? Math.Round(adjusted, 2) : adjusted;
    }

    private static decimal ComputeMargin(decimal cost, decimal cogs)
    {
        if (cogs == 0m)
        {
            return 0m;
        }

        return Math.Round((cogs - cost) / cogs * 100m, 2);
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

        while (RunHistory.Count > 12)
        {
            RunHistory.RemoveAt(RunHistory.Count - 1);
        }
    }

    private void BuildSensitivityTable()
    {
        SensitivityResults.Clear();
        var baseCost = ComputeBaselineCost();
        var steps = new[] { -10, -5, 0, 5, 10 };
        foreach (var step in steps)
        {
            var adjusted = baseCost * (1 + step / 100m);
            var variance = baseCost == 0 ? 0m : (adjusted - baseCost) / baseCost * 100m;
            SensitivityResults.Add(new SensitivityRow(
                $"{step}%",
                Math.Round(adjusted, 2),
                Math.Round(variance, 2)));
        }
    }

    private void BuildForecast()
    {
        ForecastPoints.Clear();
        var months = SelectedHorizon switch
        {
            "3 Months" => 3,
            "6 Months" => 6,
            _ => 12
        };

        var riskModifier = SelectedRiskProfile switch
        {
            "Conservative" => 0.98m,
            "Aggressive" => 1.05m,
            _ => 1.01m
        };

        var baseCost = ComputeBaselineCost();
        for (var i = 1; i <= months; i++)
        {
            var factor = 1 + (decimal)(AdjustmentPercent / 100d) * (i / (decimal)months);
            var projected = baseCost * factor * riskModifier;
            ForecastPoints.Add(new ForecastPointRow(
                $"M{i}",
                Math.Round(projected, 2),
                riskModifier));
        }
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
