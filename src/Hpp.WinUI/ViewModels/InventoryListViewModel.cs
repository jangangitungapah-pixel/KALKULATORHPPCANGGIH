using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hpp.Application.Interfaces;
using Hpp.Domain.Entities;
using Hpp.Domain.ValueObjects;
using Hpp.Shared.Primitives;

namespace Hpp.WinUI.ViewModels;

public sealed partial class InventoryListViewModel : ObservableObject
{
    private readonly IInventoryRepository _repository;
    private readonly IReportService _reportService;

    public InventoryListViewModel(IInventoryRepository repository, IReportService reportService)
    {
        _repository = repository;
        _reportService = reportService;
        Items = new ObservableCollection<ItemRowViewModel>();
        NewSku = string.Empty;
        NewName = string.Empty;
        NewCategory = string.Empty;
        NewStandardCost = 0m;
        EditSku = string.Empty;
        EditName = string.Empty;
        EditCategory = string.Empty;
        EditStandardCost = 0m;
    }

    public ObservableCollection<ItemRowViewModel> Items { get; }

    [ObservableProperty]
    private ItemRowViewModel? _selectedItem;

    [ObservableProperty]
    private string _newSku;

    [ObservableProperty]
    private string _newName;

    [ObservableProperty]
    private string _newCategory;

    [ObservableProperty]
    private decimal _newStandardCost;

    [ObservableProperty]
    private string _editSku;

    [ObservableProperty]
    private string _editName;

    [ObservableProperty]
    private string _editCategory;

    [ObservableProperty]
    private decimal _editStandardCost;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    partial void OnSelectedItemChanged(ItemRowViewModel? value)
    {
        if (value is null)
        {
            EditSku = string.Empty;
            EditName = string.Empty;
            EditCategory = string.Empty;
            EditStandardCost = 0m;
            return;
        }

        EditSku = value.Sku;
        EditName = value.Name;
        EditCategory = value.Category;
        EditStandardCost = value.StandardCost;
    }

    [RelayCommand]
    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        try
        {
            var items = await _repository.GetItemsAsync(cancellationToken);
            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(new ItemRowViewModel(item.Id, item.Sku, item.Name, item.Category, item.StandardCost.Amount));
            }

            StatusMessage = $"Loaded {Items.Count} items.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CreateItemAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(NewSku) || string.IsNullOrWhiteSpace(NewName))
        {
            StatusMessage = "SKU and Name are required to create an item.";
            return;
        }

        IsBusy = true;
        try
        {
            var item = new Item
            {
                Sku = NewSku.Trim(),
                Name = NewName.Trim(),
                Category = NewCategory.Trim(),
                StandardCost = new Money(NewStandardCost, AppConstants.DefaultCurrency)
            };

            await _repository.AddItemAsync(item, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            await LoadAsync(cancellationToken);

            NewSku = string.Empty;
            NewName = string.Empty;
            NewCategory = string.Empty;
            NewStandardCost = 0m;
            StatusMessage = $"Created item {item.Sku}.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task UpdateSelectedAsync(CancellationToken cancellationToken)
    {
        if (SelectedItem is null)
        {
            StatusMessage = "Select an item to update.";
            return;
        }

        IsBusy = true;
        try
        {
            var item = await _repository.GetItemAsync(SelectedItem.Id, cancellationToken);
            if (item is null)
            {
                StatusMessage = "Item not found.";
                return;
            }

            item.Sku = EditSku.Trim();
            item.Name = EditName.Trim();
            item.Category = EditCategory.Trim();
            item.StandardCost = new Money(EditStandardCost, AppConstants.DefaultCurrency);

            await _repository.SaveChangesAsync(cancellationToken);
            await LoadAsync(cancellationToken);
            StatusMessage = $"Updated item {item.Sku}.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeleteSelectedAsync(CancellationToken cancellationToken)
    {
        if (SelectedItem is null)
        {
            StatusMessage = "Select an item to delete.";
            return;
        }

        IsBusy = true;
        try
        {
            await _repository.DeleteItemAsync(SelectedItem.Id, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            SelectedItem = null;
            await LoadAsync(cancellationToken);
            StatusMessage = "Item deleted.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ExportCsvAsync(CancellationToken cancellationToken)
    {
        var csv = await _reportService.ExportInventoryToCsvAsync(cancellationToken);
        // TODO: Save to file using file picker.
        _ = csv;
    }

    [RelayCommand]
    private async Task ExportExcelAsync(CancellationToken cancellationToken)
    {
        var excel = await _reportService.ExportInventoryToExcelAsync(cancellationToken);
        _ = excel;
    }
}

public sealed record ItemRowViewModel(Guid Id, string Sku, string Name, string Category, decimal StandardCost);
