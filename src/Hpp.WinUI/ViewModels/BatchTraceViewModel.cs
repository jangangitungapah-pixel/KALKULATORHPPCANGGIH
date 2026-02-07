using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hpp.Application.Interfaces;

namespace Hpp.WinUI.ViewModels;

public sealed partial class BatchTraceViewModel : ObservableObject
{
    private readonly IInventoryRepository _repository;

    public BatchTraceViewModel(IInventoryRepository repository)
    {
        _repository = repository;
        BatchRows = new ObservableCollection<BatchTraceRowViewModel>();
        StatusMessage = "Ready.";
        AgingFilterDays = 30;
    }

    public ObservableCollection<BatchTraceRowViewModel> BatchRows { get; }

    [ObservableProperty]
    private int _agingFilterDays;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage;

    [ObservableProperty]
    private int _staleBatches;

    [ObservableProperty]
    private decimal _staleBatchValue;

    [RelayCommand]
    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        try
        {
            var items = await _repository.GetItemsAsync(cancellationToken);
            var threshold = DateTimeOffset.UtcNow.AddDays(-Math.Max(1, AgingFilterDays));

            BatchRows.Clear();
            foreach (var item in items)
            {
                foreach (var lot in item.Lots.OrderBy(l => l.ReceivedAt))
                {
                    var ageDays = (DateTimeOffset.UtcNow - lot.ReceivedAt).Days;
                    var isStale = lot.ReceivedAt < threshold;
                    BatchRows.Add(new BatchTraceRowViewModel(
                        item.Sku,
                        item.Name,
                        lot.ReceivedAt.ToString("yyyy-MM-dd"),
                        ageDays,
                        lot.QuantityOnHand.Value,
                        lot.UnitCost.Amount,
                        Math.Round(lot.QuantityOnHand.Value * lot.UnitCost.Amount, 2),
                        isStale));
                }
            }

            StaleBatches = BatchRows.Count(row => row.IsStale);
            StaleBatchValue = BatchRows.Where(row => row.IsStale).Sum(row => row.EstimatedValue);
            StatusMessage = $"Loaded {BatchRows.Count} lots.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Batch trace failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public sealed record BatchTraceRowViewModel(
    string Sku,
    string Name,
    string ReceivedAt,
    int AgeDays,
    decimal Quantity,
    decimal UnitCost,
    decimal EstimatedValue,
    bool IsStale);
