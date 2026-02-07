using Hpp.Application.DTOs;
using Hpp.Application.Interfaces;

namespace Hpp.Infrastructure.Integrations;

/// <summary>
/// Ingests invoices using configured parser.
/// </summary>
public sealed class InvoiceIngestor : IInvoiceIngestor
{
    private readonly IInvoiceParser _parser;

    public InvoiceIngestor(IInvoiceParser parser)
    {
        _parser = parser;
    }

    public async Task<IReadOnlyList<PurchaseDto>> ParseAsync(Stream invoiceStream, CancellationToken cancellationToken)
    {
        using var memory = new MemoryStream();
        await invoiceStream.CopyToAsync(memory, cancellationToken);
        return await _parser.ParseAsync(memory.ToArray(), cancellationToken);
    }
}
