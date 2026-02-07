using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hpp.Application.Interfaces;

namespace Hpp.WinUI.ViewModels;

public sealed partial class DashboardViewModel : ObservableObject
{
    private readonly ISeedService _seedService;
    private readonly IInventoryRepository _repository;

    public DashboardViewModel(ISeedService seedService, IInventoryRepository repository)
    {
        _seedService = seedService;
        _repository = repository;
        TopSkus = new ObservableCollection<SkuViewModel>();
        TrendPoints = new ObservableCollection<KpiTrendPointViewModel>();
        StatusMessage = "Ready.";
    }

    public ObservableCollection<SkuViewModel> TopSkus { get; }

    public ObservableCollection<KpiTrendPointViewModel> TrendPoints { get; }

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage;

    [ObservableProperty]
    private int _totalItems;

    [ObservableProperty]
    private decimal _totalInventoryValue;

    [ObservableProperty]
    private decimal _averageStandardCost;

    [ObservableProperty]
    private int _lowStockLots;

    [ObservableProperty]
    private decimal _portfolioRiskScore;

    [RelayCommand]
    private async Task SeedAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        try
        {
            await _seedService.SeedAsync(cancellationToken);
            await LoadAsync(cancellationToken);
            StatusMessage = "Sample dataset ready.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task RefreshAsync(CancellationToken cancellationToken)
        => LoadAsync(cancellationToken);

    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        try
        {
            var items = await _repository.GetItemsAsync(cancellationToken);
            TopSkus.Clear();

            foreach (var item in items
                         .OrderByDescending(x => x.StandardCost.Amount)
                         .Take(8))
            {
                var onHand = item.Lots.Sum(lot => lot.QuantityOnHand.Value);
                TopSkus.Add(new SkuViewModel(
                    item.Sku,
                    item.Name,
                    item.StandardCost.Amount,
                    onHand));
            }

            TotalItems = items.Count;
            TotalInventoryValue = items.Sum(item =>
            {
                var onHand = item.Lots.Sum(lot => lot.QuantityOnHand.Value);
                var quantity = onHand <= 0m ? 1m : onHand;
                return item.StandardCost.Amount * quantity;
            });
            AverageStandardCost = items.Count == 0 ? 0m : decimal.Round(items.Average(item => item.StandardCost.Amount), 2);
            LowStockLots = items.Sum(item => item.Lots.Count(lot => lot.QuantityOnHand.Value is > 0m and < 10m));
            PortfolioRiskScore = ComputeRiskScore(items);
            BuildTrend(items);
            StatusMessage = $"Loaded {items.Count} items.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static decimal ComputeRiskScore(IReadOnlyList<Hpp.Domain.Entities.Item> items)
    {
        if (items.Count == 0)
        {
            return 0m;
        }

        var expensiveSkus = items.Count(item => item.StandardCost.Amount > 15000m);
        var sparseLots = items.Sum(item => item.Lots.Count(lot => lot.QuantityOnHand.Value is > 0m and < 10m));
        var score = (expensiveSkus * 8m) + (sparseLots * 5m);
        return Math.Min(100m, decimal.Round(score / Math.Max(1, items.Count), 2));
    }

    private void BuildTrend(IReadOnlyList<Hpp.Domain.Entities.Item> items)
    {
        TrendPoints.Clear();
        var baseAverage = items.Count == 0 ? 0m : items.Average(item => item.StandardCost.Amount);
        var factors = new[] { 0.94m, 0.97m, 0.99m, 1.00m, 1.03m, 1.05m };
        for (var i = 0; i < factors.Length; i++)
        {
            TrendPoints.Add(new KpiTrendPointViewModel($"M-{factors.Length - i}", decimal.Round(baseAverage * factors[i], 2)));
        }
    }
}

public sealed record SkuViewModel(string Sku, string Name, decimal StandardCost, decimal OnHandQuantity);

public sealed record KpiTrendPointViewModel(string Label, decimal Value);
