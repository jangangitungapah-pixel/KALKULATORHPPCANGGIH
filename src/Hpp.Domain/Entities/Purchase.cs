using System.ComponentModel.DataAnnotations;
using Hpp.Domain.ValueObjects;

namespace Hpp.Domain.Entities;

/// <summary>
/// Represents a purchase transaction.
/// </summary>
public class Purchase
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset PurchasedAt { get; set; }

    [Required]
    public Guid ItemId { get; set; }

    public Item? Item { get; set; }

    public Quantity Quantity { get; set; }

    public Money UnitCost { get; set; } = Money.Zero("IDR");

    [MaxLength(128)]
    public string SupplierName { get; set; } = string.Empty;
}
