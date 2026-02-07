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
    }

    public ObservableCollection<SkuViewModel> TopSkus { get; }

    [RelayCommand]
    private async Task SeedAsync(CancellationToken cancellationToken)
    {
        await _seedService.SeedAsync(cancellationToken);
        await LoadTopSkusAsync(cancellationToken);
    }

    public async Task LoadTopSkusAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetItemsAsync(cancellationToken);
        TopSkus.Clear();
        foreach (var item in items)
        {
            TopSkus.Add(new SkuViewModel(item.Sku, item.Name, item.StandardCost.Amount));
        }
    }
}

public sealed record SkuViewModel(string Sku, string Name, decimal StandardCost);
