using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hpp.Application.Interfaces;

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
    }

    public ObservableCollection<ItemRowViewModel> Items { get; }

    [RelayCommand]
    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetItemsAsync(cancellationToken);
        Items.Clear();
        foreach (var item in items)
        {
            Items.Add(new ItemRowViewModel(item.Sku, item.Name, item.Category, item.StandardCost.Amount));
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

public sealed record ItemRowViewModel(string Sku, string Name, string Category, decimal StandardCost);
