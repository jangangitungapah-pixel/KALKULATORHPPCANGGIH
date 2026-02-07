using Hpp.Application.DTOs;

namespace Hpp.Application.Interfaces;

/// <summary>
/// Parses invoice documents into purchases.
/// </summary>
public interface IInvoiceParser
{
    Task<IReadOnlyList<PurchaseDto>> ParseAsync(byte[] content, CancellationToken cancellationToken);
}
