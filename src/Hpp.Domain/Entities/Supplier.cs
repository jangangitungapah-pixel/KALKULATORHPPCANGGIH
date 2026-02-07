using System.ComponentModel.DataAnnotations;

namespace Hpp.Domain.Entities;

/// <summary>
/// Represents a supplier.
/// </summary>
public class Supplier
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(256)]
    public string ContactEmail { get; set; } = string.Empty;
}
