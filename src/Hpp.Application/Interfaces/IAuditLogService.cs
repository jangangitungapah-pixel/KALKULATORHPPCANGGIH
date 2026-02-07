using Hpp.Domain.Entities;

namespace Hpp.Application.Interfaces;

/// <summary>
/// Persists immutable audit logs.
/// </summary>
public interface IAuditLogService
{
    Task WriteAsync(AuditLog entry, CancellationToken cancellationToken);
}
