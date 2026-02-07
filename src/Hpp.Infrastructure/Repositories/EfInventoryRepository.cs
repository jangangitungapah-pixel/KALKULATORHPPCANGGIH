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

    public async Task AddItemAsync(Item item, CancellationToken cancellationToken)
    {
        await _dbContext.Items.AddAsync(item, cancellationToken);
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
