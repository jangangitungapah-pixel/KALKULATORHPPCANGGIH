using Hpp.Application.Interfaces;
using Hpp.Domain.Entities;

namespace Hpp.Infrastructure.Services;

/// <summary>
/// Writes audit logs to the database.
/// </summary>
public sealed class AuditLogService : IAuditLogService
{
    private readonly HppDbContext _dbContext;

    public AuditLogService(HppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task WriteAsync(AuditLog entry, CancellationToken cancellationToken)
    {
        await _dbContext.AuditLogs.AddAsync(entry, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
