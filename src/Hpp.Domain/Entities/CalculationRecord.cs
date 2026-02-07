using System.ComponentModel.DataAnnotations;
using Hpp.Domain.ValueObjects;

namespace Hpp.Domain.Entities;

/// <summary>
/// Captures HPP calculation output.
/// </summary>
public class CalculationRecord
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset CalculatedAt { get; set; }

    [Required]
    public Guid ItemId { get; set; }

    public Item? Item { get; set; }

    public Money ResultCost { get; set; } = Money.Zero("IDR");

    [MaxLength(128)]
    public string Strategy { get; set; } = string.Empty;
}
