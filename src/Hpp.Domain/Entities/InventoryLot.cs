using System.ComponentModel.DataAnnotations;
using Hpp.Domain.ValueObjects;

namespace Hpp.Domain.Entities;

/// <summary>
/// Represents a lot of inventory for costing.
/// </summary>
public class InventoryLot
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid ItemId { get; set; }

    public Item? Item { get; set; }

    public Quantity QuantityOnHand { get; set; }

    public Money UnitCost { get; set; } = Money.Zero("IDR");

    public DateTimeOffset ReceivedAt { get; set; }
}
