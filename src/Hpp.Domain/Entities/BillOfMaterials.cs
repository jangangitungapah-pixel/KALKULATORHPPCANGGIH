using System.ComponentModel.DataAnnotations;

namespace Hpp.Domain.Entities;

/// <summary>
/// Represents a bill of materials.
/// </summary>
public class BillOfMaterials
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    public ICollection<BillOfMaterialsLine> Lines { get; set; } = new List<BillOfMaterialsLine>();
}

public class BillOfMaterialsLine
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid ItemId { get; set; }

    public Item? Item { get; set; }

    public decimal Quantity { get; set; }
}
