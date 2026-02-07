using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hpp.Application.Interfaces;

namespace Hpp.WinUI.ViewModels;

public sealed partial class ItemDetailViewModel : ObservableObject
{
    private readonly IInventoryRepository _repository;

    public ItemDetailViewModel(IInventoryRepository repository)
    {
        _repository = repository;
        Lots = new ObservableCollection<ItemLotRowViewModel>();
        RecentPurchases = new ObservableCollection<ItemPurchaseRowViewModel>();
        Status = "Select an item to view details.";
    }

    public ObservableCollection<ItemLotRowViewModel> Lots { get; }

    public ObservableCollection<ItemPurchaseRowViewModel> RecentPurchases { get; }

    [ObservableProperty]
    private string _lookupSku = string.Empty;

    [ObservableProperty]
    private string _status = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _itemName = "-";

    [ObservableProperty]
    private string _itemCategory = "-";

    [ObservableProperty]
    private decimal _standardCost;

    [ObservableProperty]
    private decimal _totalOnHand;

    [ObservableProperty]
    private decimal _estimatedValue;

    [RelayCommand]
    private async Task LoadDefaultAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetItemsAsync(cancellationToken);
        var first = items.FirstOrDefault();
        if (first is null)
        {
            Status = "No inventory items available.";
            return;
        }

        LookupSku = first.Sku;
        await SearchAsync(cancellationToken);
    }

    [RelayCommand]
    private async Task SearchAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(LookupSku))
        {
            Status = "Enter SKU to search.";
            return;
        }

        IsBusy = true;
        try
        {
            var item = await _repository.GetItemBySkuAsync(LookupSku.Trim().ToUpperInvariant(), cancellationToken);
            if (item is null)
            {
                Status = "Item not found.";
                return;
            }

            ItemName = item.Name;
            ItemCategory = item.Category;
            StandardCost = item.StandardCost.Amount;

            var lots = await _repository.GetLotsByItemAsync(item.Id, cancellationToken);
            Lots.Clear();
            foreach (var lot in lots)
            {
                Lots.Add(new ItemLotRowViewModel(
                    lot.ReceivedAt.ToString("yyyy-MM-dd"),
                    lot.QuantityOnHand.Value,
                    lot.UnitCost.Amount,
                    Math.Round(lot.QuantityOnHand.Value * lot.UnitCost.Amount, 2)));
            }

            TotalOnHand = lots.Sum(x => x.QuantityOnHand.Value);
            EstimatedValue = Math.Round(lots.Sum(x => x.QuantityOnHand.Value * x.UnitCost.Amount), 2);
            await LoadRecentPurchases(item.Id, cancellationToken);
            Status = $"Loaded details for {item.Sku}.";
        }
        catch (Exception ex)
        {
            Status = $"Lookup failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadRecentPurchases(Guid itemId, CancellationToken cancellationToken)
    {
        var purchases = await _repository.GetRecentPurchasesAsync(50, cancellationToken);
        RecentPurchases.Clear();
        foreach (var purchase in purchases.Where(x => x.ItemId == itemId).Take(10))
        {
            RecentPurchases.Add(new ItemPurchaseRowViewModel(
                purchase.PurchasedAt.ToString("yyyy-MM-dd HH:mm"),
                purchase.Quantity.Value,
                purchase.UnitCost.Amount,
                purchase.SupplierName));
        }
    }
}

public sealed record ItemLotRowViewModel(string ReceivedDate, decimal Quantity, decimal UnitCost, decimal TotalCost);

public sealed record ItemPurchaseRowViewModel(string PurchasedAt, decimal Quantity, decimal UnitCost, string Supplier);
