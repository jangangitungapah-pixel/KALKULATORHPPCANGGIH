using Hpp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hpp.Infrastructure.Data;

/// <summary>
/// Seeds the database with sample data.
/// </summary>
public sealed class SeedService : ISeedService
{
    private readonly HppDbContext _dbContext;

    public SeedService(HppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.Items.AnyAsync(cancellationToken))
        {
            return;
        }

        var items = SeedData.BuildItems();
        await _dbContext.Items.AddRangeAsync(items, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
