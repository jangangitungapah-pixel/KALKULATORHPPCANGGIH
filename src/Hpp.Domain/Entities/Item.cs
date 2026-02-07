using System.ComponentModel.DataAnnotations;
using Hpp.Domain.ValueObjects;

namespace Hpp.Domain.Entities;

/// <summary>
/// Represents a stock keeping unit.
/// </summary>
public class Item
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(64)]
    public string Sku { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(128)]
    public string Category { get; set; } = string.Empty;

    public Money StandardCost { get; set; } = Money.Zero("IDR");

    public ICollection<InventoryLot> Lots { get; set; } = new List<InventoryLot>();
}
