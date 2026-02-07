using Hpp.Application.Interfaces;
using Hpp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hpp.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of inventory repository.
/// </summary>
public sealed class EfInventoryRepository : IInventoryRepository
{
    private readonly HppDbContext _dbContext;

    public EfInventoryRepository(HppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Item>> GetItemsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Items.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Item?> GetItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Items
            .Include(item => item.Lots)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<Item?> GetItemBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            return null;
        }

        var normalized = sku.Trim();
        return await _dbContext.Items
            .Include(item => item.Lots)
            .FirstOrDefaultAsync(item => item.Sku == normalized, cancellationToken);
    }

    public Task<bool> SkuExistsAsync(string sku, Guid? excludingItemId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            return Task.FromResult(false);
        }

        var normalized = sku.Trim();
        return _dbContext.Items.AnyAsync(item =>
            item.Sku == normalized &&
            (!excludingItemId.HasValue || item.Id != excludingItemId.Value), cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryLot>> GetLotsByItemAsync(Guid itemId, CancellationToken cancellationToken)
    {
        return await _dbContext.InventoryLots
            .AsNoTracking()
            .Where(lot => lot.ItemId == itemId)
            .OrderByDescending(lot => lot.ReceivedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Purchase>> GetRecentPurchasesAsync(int take, CancellationToken cancellationToken)
    {
        var safeTake = take <= 0 ? 20 : Math.Min(take, 500);
        return await _dbContext.Purchases
            .AsNoTracking()
            .OrderByDescending(purchase => purchase.PurchasedAt)
            .Take(safeTake)
            .ToListAsync(cancellationToken);
    }

    public async Task AddItemAsync(Item item, CancellationToken cancellationToken)
    {
        item.Sku = item.Sku.Trim();
        item.Name = item.Name.Trim();
        item.Category = item.Category.Trim();
        await _dbContext.Items.AddAsync(item, cancellationToken);
    }

    public async Task DeleteItemAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await _dbContext.Items.FindAsync([id], cancellationToken);
        if (item is null)
        {
            return;
        }

        _dbContext.Items.Remove(item);
    }

    public async Task AddPurchaseAsync(Purchase purchase, CancellationToken cancellationToken)
    {
        await _dbContext.Purchases.AddAsync(purchase, cancellationToken);
    }

    public async Task AddInventoryLotAsync(InventoryLot lot, CancellationToken cancellationToken)
    {
        await _dbContext.InventoryLots.AddAsync(lot, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
