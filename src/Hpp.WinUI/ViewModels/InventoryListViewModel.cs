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
    private readonly List<ItemRowViewModel> _allItems;

    public InventoryListViewModel(IInventoryRepository repository, IReportService reportService)
    {
        _repository = repository;
        _reportService = reportService;
        Items = new ObservableCollection<ItemRowViewModel>();
        _allItems = new List<ItemRowViewModel>();
        Categories = new ObservableCollection<string>();
        SortOptions = new ObservableCollection<string>
        {
            "SKU",
            "Name",
            "Category",
            "Standard Cost"
        };
        NewSku = string.Empty;
        NewName = string.Empty;
        NewCategory = string.Empty;
        NewStandardCost = 0m;
        EditSku = string.Empty;
        EditName = string.Empty;
        EditCategory = string.Empty;
        EditStandardCost = 0m;
        FilterText = string.Empty;
        FilterCategory = "All";
        SortBy = "SKU";
        StatusMessage = "Ready.";
        ExportPreview = string.Empty;
    }

    public ObservableCollection<ItemRowViewModel> Items { get; }

    public ObservableCollection<string> Categories { get; }

    public ObservableCollection<string> SortOptions { get; }

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
    private string _filterText;

    [ObservableProperty]
    private string _filterCategory;

    [ObservableProperty]
    private decimal? _minCost;

    [ObservableProperty]
    private decimal? _maxCost;

    [ObservableProperty]
    private string _sortBy;

    [ObservableProperty]
    private bool _sortDescending;

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

    [ObservableProperty]
    private int _totalItems;

    [ObservableProperty]
    private decimal _totalInventoryValue;

    [ObservableProperty]
    private decimal _averageCost;

    [ObservableProperty]
    private string _lastUpdated = "Never";

    [ObservableProperty]
    private string _exportPreview;

    partial void OnFilterTextChanged(string value) => ApplyFilters();
    partial void OnFilterCategoryChanged(string value) => ApplyFilters();
    partial void OnMinCostChanged(decimal? value) => ApplyFilters();
    partial void OnMaxCostChanged(decimal? value) => ApplyFilters();
    partial void OnSortByChanged(string value) => ApplyFilters();
    partial void OnSortDescendingChanged(bool value) => ApplyFilters();

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
            _allItems.Clear();
            foreach (var item in items)
            {
                _allItems.Add(new ItemRowViewModel(item.Id, item.Sku, item.Name, item.Category, item.StandardCost.Amount));
            }

            Categories.Clear();
            Categories.Add("All");
            foreach (var category in _allItems
                         .Select(item => item.Category)
                         .Where(category => !string.IsNullOrWhiteSpace(category))
                         .Distinct(StringComparer.OrdinalIgnoreCase)
                         .OrderBy(category => category, StringComparer.OrdinalIgnoreCase))
            {
                Categories.Add(category);
            }

            ApplyFilters();
            LastUpdated = DateTimeOffset.Now.ToString("g");
            StatusMessage = $"Loaded {Items.Count} items.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load items: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CreateItemAsync(CancellationToken cancellationToken)
    {
        if (!ValidateCreateInput(out var error))
        {
            StatusMessage = error;
            return;
        }

        IsBusy = true;
        try
        {
            var normalizedSku = NewSku.Trim().ToUpperInvariant();
            if (await _repository.SkuExistsAsync(normalizedSku, excludingItemId: null, cancellationToken))
            {
                StatusMessage = $"SKU {normalizedSku} already exists.";
                return;
            }

            var item = new Item
            {
                Sku = normalizedSku,
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
        catch (Exception ex)
        {
            StatusMessage = $"Failed to create item: {ex.Message}";
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

        if (!ValidateEditInput(out var error))
        {
            StatusMessage = error;
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

            var normalizedSku = EditSku.Trim().ToUpperInvariant();
            if (await _repository.SkuExistsAsync(normalizedSku, excludingItemId: item.Id, cancellationToken))
            {
                StatusMessage = $"SKU {normalizedSku} already exists.";
                return;
            }

            item.Sku = normalizedSku;
            item.Name = EditName.Trim();
            item.Category = EditCategory.Trim();
            item.StandardCost = new Money(EditStandardCost, AppConstants.DefaultCurrency);

            await _repository.SaveChangesAsync(cancellationToken);
            await LoadAsync(cancellationToken);
            StatusMessage = $"Updated item {item.Sku}.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to update item: {ex.Message}";
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
        catch (Exception ex)
        {
            StatusMessage = $"Failed to delete item: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DuplicateSelectedAsync(CancellationToken cancellationToken)
    {
        if (SelectedItem is null)
        {
            StatusMessage = "Select an item to duplicate.";
            return;
        }

        IsBusy = true;
        try
        {
            var baseSku = $"{SelectedItem.Sku}-COPY";
            var suffix = 1;
            var sku = baseSku;
            while (await _repository.SkuExistsAsync(sku, excludingItemId: null, cancellationToken))
            {
                suffix++;
                sku = $"{baseSku}-{suffix}";
            }

            var clone = new Item
            {
                Sku = sku,
                Name = $"{SelectedItem.Name} (Copy)",
                Category = SelectedItem.Category,
                StandardCost = new Money(SelectedItem.StandardCost, AppConstants.DefaultCurrency)
            };

            await _repository.AddItemAsync(clone, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            await LoadAsync(cancellationToken);
            StatusMessage = $"Duplicated item {SelectedItem.Sku}.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to duplicate item: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ApplyFilters()
    {
        var filtered = _allItems.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(FilterText))
        {
            var text = FilterText.Trim();
            filtered = filtered.Where(item =>
                item.Sku.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                item.Name.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                item.Category.Contains(text, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(FilterCategory) && !string.Equals(FilterCategory, "All", StringComparison.OrdinalIgnoreCase))
        {
            filtered = filtered.Where(item => string.Equals(item.Category, FilterCategory, StringComparison.OrdinalIgnoreCase));
        }

        if (MinCost.HasValue)
        {
            filtered = filtered.Where(item => item.StandardCost >= MinCost.Value);
        }

        if (MaxCost.HasValue)
        {
            filtered = filtered.Where(item => item.StandardCost <= MaxCost.Value);
        }

        filtered = SortBy switch
        {
            "Name" => SortDescending ? filtered.OrderByDescending(item => item.Name) : filtered.OrderBy(item => item.Name),
            "Category" => SortDescending ? filtered.OrderByDescending(item => item.Category) : filtered.OrderBy(item => item.Category),
            "Standard Cost" => SortDescending ? filtered.OrderByDescending(item => item.StandardCost) : filtered.OrderBy(item => item.StandardCost),
            _ => SortDescending ? filtered.OrderByDescending(item => item.Sku) : filtered.OrderBy(item => item.Sku)
        };

        Items.Clear();
        foreach (var item in filtered)
        {
            Items.Add(item);
        }

        RecomputeSummary();
    }

    [RelayCommand]
    private void ClearFilters()
    {
        FilterText = string.Empty;
        FilterCategory = "All";
        MinCost = null;
        MaxCost = null;
        SortBy = "SKU";
        SortDescending = false;
    }

    private void RecomputeSummary()
    {
        TotalItems = Items.Count;
        TotalInventoryValue = Items.Sum(item => item.StandardCost);
        AverageCost = Items.Count == 0 ? 0m : Items.Average(item => item.StandardCost);
    }

    [RelayCommand]
    private async Task ExportCsvAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        try
        {
            var csv = await _reportService.ExportInventoryToCsvAsync(cancellationToken);
            ExportPreview = BuildPreview(csv);
            StatusMessage = $"CSV exported ({csv.Length} chars).";
        }
        catch (Exception ex)
        {
            StatusMessage = $"CSV export failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ExportExcelAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        try
        {
            var excel = await _reportService.ExportInventoryToExcelAsync(cancellationToken);
            ExportPreview = BuildPreview(excel);
            StatusMessage = $"Excel XML exported ({excel.Length} chars).";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Excel export failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool ValidateCreateInput(out string error)
    {
        if (string.IsNullOrWhiteSpace(NewSku))
        {
            error = "SKU is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(NewName))
        {
            error = "Name is required.";
            return false;
        }

        if (NewStandardCost < 0m)
        {
            error = "Standard cost cannot be negative.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private bool ValidateEditInput(out string error)
    {
        if (string.IsNullOrWhiteSpace(EditSku))
        {
            error = "SKU is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(EditName))
        {
            error = "Name is required.";
            return false;
        }

        if (EditStandardCost < 0m)
        {
            error = "Standard cost cannot be negative.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private static string BuildPreview(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return "No data.";
        }

        var rows = data.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        return string.Join(Environment.NewLine, rows.Take(8));
    }
}

public sealed record ItemRowViewModel(Guid Id, string Sku, string Name, string Category, decimal StandardCost);
