namespace Hpp.Application.DTOs;

/// <summary>
/// Purchase data transfer object.
/// </summary>
public sealed record PurchaseDto(Guid Id, Guid ItemId, decimal Quantity, decimal UnitCost, string Currency, DateTimeOffset PurchasedAt);
