using Hpp.Domain.Entities;

namespace Hpp.Application.Interfaces;

/// <summary>
/// Inventory persistence abstraction.
/// </summary>
public interface IInventoryRepository
{
    Task<IReadOnlyList<Item>> GetItemsAsync(CancellationToken cancellationToken);
    Task<Item?> GetItemAsync(Guid id, CancellationToken cancellationToken);
    Task AddItemAsync(Item item, CancellationToken cancellationToken);
    Task DeleteItemAsync(Guid id, CancellationToken cancellationToken);
    Task AddPurchaseAsync(Purchase purchase, CancellationToken cancellationToken);
    Task AddInventoryLotAsync(InventoryLot lot, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
