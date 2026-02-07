using Hpp.Application.DTOs;
using Hpp.Application.Interfaces;

namespace Hpp.Infrastructure.Integrations;

/// <summary>
/// Azure Form Recognizer adapter (placeholder).
/// </summary>
public sealed class FormRecognizerAdapter : IInvoiceParser
{
    public Task<IReadOnlyList<PurchaseDto>> ParseAsync(byte[] content, CancellationToken cancellationToken)
    {
        // TODO: Implement with Azure Form Recognizer.
        return Task.FromResult<IReadOnlyList<PurchaseDto>>(Array.Empty<PurchaseDto>());
    }
}

/// <summary>
/// Tesseract OCR fallback adapter.
/// </summary>
public sealed class TesseractAdapter : IInvoiceParser
{
    public Task<IReadOnlyList<PurchaseDto>> ParseAsync(byte[] content, CancellationToken cancellationToken)
    {
        // TODO: Implement Tesseract parsing pipeline.
        return Task.FromResult<IReadOnlyList<PurchaseDto>>(Array.Empty<PurchaseDto>());
    }
}

/// <summary>
/// Mock invoice parser for offline usage.
/// </summary>
public sealed class InvoiceParserMock : IInvoiceParser
{
    public Task<IReadOnlyList<PurchaseDto>> ParseAsync(byte[] content, CancellationToken cancellationToken)
    {
        var sample = new PurchaseDto(Guid.NewGuid(), Guid.NewGuid(), 1, 10000m, "IDR", DateTimeOffset.UtcNow);
        return Task.FromResult<IReadOnlyList<PurchaseDto>>(new List<PurchaseDto> { sample });
    }
}
