using Hpp.Application.DTOs;

namespace Hpp.Application.Interfaces;

/// <summary>
/// Parses invoices and returns purchase DTOs.
/// </summary>
public interface IInvoiceIngestor
{
    Task<IReadOnlyList<PurchaseDto>> ParseAsync(Stream invoiceStream, CancellationToken cancellationToken);
}
