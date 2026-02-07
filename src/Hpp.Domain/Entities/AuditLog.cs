using System.ComponentModel.DataAnnotations;

namespace Hpp.Domain.Entities;

/// <summary>
/// Immutable audit log entry.
/// </summary>
public class AuditLog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset Timestamp { get; set; }

    [Required]
    [MaxLength(128)]
    public string Actor { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(1024)]
    public string Details { get; set; } = string.Empty;
}
