namespace Hpp.Application.DTOs;

/// <summary>
/// Item data transfer object.
/// </summary>
public sealed record ItemDto(Guid Id, string Sku, string Name, string Category, decimal StandardCost, string Currency);
